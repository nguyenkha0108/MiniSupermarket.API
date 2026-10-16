using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models
{
    public class CheckoutRequest
    {
        [Required, StringLength(50, MinimumLength = 1)]
        public string CashierUsername { get; set; } = "cashier01";

        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required, MinLength(1)]
        public List<CheckoutItemRequest> Items { get; set; } = new();
    }

    public class CheckoutItemRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, 100000)]
        public int Quantity { get; set; }

        // Được giữ để tương thích mẫu bài giảng; giá thanh toán thực tế lấy từ CSDL.
        [Range(0, 1000000000)]
        public decimal UnitPrice { get; set; }
    }
}
