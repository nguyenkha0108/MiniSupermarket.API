using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoryProductSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 7, "Rau hữu cơ", "Rau canh tác theo hướng hữu cơ" },
                    { 8, "Rau mầm", "Các loại rau mầm tươi" },
                    { 9, "Đậu và hạt", "Các loại đậu và hạt thực phẩm" },
                    { 10, "Trái cây nhập khẩu", "Trái cây nhập khẩu tươi ngon" },
                    { 11, "Trái cây nhiệt đới", "Các loại trái cây nhiệt đới" },
                    { 12, "Rau đông lạnh", "Rau củ bảo quản đông lạnh" },
                    { 13, "Củ quả sơ chế", "Rau củ đã sơ chế tiện lợi" },
                    { 14, "Nước ép và sinh tố", "Đồ uống từ trái cây và rau củ" },
                    { 15, "Combo rau củ", "Các combo rau củ tiện lợi" }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 2, 22000m, "Khoai lang", 40 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9,
                columns: new[] { "Price", "ProductName", "StockQuantity" },
                values: new object[] { 30000m, "Cà chua", 50 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10,
                columns: new[] { "Price", "ProductName", "StockQuantity" },
                values: new object[] { 18000m, "Dưa leo", 45 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 3, 45000m, "Ớt chuông Đà Lạt", 30 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 3, 32000m, "Bí ngòi xanh" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 4, 120000m, "Dâu tây Đà Lạt", 20 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 4, 65000m, "Hồng giòn Đà Lạt" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 4, 55000m, "Bơ Đà Lạt", 30 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 16, "8930000000165", 4, 35000m, "Chuối Laba", 35 },
                    { 17, "8930000000172", 5, 40000m, "Nấm bào ngư", 30 },
                    { 18, "8930000000189", 5, 60000m, "Nấm đùi gà", 25 },
                    { 19, "8930000000196", 5, 18000m, "Nấm kim châm", 40 },
                    { 20, "8930000000202", 5, 75000m, "Nấm hương tươi", 20 },
                    { 21, "8930000000219", 6, 15000m, "Hành lá", 35 },
                    { 22, "8930000000226", 6, 12000m, "Ngò rí", 30 },
                    { 23, "8930000000233", 6, 15000m, "Rau húng quế", 25 },
                    { 24, "8930000000240", 6, 14000m, "Ngò gai", 25 },
                    { 25, "8930000000257", 7, 35000m, "Cải ngọt hữu cơ", 30 },
                    { 26, "8930000000264", 7, 30000m, "Rau muống hữu cơ", 35 },
                    { 27, "8930000000271", 7, 40000m, "Xà lách hữu cơ", 25 },
                    { 28, "8930000000288", 7, 32000m, "Cải thìa hữu cơ", 25 },
                    { 29, "8930000000295", 8, 25000m, "Rau mầm cải đỏ", 20 },
                    { 30, "8930000000301", 8, 30000m, "Rau mầm hướng dương", 20 },
                    { 31, "8930000000318", 8, 22000m, "Rau mầm đậu xanh", 25 },
                    { 32, "8930000000325", 8, 25000m, "Rau mầm củ cải trắng", 20 },
                    { 33, "8930000000332", 9, 45000m, "Đậu Hà Lan", 30 },
                    { 34, "8930000000349", 9, 28000m, "Đậu cô ve", 35 },
                    { 35, "8930000000356", 9, 30000m, "Đậu bắp", 25 },
                    { 36, "8930000000363", 9, 26000m, "Đậu đũa", 25 },
                    { 37, "8930000000370", 10, 85000m, "Táo đỏ", 25 },
                    { 38, "8930000000387", 10, 150000m, "Nho xanh", 20 },
                    { 39, "8930000000394", 10, 95000m, "Lê Hàn Quốc", 20 },
                    { 40, "8930000000400", 10, 120000m, "Kiwi xanh", 15 },
                    { 41, "8930000000417", 11, 45000m, "Xoài cát", 35 },
                    { 42, "8930000000424", 11, 35000m, "Thanh long ruột đỏ", 30 },
                    { 43, "8930000000431", 11, 25000m, "Dứa mật", 30 },
                    { 44, "8930000000448", 11, 30000m, "Cam sành", 40 },
                    { 45, "8930000000455", 12, 35000m, "Bắp Mỹ đông lạnh", 25 },
                    { 46, "8930000000462", 12, 40000m, "Đậu Hà Lan đông lạnh", 20 },
                    { 47, "8930000000479", 12, 30000m, "Cà rốt cắt hạt lựu", 25 },
                    { 48, "8930000000486", 12, 45000m, "Rau củ hỗn hợp đông lạnh", 20 },
                    { 49, "8930000000493", 13, 30000m, "Khoai tây gọt vỏ", 20 },
                    { 50, "8930000000509", 13, 25000m, "Cà rốt cắt lát", 25 },
                    { 51, "8930000000516", 13, 28000m, "Bí đỏ cắt miếng", 20 },
                    { 52, "8930000000523", 13, 22000m, "Su su gọt vỏ", 20 },
                    { 53, "8930000000530", 14, 25000m, "Nước ép cà rốt", 30 },
                    { 54, "8930000000547", 14, 35000m, "Nước ép dâu tây", 25 },
                    { 55, "8930000000554", 14, 40000m, "Sinh tố bơ", 25 },
                    { 56, "8930000000561", 14, 28000m, "Nước ép cam", 30 },
                    { 57, "8930000000578", 15, 45000m, "Combo rau nấu canh", 20 },
                    { 58, "8930000000585", 15, 50000m, "Combo rau xào", 20 },
                    { 59, "8930000000592", 15, 65000m, "Combo rau lẩu", 15 },
                    { 60, "8930000000608", 15, 55000m, "Combo salad", 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 3, 30000m, "Cà chua", 50 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9,
                columns: new[] { "Price", "ProductName", "StockQuantity" },
                values: new object[] { 18000m, "Dưa leo", 45 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10,
                columns: new[] { "Price", "ProductName", "StockQuantity" },
                values: new object[] { 45000m, "Ớt chuông Đà Lạt", 30 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 4, 120000m, "Dâu tây Đà Lạt", 20 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 4, 65000m, "Hồng giòn Đà Lạt" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 5, 40000m, "Nấm bào ngư", 30 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 5, 60000m, "Nấm đùi gà" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 6, 15000m, "Hành lá", 35 });
        }
    }
}
