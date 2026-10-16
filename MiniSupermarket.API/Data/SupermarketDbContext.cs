
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(
            DbContextOptions<SupermarketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. DANH MỤC RAU SẠCH ĐÀ LẠT FARM
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Rau ăn lá",
                    Description = "Các loại rau lá tươi sạch"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Rau củ",
                    Description = "Các loại rau củ tươi"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Củ quả",
                    Description = "Cà chua, dưa leo, ớt chuông"
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Trái cây Đà Lạt",
                    Description = "Trái cây tươi theo mùa"
                },
                new Category
                {
                    CategoryId = 5,
                    CategoryName = "Nấm tươi",
                    Description = "Các loại nấm tươi"
                },
                new Category
                {
                    CategoryId = 6,
                    CategoryName = "Rau gia vị",
                    Description = "Hành lá và rau thơm"
                },
                new Category
                {
                    CategoryId = 7,
                    CategoryName = "Rau hữu cơ",
                    Description = "Rau canh tác theo hướng hữu cơ"
                },
                new Category
                {
                    CategoryId = 8,
                    CategoryName = "Rau mầm",
                    Description = "Các loại rau mầm tươi"
                },
                new Category
                {
                    CategoryId = 9,
                    CategoryName = "Đậu và hạt",
                    Description = "Các loại đậu và hạt thực phẩm"
                },
                new Category
                {
                    CategoryId = 10,
                    CategoryName = "Trái cây nhập khẩu",
                    Description = "Trái cây nhập khẩu tươi ngon"
                },
                new Category
                {
                    CategoryId = 11,
                    CategoryName = "Trái cây nhiệt đới",
                    Description = "Các loại trái cây nhiệt đới"
                },
                new Category
                {
                    CategoryId = 12,
                    CategoryName = "Rau đông lạnh",
                    Description = "Rau củ bảo quản đông lạnh"
                },
                new Category
                {
                    CategoryId = 13,
                    CategoryName = "Củ quả sơ chế",
                    Description = "Rau củ đã sơ chế tiện lợi"
                },
                new Category
                {
                    CategoryId = 14,
                    CategoryName = "Nước ép và sinh tố",
                    Description = "Đồ uống từ trái cây và rau củ"
                },
                new Category
                {
                    CategoryId = 15,
                    CategoryName = "Combo rau củ",
                    Description = "Các combo rau củ tiện lợi"
                }
            );

            // 2. SẢN PHẨM
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Barcode = "8930000000011", ProductName = "Xà lách Đà Lạt", Price = 25000m, StockQuantity = 50, CategoryId = 1 },
                new Product { ProductId = 2, Barcode = "8930000000028", ProductName = "Cải bó xôi", Price = 30000m, StockQuantity = 40, CategoryId = 1 },
                new Product { ProductId = 3, Barcode = "8930000000035", ProductName = "Cải thảo", Price = 22000m, StockQuantity = 35, CategoryId = 1 },
                new Product { ProductId = 4, Barcode = "8930000000042", ProductName = "Bắp cải xanh", Price = 18000m, StockQuantity = 45, CategoryId = 1 },
                new Product { ProductId = 5, Barcode = "8930000000059", ProductName = "Cà rốt Đà Lạt", Price = 20000m, StockQuantity = 60, CategoryId = 2 },
                new Product { ProductId = 6, Barcode = "8930000000066", ProductName = "Khoai tây Đà Lạt", Price = 25000m, StockQuantity = 55, CategoryId = 2 },
                new Product { ProductId = 7, Barcode = "8930000000073", ProductName = "Củ dền", Price = 28000m, StockQuantity = 25, CategoryId = 2 },
                new Product { ProductId = 8, Barcode = "8930000000080", ProductName = "Cà chua", Price = 30000m, StockQuantity = 50, CategoryId = 3 },
                new Product { ProductId = 9, Barcode = "8930000000097", ProductName = "Dưa leo", Price = 18000m, StockQuantity = 45, CategoryId = 3 },
                new Product { ProductId = 10, Barcode = "8930000000103", ProductName = "Ớt chuông Đà Lạt", Price = 45000m, StockQuantity = 30, CategoryId = 3 },
                new Product { ProductId = 11, Barcode = "8930000000110", ProductName = "Dâu tây Đà Lạt", Price = 120000m, StockQuantity = 20, CategoryId = 4 },
                new Product { ProductId = 12, Barcode = "8930000000127", ProductName = "Hồng giòn Đà Lạt", Price = 65000m, StockQuantity = 25, CategoryId = 4 },
                new Product { ProductId = 13, Barcode = "8930000000134", ProductName = "Nấm bào ngư", Price = 40000m, StockQuantity = 30, CategoryId = 5 },
                new Product { ProductId = 14, Barcode = "8930000000141", ProductName = "Nấm đùi gà", Price = 60000m, StockQuantity = 25, CategoryId = 5 },
                new Product { ProductId = 15, Barcode = "8930000000158", ProductName = "Hành lá", Price = 15000m, StockQuantity = 35, CategoryId = 6 },

                new Product { ProductId = 16, Barcode = "8930000000165", ProductName = "Chuối Laba", Price = 35000m, StockQuantity = 35, CategoryId = 4 },
                new Product { ProductId = 17, Barcode = "8930000000172", ProductName = "Nấm bào ngư loại lớn", Price = 40000m, StockQuantity = 30, CategoryId = 5 },
                new Product { ProductId = 18, Barcode = "8930000000189", ProductName = "Nấm đùi gà loại lớn", Price = 60000m, StockQuantity = 25, CategoryId = 5 },
                new Product { ProductId = 19, Barcode = "8930000000196", ProductName = "Nấm kim châm", Price = 18000m, StockQuantity = 40, CategoryId = 5 },
                new Product { ProductId = 20, Barcode = "8930000000202", ProductName = "Nấm hương tươi", Price = 75000m, StockQuantity = 20, CategoryId = 5 },
                new Product { ProductId = 21, Barcode = "8930000000219", ProductName = "Hành lá loại 2", Price = 15000m, StockQuantity = 35, CategoryId = 6 },
                new Product { ProductId = 22, Barcode = "8930000000226", ProductName = "Ngò rí", Price = 12000m, StockQuantity = 30, CategoryId = 6 },
                new Product { ProductId = 23, Barcode = "8930000000233", ProductName = "Rau húng quế", Price = 15000m, StockQuantity = 25, CategoryId = 6 },
                new Product { ProductId = 24, Barcode = "8930000000240", ProductName = "Ngò gai", Price = 14000m, StockQuantity = 25, CategoryId = 6 },
                new Product { ProductId = 25, Barcode = "8930000000257", ProductName = "Cải ngọt hữu cơ", Price = 35000m, StockQuantity = 30, CategoryId = 7 },
                new Product { ProductId = 26, Barcode = "8930000000264", ProductName = "Rau muống hữu cơ", Price = 30000m, StockQuantity = 35, CategoryId = 7 },
                new Product { ProductId = 27, Barcode = "8930000000271", ProductName = "Xà lách hữu cơ", Price = 40000m, StockQuantity = 25, CategoryId = 7 },
                new Product { ProductId = 28, Barcode = "8930000000288", ProductName = "Cải thìa hữu cơ", Price = 32000m, StockQuantity = 25, CategoryId = 7 },
                new Product { ProductId = 29, Barcode = "8930000000295", ProductName = "Rau mầm cải đỏ", Price = 25000m, StockQuantity = 20, CategoryId = 8 },
                new Product { ProductId = 30, Barcode = "8930000000301", ProductName = "Rau mầm hướng dương", Price = 30000m, StockQuantity = 20, CategoryId = 8 },
                new Product { ProductId = 31, Barcode = "8930000000318", ProductName = "Rau mầm đậu xanh", Price = 22000m, StockQuantity = 25, CategoryId = 8 },
                new Product { ProductId = 32, Barcode = "8930000000325", ProductName = "Rau mầm củ cải trắng", Price = 25000m, StockQuantity = 20, CategoryId = 8 },
                new Product { ProductId = 33, Barcode = "8930000000332", ProductName = "Đậu Hà Lan", Price = 45000m, StockQuantity = 30, CategoryId = 9 },
                new Product { ProductId = 34, Barcode = "8930000000349", ProductName = "Đậu cô ve", Price = 28000m, StockQuantity = 35, CategoryId = 9 },
                new Product { ProductId = 35, Barcode = "8930000000356", ProductName = "Đậu bắp", Price = 30000m, StockQuantity = 25, CategoryId = 9 },
                new Product { ProductId = 36, Barcode = "8930000000363", ProductName = "Đậu đũa", Price = 26000m, StockQuantity = 25, CategoryId = 9 },
                new Product { ProductId = 37, Barcode = "8930000000370", ProductName = "Táo đỏ", Price = 85000m, StockQuantity = 25, CategoryId = 10 },
                new Product { ProductId = 38, Barcode = "8930000000387", ProductName = "Nho xanh", Price = 150000m, StockQuantity = 20, CategoryId = 10 },
                new Product { ProductId = 39, Barcode = "8930000000394", ProductName = "Lê Hàn Quốc", Price = 95000m, StockQuantity = 20, CategoryId = 10 },
                new Product { ProductId = 40, Barcode = "8930000000400", ProductName = "Kiwi xanh", Price = 120000m, StockQuantity = 15, CategoryId = 10 },
                new Product { ProductId = 41, Barcode = "8930000000417", ProductName = "Xoài cát", Price = 45000m, StockQuantity = 35, CategoryId = 11 },
                new Product { ProductId = 42, Barcode = "8930000000424", ProductName = "Thanh long ruột đỏ", Price = 35000m, StockQuantity = 30, CategoryId = 11 },
                new Product { ProductId = 43, Barcode = "8930000000431", ProductName = "Dứa mật", Price = 25000m, StockQuantity = 30, CategoryId = 11 },
                new Product { ProductId = 44, Barcode = "8930000000448", ProductName = "Cam sành", Price = 30000m, StockQuantity = 40, CategoryId = 11 },
                new Product { ProductId = 45, Barcode = "8930000000455", ProductName = "Bắp Mỹ đông lạnh", Price = 35000m, StockQuantity = 25, CategoryId = 12 },
                new Product { ProductId = 46, Barcode = "8930000000462", ProductName = "Đậu Hà Lan đông lạnh", Price = 40000m, StockQuantity = 20, CategoryId = 12 },
                new Product { ProductId = 47, Barcode = "8930000000479", ProductName = "Cà rốt cắt hạt lựu", Price = 30000m, StockQuantity = 25, CategoryId = 12 },
                new Product { ProductId = 48, Barcode = "8930000000486", ProductName = "Rau củ hỗn hợp đông lạnh", Price = 45000m, StockQuantity = 20, CategoryId = 12 },
                new Product { ProductId = 49, Barcode = "8930000000493", ProductName = "Khoai tây gọt vỏ", Price = 30000m, StockQuantity = 20, CategoryId = 13 },
                new Product { ProductId = 50, Barcode = "8930000000509", ProductName = "Cà rốt cắt lát", Price = 25000m, StockQuantity = 25, CategoryId = 13 },
                new Product { ProductId = 51, Barcode = "8930000000516", ProductName = "Bí đỏ cắt miếng", Price = 28000m, StockQuantity = 20, CategoryId = 13 },
                new Product { ProductId = 52, Barcode = "8930000000523", ProductName = "Su su gọt vỏ", Price = 22000m, StockQuantity = 20, CategoryId = 13 },
                new Product { ProductId = 53, Barcode = "8930000000530", ProductName = "Nước ép cà rốt", Price = 25000m, StockQuantity = 30, CategoryId = 14 },
                new Product { ProductId = 54, Barcode = "8930000000547", ProductName = "Nước ép dâu tây", Price = 35000m, StockQuantity = 25, CategoryId = 14 },
                new Product { ProductId = 55, Barcode = "8930000000554", ProductName = "Sinh tố bơ", Price = 40000m, StockQuantity = 25, CategoryId = 14 },
                new Product { ProductId = 56, Barcode = "8930000000561", ProductName = "Nước ép cam", Price = 28000m, StockQuantity = 30, CategoryId = 14 },
                new Product { ProductId = 57, Barcode = "8930000000578", ProductName = "Combo rau nấu canh", Price = 45000m, StockQuantity = 20, CategoryId = 15 },
                new Product { ProductId = 58, Barcode = "8930000000585", ProductName = "Combo rau xào", Price = 50000m, StockQuantity = 20, CategoryId = 15 },
                new Product { ProductId = 59, Barcode = "8930000000592", ProductName = "Combo rau lẩu", Price = 65000m, StockQuantity = 15, CategoryId = 15 },
                new Product { ProductId = 60, Barcode = "8930000000608", ProductName = "Combo salad", Price = 55000m, StockQuantity = 20, CategoryId = 15 }
            );

            // 3. KHÁCH HÀNG
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Nguyễn Văn An", PhoneNumber = "0901000001", Address = "12 Nguyễn Văn Cừ, Đà Lạt, Lâm Đồng", RewardPoints = 150, MembershipRank = "Vàng" },
                new Customer { CustomerId = 2, CustomerName = "Trần Thị Bình", PhoneNumber = "0901000002", Address = "25 Phan Đình Phùng, Đà Lạt, Lâm Đồng", RewardPoints = 50, MembershipRank = "Bạc" },
                new Customer { CustomerId = 3, CustomerName = "Lê Hoàng Nam", PhoneNumber = "0901000003", Address = "38 Thống Nhất, Đức Trọng, Lâm Đồng", RewardPoints = 10, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 4, CustomerName = "Phạm Thị Lan", PhoneNumber = "0901000004", Address = "46 Hai Bà Trưng, Đà Lạt, Lâm Đồng", RewardPoints = 220, MembershipRank = "Vàng" },
                new Customer { CustomerId = 5, CustomerName = "Hoàng Minh Khang", PhoneNumber = "0901000005", Address = "19 Trần Phú, Bảo Lộc, Lâm Đồng", RewardPoints = 30, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 6, CustomerName = "Võ Ngọc Hà", PhoneNumber = "0901000006", Address = "57 Bùi Thị Xuân, Đà Lạt, Lâm Đồng", RewardPoints = 85, MembershipRank = "Bạc" },
                new Customer { CustomerId = 7, CustomerName = "Đặng Quốc Bảo", PhoneNumber = "0901000007", Address = "23 Lang Biang, Lạc Dương, Lâm Đồng", RewardPoints = 15, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 8, CustomerName = "Bùi Thị Mai", PhoneNumber = "0901000008", Address = "68 Xô Viết Nghệ Tĩnh, Đà Lạt, Lâm Đồng", RewardPoints = 310, MembershipRank = "Vàng" },
                new Customer { CustomerId = 9, CustomerName = "Đỗ Thành Đạt", PhoneNumber = "0901000009", Address = "31 Lê Lợi, Đơn Dương, Lâm Đồng", RewardPoints = 40, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 10, CustomerName = "Ngô Thị Hương", PhoneNumber = "0901000010", Address = "42 Trần Hưng Đạo, Đà Lạt, Lâm Đồng", RewardPoints = 65, MembershipRank = "Bạc" },
                new Customer { CustomerId = 11, CustomerName = "Dương Minh Đức", PhoneNumber = "0901000011", Address = "15 Nguyễn Công Trứ, Bảo Lộc, Lâm Đồng", RewardPoints = 20, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 12, CustomerName = "Phan Ngọc Trâm", PhoneNumber = "0901000012", Address = "72 Hoàng Diệu, Đà Lạt, Lâm Đồng", RewardPoints = 180, MembershipRank = "Vàng" },
                new Customer { CustomerId = 13, CustomerName = "Lý Tuấn Kiệt", PhoneNumber = "0901000013", Address = "29 Quốc lộ 20, Đức Trọng, Lâm Đồng", RewardPoints = 55, MembershipRank = "Bạc" },
                new Customer { CustomerId = 14, CustomerName = "Mai Thị Thảo", PhoneNumber = "0901000014", Address = "36 Pasteur, Đà Lạt, Lâm Đồng", RewardPoints = 5, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 15, CustomerName = "Cao Gia Huy", PhoneNumber = "0901000015", Address = "54 đường 19/5, Lạc Dương, Lâm Đồng", RewardPoints = 100, MembershipRank = "Bạc" }
            );
        }
    }
}
