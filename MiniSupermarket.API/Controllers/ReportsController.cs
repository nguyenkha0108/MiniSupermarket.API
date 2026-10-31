
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        // GET: api/Reports/revenue-by-cashier
        [HttpGet("revenue-by-cashier")]
        public IActionResult GetRevenueByCashier()
        {
            var reports = new List<CashierRevenueDto>();

            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var command = new SqlCommand(
                "sp_GetRevenueByCashier", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                connection.Open();

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reports.Add(new CashierRevenueDto
                    {
                        CashierUsername =
                            Convert.ToString(reader["CashierUsername"]) ?? "",

                        TotalOrders =
                            Convert.ToInt32(reader["TotalOrders"]),

                        TotalRevenue =
                            reader["TotalRevenue"] == DBNull.Value
                                ? 0m
                                : Convert.ToDecimal(reader["TotalRevenue"]),

                        TotalPointsAwarded =
                            reader["TotalPointsAwarded"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["TotalPointsAwarded"])
                    });
                }

                return Ok(new
                {
                    success = true,
                    generatedAt = DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm:ss"),
                    totalCashiers = reports.Count,
                    grandTotalRevenue = reports.Sum(
                        x => x.TotalRevenue),
                    data = reports
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Lỗi thực thi Stored Procedure: " + ex.Message
                });
            }
        }

        // GET: api/Reports/top5-selling-products
        [HttpGet("top5-selling-products")]
        public IActionResult GetTop5SellingProducts()
        {
            var data = new List<object>();

            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var command = new SqlCommand(
                "sp_GetTop5SellingProducts", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                connection.Open();

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    data.Add(new
                    {
                        productId =
                            Convert.ToInt32(reader["ProductId"]),

                        barcode =
                            Convert.ToString(reader["Barcode"]) ?? "",

                        productName =
                            Convert.ToString(reader["ProductName"]) ?? "",

                        totalQuantitySold =
                            Convert.ToInt32(reader["TotalQuantitySold"]),

                        totalRevenue =
                            reader["TotalRevenue"] == DBNull.Value
                                ? 0m
                                : Convert.ToDecimal(reader["TotalRevenue"])
                    });
                }

                return Ok(new
                {
                    success = true,
                    count = data.Count,
                    data
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // GET: api/Reports/revenue-by-date
        // Ví dụ: api/Reports/revenue-by-date
        //        ?fromDate=2026-10-01&toDate=2026-10-31
        [HttpGet("revenue-by-date")]
        public IActionResult GetRevenueByDate(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                return BadRequest(new
                {
                    message =
                        "fromDate phải nhỏ hơn hoặc bằng toDate."
                });
            }

            var data = new List<object>();

            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var command = new SqlCommand(
                "sp_GetRevenueByDateRange", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@FromDate", SqlDbType.DateTime)
                .Value = fromDate.Date;

            // Lấy toàn bộ ngày cuối, kể cả các giao dịch cuối ngày.
            command.Parameters.Add("@ToDate", SqlDbType.DateTime)
                .Value = toDate.Date.AddDays(1);

            try
            {
                connection.Open();

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    data.Add(new
                    {
                        cashierUsername =
                            Convert.ToString(reader["CashierUsername"]) ?? "",

                        totalOrders =
                            Convert.ToInt32(reader["TotalOrders"]),

                        totalRevenue =
                            reader["TotalRevenue"] == DBNull.Value
                                ? 0m
                                : Convert.ToDecimal(reader["TotalRevenue"])
                    });
                }

                return Ok(new
                {
                    success = true,
                    fromDate = fromDate.Date.ToString("yyyy-MM-dd"),
                    toDate = toDate.Date.ToString("yyyy-MM-dd"),
                    totalCashiers = data.Count,
                    data
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // GET: api/Reports/quick-report?date=2026-10-24
        // Dùng cho FormQuickReport.cs
        [HttpGet("quick-report")]
        public IActionResult GetQuickReport(
            [FromQuery] DateTime date)
        {
            DateTime startDate = date.Date;
            DateTime endDate = startDate.AddDays(1);

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Chưa cấu hình ConnectionStrings:DefaultConnection."
                });
            }

            try
            {
                using var connection =
                    new SqlConnection(connectionString);

                connection.Open();

                int totalOrders;
                decimal totalRevenue;

                // Thống kê số hóa đơn và tổng doanh thu.
                const string summarySql = @"
                    SELECT
                        COUNT(*) AS TotalOrders,
                        COALESCE(SUM(FinalAmount), 0) AS TotalRevenue
                    FROM Orders
                    WHERE CreatedDate >= @StartDate
                      AND CreatedDate < @EndDate;";

                using (var command = new SqlCommand(
                    summarySql, connection))
                {
                    command.Parameters.Add(
                        "@StartDate", SqlDbType.DateTime).Value = startDate;

                    command.Parameters.Add(
                        "@EndDate", SqlDbType.DateTime).Value = endDate;

                    using var reader = command.ExecuteReader();

                    if (!reader.Read())
                    {
                        totalOrders = 0;
                        totalRevenue = 0m;
                    }
                    else
                    {
                        totalOrders =
                            Convert.ToInt32(reader["TotalOrders"]);

                        totalRevenue =
                            Convert.ToDecimal(reader["TotalRevenue"]);
                    }
                }

                // Tìm sản phẩm bán chạy theo số lượng.
                string bestSeller = "Chưa có dữ liệu";

                const string bestSellerSql = @"
                    SELECT TOP 1
                        p.ProductName,
                        SUM(od.Quantity) AS TotalQuantity
                    FROM Orders o
                    INNER JOIN OrderDetails od
                        ON o.OrderId = od.OrderId
                    INNER JOIN Products p
                        ON od.ProductId = p.ProductId
                    WHERE o.CreatedDate >= @StartDate
                      AND o.CreatedDate < @EndDate
                    GROUP BY p.ProductId, p.ProductName
                    ORDER BY SUM(od.Quantity) DESC, p.ProductName ASC;";

                using (var command = new SqlCommand(
                    bestSellerSql, connection))
                {
                    command.Parameters.Add(
                        "@StartDate", SqlDbType.DateTime).Value = startDate;

                    command.Parameters.Add(
                        "@EndDate", SqlDbType.DateTime).Value = endDate;

                    using var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        bestSeller =
                            Convert.ToString(reader["ProductName"])
                            ?? "Chưa có dữ liệu";

                        bestSeller +=
                            $" ({Convert.ToInt32(reader["TotalQuantity"])} sản phẩm)";
                    }
                }

                return Ok(new
                {
                    success = true,
                    date = startDate.ToString("yyyy-MM-dd"),
                    totalOrders,
                    totalRevenue,
                    bestSeller
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Lỗi truy vấn báo cáo doanh thu: " + ex.Message
                });
            }
        }
    }
}
