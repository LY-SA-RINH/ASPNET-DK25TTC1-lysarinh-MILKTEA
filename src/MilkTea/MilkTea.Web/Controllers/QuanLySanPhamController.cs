using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class QuanLySanPhamController : Controller
    {
        private const int SoSanPhamMoiTrang = 10;

        private const long KichThuocHinhAnhToiDa =
            5 * 1024 * 1024;

        private static readonly HashSet<string>
            DuoiTepHinhAnhHopLe =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };

        private readonly MilkTeaDbContext _context;
        private readonly IWebHostEnvironment _moiTruong;

        public QuanLySanPhamController(
            MilkTeaDbContext context,
            IWebHostEnvironment moiTruong)
        {
            _context = context;
            _moiTruong = moiTruong;
        }

        // Danh sách quản lý sản phẩm
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? danhMucID,
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

            if (trangThai != "dang-ban" &&
                trangThai != "ngung-ban")
            {
                trangThai = null;
            }

            IQueryable<SanPham> truyVan =
                _context.SanPhams
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                truyVan = truyVan.Where(sp =>
                    sp.TenSanPham.Contains(tuKhoa));
            }

            if (danhMucID.HasValue &&
                danhMucID.Value > 0)
            {
                truyVan = truyVan.Where(sp =>
                    sp.DanhMucID == danhMucID.Value);
            }

            if (trangThai == "dang-ban")
            {
                truyVan = truyVan.Where(sp =>
                    sp.TrangThai);
            }
            else if (trangThai == "ngung-ban")
            {
                truyVan = truyVan.Where(sp =>
                    !sp.TrangThai);
            }

            int tongKetQua =
                await truyVan.CountAsync();

            int tongSoTrang = tongKetQua == 0
                ? 0
                : (int)Math.Ceiling(
                    tongKetQua /
                    (double)SoSanPhamMoiTrang);

            if (tongSoTrang > 0 &&
                trang > tongSoTrang)
            {
                trang = tongSoTrang;
            }

            List<SanPham> sanPhams =
                await truyVan
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

            List<DanhMuc> danhMucs =
                await _context.DanhMucs
                    .AsNoTracking()
                    .OrderBy(dm =>
                        dm.TenDanhMuc)
                    .ToListAsync();

            Dictionary<int, string> tenDanhMucTheoID =
                danhMucs.ToDictionary(
                    dm => dm.DanhMucID,
                    dm => dm.TenDanhMuc);

            var model = new QuanLySanPhamViewModel
            {
                SanPhams = sanPhams,
                DanhMucs = danhMucs,
                TenDanhMucTheoID = tenDanhMucTheoID,

                TuKhoa = tuKhoa,
                DanhMucID = danhMucID,
                TrangThai = trangThai,

                TrangHienTai = trang,
                TongSoTrang = tongSoTrang,
                TongKetQua = tongKetQua,

                TongTatCaSanPham =
                    await _context.SanPhams
                        .AsNoTracking()
                        .CountAsync(),

                TongSanPhamDangBan =
                    await _context.SanPhams
                        .AsNoTracking()
                        .CountAsync(sp =>
                            sp.TrangThai),

                TongSanPhamNgungBan =
                    await _context.SanPhams
                        .AsNoTracking()
                        .CountAsync(sp =>
                            !sp.TrangThai),

                TongSanPhamHetHang =
                    await _context.SanPhams
                        .AsNoTracking()
                        .CountAsync(sp =>
                            sp.SoLuongTon <= 0)
            };

            return View(model);
        }

        // Trang thêm sản phẩm
        [HttpGet]
        public async Task<IActionResult> Them()
        {
            int thuTuLonNhat =
                await _context.SanPhams
                    .AsNoTracking()
                    .Select(sp =>
                        (int?)sp.ThuTuHienThi)
                    .MaxAsync()
                ?? 0;

            var model = new ThemSanPhamViewModel
            {
                ThuTuHienThi = thuTuLonNhat + 1,
                SoLuongTon = 0,
                NhanSanPham = 0,
                TrangThai = true
            };

            await NapDanhMucVaoModel(model);

            return View(model);
        }

        // Xử lý thêm sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(
            ThemSanPhamViewModel model)
        {
            model.TenSanPham =
                model.TenSanPham?.Trim()
                ?? string.Empty;

            model.MoTa =
                string.IsNullOrWhiteSpace(model.MoTa)
                    ? null
                    : model.MoTa.Trim();

            bool danhMucHopLe =
                await _context.DanhMucs
                    .AsNoTracking()
                    .AnyAsync(dm =>
                        dm.DanhMucID == model.DanhMucID &&
                        dm.TrangThai);

            if (!danhMucHopLe)
            {
                ModelState.AddModelError(
                    nameof(model.DanhMucID),
                    "Danh mục không tồn tại hoặc đang tạm ngưng.");
            }

            bool tenSanPhamDaTonTai =
                await _context.SanPhams
                    .AsNoTracking()
                    .AnyAsync(sp =>
                        sp.TenSanPham == model.TenSanPham);

            if (tenSanPhamDaTonTai)
            {
                ModelState.AddModelError(
                    nameof(model.TenSanPham),
                    "Tên sản phẩm này đã tồn tại.");
            }

            if (model.GiaGoc.HasValue &&
                model.GiaGoc.Value > 0 &&
                model.GiaGoc.Value <= model.Gia)
            {
                ModelState.AddModelError(
                    nameof(model.GiaGoc),
                    "Giá gốc phải lớn hơn giá bán hoặc để trống.");
            }

            if (model.HinhAnhTaiLen != null &&
                model.HinhAnhTaiLen.Length > 0)
            {
                string duoiTep =
                    Path.GetExtension(
                        model.HinhAnhTaiLen.FileName);

                if (!DuoiTepHinhAnhHopLe.Contains(duoiTep))
                {
                    ModelState.AddModelError(
                        nameof(model.HinhAnhTaiLen),
                        "Chỉ chấp nhận hình ảnh JPG, JPEG, PNG hoặc WEBP.");
                }

                if (model.HinhAnhTaiLen.Length >
                    KichThuocHinhAnhToiDa)
                {
                    ModelState.AddModelError(
                        nameof(model.HinhAnhTaiLen),
                        "Hình ảnh không được vượt quá 5 MB.");
                }

                if (string.IsNullOrWhiteSpace(
                        model.HinhAnhTaiLen.ContentType) ||
                    !model.HinhAnhTaiLen.ContentType.StartsWith(
                        "image/",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        nameof(model.HinhAnhTaiLen),
                        "Tệp được chọn không phải là hình ảnh hợp lệ.");
                }
            }

            if (!ModelState.IsValid)
            {
                await NapDanhMucVaoModel(model);

                return View(model);
            }

            string? tenTepHinhAnh = null;
            string? duongDanHinhAnhDaLuu = null;

            try
            {
                if (model.HinhAnhTaiLen != null &&
                    model.HinhAnhTaiLen.Length > 0)
                {
                    string duoiTep =
                        Path.GetExtension(
                            model.HinhAnhTaiLen.FileName)
                        .ToLowerInvariant();

                    tenTepHinhAnh =
                        $"san-pham-{Guid.NewGuid():N}{duoiTep}";

                    string thuMucHinhAnh =
                        Path.Combine(
                            _moiTruong.WebRootPath,
                            "images",
                            "products");

                    Directory.CreateDirectory(
                        thuMucHinhAnh);

                    duongDanHinhAnhDaLuu =
                        Path.Combine(
                            thuMucHinhAnh,
                            tenTepHinhAnh);

                    await using FileStream tepMoi =
                        new FileStream(
                            duongDanHinhAnhDaLuu,
                            FileMode.CreateNew);

                    await model.HinhAnhTaiLen
                        .CopyToAsync(tepMoi);
                }

                var sanPham = new SanPham
                {
                    DanhMucID = model.DanhMucID,
                    TenSanPham = model.TenSanPham,
                    MoTa = model.MoTa,
                    Gia = model.Gia,

                    GiaGoc =
                        model.GiaGoc.HasValue &&
                        model.GiaGoc.Value > 0
                            ? model.GiaGoc
                            : null,

                    HinhAnh = tenTepHinhAnh,

                    // Chưa có chức năng khách hàng đánh giá,
                    // sản phẩm mới tạm mặc định 5 sao.
                    DanhGia = 5.00m,

                    SoLuongTon = model.SoLuongTon,
                    NhanSanPham = model.NhanSanPham,
                    ThuTuHienThi = model.ThuTuHienThi,
                    TrangThai = model.TrangThai,
                    NgayTao = DateTime.Now
                };

                _context.SanPhams.Add(sanPham);

                await _context.SaveChangesAsync();

                TempData["ThanhCong"] =
                    $"Đã thêm sản phẩm “{sanPham.TenSanPham}” thành công.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(
                        duongDanHinhAnhDaLuu) &&
                    System.IO.File.Exists(
                        duongDanHinhAnhDaLuu))
                {
                    System.IO.File.Delete(
                        duongDanHinhAnhDaLuu);
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm sản phẩm. Vui lòng thử lại.");

                await NapDanhMucVaoModel(model);

                return View(model);
            }
        }

        // Ngừng bán hoặc mở bán lại sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string thaoTac,
            string? tuKhoa,
            int? danhMucID)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            SanPham? sanPham =
                await _context.SanPhams
                    .FirstOrDefaultAsync(sp =>
                        sp.SanPhamID == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            bool trangThaiMoi;

            if (thaoTac == "mo-ban")
            {
                trangThaiMoi = true;
            }
            else if (thaoTac == "ngung-ban")
            {
                trangThaiMoi = false;
            }
            else
            {
                TempData["Loi"] =
                    "Thao tác cập nhật trạng thái sản phẩm không hợp lệ.";

                return RedirectToAction(
                    nameof(Index));
            }

            sanPham.TrangThai = trangThaiMoi;

            await _context.SaveChangesAsync();

            TempData["ThanhCong"] =
                trangThaiMoi
                    ? $"Đã mở bán lại sản phẩm “{sanPham.TenSanPham}”."
                    : $"Đã ngừng bán sản phẩm “{sanPham.TenSanPham}”.";

            string boLocTrangThai =
                trangThaiMoi
                    ? "dang-ban"
                    : "ngung-ban";

            return RedirectToAction(
                nameof(Index),
                new
                {
                    tuKhoa,
                    danhMucID,
                    trangThai = boLocTrangThai,
                    trang = 1
                });
        }

        // Nạp danh mục đang hoạt động vào biểu mẫu
        private async Task NapDanhMucVaoModel(
            ThemSanPhamViewModel model)
        {
            model.DanhMucs =
                await _context.DanhMucs
                    .AsNoTracking()
                    .Where(dm =>
                        dm.TrangThai)
                    .OrderBy(dm =>
                        dm.TenDanhMuc)
                    .ToListAsync();
        }
    }
}