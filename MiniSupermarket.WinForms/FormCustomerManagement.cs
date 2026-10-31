using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/card/panel)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color accentHover = Color.FromArgb(147, 51, 234);    // Tím đậm khi hover
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color btnGreen = Color.FromArgb(22, 163, 74);        // Xanh lá (Thêm)
        private readonly Color btnOrange = Color.FromArgb(234, 88, 12);       // Cam (Sửa)
        private readonly Color btnRed = Color.FromArgb(220, 38, 38);          // Đỏ (Xóa)

        // ================= PHÂN TRANG =================
        private List<CustomerRow> _allCustomers = new();
        private int _currentPage = 1;
        private int _pageSize = 10;

        private Label lblPageInfo = new();
        private Button btnPreviousPage = new();
        private Button btnNextPage = new();
        private ComboBox cboPageSize = new();
        private FlowLayoutPanel paginationPanel = new();

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
            InitializePagination();
            SessionManager.AddAuthorizationHeader(_client);
            ApplyPermissions();
        }

        private void InitializeComponentCustom()
        {
            Text = "KhaMiniMart | Quản lý khách hàng";
            BackColor = darkBackground;
            ForeColor = textLight;
            Font = new Font("Segoe UI", 10F);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(15);

            // 1. Tiêu đề đầu Form
            var heading = new Label
            {
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = textLight,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 2. Thanh tìm kiếm (Panel Top)
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = darkBackground
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm khách hàng",
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            txtKeyword = new TextBox { Dock = DockStyle.Left, Width = 450, MinimumSize = new Size(150, 32), BackColor = inputBackground, ForeColor = textLight };
            btnSearch = new Button { Text = "Tìm kiếm", Width = 100, Dock = DockStyle.Left, Margin = new Padding(8, 0, 0, 0), MinimumSize = new Size(100, 36) };
            btnLoad = new Button { Text = "Tải lại", Width = 100, Dock = DockStyle.Left, Margin = new Padding(8, 0, 0, 0), MinimumSize = new Size(100, 36) };

            btnSearch.Click += btnSearch_Click;
            btnLoad.Click += btnLoad_Click;

            var pnlSearchBox = new Panel { Dock = DockStyle.Fill, Height = 36 };
            pnlSearchBox.Controls.Add(btnLoad);
            pnlSearchBox.Controls.Add(btnSearch);
            pnlSearchBox.Controls.Add(txtKeyword);

            pnlSearch.Controls.Add(pnlSearchBox);
            pnlSearch.Controls.Add(lblSearch);

            // 3. Khung chi tiết bên phải (Panel Right)
            var pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 360,
                BackColor = darkBackground,
                Padding = new Padding(15, 0, 0, 0)
            };

            var gbDetail = new GroupBox
            {
                Text = "Thông tin khách hàng",
                Dock = DockStyle.Fill,
                ForeColor = textLight,
                BackColor = cardBackground,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(15)
            };

            // Các trường nhập liệu bên phải
            var lblId = new Label { Text = "Mã KH (Tự tăng)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtId = new TextBox { Dock = DockStyle.Top, ReadOnly = true, BackColor = Color.FromArgb(71, 85, 105), ForeColor = textLight, Margin = new Padding(0, 4, 0, 12), Height = 28 };

            var lblName = new Label { Text = "Tên khách hàng", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtName = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 12), Height = 28 };

            var lblPhone = new Label { Text = "Số điện thoại", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtPhone = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 12), Height = 28 };

            var lblAddress = new Label { Text = "Địa chỉ", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtAddress = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 12), Height = 28 };

            var lblPoints = new Label { Text = "Điểm tích lũy", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtPoints = new TextBox { Text = "0", Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 12), Height = 28 };

            var lblRank = new Label { Text = "Hạng thành viên", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            cboRank = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 15), Height = 28 };
            cboRank.Items.AddRange(MembershipRanks);
            cboRank.SelectedIndex = 0;

            // Bảng chứa 3 nút Thêm mới, Cập nhật, Xóa ở dưới cùng của khung bên phải
            btnAdd = new Button { Text = "Thêm mới", Dock = DockStyle.Fill };
            btnUpdate = new Button { Text = "Cập nhật", Dock = DockStyle.Fill };
            btnDelete = new Button { Text = "Xóa", Dock = DockStyle.Fill };

            var tableButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
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

            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 5, 5, 0), BackColor = darkBackground };
            pnlCenter.Controls.Add(dgvCustomers);

            // Thêm tất cả vào Form chính
            Controls.Add(pnlCenter);
            Controls.Add(pnlRight);
            Controls.Add(pnlSearch);
            Controls.Add(heading);

            // Style màu sắc các nút chức năng chính
            StyleActionButton(btnAdd, btnGreen);
            StyleActionButton(btnUpdate, btnOrange);
            StyleActionButton(btnDelete, btnRed);
            StyleActionButton(btnLoad, accentColor);
            StyleActionButton(btnSearch, accentColor);

            Load += async (_, _) => await LoadCustomersAsync();
        }

        private void InitializePagination()
        {
            paginationPanel = new FlowLayoutPanel
            {
                Name = "paginationPanel",
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10),
                Margin = Padding.Empty,
                BackColor = cardBackground,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };

            var lblPageSize = new Label
            {
                Text = "Số dòng/trang:",
                AutoSize = true,
                Margin = new Padding(5, 12, 5, 0),
                ForeColor = textMuted
            };

            cboPageSize = new ComboBox
            {
                Name = "cboPageSize",
                Width = 80,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = inputBackground,
                ForeColor = textLight,
                Margin = new Padding(3, 8, 15, 0)
            };

            cboPageSize.Items.AddRange(new object[] { 5, 10, 20, 50 });
            cboPageSize.SelectedItem = 10;

            btnPreviousPage = new Button
            {
                Name = "btnPreviousPage",
                Text = "◀ Trước",
                Width = 100,
                Height = 36,
                Margin = new Padding(3, 5, 5, 0)
            };

            lblPageInfo = new Label
            {
                Name = "lblPageInfo",
                Text = "Trang 0/0",
                AutoSize = true,
                MinimumSize = new Size(180, 34),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(5, 6, 5, 0),
                ForeColor = textLight,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            btnNextPage = new Button
            {
                Name = "btnNextPage",
                Text = "Sau ▶",
                Width = 100,
                Height = 36,
                Margin = new Padding(5, 5, 3, 0)
            };

            StyleActionButton(btnPreviousPage, accentColor);
            StyleActionButton(btnNextPage, accentColor);

            paginationPanel.Controls.Add(lblPageSize);
            paginationPanel.Controls.Add(cboPageSize);
            paginationPanel.Controls.Add(btnPreviousPage);
            paginationPanel.Controls.Add(lblPageInfo);
            paginationPanel.Controls.Add(btnNextPage);

            Control parent = dgvCustomers.Parent;
            if (parent != null)
            {
                parent.Controls.Add(paginationPanel);
                paginationPanel.BringToFront();
            }

            btnPreviousPage.Click += btnPreviousPage_Click;
            btnNextPage.Click += btnNextPage_Click;
            cboPageSize.SelectedIndexChanged += cboPageSize_SelectedIndexChanged;

            UpdatePaginationControls();
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
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.BackColor = cardBackground;
            grid.DefaultCellStyle.ForeColor = textLight;
            grid.DefaultCellStyle.SelectionBackColor = accentColor;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 47, 68);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = textLight;
        }

        private void StyleActionButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.15F);
            button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
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

                SetCustomers(customers);
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách khách hàng.", ex);
            }
        }

        private void SetCustomers(List<CustomerRow>? customers)
        {
            _allCustomers = customers ?? new List<CustomerRow>();
            _currentPage = 1;
            DisplayCurrentPage();
        }

        private void DisplayCurrentPage()
        {
            int totalItems = _allCustomers.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)_pageSize));

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageData = _allCustomers
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            dgvCustomers.DataSource = null;
            dgvCustomers.DataSource = pageData;
            dgvCustomers.ClearSelection();

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            int totalItems = _allCustomers.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)_pageSize));

            int firstItem = totalItems == 0 ? 0 : (_currentPage - 1) * _pageSize + 1;
            int lastItem = Math.Min(_currentPage * _pageSize, totalItems);

            lblPageInfo.Text = $"Trang {_currentPage}/{totalPages}   |   {firstItem}-{lastItem} / {totalItems} khách hàng";

            btnPreviousPage.Enabled = totalItems > 0 && _currentPage > 1;
            btnNextPage.Enabled = totalItems > 0 && _currentPage < totalPages;
        }

        private void btnPreviousPage_Click(object? sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                DisplayCurrentPage();
            }
        }

        private void btnNextPage_Click(object? sender, EventArgs e)
        {
            int totalPages = Math.Max(1, (int)Math.Ceiling(_allCustomers.Count / (double)_pageSize));
            if (_currentPage < totalPages)
            {
                _currentPage++;
                DisplayCurrentPage();
            }
        }

        private void cboPageSize_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboPageSize.SelectedItem == null) return;

            _pageSize = Convert.ToInt32(cboPageSize.SelectedItem);
            _currentPage = 1;
            DisplayCurrentPage();
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
                SetCustomers(result);
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