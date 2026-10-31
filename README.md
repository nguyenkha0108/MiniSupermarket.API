# 🌿 NGHIÊN CỨU VÀ XÂY DỰNG ỨNG DỤNG QUẢN LÝ CỬA HÀNG BÁN LẺ SỬ DỤNG .NET CORE WEB API
## SIÊU THỊ MINI RAU SẠCH ĐÀ LẠT FARM

<p align="center">
  <strong>ĐỒ ÁN / BÀI TẬP LỚN – LẬP TRÌNH ỨNG DỤNG QUẢN LÝ</strong>
</p>

---

## 1. THÔNG TIN ĐỀ TÀI

| Nội dung | Thông tin |
|---|---|
| Tên đề tài | Nghiên cứu và Xây dựng Ứng dụng Quản lý Cửa hàng Bán lẻ sử dụng .NET Core Web API |
| Tên ứng dụng | Siêu thị Mini Rau sạch Đà Lạt Farm |
| Sinh viên thực hiện | Nguyễn Kha |
| Mã số sinh viên | 2124110095 |
| Lớp học phần | CCQ2411C |
| Ngôn ngữ lập trình | C# |
| Nền tảng phát triển | .NET 8 / ASP.NET Core Web API |
| Giao diện quản lý | Windows Forms |
| Hệ quản trị cơ sở dữ liệu | Microsoft SQL Server |
| Công cụ phát triển | Visual Studio, SQL Server Management Studio, Git, GitHub |

---

# 2. GIỚI THIỆU ĐỀ TÀI

Trong thời đại công nghệ thông tin phát triển mạnh mẽ, việc ứng dụng công nghệ vào hoạt động kinh doanh và quản lý cửa hàng bán lẻ ngày càng trở nên cần thiết. Các cửa hàng thực phẩm, siêu thị mini và cửa hàng rau sạch cần có một hệ thống quản lý khoa học nhằm kiểm soát hàng hóa, doanh thu, khách hàng, nhân viên và hoạt động bán hàng.

Đối với một cửa hàng chuyên kinh doanh rau củ quả sạch như **Đà Lạt Farm**, việc quản lý thủ công bằng sổ sách hoặc bảng tính có thể gây ra nhiều khó khăn. Các vấn đề thường gặp bao gồm:

- Khó kiểm soát số lượng hàng hóa tồn kho.
- Dễ xảy ra sai sót trong quá trình tính tiền.
- Khó theo dõi lịch sử mua hàng của khách hàng.
- Việc thống kê doanh thu mất nhiều thời gian.
- Khó kiểm soát quyền hạn của từng nhân viên.
- Khó phát hiện những mặt hàng sắp hết hàng.
- Việc tổng hợp báo cáo bán hàng chưa thực sự thuận tiện.

Xuất phát từ những vấn đề trên, đề tài **“Nghiên cứu và Xây dựng Ứng dụng Quản lý Cửa hàng Bán lẻ sử dụng .NET Core Web API – Siêu thị Mini Rau sạch Đà Lạt Farm”** được thực hiện nhằm xây dựng một hệ thống hỗ trợ quản lý cửa hàng bán lẻ một cách hiệu quả, hiện đại và dễ sử dụng.

Hệ thống sử dụng ASP.NET Core Web API để xây dựng tầng xử lý dữ liệu và nghiệp vụ, Windows Forms để xây dựng giao diện quản lý, kết hợp với Microsoft SQL Server để lưu trữ dữ liệu.

---

# 3. MỤC TIÊU ĐỀ TÀI

## 3.1. Mục tiêu tổng quát

Xây dựng một ứng dụng quản lý cửa hàng bán lẻ có khả năng hỗ trợ các hoạt động quản lý hàng hóa, danh mục, khách hàng, nhân viên, bán hàng, hóa đơn và thống kê doanh thu.

Ứng dụng hướng đến việc giảm thao tác thủ công, hạn chế sai sót trong quá trình bán hàng và nâng cao hiệu quả quản lý hoạt động kinh doanh.

## 3.2. Mục tiêu cụ thể

- Xây dựng hệ thống đăng nhập và xác thực người dùng.
- Phân quyền sử dụng theo từng vai trò.
- Quản lý thông tin sản phẩm.
- Quản lý danh mục sản phẩm.
- Quản lý thông tin khách hàng.
- Quản lý tài khoản nhân viên.
- Xây dựng chức năng bán hàng tại quầy.
- Hỗ trợ tìm kiếm sản phẩm và thêm sản phẩm vào giỏ hàng.
- Tính tổng tiền, tiền khách đưa và tiền thối lại.
- Quản lý số lượng hàng tồn kho.
- Xây dựng chức năng thanh toán bằng giao dịch SQL.
- Hỗ trợ tích lũy và đổi điểm khách hàng thân thiết.
- Thống kê doanh thu theo ngày, tuần và tháng.
- Hiển thị các chỉ số tổng quan trên Dashboard.
- Thống kê các sản phẩm bán chạy.
- Xuất dữ liệu báo cáo ra định dạng CSV.
- Xây dựng giao diện quản lý dễ sử dụng, bố cục rõ ràng.

