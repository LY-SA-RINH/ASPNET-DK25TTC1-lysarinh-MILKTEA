using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkTea.Web.Data;
using MilkTea.Web.Models;

namespace MilkTea.Web.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly IWebHostEnvironment _moiTruong;

        public TaiKhoanController(
            UserManager<NguoiDung> userManager,
            SignInManager<NguoiDung> signInManager,
            IWebHostEnvironment moiTruong)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _moiTruong = moiTruong;
        }

        // Trang đăng ký
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DangKy()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        // Xử lý đăng ký
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(
            DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email
                .Trim()
                .ToLowerInvariant();

            NguoiDung? taiKhoanDaTonTai =
                await _userManager.FindByEmailAsync(email);

            if (taiKhoanDaTonTai != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Email này đã được sử dụng.");

                return View(model);
            }

            var nguoiDung = new NguoiDung
            {
                HoTen = model.HoTen.Trim(),
                UserName = email,
                Email = email,
                PhoneNumber = model.SoDienThoai.Trim(),
                EmailConfirmed = true,
                NgayTao = DateTime.Now
            };

            IdentityResult ketQua =
                await _userManager.CreateAsync(
                    nguoiDung,
                    model.MatKhau);

            if (!ketQua.Succeeded)
            {
                ThemLoiIdentityVaoModelState(ketQua);

                return View(model);
            }

            IdentityResult ketQuaVaiTro =
                await _userManager.AddToRoleAsync(
                    nguoiDung,
                    KhoiTaoTaiKhoan.KhachHang);

            if (!ketQuaVaiTro.Succeeded)
            {
                await _userManager.DeleteAsync(nguoiDung);

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể tạo quyền khách hàng.");

                return View(model);
            }

            await _signInManager.SignInAsync(
                nguoiDung,
                isPersistent: false);

            TempData["ThanhCong"] =
                "Đăng ký tài khoản thành công.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // Trang đăng nhập
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DangNhap(
            string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View(
                new DangNhapViewModel
                {
                    ReturnUrl = returnUrl
                });
        }

        // Xử lý đăng nhập
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(
            DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email
                .Trim()
                .ToLowerInvariant();

            NguoiDung? nguoiDung =
                await _userManager.FindByEmailAsync(email);

            if (nguoiDung == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email hoặc mật khẩu không chính xác.");

                return View(model);
            }

            var ketQua =
                await _signInManager.PasswordSignInAsync(
                    nguoiDung,
                    model.MatKhau,
                    model.GhiNhoDangNhap,
                    lockoutOnFailure: true);

            if (ketQua.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tài khoản đang tạm khóa do nhập sai " +
                    "mật khẩu quá nhiều lần.");

                return View(model);
            }

            if (!ketQua.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email hoặc mật khẩu không chính xác.");

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            bool laNhanVien =
                await _userManager.IsInRoleAsync(
                    nguoiDung,
                    KhoiTaoTaiKhoan.NhanVien);

            bool laQuanTriVien =
                await _userManager.IsInRoleAsync(
                    nguoiDung,
                    KhoiTaoTaiKhoan.QuanTriVien);

            if (laNhanVien || laQuanTriVien)
            {
                return RedirectToAction(
                    "Index",
                    "QuanLyDonHang");
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

        // Trang nhập email khi quên mật khẩu
        [AllowAnonymous]
        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        // Xử lý yêu cầu quên mật khẩu
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(
            QuenMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email
                .Trim()
                .ToLowerInvariant();

            NguoiDung? nguoiDung =
                await _userManager.FindByEmailAsync(email);

            if (nguoiDung != null)
            {
                string maXacNhan =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(
                            nguoiDung);

                string? lienKetDatLai = Url.Action(
                    action: nameof(DatLaiMatKhau),
                    controller: "TaiKhoan",
                    values: new
                    {
                        email,
                        maXacNhan
                    },
                    protocol: Request.Scheme);

                // Chỉ hiển thị liên kết khi chạy local.
                // Khi triển khai thật, liên kết này phải gửi qua email.
                if (_moiTruong.IsDevelopment() &&
                    !string.IsNullOrWhiteSpace(lienKetDatLai))
                {
                    TempData["LienKetDatLaiMatKhau"] =
                        lienKetDatLai;
                }
            }

            // Luôn chuyển đến cùng một trang để không tiết lộ
            // email có tồn tại trong hệ thống hay không.
            return RedirectToAction(
                nameof(QuenMatKhauXacNhan));
        }

        // Trang thông báo tiếp nhận yêu cầu
        [AllowAnonymous]
        [HttpGet]
        public IActionResult QuenMatKhauXacNhan()
        {
            return View();
        }

        // Trang nhập mật khẩu mới
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DatLaiMatKhau(
            string? email,
            string? maXacNhan)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(maXacNhan))
            {
                TempData["Loi"] =
                    "Liên kết đặt lại mật khẩu không hợp lệ.";

                return RedirectToAction(
                    nameof(DangNhap));
            }

            var model = new DatLaiMatKhauViewModel
            {
                Email = email,
                MaXacNhan = maXacNhan
            };

            return View(model);
        }

        // Xử lý đặt lại mật khẩu
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLaiMatKhau(
            DatLaiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email
                .Trim()
                .ToLowerInvariant();

            NguoiDung? nguoiDung =
                await _userManager.FindByEmailAsync(email);

            if (nguoiDung == null)
            {
                return RedirectToAction(
                    nameof(DatLaiMatKhauThanhCong));
            }

            IdentityResult ketQua =
                await _userManager.ResetPasswordAsync(
                    nguoiDung,
                    model.MaXacNhan,
                    model.MatKhauMoi);

            if (!ketQua.Succeeded)
            {
                ThemLoiIdentityVaoModelState(ketQua);

                return View(model);
            }

            // Mở khóa nếu tài khoản từng bị khóa.
            await _userManager.SetLockoutEndDateAsync(
                nguoiDung,
                null);

            await _userManager.ResetAccessFailedCountAsync(
                nguoiDung);

            // Đăng xuất phiên đăng nhập hiện tại.
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                nameof(DatLaiMatKhauThanhCong));
        }

        // Trang thông báo đặt lại mật khẩu thành công
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DatLaiMatKhauThanhCong()
        {
            return View();
        }

        // Trang thông tin cá nhân
        [Authorize(Roles = "KhachHang")]
        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            NguoiDung? nguoiDung =
                await _userManager.GetUserAsync(User);

            if (nguoiDung == null)
            {
                return Challenge();
            }

            var model = new ThongTinCaNhanViewModel
            {
                Email = nguoiDung.Email ?? string.Empty,
                HoTen = nguoiDung.HoTen,
                SoDienThoai = nguoiDung.PhoneNumber
                    ?? string.Empty,
                DiaChi = nguoiDung.DiaChi
                    ?? string.Empty
            };

            return View(model);
        }

        // Xử lý cập nhật thông tin cá nhân
        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinCaNhan(
            ThongTinCaNhanViewModel model)
        {
            NguoiDung? nguoiDung =
                await _userManager.GetUserAsync(User);

            if (nguoiDung == null)
            {
                return Challenge();
            }

            // Email không được cập nhật từ biểu mẫu này.
            model.Email = nguoiDung.Email ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            nguoiDung.HoTen = model.HoTen.Trim();
            nguoiDung.PhoneNumber =
                model.SoDienThoai.Trim();
            nguoiDung.DiaChi =
                model.DiaChi.Trim();

            IdentityResult ketQua =
                await _userManager.UpdateAsync(nguoiDung);

            if (!ketQua.Succeeded)
            {
                ThemLoiIdentityVaoModelState(ketQua);

                return View(model);
            }

            // Làm mới thông tin phiên đăng nhập hiện tại.
            await _signInManager.RefreshSignInAsync(
                nguoiDung);

            TempData["ThanhCong"] =
                "Cập nhật thông tin cá nhân thành công.";

            return RedirectToAction(
                nameof(ThongTinCaNhan));
        }

        // Trang đổi mật khẩu khi đang đăng nhập
        [Authorize(Roles = "KhachHang")]
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View(new DoiMatKhauViewModel());
        }

        // Xử lý đổi mật khẩu khi đang đăng nhập
        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(
            DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            NguoiDung? nguoiDung =
                await _userManager.GetUserAsync(User);

            if (nguoiDung == null)
            {
                return Challenge();
            }

            IdentityResult ketQua =
                await _userManager.ChangePasswordAsync(
                    nguoiDung,
                    model.MatKhauHienTai,
                    model.MatKhauMoi);

            if (!ketQua.Succeeded)
            {
                ThemLoiIdentityVaoModelState(ketQua);

                return View(model);
            }

            // ChangePasswordAsync thay đổi SecurityStamp.
            // Làm mới phiên để khách hàng vẫn tiếp tục đăng nhập.
            await _signInManager.RefreshSignInAsync(
                nguoiDung);

            TempData["ThanhCong"] =
                "Đổi mật khẩu thành công.";

            return RedirectToAction(
                nameof(DoiMatKhau));
        }

        // Đăng xuất
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Index",
                "Home");
        }

        // Trang từ chối truy cập
        [AllowAnonymous]
        [HttpGet]
        public IActionResult TuChoiTruyCap()
        {
            return View();
        }

        // Chuyển lỗi Identity sang thông báo tiếng Việt
        private void ThemLoiIdentityVaoModelState(
            IdentityResult ketQua)
        {
            foreach (IdentityError loi in ketQua.Errors)
            {
                string thongBao = loi.Code switch
                {
                    "DuplicateEmail" =>
                        "Email này đã được sử dụng.",

                    "DuplicateUserName" =>
                        "Email này đã được sử dụng.",

                    "PasswordMismatch" =>
                        "Mật khẩu hiện tại không chính xác.",

                    "PasswordTooShort" =>
                        "Mật khẩu phải có ít nhất 6 ký tự.",

                    "PasswordRequiresDigit" =>
                        "Mật khẩu phải có ít nhất một chữ số.",

                    "PasswordRequiresLower" =>
                        "Mật khẩu phải có ít nhất một chữ thường.",

                    "PasswordRequiresUpper" =>
                        "Mật khẩu phải có ít nhất một chữ hoa.",

                    "PasswordRequiresNonAlphanumeric" =>
                        "Mật khẩu phải có ít nhất một ký tự đặc biệt.",

                    "PasswordRequiresUniqueChars" =>
                        "Mật khẩu chưa có đủ số ký tự khác nhau.",

                    "InvalidToken" =>
                        "Liên kết đặt lại mật khẩu không hợp lệ " +
                        "hoặc đã hết hạn.",

                    _ => loi.Description
                };

                ModelState.AddModelError(
                    string.Empty,
                    thongBao);
            }
        }
    }
}