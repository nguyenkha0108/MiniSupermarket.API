using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        // BẢNG MÀU MỚI: Dark Mode cao cấp (Tông nền tối, điểm nhấn Tím Violet & Hồng Fuchsia)
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Nền chính tối sâu (Slate 900)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Nền thẻ Card nổi bật (Slate 800)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet hiện đại
        private readonly Color accentHover = Color.FromArgb(147, 51, 234);    // Tím đậm hơn khi hover
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)

        private Panel loginCard = null!;
        private Label lblStatus = null!;
        private Button btnTogglePassword = null!;

        public FormLogin()
        {
            InitializeComponent();

            BuildLoginInterface();

            txtPassword.UseSystemPasswordChar = true;
        }

        private void BuildLoginInterface()
        {
            // GIAO DIỆN CHÍNH (DARK THEME)
            Text = "KhaMiniMart | Đăng nhập hệ thống";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(950, 620);
            Size = new Size(1150, 720);
            BackColor = darkBackground;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // KHUNG ĐĂNG NHẬP (CARD)
            loginCard = new Panel
            {
                Size = new Size(460, 540),
                BackColor = cardBackground,
                Padding = new Padding(40)
            };
            loginCard.Paint += LoginCard_Paint;

            Controls.Add(loginCard);

            // LOGO
            var lblLogo = new Label
            {
                Text = "⚡ KHA MINI MART",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // TIÊU ĐỀ
            var lblWelcome = new Label
            {
                Text = "Đăng Nhập Quản Trị",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = textLight,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // MÔ TẢ
            var lblDescription = new Label
            {
                Text = "Hệ thống quản lý siêu thị mini thông minh",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = textMuted,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // TÀI KHOẢN
            var lblUsername = new Label
            {
                Text = "Tài khoản truy cập",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textMuted,
                TextAlign = ContentAlignment.MiddleLeft
            };

            StyleInput(txtUsername);
            txtUsername.Dock = DockStyle.Top;
            txtUsername.Height = 40;
            txtUsername.Font = new Font("Segoe UI", 11F);

            // MẬT KHẨU & NÚT ẨN/HIỆN
            var lblPassword = new Label
            {
                Text = "Mật khẩu bảo mật",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textMuted,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var passwordPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(51, 65, 85)
            };

            StyleInput(txtPassword);
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Location = new Point(14, 10);
            txtPassword.Width = 330;
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.UseSystemPasswordChar = true;

            btnTogglePassword = new Button
            {
                Text = "👁",
                Width = 35,
                Height = 32,
                Location = new Point(355, 4),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                ForeColor = textLight,
                Font = new Font("Segoe UI", 9.5F)
            };
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.Click += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
                btnTogglePassword.Text = txtPassword.UseSystemPasswordChar ? "👁" : "🔒";
            };

            passwordPanel.Controls.Add(txtPassword);
            passwordPanel.Controls.Add(btnTogglePassword);

            // NÚT ĐĂNG NHẬP
            StyleLoginButton(btnLogin);
            btnLogin.Dock = DockStyle.Top;
            btnLogin.Height = 48;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.Text = "TRUY CẬP HỆ THỐNG";
            btnLogin.Click -= btnLogin_Click;
            btnLogin.Click += btnLogin_Click;

            // TRẠNG THÁI
            lblStatus = new Label
            {
                Text = "Vui lòng nhập thông tin xác thực.",
                Dock = DockStyle.Top,
                Height = 36,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // SẮP XẾP CONTROL TRONG CARD (Viết tường minh để tránh hoàn toàn lỗi cú pháp)
            loginCard.Controls.Clear();

            loginCard.Controls.Add(lblStatus);
            loginCard.Controls.Add(btnLogin);

            var spacer1 = new Panel();
            spacer1.Height = 18;
            spacer1.Dock = DockStyle.Top;
            loginCard.Controls.Add(spacer1);

            loginCard.Controls.Add(passwordPanel);
            loginCard.Controls.Add(lblPassword);

            var spacer2 = new Panel();
            spacer2.Height = 10;
            spacer2.Dock = DockStyle.Top;
            loginCard.Controls.Add(spacer2);

            loginCard.Controls.Add(txtUsername);
            loginCard.Controls.Add(lblUsername);

            var spacer3 = new Panel();
            spacer3.Height = 14;
            spacer3.Dock = DockStyle.Top;
            loginCard.Controls.Add(spacer3);

            loginCard.Controls.Add(lblDescription);
            loginCard.Controls.Add(lblWelcome);
            loginCard.Controls.Add(lblLogo);

            lblLogo.BringToFront();
            // DÒNG BẢN QUYỀN
            var lblFooter = new Label
            {
                Text = "KhaMiniMart © 2026 • Secure Enterprise Portal",
                Dock = DockStyle.Bottom,
                Height = 35,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(lblFooter);

            // CANH GIỮA
            Resize += (_, _) => CenterLoginCard();
            CenterLoginCard();

            AcceptButton = btnLogin;
        }

        // Vẽ viền bo tròn nhẹ cho Card Dark Mode
        private void LoginCard_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, loginCard.Width - 1, loginCard.Height - 1);
            using (GraphicsPath path = GetRoundedRectanglePath(rect, 16))
            {
                e.Graphics.DrawPath(new Pen(Color.FromArgb(71, 85, 105), 1.5f), path);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void CenterLoginCard()
        {
            if (loginCard == null)
                return;

            loginCard.Left = (ClientSize.Width - loginCard.Width) / 2;
            loginCard.Top = (ClientSize.Height - loginCard.Height) / 2 - 15;
        }

        private static void StyleInput(Control input)
        {
            input.BackColor = Color.FromArgb(51, 65, 85); // Slate 700
            input.ForeColor = Color.FromArgb(248, 250, 252);
        }

        private void StyleLoginButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = accentHover;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(126, 34, 206);

            button.BackColor = accentColor;
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
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113); // Màu đỏ sáng nổi bật trên nền tối

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
                btnLogin.Text = "ĐANG XÁC THỰC...";
                lblStatus.Text = "Đang kết nối đến hệ thống...";
                lblStatus.ForeColor = textMuted;

                var response = await _client.PostAsJsonAsync(
                    "Auth/login",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    lblStatus.Text = "Đăng nhập thất bại. Kiểm tra lại thông tin.";
                    lblStatus.ForeColor = Color.FromArgb(248, 113, 113);

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
                    lblStatus.ForeColor = Color.FromArgb(248, 113, 113);

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
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113);

                MessageBox.Show(
                    "Không thể kết nối đến API:\n\n" + ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "TRUY CẬP HỆ THỐNG";
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