---

# 4. CÔNG NGHỆ SỬ DỤNG

## 4.1. C# và .NET 8

C# là ngôn ngữ lập trình hướng đối tượng được sử dụng để phát triển ứng dụng.

.NET 8 là nền tảng phát triển ứng dụng đa năng, hỗ trợ xây dựng các hệ thống có hiệu năng tốt, dễ bảo trì và mở rộng.

Trong đề tài, C# được sử dụng để xây dựng các chức năng xử lý nghiệp vụ, giao tiếp API và phát triển giao diện quản lý.

## 4.2. ASP.NET Core Web API

ASP.NET Core Web API được sử dụng để xây dựng các dịch vụ xử lý dữ liệu và nghiệp vụ phía máy chủ.

Các chức năng chính:

- Tiếp nhận yêu cầu từ ứng dụng Windows Forms.
- Xử lý nghiệp vụ quản lý sản phẩm.
- Xử lý nghiệp vụ khách hàng.
- Xử lý đăng nhập và xác thực.
- Xử lý bán hàng và thanh toán.
- Truy vấn dữ liệu từ SQL Server.
- Trả kết quả về ứng dụng thông qua JSON.
- Cung cấp các API phục vụ thống kê và báo cáo.

## 4.3. Windows Forms

Windows Forms được sử dụng để xây dựng giao diện người dùng trên máy tính.

Giao diện cung cấp các màn hình:

- Đăng nhập.
- Trang quản lý chính.
- Dashboard thống kê.
- Quản lý sản phẩm.
- Quản lý danh mục.
- Quản lý khách hàng.
- Quản lý nhân viên.
- Bán hàng tại quầy.
- Quản lý hóa đơn.
- Báo cáo doanh thu.

## 4.4. Microsoft SQL Server

SQL Server là hệ quản trị cơ sở dữ liệu quan hệ được sử dụng để lưu trữ thông tin của cửa hàng.

Dữ liệu được tổ chức thành các bảng có quan hệ với nhau, giúp đảm bảo tính nhất quán và hỗ trợ truy vấn, thống kê.

## 4.5. Entity Framework Core

Entity Framework Core hỗ trợ thao tác với cơ sở dữ liệu thông qua các đối tượng C#.

Các thao tác được sử dụng gồm:

- Truy vấn dữ liệu.
- Thêm dữ liệu.
- Cập nhật dữ liệu.
- Xóa dữ liệu.
- Quản lý quan hệ giữa các bảng.
- Sử dụng LINQ để xử lý dữ liệu.

## 4.6. JWT Authentication

JSON Web Token (JWT) được sử dụng để xác thực người dùng khi truy cập các API có yêu cầu đăng nhập.

Sau khi đăng nhập thành công, hệ thống nhận token và sử dụng token này trong các yêu cầu tiếp theo.

## 4.7. Git và GitHub

Git được sử dụng để quản lý phiên bản mã nguồn.

GitHub được sử dụng để lưu trữ mã nguồn trực tuyến, theo dõi lịch sử thay đổi và hỗ trợ quản lý quá trình phát triển dự án.

---

# 5. KIẾN TRÚC HỆ THỐNG

Hệ thống được xây dựng theo mô hình phân chia thành các tầng nhằm giúp mã nguồn dễ quản lý, dễ mở rộng và bảo trì.

## 5.1. Mô hình tổng quan

```text
              NGƯỜI SỬ DỤNG
                     |
                     v
          WINDOWS FORMS APPLICATION
                     |
                     v
               APICLIENTSERVICE
                     |
             HTTP REQUEST / RESPONSE
                     |
                     v
             ASP.NET CORE WEB API
                     |
          -------------------------
          |           |           |
          v           v           v
      CONTROLLERS   SERVICES    JWT AUTH
          |           |           |
          ----------- | -----------
                     |
                     v
              ENTITY FRAMEWORK
                     |
                     v
               SQL SERVER
              MiniSupermarketDb
```

## 5.2. Tầng giao diện – Presentation Layer

Đây là tầng tương tác trực tiếp với người sử dụng.

Tầng giao diện được xây dựng bằng Windows Forms, chịu trách nhiệm hiển thị dữ liệu, nhận thông tin đầu vào và gửi yêu cầu đến Web API.

## 5.3. Tầng xử lý nghiệp vụ – Business Layer

ASP.NET Core Web API chịu trách nhiệm tiếp nhận yêu cầu, kiểm tra dữ liệu, thực hiện nghiệp vụ và trả kết quả.

Các nghiệp vụ quan trọng gồm:

