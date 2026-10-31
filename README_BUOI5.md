# MiniSupermarket – Hệ thống quản lý siêu thị mini

## 1. Giới thiệu
MiniSupermarket là ứng dụng quản lý siêu thị mini, được xây dựng bằng ASP.NET Core Web API, Windows Forms và SQL Server.

Hệ thống hỗ trợ quản lý sản phẩm, danh mục, khách hàng, tài khoản, bán hàng tại quầy và báo cáo doanh thu.

## 2. Công nghệ sử dụng
- Backend: ASP.NET Core Web API (.NET 8).
- Frontend: Windows Forms (.NET 8).
- Database: Microsoft SQL Server.
- ORM: Entity Framework Core.
- Data Access: ADO.NET.
- Authentication: JWT Bearer.
- API Documentation: Swagger.
- IDE: Visual Studio 2022.
- Version Control: Git và GitHub.

## 3. Các chức năng đã thực hiện

### 3.1. Quản lý tài khoản và phân quyền
- Đăng nhập bằng tài khoản hệ thống.
- Xác thực người dùng bằng JWT.
- Phân quyền Admin, Cashier và Warehouse.
- Quản lý tài khoản người dùng.

### 3.2. Quản lý dữ liệu
- Quản lý sản phẩm: thêm, sửa, xóa, tìm kiếm.
- Quản lý danh mục sản phẩm.
- Quản lý khách hàng.
- Quản lý đơn hàng.
- Theo dõi số lượng tồn kho.

### 3.3. Bán hàng POS
- Tìm kiếm sản phẩm.
- Thêm sản phẩm vào giỏ hàng.
- Tính tổng tiền thanh toán.
- Kiểm tra tồn kho trước khi bán.
- Tạo hóa đơn và chi tiết hóa đơn.
- Cập nhật số lượng tồn kho.
- Cộng điểm thưởng khách hàng.
- Hỗ trợ hủy đơn và hoàn trả tồn kho.

### 3.4. Báo cáo và thống kê
- Báo cáo doanh thu theo nhân viên thu ngân.
- Báo cáo doanh thu theo ngày.
- Thống kê 5 sản phẩm bán chạy.
- Tra cứu dữ liệu sản phẩm bằng mã vạch.

### 3.5. ADO.NET và giao dịch
- Thực hành ExecuteScalar, ExecuteNonQuery và ExecuteReader.
- Sử dụng tham số trong câu lệnh SQL.
- Sử dụng SqlTransaction.
- Thực hiện Commit và Rollback.
- Xây dựng Stored Procedure phục vụ báo cáo.

## 4. Cấu trúc dự án

- MiniSupermarket.API: Backend Web API.
- MiniSupermarket.WinForms: Giao diện Windows Forms.
- ThucHanhAdoNetConsole: Các bài thực hành ADO.NET.
- MiniSupermarket.API/Database: Script SQL.
- MiniSupermarket.API/Migrations: Các migration Entity Framework Core.

## 5. Yêu cầu môi trường
- Windows 10/11.
- Visual Studio 2022.
- .NET 8 SDK.
- SQL Server.
- SQL Server Management Studio (SSMS).

## 6. Hướng dẫn cài đặt và chạy

### Bước 1: Tạo cơ sở dữ liệu
1. Mở SQL Server Management Studio.
2. Kết nối SQL Server.
3. Tạo hoặc khôi phục database MiniSupermarketDb.
4. Cấu hình chuỗi kết nối trong MiniSupermarket.API/appsettings.json.
5. Đảm bảo database có các bảng cần thiết trước khi chạy ứng dụng.

### Bước 2: Chạy Backend
1. Mở MiniSupermarket.API.sln bằng Visual Studio 2022.
2. Chọn project MiniSupermarket.API làm Startup Project.
3. Kiểm tra cấu hình ConnectionStrings và JwtSettings.
4. Khôi phục các NuGet packages nếu Visual Studio yêu cầu.
5. Nhấn F5 hoặc Ctrl + F5.
6. Mở Swagger tại https://localhost:7044/swagger.

### Bước 3: Chuẩn bị dữ liệu Buổi 5
1. Mở file MiniSupermarket.API/Database/Buoi5_Setup.sql bằng SSMS.
2. Chọn database MiniSupermarketDb.
3. Thực thi script SQL.
4. Kiểm tra các bảng và Stored Procedure đã được tạo.

### Bước 4: Chạy Frontend WinForms
1. Trong Visual Studio, nhấp chuột phải project MiniSupermarket.WinForms.
2. Chọn Set as Startup Project.
3. Kiểm tra địa chỉ API trong các file giao diện, mặc định là https://localhost:7044.
4. Đảm bảo Backend đang chạy.
5. Nhấn F5 để khởi động ứng dụng.
6. Đăng nhập bằng tài khoản đã được tạo trong database.

### Bước 5: Chạy chương trình thực hành ADO.NET
1. Mở Terminal tại thư mục ThucHanhAdoNetConsole.
2. Cấu hình chuỗi kết nối SQL Server trong Program.cs.
3. Chạy lệnh:

```bash
dotnet run
```

## 7. API kiểm thử

- POST /api/Auth/login: Đăng nhập.
- GET /api/Products: Lấy danh sách sản phẩm.
- GET /api/ado-products/barcode/{barcode}: Tra cứu mã vạch.
- GET /api/Reports/revenue-by-cashier: Doanh thu theo thu ngân.
- GET /api/Reports/revenue-by-date: Doanh thu theo ngày.
- GET /api/Reports/top5-selling-products: Top 5 sản phẩm bán chạy.
- POST /api/checkout: Thanh toán POS.
- DELETE /api/checkout/cancel/{orderId}: Hủy đơn hàng.

## 8. Lưu ý
- Phải cấu hình đúng SQL Server trước khi chạy ứng dụng.
- Backend cần được khởi động trước Frontend.
- Không đưa mật khẩu, khóa JWT hoặc thông tin kết nối cá nhân lên GitHub.
- Không đưa các thư mục bin, obj, .vs và node_modules lên GitHub.
- Các thư viện NuGet sẽ được khôi phục khi mở và build dự án.

## 9. Tác giả
Đồ án môn học: Xây dựng hệ thống quản lý siêu thị mini.
