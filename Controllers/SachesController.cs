using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.Data;
using BookStoreManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks; 
using System; 

namespace BookStoreManagement.Controllers
{
    public class SachesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SachesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString, string loai)
        {
            var danhSachSach = _context.Saches.AsQueryable();
            bool dangLocDuLieu = false; 

            if (!string.IsNullOrEmpty(loai))
            {
                dangLocDuLieu = true; 
                
                if (loai == "TrongNuoc")
                {
                    danhSachSach = danhSachSach.Where(s => s.LoaiSach == "Trong Nuoc");
                    ViewBag.Keyword = "Danh mục: Sách Trong Nước";
                }
                else if (loai == "NuocNgoai")
                {
                    danhSachSach = danhSachSach.Where(s => s.LoaiSach == "Nuoc Ngoai");
                    ViewBag.Keyword = "Danh mục: Sách Nước Ngoài";
                }
                else if (loai == "TatCa")
                {
                    ViewBag.Keyword = "Tất cả danh mục sản phẩm";
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                dangLocDuLieu = true; 
                danhSachSach = danhSachSach.Where(s => s.TenSach != null && 
                                                        EF.Functions.Collate(s.TenSach, "SQL_Latin1_General_CP1_CI_AI").Contains(searchString));
                ViewBag.Keyword = $"Kết quả tìm kiếm cho: {searchString}";
            }

            if (dangLocDuLieu)
            {
                ViewBag.IsSearch = true; 
            }
            else
            {
                ViewBag.IsSearch = false;
                ViewBag.SachTrongNuoc = _context.Saches.Where(s => s.LoaiSach == "Trong Nuoc").Take(10).ToList();
                ViewBag.SachNuocNgoai = _context.Saches.Where(s => s.LoaiSach == "Nuoc Ngoai").Take(10).ToList();
            }

            return View(danhSachSach.ToList());
        }

        [HttpGet]
        public IActionResult GoiYTimKiem(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new { goiY = new List<string>(), sanPham = new List<object>() });
            }

            var query = _context.Saches.Where(s => s.TenSach != null && 
                                                EF.Functions.Collate(s.TenSach, "SQL_Latin1_General_CP1_CI_AI").Contains(term));

            var danhSachGoiY = query.Select(s => s.TenSach).Take(6).ToList();
            var danhSachSanPham = query.Select(s => new {
                id = s.MaSach,
                tenSach = s.TenSach,
                hinhAnh = s.HinhAnhUrl ?? "/images/no-image.png" 
            }).Take(6).ToList(); 

            return Json(new { goiY = danhSachGoiY, sanPham = danhSachSanPham });
        }

        public IActionResult Details(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }

            var sach = _context.Saches.Find(id);
            
            if (sach == null) 
            {
                return NotFound();
            }

            return View(sach);
        }

        // --- CẬP NHẬT: Thêm logic truyền ViewBag sang cho View Checkout.cshtml ---
       [HttpGet]
        public IActionResult Checkout()
        {
            var cartJson = HttpContext.Session.GetString("Cart"); 
            if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index"); 

            var cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            string userName = User.Identity?.Name ?? "";
            var user = _context.TaiKhoans.FirstOrDefault(t => t.Username == userName);

            decimal tamTinh = cart?.Sum(c => c.ThanhTien) ?? 0;
            int phanTramGiam = 0;
            string tenHang = "Đồng";

            if (user != null)
            {
                decimal tongChiTieu = _context.DonHangs
                    .Where(d => d.MaTK == user.MaTK && (d.TrangThai == 1 || d.TrangThai == 3))
                    .Sum(d => (decimal?)d.TongTien) ?? 0;

                if (tongChiTieu >= 10000000) { phanTramGiam = 15; tenHang = "Kim Cương"; }
                else if (tongChiTieu >= 5000000) { phanTramGiam = 10; tenHang = "Vàng"; }
                else if (tongChiTieu >= 2000000) { phanTramGiam = 5; tenHang = "Bạc"; }
                
                // ==========================================
                // ĐÃ SỬA: Lấy theo Username và đưa địa chỉ mặc định lên đầu
                // ==========================================
                ViewBag.DanhSachDiaChi = _context.SoDiaChis
                    .Where(d => d.Username == userName) 
                    .OrderByDescending(d => d.LaMacDinh) 
                    .ToList();
            }
            else
            {
                ViewBag.DanhSachDiaChi = null;
            }

            decimal soTienGiamHang = tamTinh * phanTramGiam / 100;
            decimal khuyenMaiDon = (tamTinh >= 150000) ? 10000 : 0;
            decimal tongCong = tamTinh - soTienGiamHang - khuyenMaiDon;

            ViewBag.TamTinh = tamTinh;
            ViewBag.PhanTramGiam = phanTramGiam;
            ViewBag.TenHang = tenHang;
            ViewBag.SoTienGiam = soTienGiamHang;
            ViewBag.KhuyenMaiDon = khuyenMaiDon; 
            ViewBag.TongCong = tongCong;

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteCheckout(string hoTen, string soDienThoai, string diaChi, string PhuongThucThanhToan, int? MaDiaChiDuocChon)
        {
            var cartJson = HttpContext.Session.GetString("Cart"); 
            if (string.IsNullOrEmpty(cartJson))
            {
                return RedirectToAction("Index");
            }

            var cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();

            if (cart.Count > 0)
            {
                string userName = User.Identity?.Name ?? "";
                var user = _context.TaiKhoans.FirstOrDefault(t => t.Username == userName);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // ==========================================
                // ĐÃ SỬA: Ghép Họ + Tên và ghép chuỗi Địa Chỉ
                // ==========================================
                if (MaDiaChiDuocChon.HasValue && MaDiaChiDuocChon.Value > 0)
                {
                    var diaChiCu = _context.SoDiaChis.Find(MaDiaChiDuocChon.Value);
                    if (diaChiCu != null)
                    {
                        hoTen = $"{diaChiCu.Ho} {diaChiCu.Ten}"; // Nối Họ và Tên
                        soDienThoai = diaChiCu.SoDienThoai;
                        
                        // Nối các cấp hành chính thành 1 chuỗi địa chỉ đầy đủ để lưu vào Đơn hàng
                        diaChi = $"{diaChiCu.DiaChiCuThe}, {diaChiCu.XaPhuong}, {diaChiCu.QuanHuyen}, {diaChiCu.TinhThanhPho}"; 
                    }
                }

                decimal tamTinh = cart.Sum(x => x.ThanhTien);
                int phanTramGiam = 0;
                
                var donHangs = _context.DonHangs.Where(d => d.MaTK == user.MaTK && d.TrangThai == 1).ToList();
                decimal tongChiTieu = donHangs.Sum(d => (decimal)(d.TongTien ?? 0));

                if (tongChiTieu >= 10000000) { phanTramGiam = 15; }
                else if (tongChiTieu >= 5000000) { phanTramGiam = 10; }
                else if (tongChiTieu >= 2000000) { phanTramGiam = 5; }

                decimal giamGiaHang = tamTinh * phanTramGiam / 100;
                decimal khuyenMaiDon = (tamTinh >= 150000) ? 10000 : 0;
                decimal tongCong = tamTinh - giamGiaHang - khuyenMaiDon;

                var donHang = new DonHang
                {
                    TenNguoiNhan = hoTen, // Lưu chuỗi đã ghép
                    SoDienThoai = soDienThoai,
                    DiaChiGiaoHang = diaChi, // Lưu chuỗi địa chỉ đã ghép
                    NgayDatHang = DateTime.Now,
                    TongTien = (decimal)tongCong, 
                    TrangThai = 0, 
                    MaTK = user.MaTK, 
                    PhuongThucThanhToan = PhuongThucThanhToan 
                };
                
                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync(); 

                foreach (var item in cart)
                {
                    var ctdh = new ChiTietDonHang {
                        MaDH = donHang.MaDH, 
                        MaSach = item.MaSach,
                        SoLuong = item.SoLuong,
                        DonGia = (decimal)(item.ThanhTien / item.SoLuong)
                    };
                    _context.ChiTietDonHangs.Add(ctdh);

                    var sach = await _context.Saches.FindAsync(item.MaSach);
                    if (sach != null) {
                        sach.SoLuongDaBan += item.SoLuong; 
                        _context.Saches.Update(sach);
                    }
                }
                
                await _context.SaveChangesAsync(); 
                HttpContext.Session.Remove("Cart"); 

                if (PhuongThucThanhToan == "BANKING")
                {
                    return RedirectToAction("PaymentQR", new { orderId = donHang.MaDH, amount = tongCong });
                }
                
                return RedirectToAction("DonHangCuaToi", "TaiKhoan");
            }

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult PaymentQR(int orderId, decimal amount)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            return View();
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MuaNgay(int id, int soLuong = 1)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach == null)
            {
                return NotFound();
            }

            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) 
                ? new List<CartItem>() 
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.MaSach == id);
            if (item != null)
            {
                item.SoLuong += soLuong;
            }
            else
            {
                cart.Add(new CartItem 
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    HinhAnhDaiDien = sach.HinhAnhUrl,
                    GiaBan = sach.GiaBan,
                    SoLuong = soLuong
                });
            }

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

            return RedirectToAction("Checkout"); 
        }

        public IActionResult DieuKhoanSuDung()
        {
            return View();
        }

        // Trang Chính sách đổi trả
        public IActionResult DoiTra()
        {
            return View();
        }

        // Nếu bạn đã tạo luôn file View cho 2 trang kia thì tiện tay thêm luôn nhé:
        public IActionResult BaoMatThanhToan()
        {
            return View();
        }

        public IActionResult PhuongThucVanChuyen()
        {
            return View();
        }
        
    }
}