- Đăng nhập.
- Quản lý sản phẩm.
- Quản lý danh mục.
- Quản lý khách hàng.
- Bán hàng.
- Thanh toán.
- Cập nhật tồn kho.
- Tích điểm và đổi điểm.
- Thống kê doanh thu.

## 5.4. Tầng dữ liệu – Data Access Layer

Tầng dữ liệu chịu trách nhiệm kết nối và thao tác với SQL Server thông qua Entity Framework Core và các truy vấn SQL phù hợp.

---

# 6. CÁC CHỨC NĂNG CHÍNH CỦA HỆ THỐNG

## 6.1. Chức năng đăng nhập

Hệ thống cung cấp giao diện đăng nhập dành cho nhân viên và người quản lý.

Người dùng nhập tên đăng nhập và mật khẩu. Hệ thống gửi thông tin đến API để kiểm tra.

Nếu thông tin hợp lệ, API trả về JWT Token và thông tin tài khoản.

Chức năng bao gồm:

- Kiểm tra thông tin đăng nhập.
- Xác thực tài khoản.
- Nhận JWT Token.
- Lưu trạng thái đăng nhập.
- Xác định vai trò người dùng.
- Đăng xuất và xóa thông tin phiên đăng nhập.

## 6.2. Chức năng phân quyền

Hệ thống hỗ trợ phân quyền dựa trên vai trò tài khoản.

| Vai trò | Quyền sử dụng |
|---|---|
| Admin | Quản lý toàn bộ chức năng hệ thống |
| Manager | Quản lý và theo dõi các chức năng nghiệp vụ được cấp quyền |
| Cashier | Thực hiện chức năng bán hàng tại quầy |
| Warehouse | Quản lý sản phẩm và danh mục hàng hóa |

Giao diện sẽ hiển thị các chức năng tương ứng với quyền của tài khoản đã đăng nhập.

Các API cần thiết được bảo vệ bằng cơ chế xác thực và phân quyền phù hợp.

## 6.3. Quản lý sản phẩm

Chức năng quản lý sản phẩm hỗ trợ người quản lý theo dõi thông tin hàng hóa trong cửa hàng.

Các thao tác:

- Hiển thị danh sách sản phẩm.
- Thêm sản phẩm mới.
- Cập nhật thông tin sản phẩm.
- Xóa sản phẩm.
- Tìm kiếm sản phẩm.
- Lọc sản phẩm theo danh mục.
- Theo dõi giá bán.
- Theo dõi số lượng tồn kho.

Các thông tin sản phẩm gồm:

- Mã sản phẩm.
- Tên sản phẩm.
- Danh mục.
- Đơn giá.
- Số lượng tồn kho.
- Mô tả sản phẩm.

## 6.4. Quản lý danh mục

Chức năng này hỗ trợ phân loại sản phẩm thành từng nhóm hàng hóa.

Ví dụ:

- Rau ăn lá.
- Rau củ.
- Trái cây.
- Nấm.
- Thực phẩm sạch.
- Sản phẩm khác.

Các thao tác:

- Thêm danh mục.
- Cập nhật danh mục.
- Xóa danh mục.
- Tìm kiếm danh mục.
- Hiển thị danh sách danh mục.

## 6.5. Quản lý khách hàng

Hệ thống lưu trữ thông tin khách hàng nhằm hỗ trợ chăm sóc khách hàng và theo dõi lịch sử mua hàng.

Thông tin quản lý gồm:

- Mã khách hàng.
- Họ tên.
- Số điện thoại.
- Địa chỉ.
- Điểm thưởng.
- Hạng thành viên.

Các chức năng:

- Thêm khách hàng.
- Cập nhật khách hàng.
- Xóa khách hàng.
- Tìm kiếm khách hàng.
- Theo dõi điểm tích lũy.
- Quản lý khách hàng thân thiết.

## 6.6. Quản lý nhân viên

Chức năng này hỗ trợ quản lý tài khoản người dùng của hệ thống.

Các thao tác:

- Thêm tài khoản.
- Cập nhật tài khoản.
- Xóa tài khoản.
- Tìm kiếm nhân viên.
- Phân quyền tài khoản.
- Theo dõi trạng thái tài khoản.

## 6.7. Chức năng bán hàng tại quầy – POS

POS là chức năng quan trọng của hệ thống, hỗ trợ nhân viên thực hiện các giao dịch bán hàng.

Quy trình bán hàng:

1. Nhân viên mở màn hình bán hàng.
2. Tìm kiếm sản phẩm bằng mã sản phẩm hoặc tên sản phẩm.
3. Chọn sản phẩm cần bán.
4. Thêm sản phẩm vào giỏ hàng.
5. Nhập số lượng cần mua.
6. Hệ thống tính thành tiền.
7. Chọn khách hàng nếu có.
8. Áp dụng đổi điểm thưởng nếu khách hàng đủ điều kiện.
9. Nhập số tiền khách đưa.
10. Hệ thống tính tiền thối lại.
11. Xác nhận thanh toán.
12. Hệ thống lưu hóa đơn và cập nhật tồn kho.

