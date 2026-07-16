using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class LienHeViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(
            100,
            ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(
            256,
            ErrorMessage = "Email không được vượt quá 256 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
            = string.Empty;

        [StringLength(
            20,
            ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [StringLength(
            150,
            MinimumLength = 3,
            ErrorMessage = "Tiêu đề phải từ 3 đến 150 ký tự.")]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ.")]
        [StringLength(
            2000,
            MinimumLength = 10,
            ErrorMessage = "Nội dung phải từ 10 đến 2.000 ký tự.")]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }
            = string.Empty;
    }
}
