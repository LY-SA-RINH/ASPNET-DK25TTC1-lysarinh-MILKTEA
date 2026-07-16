using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class QuanLyLienHeController : Controller
    {
        private const int SoLienHeMoiTrang = 10;

        private const string TrangThaiChuaXuLy =
            "Chưa xử lý";

        private const string TrangThaiDaXuLy =
            "Đã xử lý";

        private readonly MilkTeaDbContext _context;

        public QuanLyLienHeController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        // Danh sách liên hệ
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

            if (tuKhoa != null &&
                tuKhoa.Length > 150)
            {
                tuKhoa = tuKhoa[..150];
            }

            trangThai = string.IsNullOrWhiteSpace(
                    trangThai)
                ? null
                : trangThai.Trim()
                    .ToLowerInvariant();

            if (trangThai != "chua-xu-ly" &&
                trangThai != "da-xu-ly")
            {
                trangThai = null;
            }

            IQueryable<LienHe> tatCaLienHe =
                _context.LienHes
                    .AsNoTracking();

            int tongLienHe =
                await tatCaLienHe.CountAsync();

            int tongChuaXuLy =
                await tatCaLienHe.CountAsync(lienHe =>
                    lienHe.TrangThai ==
                    TrangThaiChuaXuLy);

            int tongDaXuLy =
                await tatCaLienHe.CountAsync(lienHe =>
                    lienHe.TrangThai ==
                    TrangThaiDaXuLy);

            IQueryable<LienHe> truyVan =
                tatCaLienHe;

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                truyVan = truyVan.Where(lienHe =>
                    lienHe.HoTen.Contains(tuKhoa) ||
                    lienHe.Email.Contains(tuKhoa) ||
                    (
                        lienHe.SoDienThoai != null &&
                        lienHe.SoDienThoai.Contains(
                            tuKhoa)
                    ) ||
                    lienHe.TieuDe.Contains(tuKhoa) ||
                    lienHe.NoiDung.Contains(tuKhoa));
            }

            if (trangThai == "chua-xu-ly")
            {
                truyVan = truyVan.Where(lienHe =>
                    lienHe.TrangThai ==
                    TrangThaiChuaXuLy);
            }
            else if (trangThai == "da-xu-ly")
            {
                truyVan = truyVan.Where(lienHe =>
                    lienHe.TrangThai ==
                    TrangThaiDaXuLy);
            }

            int tongKetQua =
                await truyVan.CountAsync();

            int tongSoTrang = tongKetQua == 0
                ? 0
                : (int)Math.Ceiling(
                    tongKetQua /
                    (double)SoLienHeMoiTrang);

            if (tongSoTrang > 0 &&
                trang > tongSoTrang)
            {
                trang = tongSoTrang;
            }

            List<LienHeQuanLyItemViewModel>
                danhSachLienHe =
                    await truyVan
                        .OrderBy(lienHe =>
                            lienHe.TrangThai ==
                            TrangThaiDaXuLy)
                        .ThenByDescending(lienHe =>
                            lienHe.NgayGui)
                        .ThenByDescending(lienHe =>
                            lienHe.LienHeID)
                        .Skip(
                            (trang - 1) *
                            SoLienHeMoiTrang)
                        .Take(SoLienHeMoiTrang)
                        .Select(lienHe =>
                            new LienHeQuanLyItemViewModel
                            {
                                LienHeID =
                                    lienHe.LienHeID,

                                HoTen =
                                    lienHe.HoTen,

                                Email =
                                    lienHe.Email,

                                SoDienThoai =
                                    lienHe.SoDienThoai,

                                TieuDe =
                                    lienHe.TieuDe,

                                NoiDung =
                                    lienHe.NoiDung,

                                TrangThai =
                                    lienHe.TrangThai,

                                NgayGui =
                                    lienHe.NgayGui,

                                DaXuLy =
                                    lienHe.TrangThai ==
                                    TrangThaiDaXuLy,

                                CoTaiKhoan =
                                    lienHe.NguoiDungID != null
                            })
                        .ToListAsync();

            var model =
                new QuanLyLienHeViewModel
                {
                    TuKhoa = tuKhoa,
                    TrangThai = trangThai,
                    TrangHienTai = trang,
                    TongSoTrang = tongSoTrang,
                    TongKetQua = tongKetQua,
                    TongLienHe = tongLienHe,
                    TongChuaXuLy = tongChuaXuLy,
                    TongDaXuLy = tongDaXuLy,
                    LienHes = danhSachLienHe
                };

            return View(model);
        }

        // Xem chi tiết nội dung liên hệ
        [HttpGet]
        public async Task<IActionResult> ChiTiet(
            int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ChiTietLienHeViewModel? model =
                await _context.LienHes
                    .AsNoTracking()
                    .Where(lienHe =>
                        lienHe.LienHeID == id)
                    .Select(lienHe =>
                        new ChiTietLienHeViewModel
                        {
                            LienHeID =
                                lienHe.LienHeID,

                            NguoiDungID =
                                lienHe.NguoiDungID,

                            HoTen =
                                lienHe.HoTen,

                            Email =
                                lienHe.Email,

                            SoDienThoai =
                                lienHe.SoDienThoai,

                            TieuDe =
                                lienHe.TieuDe,

                            NoiDung =
                                lienHe.NoiDung,

                            TrangThai =
                                lienHe.TrangThai,

                            NgayGui =
                                lienHe.NgayGui,

                            DaXuLy =
                                lienHe.TrangThai ==
                                TrangThaiDaXuLy,

                            CoTaiKhoan =
                                lienHe.NguoiDungID != null
                        })
                    .FirstOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // Đánh dấu đã xử lý hoặc mở lại liên hệ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string thaoTac,
            string? returnUrl)
        {
            if (id <= 0)
            {
                TempData["Loi"] =
                    "Không tìm thấy nội dung liên hệ.";

                return RedirectToAction(nameof(Index));
            }

            LienHe? lienHe =
                await _context.LienHes
                    .FirstOrDefaultAsync(item =>
                        item.LienHeID == id);

            if (lienHe == null)
            {
                TempData["Loi"] =
                    "Nội dung liên hệ không tồn tại.";

                return RedirectToAction(nameof(Index));
            }

            string trangThaiMoi;

            if (thaoTac == "danh-dau-da-xu-ly")
            {
                trangThaiMoi =
                    TrangThaiDaXuLy;
            }
            else if (thaoTac == "mo-lai")
            {
                trangThaiMoi =
                    TrangThaiChuaXuLy;
            }
            else
            {
                TempData["Loi"] =
                    "Thao tác cập nhật trạng thái " +
                    "không hợp lệ.";

                return ChuyenHuongSauCapNhat(
                    returnUrl,
                    id);
            }

            if (lienHe.TrangThai != trangThaiMoi)
            {
                lienHe.TrangThai =
                    trangThaiMoi;

                await _context.SaveChangesAsync();
            }

            TempData["ThanhCong"] =
                trangThaiMoi == TrangThaiDaXuLy
                    ? "Đã đánh dấu nội dung liên hệ là đã xử lý."
                    : "Đã mở lại nội dung liên hệ để tiếp tục xử lý.";

            return ChuyenHuongSauCapNhat(
                returnUrl,
                id);
        }

        private IActionResult ChuyenHuongSauCapNhat(
            string? returnUrl,
            int id)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(
                nameof(ChiTiet),
                new
                {
                    id
                });
        }
    }
}