Các chức năng hỗ trợ:

- Tìm kiếm sản phẩm.
- Thêm sản phẩm vào giỏ hàng.
- Cập nhật số lượng.
- Xóa sản phẩm khỏi giỏ.
- Tính tổng tiền.
- Kiểm tra số lượng tồn kho.
- Kiểm tra số tiền khách thanh toán.
- Tính tiền thối lại.
- Thông báo lỗi khi dữ liệu không hợp lệ.

## 6.8. Quản lý tồn kho và cảnh báo hàng sắp hết

Hệ thống theo dõi số lượng tồn kho của từng sản phẩm.

Khi số lượng tồn kho nhỏ hơn hoặc bằng 10, sản phẩm được đánh dấu bằng màu sắc cảnh báo trên bảng dữ liệu.

Mục đích:

- Giúp nhân viên dễ dàng nhận biết sản phẩm sắp hết.
- Hạn chế tình trạng hết hàng khi bán.
- Hỗ trợ người quản lý chủ động bổ sung hàng hóa.
- Nâng cao hiệu quả quản lý kho.

## 6.9. Quản lý hóa đơn

Hệ thống lưu trữ thông tin các giao dịch bán hàng.

Thông tin hóa đơn gồm:

- Mã hóa đơn.
- Ngày tạo.
- Khách hàng.
- Nhân viên thực hiện.
- Tổng tiền.
- Điểm đã sử dụng.
- Tổng tiền thanh toán.

Thông tin chi tiết hóa đơn gồm:

- Sản phẩm.
- Số lượng.
- Đơn giá.
- Thành tiền.

## 6.10. Tích điểm và đổi điểm khách hàng thân thiết

Hệ thống hỗ trợ chương trình khách hàng thân thiết nhằm khuyến khích khách hàng quay lại mua sắm.

Khách hàng có thể tích lũy điểm sau các giao dịch hợp lệ và sử dụng điểm để giảm giá cho hóa đơn tiếp theo.

Quy tắc quy đổi trong chương trình:

**50 điểm = 50.000 VNĐ.**

Khi đổi điểm, hệ thống cần:

- Kiểm tra số điểm hiện có.
- Kiểm tra số điểm muốn sử dụng.
- Tính số tiền được giảm.
- Cập nhật điểm khách hàng.
- Tính lại số tiền thanh toán.
- Lưu hóa đơn và điểm thưởng trong cùng giao dịch SQL.

## 6.11. Dashboard thống kê

Dashboard giúp người quản lý theo dõi tình hình kinh doanh thông qua các chỉ số tổng quan.

Các chỉ số chính:

1. Doanh thu hôm nay.
2. Tổng số hóa đơn trong ngày.
3. Số khách hàng mới.
4. Số sản phẩm tồn kho thấp.

Dashboard giúp người quản lý nắm bắt tình hình hoạt động của cửa hàng nhanh chóng mà không cần truy vấn từng màn hình riêng biệt.

## 6.12. Thống kê doanh thu

Hệ thống hỗ trợ thống kê doanh thu theo khoảng thời gian.

Các hình thức thống kê:

- Theo ngày.
- Theo tuần.
- Theo tháng.
- Theo khoảng thời gian tùy chọn.

Dữ liệu được tổng hợp từ các hóa đơn và hiển thị dưới dạng biểu đồ cột để thuận tiện theo dõi.

## 6.13. Thống kê sản phẩm bán chạy

Hệ thống tổng hợp số lượng sản phẩm đã bán để xác định những sản phẩm có doanh số cao.

Kết quả thống kê hiển thị Top 5 sản phẩm bán chạy.

Việc thống kê giúp cửa hàng:

- Xác định mặt hàng được khách hàng ưa chuộng.
- Hỗ trợ kế hoạch nhập hàng.
- Theo dõi xu hướng tiêu thụ.
- Hỗ trợ đưa ra quyết định kinh doanh.

## 6.14. Xuất báo cáo

Hệ thống hỗ trợ xuất dữ liệu báo cáo ra tệp CSV.

Các loại dữ liệu hỗ trợ xuất:

- Danh sách sản phẩm.
- Danh sách hóa đơn.

Tệp CSV có thể được mở bằng Microsoft Excel hoặc các phần mềm bảng tính tương thích.

---

# 7. CƠ SỞ DỮ LIỆU

## 7.1. Tổng quan

Cơ sở dữ liệu được xây dựng trên Microsoft SQL Server với tên:

`MiniSupermarketDb`

Cơ sở dữ liệu lưu trữ thông tin sản phẩm, danh mục, khách hàng, tài khoản, hóa đơn và chi tiết giao dịch.

