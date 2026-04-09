using Microsoft.EntityFrameworkCore;
using BookStoreManagement.Models;

namespace BookStoreManagement.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) {}
            
        // Bảng Sách hiện tại của bạn
        public DbSet<Sach> Saches { get; set; }

        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        // THÊM MỚI Ở ĐÂY: Khai báo bảng Tài Khoản
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<SoDiaChi> SoDiaChis { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
        // Thêm hàm này để tạo dữ liệu mẫu (Seed Data) theo yêu cầu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Sach>().HasData(
                new Sach { MaSach = 1, TenSach = "Lập trình ASP.NET Core", TacGia = "Nguyễn Văn A", GiaBan = 100000, MoTa = "Cuốn sách cung cấp kiến thức toàn diện về ASP.NET Core, từ cơ bản đến nâng cao, giúp bạn xây dựng website thực tế." },
                new Sach { MaSach = 2, TenSach = "Cấu trúc dữ liệu", TacGia = "Trần Thị B", GiaBan = 120000, MoTa = "Cuốn sách cung cấp kiến thức toàn diện về ASP.NET Core, từ cơ bản đến nâng cao, giúp bạn xây dựng website thực tế." }
            );

            // Tặng bạn thêm đoạn code này để tự động tạo sẵn 2 tài khoản mẫu (1 Admin, 1 Khách) luôn!
            // Khi chạy lệnh Update-Database, nó sẽ tự chèn vào SQL cho bạn không cần nhập tay.
            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan { 
                    MaTK = 1,
                    HoTen = "NTP",
                    Username = "admin", 
                    Password = "123", 
                    Role = "Admin",
                    GioiTinh = "Nam",
                    Email = "admin@bookstore.com", // Bổ sung Email
                    SoDienThoai = "0988888888",// Bổ sung SĐT
                    NgayTao = new DateTime(2026, 3, 15)     
                },
                new TaiKhoan { 
                    MaTK = 2, 
                    HoTen = "khachhang",
                    Username = "khach", 
                    Password = "123", 
                    Role = "Customer",
                    GioiTinh = "Nam",
                    Email = "khachhang@gmail.com", // Bổ sung Email
                    SoDienThoai = "0909090909",     // Bổ sung SĐT
                    NgayTao = new DateTime(2024, 3, 15)
                }
            );
        }
    }
}