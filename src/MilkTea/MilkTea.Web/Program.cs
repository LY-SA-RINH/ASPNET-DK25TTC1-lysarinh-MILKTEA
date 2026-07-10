using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controller và View
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();