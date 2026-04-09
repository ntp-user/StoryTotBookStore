using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm dòng này để dùng NotMapped
using Microsoft.AspNetCore.Http; // Thêm dòng này để dùng IFormFile

namespace BookStoreManagement.Models
{
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }

        public string? TenSach { get; set; } 
        public string? TacGia { get; set; }
        public string? MoTa { get; set; }
        public decimal GiaBan { get; set; }
        public string? LoaiSach { get; set; }

        // Cột này vẫn lưu trong CSDL (dùng để lưu đường dẫn "/images/ten-anh.jpg")
        public string? HinhAnhUrl { get; set; } 

        // THÊM MỚI: Biến này dùng để nhận file từ máy tính. 
        // [NotMapped] nghĩa là không lưu cột này vào CSDL.
        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public int SoLuongDaBan { get; set; } = 0; // Mặc định khi mới tạo là bán được 0 cuốn
    }
}