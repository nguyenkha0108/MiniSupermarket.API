using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 1. API cung cấp dữ liệu cho 4 thẻ KPI Dashboard
        // GET: api/Reports/dashboard-summary
        [HttpGet("dashboard-summary")]
        public IActionResult GetDashboardSummary()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return StatusCode(500, new { success = false, message = "Chưa cấu hình ConnectionString." });
            }

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Doanh thu hôm nay
                decimal todayRevenue = 0m;
                const string todayRevSql = @"
                    SELECT COALESCE(SUM(FinalAmount), 0) 
                    FROM Orders 
                    WHERE CreatedDate >= CAST(GETDATE() AS DATE) 
                      AND CreatedDate < DATEADD(day, 1, CAST(GETDATE() AS DATE));";
                using (var cmd = new SqlCommand(todayRevSql, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        todayRevenue = Convert.ToDecimal(result);
                }

                // Tổng số hóa đơn
                int totalInvoices = 0;
                const string totalInvSql = "SELECT COUNT(*) FROM Orders;";
                using (var cmd = new SqlCommand(totalInvSql, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        totalInvoices = Convert.ToInt32(result);
                }

                // Khách hàng mới (30 ngày gần đây tính theo đơn hàng)
                int newCustomers30Days = 0;
                const string newCustSql = @"
                    SELECT COUNT(DISTINCT CustomerId) 
                    FROM Orders 
                    WHERE CreatedDate >= DATEADD(day, -30, GETDATE()) AND CustomerId IS NOT NULL;";
                using (var cmd = new SqlCommand(newCustSql, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        newCustomers30Days = Convert.ToInt32(result);
                }

                // Sắp hết hàng (Tồn kho <= 10)
                int lowStockProducts = 0;
                const string lowStockSql = "SELECT COUNT(*) FROM Products WHERE StockQuantity <= 10;";
                using (var cmd = new SqlCommand(lowStockSql, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        lowStockProducts = Convert.ToInt32(result);
                }

                return Ok(new
                {
                    todayRevenue,
                    totalInvoices,
                    newCustomers30Days,
                    lowStockProducts
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // 2. API cung cấp doanh thu theo ngày cho biểu đồ
        // GET: api/Reports/daily-revenue?fromDate=2026-10-01&toDate=2026-10-31
        [HttpGet("daily-revenue")]
        public IActionResult GetDailyRevenue([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                return BadRequest(new { message = "fromDate phải nhỏ hơn hoặc bằng toDate." });
            }

            var items = new List<object>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string sql = @"
                    SELECT 
                        CAST(CreatedDate AS DATE) AS OrderDate,
                        COUNT(*) AS TotalOrders,
                        SUM(FinalAmount) AS TotalRevenue
                    FROM Orders
                    WHERE CreatedDate >= @FromDate AND CreatedDate < @ToDate
                    GROUP BY CAST(CreatedDate AS DATE)
                    ORDER BY OrderDate ASC;";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate.Date;
                // Cộng thêm 1 ngày để bao trọn mốc thời gian kết thúc
                command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.Date.AddDays(1);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dateVal = Convert.ToDateTime(reader["OrderDate"]);
                    items.Add(new
                    {
                        date = dateVal.ToString("yyyy-MM-dd"),
                        totalOrders = Convert.ToInt32(reader["TotalOrders"]),
                        totalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                    });
                }

                // Trả về đối tượng bọc có thuộc tính "Data" khớp với định dạng DailyRevenueResponse ở WinForms
                return Ok(new { data = items });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // 3. API cung cấp Top 5 sản phẩm bán chạy cho bảng DataGridView
        // GET: api/Reports/top5-selling-products
        [HttpGet("top5-selling-products")]
        public IActionResult GetTop5SellingProducts()
        {
            var data = new List<object>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string sql = @"
                    SELECT TOP 5
                        p.ProductId,
                        p.Barcode,
                        p.ProductName,
                        SUM(od.Quantity) AS TotalQuantitySold,
                        SUM(od.Quantity * od.UnitPrice) AS TotalRevenue
                    FROM OrderDetails od
                    INNER JOIN Products p ON od.ProductId = p.ProductId
                    GROUP BY p.ProductId, p.Barcode, p.ProductName
                    ORDER BY TotalQuantitySold DESC;";

                using var command = new SqlCommand(sql, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    data.Add(new
                    {
                        productId = Convert.ToInt32(reader["ProductId"]),
                        barcode = Convert.ToString(reader["Barcode"]) ?? "",
                        productName = Convert.ToString(reader["ProductName"]) ?? "",
                        totalQuantitySold = Convert.ToInt32(reader["TotalQuantitySold"]),
                        totalRevenue = reader["TotalRevenue"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["TotalRevenue"])
                    });
                }

                return Ok(new { data });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}