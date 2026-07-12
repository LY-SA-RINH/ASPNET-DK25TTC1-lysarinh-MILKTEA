using Microsoft.EntityFrameworkCore;
using MilkTea.Web.Models;

namespace MilkTea.Web.Data
{
    public static class KhoiTaoDuLieu
    {
        public static void KhoiTao(MilkTeaDbContext context)
        {
            // Tự động áp dụng các Migration còn thiếu.
            context.Database.Migrate();

            // Bảo đảm bốn danh mục cần thiết luôn tồn tại.
            int danhMucTraSuaID = LayHoacTaoDanhMuc(
                context,
                "Trà sữa",
                "Các loại trà sữa thơm ngon");

            int danhMucTraTraiCayID = LayHoacTaoDanhMuc(
                context,
                "Trà trái cây",
                "Các loại trà kết hợp trái cây");

            int danhMucDaXayID = LayHoacTaoDanhMuc(
                context,
                "Đá xay",
                "Các loại thức uống đá xay");

            int danhMucToppingID = LayHoacTaoDanhMuc(
                context,
                "Topping",
                "Các loại topping dùng kèm");

            // Nếu bảng SanPhams đã có dữ liệu thì không thêm lại.
            // Nhờ đó dữ liệu hiện tại không bị nhân đôi.
            if (context.SanPhams.Any())
            {
                return;
            }

            DateTime ngayTao = DateTime.Now;

            context.SanPhams.AddRange(
                // =========================
                // DANH MỤC TRÀ SỮA
                // =========================
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Matcha",
                    MoTa = "Trà sữa matcha thơm nhẹ, béo dịu và dễ uống.",
                    Gia = 29000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-sua-matcha.png",
                    DanhGia = 4.90m,
                    SoLuongTon = 50,
                    NhanSanPham = 1,
                    ThuTuHienThi = 1,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa đường đen",
                    MoTa = "Trà sữa béo thơm kết hợp vị đường đen đậm đà.",
                    Gia = 29000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-sua-duong-den.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 40,
                    NhanSanPham = 1,
                    ThuTuHienThi = 2,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa truyền thống",
                    MoTa = "Hương vị trà sữa truyền thống thơm béo và quen thuộc.",
                    Gia = 25000,
                    GiaGoc = 30000,
                    HinhAnh = "tra-sua-truyen-thong.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 60,
                    NhanSanPham = 2,
                    ThuTuHienThi = 3,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Socola",
                    MoTa = "Trà sữa hòa quyện cùng vị socola thơm ngọt.",
                    Gia = 29000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-sua-socola.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 35,
                    NhanSanPham = 1,
                    ThuTuHienThi = 4,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Oolong",
                    MoTa = "Trà sữa sử dụng trà Oolong thơm dịu và thanh vị.",
                    Gia = 32000,
                    GiaGoc = 38000,
                    HinhAnh = "tra-sua-oolong.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 80,
                    NhanSanPham = 0,
                    ThuTuHienThi = 5,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Khoai môn",
                    MoTa = "Trà sữa khoai môn thơm béo với màu tím đặc trưng.",
                    Gia = 32000,
                    GiaGoc = 38000,
                    HinhAnh = "tra-sua-khoai-mon.png",
                    DanhGia = 4.60m,
                    SoLuongTon = 80,
                    NhanSanPham = 0,
                    ThuTuHienThi = 6,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Dâu",
                    MoTa = "Trà sữa kết hợp hương dâu thơm ngọt và tươi mát.",
                    Gia = 30000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-sua-dau.png",
                    DanhGia = 4.60m,
                    SoLuongTon = 80,
                    NhanSanPham = 2,
                    ThuTuHienThi = 7,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraSuaID,
                    TenSanPham = "Trà sữa Caramel",
                    MoTa = "Trà sữa thơm béo kết hợp vị caramel ngọt dịu.",
                    Gia = 32000,
                    GiaGoc = 38000,
                    HinhAnh = "tra-sua-caramel.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 70,
                    NhanSanPham = 0,
                    ThuTuHienThi = 8,
                    TrangThai = true,
                    NgayTao = ngayTao
                },

                // =========================
                // DANH MỤC TRÀ TRÁI CÂY
                // =========================
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà đào cam sả",
                    MoTa = "Trà đào thanh mát kết hợp cam tươi và hương sả.",
                    Gia = 30000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-dao-cam-sa.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 90,
                    NhanSanPham = 1,
                    ThuTuHienThi = 9,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà chanh dây",
                    MoTa = "Trà chanh dây có vị chua ngọt và thanh mát.",
                    Gia = 28000,
                    GiaGoc = 33000,
                    HinhAnh = "tra-chanh-day.png",
                    DanhGia = 4.60m,
                    SoLuongTon = 90,
                    NhanSanPham = 0,
                    ThuTuHienThi = 10,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà vải",
                    MoTa = "Trà vải thơm mát với vị ngọt nhẹ dễ uống.",
                    Gia = 30000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-vai.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 90,
                    NhanSanPham = 0,
                    ThuTuHienThi = 11,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà xoài nhiệt đới",
                    MoTa = "Trà xoài có vị trái cây nhiệt đới thơm ngon.",
                    Gia = 30000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-xoai.png",
                    DanhGia = 4.60m,
                    SoLuongTon = 80,
                    NhanSanPham = 0,
                    ThuTuHienThi = 12,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà dâu",
                    MoTa = "Trà dâu thơm nhẹ, chua ngọt và thanh mát.",
                    Gia = 30000,
                    GiaGoc = 35000,
                    HinhAnh = "tra-dau.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 80,
                    NhanSanPham = 2,
                    ThuTuHienThi = 13,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucTraTraiCayID,
                    TenSanPham = "Trà việt quất",
                    MoTa = "Trà việt quất có vị chua ngọt và màu sắc hấp dẫn.",
                    Gia = 32000,
                    GiaGoc = 38000,
                    HinhAnh = "tra-viet-quat.png",
                    DanhGia = 4.60m,
                    SoLuongTon = 70,
                    NhanSanPham = 0,
                    ThuTuHienThi = 14,
                    TrangThai = true,
                    NgayTao = ngayTao
                },

