using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BookStoreManagement.Data; // Lưu ý: Chỉnh lại tên namespace nếu project của bạn tên khác
using BookStoreManagement.Models;

namespace BookStoreManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor để gọi Database
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Giao diện trang đăng nhập (GET)
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì tự động đẩy về trang chủ, không cho vào trang Login nữa
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // 2. Xử lý khi người dùng bấm nút ĐĂNG NHẬP (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Tìm tài khoản trong Database
            var user = _context.TaiKhoans.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // A. Tạo "Thẻ bài" (Claims) chứa thông tin User
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "") // Rất quan trọng: Ghi nhớ quyền Admin hay Customer
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // B. Cấu hình thời gian lưu đăng nhập (giữ đăng nhập 30 ngày)
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                };

                // C. Chính thức phát thẻ bài (Lưu Cookie xuống trình duyệt)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), 
                    authProperties);

                // D. Kiểm tra quyền để chuyển hướng
                if (user.Role == "Admin")
                {
                    // Nếu là Admin -> Cho thẳng vào trang quản trị
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    // Nếu là Khách hàng -> Cho về trang chủ mua sách
                    return RedirectToAction("Index", "Saches");
                }
            }

            // Nếu sai tài khoản hoặc mật khẩu
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        // 3. Hàm Đăng xuất
        public async Task<IActionResult> Logout()
        {
            // Xóa thẻ bài (Xóa Cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Quay về trang chủ
            return RedirectToAction("Index", "Saches");
        }

        // 4. Trang Báo Lỗi khi Khách hàng cố tình đi lạc vào Admin
        public IActionResult AccessDenied()
        {
            return View();
        }
        // 1. Hàm này dùng để hiển thị giao diện trang Đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. Hàm này dùng để xử lý dữ liệu khi khách hàng bấm nút "ĐĂNG KÝ"

        [HttpPost]
        // Thêm string XacNhanMatKhau vào tham số
        public async Task<IActionResult> Register(TaiKhoan model, string XacNhanMatKhau) 
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra 2 mật khẩu có khớp không
                if (model.Password != XacNhanMatKhau)
                {
                    ViewBag.Loi = "Mật khẩu và Nhập lại mật khẩu không trùng khớp!";
                    return View(model);
                }

                // 2. Kiểm tra tên đăng nhập đã tồn tại chưa
                var daTonTai = _context.TaiKhoans.Any(x => x.Username == model.Username);
                if (daTonTai)
                {
                    ViewBag.Loi = "Tên đăng nhập này đã có người sử dụng!";
                    return View(model);
                }
                
                // 3. Nếu mọi thứ OK thì lưu vào Database
                model.NgayTao = DateTime.Now;
                _context.TaiKhoans.Add(model);
                await _context.SaveChangesAsync();

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            
            return View(model);
        }
    }
}