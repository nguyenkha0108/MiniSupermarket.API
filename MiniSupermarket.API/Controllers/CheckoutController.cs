using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Models;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CheckoutController(IConfiguration configuration) => _configuration = configuration;

        // POST: api/checkout
        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(new { success = false, message = "Giỏ hàng không được trống." });
            if (request.Items.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
                return BadRequest(new { success = false, message = "Mỗi sản phẩm chỉ được xuất hiện một lần trong giỏ hàng." });

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Xác nhận khách hàng tồn tại trước khi ghi hóa đơn.
                using (var customerCheck = new SqlCommand(
                    "SELECT COUNT(1) FROM Customers WHERE CustomerId = @CustomerId", connection, transaction))
                {
                    customerCheck.Parameters.Add("@CustomerId", SqlDbType.Int).Value = request.CustomerId;
                    if (Convert.ToInt32(customerCheck.ExecuteScalar()) == 0)
                        throw new InvalidOperationException($"Không tìm thấy khách hàng ID = {request.CustomerId}.");
                }

                decimal finalAmount = 0m;
                int totalItems = 0;
                var verifiedItems = new List<(int ProductId, int Quantity, decimal UnitPrice, string Name)>();

                // Đọc giá thật từ CSDL; không tin giá do client gửi lên.
                foreach (var item in request.Items)
                {
                    using var cmd = new SqlCommand(
                        "SELECT ProductName, Price, StockQuantity FROM Products WITH (UPDLOCK, ROWLOCK) WHERE ProductId = @ProductId",
                        connection, transaction);
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    using var reader = cmd.ExecuteReader();
                    if (!reader.Read())
                        throw new InvalidOperationException($"Không tìm thấy sản phẩm có ID: {item.ProductId}.");
                    var name = Convert.ToString(reader["ProductName"]) ?? "";
                    var price = Convert.ToDecimal(reader["Price"]);
                    var stock = Convert.ToInt32(reader["StockQuantity"]);
                    reader.Close();
                    if (stock < item.Quantity)
                        throw new InvalidOperationException($"Sản phẩm '{name}' (ID: {item.ProductId}) không đủ tồn kho! (Tồn kho thực tế: {stock}, Số lượng yêu cầu mua: {item.Quantity})");
                    verifiedItems.Add((item.ProductId, item.Quantity, price, name));
                    finalAmount += price * item.Quantity;
                    totalItems += item.Quantity;
                }

                var rewardPoints = (int)(finalAmount / 10000m);
                var orderCode = $"HD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}";
                int orderId;
                const string insertOrder = @"
                    INSERT INTO Orders (OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
                    OUTPUT INSERTED.OrderId
                    VALUES (@OrderCode, GETDATE(), @CashierUsername, @CustomerId, @TotalItems, @FinalAmount, @RewardPoints)";
                using (var cmd = new SqlCommand(insertOrder, connection, transaction))
                {
                    cmd.Parameters.Add("@OrderCode", SqlDbType.VarChar, 30).Value = orderCode;
                    cmd.Parameters.Add("@CashierUsername", SqlDbType.VarChar, 50).Value = request.CashierUsername.Trim();
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = request.CustomerId;
                    cmd.Parameters.Add("@TotalItems", SqlDbType.Int).Value = totalItems;
                    cmd.Parameters.Add("@FinalAmount", SqlDbType.Decimal).Value = finalAmount;
                    cmd.Parameters["@FinalAmount"].Precision = 18;
                    cmd.Parameters["@FinalAmount"].Scale = 2;
                    cmd.Parameters.Add("@RewardPoints", SqlDbType.Int).Value = rewardPoints;
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var item in verifiedItems)
                {
                    using (var detail = new SqlCommand(
                        @"INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, LineTotal)
                          VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @LineTotal)", connection, transaction))
                    {
                        detail.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                        detail.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                        detail.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                        detail.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = item.UnitPrice;
                        detail.Parameters["@UnitPrice"].Precision = 18; detail.Parameters["@UnitPrice"].Scale = 2;
                        detail.Parameters.Add("@LineTotal", SqlDbType.Decimal).Value = item.UnitPrice * item.Quantity;
                        detail.Parameters["@LineTotal"].Precision = 18; detail.Parameters["@LineTotal"].Scale = 2;
                        detail.ExecuteNonQuery();
                    }
                    using (var stock = new SqlCommand(
                        "UPDATE Products SET StockQuantity = StockQuantity - @Qty WHERE ProductId = @ProductId AND StockQuantity >= @Qty",
                        connection, transaction))
                    {
                        stock.Parameters.Add("@Qty", SqlDbType.Int).Value = item.Quantity;
                        stock.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                        if (stock.ExecuteNonQuery() != 1)
                            throw new InvalidOperationException($"Không thể trừ tồn kho sản phẩm ID = {item.ProductId}.");
                    }
                }

                if (rewardPoints > 0)
                {
                    using var points = new SqlCommand(
                        "UPDATE Customers SET RewardPoints = RewardPoints + @Points WHERE CustomerId = @CustomerId",
                        connection, transaction);
                    points.Parameters.Add("@Points", SqlDbType.Int).Value = rewardPoints;
                    points.Parameters.Add("@CustomerId", SqlDbType.Int).Value = request.CustomerId;
                    points.ExecuteNonQuery();
                }

                transaction.Commit();
                return Ok(new { success = true, message = "Thanh toán hóa đơn POS thành công!", orderId, orderCode, totalItems, finalAmount, rewardPointsAdded = rewardPoints });
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); }
                catch (Exception rollbackEx)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi nghiêm trọng khi thực thi Rollback: " + rollbackEx.Message });
                }
                return BadRequest(new { success = false, message = "Giao dịch thanh toán bị hủy và đã được Rollback an toàn!", errorDetail = ex.Message });
            }
        }

        // GET: api/checkout/orders?search=HD-20261016
        // Danh sách hóa đơn đã lưu trong SQL Server.
        [HttpGet("orders")]
        public IActionResult GetOrders([FromQuery] string? search = null)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            const string sql = @"
                SELECT TOP (500)
                    o.OrderId, o.OrderCode, o.CreatedDate, o.CashierUsername,
                    o.CustomerId, o.TotalItems, o.FinalAmount, o.RewardPoints
                FROM Orders o
                WHERE (@Search IS NULL OR @Search = ''
                    OR o.OrderCode LIKE '%' + @Search + '%'
                    OR o.CashierUsername LIKE '%' + @Search + '%'
                    OR CONVERT(VARCHAR(20), o.CustomerId) LIKE '%' + @Search + '%')
                ORDER BY o.CreatedDate DESC, o.OrderId DESC;";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = (object?)search?.Trim() ?? DBNull.Value;
            var rows = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new
                {
                    orderId = Convert.ToInt32(reader["OrderId"]),
                    orderCode = Convert.ToString(reader["OrderCode"]),
                    createdDate = Convert.ToDateTime(reader["CreatedDate"]),
                    cashierUsername = Convert.ToString(reader["CashierUsername"]),
                    customerId = Convert.ToInt32(reader["CustomerId"]),
                    totalItems = Convert.ToInt32(reader["TotalItems"]),
                    finalAmount = Convert.ToDecimal(reader["FinalAmount"]),
                    rewardPoints = Convert.ToInt32(reader["RewardPoints"])
                });
            }
            return Ok(rows);
        }

        // GET: api/checkout/orders/1
        // Chi tiết các sản phẩm trong một hóa đơn.
        [HttpGet("orders/{orderId:int}")]
        public IActionResult GetOrderDetails(int orderId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            using (var check = new SqlCommand("SELECT COUNT(1) FROM Orders WHERE OrderId = @OrderId", connection))
            {
                check.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                    return NotFound(new { message = $"Không tìm thấy hóa đơn ID = {orderId}." });
            }
            const string sql = @"
                SELECT od.OrderDetailId, od.ProductId, p.ProductName,
                       od.Quantity, od.UnitPrice, od.LineTotal
                FROM OrderDetails od
                LEFT JOIN Products p ON p.ProductId = od.ProductId
                WHERE od.OrderId = @OrderId
                ORDER BY od.OrderDetailId;";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
            var rows = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new
                {
                    orderDetailId = Convert.ToInt32(reader["OrderDetailId"]),
                    productId = Convert.ToInt32(reader["ProductId"]),
                    productName = reader["ProductName"] == DBNull.Value ? "(Sản phẩm đã bị xóa)" : Convert.ToString(reader["ProductName"]),
                    quantity = Convert.ToInt32(reader["Quantity"]),
                    unitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                    lineTotal = Convert.ToDecimal(reader["LineTotal"])
                });
            }
            return Ok(rows);
        }

        // DELETE: api/checkout/cancel/1
        [HttpDelete("cancel/{orderId:int}")]
        public IActionResult CancelOrder(int orderId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                int customerId, pointsToDeduct;
                using (var cmd = new SqlCommand(
                    "SELECT CustomerId, RewardPoints FROM Orders WHERE OrderId = @OrderId", connection, transaction))
                {
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    using var reader = cmd.ExecuteReader();
                    if (!reader.Read()) throw new InvalidOperationException($"Không tìm thấy hóa đơn ID = {orderId} để hủy!");
                    customerId = Convert.ToInt32(reader["CustomerId"]);
                    pointsToDeduct = Convert.ToInt32(reader["RewardPoints"]);
                }

                var items = new List<(int ProductId, int Quantity)>();
                using (var cmd = new SqlCommand(
                    "SELECT ProductId, Quantity FROM OrderDetails WHERE OrderId = @OrderId", connection, transaction))
                {
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read()) items.Add((Convert.ToInt32(reader["ProductId"]), Convert.ToInt32(reader["Quantity"])));
                }

                foreach (var item in items)
                {
                    using var cmd = new SqlCommand(
                        "UPDATE Products SET StockQuantity = StockQuantity + @Qty WHERE ProductId = @ProductId", connection, transaction);
                    cmd.Parameters.Add("@Qty", SqlDbType.Int).Value = item.Quantity;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException($"Không thể hoàn kho sản phẩm ID = {item.ProductId}.");
                }

                if (pointsToDeduct > 0)
                {
                    using var cmd = new SqlCommand(
                        @"UPDATE Customers SET RewardPoints = CASE WHEN RewardPoints >= @Pts THEN RewardPoints - @Pts ELSE 0 END
                          WHERE CustomerId = @CustomerId", connection, transaction);
                    cmd.Parameters.Add("@Pts", SqlDbType.Int).Value = pointsToDeduct;
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("DELETE FROM OrderDetails WHERE OrderId = @OrderId", connection, transaction))
                {
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId; cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Orders WHERE OrderId = @OrderId", connection, transaction))
                {
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId; cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return Ok(new { success = true, message = $"Đã hủy thành công hóa đơn #{orderId} và hoàn trả toàn bộ tồn kho vào CSDL!" });
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); }
                catch (Exception rollbackEx) { return StatusCode(500, new { success = false, message = "Rollback thất bại: " + rollbackEx.Message }); }
                return BadRequest(new { success = false, message = "Hủy hóa đơn thất bại, giao dịch đã được Rollback!", errorDetail = ex.Message });
            }
        }
    }
}
