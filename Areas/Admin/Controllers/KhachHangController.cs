using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreManagement.Models;
using BookStoreManagement.Data; 

namespace BookStoreManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách khách hàng
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.TaiKhoans.ToListAsync();
            return View(danhSach);
        }
        // Hàm xử lý việc xóa tài khoản
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Tìm tài khoản trong Database dựa vào ID (MaTK)
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);

            // Nếu không tìm thấy
            if (taiKhoan == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản này!";
                return RedirectToAction("Index");
            }

            // 2. TÍNH NĂNG BẢO VỆ: Không cho phép xóa tài khoản Admin để tránh "tự hủy"
            if (taiKhoan.Role == "Admin")
            {
                TempData["Error"] = "Lỗi: Không được phép xóa tài khoản Quản trị viên!";
                return RedirectToAction("Index");
            }

            // 3. Tiến hành xóa khỏi Database
            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            // Báo thành công và quay lại trang danh sách
            TempData["Success"] = $"Đã xóa thành công tài khoản: {taiKhoan.Username}";
            return RedirectToAction("Index");
        }
    }
    
    

}
