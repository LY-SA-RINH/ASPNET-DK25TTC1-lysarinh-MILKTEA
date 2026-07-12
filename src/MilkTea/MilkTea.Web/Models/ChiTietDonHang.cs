using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.Web.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int ChiTietDonHangID { get; set; }

        public int DonHangID { get; set; }

        public int SanPhamID { get; set; }

        [Required]
        [StringLength(150)]
        public string TenSanPham { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        public DonHang DonHang { get; set; } = null!;
    }
}