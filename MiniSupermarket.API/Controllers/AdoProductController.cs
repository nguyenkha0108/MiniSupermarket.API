using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Models;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/ado-products")]
    [ApiController]
    public class AdoProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AdoProductController(IConfiguration configuration) => _configuration = configuration;

        // GET: api/ado-products/barcode/8930000000011
        [HttpGet("barcode/{barcode}")]
        public IActionResult GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return BadRequest(new { message = "Mã vạch không được để trống!" });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            const string sql = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity
                                 FROM Products WHERE Barcode = @Barcode";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 50).Value = barcode.Trim();

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                if (!reader.Read())
                    return NotFound(new { message = $"Không tìm thấy sản phẩm có mã vạch: {barcode}" });

                return Ok(new ProductLookupDto
                {
                    ProductId = Convert.ToInt32(reader["ProductId"]),
                    Barcode = Convert.ToString(reader["Barcode"]) ?? "",
                    ProductName = Convert.ToString(reader["ProductName"]) ?? "",
                    Price = Convert.ToDecimal(reader["Price"]),
                    StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
            }
        }

        // GET: api/ado-products/disconnected-cache
        [HttpGet("disconnected-cache")]
        public IActionResult GetProductsDisconnected()
        {
            var table = new DataTable("CachedProducts");
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(
                    @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity
                      FROM Products ORDER BY ProductId ASC", connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                var data = table.Rows.Cast<DataRow>().Select(row => new ProductLookupDto
                {
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Barcode = Convert.ToString(row["Barcode"]) ?? "",
                    ProductName = Convert.ToString(row["ProductName"]) ?? "",
                    Price = Convert.ToDecimal(row["Price"]),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"])
                }).ToList();

                return Ok(new { totalLoaded = table.Rows.Count, cachedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
            }
        }

        // Bài tập mở rộng 2.1: GET api/ado-products/filter-stock?minStock=50
        [HttpGet("filter-stock")]
        public IActionResult FilterStock([FromQuery] int minStock = 50)
        {
            if (minStock < 0) return BadRequest(new { message = "minStock không được âm." });
            var result = new List<ProductLookupDto>();
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(
                @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity
                  FROM Products WHERE StockQuantity <= @MinStock ORDER BY StockQuantity ASC", connection);
            command.Parameters.Add("@MinStock", SqlDbType.Int).Value = minStock;
            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                    result.Add(new ProductLookupDto
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        Barcode = Convert.ToString(reader["Barcode"]) ?? "",
                        ProductName = Convert.ToString(reader["ProductName"]) ?? "",
                        Price = Convert.ToDecimal(reader["Price"]),
                        StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                    });
                return Ok(new { minStock, total = result.Count, data = result });
            }
            catch (SqlException ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
