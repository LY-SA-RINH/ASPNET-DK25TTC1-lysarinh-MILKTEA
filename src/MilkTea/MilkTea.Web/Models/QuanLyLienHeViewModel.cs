namespace MilkTea.Web.Models
{
    public class QuanLyLienHeViewModel
    {
        public string? TuKhoa { get; set; }

        public string? TrangThai { get; set; }

        public int TrangHienTai { get; set; }

        public int TongSoTrang { get; set; }

        public int TongKetQua { get; set; }

        public int TongLienHe { get; set; }

        public int TongChuaXuLy { get; set; }

        public int TongDaXuLy { get; set; }

        public List<LienHeQuanLyItemViewModel> LienHes
        {
            get;
            set;
        } = new List<LienHeQuanLyItemViewModel>();
    }
}
