# MILKTEA HOUSE - WEBSITE BÁN TRÀ SỮA

## 1. Thông tin đồ án

- Tên đề tài: Website bán trà sữa
- Tên website: MilkTea House
- Sinh viên thực hiện: LÝ SA RINH
- Lớp: DK25TTC1
- Môn học: Chuyên đề ASP.NET
- Giảng viên hướng dẫn: TS. NGUYỄN NHỨT LAM
- Email sinh viên: rinhls260889@tvu-onschool.edu.vn
- Số điện thoại: 0917628587

## 2. Giới thiệu đề tài

MilkTea House là website bán trà sữa được xây dựng bằng ASP.NET Core MVC và SQL Server.

Website hướng đến việc hỗ trợ khách hàng xem sản phẩm, xem thông tin sản phẩm, thêm sản phẩm vào giỏ hàng và đặt hàng trực tuyến.

Hệ thống dự kiến có các chức năng dành cho khách hàng, nhân viên và quản trị viên.

## 3. Công nghệ sử dụng

- Ngôn ngữ lập trình C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- HTML
- CSS
- Bootstrap
- Visual Studio
- Git
- GitHub
- GitHub Desktop

## 4. Các chức năng đã thực hiện

- Khởi tạo dự án ASP.NET Core MVC.
- Xây dựng giao diện trang chủ.
- Xây dựng phần đầu trang và thanh điều hướng.
- Xây dựng khu vực banner giới thiệu.
- Xây dựng khu vực giới thiệu ưu điểm của cửa hàng.
- Xây dựng giao diện thẻ sản phẩm nổi bật.
- Tạo hiệu ứng chuyển slide cho banner.
- Điều chỉnh giao diện theo tông màu be và nâu.
- Tạo cơ sở dữ liệu MilkTeaHouse trên SQL Server.
- Kết nối website với cơ sở dữ liệu SQL Server.
- Hiển thị danh sách sản phẩm từ cơ sở dữ liệu lên trang chủ.
- Quản lý mã nguồn bằng Git và GitHub.
- Thực hiện commit và push trong quá trình phát triển dự án.

## 5. Dữ liệu sản phẩm hiện tại

Cơ sở dữ liệu hiện có 4 sản phẩm mẫu đang được sử dụng để kiểm tra chức năng hiển thị trên website, trong đó có:

- Trà sữa Matcha
- Trà sữa đường đen
- Các sản phẩm trà sữa mẫu khác

Dữ liệu sản phẩm được đọc từ cơ sở dữ liệu MilkTeaHouse và hiển thị lên website.

## 6. Các chức năng dự kiến phát triển

- Trang danh sách sản phẩm.
- Phân trang sản phẩm.
- Trang chi tiết sản phẩm.
- Giỏ hàng.
- Đặt hàng và thanh toán.
- Đăng ký tài khoản.
- Đăng nhập.
- Quản lý danh mục sản phẩm.
- Quản lý sản phẩm.
- Quản lý đơn hàng.
- Báo cáo doanh thu.
- Báo cáo tồn kho.

## 7. Cấu trúc thư mục

- `.github`: chứa các tập tin cấu hình liên quan đến GitHub.
- `src`: chứa mã nguồn dự án ASP.NET Core MVC.
- `progress-report`: chứa báo cáo tiến độ thực hiện đồ án hàng tuần.
- `thesis/doc`: chứa báo cáo đồ án định dạng Word.
- `thesis/pdf`: chứa báo cáo đồ án định dạng PDF.
- `thesis/html`: chứa tài liệu dạng web nếu có.
- `thesis/abs`: chứa slide PowerPoint và tài liệu phục vụ báo cáo.
- `thesis/refs`: chứa tài liệu tham khảo.
- `.gitignore`: quy định các tập tin không đưa lên GitHub.
- `README.md`: giới thiệu và hướng dẫn sử dụng dự án.

## 8. Thông tin cơ sở dữ liệu

- Hệ quản trị cơ sở dữ liệu: SQL Server
- Tên cơ sở dữ liệu: MilkTeaHouse
- Tên máy chủ SQL Server thường sử dụng: `.\SQLEXPRESS`

## 9. Hướng dẫn chạy dự án

### Bước 1: Cài đặt phần mềm

Cài đặt các phần mềm cần thiết:

- Visual Studio
- SQL Server Express
- SQL Server Management Studio
- GitHub Desktop

Khi cài Visual Studio, cần chọn thành phần:

`ASP.NET and web development`

### Bước 2: Tải mã nguồn

Clone repository từ GitHub về máy tính bằng GitHub Desktop.

### Bước 3: Mở dự án

Mở thư mục `src`, sau đó mở file Solution của dự án bằng Visual Studio.

### Bước 4: Chuẩn bị cơ sở dữ liệu

- Mở SQL Server Management Studio.
- Kết nối đến máy chủ `.\SQLEXPRESS`.
- Kiểm tra cơ sở dữ liệu `MilkTeaHouse`.
- Đảm bảo cơ sở dữ liệu đã có dữ liệu sản phẩm mẫu.

### Bước 5: Kiểm tra chuỗi kết nối

Kiểm tra chuỗi kết nối trong file `appsettings.json` để bảo đảm tên máy chủ và tên cơ sở dữ liệu phù hợp với máy đang sử dụng.

### Bước 6: Chạy website

Mở dự án bằng Visual Studio và nhấn nút Run để chạy website.

## 10. Tiến độ thực hiện

### Tuần 1: Từ ngày 02/07/2026 đến ngày 10/07/2026

- Tìm hiểu quy định thực hiện đồ án.
- Tạo GitHub Repository.
- Cài đặt và sử dụng GitHub Desktop.
- Khởi tạo dự án ASP.NET Core MVC.
- Xây dựng giao diện trang chủ.
- Xây dựng thanh điều hướng và banner.
- Xây dựng giao diện sản phẩm nổi bật.
- Tạo cơ sở dữ liệu SQL Server.
- Kết nối website với cơ sở dữ liệu.
- Hiển thị sản phẩm từ cơ sở dữ liệu lên trang chủ.
- Commit và push mã nguồn lên GitHub.

README.md sẽ tiếp tục được cập nhật trong quá trình thực hiện đồ án.

## 11. Thông tin liên hệ

- Sinh viên: LÝ SA RINH
- Email: rinhls260889@tvu-onschool.edu.vn
- Điện thoại: 0917628587