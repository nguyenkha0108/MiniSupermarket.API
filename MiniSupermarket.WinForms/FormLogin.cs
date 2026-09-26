using System.Net.Http.Json;

namespace MiniSupermarket.WinForms
{
    public partial class FormLogin : Form
    {
        private static readonly HttpClient _client =
            new HttpClient
            {
                BaseAddress =
                    new Uri("https://localhost:7044/api/")
            };

        public FormLogin()
        {
            InitializeComponent();

            txtPassword.UseSystemPasswordChar = true;
        }

        private async void btnLogin_Click(
            object sender,
            EventArgs e)
        {
            string username =
                txtUsername.Text.Trim();

            string password =
                txtPassword.Text;

            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Vui lòng nhập tài khoản và mật khẩu!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Tạo request gửi lên API
            var request = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            try
            {
                btnLogin.Enabled = false;

                // POST /api/Auth/login
                var response =
                    await _client.PostAsJsonAsync(
                        "Auth/login",
                        request);

                // Đăng nhập thất bại
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Sai tài khoản hoặc mật khẩu!",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                // Đọc kết quả từ API
                var result =
                    await response.Content
                        .ReadFromJsonAsync<LoginResponseDto>();

                if (result == null ||
                    string.IsNullOrEmpty(result.Token))
                {
                    MessageBox.Show(
                        "API không trả về Token!",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                // Lưu thông tin đăng nhập
                SessionManager.SetSession(
                    result.Token,
                    username,
                    result.Role);

                MessageBox.Show(
                    $"Đăng nhập thành công!\n\n" +
                    $"Tài khoản: {username}\n" +
                    $"Quyền: {result.Role}",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Mở màn hình quản lý nhóm hàng
                FormCategoryManagement form =
                    new FormCategoryManagement();

                form.Show();

                // Ẩn Form Login
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể kết nối đến API:\n\n" +
                    ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }


    // =====================================================
    // REQUEST LOGIN
    // =====================================================

    public class LoginRequestDto
    {
        public string Username { get; set; }
            = string.Empty;

        public string Password { get; set; }
            = string.Empty;
    }


    // =====================================================
    // RESPONSE LOGIN
    // =====================================================

    public class LoginResponseDto
    {
        public bool Success { get; set; }

        public string Token { get; set; }
            = string.Empty;

        public string Role { get; set; }
            = string.Empty;
    }
}