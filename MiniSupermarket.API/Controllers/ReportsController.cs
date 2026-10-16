using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Models;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ReportsController(IConfiguration configuration) => _configuration = configuration;

        // GET: api/Reports/revenue-by-cashier
        [HttpGet("revenue-by-cashier")]
        public IActionResult GetRevenueByCashier()
        {
            var reports = new List<CashierRevenueDto>();
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_GetRevenueByCashier", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                    reports.Add(new CashierRevenueDto
                    {
                        CashierUsername = Convert.ToString(reader["CashierUsername"]) ?? "",
                        TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                        TotalRevenue = reader["TotalRevenue"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalRevenue"]),
                        TotalPointsAwarded = reader["TotalPointsAwarded"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalPointsAwarded"])
                    });
                return Ok(new
                {
                    success = true,
                    generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    totalCashiers = reports.Count,
                    grandTotalRevenue = reports.Sum(x => x.TotalRevenue),
                    data = reports
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi thực thi Stored Procedure: " + ex.Message });
            }
        }


        // Bài tập mở rộng 3.2: GET api/Reports/top5-selling-products
        [HttpGet("top5-selling-products")]
        public IActionResult GetTop5SellingProducts()
        {
            var data = new List<object>();
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_GetTop5SellingProducts", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                    data.Add(new
                    {
                        productId = Convert.ToInt32(reader["ProductId"]),
                        barcode = Convert.ToString(reader["Barcode"]) ?? "",
                        productName = Convert.ToString(reader["ProductName"]) ?? "",
                        totalQuantitySold = Convert.ToInt32(reader["TotalQuantitySold"]),
                        totalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                    });
                return Ok(new { success = true, count = data.Count, data });
            }
            catch (SqlException ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        // Bài tập mở rộng 3.1: GET api/Reports/revenue-by-date?fromDate=2026-10-01&toDate=2026-10-31
        [HttpGet("revenue-by-date")]
        public IActionResult GetRevenueByDate([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (fromDate > toDate) return BadRequest(new { message = "fromDate phải nhỏ hơn hoặc bằng toDate." });
            var data = new List<object>();
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_GetRevenueByDateRange", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate;
            command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.Date.AddDays(1).AddTicks(-1);
            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                    data.Add(new
                    {
                        cashierUsername = Convert.ToString(reader["CashierUsername"]) ?? "",
                        totalOrders = Convert.ToInt32(reader["TotalOrders"]),
                        totalRevenue = reader["TotalRevenue"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["TotalRevenue"])
                    });
                return Ok(new { fromDate, toDate, totalCashiers = data.Count, data });
            }
            catch (SqlException ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
