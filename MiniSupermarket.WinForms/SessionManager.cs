using System.Net.Http.Headers;

namespace MiniSupermarket.WinForms
{
    public static class SessionManager
    {
        public static string Token { get; private set; }
            = string.Empty;

        public static string Username { get; private set; }
            = string.Empty;

        public static string Role { get; private set; }
            = string.Empty;

        public static bool IsAdmin
        {
            get
            {
                return Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsCashier
        {
            get
            {
                return Role.Equals(
                    "Cashier",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsWarehouse => Role.Equals(
            "Warehouse", StringComparison.OrdinalIgnoreCase);

        public static void SetSession(
            string token,
            string username,
            string role)
        {
            Token = token;
            Username = username;
            Role = role;
        }

        public static void ClearSession()
        {
            Token = string.Empty;
            Username = string.Empty;
            Role = string.Empty;
        }

        public static void AddAuthorizationHeader(
            HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    Token);
        }
    }
}