## 7.2. Các bảng dữ liệu chính

| Bảng | Chức năng |
|---|---|
| Categories | Lưu thông tin danh mục sản phẩm |
| Products | Lưu thông tin sản phẩm và số lượng tồn kho |
| Customers | Lưu thông tin khách hàng và điểm thưởng |
| Users | Lưu thông tin tài khoản người dùng |
| Orders | Lưu thông tin hóa đơn |
| OrderDetails | Lưu chi tiết các sản phẩm trong hóa đơn |

*Lưu ý: Danh sách trên là các bảng nghiệp vụ chính; cấu trúc thực tế có thể bao gồm thêm các bảng khác theo phiên bản cơ sở dữ liệu.*

## 7.3. Quan hệ giữa các bảng

- Một danh mục có thể chứa nhiều sản phẩm.
- Một khách hàng có thể có nhiều hóa đơn.
- Một nhân viên có thể thực hiện nhiều hóa đơn.
- Một hóa đơn có thể chứa nhiều dòng chi tiết.
- Một sản phẩm có thể xuất hiện trong nhiều chi tiết hóa đơn.

Các quan hệ được sử dụng để đảm bảo dữ liệu có tính liên kết và nhất quán.

## 7.4. Quản lý dữ liệu

Hệ thống sử dụng khóa chính và khóa ngoại để liên kết dữ liệu.

Các trường dữ liệu được xác định kiểu phù hợp nhằm hạn chế sai sót trong quá trình lưu trữ.

---

# 8. XỬ LÝ GIAO DỊCH VÀ ĐẢM BẢO TÍNH TOÀN VẸN DỮ LIỆU

Trong hoạt động bán hàng, một giao dịch có thể bao gồm nhiều thao tác liên quan:

- Tạo hóa đơn.
- Lưu chi tiết hóa đơn.
- Trừ số lượng tồn kho.
- Cập nhật điểm thưởng.
- Ghi nhận số điểm đã đổi.

Nếu một thao tác thất bại nhưng những thao tác trước đó đã được lưu, dữ liệu có thể không còn đồng nhất.

Để giải quyết vấn đề này, hệ thống sử dụng SQL Transaction.

## 8.1. Quy trình giao dịch

```text
BẮT ĐẦU GIAO DỊCH
        |
        v
  Kiểm tra sản phẩm
        |
        v
  Kiểm tra tồn kho
        |
        v
  Kiểm tra khách hàng
        |
        v
  Tính tiền thanh toán
        |
        v
  Tạo hóa đơn
        |
        v
  Lưu chi tiết hóa đơn
        |
        v
  Cập nhật tồn kho
        |
        v
  Cập nhật điểm thưởng
        |
        v
    COMMIT
        |
        v
  HOÀN TẤT GIAO DỊCH

Nếu có lỗi:
        |
        v
     ROLLBACK
        |
        v
  HỦY CÁC THAY ĐỔI
```

## 8.2. Commit

Khi tất cả thao tác thực hiện thành công, hệ thống gọi Commit để xác nhận và lưu toàn bộ thay đổi.

## 8.3. Rollback

Khi xảy ra lỗi trong quá trình thanh toán, hệ thống gọi Rollback để hủy các thay đổi chưa được xác nhận.

Điều này giúp tránh tình trạng đã tạo hóa đơn nhưng chưa trừ kho hoặc đã trừ điểm khách hàng nhưng thanh toán thất bại.

---

# 9. BẢO MẬT HỆ THỐNG

## 9.1. Xác thực JWT

JWT được sử dụng để xác định người dùng đã đăng nhập và cho phép truy cập các API được bảo vệ.

## 9.2. Phân quyền

Hệ thống kiểm tra vai trò người dùng trước khi cho phép thực hiện các chức năng phù hợp.

## 9.3. Quản lý phiên đăng nhập

Thông tin phiên đăng nhập được quản lý tập trung thông qua SessionManager.

## 9.4. Gửi token khi gọi API

Ứng dụng Windows Forms sử dụng ApiClientService để quản lý HttpClient dùng chung và gửi JWT Token trong header Authorization.

## 9.5. Bảo vệ thông tin cấu hình

Thông tin kết nối cơ sở dữ liệu và khóa JWT cần được cấu hình riêng theo môi trường chạy.

Không nên đưa mật khẩu cơ sở dữ liệu, khóa bí mật hoặc thông tin đăng nhập thật lên GitHub.

---

# 10. CẤU TRÚC THƯ MỤC DỰ ÁN

