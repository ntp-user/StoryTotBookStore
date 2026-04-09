using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.Models
{
    public class SoDiaChi
    {
        [Key]
        public int MaDiaChi { get; set; }
        
        public string Username { get; set; } = string.Empty; // Liên kết với tài khoản

        // Fahasa tách riêng Họ và Tên
        [Required(ErrorMessage = "Vui lòng nhập họ")]
        public string Ho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Ten { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        // Các trường địa lý chi tiết
        public string QuocGia { get; set; } = "Việt Nam"; // Mặc định là VN

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        public string TinhThanhPho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public string QuanHuyen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn Xã/Phường")]
        public string XaPhuong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
        public string DiaChiCuThe { get; set; } = string.Empty; // Số nhà, tên đường...

        public string? MaBuuDien { get; set; } // Dấu ? vì Mã bưu điện có thể không bắt buộc nhập

        public bool LaMacDinh { get; set; } = false;
    }
}