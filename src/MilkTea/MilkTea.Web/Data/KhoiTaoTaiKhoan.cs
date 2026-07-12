using Microsoft.AspNetCore.Identity;
using MilkTea.Web.Models;

namespace MilkTea.Web.Data
{
    public static class KhoiTaoTaiKhoan
    {
        public const string KhachHang = "KhachHang";
        public const string NhanVien = "NhanVien";
        public const string QuanTriVien = "QuanTriVien";

        public static async Task KhoiTaoAsync(
            IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<
                    RoleManager<IdentityRole>>();

            UserManager<NguoiDung> userManager =
                serviceProvider.GetRequiredService<
                    UserManager<NguoiDung>>();

            string[] danhSachVaiTro =
            {
                KhachHang,
                NhanVien,
                QuanTriVien
            };

            foreach (string vaiTro in danhSachVaiTro)
            {
                if (!await roleManager.RoleExistsAsync(vaiTro))
                {
                    IdentityResult ketQuaVaiTro =
                        await roleManager.CreateAsync(
                            new IdentityRole(vaiTro));

                    if (!ketQuaVaiTro.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Không thể tạo vai trò {vaiTro}.");
                    }
                }
            }

            await TaoTaiKhoanMauAsync(
                userManager,
                "admin@milkteahouse.local",
                "Admin@123456",
                "Quản trị viên MilkTea House",
                QuanTriVien);

            await TaoTaiKhoanMauAsync(
                userManager,
                "nhanvien@milkteahouse.local",
                "NhanVien@123456",
                "Nhân viên MilkTea House",
                NhanVien);
        }

        private static async Task TaoTaiKhoanMauAsync(
            UserManager<NguoiDung> userManager,
            string email,
            string matKhau,
            string hoTen,
            string vaiTro)
        {
            NguoiDung? nguoiDung =
                await userManager.FindByEmailAsync(email);

            if (nguoiDung == null)
            {
                nguoiDung = new NguoiDung
                {
                    HoTen = hoTen,
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    NgayTao = DateTime.Now
                };

                IdentityResult ketQua =
                    await userManager.CreateAsync(
                        nguoiDung,
                        matKhau);

                if (!ketQua.Succeeded)
                {
                    string danhSachLoi = string.Join(
                        "; ",
                        ketQua.Errors.Select(loi =>
                            loi.Description));

                    throw new InvalidOperationException(
                        $"Không thể tạo tài khoản {email}: " +
                        danhSachLoi);
                }
            }

            if (!await userManager.IsInRoleAsync(
                    nguoiDung,
                    vaiTro))
            {
                IdentityResult ketQuaPhanQuyen =
                    await userManager.AddToRoleAsync(
                        nguoiDung,
                        vaiTro);

                if (!ketQuaPhanQuyen.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Không thể cấp vai trò {vaiTro} " +
                        $"cho tài khoản {email}.");
                }
            }
        }
    }
}