```text
MiniSupermarket/
│
├── MiniSupermarket.API/
│   │
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── CustomersController.cs
│   │   ├── CheckoutController.cs
│   │   ├── ReportsController.cs
│   │   └── DashboardController.cs
│   │
│   ├── Data/
│   │   └── SupermarketDbContext.cs
│   │
│   ├── Models/
│   │
│   ├── Migrations/
│   │
│   ├── Database/
│   │   └── Buoi5_Setup.sql
│   │
│   ├── Properties/
│   │
│   ├── appsettings.json
│   ├── Program.cs
│   └── MiniSupermarket.API.csproj
│
├── MiniSupermarket.WinForms/
│   │
│   ├── Forms/
│   │
│   ├── ApiClientService.cs
│   ├── SessionManager.cs
│   ├── FormLogin.cs
│   ├── FormMainShell.cs
│   ├── FormDashboard.cs
│   ├── FormPOS.cs
│   ├── FormProductManagement.cs
│   ├── FormCategoryManagement.cs
│   ├── FormCustomerManagement.cs
│   ├── FormOrders.cs
│   └── Program.cs
│
├── ThucHanhAdoNetConsole/
│
├── README.md
├── .gitignore
└── MiniSupermarket.sln
```

*Lưu ý: Cấu trúc trên mô tả các thành phần chính; tên thư mục hoặc vị trí file có thể khác tùy phiên bản mã nguồn.*

---

# 11. HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN

## 11.1. Yêu cầu môi trường

Trước khi chạy chương trình, cần chuẩn bị:

- Windows 10 hoặc Windows 11.
- Visual Studio hỗ trợ .NET 8.
- .NET 8 SDK.
- SQL Server.
- SQL Server Management Studio (SSMS).
- Git (nếu lấy mã nguồn từ GitHub).

## 11.2. Tải mã nguồn

Clone repository:

```bash
git clone https://github.com/nguyenkha0108/buoi5.git
```

Sau khi tải về, mở file:

`MiniSupermarket.sln`

bằng Visual Studio.

## 11.3. Cấu hình SQL Server

Mở SQL Server Management Studio.

Kết nối đến SQL Server đang sử dụng.

Mở file:

`MiniSupermarket.API/Database/Buoi5_Setup.sql`

Kiểm tra cơ sở dữ liệu và thực hiện script theo hướng dẫn.

## 11.4. Cấu hình chuỗi kết nối

Mở file:

`MiniSupermarket.API/appsettings.json`

Thay `YOUR_SQL_SERVER` bằng tên SQL Server trên máy tính.

Ví dụ:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Không sao chép nguyên tên máy chủ trong ví dụ nếu máy tính của bạn sử dụng instance khác.

## 11.5. Cấu hình JWT

Kiểm tra phần cấu hình JWT trong `appsettings.json`.

Thay khóa bí mật mẫu bằng một khóa mới đủ mạnh, tối thiểu 32 ký tự.

Không sử dụng khóa bí mật thật trong repository công khai.

## 11.6. Khôi phục thư viện NuGet

Trong Visual Studio:

1. Mở Solution.
2. Nhấn chuột phải vào Solution.
3. Chọn Restore NuGet Packages.
4. Chờ quá trình khôi phục hoàn tất.

## 11.7. Chạy API

Thiết lập `MiniSupermarket.API` làm Startup Project.

Nhấn F5 để chạy.

Kiểm tra địa chỉ API theo cấu hình ứng dụng.

Địa chỉ API mặc định được sử dụng trong ứng dụng WinForms:

```text
https://localhost:7044/api/
```

Nếu cổng chạy thực tế khác, cần đồng bộ lại địa chỉ trong cấu hình ApiClientService.

## 11.8. Chạy Windows Forms

Sau khi API đã chạy:

1. Thiết lập `MiniSupermarket.WinForms` làm Startup Project.
2. Nhấn F5.
3. Giao diện đăng nhập sẽ xuất hiện.
4. Đăng nhập bằng tài khoản hợp lệ đã được tạo trong cơ sở dữ liệu.

---

# 12. DANH SÁCH API CHÍNH

Các API được sử dụng để giao tiếp giữa ứng dụng Windows Forms và máy chủ.

| Nhóm chức năng | Endpoint tham khảo | Phương thức |
|---|---|---|
| Đăng nhập | `/api/Auth/login` | POST |
| Sản phẩm | `/api/Products` | GET |
| Sản phẩm | `/api/Products` | POST |
| Danh mục | `/api/Categories` | GET |
| Khách hàng | `/api/Customers` | GET |
| Thanh toán | `/api/Checkout` | POST |
| Báo cáo | `/api/reports/dashboard-summary` | GET |
| Doanh thu | `/api/reports/revenue-daily` | GET |
| Sản phẩm bán chạy | `/api/reports/top5-selling-products` | GET |
| Xuất sản phẩm | `/api/reports/export-products.csv` | GET |
| Xuất hóa đơn | `/api/reports/export-orders.csv` | GET |

*Các endpoint có thể khác chữ hoa/chữ thường hoặc tham số theo cấu hình route thực tế trong Controller.*

---

# 13. KẾT QUẢ ĐẠT ĐƯỢC

