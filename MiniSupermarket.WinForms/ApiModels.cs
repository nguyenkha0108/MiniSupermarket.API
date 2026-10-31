using System.Collections.Generic;

namespace MiniSupermarket.WinForms
{
    public sealed class CustomerLookupDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
    }

    public sealed class DashboardSummaryDto
    {
        public bool Success { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int NewCustomers30Days { get; set; }
        public int LowStockProducts { get; set; }
    }

    public sealed class DailyRevenueItem
    {
        public string Date { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public sealed class DailyRevenueResponse
    {
        public bool Success { get; set; }
        public List<DailyRevenueItem> Data { get; set; } = new();
    }

    public sealed class TopProductItem
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public sealed class Top5Response
    {
        public bool Success { get; set; }
        public int Count { get; set; }
        public List<TopProductItem> Data { get; set; } = new();
    }
}
