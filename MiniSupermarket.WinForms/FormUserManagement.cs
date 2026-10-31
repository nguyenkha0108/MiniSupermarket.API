using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormUserManagement : Form
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/panel)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color btnGreen = Color.FromArgb(22, 163, 74);        // Xanh lá (Thêm)
        private readonly Color btnOrange = Color.FromArgb(234, 88, 12);       // Cam (Sửa/Khóa)
        private readonly Color btnRed = Color.FromArgb(220, 38, 38);          // Đỏ (Xóa)

        private DataGridView dgvUsers = null!;
        private TextBox txtKeyword = null!;
        private TextBox txtId = null!;
        private TextBox txtUsername = null!;
        private TextBox txtRole = null!;
        private TextBox txtStatus = null!;
        private TextBox txtCreatedAt = null!;

        private Button btnSearch = null!;
        private Button btnLoad = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        private Button btnReset = null!;
        private Button btnLock = null!;

        private readonly JsonSerializerOptions jsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        private List<UserDto> users = new();

        public FormUserManagement()
        {
            InitializeComponentCustom();
            SessionManager.AddAuthorizationHeader(_client);
            ApplyPermissions();
        }

        private void InitializeComponentCustom()
        {
            Text = "KhaMiniMart | Quản lý tài khoản";
            BackColor = darkBackground;
            ForeColor = textLight;
            Font = new Font("Segoe UI", 10F);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(15);

            var heading = new Label
            {
                Text = "QUẢN LÝ TÀI KHOẢN",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = textLight,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Thanh tìm kiếm
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = darkBackground
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm theo tên đăng nhập hoặc vai trò",
                Dock = DockStyle.Top,
                Height = 23,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            txtKeyword = new TextBox
            {
                Dock = DockStyle.Left,
                Width = 300,
                MinimumSize = new Size(150, 32),
                BackColor = inputBackground,
                ForeColor = textLight
            };

            btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Width = 100,
                Dock = DockStyle.Left,
                Margin = new Padding(8, 0, 0, 0),
                MinimumSize = new Size(100, 36)
            };

            btnLoad = new Button
            {
                Text = "Tải lại",
                Width = 90,
                Dock = DockStyle.Left,
                Margin = new Padding(8, 0, 0, 0),
                MinimumSize = new Size(90, 36)
            };

            btnSearch.Click += async (_, _) => await LoadUsersAsync();
            btnLoad.Click += async (_, _) =>
            {
                txtKeyword.Clear();
                await LoadUsersAsync();
            };

            var searchBox = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 36,
                BackColor = darkBackground
            };

            searchBox.Controls.Add(btnLoad);
            searchBox.Controls.Add(btnSearch);
            searchBox.Controls.Add(txtKeyword);

            pnlSearch.Controls.Add(searchBox);
            pnlSearch.Controls.Add(lblSearch);

            // Khung thông tin bên phải
            var pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 360,
                BackColor = darkBackground,
                Padding = new Padding(15, 0, 0, 0)
            };

            var gbDetail = new GroupBox
            {
                Text = "Thông tin tài khoản",
                Dock = DockStyle.Fill,
                ForeColor = textLight,
                BackColor = cardBackground,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(15)
            };

            txtId = CreateReadOnlyField();
            txtUsername = CreateReadOnlyField();
            txtRole = CreateReadOnlyField();
            txtStatus = CreateReadOnlyField();
            txtCreatedAt = CreateReadOnlyField();

            var lblId = CreateLabel("Mã tài khoản");
            var lblUsername = CreateLabel("Tên đăng nhập");
            var lblRole = CreateLabel("Vai trò");
            var lblStatus = CreateLabel("Trạng thái");
            var lblCreated = CreateLabel("Ngày tạo");

            // Dùng ComboBox để chọn vai trò khi sửa
            var cbRole = new ComboBox
            {
                Name = "cbRole",
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = inputBackground,
                ForeColor = textLight,
                Margin = new Padding(0, 4, 0, 12),
                Height = 28
            };

            cbRole.Items.AddRange(new object[]
            {
                "Admin", "Cashier", "Warehouse"
            });

            // Thay TextBox vai trò bằng ComboBox chọn vai trò
            txtRole.Visible = false;

            btnAdd = new Button { Text = "Thêm mới", Dock = DockStyle.Fill };
            btnUpdate = new Button { Text = "Cập nhật vai trò", Dock = DockStyle.Fill };
            btnDelete = new Button { Text = "Xóa", Dock = DockStyle.Fill };
            btnReset = new Button { Text = "Đặt lại mật khẩu", Dock = DockStyle.Fill };
            btnLock = new Button { Text = "Khóa / mở khóa", Dock = DockStyle.Fill };

            var buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 110,
                ColumnCount = 2,
                RowCount = 3
            };

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));
            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            buttonLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.3F));
            buttonLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.3F));
            buttonLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.4F));

            buttonLayout.Controls.Add(btnAdd, 0, 0);
            buttonLayout.Controls.Add(btnUpdate, 1, 0);
            buttonLayout.Controls.Add(btnReset, 0, 1);
            buttonLayout.Controls.Add(btnLock, 1, 1);
            buttonLayout.Controls.Add(btnDelete, 0, 2);
            buttonLayout.SetColumnSpan(btnDelete, 2);

            btnAdd.Click += async (_, _) => await AddUserAsync(cbRole);
            btnUpdate.Click += async (_, _) => await UpdateRoleAsync(cbRole);
            btnReset.Click += async (_, _) => await ResetPasswordAsync();
            btnLock.Click += async (_, _) => await ToggleStatusAsync();
            btnDelete.Click += async (_, _) => await DeleteUserAsync();

            gbDetail.Controls.Add(buttonLayout);
            gbDetail.Controls.Add(txtCreatedAt);
            gbDetail.Controls.Add(lblCreated);
            gbDetail.Controls.Add(txtStatus);
            gbDetail.Controls.Add(lblStatus);
            gbDetail.Controls.Add(cbRole);
            gbDetail.Controls.Add(lblRole);
            gbDetail.Controls.Add(txtUsername);
            gbDetail.Controls.Add(lblUsername);
            gbDetail.Controls.Add(txtId);
            gbDetail.Controls.Add(lblId);

            pnlRight.Controls.Add(gbDetail);

            // Bảng tài khoản bên trái
            dgvUsers = new DataGridView();
            StyleDataGridView(dgvUsers);

            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UserId",
                HeaderText = "Mã TK",
                DataPropertyName = "UserId",
                FillWeight = 45
            });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                HeaderText = "Tên đăng nhập",
                DataPropertyName = "Username",
                FillWeight = 120
            });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Role",
                HeaderText = "Vai trò",
                DataPropertyName = "Role",
                FillWeight = 80
            });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Trạng thái",
                DataPropertyName = "Status",
                FillWeight = 80
            });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreatedAt",
                HeaderText = "Ngày tạo",
                DataPropertyName = "CreatedAt",
                FillWeight = 100
            });

            dgvUsers.CellClick += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    ShowSelectedUser(cbRole);
            };

            var pnlCenter = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 5, 0),
                BackColor = darkBackground
            };

            pnlCenter.Controls.Add(dgvUsers);

            Controls.Add(pnlCenter);
            Controls.Add(pnlRight);
            Controls.Add(pnlSearch);
            Controls.Add(heading);

            StyleActionButton(btnAdd, btnGreen);
            StyleActionButton(btnUpdate, btnOrange);
            StyleActionButton(btnDelete, btnRed);
            StyleActionButton(btnReset, accentColor);
            StyleActionButton(btnLock, btnOrange);
            StyleActionButton(btnLoad, accentColor);
            StyleActionButton(btnSearch, accentColor);

            Load += async (_, _) => await LoadUsersAsync();
        }

        private TextBox CreateReadOnlyField()
        {
            return new TextBox
            {
                Dock = DockStyle.Top,
                ReadOnly = true,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = textLight,
                Margin = new Padding(0, 4, 0, 10),
                Height = 28
            };
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = textMuted
            };
        }

        private void StyleDataGridView(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = cardBackground;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.GridColor = Color.FromArgb(51, 65, 85);
            grid.RowTemplate.Height = 40;
            grid.ColumnHeadersHeight = 44;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            grid.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.BackColor = cardBackground;
            grid.DefaultCellStyle.ForeColor = textLight;
            grid.DefaultCellStyle.SelectionBackColor = accentColor;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(35, 47, 68);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = textLight;
        }

        private void StyleActionButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.15F);
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            button.Height = 36;
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var keyword = Uri.EscapeDataString(txtKeyword.Text.Trim());

                using var response = await _client.GetAsync(
                    $"users?keyword={keyword}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(
                        $"Không tải được tài khoản.\n{(int)response.StatusCode}: {error}");
                    return;
                }

                var result = await response.Content.ReadFromJsonAsync<List<UserDto>>(
                    jsonOptions);

                users = result ?? new List<UserDto>();

                dgvUsers.DataSource = null;
                dgvUsers.DataSource = users.ConvertAll(u => new UserRow
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Role = u.Role,
                    Status = u.IsActive ? "Hoạt động" : "Đã khóa",
                    CreatedAt = u.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                });

                ClearDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tải được danh sách tài khoản:\n" + ex.Message);
            }
        }

        private UserDto? GetSelectedUser()
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is not UserRow row)
            {
                MessageBox.Show("Vui lòng chọn tài khoản trong bảng.");
                return null;
            }

            return users.Find(u => u.UserId == row.UserId);
        }

        private void ShowSelectedUser(ComboBox cbRole)
        {
            var user = GetSelectedUser();
            if (user == null) return;

            txtId.Text = user.UserId.ToString();
            txtUsername.Text = user.Username;
            txtStatus.Text = user.IsActive ? "Hoạt động" : "Đã khóa";
            txtCreatedAt.Text = user.CreatedAt.ToString("dd/MM/yyyy HH:mm");

            cbRole.SelectedItem = user.Role;
        }

        private void ClearDetail()
        {
            txtId.Clear();
            txtUsername.Clear();
            txtRole.Clear();
            txtStatus.Clear();
            txtCreatedAt.Clear();
        }

        private async Task AddUserAsync(ComboBox cbRole)
        {
            var username = Prompt("Tên đăng nhập:", "Thêm tài khoản");
            if (string.IsNullOrWhiteSpace(username)) return;

            var password = Prompt("Mật khẩu (ít nhất 8 ký tự):", "Thêm tài khoản");
            if (password == null) return;

            var role = cbRole.SelectedItem?.ToString() ?? "Cashier";

            if (password.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự.");
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.PostAsJsonAsync("users", new
                {
                    Username = username.Trim(),
                    Password = password,
                    Role = role
                });

                if (!response.IsSuccessStatusCode)
                {
                    await ShowApiError(response);
                    return;
                }

                MessageBox.Show("Thêm tài khoản thành công.");
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm tài khoản:\n" + ex.Message);
            }
        }

        private async Task UpdateRoleAsync(ComboBox cbRole)
        {
            var user = GetSelectedUser();
            if (user == null) return;

            var role = cbRole.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Vui lòng chọn vai trò.");
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.PutAsJsonAsync(
                    $"users/{user.UserId}", new { Role = role });

                if (!response.IsSuccessStatusCode)
                {
                    await ShowApiError(response);
                    return;
                }

                MessageBox.Show("Cập nhật vai trò thành công.");
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật:\n" + ex.Message);
            }
        }

        private async Task ResetPasswordAsync()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            var password = Prompt(
                $"Nhập mật khẩu mới cho {user.Username}:",
                "Đặt lại mật khẩu");

            if (password == null) return;

            if (password.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự.");
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.PostAsJsonAsync(
                    $"users/{user.UserId}/reset-password",
                    new { NewPassword = password });

                if (!response.IsSuccessStatusCode)
                {
                    await ShowApiError(response);
                    return;
                }

                MessageBox.Show("Đặt lại mật khẩu thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đặt lại mật khẩu:\n" + ex.Message);
            }
        }

        private async Task ToggleStatusAsync()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            bool newStatus = !user.IsActive;

            if (MessageBox.Show(
                $"Bạn có chắc muốn {(newStatus ? "mở khóa" : "khóa")} tài khoản {user.Username}?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.PatchAsJsonAsync(
                    $"users/{user.UserId}/status",
                    new { IsActive = newStatus });

                if (!response.IsSuccessStatusCode)
                {
                    await ShowApiError(response);
                    return;
                }

                MessageBox.Show("Cập nhật trạng thái thành công.");
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật trạng thái:\n" + ex.Message);
            }
        }

        private async Task DeleteUserAsync()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            if (MessageBox.Show(
                $"Bạn có chắc muốn xóa tài khoản {user.Username}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.DeleteAsync(
                    $"users/{user.UserId}");

                if (!response.IsSuccessStatusCode)
                {
                    await ShowApiError(response);
                    return;
                }

                MessageBox.Show("Xóa tài khoản thành công.");
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa tài khoản:\n" + ex.Message);
            }
        }

        private static async Task ShowApiError(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            MessageBox.Show(
                $"API trả về {(int)response.StatusCode}:\n{body}",
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private string? Prompt(string message, string title)
        {
            using var form = new Form
            {
                Text = title,
                Width = 400,
                Height = 160,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                BackColor = darkBackground,
                ForeColor = textLight
            };

            var label = new Label
            {
                Text = message,
                Left = 15,
                Top = 15,
                Width = 350,
                Height = 25,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9.5F)
            };

            var input = new TextBox
            {
                Left = 15,
                Top = 45,
                Width = 350,
                Height = 28,
                BackColor = inputBackground,
                ForeColor = textLight,
                Font = new Font("Segoe UI", 10F)
            };

            var ok = new Button
            {
                Text = "Xác nhận",
                Left = 180,
                Top = 85,
                Width = 90,
                Height = 32,
                DialogResult = DialogResult.OK
            };

            var cancel = new Button
            {
                Text = "Hủy",
                Left = 275,
                Top = 85,
                Width = 90,
                Height = 32,
                DialogResult = DialogResult.Cancel
            };

            StyleActionButton(ok, accentColor);
            StyleActionButton(cancel, Color.FromArgb(71, 85, 105));

            form.Controls.AddRange(new Control[] { label, input, ok, cancel });
            form.AcceptButton = ok;
            form.CancelButton = cancel;

            return form.ShowDialog() == DialogResult.OK ? input.Text : null;
        }

        private void ApplyPermissions()
        {
            bool canManage = SessionManager.IsAdmin;

            btnAdd.Enabled = canManage;
            btnUpdate.Enabled = canManage;
            btnDelete.Enabled = canManage;
            btnReset.Enabled = canManage;
            btnLock.Enabled = canManage;
        }

        private class UserRow
        {
            public int UserId { get; set; }
            public string Username { get; set; } = "";
            public string Role { get; set; } = "";
            public string Status { get; set; } = "";
            public string CreatedAt { get; set; } = "";
        }

        private class UserDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = "";
            public string Role { get; set; } = "";
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}