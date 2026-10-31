using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormCategoryManagement : Form
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/card)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color accentHover = Color.FromArgb(147, 51, 234);    // Tím đậm khi hover
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color btnGreen = Color.FromArgb(22, 163, 74);        // Xanh lá (Thêm)
        private readonly Color btnOrange = Color.FromArgb(234, 88, 12);       // Cam (Sửa)
        private readonly Color btnRed = Color.FromArgb(220, 38, 38);          // Đỏ (Xóa)

        // ================= PHÂN TRANG =================

        private List<CategoryDto> _allCategories = new();

        private int _currentPage = 1;
        private int _pageSize = 10;

        private Label lblPageInfo = new();
        private Button btnPreviousPage = new();
        private Button btnNextPage = new();
        private ComboBox cboPageSize = new();

        private FlowLayoutPanel paginationPanel = new();

        public FormCategoryManagement()
        {
            InitializeComponent();

            StyleExistingControls();

            InitializePagination();

            SessionManager.AddAuthorizationHeader(_client);

            ApplyPermissions();
        }

        // =====================================================
        // GIAO DIỆN DARK MODE
        // =====================================================

        private void StyleExistingControls()
        {
            Text = "KhaMiniMart | Quản lý danh mục";
            BackColor = darkBackground;
            ForeColor = textLight;
            Font = new Font("Segoe UI", 10F);

            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(15);

            StyleControlsRecursively(this);

            dgvCategories.Dock = DockStyle.Fill;
            dgvCategories.Margin = Padding.Empty;

            dgvCategories.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCategories.ScrollBars = ScrollBars.Both;

            StyleActionButton(btnAdd, btnGreen);
            StyleActionButton(btnUpdate, btnOrange);
            StyleActionButton(btnDelete, btnRed);
            StyleActionButton(btnLoad, accentColor);
            StyleActionButton(btnSearch, accentColor);

            btnSearch.MinimumSize = new Size(95, 38);
            btnLoad.MinimumSize = new Size(95, 38);

            txtKeyword.MinimumSize = new Size(150, 30);
        }

        private void StyleControlsRecursively(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is DataGridView grid)
                {
                    grid.BackgroundColor = cardBackground;
                    grid.BorderStyle = BorderStyle.None;
                    grid.EnableHeadersVisualStyles = false;
                    grid.RowHeadersVisible = false;

                    grid.ReadOnly = true;
                    grid.AllowUserToAddRows = false;
                    grid.AllowUserToDeleteRows = false;
                    grid.AllowUserToResizeRows = false;

                    grid.SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect;

                    grid.MultiSelect = false;
                    grid.GridColor = Color.FromArgb(51, 65, 85);
                    grid.RowTemplate.Height = 40;
                    grid.ColumnHeadersHeight = 44;

                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
                    grid.ColumnHeadersDefaultCellStyle.Font =
                        new Font("Segoe UI", 10F, FontStyle.Bold);

                    grid.DefaultCellStyle.Font =
                        new Font("Segoe UI", 10F);

                    grid.DefaultCellStyle.BackColor = cardBackground;
                    grid.DefaultCellStyle.ForeColor = textLight;

                    grid.DefaultCellStyle.SelectionBackColor =
                        accentColor;

                    grid.DefaultCellStyle.SelectionForeColor =
                        Color.White;

                    grid.AlternatingRowsDefaultCellStyle.BackColor =
                        Color.FromArgb(35, 47, 68);

                    grid.AlternatingRowsDefaultCellStyle.ForeColor = textLight;

                    grid.Margin = Padding.Empty;
                }
                else if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = accentColor;
                    button.ForeColor = Color.White;

                    button.Font = new Font(
                        "Segoe UI", 9.5F, FontStyle.Bold);

                    button.Cursor = Cursors.Hand;
                    button.UseVisualStyleBackColor = false;

                    button.FlatAppearance.MouseOverBackColor =
                        accentHover;
                }
                else if (control is TextBox textBox)
                {
                    textBox.Font = new Font("Segoe UI", 10.5F);
                    textBox.BackColor = inputBackground;
                    textBox.ForeColor = textLight;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is Label label)
                {
                    label.ForeColor = textMuted;

                    if (label.Font.Size >= 14)
                    {
                        label.Font = new Font(
                            "Segoe UI", 16F, FontStyle.Bold);

                        label.ForeColor = textLight;
                    }
                }

                if (control.HasChildren)
                {
                    StyleControlsRecursively(control);
                }
            }
        }

        private void StyleActionButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            button.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(color, 0.15F);

            button.Font = new Font(
                "Segoe UI", 9.5F, FontStyle.Bold);

            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        // =====================================================
        // KHỞI TẠO ĐIỀU KHIỂN PHÂN TRANG (DARK THEME)
        // =====================================================

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

            cboPageSize.Items.AddRange(
                new object[] { 5, 10, 20, 50 });

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

            Control parent = dgvCategories.Parent;

            if (parent != null)
            {
                parent.Controls.Add(paginationPanel);
                paginationPanel.BringToFront();
            }

            btnPreviousPage.Click += btnPreviousPage_Click;
            btnNextPage.Click += btnNextPage_Click;

            cboPageSize.SelectedIndexChanged +=
                cboPageSize_SelectedIndexChanged;

            UpdatePaginationControls();
        }

        // =====================================================
        // PHÂN QUYỀN
        // =====================================================

        private void ApplyPermissions()
        {
            bool canManage =
                SessionManager.IsAdmin ||
                SessionManager.IsWarehouse;

            btnAdd.Enabled = canManage;
            btnUpdate.Enabled = canManage;
            btnDelete.Enabled = canManage;

            btnLoad.Enabled = true;
            btnSearch.Enabled = true;
        }

        // =====================================================
        // FORM LOAD
        // =====================================================

        private async void FormCategoryManagement_Load(
            object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                MessageBox.Show(
                    "Bạn chưa đăng nhập!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                Close();
                return;
            }

            Text = $"KhaMiniMart | Quản lý danh mục - " +
                   $"{SessionManager.Username} - {SessionManager.Role}";

            await LoadDataAsync();
        }

        // =====================================================
        // TẢI DANH SÁCH
        // =====================================================

        private async Task LoadDataAsync()
        {
            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var categories =
                    await _client.GetFromJsonAsync<List<CategoryDto>>(
                        "categories");

                SetCategories(categories);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    "Không thể kết nối đến API:\n" + ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải dữ liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        // =====================================================
        // NHẬN DANH SÁCH VÀ HIỂN THỊ THEO TRANG
        // =====================================================

        private void SetCategories(List<CategoryDto>? categories)
        {
            _allCategories = categories ?? new List<CategoryDto>();

            _currentPage = 1;

            DisplayCurrentPage();
        }

        private void DisplayCurrentPage()
        {
            int totalItems = _allCategories.Count;

            int totalPages = Math.Max(
                1,
                (int)Math.Ceiling(totalItems / (double)_pageSize));

            if (_currentPage > totalPages)
            {
                _currentPage = totalPages;
            }

            if (_currentPage < 1)
            {
                _currentPage = 1;
            }

            var pageData = _allCategories
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            dgvCategories.DataSource = null;
            dgvCategories.DataSource = pageData;

            dgvCategories.ClearSelection();

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            int totalItems = _allCategories.Count;

            int totalPages = Math.Max(
                1,
                (int)Math.Ceiling(totalItems / (double)_pageSize));

            int firstItem = totalItems == 0
                ? 0
                : (_currentPage - 1) * _pageSize + 1;

            int lastItem = Math.Min(
                _currentPage * _pageSize,
                totalItems);

            lblPageInfo.Text =
                $"Trang {_currentPage}/{totalPages}   |   " +
                $"{firstItem}-{lastItem} / {totalItems} danh mục";

            btnPreviousPage.Enabled =
                totalItems > 0 && _currentPage > 1;

            btnNextPage.Enabled =
                totalItems > 0 && _currentPage < totalPages;
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
            int totalPages = Math.Max(
                1,
                (int)Math.Ceiling(
                    _allCategories.Count / (double)_pageSize));

            if (_currentPage < totalPages)
            {
                _currentPage++;
                DisplayCurrentPage();
            }
        }

        private void cboPageSize_SelectedIndexChanged(
            object? sender, EventArgs e)
        {
            if (cboPageSize.SelectedItem == null)
                return;

            _pageSize = Convert.ToInt32(cboPageSize.SelectedItem);

            _currentPage = 1;

            DisplayCurrentPage();
        }

        // =====================================================
        // CHỌN DANH MỤC
        // =====================================================

        private void dgvCategories_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvCategories.Rows[e.RowIndex];

            if (row.DataBoundItem is CategoryDto category)
            {
                txtId.Text = category.CategoryId.ToString();
                txtCategoryName.Text = category.CategoryName;

                txtDescription.Text =
                    category.Description ?? string.Empty;
            }
        }

        // =====================================================
        // THÊM DANH MỤC
        // =====================================================

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!CanManageCategories())
                return;

            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCategoryName.Focus();
                return;
            }

            var newCategory = new
            {
                CategoryName = txtCategoryName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.PostAsJsonAsync(
                    "categories", newCategory);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Thêm danh mục thành công!",
                        "KhaMiniMart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowApiErrorAsync(response, "Thêm danh mục");
                }
            }
            catch (Exception ex)
            {
                ShowException("Lỗi thêm danh mục", ex);
            }
        }

        // =====================================================
        // SỬA DANH MỤC
        // =====================================================

        private async void btnUpdate_Click(
            object sender, EventArgs e)
        {
            if (!CanManageCategories())
                return;

            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Vui lòng chọn danh mục cần sửa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCategoryName.Focus();
                return;
            }

            var updateCategory = new
            {
                CategoryId = id,
                CategoryName = txtCategoryName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response = await _client.PutAsJsonAsync(
                    $"categories/{id}", updateCategory);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Cập nhật danh mục thành công!",
                        "KhaMiniMart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowApiErrorAsync(
                        response, "Cập nhật danh mục");
                }
            }
            catch (Exception ex)
            {
                ShowException("Lỗi cập nhật danh mục", ex);
            }
        }

        // =====================================================
        // XÓA DANH MỤC
        // =====================================================

        private async void btnDelete_Click(
            object sender, EventArgs e)
        {
            if (!CanManageCategories())
                return;

            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Vui lòng chọn danh mục cần xóa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string categoryName = txtCategoryName.Text.Trim();

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa danh mục \"{categoryName}\" (ID: {id})?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                using var response =
                    await _client.DeleteAsync($"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Xóa danh mục thành công!",
                        "KhaMiniMart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowApiErrorAsync(response, "Xóa danh mục");
                }
            }
            catch (Exception ex)
            {
                ShowException("Lỗi xóa danh mục", ex);
            }
        }

        // =====================================================
        // TÌM KIẾM
        // =====================================================

        private async void btnSearch_Click(
            object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                string url =
                    "categories/search?keyword=" +
                    Uri.EscapeDataString(keyword);

                var result =
                    await _client.GetFromJsonAsync<List<CategoryDto>>(url);

                SetCategories(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tìm được danh mục phù hợp:\n" + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        // =====================================================
        // KIỂM TRA QUYỀN
        // =====================================================

        private bool CanManageCategories()
        {
            if (SessionManager.IsAdmin || SessionManager.IsWarehouse)
                return true;

            MessageBox.Show(
                "Chỉ Admin hoặc Warehouse mới được thêm, sửa, xóa danh mục!",
                "Không có quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        // =====================================================
        // XỬ LÝ LỖI API
        // =====================================================

        private async Task ShowApiErrorAsync(
            HttpResponseMessage response, string action)
        {
            string detail =
                await response.Content.ReadAsStringAsync();

            string message = response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized =>
                    "Bạn chưa đăng nhập hoặc token đã hết hạn.",

                System.Net.HttpStatusCode.Forbidden =>
                    "Bạn không có quyền thực hiện thao tác này.",

                System.Net.HttpStatusCode.NotFound =>
                    "Không tìm thấy danh mục hoặc API endpoint.",

                System.Net.HttpStatusCode.Conflict =>
                    "Danh mục đang được sử dụng hoặc dữ liệu bị trùng.",

                _ => $"{action} thất bại! " +
                     $"Mã lỗi: {(int)response.StatusCode}."
            };

            if (!string.IsNullOrWhiteSpace(detail))
                message += "\n\nChi tiết: " + detail;

            MessageBox.Show(
                message,
                "Lỗi API",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private static void ShowException(
            string title, Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        // =====================================================
        // XÓA Ô NHẬP
        // =====================================================

        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();
            dgvCategories.ClearSelection();
        }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}