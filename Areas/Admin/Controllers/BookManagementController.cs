using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm thư viện phân quyền
using BookStoreManagement.Data;
using BookStoreManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using System;

namespace BookStoreManagement.Areas.Admin.Controllers // Đã sửa đúng chuẩn Admin
{
    [Area("Admin")] // Bảo vệ khu vực
    [Authorize(Roles = "Admin")] // Chặn không cho khách vào
    public class BookManagementController : Controller // Đã sửa tên Class
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Đã sửa tên Constructor
        public BookManagementController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==========================================
        // 1. DANH SÁCH SÁCH (INDEX)
        // ==========================================
        public IActionResult Index()
        {   
            // Chỉ lấy danh sách sách để hiện ra bảng
            var danhSachSach = _context.Saches.ToList();
            return View(danhSachSach);
        }

        // ==========================================
        // 2. THÊM SÁCH (CREATE)
        // ==========================================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Sach sach)
        {
            if (ModelState.IsValid)
            {
                if (sach.ImageUpload != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + sach.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        sach.ImageUpload.CopyTo(fileStream);
                    }

                    sach.HinhAnhUrl = "/images/" + uniqueFileName;
                }

                _context.Saches.Add(sach);
                _context.SaveChanges();
                
                return RedirectToAction(nameof(Index));
            }
            return View(sach);
        }

        // ==========================================
        // 3. SỬA SÁCH (EDIT)
        // ==========================================
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound(); 

            var sach = _context.Saches.Find(id);
            if (sach == null) return NotFound(); 
            
            return View(sach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm tham số IFormFile fileTaiLen vào cuối ngoặc
        public async Task<IActionResult> Edit(int id, [Bind("MaSach,TenSach,TacGia,GiaBan,HinhAnhUrl,SoLuongDaBan,MoTa,LoaiSach")] Sach sach, IFormFile? fileTaiLen)
        {
            if (id != sach.MaSach)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Nếu người dùng CÓ chọn file ảnh mới
                    if (fileTaiLen != null && fileTaiLen.Length > 0)
                    {
                        // Tạo thư mục wwwroot/images nếu chưa có
                        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(imageFolder))
                        {
                            Directory.CreateDirectory(imageFolder);
                        }

                        // Tạo tên file ngẫu nhiên để không bị trùng (Ví dụ: abc123_tenanh.jpg)
                        var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(fileTaiLen.FileName);
                        var filePath = Path.Combine(imageFolder, fileName);

                        // Lưu file vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileTaiLen.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh mới vào database
                        sach.HinhAnhUrl = "/images/" + fileName; 
                    }
                    // 2. Nếu người dùng KHÔNG chọn ảnh mới, thuộc tính sach.HinhAnhUrl 
                    // đã tự động được giữ nguyên nhờ thẻ <input type="hidden"> ở view.

                    _context.Update(sach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Kiểm tra trực tiếp trong Database xem sách còn tồn tại không
                    var sachCoTonTai = _context.Saches.Any(e => e.MaSach == sach.MaSach);
                    
                    if (!sachCoTonTai)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Sửa xong quay về danh sách
            }
            return View(sach);
        }

        // ==========================================
        // 4. XEM CHI TIẾT (DETAILS)
        // ==========================================
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var sach = _context.Saches.Find(id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // ==========================================
        // 5. XÓA SÁCH (DELETE) - Mình viết thêm cho bạn
        // ==========================================
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var sach = _context.Saches.Find(id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var sach = _context.Saches.Find(id);
            if (sach != null)
            {
                _context.Saches.Remove(sach);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}