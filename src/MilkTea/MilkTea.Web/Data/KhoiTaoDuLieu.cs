using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Models;

namespace MilkTea.Web.Data
{
    public static class KhoiTaoDuLieu
    {
        public static void KhoiTao(MilkTeaDbContext context)
        {
            // Tự động áp dụng Migration còn thiếu
            context.Database.Migrate();

            // Thêm danh mục nếu bảng DanhMucs chưa có dữ liệu
            if (!context.DanhMucs.Any())
            {
                context.DanhMucs.AddRange(
                    new DanhMuc
                    {
                        TenDanhMuc = "Trà sữa",
                        MoTa = "Các loại trà sữa thơm ngon, béo dịu",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    },
                    new DanhMuc
                    {
                        TenDanhMuc = "Trà trái cây",
                        MoTa = "Các loại trà kết hợp với trái cây tươi",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    },
                    new DanhMuc
                    {
                        TenDanhMuc = "Cà phê",
                        MoTa = "Các loại cà phê đậm vị",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    }
                );

                context.SaveChanges();
            }

            // Không thêm lại nếu bảng SanPhams đã có dữ liệu
            if (context.SanPhams.Any())
            {
                return;
            }

            int danhMucTraSuaID = context.DanhMucs
                .Single(dm => dm.TenDanhMuc == "Trà sữa")
                .DanhMucID;

            int danhMucTraTraiCayID = context.DanhMucs
                .Single(dm => dm.TenDanhMuc == "Trà trái cây")
                .DanhMucID;

            int danhMucCaPheID = context.DanhMucs
                .Single(dm => dm.TenDanhMuc == "Cà phê")
                .DanhMucID;

            context.SanPhams.AddRange(
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa trân châu đường đen",
                    MoTa = "Trà sữa béo thơm kết hợp trân châu đường đen.",
                    Gia = 39000,
                    GiaGoc = 45000,
                    HinhAnh = "/images/products/tra-sua-tran-chau-duong-den.png",
                    DanhGia = 4.9m,
                    SoLuongTon = 100,
                    NhanSanPham = 1,
                    ThuTuHienThi = 1,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa truyền thống",
                    MoTa = "Hương vị trà sữa truyền thống thơm béo, dễ uống.",
                    Gia = 32000,
                    GiaGoc = 35000,
                    HinhAnh = "/images/products/tra-sua-truyen-thong.png",
                    DanhGia = 4.8m,
                    SoLuongTon = 100,
                    NhanSanPham = 2,
                    ThuTuHienThi = 2,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Matcha",
                    MoTa = "Matcha thơm nhẹ kết hợp vị sữa béo dịu.",
                    Gia = 42000,
                    GiaGoc = 48000,
                    HinhAnh = "/images/products/tra-sua-matcha.png",
                    DanhGia = 4.7m,
                    SoLuongTon = 80,
                    NhanSanPham = 3,
                    ThuTuHienThi = 3,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà đào cam sả",
                    MoTa = "Trà đào thanh mát kết hợp cam tươi và sả.",
                    Gia = 40000,
                    GiaGoc = 45000,
                    HinhAnh = "/images/products/tra-dao-cam-sa.png",
                    DanhGia = 4.8m,
                    SoLuongTon = 90,
                    NhanSanPham = 1,
                    ThuTuHienThi = 4,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà vải",
                    MoTa = "Trà vải thơm mát với vị ngọt nhẹ.",
                    Gia = 38000,
                    GiaGoc = null,
                    HinhAnh = "/images/products/tra-vai.png",
                    DanhGia = 4.6m,
                    SoLuongTon = 70,
                    NhanSanPham = 0,
                    ThuTuHienThi = 5,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà chanh dây",
                    MoTa = "Trà chanh dây chua ngọt, thanh mát.",
                    Gia = 38000,
                    GiaGoc = null,
                    HinhAnh = "/images/products/tra-chanh-day.png",
                    DanhGia = 4.5m,
                    SoLuongTon = 75,
                    NhanSanPham = 0,
                    ThuTuHienThi = 6,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucCaPheID,
                    TenSanPham = "Cà phê sữa",
                    MoTa = "Cà phê đậm vị kết hợp sữa đặc.",
                    Gia = 30000,
                    GiaGoc = null,
                    HinhAnh = "/images/products/ca-phe-sua.png",
                    DanhGia = 4.7m,
                    SoLuongTon = 60,
                    NhanSanPham = 0,
                    ThuTuHienThi = 7,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new SanPham
                {
                    DanhMucID = danhMucCaPheID,
                    TenSanPham = "Bạc xỉu",
                    MoTa = "Bạc xỉu thơm béo, ít đắng.",
                    Gia = 35000,
                    GiaGoc = null,
                    HinhAnh = "/images/products/bac-xiu.png",
                    DanhGia = 4.6m,
                    SoLuongTon = 60,
                    NhanSanPham = 0,
                    ThuTuHienThi = 8,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                }
            );

            context.SaveChanges();
        }
    }
}