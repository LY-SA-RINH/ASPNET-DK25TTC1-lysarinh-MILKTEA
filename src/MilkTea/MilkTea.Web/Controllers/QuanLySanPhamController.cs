using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        // =====================================================
        // DANH SÁCH QUẢN LÝ SẢN PHẨM
        // =====================================================

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

        // =====================================================
        // THÊM SẢN PHẨM
        // =====================================================

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

            KiemTraGiaGoc(
                model.Gia,
                model.GiaGoc,
                nameof(model.GiaGoc));

            KiemTraHinhAnhTaiLen(
                model.HinhAnhTaiLen,
                nameof(model.HinhAnhTaiLen));

            if (!ModelState.IsValid)
            {
                await NapDanhMucVaoModel(model);

                return View(model);
            }

            string? tenTepHinhAnhMoi = null;
            string? duongDanHinhAnhMoi = null;

            try
            {
                if (model.HinhAnhTaiLen != null &&
                    model.HinhAnhTaiLen.Length > 0)
                {
                    var ketQuaLuuHinhAnh =
                        await LuuHinhAnhMoi(
                            model.HinhAnhTaiLen,
                            model.TenSanPham);

                    tenTepHinhAnhMoi =
                        ketQuaLuuHinhAnh.TenTep;

                    duongDanHinhAnhMoi =
                        ketQuaLuuHinhAnh.DuongDanDayDu;
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

                    HinhAnh = tenTepHinhAnhMoi,

                    // Chưa có chức năng khách hàng đánh giá.
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
                XoaTepHinhAnhMoiNeuCo(
                    duongDanHinhAnhMoi);

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm sản phẩm. Vui lòng thử lại.");

                await NapDanhMucVaoModel(model);

                return View(model);
            }
        }

        // =====================================================
        // SỬA SẢN PHẨM
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            SanPham? sanPham =
                await _context.SanPhams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sp =>
                        sp.SanPhamID == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var model = new SuaSanPhamViewModel
            {
                SanPhamID = sanPham.SanPhamID,
                DanhMucID = sanPham.DanhMucID,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                Gia = sanPham.Gia,
                GiaGoc = sanPham.GiaGoc,
                SoLuongTon = sanPham.SoLuongTon,
                NhanSanPham = sanPham.NhanSanPham,
                ThuTuHienThi = sanPham.ThuTuHienThi,
                TrangThai = sanPham.TrangThai,
                HinhAnhHienTai = sanPham.HinhAnh
            };

            await NapDanhMucVaoModel(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(
            SuaSanPhamViewModel model)
        {
            if (model.SanPhamID <= 0)
            {
                return NotFound();
            }

            SanPham? sanPham =
                await _context.SanPhams
                    .FirstOrDefaultAsync(sp =>
                        sp.SanPhamID == model.SanPhamID);

            if (sanPham == null)
            {
                return NotFound();
            }

            string? hinhAnhCu =
                sanPham.HinhAnh;

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
                        (
                            dm.TrangThai ||
                            dm.DanhMucID ==
                            sanPham.DanhMucID
                        ));

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
                        sp.SanPhamID != model.SanPhamID &&
                        sp.TenSanPham == model.TenSanPham);

            if (tenSanPhamDaTonTai)
            {
                ModelState.AddModelError(
                    nameof(model.TenSanPham),
                    "Tên sản phẩm này đã được sử dụng.");
            }

            KiemTraGiaGoc(
                model.Gia,
                model.GiaGoc,
                nameof(model.GiaGoc));

            KiemTraHinhAnhTaiLen(
                model.HinhAnhTaiLen,
                nameof(model.HinhAnhTaiLen));

            if (!ModelState.IsValid)
            {
                model.HinhAnhHienTai =
                    hinhAnhCu;

                await NapDanhMucVaoModel(model);

                return View(model);
            }

            string? tenTepHinhAnhMoi = null;
            string? duongDanHinhAnhMoi = null;

            try
            {
                if (model.HinhAnhTaiLen != null &&
                    model.HinhAnhTaiLen.Length > 0)
                {
                    var ketQuaLuuHinhAnh =
                        await LuuHinhAnhMoi(
                            model.HinhAnhTaiLen,
                            model.TenSanPham);

                    tenTepHinhAnhMoi =
                        ketQuaLuuHinhAnh.TenTep;

                    duongDanHinhAnhMoi =
                        ketQuaLuuHinhAnh.DuongDanDayDu;
                }

                sanPham.DanhMucID =
                    model.DanhMucID;

                sanPham.TenSanPham =
                    model.TenSanPham;

                sanPham.MoTa =
                    model.MoTa;

                sanPham.Gia =
                    model.Gia;

                sanPham.GiaGoc =
                    model.GiaGoc.HasValue &&
                    model.GiaGoc.Value > 0
                        ? model.GiaGoc
                        : null;

                sanPham.SoLuongTon =
                    model.SoLuongTon;

                sanPham.NhanSanPham =
                    model.NhanSanPham;

                sanPham.ThuTuHienThi =
                    model.ThuTuHienThi;

                sanPham.TrangThai =
                    model.TrangThai;

                // Chỉ thay hình khi Quản trị viên chọn hình mới.
                if (!string.IsNullOrWhiteSpace(
                        tenTepHinhAnhMoi))
                {
                    sanPham.HinhAnh =
                        tenTepHinhAnhMoi;
                }

                await _context.SaveChangesAsync();

                // Chỉ xóa hình cũ sau khi dữ liệu mới
                // đã được cập nhật thành công.
                if (!string.IsNullOrWhiteSpace(
                        tenTepHinhAnhMoi))
                {
                    await XoaHinhAnhCuNeuKhongConSuDung(
                        hinhAnhCu,
                        sanPham.SanPhamID);
                }

                TempData["ThanhCong"] =
                    $"Đã cập nhật sản phẩm “{sanPham.TenSanPham}” thành công.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                // Nếu cập nhật database thất bại,
                // xóa hình mới vừa được tải lên.
                XoaTepHinhAnhMoiNeuCo(
                    duongDanHinhAnhMoi);

                model.HinhAnhHienTai =
                    hinhAnhCu;

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật sản phẩm. Vui lòng thử lại.");

                await NapDanhMucVaoModel(model);

                return View(model);
            }
        }

        // =====================================================
        // MỞ BÁN HOẶC TẠM NGƯNG BÁN
        // =====================================================

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

            sanPham.TrangThai =
                trangThaiMoi;

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

        // =====================================================
        // KIỂM TRA GIÁ GỐC
        // =====================================================

        private void KiemTraGiaGoc(
            decimal gia,
            decimal? giaGoc,
            string tenThuocTinh)
        {
            if (giaGoc.HasValue &&
                giaGoc.Value > 0 &&
                giaGoc.Value <= gia)
            {
                ModelState.AddModelError(
                    tenThuocTinh,
                    "Giá gốc phải lớn hơn giá bán hoặc để trống.");
            }
        }

        // =====================================================
        // KIỂM TRA HÌNH ẢNH
        // =====================================================

        private void KiemTraHinhAnhTaiLen(
            IFormFile? hinhAnhTaiLen,
            string tenThuocTinh)
        {
            if (hinhAnhTaiLen == null ||
                hinhAnhTaiLen.Length <= 0)
            {
                return;
            }

            string duoiTep =
                Path.GetExtension(
                    hinhAnhTaiLen.FileName);

            if (!DuoiTepHinhAnhHopLe.Contains(
                    duoiTep))
            {
                ModelState.AddModelError(
                    tenThuocTinh,
                    "Chỉ chấp nhận hình ảnh JPG, JPEG, PNG hoặc WEBP.");
            }

            if (hinhAnhTaiLen.Length >
                KichThuocHinhAnhToiDa)
            {
                ModelState.AddModelError(
                    tenThuocTinh,
                    "Hình ảnh không được vượt quá 5 MB.");
            }

            if (string.IsNullOrWhiteSpace(
                    hinhAnhTaiLen.ContentType) ||
                !hinhAnhTaiLen.ContentType.StartsWith(
                    "image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    tenThuocTinh,
                    "Tệp được chọn không phải là hình ảnh hợp lệ.");
            }
        }

        // =====================================================
        // LƯU HÌNH ẢNH VỚI TÊN DỄ NHẬN BIẾT
        // =====================================================

        private async Task<(
            string TenTep,
            string DuongDanDayDu)> LuuHinhAnhMoi(
                IFormFile hinhAnhTaiLen,
                string tenSanPham)
        {
            string duoiTep =
                Path.GetExtension(
                    hinhAnhTaiLen.FileName)
                .ToLowerInvariant();

            string tenSanPhamKhongDau =
                ChuyenTenThanhSlug(
                    tenSanPham);

            if (string.IsNullOrWhiteSpace(
                    tenSanPhamKhongDau))
            {
                tenSanPhamKhongDau =
                    "san-pham";
            }

            // Giới hạn độ dài để tên tệp không quá dài.
            if (tenSanPhamKhongDau.Length > 80)
            {
                tenSanPhamKhongDau =
                    tenSanPhamKhongDau[..80]
                        .TrimEnd('-');
            }

            string thoiGian =
                DateTime.Now.ToString(
                    "yyyyMMdd-HHmmss");

            string maChongTrung =
                Guid.NewGuid()
                    .ToString("N")[..6];

            string tenTep =
                $"{tenSanPhamKhongDau}-{thoiGian}-{maChongTrung}{duoiTep}";

            string thuMucHinhAnh =
                Path.Combine(
                    _moiTruong.WebRootPath,
                    "images",
                    "products");

            Directory.CreateDirectory(
                thuMucHinhAnh);

            string duongDanDayDu =
                Path.Combine(
                    thuMucHinhAnh,
                    tenTep);

            await using FileStream tepMoi =
                new FileStream(
                    duongDanDayDu,
                    FileMode.CreateNew);

            await hinhAnhTaiLen.CopyToAsync(
                tepMoi);

            return (
                tenTep,
                duongDanDayDu);
        }

        // =====================================================
        // CHUYỂN TÊN SẢN PHẨM THÀNH TÊN TỆP KHÔNG DẤU
        // =====================================================

        private string ChuyenTenThanhSlug(
            string noiDung)
        {
            if (string.IsNullOrWhiteSpace(
                    noiDung))
            {
                return string.Empty;
            }

            string noiDungChuanHoa =
                noiDung.Trim()
                    .Normalize(
                        NormalizationForm.FormD);

            var ketQua =
                new StringBuilder();

            bool kyTuTruocLaDauGach =
                false;

            foreach (char kyTu in noiDungChuanHoa)
            {
                UnicodeCategory loaiKyTu =
                    CharUnicodeInfo.GetUnicodeCategory(
                        kyTu);

                // Bỏ dấu tiếng Việt.
                if (loaiKyTu ==
                    UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                char kyTuThuong =
                    char.ToLowerInvariant(
                        kyTu);

                // Chữ đ không bị tách khi Normalize,
                // nên cần chuyển riêng thành d.
                if (kyTuThuong == 'đ')
                {
                    kyTuThuong = 'd';
                }

                if (char.IsLetterOrDigit(
                        kyTuThuong))
                {
                    ketQua.Append(
                        kyTuThuong);

                    kyTuTruocLaDauGach =
                        false;
                }
                else if (!kyTuTruocLaDauGach)
                {
                    ketQua.Append('-');

                    kyTuTruocLaDauGach =
                        true;
                }
            }

            return ketQua
                .ToString()
                .Trim('-');
        }

        // =====================================================
        // XÓA HÌNH MỚI NẾU LƯU DATABASE THẤT BẠI
        // =====================================================

        private void XoaTepHinhAnhMoiNeuCo(
            string? duongDanHinhAnh)
        {
            if (string.IsNullOrWhiteSpace(
                    duongDanHinhAnh))
            {
                return;
            }

            try
            {
                if (System.IO.File.Exists(
                        duongDanHinhAnh))
                {
                    System.IO.File.Delete(
                        duongDanHinhAnh);
                }
            }
            catch
            {
                // Không làm gián đoạn luồng xử lý
                // nếu không thể xóa tệp vừa tải lên.
            }
        }

        // =====================================================
        // XÓA HÌNH CŨ NẾU KHÔNG CÒN ĐƯỢC SỬ DỤNG
        // =====================================================

        private async Task XoaHinhAnhCuNeuKhongConSuDung(
            string? hinhAnhCu,
            int sanPhamID)
        {
            if (string.IsNullOrWhiteSpace(
                    hinhAnhCu))
            {
                return;
            }

            string hinhAnhDaChuanHoa =
                hinhAnhCu.Replace(
                    '\\',
                    '/');

            string tenTepHinhAnhCu =
                Path.GetFileName(
                    hinhAnhDaChuanHoa);

            if (string.IsNullOrWhiteSpace(
                    tenTepHinhAnhCu))
            {
                return;
            }

            // Không xóa hình mặc định dùng chung.
            string[] cacHinhMacDinh =
            {
                "san-pham-mac-dinh.png",
                "san-pham-mac-dinh.jpg",
                "default-product.png",
                "no-image.png"
            };

            bool laHinhMacDinh =
                cacHinhMacDinh.Any(tenHinh =>
                    string.Equals(
                        tenHinh,
                        tenTepHinhAnhCu,
                        StringComparison.OrdinalIgnoreCase));

            if (laHinhMacDinh)
            {
                return;
            }

            List<string> cacHinhAnhKhac =
                await _context.SanPhams
                    .AsNoTracking()
                    .Where(sp =>
                        sp.SanPhamID != sanPhamID &&
                        sp.HinhAnh != null)
                    .Select(sp =>
                        sp.HinhAnh!)
                    .ToListAsync();

            bool dangDuocSanPhamKhacSuDung =
                cacHinhAnhKhac.Any(hinhAnh =>
                {
                    string hinhAnhKhacDaChuanHoa =
                        hinhAnh.Replace(
                            '\\',
                            '/');

                    string tenTepKhac =
                        Path.GetFileName(
                            hinhAnhKhacDaChuanHoa);

                    return string.Equals(
                        tenTepKhac,
                        tenTepHinhAnhCu,
                        StringComparison.OrdinalIgnoreCase);
                });

            if (dangDuocSanPhamKhacSuDung)
            {
                return;
            }

            string duongDanHinhAnhCu =
                Path.Combine(
                    _moiTruong.WebRootPath,
                    "images",
                    "products",
                    tenTepHinhAnhCu);

            try
            {
                if (System.IO.File.Exists(
                        duongDanHinhAnhCu))
                {
                    System.IO.File.Delete(
                        duongDanHinhAnhCu);
                }
            }
            catch
            {
                // Dữ liệu sản phẩm đã cập nhật thành công.
                // Không báo lỗi chỉ vì không xóa được hình cũ.
            }
        }

        // =====================================================
        // NẠP DANH MỤC CHO TRANG THÊM
        // =====================================================

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

        // =====================================================
        // NẠP DANH MỤC CHO TRANG SỬA
        // =====================================================

        private async Task NapDanhMucVaoModel(
            SuaSanPhamViewModel model)
        {
            model.DanhMucs =
                await _context.DanhMucs
                    .AsNoTracking()
                    .Where(dm =>
                        dm.TrangThai ||
                        dm.DanhMucID ==
                        model.DanhMucID)
                    .OrderBy(dm =>
                        dm.TenDanhMuc)
                    .ToListAsync();
        }
    }
}