using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MilkTea.Web.Models
{
    public class ThemSanPhamViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int DanhMucID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(
            150,
            ErrorMessage = "Tên sản phẩm không được vượt quá 150 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [StringLength(
            1000,
            ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
        [Range(
            typeof(decimal),
            "1000",
            "999999999",
            ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 1.000 đồng.")]
        [Display(Name = "Giá bán")]
        public decimal Gia { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "999999999",
            ErrorMessage = "Giá gốc không hợp lệ.")]
        [Display(Name = "Giá gốc")]
        public decimal? GiaGoc { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Số lượng tồn không được nhỏ hơn 0.")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [Range(
            0,
            2,
            ErrorMessage = "Nhãn sản phẩm không hợp lệ.")]
        [Display(Name = "Nhãn sản phẩm")]
        public int NhanSanPham { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Thứ tự hiển thị không được nhỏ hơn 0.")]
        [Display(Name = "Thứ tự hiển thị")]
        public int ThuTuHienThi { get; set; }

        [Display(Name = "Mở bán ngay")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Hình ảnh sản phẩm")]
        public IFormFile? HinhAnhTaiLen { get; set; }

        public List<DanhMuc> DanhMucs { get; set; }
            = new List<DanhMuc>();
    }
}