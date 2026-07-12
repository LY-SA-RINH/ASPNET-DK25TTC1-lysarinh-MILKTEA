namespace MilkTea.Web.Models
{
    public class QuanLySanPhamViewModel
    {
        public List<SanPham> SanPhams { get; set; }
            = new List<SanPham>();

        public List<DanhMuc> DanhMucs { get; set; }
            = new List<DanhMuc>();

        public Dictionary<int, string> TenDanhMucTheoID { get; set; }
            = new Dictionary<int, string>();

        public string? TuKhoa { get; set; }

        public int? DanhMucID { get; set; }

        public string? TrangThai { get; set; }

        public int TrangHienTai { get; set; } = 1;

        public int TongSoTrang { get; set; }

        public int TongKetQua { get; set; }

        public int TongTatCaSanPham { get; set; }

        public int TongSanPhamDangBan { get; set; }

        public int TongSanPhamNgungBan { get; set; }

        public int TongSanPhamHetHang { get; set; }
    }
}