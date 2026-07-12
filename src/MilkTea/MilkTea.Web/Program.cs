using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controller và View
builder.Services.AddControllersWithViews();

// Đăng ký bộ nhớ tạm dùng cho Session
builder.Services.AddDistributedMemoryCache();

// Đăng ký Session lưu giỏ hàng
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Kết nối MilkTeaDbContext với SQL Server
builder.Services.AddDbContext<MilkTeaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// Đăng ký ASP.NET Core Identity
builder.Services
    .AddIdentity<NguoiDung, IdentityRole>(options =>
    {
        // Quy định mật khẩu
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Email không được trùng
        options.User.RequireUniqueEmail = true;

        // Đồ án chưa yêu cầu xác nhận email
        options.SignIn.RequireConfirmedEmail = false;

        // Khóa tạm thời khi nhập sai mật khẩu nhiều lần
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<MilkTeaDbContext>()
    .AddDefaultTokenProviders();

// Liên kết đặt lại mật khẩu có hiệu lực trong 2 giờ
builder.Services.Configure<DataProtectionTokenProviderOptions>(
    options =>
    {
        options.TokenLifespan = TimeSpan.FromHours(2);
    });

// Cấu hình Cookie đăng nhập
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/TaiKhoan/DangNhap";
    options.AccessDeniedPath = "/TaiKhoan/TuChoiTruyCap";

    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Khởi tạo dữ liệu sản phẩm hiện có
using (var scope = app.Services.CreateScope())
{
    MilkTeaDbContext context = scope.ServiceProvider
        .GetRequiredService<MilkTeaDbContext>();

    KhoiTaoDuLieu.KhoiTao(context);

    await KhoiTaoTaiKhoan.KhoiTaoAsync(
        scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session dùng cho giỏ hàng
app.UseSession();

// Xác định người đang đăng nhập
app.UseAuthentication();

// Kiểm tra quyền của người dùng
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();