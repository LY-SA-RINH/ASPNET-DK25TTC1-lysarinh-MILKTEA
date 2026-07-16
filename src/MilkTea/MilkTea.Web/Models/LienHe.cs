using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class LienHe
    {
        [Key]
        public int LienHeID { get; set; }

        [StringLength(450)]
        public string? NguoiDungID { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        [Required]
        [StringLength(150)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; }
            = "Chưa xử lý";

        public DateTime NgayGui { get; set; }
            = DateTime.Now;
    }
}
