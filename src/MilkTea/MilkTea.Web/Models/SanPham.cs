using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.Web.Models
{
    public class SanPham
    {
        [Key]
        public int SanPhamID { get; set; }

        public int DanhMucID { get; set; }

        [Required]
        public string TenSanPham { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaGoc { get; set; }

        public string? HinhAnh { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DanhGia { get; set; }

        public int SoLuongTon { get; set; }

        public int NhanSanPham { get; set; }

        public int ThuTuHienThi { get; set; }

        public bool TrangThai { get; set; }

        public DateTime NgayTao { get; set; }
    }
}