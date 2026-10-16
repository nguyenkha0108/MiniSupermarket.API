using Microsoft.Data.SqlClient;
using System.Data;

const string connectionString = "Server=.\\NGUYENKHA;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("THỰC HÀNH BUỔI 5 - ADO.NET / TRANSACTION");
Console.WriteLine("1. ExecuteScalar - đếm sản phẩm");
Console.WriteLine("2. ExecuteNonQuery + SqlParameter - cập nhật tồn kho");
Console.WriteLine("3. ExecuteReader - đọc danh sách sản phẩm");
Console.WriteLine("4. SqlTransaction - mô phỏng Rollback an toàn");
Console.Write("Chọn bài (1-4): ");
var choice = Console.ReadLine();

try
{
    switch (choice)
    {
        case "1":
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT COUNT(*) FROM Products", connection))
            {
                connection.Open();
                var count = Convert.ToInt32(command.ExecuteScalar());
                Console.WriteLine($"Tổng số sản phẩm: {count}");
            }
            break;
        case "2":
            Console.Write("ProductId cần cập nhật: ");
            int id = int.Parse(Console.ReadLine() ?? "1");
            Console.Write("Tồn kho mới: ");
            int stock = int.Parse(Console.ReadLine() ?? "10");
            if (stock < 0) throw new ArgumentException("Tồn kho không được âm.");
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("UPDATE Products SET StockQuantity=@Stock WHERE ProductId=@Id", connection))
            {
                command.Parameters.Add("@Stock", SqlDbType.Int).Value = stock;
                command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                connection.Open();
                int rows = command.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? $"Đã cập nhật {rows} dòng." : "Không tìm thấy sản phẩm.");
            }
            break;
        case "3":
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "SELECT ProductId, Barcode, ProductName, Price, StockQuantity FROM Products ORDER BY ProductId", connection))
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                Console.WriteLine($"{"ID",-5} | {"MÃ VẠCH",-15} | {"TÊN SẢN PHẨM",-30} | {"GIÁ",12} | {"TỒN",6}");
                while (reader.Read())
                    Console.WriteLine($"{reader["ProductId"],-5} | {reader["Barcode"],-15} | {reader["ProductName"],-30} | {Convert.ToDecimal(reader["Price"]),12:N0} | {reader["StockQuantity"],6}");
            }
            break;
        case "4":
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    using (var command = new SqlCommand(
                        "UPDATE Products SET StockQuantity=StockQuantity-1 WHERE ProductId=@Id AND StockQuantity>0", connection, transaction))
                    {
                        command.Parameters.Add("@Id", SqlDbType.Int).Value = 1;
                        if (command.ExecuteNonQuery() != 1) throw new InvalidOperationException("Không có tồn kho cho sản phẩm ID 1.");
                    }
                    // Cố tình tạo lỗi để kiểm chứng rollback.
                    throw new InvalidOperationException("Lỗi mô phỏng: giao dịch bị hủy.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Rollback đã chạy. Tồn kho sản phẩm ID 1 được khôi phục.");
                }
            }
            break;
        default:
            Console.WriteLine("Lựa chọn không hợp lệ.");
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine("Lỗi: " + ex.Message);
    Console.WriteLine("Kiểm tra SQL Server, database, connection string và dữ liệu.");
}
Console.WriteLine("Nhấn phím bất kỳ để kết thúc...");
Console.ReadKey();
