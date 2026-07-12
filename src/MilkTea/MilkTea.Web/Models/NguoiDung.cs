using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class NguoiDung : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        // Cho phép null vì các tài khoản cũ chưa có địa chỉ.
        [StringLength(250)]
        public string? DiaChi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}