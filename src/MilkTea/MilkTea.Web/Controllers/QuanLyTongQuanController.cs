using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class QuanLyTongQuanController : Controller
    {
        private readonly MilkTeaDbContext _context;

        public QuanLyTongQuanController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // TRANG TỔNG QUAN QUẢN TRỊ
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IQueryable<SanPham> truyVanSanPham =
                _context.SanPhams
                    .AsNoTracking();

            IQueryable<DonHang> truyVanDonHang =
                _context.DonHangs
                    .AsNoTracking();

            var model = new TongQuanQuanTriViewModel
            {
                // =================================================
                // THỐNG KÊ SẢN PHẨM
                // =================================================

                TongSanPham =
                    await truyVanSanPham
                        .CountAsync(),

                TongSanPhamDangBan =
                    await truyVanSanPham
                        .CountAsync(sp =>
                            sp.TrangThai),

                TongSanPhamNgungBan =
                    await truyVanSanPham
                        .CountAsync(sp =>
                            !sp.TrangThai),

                TongSanPhamHetHang =
                    await truyVanSanPham
                        .CountAsync(sp =>
                            sp.SoLuongTon <= 0),

                // =================================================
                // THỐNG KÊ ĐƠN HÀNG
                // =================================================

                TongDonHang =
                    await truyVanDonHang
                        .CountAsync(),

                TongDonHangChoXacNhan =
                    await truyVanDonHang
                        .CountAsync(dh =>
                            dh.TrangThai == "Chờ xác nhận"),

                TongDonHangDaXacNhan =
                    await truyVanDonHang
                        .CountAsync(dh =>
                            dh.TrangThai == "Đã xác nhận"),

                TongDonHangDangGiao =
                    await truyVanDonHang
                        .CountAsync(dh =>
                            dh.TrangThai == "Đang giao"),

                TongDonHangHoanThanh =
                    await truyVanDonHang
                        .CountAsync(dh =>
                            dh.TrangThai == "Hoàn thành"),

                TongDonHangDaHuy =
                    await truyVanDonHang
                        .CountAsync(dh =>
                            dh.TrangThai == "Đã hủy"),

                // =================================================
                // DOANH THU
                // Chỉ tính đơn hàng đã hoàn thành.
                // =================================================

                DoanhThuDonHoanThanh =
                    await truyVanDonHang
                        .Where(dh =>
                            dh.TrangThai == "Hoàn thành")
                        .SumAsync(dh =>
                            (decimal?)dh.TongTien)
                    ?? 0m,

                // =================================================
                // 5 ĐƠN HÀNG MỚI NHẤT
                // =================================================

                DonHangMoiNhat =
                    await truyVanDonHang
                        .OrderByDescending(dh =>
                            dh.NgayDat)
                        .ThenByDescending(dh =>
                            dh.DonHangID)
                        .Take(5)
                        .ToListAsync(),

                // =================================================
                // SẢN PHẨM SẮP HẾT HÀNG
                // Đang bán và còn từ 1 đến 5 sản phẩm.
                // =================================================

                SanPhamSapHetHang =
                    await truyVanSanPham
                        .Where(sp =>
                            sp.TrangThai &&
                            sp.SoLuongTon > 0 &&
                            sp.SoLuongTon <= 5)
                        .OrderBy(sp =>
                            sp.SoLuongTon)
                        .ThenBy(sp =>
                            sp.TenSanPham)
                        .Take(5)
                        .ToListAsync()
            };

            return View(model);
        }
    }
}