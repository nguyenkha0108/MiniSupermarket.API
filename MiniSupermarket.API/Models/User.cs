
using System;

namespace MiniSupermarket.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Cashier";

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
