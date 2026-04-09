using System;
using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.Models
{
    public class DonHang
    {
        [Key]
        public int MaDH { get; set; } // Mã Đơn Hàng (Khóa chính)

        // Nếu bạn có bảng TaiKhoan hoặc KhachHang thì đây là khóa ngoại
        public int MaTK { get; set; } 

        public DateTime? NgayDatHang { get; set; }

        public decimal? TongTien { get; set; }

        // Trạng thái: 0 = Chưa duyệt, 1 = Đã duyệt, 2 = Đã giao, 3 = Đã hủy...
        public int TrangThai { get; set; } 
        
        // Bạn có thể thêm Tên người nhận, SĐT, Địa chỉ giao hàng vào đây nếu cần
        public string? TenNguoiNhan { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? PhuongThucThanhToan { get; set; }

        public string? LyDoTuChoi { get; set; } // Lưu lý do admin từ chối
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}