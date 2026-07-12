using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;
using MilkTea.Web.TienIch;

namespace MilkTea.Web.Controllers
{
    public class ThanhToanController : Controller
    {
        private const string KhoaGioHang = "GioHang";
        private const string KhoaDonHangVuaDat = "DonHangVuaDat";

        private readonly MilkTeaDbContext _context;

        public ThanhToanController(MilkTeaDbContext context)
        {
            _context = context;
        }

        // Trang nhập thông tin nhận hàng
        [HttpGet]
        public IActionResult Index()
        {
            List<GioHangItem> gioHang = LayGioHang();

            if (gioHang.Count == 0)
            {
                TempData["Loi"] =
                    "Giỏ hàng đang trống. Vui lòng chọn sản phẩm trước.";

                return RedirectToAction(
                    "Index",
                    "GioHang");
            }

            var model = new ThanhToanViewModel
            {
                GioHang = gioHang
            };

            return View(model);
        }

        // Xử lý đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(
            ThanhToanViewModel model)
        {
            List<GioHangItem> gioHang = LayGioHang();

            model.GioHang = gioHang;

            if (gioHang.Count == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Giỏ hàng đang trống.");
            }

            if (model.PhuongThucThanhToan
                != "Thanh toán khi nhận hàng")
            {
                ModelState.AddModelError(
                    nameof(model.PhuongThucThanhToan),
                    "Phương thức thanh toán không hợp lệ.");
            }

            List<int> danhSachSanPhamID = gioHang
                .Select(item => item.SanPhamID)
                .ToList();

            List<SanPham> sanPhams = await _context.SanPhams
                .Where(sp => danhSachSanPhamID.Contains(sp.SanPhamID))
                .ToListAsync();

            foreach (GioHangItem item in gioHang)
            {
                SanPham? sanPham = sanPhams
                    .FirstOrDefault(sp =>
                        sp.SanPhamID == item.SanPhamID);

                if (sanPham == null || !sanPham.TrangThai)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"Sản phẩm {item.TenSanPham} không còn được bán.");

                    continue;
                }

                if (item.SoLuong > sanPham.SoLuongTon)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"Sản phẩm {sanPham.TenSanPham} chỉ còn " +
                        $"{sanPham.SoLuongTon} sản phẩm.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            await using var giaoDich =
                await _context.Database.BeginTransactionAsync();

            try
            {
                decimal tongTien = 0;

                var donHang = new DonHang
                {
                    HoTen = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    DiaChi = model.DiaChi.Trim(),
                    GhiChu = string.IsNullOrWhiteSpace(model.GhiChu)
                        ? null
                        : model.GhiChu.Trim(),
                    PhuongThucThanhToan =
                        model.PhuongThucThanhToan,
                    TrangThai = "Chờ xác nhận",
                    NgayDat = DateTime.Now
                };

                foreach (GioHangItem item in gioHang)
                {
                    SanPham sanPham = sanPhams
                        .First(sp =>
                            sp.SanPhamID == item.SanPhamID);

                    decimal thanhTien =
                        sanPham.Gia * item.SoLuong;

                    tongTien += thanhTien;

                    donHang.ChiTietDonHangs.Add(
                        new ChiTietDonHang
                        {
                            SanPhamID = sanPham.SanPhamID,
                            TenSanPham = sanPham.TenSanPham,
                            DonGia = sanPham.Gia,
                            SoLuong = item.SoLuong,
                            ThanhTien = thanhTien
                        });

                    sanPham.SoLuongTon -= item.SoLuong;
                }

                donHang.TongTien = tongTien;

                _context.DonHangs.Add(donHang);

                await _context.SaveChangesAsync();
                await giaoDich.CommitAsync();

                HttpContext.Session.Remove(KhoaGioHang);

                HttpContext.Session.SetInt32(
                    KhoaDonHangVuaDat,
                    donHang.DonHangID);

                return RedirectToAction(
                    nameof(ThanhCong),
                    new
                    {
                        id = donHang.DonHangID
                    });
            }
            catch
            {
                await giaoDich.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể lưu đơn hàng. Vui lòng thử lại.");

                model.GioHang = gioHang;

                return View("Index", model);
            }
        }

        // Trang thông báo đặt hàng thành công
        [HttpGet]
        public async Task<IActionResult> ThanhCong(int id)
        {
            int? donHangVuaDat = HttpContext.Session
                .GetInt32(KhoaDonHangVuaDat);

            if (!donHangVuaDat.HasValue ||
                donHangVuaDat.Value != id)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            DonHang? donHang = await _context.DonHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(dh =>
                    dh.DonHangID == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        private List<GioHangItem> LayGioHang()
        {
            return HttpContext.Session
                .LayDoiTuong<List<GioHangItem>>(KhoaGioHang)
                ?? new List<GioHangItem>();
        }
    }
}