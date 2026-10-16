using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormCustomerManagement : Form

    {
        private const string ApiUrl = "Customers";
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color background = Color.FromArgb(243, 246, 251);
        private readonly Color green = Color.FromArgb(22, 163, 74);
        private readonly Color red = Color.FromArgb(220, 38, 38);
        private readonly Color orange = Color.FromArgb(234, 88, 12);

        // Các controls chính
        private DataGridView dgvCustomers;
        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnLoad;
        private TextBox txtId;
        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtPoints;
        private ComboBox cboRank;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        private static readonly string[] MembershipRanks = { "Đồng", "Bạc", "Vàng", "Kim cương" };

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FormCustomerManagement()
        {
            InitializeComponentCustom();
            SessionManager.AddAuthorizationHeader(_client);
            ApplyPermissions();
        }

        private void InitializeComponentCustom()
        {
            Text = "KhaMiniMart | Quản lý khách hàng";
            BackColor = background;
            ForeColor = Color.FromArgb(15, 23, 42);
            Font = new Font("Segoe UI", 10F);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(10);

            // 1. Tiêu đề đầu Form
            var heading = new Label
            {
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = navy
            };

            // 2. Thanh tìm kiếm (Panel Top)
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = background
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm khách hàng",
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            txtKeyword = new TextBox { Dock = DockStyle.Left, Width = 450 };
            btnSearch = new Button { Text = "Tìm kiếm", Width = 95, Dock = DockStyle.Left, Margin = new Padding(5, 0, 0, 0) };
            btnLoad = new Button { Text = "Tải lại", Width = 95, Dock = DockStyle.Left, Margin = new Padding(5, 0, 0, 0) };

            btnSearch.Click += btnSearch_Click;
            btnLoad.Click += btnLoad_Click;

            var pnlSearchBox = new Panel { Dock = DockStyle.Fill, Height = 30 };
            pnlSearchBox.Controls.Add(btnLoad);
            pnlSearchBox.Controls.Add(btnSearch);
            pnlSearchBox.Controls.Add(txtKeyword);

            pnlSearch.Controls.Add(pnlSearchBox);
            pnlSearch.Controls.Add(lblSearch);

            // 3. Khung chi tiết bên phải (Panel Right)
            var pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = background,
                Padding = new Padding(10, 0, 0, 0)
            };

            var gbDetail = new GroupBox
            {
                Text = "Thông tin khách hàng",
                Dock = DockStyle.Fill,
                ForeColor = navy,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10)
            };

            // Các trường nhập liệu bên phải
            var lblId = new Label { Text = "Mã KH (Tự tăng)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            txtId = new TextBox { Dock = DockStyle.Top, ReadOnly = true, BackColor = Color.WhiteSmoke, Margin = new Padding(0, 0, 0, 10) };

            var lblName = new Label { Text = "Tên khách hàng", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            txtName = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 10) };

            var lblPhone = new Label { Text = "Số điện thoại", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            txtPhone = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 10) };

            var lblAddress = new Label { Text = "Địa chỉ", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            txtAddress = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 10) };

            var lblPoints = new Label { Text = "Điểm tích lũy", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            txtPoints = new TextBox { Text = "0", Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 10) };

            var lblRank = new Label { Text = "Hạng thành viên", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            cboRank = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 0, 0, 10) };
            cboRank.Items.AddRange(MembershipRanks);
            cboRank.SelectedIndex = 0;

            // Bảng chứa 3 nút Thêm mới, Cập nhật, Xóa ở dưới cùng của khung bên phải
            btnAdd = new Button { Text = "Thêm mới", Dock = DockStyle.Fill };
            btnUpdate = new Button { Text = "Cập nhật", Dock = DockStyle.Fill };
            btnDelete = new Button { Text = "Xóa", Dock = DockStyle.Fill };

            var tableButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                ColumnCount = 3,
                RowCount = 1
            };
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
            tableButtons.Controls.Add(btnAdd, 0, 0);
            tableButtons.Controls.Add(btnUpdate, 1, 0);
            tableButtons.Controls.Add(btnDelete, 2, 0);

            btnAdd.Click += async (_, _) => await AddCustomerAsync();
            btnUpdate.Click += async (_, _) => await EditCustomerAsync();
            btnDelete.Click += async (_, _) => await DeleteCustomerAsync();

            // Thêm các control vào GroupBox (Thêm theo thứ tự từ dưới lên do dùng Dock.Top)
            gbDetail.Controls.Add(tableButtons);
            gbDetail.Controls.Add(cboRank);
            gbDetail.Controls.Add(lblRank);
            gbDetail.Controls.Add(txtPoints);
            gbDetail.Controls.Add(lblPoints);
            gbDetail.Controls.Add(txtAddress);
            gbDetail.Controls.Add(lblAddress);
            gbDetail.Controls.Add(txtPhone);
            gbDetail.Controls.Add(lblPhone);
            gbDetail.Controls.Add(txtName);
            gbDetail.Controls.Add(lblName);
            gbDetail.Controls.Add(txtId);
            gbDetail.Controls.Add(lblId);

            pnlRight.Controls.Add(gbDetail);

            // 4. DataGridView ở giữa hiển thị danh sách khách hàng
            dgvCustomers = new DataGridView();
            StyleDataGridView(dgvCustomers);

            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerId", HeaderText = "Mã KH", DataPropertyName = nameof(CustomerRow.CustomerId), FillWeight = 60 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", HeaderText = "Tên khách hàng", DataPropertyName = nameof(CustomerRow.CustomerName), FillWeight = 150 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhoneNumber", HeaderText = "Số điện thoại", DataPropertyName = nameof(CustomerRow.PhoneNumber), FillWeight = 110 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Address", HeaderText = "Địa chỉ", DataPropertyName = nameof(CustomerRow.Address), FillWeight = 180 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "RewardPoints", HeaderText = "Điểm tích lũy", DataPropertyName = nameof(CustomerRow.RewardPoints), FillWeight = 90 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "MembershipRank", HeaderText = "Hạng thành viên", DataPropertyName = nameof(CustomerRow.MembershipRank), FillWeight = 110 });

            dgvCustomers.CellClick += dgvCustomers_CellClick;

            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 5, 5, 0) };
            pnlCenter.Controls.Add(dgvCustomers);

            // Thêm tất cả vào Form chính
            Controls.Add(pnlCenter);
            Controls.Add(pnlRight);
            Controls.Add(pnlSearch);
            Controls.Add(heading);

            // Style màu sắc các nút chức năng chính
            StyleActionButton(btnAdd, green);
            StyleActionButton(btnUpdate, orange);
            StyleActionButton(btnDelete, red);
            StyleActionButton(btnLoad, blue);
            StyleActionButton(btnSearch, blue);

            Load += async (_, _) => await LoadCustomersAsync();
        }

        private void StyleDataGridView(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
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
            grid.GridColor = Color.FromArgb(226, 232, 240);
            grid.RowTemplate.Height = 38;
            grid.ColumnHeadersHeight = 42;

            grid.ColumnHeadersDefaultCellStyle.BackColor = navy;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void StyleActionButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(color, 0.12F);
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                using var response = await _client.GetAsync(ApiUrl);

                if (!await CheckResponseAsync(response))
                    return;

                string json = await response.Content.ReadAsStringAsync();
                var customers = JsonSerializer.Deserialize<List<CustomerRow>>(json, JsonOptions) ?? new List<CustomerRow>();

                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = customers;
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách khách hàng.", ex);
            }
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvCustomers.Rows[e.RowIndex];
            if (row.DataBoundItem is CustomerRow c)
            {
                txtId.Text = c.CustomerId.ToString();
                txtName.Text = c.CustomerName;
                txtPhone.Text = c.PhoneNumber;
                txtAddress.Text = c.Address;
                txtPoints.Text = c.RewardPoints.ToString();

                int rankIndex = Array.FindIndex(MembershipRanks, r => string.Equals(r, c.MembershipRank, StringComparison.OrdinalIgnoreCase));
                cboRank.SelectedIndex = rankIndex >= 0 ? rankIndex : 0;
            }
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            txtKeyword.Clear();
            ClearInputs();
            await LoadCustomersAsync();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                await LoadCustomersAsync();
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                var result = await _client.GetFromJsonAsync<List<CustomerRow>>($"Customers/search?keyword={Uri.EscapeDataString(keyword)}", JsonOptions);
                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = result ?? new List<CustomerRow>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tìm thấy khách hàng:\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task AddCustomerAsync()
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (phone.Length > 20)
            {
                MessageBox.Show("Số điện thoại không được vượt quá 20 ký tự.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (!int.TryParse(txtPoints.Text.Trim(), out int points) || points < 0)
            {
                MessageBox.Show("Điểm tích lũy phải là số nguyên không âm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPoints.Focus();
                return;
            }

            var newCustomer = new CustomerRow
            {
                CustomerName = name,
                PhoneNumber = phone,
                Address = address,
                RewardPoints = points,
                MembershipRank = cboRank.SelectedItem?.ToString() ?? "Đồng"
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                string json = JsonSerializer.Serialize(newCustomer, JsonOptions);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _client.PostAsync(ApiUrl, content);
                if (!await CheckResponseAsync(response))
                    return;

                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadCustomersAsync();
                ClearInputs();
            }
            catch (Exception ex)
            {
                ShowError("Không thêm được khách hàng.", ex);
            }
        }

        private async Task EditCustomerAsync()
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa từ bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (!int.TryParse(txtPoints.Text.Trim(), out int points) || points < 0)
            {
                MessageBox.Show("Điểm tích lũy phải là số nguyên không âm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPoints.Focus();
                return;
            }

            var updateCustomer = new CustomerRow
            {
                CustomerId = id,
                CustomerName = name,
                PhoneNumber = phone,
                Address = address,
                RewardPoints = points,
                MembershipRank = cboRank.SelectedItem?.ToString() ?? "Đồng"
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                string json = JsonSerializer.Serialize(updateCustomer, JsonOptions);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _client.PutAsync($"{ApiUrl}/{id}", content);
                if (!await CheckResponseAsync(response))
                    return;

                MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadCustomersAsync();
                ClearInputs();
            }
            catch (Exception ex)
            {
                ShowError("Không sửa được khách hàng.", ex);
            }
        }

        private async Task DeleteCustomerAsync()
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa từ bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng \"{txtName.Text}\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                using var response = await _client.DeleteAsync($"{ApiUrl}/{id}");

                if (!await CheckResponseAsync(response))
                    return;

                MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadCustomersAsync();
                ClearInputs();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được khách hàng.", ex);
            }
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtPoints.Text = "0";
            cboRank.SelectedIndex = 0;
            dgvCustomers.ClearSelection();
        }

        private async Task<bool> CheckResponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return true;

            string body = await response.Content.ReadAsStringAsync();
            string message = response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "Token hết hạn hoặc bạn chưa đăng nhập.",
                System.Net.HttpStatusCode.Forbidden => "Tài khoản không có quyền thực hiện thao tác này.",
                System.Net.HttpStatusCode.NotFound => "Không tìm thấy khách hàng hoặc API endpoint.",
                System.Net.HttpStatusCode.Conflict => "Dữ liệu bị trùng hoặc khách hàng đang được sử dụng.",
                _ => $"API trả về lỗi {(int)response.StatusCode} {response.ReasonPhrase}."
            };

            if (!string.IsNullOrWhiteSpace(body))
                message += "\n\nChi tiết: " + body;

            MessageBox.Show(message, "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show($"{message}\n\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ApplyPermissions()
        {
            bool canManage = SessionManager.IsAdmin || SessionManager.IsWarehouse;
            btnAdd.Enabled = canManage;
            btnUpdate.Enabled = canManage;
            btnDelete.Enabled = canManage;
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
    }
}