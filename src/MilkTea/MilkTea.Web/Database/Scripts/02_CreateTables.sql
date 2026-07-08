-- =============================================
-- Project: MilkTea House
-- File: 02_CreateTables.sql
-- Purpose: Tạo các bảng cho cơ sở dữ liệu MilkTeaHouse
-- =============================================

USE MilkTeaHouse;
GO

-- =========================
-- Bảng DanhMuc
-- =========================
CREATE TABLE DanhMuc (
    DanhMucID INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO
-- =========================
-- Bảng SanPham
-- =========================
CREATE TABLE SanPham (
    SanPhamID INT IDENTITY(1,1) PRIMARY KEY,
    DanhMucID INT NOT NULL,

    TenSanPham NVARCHAR(150) NOT NULL,
    MoTa NVARCHAR(MAX) NULL,

    Gia DECIMAL(18,2) NOT NULL,
    GiaGoc DECIMAL(18,2) NULL,

    HinhAnh NVARCHAR(255) NULL,

    DanhGia DECIMAL(2,1) DEFAULT 5.0,

    SoLuongTon INT DEFAULT 0,

    NhanSanPham TINYINT NOT NULL DEFAULT 0,
    ThuTuHienThi INT NOT NULL DEFAULT 0,

    TrangThai BIT DEFAULT 1,

    NgayTao DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_SanPham_DanhMuc
        FOREIGN KEY (DanhMucID)
        REFERENCES DanhMuc(DanhMucID)
);
GO
-- =========================
-- Bảng TaiKhoan
-- =========================
CREATE TABLE TaiKhoan (
    TaiKhoanID INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    MatKhau NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(15) NULL,
    DiaChi NVARCHAR(255) NULL,
    VaiTro NVARCHAR(20) NOT NULL DEFAULT N'KhachHang',
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =========================
-- Bảng DonHang
-- =========================
CREATE TABLE DonHang (
    DonHangID INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanID INT NOT NULL,
    NgayDat DATETIME NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'ChoXuLy',
    DiaChiGiao NVARCHAR(255) NULL,
    SoDienThoaiNhan NVARCHAR(15) NULL,
    GhiChu NVARCHAR(255) NULL,

    CONSTRAINT FK_DonHang_TaiKhoan
        FOREIGN KEY (TaiKhoanID)
        REFERENCES TaiKhoan(TaiKhoanID)
);
GO

-- =========================
-- Bảng ChiTietDonHang
-- =========================
CREATE TABLE ChiTietDonHang (
    ChiTietID INT IDENTITY(1,1) PRIMARY KEY,
    DonHangID INT NOT NULL,
    SanPhamID INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,

    CONSTRAINT FK_ChiTietDonHang_DonHang
        FOREIGN KEY (DonHangID)
        REFERENCES DonHang(DonHangID),

    CONSTRAINT FK_ChiTietDonHang_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
);
GO

-- =========================
-- Bảng TinTuc
-- =========================
CREATE TABLE TinTuc (
    TinTucID INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(150) NOT NULL,
    TomTat NVARCHAR(255) NULL,
    NoiDung NVARCHAR(MAX) NULL,
    HinhAnh NVARCHAR(255) NULL,
    NgayDang DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai BIT NOT NULL DEFAULT 1
);
GO