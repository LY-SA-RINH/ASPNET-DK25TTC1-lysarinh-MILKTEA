using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class BaoCaoDoanhThuController : Controller
    {
        private readonly MilkTeaDbContext _context;

        public BaoCaoDoanhThuController(
            MilkTeaDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // BÁO CÁO DOANH THU CHI TIẾT
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? tuNgay,
            DateTime? denNgay)
        {
            DateTime homNay =
                DateTime.Today;

            DateTime ngayDauThang =
                new DateTime(
                    homNay.Year,
                    homNay.Month,
                    1);

            // Khi mở trang lần đầu:
            // mặc định xem từ đầu tháng đến hôm nay.
            DateTime tuNgayDaChon =
                (tuNgay ?? ngayDauThang).Date;

            DateTime denNgayDaChon =
                (denNgay ?? homNay).Date;

            var model = new BaoCaoDoanhThuViewModel
            {
                TuNgay = tuNgayDaChon,
                DenNgay = denNgayDaChon
            };

            // Kiểm tra khoảng ngày hợp lệ.
            if (tuNgayDaChon > denNgayDaChon)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Từ ngày không được lớn hơn đến ngày.");

                return View(model);
            }

            // Dùng ngày kế tiếp để lấy trọn vẹn dữ liệu
            // của ngày kết thúc đến 23:59:59.
            DateTime ngayKetThucKhongBaoGom =
                denNgayDaChon.AddDays(1);

            List<DonHang> donHangs =
                await _context.DonHangs
                    .AsNoTracking()
                    .Where(dh =>
                        dh.TrangThai == "Hoàn thành" &&
                        dh.NgayDat >= tuNgayDaChon &&
                        dh.NgayDat < ngayKetThucKhongBaoGom)
                    .OrderByDescending(dh =>
                        dh.NgayDat)
                    .ThenByDescending(dh =>
                        dh.DonHangID)
                    .ToListAsync();

            model.DonHangs =
                donHangs;

            model.TongDonHangHoanThanh =
                donHangs.Count;

            model.TongDoanhThu =
                donHangs.Sum(dh =>
                    dh.TongTien);

            model.GiaTriDonTrungBinh =
                donHangs.Count > 0
                    ? donHangs.Average(dh =>
                        dh.TongTien)
                    : 0m;

            model.DonHangGiaTriCaoNhat =
                donHangs.Count > 0
                    ? donHangs.Max(dh =>
                        dh.TongTien)
                    : 0m;

            return View(model);
        }
    }
}