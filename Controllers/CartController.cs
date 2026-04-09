using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.Models;
using System.Text.Json; // Dùng để mã hóa dữ liệu

namespace BookStoreManagement.Controllers
{
    public class CartController : Controller
    {
        private readonly Data.ApplicationDbContext _context; // Đổi lại tên Context cho đúng với file của bạn nhé

        public CartController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // HÀM LẤY GIỎ HÀNG TỪ SESSION
        private List<CartItem> GetCartItems()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart == null) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(sessionCart) ?? new List<CartItem>();
        }

        // HÀM XEM GIỎ HÀNG (Trang Index)
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // HÀM THÊM VÀO GIỎ
        // 1. SỬA LẠI HÀM NÀY: Thêm tham số quantity = 1 (mặc định là 1 nếu không truyền)
        [HttpPost] // Bạn nhớ phải có dòng này để form POST lên được nhé
        public IActionResult AddToCart(int id, int soLuong = 1) // Đổi quantity thành soLuong
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.MaSach == id);
            
            if (item != null)
            {
                // Nếu đã có trong giỏ -> Cộng dồn số lượng khách vừa chọn
                item.SoLuong += soLuong; 
            }
            else
            {
                var sach = _context.Saches.FirstOrDefault(s => s.MaSach == id);
                if (sach != null)
                {
                    cart.Add(new CartItem {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        HinhAnhDaiDien = sach.HinhAnhUrl,
                        GiaBan = sach.GiaBan,
                        SoLuong = soLuong // Lấy số lượng từ người dùng chọn
                    });
                }
            }
            
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return RedirectToAction("Index"); 
        }

        // 2. THÊM HÀM MỚI NÀY: Để JS gọi ngầm cập nhật số lượng trong giỏ
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.MaSach == id);
            
            if (item != null)
            {
                item.SoLuong = quantity;
                if (item.SoLuong <= 0) cart.Remove(item); // Nếu số lượng về 0 thì xóa luôn
                
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
                
                // Trả về dữ liệu JSON để giao diện tự cập nhật tiền
                return Json(new { success = true, newPrice = item.ThanhTien, newTotalQty = cart.Sum(x => x.SoLuong) });
            }
            return Json(new { success = false });
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.MaSach == id);
                
            if (item != null)
            {
                cart.Remove(item); // Xóa khỏi danh sách
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart)); // Lưu lại session mới
            }
                
            return RedirectToAction("Index"); // Tải lại trang giỏ hàng
        }
        
        // ==========================================================
        // PHẦN THANH TOÁN & ĐỊA CHỈ
        // ==========================================================

        // 1. HÀM HIỂN THỊ TRANG THANH TOÁN (GET)
        [HttpGet]
        public IActionResult ThanhToan()
        {
            // Kiểm tra giỏ hàng, nếu trống thì bắt quay lại trang giỏ hàng
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index"); 
            }

            // Lấy danh sách địa chỉ của khách hàng (Nếu đã đăng nhập)
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string currentUsername = User.Identity.Name!; 
                
                var danhSachDiaChi = _context.SoDiaChis
                    .Where(d => d.Username == currentUsername)
                    .OrderByDescending(d => d.LaMacDinh) // Cho địa chỉ mặc định lên đầu
                    .ToList();

                ViewBag.DanhSachDiaChi = danhSachDiaChi;
            }
            else
            {
                ViewBag.DanhSachDiaChi = null; // Khách vãng lai chưa đăng nhập
            }

            // Truyền cái giỏ hàng sang View để lỡ bạn muốn hiện tóm tắt Đơn hàng (Tổng tiền) bên góc phải
            return View(cart); 
        }

        // 2. HÀM XỬ LÝ KHI BẤM NÚT ĐẶT HÀNG (POST)
        [HttpPost]
        public IActionResult DatHang(int? MaDiaChiDuocChon, string? hoTen, string? soDienThoai, string? diaChi, string PhuongThucThanhToan, decimal tongTienTruyVien)
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index");

            int maTK = 0;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Username == User.Identity.Name);
                if (taiKhoan != null) maTK = taiKhoan.MaTK;
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }

            string tenNguoiNhan = "";
            string sdtNguoiNhan = "";
            string diaChiGiaoHang = "";

            // NẾU CHỌN ĐỊA CHỈ ĐÃ LƯU
            if (MaDiaChiDuocChon.HasValue && MaDiaChiDuocChon.Value > 0)
            {
                var diaChiDaLuu = _context.SoDiaChis.Find(MaDiaChiDuocChon.Value);
                if (diaChiDaLuu != null)
                {
                    tenNguoiNhan = $"{diaChiDaLuu.Ho} {diaChiDaLuu.Ten}".Trim();
                    sdtNguoiNhan = diaChiDaLuu.SoDienThoai;
                    diaChiGiaoHang = $"{diaChiDaLuu.DiaChiCuThe}, {diaChiDaLuu.XaPhuong}, {diaChiDaLuu.QuanHuyen}, {diaChiDaLuu.TinhThanhPho}";
                }
            }
            // NẾU NHẬP ĐỊA CHỈ MỚI TỪ FORM
            else
            {
                tenNguoiNhan = hoTen ?? "";
                sdtNguoiNhan = soDienThoai ?? "";
                diaChiGiaoHang = diaChi ?? "";
            }

            // TẠO ĐƠN HÀNG MỚI
            var donHang = new DonHang
            {
                MaTK = maTK,
                NgayDatHang = DateTime.Now,
                TongTien = tongTienTruyVien, // Lấy tổng tiền đã tính toán ưu đãi từ thẻ <input hidden> ở view
                TrangThai = 0,
                TenNguoiNhan = tenNguoiNhan,
                SoDienThoai = sdtNguoiNhan,
                DiaChiGiaoHang = diaChiGiaoHang,
                PhuongThucThanhToan = PhuongThucThanhToan
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                _context.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDH = donHang.MaDH,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = item.GiaBan
                });
            }
            _context.SaveChanges();

            HttpContext.Session.Remove("Cart"); 

            // Chuyển hướng sang trang OrderSuccess cực đẹp của bạn!
            return RedirectToAction("OrderSuccess"); 
        }

        public IActionResult OrderSuccess()
        {
            return View(); // Nó sẽ tự động gọi file OrderSuccess.cshtml của bạn
        }   
    }
}