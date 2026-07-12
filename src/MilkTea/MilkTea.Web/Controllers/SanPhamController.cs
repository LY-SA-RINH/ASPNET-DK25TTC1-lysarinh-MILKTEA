using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;

namespace MilkTea.Web.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly MilkTeaDbContext _context;

        private const int SoSanPhamMoiTrang = 8;

        public SanPhamController(MilkTeaDbContext context)
        {
            _context = context;
        }

        // Trang danh sách tất cả sản phẩm có phân trang
        public async Task<IActionResult> Index(int trang = 1)
        {
            if (trang < 1)
            {
                trang = 1;
            }

            var truyVanSanPham = _context.SanPhams
                .AsNoTracking()
                .Where(sp => sp.TrangThai)
                .OrderBy(sp => sp.ThuTuHienThi)
                .ThenBy(sp => sp.SanPhamID);

            int tongSoSanPham = await truyVanSanPham.CountAsync();

            int tongSoTrang = tongSoSanPham == 0
                ? 0
                : (int)Math.Ceiling(
                    tongSoSanPham / (double)SoSanPhamMoiTrang);

            if (tongSoTrang > 0 && trang > tongSoTrang)
            {
                trang = tongSoTrang;
            }

            var sanPhams = await truyVanSanPham
                .Skip((trang - 1) * SoSanPhamMoiTrang)
                .Take(SoSanPhamMoiTrang)
                .ToListAsync();

            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TongSoSanPham = tongSoSanPham;

            return View(sanPhams);
        }

        // Trang chi tiết một sản phẩm
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(sp =>
                    sp.SanPhamID == id &&
                    sp.TrangThai);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // Trang kiểm tra database mà website đang kết nối thật sự
        [HttpGet]
        public async Task<IActionResult> KiemTraDuLieu()
        {
            var ketNoi = _context.Database.GetDbConnection();

            int tongTatCaSanPham = await _context.SanPhams
                .AsNoTracking()
                .CountAsync();

            int tongSanPhamDangBan = await _context.SanPhams
                .AsNoTracking()
                .CountAsync(sp => sp.TrangThai);

            var duLieuMau = await _context.SanPhams
                .AsNoTracking()
                .OrderBy(sp => sp.SanPhamID)
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
                MayChuSqlServer = ketNoi.DataSource,
                TenDatabase = ketNoi.Database,
                TongTatCaSanPham = tongTatCaSanPham,
                TongSanPhamDangBan = tongSanPhamDangBan,
                DuLieuMau = duLieuMau
            });
        }
    }
}