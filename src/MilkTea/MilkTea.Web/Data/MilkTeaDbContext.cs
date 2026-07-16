using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Models;

namespace MilkTea.Web.Data
{
    public class MilkTeaDbContext
        : IdentityDbContext<NguoiDung>
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

        public DbSet<LienHe> LienHes { get; set; }
    }
}
