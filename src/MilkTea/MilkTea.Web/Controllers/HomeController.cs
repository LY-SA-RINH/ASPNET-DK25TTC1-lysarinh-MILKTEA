using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;
using System.Diagnostics;

namespace MilkTea.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MilkTeaDbContext _context;

        public HomeController(MilkTeaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPhams
                .Where(sp => sp.TrangThai == true)
                .OrderBy(sp => sp.ThuTuHienThi)
                .Take(4)
                .ToListAsync();

            return View(sanPhams);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}