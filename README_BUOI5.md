# BUỔI 5 – ADO.NET nâng cao, Stored Procedure và POS Checkout

Dự án được phát triển tiếp từ `Buoi4-master.zip` đã gửi, giữ lại các controller, model, migration và giao diện WinForms hiện có; bổ sung các nội dung thực hành Buổi 5 theo file bài giảng.

## Nội dung đã bổ sung

- **Tiết 1 – ADO.NET cơ bản:** project Console riêng `ThucHanhAdoNetConsole` minh họa `ExecuteScalar`, `ExecuteNonQuery` + tham số, `ExecuteReader`, `SqlTransaction`/`Rollback`.
- **Tiết 2 – Barcode / DataTable:** `GET /api/ado-products/barcode/{barcode}`, `GET /api/ado-products/disconnected-cache`, bài tập `GET /api/ado-products/filter-stock?minStock=50`.
- **Tiết 3 – Stored Procedure / báo cáo:** `GET /api/Reports/revenue-by-cashier`, `GET /api/Reports/revenue-by-date`, `GET /api/Reports/top5-selling-products`.
- **Tiết 4–5 – ACID / POS checkout:** `POST /api/checkout` ghi hóa đơn, chi tiết hóa đơn, trừ tồn kho và cộng điểm trong cùng transaction; `DELETE /api/checkout/cancel/{orderId}` hủy đơn, hoàn kho, trừ điểm và xóa hóa đơn trong transaction.
- SQL tạo bảng `Orders`, `OrderDetails`, thủ tục báo cáo và dữ liệu demo: `MiniSupermarket.API/Database/Buoi5_Setup.sql`.

> **Lưu ý về dữ liệu:** project Buổi 4 bạn gửi là dữ liệu rau củ Đà Lạt, còn ví dụ trong giáo trình dùng dữ liệu siêu thị như snack/Danisa/Coca-Cola. Phần code được tích hợp theo đúng cấu trúc `Products`/`Customers` hiện có của Buổi 4 nên tên, mã vạch, giá và tồn kho sẽ theo database của bạn; chức năng ADO.NET, Stored Procedure và Transaction được bổ sung theo yêu cầu bài Buổi 5.

## Cách chạy

### 1. Mở project
Mở `MiniSupermarket.API.sln` bằng Visual Studio 2022. Chọn project `MiniSupermarket.API` làm Startup Project.

### 2. Kiểm tra chuỗi kết nối
Mở `MiniSupermarket.API/appsettings.json`, chỉnh `ConnectionStrings:DefaultConnection` trỏ đến SQL Server trên máy của bạn. File đang giữ server từ project gốc (`.\\NGUYENKHA`), ví dụ:
- SQL Server Express: `Server=.\\SQLEXPRESS;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;`
- Default instance: `Server=localhost;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;`

Database phải là `MiniSupermarketDb`. Chạy migration Buổi 4 trước để có bảng `Products`, `Categories`, `Customers`.

### 3. Tạo bảng và Stored Procedure Buổi 5
Mở SQL Server Management Studio (SSMS), chọn database `MiniSupermarketDb`, mở `MiniSupermarket.API/Database/Buoi5_Setup.sql` và Execute. Script tạo các bảng còn thiếu và các Stored Procedure; nếu bảng Orders chưa có dữ liệu và đã có khách hàng/sản phẩm, script sẽ tạo 15 hóa đơn demo. Chạy script một lần nữa cũng không tạo thêm dữ liệu demo nếu Orders đã có dữ liệu.

### 4. Chạy Web API
Nhấn F5 hoặc Ctrl+F5. Mở đường dẫn Swagger được Visual Studio hiển thị, thường là `https://localhost:<port>/swagger`.

## Kiểm thử Swagger

### Barcode
- `GET /api/ado-products/barcode/8930000000011` (mã mẫu trong dữ liệu Buổi 4; có thể thay bằng Barcode đang tồn tại trong bảng Products).
- `GET /api/ado-products/disconnected-cache`
- `GET /api/ado-products/filter-stock?minStock=50`

### Báo cáo
- `GET /api/Reports/revenue-by-cashier`
- `GET /api/Reports/revenue-by-date?fromDate=2026-10-01&toDate=2026-10-31`
- `GET /api/Reports/top5-selling-products`

### Thanh toán thành công
Dùng `CustomerId` đang tồn tại (database mẫu có CustomerId 1) và `ProductId` tồn tại, còn đủ hàng. Giá gửi từ client chỉ để phù hợp mẫu giáo trình; API lấy giá bán thật từ database.

```json
{
  "cashierUsername": "cashier01",
  "customerId": 1,
  "items": [
    { "productId": 1, "quantity": 2, "unitPrice": 25000 },
    { "productId": 5, "quantity": 1, "unitPrice": 20000 }
  ]
}
```

### Kiểm thử Rollback
Gửi `POST /api/checkout` với một sản phẩm có `quantity` lớn hơn tồn kho thực tế. API trả `400 Bad Request`, đồng thời transaction hoàn tác toàn bộ thao tác; không để lại hóa đơn/chi tiết và không trừ kho/điểm.

### Hủy đơn
Sau khi thanh toán thành công, dùng `DELETE /api/checkout/cancel/{orderId}` với `orderId` nhận được từ phản hồi thanh toán. API hoàn kho, khấu trừ điểm thưởng đã cộng, xóa OrderDetails rồi Orders trong một transaction.

## Chạy Console ADO.NET (Tiết 1 và mô phỏng Tiết 4)
Mở terminal tại thư mục `ThucHanhAdoNetConsole`, sửa `connectionString` trong `Program.cs` cho đúng máy SQL Server, rồi chạy:

```bash
dotnet run
```

Menu có 4 bài. Bài 2 thực sự thay đổi tồn kho nên chỉ chạy với ProductId phù hợp; bài 4 cố tình phát sinh lỗi để quan sát Rollback.

## Ghi chú kỹ thuật
- Tất cả câu truy vấn có đầu vào đều dùng tham số SQL, không ghép chuỗi đầu vào vào SQL.
- Kết nối, command và reader được giải phóng bằng `using`.
- `CheckoutController` gắn mọi command vào cùng `SqlTransaction`; chỉ `Commit()` sau khi hoàn tất, lỗi thì `Rollback()`.
- Không kèm `bin`, `obj`, `.vs` hoặc các file build/cache; Visual Studio sẽ khôi phục NuGet và build lại.
