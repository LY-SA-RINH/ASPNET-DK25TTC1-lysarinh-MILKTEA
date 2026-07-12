using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    public class QuanLyDonHangController : Controller
    {
        private readonly MilkTeaDbContext _context;

        private static readonly string[] TrangThaiHopLe =
        {
            "Chờ xác nhận",
            "Đã xác nhận",
            "Đang giao",
            "Hoàn thành",
            "Đã hủy"
        };

        public QuanLyDonHangController(MilkTeaDbContext context)
        {
            _context = context;
        }

        // Danh sách đơn hàng
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai)
        {
            IQueryable<DonHang> truyVan = _context.DonHangs
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                if (int.TryParse(tuKhoa, out int donHangID))
                {
                    truyVan = truyVan.Where(dh =>
                        dh.DonHangID == donHangID ||
                        dh.HoTen.Contains(tuKhoa) ||
                        dh.SoDienThoai.Contains(tuKhoa));
                }
                else
                {
                    truyVan = truyVan.Where(dh =>
                        dh.HoTen.Contains(tuKhoa) ||
                        dh.SoDienThoai.Contains(tuKhoa));
                }
            }

            if (!string.IsNullOrWhiteSpace(trangThai) &&
                TrangThaiHopLe.Contains(trangThai))
            {
                truyVan = truyVan.Where(dh =>
                    dh.TrangThai == trangThai);
            }

            List<DonHang> donHangs = await truyVan
                .OrderByDescending(dh => dh.NgayDat)
                .ThenByDescending(dh => dh.DonHangID)
                .ToListAsync();

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;

            return View(donHangs);
        }

        // Xem chi tiết một đơn hàng
        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            DonHang? donHang = await _context.DonHangs
                .AsNoTracking()
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefaultAsync(dh =>
                    dh.DonHangID == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string trangThaiMoi)
        {
            DonHang? donHang = await _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefaultAsync(dh =>
                    dh.DonHangID == id);

            if (donHang == null)
            {
                return NotFound();
            }

            if (!TrangThaiHopLe.Contains(trangThaiMoi))
            {
                TempData["Loi"] =
                    "Trạng thái đơn hàng không hợp lệ.";

                return RedirectToAction(
                    nameof(ChiTiet),
                    new { id });
            }

            if (!DuocPhepChuyenTrangThai(
                    donHang.TrangThai,
                    trangThaiMoi))
            {
                TempData["Loi"] =
                    $"Không thể chuyển từ trạng thái " +
                    $"“{donHang.TrangThai}” sang “{trangThaiMoi}”.";

                return RedirectToAction(
                    nameof(ChiTiet),
                    new { id });
            }

            await using var giaoDich =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Nếu hủy đơn thì hoàn lại tồn kho.
                if (trangThaiMoi == "Đã hủy")
                {
                    List<int> danhSachSanPhamID = donHang
                        .ChiTietDonHangs
                        .Select(ct => ct.SanPhamID)
                        .Distinct()
                        .ToList();

                    List<SanPham> sanPhams = await _context.SanPhams
                        .Where(sp =>
                            danhSachSanPhamID.Contains(sp.SanPhamID))
                        .ToListAsync();

                    foreach (ChiTietDonHang chiTiet
                             in donHang.ChiTietDonHangs)
                    {
                        SanPham? sanPham = sanPhams
                            .FirstOrDefault(sp =>
                                sp.SanPhamID == chiTiet.SanPhamID);

                        if (sanPham != null)
                        {
                            sanPham.SoLuongTon += chiTiet.SoLuong;
                        }
                    }
                }

                donHang.TrangThai = trangThaiMoi;

                await _context.SaveChangesAsync();
                await giaoDich.CommitAsync();

                TempData["ThanhCong"] =
                    $"Đã cập nhật đơn hàng " +
                    $"DH-{donHang.DonHangID:D6} sang trạng thái " +
                    $"“{trangThaiMoi}”.";
            }
            catch
            {
                await giaoDich.RollbackAsync();

                TempData["Loi"] =
                    "Không thể cập nhật trạng thái đơn hàng. " +
                    "Vui lòng thử lại.";
            }

            return RedirectToAction(
                nameof(ChiTiet),
                new { id });
        }

        private static bool DuocPhepChuyenTrangThai(
            string trangThaiHienTai,
            string trangThaiMoi)
        {
            return trangThaiHienTai switch
            {
                "Chờ xác nhận" =>
                    trangThaiMoi == "Đã xác nhận" ||
                    trangThaiMoi == "Đã hủy",

                "Đã xác nhận" =>
                    trangThaiMoi == "Đang giao" ||
                    trangThaiMoi == "Đã hủy",

                "Đang giao" =>
                    trangThaiMoi == "Hoàn thành",

                _ => false
            };
        }
    }
}