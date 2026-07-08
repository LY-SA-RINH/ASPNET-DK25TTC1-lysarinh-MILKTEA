using System.Collections.Generic;
namespace MilkTea.Web.Models
{
    public class DanhMuc
    {
        public int DanhMucID { get; set; }

        public string TenDanhMuc { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }

        public DateTime NgayTao { get; set; }
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
