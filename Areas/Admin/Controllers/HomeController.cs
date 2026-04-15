using BookStoreManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq; 

namespace TenProjectCuaBan.Areas.Admin.Controllers 
{
    [Area("Admin")] 
    [Authorize(Roles = "Admin")] 
    public class HomeController : Controller
    {
        // 1. Khai báo biến _context
        private readonly ApplicationDbContext _context;

        // 2. Tạo Constructor để tiêm (inject) _context vào Controller
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 3. Hàm Index chứa logic Thống kê
        public IActionResult Index()
        {
            // --- 1. THỐNG KÊ ĐƠN HÀNG ---
            ViewBag.DonChuaDuyet = _context.DonHangs.Count(d => d.TrangThai == 0);
            ViewBag.DonDaDuyet = _context.DonHangs.Count(d => d.TrangThai == 1);
            ViewBag.DonTuChoi = _context.DonHangs.Count(d => d.TrangThai == 2);

            // --- 2. THỐNG KÊ DOANH THU ---
            ViewBag.DoanhThu = _context.DonHangs
                                       .Where(d => d.TrangThai == 1)
                                       .Sum(d => (decimal?)d.TongTien) ?? 0;

            // --- 3. THỐNG KÊ KHÁCH HÀNG ---
            ViewBag.TongKhachHang = _context.TaiKhoans.Count(); 

            // --- 4. THỐNG KÊ SÁCH ---
            ViewBag.SachTrongNuoc = _context.Saches.Count(s => s.LoaiSach == "Trong Nuoc");
            ViewBag.SachNuocNgoai = _context.Saches.Count(s => s.LoaiSach == "Nuoc Ngoai");

            return View();
        }
    }
}
