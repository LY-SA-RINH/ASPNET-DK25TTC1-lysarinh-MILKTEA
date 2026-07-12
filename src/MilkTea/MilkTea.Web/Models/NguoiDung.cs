using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class NguoiDung : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}