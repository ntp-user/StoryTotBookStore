using Microsoft.EntityFrameworkCore;
using BookStoreManagement.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(); // Bật tính năng Session

// THÊM MỚI: Cài đặt hệ thống Xác thực bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn tới trang đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi bị từ chối truy cập (vd: Khách lẻn vào Admin)
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Giữ đăng nhập 30 ngày
    });


// 1. THÊM ĐOẠN NÀY ĐỂ KÍCH HOẠT SESSION (đặt trước builder.Build())
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Giỏ hàng tồn tại 60 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// THÊM MỚI: Đăng ký kết nối Database (ApplicationDbContext) - Lỗi của bạn do thiếu dòng này
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();



app.UseAuthentication(); // Bật kiểm tra "Ai đang vào?" (Xác thực)
app.UseAuthorization();  // Bật kiểm tra "Có quyền gì?" (Phân quyền)

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();



// Thêm đoạn này để nhận diện Area Admin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Đoạn cũ của bạn (dành cho khách hàng bên ngoài)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Saches}/{action=Index}/{id?}");

app.Run();

