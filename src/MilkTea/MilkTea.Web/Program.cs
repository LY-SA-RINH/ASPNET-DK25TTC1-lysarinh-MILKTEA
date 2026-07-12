using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controller và View
builder.Services.AddControllersWithViews();

// Đăng ký bộ nhớ tạm dùng cho Session
builder.Services.AddDistributedMemoryCache();

// Đăng ký Session để lưu giỏ hàng của khách
builder.Services.AddSession(options =>
{
    // Giỏ hàng được giữ trong 30 phút kể từ lần thao tác cuối
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Cookie Session chỉ được sử dụng bởi máy chủ
    options.Cookie.HttpOnly = true;

    // Session cần thiết cho hoạt động của giỏ hàng
    options.Cookie.IsEssential = true;
});

// Kết nối MilkTeaDbContext với SQL Server
builder.Services.AddDbContext<MilkTeaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Thêm dữ liệu mẫu vào database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<MilkTeaDbContext>();

    KhoiTaoDuLieu.KhoiTao(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Phải đặt trước UseAuthorization và MapControllerRoute
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();