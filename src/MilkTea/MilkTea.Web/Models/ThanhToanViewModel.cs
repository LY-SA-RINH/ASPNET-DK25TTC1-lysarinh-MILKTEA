using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class ThanhToanViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
        [StringLength(
            100,
            ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(
            20,
            ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        [StringLength(
            250,
            ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự.")]
        public string DiaChi { get; set; } = string.Empty;

        [StringLength(
            500,
            ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
        public string? GhiChu { get; set; }

        [Required]
        public string PhuongThucThanhToan { get; set; }
            = "Thanh toán khi nhận hàng";

        public List<GioHangItem> GioHang { get; set; }
            = new List<GioHangItem>();

        public int TongSoLuong
        {
            get
            {
                return GioHang.Sum(item => item.SoLuong);
            }
        }

        public decimal TongTien
        {
            get
            {
                return GioHang.Sum(item => item.ThanhTien);
            }
        }
    }
}