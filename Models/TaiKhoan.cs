using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.Models // Đổi lại tên namespace cho đúng project của bạn
{
    public class TaiKhoan
    {
        [Key] // Đánh dấu đây là Khóa chính
        public int MaTK { get; set; }

        // Bạn dán đoạn này vào dưới cột MaTK hoặc dưới cùng đều được nhé:
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        public string? HoTen { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255)]
        public string? Password { get; set; } // Thực tế đi làm sẽ phải mã hóa MD5/Bcrypt, nhưng giờ mình làm string cho dễ test nhé

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng (Ví dụ: tenban@gmail.com)")]
        [StringLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được dài quá 15 ký tự")] // Giới hạn độ dài, tránh tốn dung lượng database
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")] // Tự động kiểm tra xem có đúng là sđt không
        public string? SoDienThoai { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ quá dài (tối đa 500 ký tự)")]
        public string? DiaChi { get; set; }
        // ĐÂY LÀ CỘT PHÂN QUYỀN QUAN TRỌNG NHẤT
        [StringLength(20)]
        public string Role { get; set; } = "Customer"; // Mặc định ai tạo tài khoản cũng là Khách hàng
        public string? GioiTinh { get; set; }

        // Thêm dòng này vào Model nếu chưa có
        public DateTime? NgayTao { get; set; }

        public string? Avatar { get; set; } // Lưu đường dẫn ảnh đại diện

        public string? HangThanhVien { get; set; } = "Thành viên Mới"; // Bạc, Vàng, Kim cương...

        public int DiemTichLuy { get; set; } = 0; // Thường các trang web sẽ có điểm để xét lên hạng
    }
}