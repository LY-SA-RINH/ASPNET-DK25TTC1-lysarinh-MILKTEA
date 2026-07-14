namespace MilkTea.Web.Models
{
    public class QuanLyDanhMucViewModel
    {
        public List<DanhMuc> DanhMucs { get; set; }
            = new List<DanhMuc>();

        public Dictionary<int, int> SoSanPhamTheoDanhMuc { get; set; }
            = new Dictionary<int, int>();

        public string? TuKhoa { get; set; }

        public string? TrangThai { get; set; }

        public int TrangHienTai { get; set; } = 1;

        public int TongSoTrang { get; set; }

        public int TongKetQua { get; set; }

        public int TongTatCaDanhMuc { get; set; }

        public int TongDanhMucHoatDong { get; set; }

        public int TongDanhMucTamNgung { get; set; }

        public int TongDanhMucCoSanPham { get; set; }
    }
}