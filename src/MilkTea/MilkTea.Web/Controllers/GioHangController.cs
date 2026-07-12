using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;
using MilkTea.Web.TienIch;

namespace MilkTea.Web.Controllers
{
    public class GioHangController : Controller
    {
        private const string KhoaGioHang = "GioHang";

        private readonly MilkTeaDbContext _context;

        public GioHangController(MilkTeaDbContext context)
        {
            _context = context;
        }

        // Trang xem giỏ hàng
        public IActionResult Index()
        {
            List<GioHangItem> gioHang = LayGioHang();

            ViewBag.TongSoLuong = gioHang.Sum(item => item.SoLuong);
            ViewBag.TongTien = gioHang.Sum(item => item.ThanhTien);

            return View(gioHang);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(
            int id,
            int soLuong = 1,
            string? returnUrl = null)
        {
            if (soLuong < 1)
            {
                soLuong = 1;
            }

            SanPham? sanPham = await _context.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(sp =>
                    sp.SanPhamID == id &&
                    sp.TrangThai);

            if (sanPham == null)
            {
                TempData["Loi"] = "Không tìm thấy sản phẩm.";

                return ChuyenHuongSauKhiThem(returnUrl);
            }

            if (sanPham.SoLuongTon <= 0)
            {
                TempData["Loi"] = "Sản phẩm hiện đã hết hàng.";

                return ChuyenHuongSauKhiThem(returnUrl);
            }

            List<GioHangItem> gioHang = LayGioHang();

            GioHangItem? sanPhamTrongGio = gioHang
                .FirstOrDefault(item =>
                    item.SanPhamID == sanPham.SanPhamID);

            if (sanPhamTrongGio == null)
            {
                gioHang.Add(new GioHangItem
                {
                    SanPhamID = sanPham.SanPhamID,
                    TenSanPham = sanPham.TenSanPham,
                    HinhAnh = sanPham.HinhAnh,
                    Gia = sanPham.Gia,
                    SoLuong = Math.Min(soLuong, sanPham.SoLuongTon),
                    SoLuongTon = sanPham.SoLuongTon
                });
            }
            else
            {
                sanPhamTrongGio.TenSanPham = sanPham.TenSanPham;
                sanPhamTrongGio.HinhAnh = sanPham.HinhAnh;
                sanPhamTrongGio.Gia = sanPham.Gia;
                sanPhamTrongGio.SoLuongTon = sanPham.SoLuongTon;

                sanPhamTrongGio.SoLuong = Math.Min(
                    sanPhamTrongGio.SoLuong + soLuong,
                    sanPham.SoLuongTon);
            }

            LuuGioHang(gioHang);

            TempData["ThanhCong"] =
                $"Đã thêm {sanPham.TenSanPham} vào giỏ hàng.";

            return ChuyenHuongSauKhiThem(returnUrl);
        }

        // Tăng một đơn vị
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tang(int id)
        {
            List<GioHangItem> gioHang = LayGioHang();

            GioHangItem? item = gioHang
                .FirstOrDefault(gh => gh.SanPhamID == id);

            if (item == null)
            {
                return RedirectToAction(nameof(Index));
            }

            SanPham? sanPham = await _context.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(sp =>
                    sp.SanPhamID == id &&
                    sp.TrangThai);

            if (sanPham == null)
            {
                gioHang.Remove(item);
                LuuGioHang(gioHang);

                TempData["Loi"] =
                    "Sản phẩm không còn được bán và đã được xóa khỏi giỏ.";

                return RedirectToAction(nameof(Index));
            }

            item.Gia = sanPham.Gia;
            item.SoLuongTon = sanPham.SoLuongTon;

            if (item.SoLuong < sanPham.SoLuongTon)
            {
                item.SoLuong++;
            }
            else
            {
                TempData["Loi"] =
                    "Số lượng trong giỏ đã đạt mức tồn kho hiện tại.";
            }

            LuuGioHang(gioHang);

            return RedirectToAction(nameof(Index));
        }

        // Giảm một đơn vị
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Giam(int id)
        {
            List<GioHangItem> gioHang = LayGioHang();

            GioHangItem? item = gioHang
                .FirstOrDefault(gh => gh.SanPhamID == id);

            if (item != null)
            {
                item.SoLuong--;

                if (item.SoLuong <= 0)
                {
                    gioHang.Remove(item);
                }

                LuuGioHang(gioHang);
            }

            return RedirectToAction(nameof(Index));
        }

        // Xóa một sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Xoa(int id)
        {
            List<GioHangItem> gioHang = LayGioHang();

            GioHangItem? item = gioHang
                .FirstOrDefault(gh => gh.SanPhamID == id);

            if (item != null)
            {
                gioHang.Remove(item);
                LuuGioHang(gioHang);

                TempData["ThanhCong"] =
                    "Đã xóa sản phẩm khỏi giỏ hàng.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Xóa toàn bộ giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaTatCa()
        {
            HttpContext.Session.Remove(KhoaGioHang);

            TempData["ThanhCong"] = "Đã xóa toàn bộ giỏ hàng.";

            return RedirectToAction(nameof(Index));
        }

        private List<GioHangItem> LayGioHang()
        {
            return HttpContext.Session
                .LayDoiTuong<List<GioHangItem>>(KhoaGioHang)
                ?? new List<GioHangItem>();
        }

        private void LuuGioHang(List<GioHangItem> gioHang)
        {
            if (gioHang.Count == 0)
            {
                HttpContext.Session.Remove(KhoaGioHang);
                return;
            }

            HttpContext.Session.DatDoiTuong(
                KhoaGioHang,
                gioHang);
        }

        private IActionResult ChuyenHuongSauKhiThem(
            string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}