-- =============================================
-- Project: MilkTea House
-- File: 03_InsertData.sql
-- Purpose: Thêm dữ liệu mẫu cho MilkTeaHouse
-- =============================================

USE MilkTeaHouse;
GO

-- =========================
-- Dữ liệu bảng DanhMuc
-- =========================
INSERT INTO DanhMuc (TenDanhMuc, MoTa)
VALUES
(N'Trà sữa', N'Các loại trà sữa thơm ngon, béo nhẹ'),
(N'Trà trái cây', N'Các loại trà thanh mát kết hợp trái cây'),
(N'Đá xay', N'Các món đá xay mát lạnh'),
(N'Topping', N'Trân châu, pudding, thạch và các món thêm');
GO
-- =========================
-- Dữ liệu bảng SanPham
-- NhanSanPham: 0 = Không nhãn, 1 = HOT, 2 = NEW
-- =========================
INSERT INTO SanPham
(DanhMucID, TenSanPham, MoTa, Gia, GiaGoc, HinhAnh, DanhGia, SoLuongTon, NhanSanPham, ThuTuHienThi)
VALUES
-- Trà sữa
(1, N'Trà sữa Matcha', N'Matcha thơm béo, trân châu dai ngon', 29000, 35000, N'tra-sua-matcha.png', 4.9, 100, 1, 1),
(1, N'Trà sữa Đường đen', N'Vị đường đen đậm đà, béo nhẹ', 29000, 35000, N'tra-sua-duong-den.png', 4.8, 100, 1, 2),
(1, N'Trà sữa Truyền thống', N'Hương vị trà sữa truyền thống dễ uống', 25000, 30000, N'tra-sua-truyen-thong.png', 4.7, 100, 2, 3),
(1, N'Trà sữa Socola', N'Socola thơm béo, phù hợp mọi lứa tuổi', 29000, 35000, N'tra-sua-socola.png', 4.8, 100, 1, 4),
(1, N'Trà sữa Oolong', N'Trà oolong thanh nhẹ, hậu vị thơm', 32000, 38000, N'tra-sua-oolong.png', 4.7, 80, 0, 5),
(1, N'Trà sữa Khoai môn', N'Khoai môn béo bùi, màu tím đẹp mắt', 32000, 38000, N'tra-sua-khoai-mon.png', 4.6, 80, 0, 6),
(1, N'Trà sữa Dâu', N'Vị dâu ngọt nhẹ, thơm mát', 30000, 35000, N'tra-sua-dau.png', 4.6, 80, 2, 7),
(1, N'Trà sữa Caramel', N'Caramel thơm ngọt, béo nhẹ', 32000, 38000, N'tra-sua-caramel.png', 4.7, 70, 0, 8),

-- Trà trái cây
(2, N'Trà đào cam sả', N'Trà đào kết hợp cam sả thanh mát', 30000, 35000, N'tra-dao-cam-sa.png', 4.8, 90, 1, 9),
(2, N'Trà chanh dây', N'Vị chanh dây chua ngọt, giải khát', 28000, 33000, N'tra-chanh-day.png', 4.6, 90, 0, 10),
(2, N'Trà vải', N'Trà vải thơm nhẹ, thanh mát', 30000, 35000, N'tra-vai.png', 4.7, 90, 0, 11),
(2, N'Trà xoài', N'Vị xoài nhiệt đới thơm ngon', 30000, 35000, N'tra-xoai.png', 4.6, 80, 0, 12),
(2, N'Trà dâu', N'Trà dâu tươi mát, vị ngọt dịu', 30000, 35000, N'tra-dau.png', 4.7, 80, 2, 13),
(2, N'Trà việt quất', N'Việt quất chua nhẹ, hương thơm đặc trưng', 32000, 38000, N'tra-viet-quat.png', 4.6, 70, 0, 14),

-- Đá xay
(3, N'Matcha đá xay', N'Matcha đá xay mịn, béo thơm', 39000, 45000, N'matcha-da-xay.png', 4.8, 60, 1, 15),
(3, N'Socola đá xay', N'Socola đá xay mát lạnh', 39000, 45000, N'socola-da-xay.png', 4.7, 60, 0, 16),
(3, N'Oreo đá xay', N'Oreo đá xay giòn thơm, béo nhẹ', 42000, 48000, N'oreo-da-xay.png', 4.8, 60, 2, 17),
(3, N'Cookie đá xay', N'Cookie đá xay thơm béo, hấp dẫn', 42000, 48000, N'cookie-da-xay.png', 4.7, 60, 0, 18),

-- Topping
(4, N'Trân châu đen', N'Trân châu đen dai mềm', 7000, NULL, N'tran-chau-den.png', 4.8, 200, 0, 19),
(4, N'Trân châu trắng', N'Trân châu trắng giòn dai', 8000, NULL, N'tran-chau-trang.png', 4.7, 200, 0, 20),
(4, N'Pudding trứng', N'Pudding mềm mịn, béo nhẹ', 8000, NULL, N'pudding-trung.png', 4.7, 150, 0, 21),
(4, N'Thạch trái cây', N'Thạch trái cây nhiều màu, thanh mát', 7000, NULL, N'thach-trai-cay.png', 4.6, 150, 0, 22),
(4, N'Cheese Foam', N'Lớp kem cheese mặn nhẹ, béo thơm', 10000, NULL, N'cheese-foam.png', 4.8, 120, 1, 23),
(4, N'Kem Cheese', N'Kem cheese béo mịn dùng kèm đồ uống', 10000, NULL, N'kem-cheese.png', 4.7, 120, 0, 24);
GO