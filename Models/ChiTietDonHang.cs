using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaCTDH { get; set; }

        public int MaDH { get; set; } // Thuộc đơn hàng nào
        
        public int MaSach { get; set; } // Sách nào

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        // Móc nối dữ liệu để sau này gọi .Include() lấy ra Tên và Ảnh sách
        [ForeignKey("MaDH")]
        public virtual DonHang? DonHang { get; set; }

        [ForeignKey("MaSach")]
        public virtual Sach? Sach { get; set; }
    }
}