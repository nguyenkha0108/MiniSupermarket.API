using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDalatFarmData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    MembershipRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Rau ăn lá", "Các loại rau lá tươi sạch" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Rau củ", "Các loại rau củ tươi" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Củ quả", "Cà chua, dưa leo, ớt chuông" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Trái cây Đà Lạt", "Trái cây tươi theo mùa" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Nấm tươi", "Các loại nấm tươi" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[] { 6, "Rau gia vị", "Hành lá và rau thơm" });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 1, "Đà Lạt, Lâm Đồng", "Nguyễn Văn An", "Vàng", "0901000001", 150 },
                    { 2, "Đà Lạt, Lâm Đồng", "Trần Thị Bình", "Bạc", "0901000002", 50 },
                    { 3, "Đức Trọng, Lâm Đồng", "Lê Hoàng Nam", "Chuẩn", "0901000003", 10 },
                    { 4, "Đà Lạt, Lâm Đồng", "Phạm Thị Lan", "Vàng", "0901000004", 220 },
                    { 5, "Bảo Lộc, Lâm Đồng", "Hoàng Minh Khang", "Chuẩn", "0901000005", 30 },
                    { 6, "Đà Lạt, Lâm Đồng", "Võ Ngọc Hà", "Bạc", "0901000006", 85 },
                    { 7, "Lạc Dương, Lâm Đồng", "Đặng Quốc Bảo", "Chuẩn", "0901000007", 15 },
                    { 8, "Đà Lạt, Lâm Đồng", "Bùi Thị Mai", "Vàng", "0901000008", 310 },
                    { 9, "Đơn Dương, Lâm Đồng", "Đỗ Thành Đạt", "Chuẩn", "0901000009", 40 },
                    { 10, "Đà Lạt, Lâm Đồng", "Ngô Thị Hương", "Bạc", "0901000010", 65 },
                    { 11, "Bảo Lộc, Lâm Đồng", "Dương Minh Đức", "Chuẩn", "0901000011", 20 },
                    { 12, "Đà Lạt, Lâm Đồng", "Phan Ngọc Trâm", "Vàng", "0901000012", 180 },
                    { 13, "Đức Trọng, Lâm Đồng", "Lý Tuấn Kiệt", "Bạc", "0901000013", 55 },
                    { 14, "Đà Lạt, Lâm Đồng", "Mai Thị Thảo", "Chuẩn", "0901000014", 5 },
                    { 15, "Lạc Dương, Lâm Đồng", "Cao Gia Huy", "Bạc", "0901000015", 100 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "8930000000011", 1, 25000m, "Xà lách Đà Lạt", 50 },
                    { 2, "8930000000028", 1, 30000m, "Cải bó xôi", 40 },
                    { 3, "8930000000035", 1, 22000m, "Cải thảo", 35 },
                    { 4, "8930000000042", 1, 18000m, "Bắp cải xanh", 45 },
                    { 5, "8930000000059", 2, 20000m, "Cà rốt Đà Lạt", 60 },
                    { 6, "8930000000066", 2, 25000m, "Khoai tây Đà Lạt", 55 },
                    { 7, "8930000000073", 2, 28000m, "Củ dền", 25 },
                    { 8, "8930000000080", 3, 30000m, "Cà chua", 50 },
                    { 9, "8930000000097", 3, 18000m, "Dưa leo", 45 },
                    { 10, "8930000000103", 3, 45000m, "Ớt chuông Đà Lạt", 30 },
                    { 11, "8930000000110", 4, 120000m, "Dâu tây Đà Lạt", 20 },
                    { 12, "8930000000127", 4, 65000m, "Hồng giòn Đà Lạt", 25 },
                    { 13, "8930000000134", 5, 40000m, "Nấm bào ngư", 30 },
                    { 14, "8930000000141", 5, 60000m, "Nấm đùi gà", 25 },
                    { 15, "8930000000158", 6, 15000m, "Hành lá", 35 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bánh kẹo & Đồ ăn vặt", "Snack, bánh quy, kẹo dẻo" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Nước giải khát & Trà", "Nước ngọt, nước khoáng, trà" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Sữa & Sản phẩm từ sữa", "Sữa tươi, sữa chua, phô mai" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Mì gói & Thực phẩm ăn liền", "Mì ăn liền, phở khô, cháo gói" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Gia vị & Dầu ăn", "Nước mắm, hạt nêm, dầu thực vật" });
        }
    }
}
