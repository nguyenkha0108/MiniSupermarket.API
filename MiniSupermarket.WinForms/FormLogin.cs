
using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormLogin : Form
    {
        private static readonly HttpClient _client =
            new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7044/api/")
            };

        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color background = Color.FromArgb(243, 246, 251);
        private readonly Color textPrimary = Color.FromArgb(15, 23, 42);
        private readonly Color textSecondary = Color.FromArgb(100, 116, 139);

        private Panel loginCard = null!;
        private Label lblStatus = null!;

        public FormLogin()
        {
            InitializeComponent();

            BuildLoginInterface();

            txtPassword.UseSystemPasswordChar = true;
        }

        private void BuildLoginInterface()
        {
            // GIAO DIỆN CHÍNH
            Text = "KhaMiniMart | Đăng nhập";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 600);
            Size = new Size(1100, 700);
            BackColor = background;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // TẠO KHUNG ĐĂNG NHẬP
            loginCard = new Panel
            {
                Size = new Size(430, 490),
                BackColor = Color.White,
                Padding = new Padding(35)
            };

            Controls.Add(loginCard);

            // LOGO
            var lblLogo = new Label
            {
                Text = "◆ KhaMiniMart",
                Dock = DockStyle.Top,
                Height = 55,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                ForeColor = blue,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // TIÊU ĐỀ
            var lblWelcome = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Dock = DockStyle.Top,
                Height = 42,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = textPrimary,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // MÔ TẢ
            var lblDescription = new Label
            {
                Text = "Quản lý cửa hàng bán lẻ thông minh",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = textSecondary,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // TÀI KHOẢN
            var lblUsername = new Label
            {
                Text = "Tài khoản",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = textPrimary,
                TextAlign = ContentAlignment.MiddleLeft
            };

            StyleInput(txtUsername);
            txtUsername.Dock = DockStyle.Top;
            txtUsername.Height = 38;
            txtUsername.Font = new Font("Segoe UI", 11F);

            // MẬT KHẨU
            var lblPassword = new Label
            {
                Text = "Mật khẩu",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = textPrimary,
                TextAlign = ContentAlignment.MiddleLeft
            };

            StyleInput(txtPassword);
            txtPassword.Dock = DockStyle.Top;
            txtPassword.Height = 38;
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.UseSystemPasswordChar = true;

            // NÚT ĐĂNG NHẬP
            StyleLoginButton(btnLogin);
            btnLogin.Dock = DockStyle.Top;
            btnLogin.Height = 46;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Click -= btnLogin_Click;
            btnLogin.Click += btnLogin_Click;

            // TRẠNG THÁI
            lblStatus = new Label
            {
                Text = "Vui lòng nhập tài khoản và mật khẩu.",
                Dock = DockStyle.Top,
                Height = 36,
                ForeColor = textSecondary,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // SẮP XẾP CONTROL
            loginCard.Controls.Clear();

            loginCard.Controls.Add(lblStatus);
            loginCard.Controls.Add(btnLogin);

            loginCard.Controls.Add(new Panel { Height = 18, Dock = DockStyle.Top });

            loginCard.Controls.Add(txtPassword);
            loginCard.Controls.Add(lblPassword);

            loginCard.Controls.Add(new Panel { Height = 8, Dock = DockStyle.Top });

            loginCard.Controls.Add(txtUsername);
            loginCard.Controls.Add(lblUsername);

            loginCard.Controls.Add(new Panel { Height = 14, Dock = DockStyle.Top });

            loginCard.Controls.Add(lblDescription);
            loginCard.Controls.Add(lblWelcome);
            loginCard.Controls.Add(lblLogo);

            // DOCK TOP XẾP NGƯỢC THỨ TỰ THÊM CONTROL.
            // Dùng BringToFront để logo nằm phía trên.
            lblLogo.BringToFront();

            // DÒNG BẢN QUYỀN
            var lblFooter = new Label
            {
                Text = "KhaMiniMart © 2026 | Hệ thống quản lý siêu thị mini",
                Dock = DockStyle.Bottom,
                Height = 35,
                ForeColor = textSecondary,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(lblFooter);

            // CANH GIỮA KHUNG ĐĂNG NHẬP
            Resize += (_, _) => CenterLoginCard();
            CenterLoginCard();

            AcceptButton = btnLogin;
        }

        private void CenterLoginCard()
        {
            if (loginCard == null)
                return;

            loginCard.Left = (ClientSize.Width - loginCard.Width) / 2;
            loginCard.Top = (ClientSize.Height - loginCard.Height) / 2 - 10;
        }

        private static void StyleInput(Control input)
        {
            input.BackColor = Color.FromArgb(248, 250, 252);
            input.ForeColor = Color.FromArgb(15, 23, 42);
        }

        private void StyleLoginButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(29, 78, 216);
            button.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(30, 64, 175);

            button.BackColor = blue;
            button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "Vui lòng nhập đầy đủ tài khoản và mật khẩu.";
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

                MessageBox.Show(
                    "Vui lòng nhập tài khoản và mật khẩu!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var request = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "ĐANG ĐĂNG NHẬP...";
                lblStatus.Text = "Đang kết nối đến hệ thống...";
                lblStatus.ForeColor = textSecondary;

                var response = await _client.PostAsJsonAsync(
                    "Auth/login",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    lblStatus.Text = "Đăng nhập thất bại. Kiểm tra lại thông tin.";
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

                    MessageBox.Show(
                        "Sai tài khoản hoặc mật khẩu!",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                var result = await response.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

                if (result == null ||
                    string.IsNullOrEmpty(result.Token))
                {
                    lblStatus.Text = "API không trả về Token.";
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

                    MessageBox.Show(
                        "API không trả về Token!",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                SessionManager.SetSession(
                    result.Token,
                    username,
                    result.Role);

                MessageBox.Show(
                    $"Đăng nhập thành công!\n\n" +
                    $"Tài khoản: {username}\n" +
                    $"Quyền: {result.Role}",
                    "KhaMiniMart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                var mainShell = new FormMainShell(this);
                mainShell.Show();

                Hide();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Không thể kết nối đến API.";
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

                MessageBox.Show(
                    "Không thể kết nối đến API:\n\n" + ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Giữ lại để tương thích với sự kiện Label trong Designer.
        }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }

        public string Token { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}