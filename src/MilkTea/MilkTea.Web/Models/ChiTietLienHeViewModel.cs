namespace MilkTea.Web.Models
{
    public class ChiTietLienHeViewModel
    {
        public int LienHeID { get; set; }

        public string? NguoiDungID { get; set; }

        public string HoTen { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public string? SoDienThoai { get; set; }

        public string TieuDe { get; set; }
            = string.Empty;

        public string NoiDung { get; set; }
            = string.Empty;

        public string TrangThai { get; set; }
            = string.Empty;

        public DateTime NgayGui { get; set; }

        public bool DaXuLy { get; set; }

        public bool CoTaiKhoan { get; set; }
    }
}
