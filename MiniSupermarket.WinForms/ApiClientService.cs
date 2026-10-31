using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarket.WinForms
{
    /// <summary>
    /// Lỗi đã được chuyển sang tiếng Việt, có thể hiển thị trực tiếp cho người dùng.
    /// </summary>
    public sealed class ApiException : Exception
    {
        public HttpStatusCode? StatusCode { get; }
        public string ResponseBody { get; }

        public ApiException(string message, HttpStatusCode? statusCode = null, string responseBody = "")
            : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }

    /// <summary>
    /// Kết quả thô: không ném lỗi khi API trả 4xx/5xx để form đọc được thân phản hồi (ví dụ thông báo từ server).
    /// </summary>
    public readonly record struct ApiRawResponse(HttpStatusCode StatusCode, string Body)
    {
        public bool IsSuccess => (int)StatusCode is >= 200 and < 300;
    }

    /// <summary>
    /// Lớp gọi API dùng chung cho toàn bộ WinForms.
    /// Tự động đính kèm Bearer Token từ SessionManager vào mọi request.
    /// </summary>
    public static class ApiClientService
    {
        public const string BaseUrl = "https://localhost:7044/api/";

        private static readonly HttpClient Http = new()
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public static async Task<T?> GetAsync<T>(string path)
        {
            using var response = await SendAsync(HttpMethod.Get, path, null);
            string body = await response.Content.ReadAsStringAsync();
            EnsureSuccess(response.StatusCode, body);
            return Deserialize<T>(body);
        }

        public static async Task<T?> PostAsync<T>(string path, object body)
        {
            using var response = await SendAsync(HttpMethod.Post, path, body);
            string text = await response.Content.ReadAsStringAsync();
            EnsureSuccess(response.StatusCode, text);
            return Deserialize<T>(text);
        }

        public static async Task<T?> DeleteAsync<T>(string path)
        {
            using var response = await SendAsync(HttpMethod.Delete, path, null);
            string text = await response.Content.ReadAsStringAsync();
            EnsureSuccess(response.StatusCode, text);
            return Deserialize<T>(text);
        }

        /// <summary>Gửi POST và trả về nguyên trạng (không ném lỗi theo mã HTTP).</summary>
        public static async Task<ApiRawResponse> PostRawAsync(string path, object body)
        {
            using var response = await SendAsync(HttpMethod.Post, path, body);
            string text = await response.Content.ReadAsStringAsync();
            return new ApiRawResponse(response.StatusCode, text);
        }

        private static async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object? body)
        {
            using var request = new HttpRequestMessage(method, path);

            if (!string.IsNullOrWhiteSpace(SessionManager.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            }

            if (body != null)
            {
                request.Content = JsonContent.Create(body, options: JsonOptions);
            }

            try
            {
                return await Http.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException(
                    "Không kết nối được API tại " + BaseUrl + ".\nHãy kiểm tra API đã chạy chưa.\n\n" + ex.Message);
            }
            catch (TaskCanceledException)
            {
                throw new ApiException("Máy chủ phản hồi quá chậm. Vui lòng thử lại.");
            }
        }

        private static void EnsureSuccess(HttpStatusCode statusCode, string body)
        {
            if ((int)statusCode is >= 200 and < 300) return;
            throw new ApiException(BuildErrorMessage(statusCode, body), statusCode, body);
        }

        private static string BuildErrorMessage(HttpStatusCode statusCode, string body)
        {
            string? serverMessage = ExtractMessage(body);

            return statusCode switch
            {
                HttpStatusCode.Unauthorized => "Phiên đăng nhập đã hết hạn hoặc không hợp lệ. Hãy đăng nhập lại.",
                HttpStatusCode.Forbidden => "Tài khoản hiện tại không có quyền thực hiện thao tác này.",
                HttpStatusCode.NotFound => serverMessage ?? "Không tìm thấy dữ liệu yêu cầu.",
                _ => serverMessage ?? $"Yêu cầu thất bại (mã {(int)statusCode})."
            };
        }

        private static string? ExtractMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;
            try
            {
                using var document = JsonDocument.Parse(body);
                if (document.RootElement.TryGetProperty("message", out var message)
                    && message.ValueKind == JsonValueKind.String)
                {
                    return message.GetString();
                }
            }
            catch (JsonException)
            {
                // Không phải JSON: bỏ qua, dùng thông báo mặc định.
            }
            return null;
        }

        private static T? Deserialize<T>(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return default;
            try
            {
                return JsonSerializer.Deserialize<T>(body, JsonOptions);
            }
            catch (JsonException)
            {
                throw new ApiException("Dữ liệu API trả về không đúng định dạng.");
            }
        }
    }
}
