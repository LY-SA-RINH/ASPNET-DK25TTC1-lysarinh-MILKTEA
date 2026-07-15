using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class BaoCaoDoanhThuViewModel
    {
        // =====================================================
        // ĐIỀU KIỆN LỌC
        // =====================================================

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime DenNgay { get; set; }

        // =====================================================
        // THỐNG KÊ DOANH THU
        // =====================================================

        public int TongDonHangHoanThanh { get; set; }

        public decimal TongDoanhThu { get; set; }

        public decimal GiaTriDonTrungBinh { get; set; }

        public decimal DonHangGiaTriCaoNhat { get; set; }

        // =====================================================
        // DANH SÁCH ĐƠN HÀNG HOÀN THÀNH
        // =====================================================

        public List<DonHang> DonHangs { get; set; }
            = new List<DonHang>();
    }
}