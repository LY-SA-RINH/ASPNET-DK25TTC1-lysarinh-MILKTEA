using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly MilkTeaDbContext _context;

        private const int SoSanPhamMoiTrang = 8;

        public SanPhamController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        // Danh sách sản phẩm, tìm kiếm và phân trang
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int trang = 1)
        {
            if (trang < 1)
            {
                trang = 1;
            }

            tuKhoa = string.IsNullOrWhiteSpace(tuKhoa)
                ? null
                : tuKhoa.Trim();

            // Giới hạn độ dài từ khóa để URL và truy vấn gọn hơn.
            if (tuKhoa != null &&
                tuKhoa.Length > 100)
            {
                tuKhoa = tuKhoa[..100];
            }

            // Hiển thị cả sản phẩm đang bán
            // và sản phẩm tạm ngưng bán.
            IQueryable<SanPham> truyVanSanPham =
                _context.SanPhams
                    .AsNoTracking();

            // Tìm theo tên hoặc mô tả sản phẩm.
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                truyVanSanPham =
                    truyVanSanPham.Where(sp =>
                        sp.TenSanPham.Contains(tuKhoa) ||
                        (
                            sp.MoTa != null &&
                            sp.MoTa.Contains(tuKhoa)
                        ));
            }

            int tongSoSanPham =
                await truyVanSanPham.CountAsync();

            int tongSanPhamDangBan =
                await truyVanSanPham
                    .CountAsync(sp =>
                        sp.TrangThai);

            int tongSoTrang = tongSoSanPham == 0
                ? 0
                : (int)Math.Ceiling(
                    tongSoSanPham /
                    (double)SoSanPhamMoiTrang);

            if (tongSoTrang > 0 &&
                trang > tongSoTrang)
            {
                trang = tongSoTrang;
            }

            List<SanPham> sanPhams =
                await truyVanSanPham
                    .OrderByDescending(sp =>
                        sp.TrangThai)
                    .ThenBy(sp =>
                        sp.ThuTuHienThi)
                    .ThenBy(sp =>
                        sp.SanPhamID)
                    .Skip(
                        (trang - 1) *
                        SoSanPhamMoiTrang)
                    .Take(SoSanPhamMoiTrang)
                    .ToListAsync();

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TongSoSanPham = tongSoSanPham;
            ViewBag.TongSanPhamDangBan =
                tongSanPhamDangBan;

            return View(sanPhams);
        }

        // Trang chi tiết một sản phẩm
        [HttpGet]
        public async Task<IActionResult> ChiTiet(
            int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            // Cho phép xem cả sản phẩm tạm ngưng bán.
            SanPham? sanPham =
                await _context.SanPhams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sp =>
                        sp.SanPhamID == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // Trang kiểm tra database
        [HttpGet]
        public async Task<IActionResult> KiemTraDuLieu()
        {
            var ketNoi =
                _context.Database.GetDbConnection();

            int tongTatCaSanPham =
                await _context.SanPhams
                    .AsNoTracking()
                    .CountAsync();

            int tongSanPhamDangBan =
                await _context.SanPhams
                    .AsNoTracking()
                    .CountAsync(sp =>
                        sp.TrangThai);

            var duLieuMau =
                await _context.SanPhams
                    .AsNoTracking()
                    .OrderBy(sp =>
                        sp.SanPhamID)
                    .Select(sp => new
                    {
                        sp.SanPhamID,
                        sp.TenSanPham,
                        sp.TrangThai,
                        sp.HinhAnh
                    })
                    .Take(10)
                    .ToListAsync();

            return Json(new
            {
                MayChuSqlServer =
                    ketNoi.DataSource,

                TenDatabase =
                    ketNoi.Database,

                TongTatCaSanPham =
                    tongTatCaSanPham,

                TongSanPhamDangBan =
                    tongSanPhamDangBan,

                DuLieuMau =
                    duLieuMau
            });
        }
    }
}