                // =========================
                // DANH MỤC ĐÁ XAY
                // =========================
                new SanPham
                {
                    DanhMucID = danhMucDaXayID,
                    TenSanPham = "Matcha đá xay",
                    MoTa = "Matcha đá xay mát lạnh, thơm nhẹ và béo dịu.",
                    Gia = 39000,
                    GiaGoc = 45000,
                    HinhAnh = "matcha-da-xay.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 60,
                    NhanSanPham = 1,
                    ThuTuHienThi = 15,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucDaXayID,
                    TenSanPham = "Socola đá xay",
                    MoTa = "Socola đá xay đậm vị, mát lạnh và thơm béo.",
                    Gia = 39000,
                    GiaGoc = 45000,
                    HinhAnh = "socola-da-xay.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 60,
                    NhanSanPham = 0,
                    ThuTuHienThi = 16,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucDaXayID,
                    TenSanPham = "Oreo đá xay",
                    MoTa = "Oreo đá xay giòn thơm, béo ngọt và mát lạnh.",
                    Gia = 42000,
                    GiaGoc = 48000,
                    HinhAnh = "oreo-da-xay.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 60,
                    NhanSanPham = 2,
                    ThuTuHienThi = 17,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucDaXayID,
                    TenSanPham = "Cookie đá xay",
                    MoTa = "Cookie đá xay thơm béo với bánh quy nghiền.",
                    Gia = 42000,
                    GiaGoc = 48000,
                    HinhAnh = "cookie-da-xay.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 60,
                    NhanSanPham = 0,
                    ThuTuHienThi = 18,
                    TrangThai = true,
                    NgayTao = ngayTao
                },

                // =========================
                // DANH MỤC TOPPING
                // =========================
                new SanPham
                {
                    DanhMucID = danhMucToppingID,
                    TenSanPham = "Trân châu đen",
                    MoTa = "Trân châu đen mềm dẻo dùng kèm các loại thức uống.",
                    Gia = 7000,
                    GiaGoc = null,
                    HinhAnh = "tran-chau-den.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 200,
                    NhanSanPham = 0,
                    ThuTuHienThi = 19,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucToppingID,
                    TenSanPham = "Trân châu trắng",
                    MoTa = "Trân châu trắng giòn dai dùng kèm thức uống.",
                    Gia = 8000,
                    GiaGoc = null,
                    HinhAnh = "tran-chau-trang.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 200,
                    NhanSanPham = 0,
                    ThuTuHienThi = 20,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucToppingID,
                    TenSanPham = "Pudding trứng",
                    MoTa = "Pudding trứng mềm mịn, thơm béo và ngọt dịu.",
                    Gia = 8000,
                    GiaGoc = null,
                    HinhAnh = "pudding-trung.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 150,
                    NhanSanPham = 0,
                    ThuTuHienThi = 21,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucToppingID,
                    TenSanPham = "Thạch trái cây",
                    MoTa = "Thạch trái cây nhiều hương vị, giòn và thanh mát.",
                    Gia = 7000,
                    GiaGoc = null,
                    HinhAnh = "thach-trai-cay.png",
                    DanhGia = 4.60m,
                    SoLuongTon = 150,
                    NhanSanPham = 0,
                    ThuTuHienThi = 22,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucToppingID,
                    TenSanPham = "Cheese Foam",
                    MoTa = "Lớp kem cheese foam béo mịn dùng kèm thức uống.",
                    Gia = 10000,
                    GiaGoc = null,
                    HinhAnh = "cheese-foam.png",
                    DanhGia = 4.80m,
                    SoLuongTon = 120,
                    NhanSanPham = 1,
                    ThuTuHienThi = 23,
                    TrangThai = true,
                    NgayTao = ngayTao
                },
                new SanPham
                {
                    DanhMucID = danhMucToppingID,
                    TenSanPham = "Kem Cheese",
                    MoTa = "Kem cheese thơm béo, mềm mịn dùng kèm đồ uống.",
                    Gia = 10000,
                    GiaGoc = null,
                    HinhAnh = "kem-cheese.png",
                    DanhGia = 4.70m,
                    SoLuongTon = 120,
                    NhanSanPham = 0,
                    ThuTuHienThi = 24,
                    TrangThai = true,
                    NgayTao = ngayTao
                }
            );

            context.SaveChanges();
        }

        private static int LayHoacTaoDanhMuc(
            MilkTeaDbContext context,
            string tenDanhMuc,
            string moTa)
        {
            var danhMuc = context.DanhMucs
                .SingleOrDefault(dm => dm.TenDanhMuc == tenDanhMuc);

            if (danhMuc == null)
            {
                danhMuc = new DanhMuc
                {
                    TenDanhMuc = tenDanhMuc,
                    MoTa = moTa,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };

                context.DanhMucs.Add(danhMuc);
                context.SaveChanges();
            }

            return danhMuc.DanhMucID;
        }
    }
}