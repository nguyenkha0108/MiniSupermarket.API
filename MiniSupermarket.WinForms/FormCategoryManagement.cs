using System.Net.Http.Json;

namespace MiniSupermarket.WinForms
{
    public partial class FormCategoryManagement : Form
    {
        private static readonly HttpClient _client =
            new HttpClient
            {
                BaseAddress =
                    new Uri("https://localhost:7044/api/")
            };

        public FormCategoryManagement()
        {
            InitializeComponent();

            // Gửi JWT cho tất cả request của Form này
            SessionManager.AddAuthorizationHeader(_client);

            // Phân quyền giao diện
            ApplyPermissions();
        }

        // =====================================================
        // PHÂN QUYỀN GIAO DIỆN
        // =====================================================
        private void ApplyPermissions()
        {
            if (SessionManager.IsAdmin)
            {
                // Admin được toàn quyền
                btnAdd.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
            else if (SessionManager.IsCashier)
            {
                // Cashier chỉ được xem / tìm kiếm
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                // Không có role
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }


        // =====================================================
        // FORM LOAD
        // =====================================================
        private async void FormCategoryManagement_Load(
            object sender,
            EventArgs e)
        {
            // Kiểm tra đăng nhập
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

            // Hiển thị role trên tiêu đề Form
            Text =
                $"Quản lý nhóm hàng - " +
                $"{SessionManager.Username} - " +
                $"{SessionManager.Role}";

            await LoadDataAsync();
        }


        // =====================================================
        // LOAD DATA
        // =====================================================
        private async Task LoadDataAsync()
        {
            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var categories =
                    await _client.GetFromJsonAsync<List<CategoryDto>>(
                        "categories");

                dgvCategories.DataSource = categories;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    "Không thể kết nối đến API:\n" +
                    ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải dữ liệu:\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // =====================================================
        // LOAD / REFRESH
        // =====================================================
        private async void btnLoad_Click(
            object sender,
            EventArgs e)
        {
            await LoadDataAsync();
        }


        // =====================================================
        // CLICK DATA GRID
        // =====================================================
        private void dgvCategories_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvCategories.Rows[e.RowIndex];

            if (row.Cells["CategoryId"].Value != null)
            {
                txtId.Text =
                    row.Cells["CategoryId"]
                        .Value
                        .ToString();
            }

            if (row.Cells["CategoryName"].Value != null)
            {
                txtCategoryName.Text =
                    row.Cells["CategoryName"]
                        .Value
                        .ToString();
            }

            txtDescription.Text =
                row.Cells["Description"]
                    .Value?
                    .ToString() ?? string.Empty;
        }


        // =====================================================
        // ADD
        // CHỈ ADMIN
        // =====================================================
        private async void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            // Kiểm tra role lần nữa
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show(
                    "Chỉ Admin mới được thêm nhóm hàng!",
                    "Không có quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var newCat = new
            {
                CategoryName =
                    txtCategoryName.Text.Trim(),

                Description =
                    txtDescription.Text.Trim()
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var response =
                    await _client.PostAsJsonAsync(
                        "categories",
                        newCat);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Thêm mới thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();

                    ClearInputs();
                }
                else if (response.StatusCode ==
                         System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show(
                        "Bạn không có quyền thêm nhóm hàng!",
                        "403 - Forbidden",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Thêm mới thất bại!\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // =====================================================
        // UPDATE
        // CHỈ ADMIN
        // =====================================================
        private async void btnUpdate_Click(
            object sender,
            EventArgs e)
        {
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show(
                    "Chỉ Admin mới được cập nhật nhóm hàng!",
                    "Không có quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhóm hàng cần sửa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!int.TryParse(
                txtId.Text,
                out int id))
            {
                MessageBox.Show(
                    "ID không hợp lệ!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var updateCat = new
            {
                CategoryId = id,

                CategoryName =
                    txtCategoryName.Text.Trim(),

                Description =
                    txtDescription.Text.Trim()
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var response =
                    await _client.PutAsJsonAsync(
                        $"categories/{id}",
                        updateCat);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Cập nhật thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();

                    ClearInputs();
                }
                else if (response.StatusCode ==
                         System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show(
                        "Bạn không có quyền cập nhật!",
                        "403 - Forbidden",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Cập nhật thất bại!\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // =====================================================
        // DELETE
        // CHỈ ADMIN
        // =====================================================
        private async void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show(
                    "Chỉ Admin mới được xóa nhóm hàng!",
                    "Không có quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhóm hàng cần xóa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!int.TryParse(
                txtId.Text,
                out int id))
            {
                MessageBox.Show(
                    "ID không hợp lệ!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var confirm =
                MessageBox.Show(
                    $"Bạn có chắc muốn xóa nhóm hàng ID = {id}?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var response =
                    await _client.DeleteAsync(
                        $"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Xóa thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();

                    ClearInputs();
                }
                else if (response.StatusCode ==
                         System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show(
                        "Bạn không có quyền xóa!",
                        "403 - Forbidden",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Xóa thất bại!\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // =====================================================
        // SEARCH
        // ADMIN + CASHIER
        // =====================================================
        private async void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            string keyword =
                txtKeyword.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);

                var result =
                    await _client.GetFromJsonAsync<
                        List<CategoryDto>>(
                        $"categories/search?keyword={Uri.EscapeDataString(keyword)}");

                dgvCategories.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tìm thấy kết quả phù hợp!\n" +
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }


        // =====================================================
        // CLEAR INPUTS
        // =====================================================
        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();
        }
    }


    // =========================================================
    // DTO
    // =========================================================
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
            = string.Empty;

        public string? Description { get; set; }
    }
}