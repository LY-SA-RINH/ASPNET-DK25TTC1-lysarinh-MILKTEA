using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class ThemDanhMucViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(
            100,
            ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên danh mục")]
        public string TenDanhMuc { get; set; }
            = string.Empty;

        [StringLength(
            1000,
            ErrorMessage = "Mô tả không được vượt quá 1.000 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool TrangThai { get; set; } = true;
    }
}