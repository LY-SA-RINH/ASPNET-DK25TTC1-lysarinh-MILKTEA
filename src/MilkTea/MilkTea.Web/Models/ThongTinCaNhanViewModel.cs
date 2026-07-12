using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class ThongTinCaNhanViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
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

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(
            250,
            ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự.")]
        public string DiaChi { get; set; } = string.Empty;
    }
}