namespace MilkTea.Web.Models
{
    public class TongQuanQuanTriViewModel
    {
        // Thống kê sản phẩm
        public int TongSanPham { get; set; }

        public int TongSanPhamDangBan { get; set; }

        public int TongSanPhamNgungBan { get; set; }

        public int TongSanPhamHetHang { get; set; }

        // Thống kê đơn hàng
        public int TongDonHang { get; set; }

        public int TongDonHangChoXacNhan { get; set; }

        public int TongDonHangDaXacNhan { get; set; }

        public int TongDonHangDangGiao { get; set; }

        public int TongDonHangHoanThanh { get; set; }

        public int TongDonHangDaHuy { get; set; }

        // Doanh thu chỉ tính đơn đã hoàn thành
        public decimal DoanhThuDonHoanThanh { get; set; }

        // Liên hệ khách hàng đang chờ xử lý
        public int TongLienHeChuaXuLy { get; set; }

        // Dữ liệu hiển thị nhanh trên Tổng quan
        public List<DonHang> DonHangMoiNhat
        {
            get;
            set;
        } = new List<DonHang>();

        public List<SanPham> SanPhamSapHetHang
        {
            get;
            set;
        } = new List<SanPham>();
    }
}
