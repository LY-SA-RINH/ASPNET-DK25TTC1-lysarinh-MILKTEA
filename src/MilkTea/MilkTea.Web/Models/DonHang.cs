using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.Web.Models
{
    public class DonHang
    {
        [Key]
        public int DonHangID { get; set; }

        // Tài khoản khách hàng đã đặt đơn.
        // Cho phép null để các đơn hàng cũ vẫn tồn tại bình thường.
        [StringLength(450)]
        public string? NguoiDungID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        [StringLength(250)]
        public string DiaChi { get; set; } = string.Empty;

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; }
            = "Thanh toán khi nhận hàng";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; }
            = "Chờ xác nhận";

        public DateTime NgayDat { get; set; } = DateTime.Now;

        // Tài khoản Identity liên kết với đơn hàng.
        public NguoiDung? NguoiDung { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
            = new List<ChiTietDonHang>();
    }
}