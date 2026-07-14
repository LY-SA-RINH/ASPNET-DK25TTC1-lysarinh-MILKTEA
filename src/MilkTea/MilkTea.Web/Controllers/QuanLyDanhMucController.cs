using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class QuanLyDanhMucController : Controller
    {
        private const int SoDanhMucMoiTrang = 10;

        private readonly MilkTeaDbContext _context;

        public QuanLyDanhMucController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SÁCH QUẢN LÝ DANH MỤC
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai,
            int trang = 1)
        {
            if (trang < 1)
            {
                trang = 1;
            }

            tuKhoa = string.IsNullOrWhiteSpace(tuKhoa)
                ? null
                : tuKhoa.Trim();

            trangThai = string.IsNullOrWhiteSpace(trangThai)
                ? null
                : trangThai.Trim().ToLowerInvariant();

            if (trangThai != "hoat-dong" &&
                trangThai != "tam-ngung")
            {
                trangThai = null;
            }

            IQueryable<DanhMuc> truyVan =
                _context.DanhMucs
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                truyVan = truyVan.Where(dm =>
                    dm.TenDanhMuc.Contains(tuKhoa) ||
                    (
                        dm.MoTa != null &&
                        dm.MoTa.Contains(tuKhoa)
                    ));
            }

            if (trangThai == "hoat-dong")
            {
                truyVan = truyVan.Where(dm =>
                    dm.TrangThai);
            }
            else if (trangThai == "tam-ngung")
            {
                truyVan = truyVan.Where(dm =>
                    !dm.TrangThai);
            }

            int tongKetQua =
                await truyVan.CountAsync();

            int tongSoTrang = tongKetQua == 0
                ? 0
                : (int)Math.Ceiling(
                    tongKetQua /
                    (double)SoDanhMucMoiTrang);

            if (tongSoTrang > 0 &&
                trang > tongSoTrang)
            {
                trang = tongSoTrang;
            }

            List<DanhMuc> danhMucs =
                await truyVan
                    .OrderByDescending(dm =>
                        dm.TrangThai)
                    .ThenBy(dm =>
                        dm.TenDanhMuc)
                    .ThenBy(dm =>
                        dm.DanhMucID)
                    .Skip(
                        (trang - 1) *
                        SoDanhMucMoiTrang)
                    .Take(SoDanhMucMoiTrang)
                    .ToListAsync();

            List<int> danhMucIDs =
                danhMucs
                    .Select(dm =>
                        dm.DanhMucID)
                    .ToList();

            Dictionary<int, int> soSanPhamTheoDanhMuc =
                await _context.SanPhams
                    .AsNoTracking()
                    .Where(sp =>
                        danhMucIDs.Contains(
                            sp.DanhMucID))
                    .GroupBy(sp =>
                        sp.DanhMucID)
                    .Select(nhom => new
                    {
                        DanhMucID = nhom.Key,
                        SoSanPham = nhom.Count()
                    })
                    .ToDictionaryAsync(
                        item => item.DanhMucID,
                        item => item.SoSanPham);

            var model = new QuanLyDanhMucViewModel
            {
                DanhMucs = danhMucs,

                SoSanPhamTheoDanhMuc =
                    soSanPhamTheoDanhMuc,

                TuKhoa = tuKhoa,
                TrangThai = trangThai,

                TrangHienTai = trang,
                TongSoTrang = tongSoTrang,
                TongKetQua = tongKetQua,

                TongTatCaDanhMuc =
                    await _context.DanhMucs
                        .AsNoTracking()
                        .CountAsync(),

                TongDanhMucHoatDong =
                    await _context.DanhMucs
                        .AsNoTracking()
                        .CountAsync(dm =>
                            dm.TrangThai),

                TongDanhMucTamNgung =
                    await _context.DanhMucs
                        .AsNoTracking()
                        .CountAsync(dm =>
                            !dm.TrangThai),

                TongDanhMucCoSanPham =
                    await _context.SanPhams
                        .AsNoTracking()
                        .Select(sp =>
                            sp.DanhMucID)
                        .Distinct()
                        .CountAsync()
            };

            return View(model);
        }

        // =====================================================
        // THÊM DANH MỤC
        // =====================================================

        [HttpGet]
        public IActionResult Them()
        {
            var model = new ThemDanhMucViewModel
            {
                TrangThai = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(
            ThemDanhMucViewModel model)
        {
            model.TenDanhMuc =
                model.TenDanhMuc?.Trim()
                ?? string.Empty;

            model.MoTa =
                string.IsNullOrWhiteSpace(model.MoTa)
                    ? null
                    : model.MoTa.Trim();

            if (string.IsNullOrWhiteSpace(
                    model.TenDanhMuc))
            {
                ModelState.AddModelError(
                    nameof(model.TenDanhMuc),
                    "Vui lòng nhập tên danh mục.");
            }

            bool tenDanhMucDaTonTai =
                await _context.DanhMucs
                    .AsNoTracking()
                    .AnyAsync(dm =>
                        dm.TenDanhMuc ==
                        model.TenDanhMuc);

            if (tenDanhMucDaTonTai)
            {
                ModelState.AddModelError(
                    nameof(model.TenDanhMuc),
                    "Tên danh mục này đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var danhMuc = new DanhMuc
                {
                    TenDanhMuc =
                        model.TenDanhMuc,

                    MoTa =
                        model.MoTa,

                    TrangThai =
                        model.TrangThai,

                    NgayTao =
                        DateTime.Now
                };

                _context.DanhMucs.Add(
                    danhMuc);

                await _context.SaveChangesAsync();

                TempData["ThanhCong"] =
                    $"Đã thêm danh mục “{danhMuc.TenDanhMuc}” thành công.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm danh mục. Vui lòng thử lại.");

                return View(model);
            }
        }

        // =====================================================
        // SỬA DANH MỤC
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            DanhMuc? danhMuc =
                await _context.DanhMucs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dm =>
                        dm.DanhMucID == id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            int soSanPham =
                await _context.SanPhams
                    .AsNoTracking()
                    .CountAsync(sp =>
                        sp.DanhMucID ==
                        danhMuc.DanhMucID);

            var model = new SuaDanhMucViewModel
            {
                DanhMucID =
                    danhMuc.DanhMucID,

                TenDanhMuc =
                    danhMuc.TenDanhMuc,

                MoTa =
                    danhMuc.MoTa,

                TrangThai =
                    danhMuc.TrangThai,

                SoSanPham =
                    soSanPham
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(
            SuaDanhMucViewModel model)
        {
            if (model.DanhMucID <= 0)
            {
                return NotFound();
            }

            DanhMuc? danhMuc =
                await _context.DanhMucs
                    .FirstOrDefaultAsync(dm =>
                        dm.DanhMucID ==
                        model.DanhMucID);

            if (danhMuc == null)
            {
                return NotFound();
            }

            model.TenDanhMuc =
                model.TenDanhMuc?.Trim()
                ?? string.Empty;

            model.MoTa =
                string.IsNullOrWhiteSpace(model.MoTa)
                    ? null
                    : model.MoTa.Trim();

            model.SoSanPham =
                await _context.SanPhams
                    .AsNoTracking()
                    .CountAsync(sp =>
                        sp.DanhMucID ==
                        model.DanhMucID);

            if (string.IsNullOrWhiteSpace(
                    model.TenDanhMuc))
            {
                ModelState.AddModelError(
                    nameof(model.TenDanhMuc),
                    "Vui lòng nhập tên danh mục.");
            }

            bool tenDanhMucDaTonTai =
                await _context.DanhMucs
                    .AsNoTracking()
                    .AnyAsync(dm =>
                        dm.DanhMucID !=
                            model.DanhMucID &&
                        dm.TenDanhMuc ==
                            model.TenDanhMuc);

            if (tenDanhMucDaTonTai)
            {
                ModelState.AddModelError(
                    nameof(model.TenDanhMuc),
                    "Tên danh mục này đã được sử dụng.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                danhMuc.TenDanhMuc =
                    model.TenDanhMuc;

                danhMuc.MoTa =
                    model.MoTa;

                danhMuc.TrangThai =
                    model.TrangThai;

                // Giữ nguyên NgayTao vì đây là
                // ngày danh mục được tạo ban đầu.

                await _context.SaveChangesAsync();

                TempData["ThanhCong"] =
                    $"Đã cập nhật danh mục “{danhMuc.TenDanhMuc}” thành công.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật danh mục. Vui lòng thử lại.");

                return View(model);
            }
        }

        // =====================================================
        // KÍCH HOẠT HOẶC TẠM NGƯNG DANH MỤC
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string thaoTac,
            string? tuKhoa,
            string? trangThai,
            int trang = 1)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            DanhMuc? danhMuc =
                await _context.DanhMucs
                    .FirstOrDefaultAsync(dm =>
                        dm.DanhMucID == id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            bool trangThaiMoi;

            if (thaoTac == "kich-hoat")
            {
                trangThaiMoi = true;
            }
            else if (thaoTac == "tam-ngung")
            {
                trangThaiMoi = false;
            }
            else
            {
                TempData["Loi"] =
                    "Thao tác cập nhật trạng thái danh mục không hợp lệ.";

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        tuKhoa,
                        trangThai,
                        trang
                    });
            }

            danhMuc.TrangThai =
                trangThaiMoi;

            await _context.SaveChangesAsync();

            TempData["ThanhCong"] =
                trangThaiMoi
                    ? $"Đã kích hoạt danh mục “{danhMuc.TenDanhMuc}”."
                    : $"Đã tạm ngưng danh mục “{danhMuc.TenDanhMuc}”.";

            return RedirectToAction(
                nameof(Index),
                new
                {
                    tuKhoa,
                    trangThai,
                    trang
                });
        }
    }
}