
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
                new Category { CategoryId = 1, CategoryName = "Rau ăn lá", Description = "Các loại rau lá tươi sạch" },
                new Category { CategoryId = 2, CategoryName = "Rau củ", Description = "Các loại rau củ tươi" },
                new Category { CategoryId = 3, CategoryName = "Củ quả", Description = "Cà chua, dưa leo, ớt chuông" },
                new Category { CategoryId = 4, CategoryName = "Trái cây Đà Lạt", Description = "Trái cây tươi theo mùa" },
                new Category { CategoryId = 5, CategoryName = "Nấm tươi", Description = "Các loại nấm tươi" },
                new Category { CategoryId = 6, CategoryName = "Rau gia vị", Description = "Hành lá và rau thơm" }
            );

            // 2. 15 SẢN PHẨM
            // Giá bán tính bằng VNĐ; CategoryId phải tồn tại trong Categories.
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Barcode = "8930000000011", ProductName = "Xà lách Đà Lạt", Price = 25000, StockQuantity = 50, CategoryId = 1 },
                new Product { ProductId = 2, Barcode = "8930000000028", ProductName = "Cải bó xôi", Price = 30000, StockQuantity = 40, CategoryId = 1 },
                new Product { ProductId = 3, Barcode = "8930000000035", ProductName = "Cải thảo", Price = 22000, StockQuantity = 35, CategoryId = 1 },
                new Product { ProductId = 4, Barcode = "8930000000042", ProductName = "Bắp cải xanh", Price = 18000, StockQuantity = 45, CategoryId = 1 },
                new Product { ProductId = 5, Barcode = "8930000000059", ProductName = "Cà rốt Đà Lạt", Price = 20000, StockQuantity = 60, CategoryId = 2 },
                new Product { ProductId = 6, Barcode = "8930000000066", ProductName = "Khoai tây Đà Lạt", Price = 25000, StockQuantity = 55, CategoryId = 2 },
                new Product { ProductId = 7, Barcode = "8930000000073", ProductName = "Củ dền", Price = 28000, StockQuantity = 25, CategoryId = 2 },
                new Product { ProductId = 8, Barcode = "8930000000080", ProductName = "Cà chua", Price = 30000, StockQuantity = 50, CategoryId = 3 },
                new Product { ProductId = 9, Barcode = "8930000000097", ProductName = "Dưa leo", Price = 18000, StockQuantity = 45, CategoryId = 3 },
                new Product { ProductId = 10, Barcode = "8930000000103", ProductName = "Ớt chuông Đà Lạt", Price = 45000, StockQuantity = 30, CategoryId = 3 },
                new Product { ProductId = 11, Barcode = "8930000000110", ProductName = "Dâu tây Đà Lạt", Price = 120000, StockQuantity = 20, CategoryId = 4 },
                new Product { ProductId = 12, Barcode = "8930000000127", ProductName = "Hồng giòn Đà Lạt", Price = 65000, StockQuantity = 25, CategoryId = 4 },
                new Product { ProductId = 13, Barcode = "8930000000134", ProductName = "Nấm bào ngư", Price = 40000, StockQuantity = 30, CategoryId = 5 },
                new Product { ProductId = 14, Barcode = "8930000000141", ProductName = "Nấm đùi gà", Price = 60000, StockQuantity = 25, CategoryId = 5 },
                new Product { ProductId = 15, Barcode = "8930000000158", ProductName = "Hành lá", Price = 15000, StockQuantity = 35, CategoryId = 6 }
            );


            // 3. 15 KHÁCH HÀNG
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