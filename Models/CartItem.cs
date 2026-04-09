namespace BookStoreManagement.Models
{
    public class CartItem
    {
        public int MaSach { get; set; }
        public string? TenSach { get; set; }
        public string? HinhAnhDaiDien { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        
        // Tự động tính thành tiền của món đó (Giá x Số lượng)
        public decimal ThanhTien => GiaBan * SoLuong; 
    }
}