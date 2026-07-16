using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class QuanLyKhachHangController : Controller
    {
        private const int SoKhachHangMoiTrang = 10;

        private readonly UserManager<NguoiDung> _userManager;
        private readonly MilkTeaDbContext _context;

        public QuanLyKhachHangController(
            UserManager<NguoiDung> userManager,
            MilkTeaDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // =====================================================
        // DANH SÁCH KHÁCH HÀNG
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

            if (trangThai != "dang-hoat-dong" &&
                trangThai != "da-khoa")
            {
                trangThai = null;
            }

            IList<NguoiDung> tatCaKhachHang =
                await _userManager.GetUsersInRoleAsync(
                    KhoiTaoTaiKhoan.KhachHang);

            DateTimeOffset thoiDiemHienTai =
                DateTimeOffset.UtcNow;

            bool KiemTraBiKhoa(NguoiDung nguoiDung)
            {
                return nguoiDung.LockoutEnd.HasValue &&
                       nguoiDung.LockoutEnd.Value >
                       thoiDiemHienTai;
            }

            int tongKhachHang =
                tatCaKhachHang.Count;

            int tongDaKhoa =
                tatCaKhachHang.Count(KiemTraBiKhoa);

            int tongDangHoatDong =
                tongKhachHang - tongDaKhoa;

            IEnumerable<NguoiDung> truyVan =
                tatCaKhachHang;

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                truyVan = truyVan.Where(nguoiDung =>
                    (!string.IsNullOrWhiteSpace(
                        nguoiDung.HoTen) &&
                     nguoiDung.HoTen.Contains(
                         tuKhoa,
                         StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrWhiteSpace(
                        nguoiDung.Email) &&
                     nguoiDung.Email.Contains(
                         tuKhoa,
                         StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrWhiteSpace(
                        nguoiDung.PhoneNumber) &&
                     nguoiDung.PhoneNumber.Contains(
                         tuKhoa,
                         StringComparison.OrdinalIgnoreCase)));
            }

            if (trangThai == "dang-hoat-dong")
            {
                truyVan = truyVan.Where(nguoiDung =>
                    !KiemTraBiKhoa(nguoiDung));
            }
            else if (trangThai == "da-khoa")
            {
                truyVan = truyVan.Where(KiemTraBiKhoa);
            }

            truyVan = truyVan
                .OrderByDescending(nguoiDung =>
                    nguoiDung.NgayTao)
                .ThenBy(nguoiDung =>
                    nguoiDung.HoTen);

            int tongKetQua =
                truyVan.Count();

            int tongSoTrang = tongKetQua == 0
                ? 0
                : (int)Math.Ceiling(
                    tongKetQua /
                    (double)SoKhachHangMoiTrang);

            if (tongSoTrang > 0 &&
                trang > tongSoTrang)
            {
                trang = tongSoTrang;
            }

            List<NguoiDung> khachHangTrongTrang =
                truyVan
                    .Skip(
                        (trang - 1) *
                        SoKhachHangMoiTrang)
                    .Take(SoKhachHangMoiTrang)
                    .ToList();

            List<string> danhSachNguoiDungID =
                khachHangTrongTrang
                    .Select(nguoiDung =>
                        nguoiDung.Id)
                    .ToList();

            var danhSachThongKe =
                await _context.DonHangs
                    .AsNoTracking()
                    .Where(donHang =>
                        donHang.NguoiDungID != null &&
                        danhSachNguoiDungID.Contains(
                            donHang.NguoiDungID))
                    .GroupBy(donHang =>
                        donHang.NguoiDungID!)
                    .Select(nhom => new
                    {
                        NguoiDungID = nhom.Key,

                        TongDonHang = nhom.Count(),

                        TongChiTieu = nhom.Sum(donHang =>
                            donHang.TrangThai == "Hoàn thành"
                                ? donHang.TongTien
                                : 0m)
                    })
                    .ToListAsync();

            var thongKeTheoNguoiDungID =
                danhSachThongKe.ToDictionary(
                    thongKe =>
                        thongKe.NguoiDungID);

            var danhSachKhachHang =
                new List<KhachHangQuanLyItemViewModel>();

            foreach (NguoiDung nguoiDung
                     in khachHangTrongTrang)
            {
                int tongDonHangNguoiDung = 0;
                decimal tongChiTieuNguoiDung = 0m;

                if (thongKeTheoNguoiDungID.TryGetValue(
                    nguoiDung.Id,
                    out var thongKe))
                {
                    tongDonHangNguoiDung =
                        thongKe.TongDonHang;

                    tongChiTieuNguoiDung =
                        thongKe.TongChiTieu;
                }

                danhSachKhachHang.Add(
                    new KhachHangQuanLyItemViewModel
                    {
                        NguoiDungID =
                            nguoiDung.Id,

                        HoTen =
                            nguoiDung.HoTen,

                        Email =
                            nguoiDung.Email
                            ?? string.Empty,

                        SoDienThoai =
                            nguoiDung.PhoneNumber
                            ?? string.Empty,

                        DiaChi =
                            nguoiDung.DiaChi
                            ?? string.Empty,

                        NgayTao =
                            nguoiDung.NgayTao,

                        BiKhoa =
                            KiemTraBiKhoa(nguoiDung),

                        TongDonHang =
                            tongDonHangNguoiDung,

                        TongChiTieu =
                            tongChiTieuNguoiDung
                    });
            }

            var model =
                new QuanLyKhachHangViewModel
                {
                    TuKhoa = tuKhoa,

                    TrangThai = trangThai,

                    TrangHienTai = trang,

                    TongSoTrang = tongSoTrang,

                    TongKetQua = tongKetQua,

                    TongKhachHang =
                        tongKhachHang,

                    TongDangHoatDong =
                        tongDangHoatDong,

                    TongDaKhoa =
                        tongDaKhoa,

                    KhachHangs =
                        danhSachKhachHang
                };

            return View(model);
        }

        // =====================================================
        // KHÓA HOẶC MỞ KHÓA TÀI KHOẢN
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThayDoiTrangThai(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Loi"] =
                    "Không tìm thấy tài khoản khách hàng.";

                return RedirectToAction(nameof(Index));
            }

            NguoiDung? nguoiDung =
                await _userManager.FindByIdAsync(id);

            if (nguoiDung == null)
            {
                TempData["Loi"] =
                    "Tài khoản khách hàng không tồn tại.";

                return RedirectToAction(nameof(Index));
            }

            bool laKhachHang =
                await _userManager.IsInRoleAsync(
                    nguoiDung,
                    KhoiTaoTaiKhoan.KhachHang);

            if (!laKhachHang)
            {
                TempData["Loi"] =
                    "Chỉ được thay đổi trạng thái " +
                    "tài khoản khách hàng.";

                return RedirectToAction(nameof(Index));
            }

            bool dangBiKhoa =
                nguoiDung.LockoutEnd.HasValue &&
                nguoiDung.LockoutEnd.Value >
                DateTimeOffset.UtcNow;

            if (dangBiKhoa)
            {
                IdentityResult ketQuaMoKhoa =
                    await _userManager
                        .SetLockoutEndDateAsync(
                            nguoiDung,
                            null);

                if (!ketQuaMoKhoa.Succeeded)
                {
                    TempData["Loi"] =
                        "Không thể mở khóa tài khoản.";

                    return RedirectToAction(
                        nameof(Index));
                }

                await _userManager
                    .ResetAccessFailedCountAsync(
                        nguoiDung);

                TempData["ThanhCong"] =
                    "Đã mở khóa tài khoản khách hàng.";
            }
            else
            {
                if (!nguoiDung.LockoutEnabled)
                {
                    IdentityResult ketQuaChoPhepKhoa =
                        await _userManager
                            .SetLockoutEnabledAsync(
                                nguoiDung,
                                true);

                    if (!ketQuaChoPhepKhoa.Succeeded)
                    {
                        TempData["Loi"] =
                            "Không thể bật chức năng " +
                            "khóa tài khoản.";

                        return RedirectToAction(
                            nameof(Index));
                    }
                }

                IdentityResult ketQuaKhoa =
                    await _userManager
                        .SetLockoutEndDateAsync(
                            nguoiDung,
                            DateTimeOffset.MaxValue);

                if (!ketQuaKhoa.Succeeded)
                {
                    TempData["Loi"] =
                        "Không thể khóa tài khoản.";

                    return RedirectToAction(
                        nameof(Index));
                }

                TempData["ThanhCong"] =
                    "Đã khóa tài khoản khách hàng.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}