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
        private const string TrangThaiChoXacNhan =
            "Chờ xác nhận";

        private const string TrangThaiDaXacNhan =
            "Đã xác nhận";

        private const string TrangThaiDangGiao =
            "Đang giao";

        private const string TrangThaiHoanThanh =
            "Hoàn thành";

        private const string TrangThaiDaHuy =
            "Đã hủy";

        private const string TrangThaiLienHeChuaXuLy =
            "Chưa xử lý";

        private readonly MilkTeaDbContext _context;

        public QuanLyTongQuanController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model =
                new TongQuanQuanTriViewModel
                {
                    // Sản phẩm
                    TongSanPham =
                        await _context.SanPhams
                            .AsNoTracking()
                            .CountAsync(),

                    TongSanPhamDangBan =
                        await _context.SanPhams
                            .AsNoTracking()
                            .CountAsync(sanPham =>
                                sanPham.TrangThai),

                    TongSanPhamNgungBan =
                        await _context.SanPhams
                            .AsNoTracking()
                            .CountAsync(sanPham =>
                                !sanPham.TrangThai),

                    TongSanPhamHetHang =
                        await _context.SanPhams
                            .AsNoTracking()
                            .CountAsync(sanPham =>
                                sanPham.SoLuongTon <= 0),

                    // Đơn hàng
                    TongDonHang =
                        await _context.DonHangs
                            .AsNoTracking()
                            .CountAsync(),

                    TongDonHangChoXacNhan =
                        await _context.DonHangs
                            .AsNoTracking()
                            .CountAsync(donHang =>
                                donHang.TrangThai ==
                                TrangThaiChoXacNhan),

                    TongDonHangDaXacNhan =
                        await _context.DonHangs
                            .AsNoTracking()
                            .CountAsync(donHang =>
                                donHang.TrangThai ==
                                TrangThaiDaXacNhan),

                    TongDonHangDangGiao =
                        await _context.DonHangs
                            .AsNoTracking()
                            .CountAsync(donHang =>
                                donHang.TrangThai ==
                                TrangThaiDangGiao),

                    TongDonHangHoanThanh =
                        await _context.DonHangs
                            .AsNoTracking()
                            .CountAsync(donHang =>
                                donHang.TrangThai ==
                                TrangThaiHoanThanh),

                    TongDonHangDaHuy =
                        await _context.DonHangs
                            .AsNoTracking()
                            .CountAsync(donHang =>
                                donHang.TrangThai ==
                                TrangThaiDaHuy),

                    DoanhThuDonHoanThanh =
                        await _context.DonHangs
                            .AsNoTracking()
                            .Where(donHang =>
                                donHang.TrangThai ==
                                TrangThaiHoanThanh)
                            .SumAsync(donHang =>
                                (decimal?)donHang.TongTien)
                        ?? 0m,

                    // Liên hệ
                    TongLienHeChuaXuLy =
                        await _context.LienHes
                            .AsNoTracking()
                            .CountAsync(lienHe =>
                                lienHe.TrangThai ==
                                TrangThaiLienHeChuaXuLy),

                    // Năm đơn hàng mới nhất
                    DonHangMoiNhat =
                        await _context.DonHangs
                            .AsNoTracking()
                            .OrderByDescending(donHang =>
                                donHang.NgayDat)
                            .ThenByDescending(donHang =>
                                donHang.DonHangID)
                            .Take(5)
                            .ToListAsync(),

                    // Sản phẩm đang bán còn từ 1 đến 10 sản phẩm
                    SanPhamSapHetHang =
                        await _context.SanPhams
                            .AsNoTracking()
                            .Where(sanPham =>
                                sanPham.TrangThai &&
                                sanPham.SoLuongTon > 0 &&
                                sanPham.SoLuongTon <= 10)
                            .OrderBy(sanPham =>
                                sanPham.SoLuongTon)
                            .ThenBy(sanPham =>
                                sanPham.ThuTuHienThi)
                            .ThenBy(sanPham =>
                                sanPham.SanPhamID)
                            .Take(8)
                            .ToListAsync()
                };

            return View(model);
        }
    }
}