Thông qua quá trình nghiên cứu và xây dựng, đề tài hướng đến một ứng dụng quản lý cửa hàng bán lẻ với các nhóm chức năng chính:

- Quản lý tài khoản và phân quyền.
- Quản lý sản phẩm.
- Quản lý danh mục.
- Quản lý khách hàng.
- Quản lý nhân viên.
- Bán hàng tại quầy.
- Quản lý hóa đơn.
- Quản lý tồn kho.
- Tích điểm và đổi điểm khách hàng.
- Thống kê doanh thu.
- Thống kê sản phẩm bán chạy.
- Xuất báo cáo dữ liệu.

Việc sử dụng mô hình Web API giúp phân tách giao diện và xử lý nghiệp vụ, tạo điều kiện thuận lợi cho việc nâng cấp và mở rộng hệ thống trong tương lai.

---

# 14. ĐỐI CHIẾU CÁC TIÊU CHÍ ĐÁNH GIÁ

| Tiêu chí | Nội dung | File / vị trí kiểm tra |
|---|---|---|
| 35 | Transaction và Rollback | `CheckoutController.cs` |
| 36 | Giao diện Shell cố định | `FormMainShell.cs` |
| 37 | Điều hướng form bằng TopLevel và Dock | `FormMainShell.cs` |
| 38 | Lưu JWT Token sau đăng nhập | `SessionManager.cs`, `FormLogin.cs` |
| 39 | Phân quyền menu theo vai trò | `FormMainShell.cs` |
| 40 | HttpClient dùng chung và Bearer Token | `ApiClientService.cs` |
| 41 | CRUD sản phẩm và danh mục | `FormProductManagement.cs`, `FormCategoryManagement.cs` |
| 42 | POS, giỏ hàng, tổng tiền và tiền thối | `FormPOS.cs` |
| 43 | Cảnh báo tồn kho thấp | `FormProductManagement.cs`, `FormPOS.cs` |
| 44 | Dashboard 4 chỉ số tổng quan | `FormDashboard.cs`, `DashboardController.cs` |
| 45 | Biểu đồ doanh thu theo thời gian | `FormDashboard.cs`, `DashboardController.cs`, `Buoi5_Setup.sql` |
| 46 | Top 5 sản phẩm bán chạy | `ReportsController.cs` |
| 47 | Đổi điểm và cập nhật trong Transaction | `CheckoutController.cs`, `FormPOS.cs` |
| 48 | Xuất báo cáo CSV | `DashboardController.cs`, `FormDashboard.cs` |
| 49 | Demo bán hàng và kiểm tra SQL | Toàn bộ hệ thống |
| 50 | Thuyết trình và giải thích mã nguồn | Tài liệu báo cáo và mã nguồn |

---

# 15. KỊCH BẢN KIỂM THỬ VÀ DEMO

Để kiểm tra hệ thống, có thể thực hiện các bước sau:

## Bước 1: Kiểm tra đăng nhập

- Mở ứng dụng.
- Nhập tài khoản hợp lệ.
- Kiểm tra giao diện sau khi đăng nhập.
- Thử tài khoản có vai trò khác nhau để kiểm tra phân quyền.

## Bước 2: Kiểm tra quản lý sản phẩm

- Thêm một sản phẩm mới.
- Cập nhật tên hoặc giá sản phẩm.
- Tìm kiếm sản phẩm.
- Kiểm tra dữ liệu trong SQL Server.

## Bước 3: Kiểm tra quản lý khách hàng

- Thêm khách hàng mới.
- Cập nhật thông tin.
- Kiểm tra điểm thưởng.

## Bước 4: Kiểm tra bán hàng

- Mở màn hình POS.
- Chọn sản phẩm.
- Nhập số lượng.
- Thêm vào giỏ hàng.
- Nhập số tiền khách đưa.
- Thực hiện thanh toán.
- Kiểm tra hóa đơn được lưu.

## Bước 5: Kiểm tra tồn kho

- Ghi nhận số lượng sản phẩm trước khi bán.
- Thực hiện một giao dịch bán hàng.
- Kiểm tra số lượng tồn kho sau giao dịch.
- Kiểm tra màu cảnh báo khi tồn kho nhỏ hơn hoặc bằng 10.

## Bước 6: Kiểm tra đổi điểm

- Chọn khách hàng có điểm thưởng.
- Nhập số điểm muốn đổi.
- Kiểm tra số tiền được giảm.
- Thanh toán.
- Kiểm tra điểm còn lại và hóa đơn trong cơ sở dữ liệu.

## Bước 7: Kiểm tra Dashboard

- Mở màn hình Dashboard.
- Kiểm tra doanh thu.
- Kiểm tra số hóa đơn.
- Kiểm tra khách hàng mới.
- Kiểm tra sản phẩm tồn kho thấp.
- Kiểm tra biểu đồ doanh thu và Top 5 sản phẩm bán chạy.

