using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;
using System.Data;
using System.Security.Claims;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class DonHangCuaToiController : Controller
    {
        private readonly MilkTeaDbContext _context;

        public DonHangCuaToiController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        // Danh sách đơn hàng của khách hàng đang đăng nhập
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? nguoiDungID = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(nguoiDungID))
            {
                return Challenge();
            }

            List<DonHang> donHangs = await _context.DonHangs
                .AsNoTracking()
                .Where(dh =>
                    dh.NguoiDungID == nguoiDungID)
                .OrderByDescending(dh => dh.NgayDat)
                .ThenByDescending(dh => dh.DonHangID)
                .ToListAsync();

            return View(donHangs);
        }

        // Chi tiết một đơn hàng của khách hàng
        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            string? nguoiDungID = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(nguoiDungID))
            {
                return Challenge();
            }

            DonHang? donHang = await _context.DonHangs
                .AsNoTracking()
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefaultAsync(dh =>
                    dh.DonHangID == id &&
                    dh.NguoiDungID == nguoiDungID);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // Khách hàng hủy đơn khi đơn còn chờ xác nhận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDonHang(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            string? nguoiDungID = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(nguoiDungID))
            {
                return Challenge();
            }

            await using var giaoDich =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                DonHang? donHang = await _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                    .FirstOrDefaultAsync(dh =>
                        dh.DonHangID == id &&
                        dh.NguoiDungID == nguoiDungID);

                // Không tìm thấy đơn hoặc đơn không thuộc khách hàng.
                if (donHang == null)
                {
                    await giaoDich.RollbackAsync();

                    return NotFound();
                }

                // Khách hàng chỉ được hủy đơn đang chờ xác nhận.
                if (donHang.TrangThai != "Chờ xác nhận")
                {
                    await giaoDich.RollbackAsync();

                    TempData["Loi"] =
                        "Chỉ có thể hủy đơn hàng đang ở trạng thái " +
                        "“Chờ xác nhận”.";

                    return RedirectToAction(
                        nameof(ChiTiet),
                        new { id });
                }

                List<int> danhSachSanPhamID = donHang
                    .ChiTietDonHangs
                    .Select(ct => ct.SanPhamID)
                    .Distinct()
                    .ToList();

                List<SanPham> sanPhams = await _context.SanPhams
                    .Where(sp =>
                        danhSachSanPhamID.Contains(sp.SanPhamID))
                    .ToListAsync();

                Dictionary<int, SanPham> sanPhamTheoID = sanPhams
                    .ToDictionary(sp => sp.SanPhamID);

                // Hoàn lại số lượng tồn kho.
                foreach (ChiTietDonHang chiTiet
                         in donHang.ChiTietDonHangs)
                {
                    if (sanPhamTheoID.TryGetValue(
                            chiTiet.SanPhamID,
                            out SanPham? sanPham))
                    {
                        sanPham.SoLuongTon += chiTiet.SoLuong;
                    }
                }

                donHang.TrangThai = "Đã hủy";

                await _context.SaveChangesAsync();
                await giaoDich.CommitAsync();

                TempData["ThanhCong"] =
                    $"Đã hủy đơn hàng " +
                    $"DH-{donHang.DonHangID:D6} thành công.";
            }
            catch
            {
                await giaoDich.RollbackAsync();

                TempData["Loi"] =
                    "Không thể hủy đơn hàng. Vui lòng thử lại.";
            }

            return RedirectToAction(
                nameof(ChiTiet),
                new { id });
        }
    }
}