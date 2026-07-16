namespace MilkTea.Web.Models
{
    public class QuanLyKhachHangViewModel
    {
        public string? TuKhoa { get; set; }

        public string? TrangThai { get; set; }

        public int TrangHienTai { get; set; }

        public int TongSoTrang { get; set; }

        public int TongKetQua { get; set; }

        public int TongKhachHang { get; set; }

        public int TongDangHoatDong { get; set; }

        public int TongDaKhoa { get; set; }

        public List<KhachHangQuanLyItemViewModel> KhachHangs
        {
            get;
            set;
        } = new List<KhachHangQuanLyItemViewModel>();
    }

    public class KhachHangQuanLyItemViewModel
    {
        public string NguoiDungID { get; set; }
            = string.Empty;

        public string HoTen { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public string SoDienThoai { get; set; }
            = string.Empty;

        public string DiaChi { get; set; }
            = string.Empty;

        public DateTime NgayTao { get; set; }

        public bool BiKhoa { get; set; }

        public int TongDonHang { get; set; }

        public decimal TongChiTieu { get; set; }
    }
}