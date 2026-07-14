using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class SuaSanPhamViewModel : ThemSanPhamViewModel
    {
        [Required]
        public int SanPhamID { get; set; }

        [Display(Name = "Hình ảnh hiện tại")]
        public string? HinhAnhHienTai { get; set; }
    }
}