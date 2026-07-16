using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [AllowAnonymous]
    public class LienHeController : Controller
    {
        private readonly MilkTeaDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public LienHeController(
            MilkTeaDbContext context,
            UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Trang liên hệ
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new LienHeViewModel();

            if (User.Identity?.IsAuthenticated == true)
            {
                NguoiDung? nguoiDung =
                    await _userManager.GetUserAsync(User);

                if (nguoiDung != null)
                {
                    model.HoTen =
                        nguoiDung.HoTen ?? string.Empty;

                    model.Email =
                        nguoiDung.Email ?? string.Empty;

                    model.SoDienThoai =
                        nguoiDung.PhoneNumber;
                }
            }

            return View(model);
        }

        // Tiếp nhận nội dung liên hệ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            LienHeViewModel model)
        {
            model.HoTen =
                model.HoTen?.Trim()
                ?? string.Empty;

            model.Email =
                model.Email?.Trim()
                    .ToLowerInvariant()
                ?? string.Empty;

            model.SoDienThoai =
                string.IsNullOrWhiteSpace(
                    model.SoDienThoai)
                    ? null
                    : model.SoDienThoai.Trim();

            model.TieuDe =
                model.TieuDe?.Trim()
                ?? string.Empty;

            model.NoiDung =
                model.NoiDung?.Trim()
                ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? nguoiDungID = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                NguoiDung? nguoiDung =
                    await _userManager.GetUserAsync(User);

                nguoiDungID = nguoiDung?.Id;
            }

            var lienHe = new LienHe
            {
                NguoiDungID = nguoiDungID,
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                TieuDe = model.TieuDe,
                NoiDung = model.NoiDung,
                TrangThai = "Chưa xử lý",
                NgayGui = DateTime.Now
            };

            try
            {
                _context.LienHes.Add(lienHe);

                await _context.SaveChangesAsync();

                TempData["LienHeThanhCong"] =
                    "MilkTea House đã nhận được nội dung liên hệ " +
                    "của bạn. Chúng tôi sẽ phản hồi trong thời gian sớm nhất.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể gửi nội dung liên hệ lúc này. " +
                    "Vui lòng thử lại sau.");

                return View(model);
            }
        }
    }
}
