namespace MilkTea.Web.Models
{
    public class GioHangItem
    {
        public int SanPhamID { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public string? HinhAnh { get; set; }

        public decimal Gia { get; set; }

        public int SoLuong { get; set; }

        public int SoLuongTon { get; set; }

        public decimal ThanhTien
        {
            get
            {
                return Gia * SoLuong;
            }
        }
    }
}