using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreManagement.Models; 
using System.Linq;
using System.Threading.Tasks;
using BookStoreManagement.Data;

namespace BookStoreManagement.Areas.Admin.Controllers
{
    [Area("Admin")] // Khai báo Controller này thuộc khu vực Admin
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor để tiêm (inject) Database Context vào Controller
        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách đơn hàng (Sắp xếp đơn mới nhất lên đầu)
        public async Task<IActionResult> Index()
        {
            var danhSachDonHang = await _context.DonHangs
                .OrderByDescending(d => d.NgayDatHang)
                .ToListAsync();
                
            return View(danhSachDonHang);
        }

        // 2. Action Xử lý Duyệt đơn hàng
        [HttpPost]
        public async Task<IActionResult> DuyetDonHang(int id) 
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            // 1. Chuyển trạng thái sang Đã duyệt (1) và lưu xuống DB ngay
            donHang.TrangThai = 1; 
            await _context.SaveChangesAsync(); 

            // 2. TÍNH TOÁN VÀ CẬP NHẬT LẠI HẠNG THÀNH VIÊN
            var user = await _context.TaiKhoans.FindAsync(donHang.MaTK);
            if (user != null)
            {
                // Tính tổng tiền các đơn hợp lệ (bao gồm cả đơn vừa được duyệt ở trên)
                decimal tongChiTieu = _context.DonHangs
                    .Where(d => d.MaTK == user.MaTK && (d.TrangThai == 1 || d.TrangThai == 3))
                    .Sum(d => (decimal?)d.TongTien) ?? 0;

                // Xét hạng mới
                string hangMoi = "Đồng";
                if (tongChiTieu >= 10000000) hangMoi = "Kim Cương";
                else if (tongChiTieu >= 5000000) hangMoi = "Vàng";
                else if (tongChiTieu >= 2000000) hangMoi = "Bạc";

                // Gán hạng mới và lưu lại
                user.HangThanhVien = hangMoi;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Đã duyệt thành công đơn hàng #{id} và cập nhật hạng thành viên (nếu có)!";
            return RedirectToAction(nameof(Index));
        }

        // 1. Hiển thị Trang Chi Tiết
        public async Task<IActionResult> Details(int id)
        {
            // Tìm đơn hàng, móc nối (Include) sang bảng ChiTietDonHang, móc nối tiếp sang bảng Sach để lấy Ảnh và Tên
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.Sach)
                .FirstOrDefaultAsync(d => d.MaDH == id);

            if (donHang == null) return NotFound();
            return View(donHang);
        }

        // 2. Nút Xử lý Từ chối đơn
        [HttpPost]
        public async Task<IActionResult> TuChoiDonHang(int id, string lyDoTuChoi)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                donHang.TrangThai = 2; // Giả sử quy ước: 2 là Bị Từ Chối
                donHang.LyDoTuChoi = lyDoTuChoi; // Lưu lại lý do admin gõ
                await _context.SaveChangesAsync();
                TempData["Error"] = $"Đã từ chối đơn hàng #{id}!";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        // Nếu hệ thống của bạn có dùng AntiForgeryToken thì thêm dòng dưới, không thì bỏ qua
        // [ValidateAntiForgeryToken] 
        public IActionResult Delete(int id)
        {
            // Tìm đơn hàng theo id
            var donHang = _context.DonHangs.Find(id);
            if (donHang != null)
            {
                // Xóa các chi tiết đơn hàng trước (nếu có khóa ngoại)
                // var chiTiet = _context.ChiTietDonHangs.Where(c => c.MaDH == id);
                // _context.ChiTietDonHangs.RemoveRange(chiTiet);

                _context.DonHangs.Remove(donHang);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa đơn hàng thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy đơn hàng cần xóa!";
            }

            // Xóa xong thì quay lại trang Index
            return RedirectToAction("Index"); 
        }
    }
}
