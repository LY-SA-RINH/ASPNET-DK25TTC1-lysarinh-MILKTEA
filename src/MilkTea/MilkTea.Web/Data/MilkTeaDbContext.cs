using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Models;

namespace MilkTea.Web.Data
{
    public class MilkTeaDbContext : DbContext
    {
        public MilkTeaDbContext(
            DbContextOptions<MilkTeaDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhMuc> DanhMucs { get; set; }

        public DbSet<SanPham> SanPhams { get; set; }

        public DbSet<DonHang> DonHangs { get; set; }

        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}