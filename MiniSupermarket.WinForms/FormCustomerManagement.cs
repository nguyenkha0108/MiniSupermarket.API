using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormCustomerManagement : Form
    {
        private readonly DataGridView grid = new();
        private readonly Label lblStatus = new();

        private readonly Button btnReload = new();
        private readonly Button btnAdd = new();
        private readonly Button btnEdit = new();
        private readonly Button btnDelete = new();

        private const string ApiUrl =
            "https://localhost:7044/api/Customers";

        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color green = Color.FromArgb(22, 163, 74);
        private readonly Color orange = Color.FromArgb(234, 88, 12);
        private readonly Color red = Color.FromArgb(220, 38, 38);
        private readonly Color background = Color.FromArgb(243, 246, 251);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FormCustomerManagement()
        {
            Text = "KhaMiniMart | Quản lý khách hàng";
            BackColor = background;
            Font = new Font("Segoe UI", 10F);
            Padding = new Padding(18);
            MinimumSize = new Size(950, 550);
            StartPosition = FormStartPosition.CenterScreen;

            var heading = new Label
            {
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Dock = DockStyle.Top,
                Height = 42,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                ForeColor = navy
            };

            var subtitle = new Label
            {
                Text = "Quản lý thông tin, điểm tích lũy và hạng thành viên",
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = background
            };

            ConfigureButton(btnAdd, "＋ Thêm khách hàng", green, 175);
            ConfigureButton(btnEdit, "Sửa khách hàng", orange, 160);
            ConfigureButton(btnDelete, "Xóa khách hàng", red, 150);
            ConfigureButton(btnReload, "Tải lại", blue, 110);

            btnAdd.Click += async (_, _) => await AddCustomerAsync();
            btnEdit.Click += async (_, _) => await EditCustomerAsync();
            btnDelete.Click += async (_, _) => await DeleteCustomerAsync();
            btnReload.Click += async (_, _) => await LoadCustomersAsync();

            toolbar.Controls.Add(btnReload);
            toolbar.Controls.Add(btnDelete);
            toolbar.Controls.Add(btnEdit);
            toolbar.Controls.Add(btnAdd);

            btnAdd.Dock = DockStyle.Left;
            btnEdit.Dock = DockStyle.Left;
            btnDelete.Dock = DockStyle.Left;
            btnReload.Dock = DockStyle.Left;

            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Height = 32;
            lblStatus.Text = "Sẵn sàng";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);

            StyleGrid();

            AddColumn("CustomerId", "Mã KH",
                nameof(CustomerRow.CustomerId), 60);
            AddColumn("CustomerName", "Tên khách hàng",
                nameof(CustomerRow.CustomerName), 150);
            AddColumn("PhoneNumber", "Số điện thoại",
                nameof(CustomerRow.PhoneNumber), 110);
            AddColumn("Address", "Địa chỉ",
                nameof(CustomerRow.Address), 180);
            AddColumn("RewardPoints", "Điểm tích lũy",
                nameof(CustomerRow.RewardPoints), 90);
            AddColumn("MembershipRank", "Hạng thành viên",
                nameof(CustomerRow.MembershipRank), 110);

            Controls.Add(grid);
            Controls.Add(lblStatus);
            Controls.Add(toolbar);
            Controls.Add(subtitle);
            Controls.Add(heading);

            Load += async (_, _) => await LoadCustomersAsync();
        }

        private void ConfigureButton(
            Button button, string text, Color color, int width)
        {
            button.Text = text;
            button.Width = width;
            button.Margin = new Padding(0, 0, 8, 0);
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private void StyleGrid()
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.BackgroundColor = Color.White;
            grid.RowHeadersVisible = false;
            grid.BorderStyle = BorderStyle.None;
            grid.AutoGenerateColumns = false;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(226, 232, 240);
            grid.RowTemplate.Height = 38;
            grid.ColumnHeadersHeight = 42;

            grid.ColumnHeadersDefaultCellStyle.BackColor = navy;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            grid.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(15, 23, 42);

            grid.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);
        }

        private void AddColumn(
            string name, string header, string property, float weight)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                DataPropertyName = property,
                FillWeight = weight
            });
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            SessionManager.AddAuthorizationHeader(client);
            return client;
        }

        private async Task LoadCustomersAsync()
        {
            btnReload.Enabled = false;
            lblStatus.Text = "Đang tải khách hàng...";

            try
            {
                using var client = CreateClient();
                using var response = await client.GetAsync(ApiUrl);

                if (!await CheckResponseAsync(response))
                    return;

                string json = await response.Content.ReadAsStringAsync();

                var customers =
                    JsonSerializer.Deserialize<List<CustomerRow>>(
                        json, JsonOptions) ?? new List<CustomerRow>();

                grid.DataSource = null;
                grid.DataSource = customers;

                lblStatus.Text = $"Đã tải {customers.Count} khách hàng.";
                lblStatus.ForeColor = green;
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách khách hàng.", ex);
            }
            finally
            {
                btnReload.Enabled = true;
            }
        }

        private async Task AddCustomerAsync()
        {
            using var dialog = new CustomerDialog();

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                using var client = CreateClient();
                string json = JsonSerializer.Serialize(dialog.Customer);

                using var content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                using var response = await client.PostAsync(ApiUrl, content);

                if (!await CheckResponseAsync(response))
                    return;

                MessageBox.Show("Thêm khách hàng thành công!",
                    "Thông báo", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                ShowError("Không thêm được khách hàng.", ex);
            }
        }

        private async Task EditCustomerAsync()
        {
            if (grid.CurrentRow?.DataBoundItem is not CustomerRow selected)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa.");
                return;
            }

            using var dialog = new CustomerDialog(selected);

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                using var client = CreateClient();
                string json = JsonSerializer.Serialize(dialog.Customer);

                using var content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                using var response = await client.PutAsync(
                    $"{ApiUrl}/{selected.CustomerId}", content);

                if (!await CheckResponseAsync(response))
                    return;

                MessageBox.Show("Cập nhật khách hàng thành công!",
                    "Thông báo", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                ShowError("Không sửa được khách hàng.", ex);
            }
        }

        private async Task DeleteCustomerAsync()
        {
            if (grid.CurrentRow?.DataBoundItem is not CustomerRow selected)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa khách hàng \"{selected.CustomerName}\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using var client = CreateClient();
                using var response = await client.DeleteAsync(
                    $"{ApiUrl}/{selected.CustomerId}");

                if (!await CheckResponseAsync(response))
                    return;

                MessageBox.Show("Xóa khách hàng thành công!",
                    "Thông báo", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được khách hàng.", ex);
            }
        }

        private async Task<bool> CheckResponseAsync(
            HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return true;

            string body = await response.Content.ReadAsStringAsync();

            string message = response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized =>
                    "Token hết hạn hoặc bạn chưa đăng nhập.",
                System.Net.HttpStatusCode.Forbidden =>
                    "Tài khoản không có quyền thực hiện thao tác này.",
                System.Net.HttpStatusCode.NotFound =>
                    "Không tìm thấy khách hàng hoặc API endpoint.",
                System.Net.HttpStatusCode.Conflict =>
                    "Dữ liệu bị trùng hoặc khách hàng đang được sử dụng.",
                _ => $"API trả về lỗi {(int)response.StatusCode} " +
                     $"{response.ReasonPhrase}."
            };

            if (!string.IsNullOrWhiteSpace(body))
                message += "\n\nChi tiết: " + body;

            lblStatus.Text = message;

            MessageBox.Show(message, "Lỗi API",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return false;
        }

        private void ShowError(string message, Exception ex)
        {
            lblStatus.Text = message;

            MessageBox.Show($"{message}\n\nChi tiết: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Model hiển thị dữ liệu khách hàng
        private sealed class CustomerRow
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string Address { get; set; } = "";
            public int RewardPoints { get; set; }
            public string MembershipRank { get; set; } = "Đồng";
        }

        // Hộp thoại thêm và sửa khách hàng
        private sealed class CustomerDialog : Form
        {
            private readonly TextBox txtName = new();
            private readonly TextBox txtPhone = new();
            private readonly TextBox txtAddress = new();
            private readonly TextBox txtPoints = new();
            private readonly ComboBox cboRank = new();

            public CustomerRow Customer { get; private set; } = new();

            private static readonly string[] MembershipRanks =
            {
                "Đồng", "Bạc", "Vàng", "Kim cương"
            };

            public CustomerDialog(CustomerRow? existing = null)
            {
                bool editing = existing != null;

                Text = editing ? "Sửa khách hàng" : "Thêm khách hàng";
                ClientSize = new Size(460, 390);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                BackColor = Color.White;
                Font = new Font("Segoe UI", 10F);
                Padding = new Padding(16);

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 6,
                    Padding = new Padding(4)
                };

                layout.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Absolute, 135));
                layout.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 100));

                for (int i = 0; i < 5; i++)
                    layout.RowStyles.Add(
                        new RowStyle(SizeType.Absolute, 52));

                layout.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 100));

                AddField(layout, "Tên khách hàng:", txtName, 0);
                AddField(layout, "Số điện thoại:", txtPhone, 1);
                AddField(layout, "Địa chỉ:", txtAddress, 2);
                AddField(layout, "Điểm tích lũy:", txtPoints, 3);

                txtPoints.Text = "0";

                cboRank.Dock = DockStyle.Fill;
                cboRank.DropDownStyle = ComboBoxStyle.DropDownList;
                cboRank.Items.AddRange(MembershipRanks);
                cboRank.SelectedIndex = 0;
                cboRank.Margin = new Padding(3, 8, 3, 8);

                layout.Controls.Add(new Label
                {
                    Text = "Hạng thành viên:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                }, 0, 4);

                layout.Controls.Add(cboRank, 1, 4);

                if (existing != null)
                {
                    txtName.Text = existing.CustomerName;
                    txtPhone.Text = existing.PhoneNumber;
                    txtAddress.Text = existing.Address;
                    txtPoints.Text = existing.RewardPoints.ToString();

                    int rankIndex = Array.FindIndex(
                        MembershipRanks,
                        rank => string.Equals(
                            rank, existing.MembershipRank,
                            StringComparison.OrdinalIgnoreCase));

                    cboRank.SelectedIndex = rankIndex >= 0 ? rankIndex : 0;
                }

                var buttonPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    Padding = new Padding(0, 8, 0, 0)
                };

                var btnSave = new Button
                {
                    Text = "Lưu thông tin",
                    Size = new Size(135, 38),
                    BackColor = Color.FromArgb(37, 99, 235),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                btnSave.FlatAppearance.BorderSize = 0;

                var btnCancel = new Button
                {
                    Text = "Hủy",
                    Size = new Size(85, 38),
                    BackColor = Color.FromArgb(226, 232, 240),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    DialogResult = DialogResult.Cancel
                };

                btnCancel.FlatAppearance.BorderSize = 0;

                btnSave.Click += (_, _) => SaveCustomer(existing);

                buttonPanel.Controls.Add(btnSave);
                buttonPanel.Controls.Add(btnCancel);

                layout.Controls.Add(buttonPanel, 1, 5);

                Controls.Add(layout);

                AcceptButton = btnSave;
                CancelButton = btnCancel;
            }

            private static void AddField(
                TableLayoutPanel layout,
                string label,
                TextBox textBox,
                int row)
            {
                layout.Controls.Add(new Label
                {
                    Text = label,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                }, 0, row);

                textBox.Dock = DockStyle.Fill;
                textBox.Margin = new Padding(3, 8, 3, 8);

                layout.Controls.Add(textBox, 1, row);
            }

            private void SaveCustomer(CustomerRow? existing)
            {
                string name = txtName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string address = txtAddress.Text.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Vui lòng nhập tên khách hàng.");
                    txtName.Focus();
                    return;
                }

                if (phone.Length > 20)
                {
                    MessageBox.Show(
                        "Số điện thoại không được vượt quá 20 ký tự.");
                    txtPhone.Focus();
                    return;
                }

                if (!int.TryParse(txtPoints.Text.Trim(), out int points)
                    || points < 0)
                {
                    MessageBox.Show(
                        "Điểm tích lũy phải là số nguyên không âm.");
                    txtPoints.Focus();
                    return;
                }

                Customer = new CustomerRow
                {
                    CustomerId = existing?.CustomerId ?? 0,
                    CustomerName = name,
                    PhoneNumber = phone,
                    Address = address,
                    RewardPoints = points,
                    MembershipRank =
                        cboRank.SelectedItem?.ToString() ?? "Đồng"
                };

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}