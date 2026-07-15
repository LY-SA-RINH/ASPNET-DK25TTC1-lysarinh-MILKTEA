namespace MilkTea.Web.Models
{
    public class TongQuanQuanTriViewModel
    {
        // =====================================================
        // THỐNG KÊ SẢN PHẨM
        // =====================================================

        public int TongSanPham { get; set; }

        public int TongSanPhamDangBan { get; set; }

        public int TongSanPhamNgungBan { get; set; }

        public int TongSanPhamHetHang { get; set; }

        // =====================================================
        // THỐNG KÊ ĐƠN HÀNG
        // =====================================================

        public int TongDonHang { get; set; }

        public int TongDonHangChoXacNhan { get; set; }

        public int TongDonHangDaXacNhan { get; set; }

        public int TongDonHangDangGiao { get; set; }

        public int TongDonHangHoanThanh { get; set; }

        public int TongDonHangDaHuy { get; set; }

        // =====================================================
        // DOANH THU
        // Chỉ tính những đơn đã hoàn thành.
        // =====================================================

        public decimal DoanhThuDonHoanThanh { get; set; }

        // =====================================================
        // DANH SÁCH HIỂN THỊ TRÊN TRANG TỔNG QUAN
        // =====================================================

        public List<DonHang> DonHangMoiNhat { get; set; }
            = new List<DonHang>();

        public List<SanPham> SanPhamSapHetHang { get; set; }
            = new List<SanPham>();
    }
}