## Bước 8: Kiểm tra xuất báo cáo

- Chọn chức năng xuất danh sách sản phẩm.
- Mở tệp CSV bằng Excel.
- Thử xuất danh sách hóa đơn.
- Kiểm tra nội dung dữ liệu.

---

# 16. ƯU ĐIỂM CỦA HỆ THỐNG

- Giao diện quản lý tập trung.
- Phân chia chức năng rõ ràng.
- Hỗ trợ nhiều vai trò người dùng.
- Dữ liệu được lưu trữ trên SQL Server.
- Sử dụng Web API để giao tiếp giữa các tầng.
- Hỗ trợ xác thực bằng JWT.
- Có cơ chế giao dịch SQL để bảo vệ tính toàn vẹn dữ liệu.
- Hỗ trợ quản lý hàng hóa và tồn kho.
- Có chức năng bán hàng tại quầy.
- Có chức năng thống kê và báo cáo.
- Có thể mở rộng thêm chức năng trong tương lai.

---

# 17. HẠN CHẾ VÀ HƯỚNG PHÁT TRIỂN

## 17.1. Hạn chế

Trong phạm vi thực hiện đề tài, hệ thống vẫn có thể còn một số hạn chế:

- Giao diện chủ yếu phục vụ máy tính Windows.
- Chưa triển khai đồng bộ dữ liệu trên nhiều chi nhánh.
- Chưa tích hợp thanh toán trực tuyến.
- Chưa tích hợp hóa đơn điện tử.
- Chưa triển khai hệ thống quản lý nhà cung cấp đầy đủ.
- Chưa có ứng dụng di động dành cho khách hàng.
- Việc triển khai thực tế cần bổ sung kiểm thử bảo mật và hiệu năng.

## 17.2. Hướng phát triển

Trong tương lai, hệ thống có thể được mở rộng theo các hướng:

1. Xây dựng website bán hàng trực tuyến.
2. Phát triển ứng dụng di động cho khách hàng.
3. Tích hợp thanh toán qua ngân hàng và ví điện tử.
4. Tích hợp máy quét mã vạch.
5. Quản lý nhà cung cấp và nhập hàng.
6. Quản lý hạn sử dụng của rau củ và thực phẩm.
7. Cảnh báo hàng hóa sắp hết hạn.
8. Quản lý nhiều chi nhánh.
9. Xây dựng báo cáo lợi nhuận.
10. Triển khai hệ thống lên máy chủ trực tuyến.

---

# 18. QUẢN LÝ MÃ NGUỒN VỚI GIT VÀ GITHUB

Dự án sử dụng Git để quản lý lịch sử thay đổi mã nguồn.

Các thao tác Git cơ bản:

```bash
git status
git add .
git commit -m "Mo ta thay doi"
git push origin buoi-5
```

Các thư mục và tệp không cần thiết được loại trừ thông qua `.gitignore`, bao gồm:

- `bin/`
- `obj/`
- `.vs/`
- `node_modules/`
- Các tệp cấu hình cục bộ chứa thông tin riêng tư.

Repository GitHub:

**https://github.com/nguyenkha0108/buoi5**

---

# 19. KẾT LUẬN

Đề tài **“Nghiên cứu và Xây dựng Ứng dụng Quản lý Cửa hàng Bán lẻ sử dụng .NET Core Web API – Siêu thị Mini Rau sạch Đà Lạt Farm”** tập trung nghiên cứu và áp dụng các công nghệ .NET để xây dựng một hệ thống hỗ trợ quản lý hoạt động kinh doanh của cửa hàng bán lẻ.

Thông qua đề tài, sinh viên có cơ hội vận dụng kiến thức lập trình C#, lập trình hướng đối tượng, xây dựng Web API, kết nối cơ sở dữ liệu SQL Server, xử lý giao dịch và phát triển giao diện Windows Forms.

Hệ thống là nền tảng thực hành giúp hiểu rõ hơn quy trình xây dựng một ứng dụng quản lý từ giao diện người dùng đến tầng xử lý nghiệp vụ và cơ sở dữ liệu.

Trong quá trình thực hiện, đề tài cũng giúp nâng cao kỹ năng phân tích yêu cầu, thiết kế cơ sở dữ liệu, tổ chức mã nguồn, kiểm thử chương trình và sử dụng GitHub để quản lý dự án.

Mặc dù vẫn còn những hạn chế nhất định, ứng dụng đã đặt nền tảng cho việc phát triển một hệ thống quản lý cửa hàng bán lẻ hoàn chỉnh hơn trong tương lai.

---

<p align="center">
  <strong>🌱 ĐÀ LẠT FARM – RAU SẠCH, TƯƠI NGON, CHẤT LƯỢNG 🌱</strong>
</p>

<p align="center">
  <em>Đề tài phục vụ mục đích học tập và nghiên cứu.</em>
</p>
