using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext để kết nối SQL Server
builder.Services.AddDbContext<MilkTeaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MilkTeaConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
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