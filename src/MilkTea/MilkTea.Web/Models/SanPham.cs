namespace MilkTea.Web.Models
{
    public class SanPham
    {
        public int SanPhamID { get; set; }

        public int DanhMucID { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public decimal Gia { get; set; }

        public decimal? GiaGoc { get; set; }

        public string? HinhAnh { get; set; }

        public decimal DanhGia { get; set; }

        public int SoLuongTon { get; set; }

        public int NhanSanPham { get; set; }

        public int ThuTuHienThi { get; set; }

        public bool TrangThai { get; set; }

        public DateTime NgayTao { get; set; }
        public DanhMuc? DanhMuc { get; set; }
    }
}