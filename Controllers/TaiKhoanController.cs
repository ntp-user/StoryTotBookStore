using BookStoreManagement.Data;
using Microsoft.EntityFrameworkCore;
using BookStoreManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace YourProjectName.Controllers // Đổi lại thành namespace đúng của bạn nếu cần
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GET: Dùng để MỞ TRANG (Hiển thị giao diện)
        // ==========================================
        [HttpGet]
        public IActionResult HoSoCaNhan()
        {
            // Lấy tài khoản đang đăng nhập
            var username = User.Identity?.Name;
            
            // Tìm trong DB
            var khachHangDb = _context.TaiKhoans.SingleOrDefault(k => k.Username == username); 

            // Nếu chưa đăng nhập hoặc không tìm thấy -> Đuổi ra trang Login
            if (khachHangDb == null)
            {
                return RedirectToAction("Login", "Account"); // Đổi đường dẫn Login cho khớp với web của bạn
            }

            // Gửi cục data khachHangDb ra View để điền sẵn vào các ô input
            return View(khachHangDb);
        }

        // ==========================================
        // 2. POST: Dùng để LƯU DỮ LIỆU khi bấm nút
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thay vì nhận nguyên cục (TaiKhoan model), mình nhận đích danh 4 cái text từ form gửi lên
        public IActionResult HoSoCaNhan(string HoTen, string SoDienThoai, string Email, string GioiTinh) 
        {
            var username = User.Identity?.Name;
            var khachHangDb = _context.TaiKhoans.SingleOrDefault(k => k.Username == username); 

            if (khachHangDb != null)
            {
                // Gán thẳng dữ liệu mới vào DB
                khachHangDb.HoTen = HoTen;
                khachHangDb.SoDienThoai = SoDienThoai;
                khachHangDb.Email = Email;
                khachHangDb.GioiTinh = GioiTinh;

                try 
                {
                    // Lưu xuống Database
                    _context.SaveChanges();
                    
                    // Xóa hết các lỗi ngầm định của ASP.NET (nếu có) để view load lên đẹp đẽ
                    ModelState.Clear(); 
                    
                    TempData["SuccessMessage"] = "Tuyệt vời! Cập nhật thông tin thành công!"; 
                }
                catch (System.Exception ex)
                {
                    // Nếu Database lỗi gì đó (ví dụ email trùng), nó sẽ báo ở đây
                    ModelState.AddModelError("", "Lỗi khi lưu vào Database: " + ex.Message);
                }

                return View(khachHangDb);
            }

            return RedirectToAction("Login", "Account");
        }
        
        // địa chỉ
        // 1. Hàm này (đã được nâng cấp) dùng để MỞ TRANG và LẤY DANH SÁCH địa chỉ hiển thị ra màn hình
        [HttpGet]
        public IActionResult SoDiaChi()
        {
            // Kiểm tra đăng nhập
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // NÂNG CẤP: Lấy danh sách địa chỉ của khách hàng này từ Database
            // (Lưu ý: Đổi _context.SoDiaChis thành tên bảng của bạn trong ApplicationDbContext nếu khác nhé)
            var danhSachDiaChi = _context.SoDiaChis.Where(d => d.Username == username).ToList();

            // Gửi danh sách này sang giao diện (View) để in ra
            return View(danhSachDiaChi); 
        }

        // 2. Thêm luôn hàm này NGAY BÊN DƯỚI hàm trên để xử lý khi khách hàng bấm nút "Lưu địa chỉ" ở cái bảng Popup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemDiaChi(SoDiaChi model)
        {
            var username = User.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                // Gán tên tài khoản cho địa chỉ này để biết của ai
                model.Username = username;

                // --- ĐOẠN SỬA LỖI 1: NẾU CHỌN LÀM MẶC ĐỊNH THÌ TẮT CÁC MẶC ĐỊNH CŨ ĐI ---
                if (model.LaMacDinh == true)
                {
                    // Tìm tất cả các địa chỉ của user này đang là mặc định
                    var cacDiaChiCu = _context.SoDiaChis
                        .Where(d => d.Username == username && d.LaMacDinh == true)
                        .ToList();

                    // Lặp qua và tắt mặc định
                    foreach (var dc in cacDiaChiCu)
                    {
                        dc.LaMacDinh = false;
                    }
                }
                // -------------------------------------------------------------------------

                // Lưu vào Database
                _context.SoDiaChis.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Thêm địa chỉ mới thành công!";
            }

            // Lưu xong thì tải lại trang Sổ địa chỉ để nó hiện cái địa chỉ vừa thêm lên
            return RedirectToAction("SoDiaChi");
        }

        // --- 1. HÀM XÓA ĐỊA CHỈ ---
        [HttpGet]
        public IActionResult XoaDiaChi(int id)
        {
            var username = User.Identity?.Name;
            // Tìm địa chỉ cần xóa (phải đúng id và đúng của user đang đăng nhập mới cho xóa)
            var diachiDb = _context.SoDiaChis.FirstOrDefault(d => d.MaDiaChi == id && d.Username == username);
            
            if (diachiDb != null)
            {
                _context.SoDiaChis.Remove(diachiDb);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Đã xóa địa chỉ thành công!";
            }
            return RedirectToAction("SoDiaChi");
        }

        // --- 2. HÀM LƯU CHỈNH SỬA ĐỊA CHỈ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaDiaChi(SoDiaChi model)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            // Tìm địa chỉ cũ trong Database
            var diachiDb = _context.SoDiaChis.FirstOrDefault(d => d.MaDiaChi == model.MaDiaChi && d.Username == username);
            if (diachiDb != null)
            {
                // Cập nhật thông tin mới
                diachiDb.Ho = model.Ho;
                diachiDb.Ten = model.Ten;
                diachiDb.SoDienThoai = model.SoDienThoai;
                diachiDb.TinhThanhPho = model.TinhThanhPho;
                diachiDb.QuanHuyen = model.QuanHuyen;
                diachiDb.XaPhuong = model.XaPhuong;
                diachiDb.DiaChiCuThe = model.DiaChiCuThe;
                diachiDb.MaBuuDien = model.MaBuuDien;

                // Xử lý nếu chọn làm mặc định (hủy mặc định các địa chỉ khác)
                if (model.LaMacDinh)
                {
                    var cacDiaChiCu = _context.SoDiaChis.Where(d => d.Username == username && d.LaMacDinh && d.MaDiaChi != model.MaDiaChi).ToList();
                    foreach (var dc in cacDiaChiCu) dc.LaMacDinh = false;
                    diachiDb.LaMacDinh = true;
                }
                else
                {
                    diachiDb.LaMacDinh = false;
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
            }

            return RedirectToAction("SoDiaChi");
        }

        // 1. Mở trang Đổi Mật Khẩu
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // 2. Xử lý khi bấm nút Lưu Thay Đổi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var username = User.Identity?.Name;
            var taikhoanDb = _context.TaiKhoans.SingleOrDefault(t => t.Username == username);

            if (taikhoanDb != null)
            {
                // KIỂM TRA MẬT KHẨU CŨ
                // LƯU Ý: Chữ "MatKhau" bên dưới bạn nhớ đổi thành tên cột lưu mật khẩu trong DB của bạn nhé (vd: Password)
                if (taikhoanDb.Password != model.MatKhauCu)
                {
                    ModelState.AddModelError("MatKhauCu", "Mật khẩu hiện tại không chính xác.");
                    return View(model);
                }

                // Cập nhật mật khẩu mới
                taikhoanDb.Password = model.MatKhauMoi;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                ModelState.Clear(); // Xóa trắng form sau khi đổi xong
                
                return RedirectToAction("DoiMatKhau");
            }

            return RedirectToAction("Login", "Account");
        }
        public IActionResult UuDaiThanhVien()
        {
            string userName = User.Identity?.Name ?? "";
            var user = _context.TaiKhoans.FirstOrDefault(t => t.Username == userName);

            if (user == null) return RedirectToAction("Login", "Account");

            // 1. TÍNH TỔNG CHI TIÊU THỰC TẾ
            // Lưu ý: Chỉ cộng tiền những đơn hàng KHÔNG bị từ chối/hủy (TrangThai != 2)
            decimal tongChiTieu = 0;
            var donHangs = _context.DonHangs
            .Where(d => d.MaTK == user.MaTK && (d.TrangThai == 1 || d.TrangThai == 3))
            .ToList();
            if (donHangs.Any())
            {
                tongChiTieu = donHangs.Sum(d => (decimal)(d.TongTien ?? 0));
            }

            // 2. XÁC ĐỊNH HẠNG VÀ QUYỀN LỢI
            string hangThanhVien = "Thành viên Đồng";
            int phanTramGiam = 0;
            decimal canMuaThem = 2000000 - tongChiTieu;
            string hangTiepTheo = "Thành viên Bạc";
            string mauSac = "#b87333"; // Màu đồng

            if (tongChiTieu >= 10000000)
            {
                hangThanhVien = "Thành viên Kim Cương";
                phanTramGiam = 15;
                canMuaThem = 0;
                hangTiepTheo = "Cấp bậc Tối đa";
                mauSac = "#b9f2ff"; // Màu kim cương
            }
            else if (tongChiTieu >= 5000000)
            {
                hangThanhVien = "Thành viên Vàng";
                phanTramGiam = 10;
                canMuaThem = 10000000 - tongChiTieu;
                hangTiepTheo = "Thành viên Kim Cương";
                mauSac = "#ffd700"; // Màu vàng
            }
            else if (tongChiTieu >= 2000000)
            {
                hangThanhVien = "Thành viên Bạc";
                phanTramGiam = 5;
                canMuaThem = 5000000 - tongChiTieu;
                hangTiepTheo = "Thành viên Vàng";
                mauSac = "#c0c0c0"; // Màu bạc
            }

            // 3. GỬI DỮ LIỆU SANG VIEW
            ViewBag.TongChiTieu = tongChiTieu;
            ViewBag.HangThanhVien = hangThanhVien;
            ViewBag.PhanTramGiam = phanTramGiam;
            ViewBag.CanMuaThem = canMuaThem > 0 ? canMuaThem : 0;
            ViewBag.HangTiepTheo = hangTiepTheo;
            ViewBag.MauSac = mauSac;

            return View();
        }
        // 1. HÀM HIỂN THỊ DANH SÁCH ĐƠN HÀNG
        public IActionResult DonHangCuaToi()
        {
            string userName = User.Identity?.Name ?? "";
            var user = _context.TaiKhoans.FirstOrDefault(t => t.Username == userName);
            
            if (user == null) return RedirectToAction("Login");

            // SỬA CÂU TRUY VẤN NÀY LẠI:
            var danhSachDonHang = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)              // Join vào bảng Chi Tiết
                    .ThenInclude(ct => ct.Sach)   // Join tiếp vào bảng Sách để lấy Tên và Hình (Lưu ý: Tùy file Model của bạn mà chữ này có thể là .Sach hoặc .MaSachNavigation)
                .Where(d => d.MaTK == user.MaTK)
                .OrderByDescending(d => d.NgayDatHang)
                .ToList();

            return View(danhSachDonHang);
        }

        // 2. HÀM XỬ LÝ CẬP NHẬT THÔNG TIN GIAO HÀNG
        [HttpPost]
        public IActionResult CapNhatThongTinDonHang(int maDonHang, string tenNguoiNhan, string soDienThoai, string diaChi)
        {
            string userName = User.Identity?.Name ?? "";
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Account");

            // TÌM LẠI MaTK CỦA USER ĐỂ BẢO MẬT (Tránh người này sửa đơn của người khác)
            var user = _context.TaiKhoans.FirstOrDefault(t => t.Username == userName);
            if (user == null) return RedirectToAction("Login", "Account");
            int maTaiKhoan = user.MaTK;

            // Dùng MaDH và MaTK từ Database của bạn
            var donHang = _context.DonHangs.FirstOrDefault(d => d.MaDH == maDonHang && d.MaTK == maTaiKhoan);

            if (donHang != null)
            {
                if (donHang.TrangThai == 0) 
                {
                    donHang.TenNguoiNhan = tenNguoiNhan;
                    donHang.SoDienThoai = soDienThoai;
                    donHang.DiaChiGiaoHang = diaChi; // Tên cột chính xác từ ảnh của bạn

                    _context.Update(donHang);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thông tin giao hàng thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Đơn hàng đã được duyệt, không thể thay đổi thông tin lúc này!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
            }

            return RedirectToAction("DonHangCuaToi");
        }
    }
}