
using System;
using System.Collections.Generic;
using System.Drawing;
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

        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color background = Color.FromArgb(243, 246, 251);
        private readonly Color green = Color.FromArgb(22, 163, 74);
        private readonly Color red = Color.FromArgb(220, 38, 38);
        private readonly Color orange = Color.FromArgb(234, 88, 12);

        public FormCategoryManagement()
        {
            InitializeComponent();

            StyleExistingControls();

            SessionManager.AddAuthorizationHeader(_client);

            ApplyPermissions();
        }

        // =====================================================
        // GIAO DIỆN
        // =====================================================
        private void StyleExistingControls()
        {
            Text = "KhaMiniMart | Quản lý danh mục";
            BackColor = background;
            ForeColor = Color.FromArgb(15, 23, 42);
            Font = new Font("Segoe UI", 10F);

            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(10);

            StyleControlsRecursively(this);

            // Bảng danh mục tự co giãn theo vùng chứa
            dgvCategories.Dock = DockStyle.Fill;
            dgvCategories.Margin = Padding.Empty;
            dgvCategories.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.ScrollBars = ScrollBars.Both;

            // Màu các nút chức năng
            StyleActionButton(btnAdd, green);
            StyleActionButton(btnUpdate, orange);
            StyleActionButton(btnDelete, red);
            StyleActionButton(btnLoad, blue);
            StyleActionButton(btnSearch, blue);

            // Giữ kích thước nút, không ép nhỏ
            btnSearch.MinimumSize = new Size(95, 35);
            btnLoad.MinimumSize = new Size(95, 35);

            // Không ép chiều rộng ô tìm kiếm theo chiều rộng Form
            txtKeyword.MinimumSize = new Size(150, 25);
        }

        private void StyleControlsRecursively(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is DataGridView grid)
                {
                    grid.BackgroundColor = Color.White;
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

                    grid.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;
                    grid.AutoSizeRowsMode =
                        DataGridViewAutoSizeRowsMode.None;

                    grid.GridColor = Color.FromArgb(226, 232, 240);
                    grid.RowTemplate.Height = 38;
                    grid.ColumnHeadersHeight = 42;

                    grid.ColumnHeadersDefaultCellStyle.BackColor = navy;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersDefaultCellStyle.Font =
                        new Font("Segoe UI", 10F, FontStyle.Bold);

                    grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
                    grid.DefaultCellStyle.ForeColor =
                        Color.FromArgb(15, 23, 42);
                    grid.DefaultCellStyle.SelectionBackColor =
                        Color.FromArgb(219, 234, 254);
                    grid.DefaultCellStyle.SelectionForeColor =
                        Color.FromArgb(15, 23, 42);

                    grid.AlternatingRowsDefaultCellStyle.BackColor =
                        Color.FromArgb(248, 250, 252);

                    grid.Margin = Padding.Empty;
                }
                else if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = blue;
                    button.ForeColor = Color.White;
                    button.Font = new Font(
                        "Segoe UI", 9.5F, FontStyle.Bold);
                    button.Cursor = Cursors.Hand;
                    button.UseVisualStyleBackColor = false;
                    button.FlatAppearance.MouseOverBackColor =
                        Color.FromArgb(29, 78, 216);
                }
                else if (control is TextBox textBox)
                {
                    textBox.Font = new Font("Segoe UI", 10F);
                    textBox.BackColor = Color.White;
                    textBox.ForeColor =
                        Color.FromArgb(15, 23, 42);
                }
                else if (control is Label label)
                {
                    label.ForeColor = Color.FromArgb(51, 65, 85);

                    if (label.Font.Size >= 14)
                    {
                        label.Font =
                            new Font("Segoe UI", 16F, FontStyle.Bold);
                        label.ForeColor = navy;
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
                ControlPaint.Dark(color, 0.12F);
            button.Font = new Font(
                "Segoe UI", 9.5F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
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

                dgvCategories.DataSource = null;
                dgvCategories.DataSource =
                    categories ?? new List<CategoryDto>();
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

                dgvCategories.DataSource = null;
                dgvCategories.DataSource =
                    result ?? new List<CategoryDto>();
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
