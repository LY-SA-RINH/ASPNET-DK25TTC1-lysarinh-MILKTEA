using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class SuaDanhMucViewModel
        : ThemDanhMucViewModel
    {
        [Required]
        public int DanhMucID { get; set; }

        public int SoSanPham { get; set; }
    }
}