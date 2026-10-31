using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormProductManagement : Form
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/panel)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color accentHover = Color.FromArgb(147, 51, 234);    // Tím đậm khi hover
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color btnGreen = Color.FromArgb(22, 163, 74);        // Xanh lá (Thêm)
        private readonly Color btnOrange = Color.FromArgb(234, 88, 12);       // Cam (Sửa)
        private readonly Color btnRed = Color.FromArgb(220, 38, 38);          // Đỏ (Xóa)

        // ================= PHÂN TRANG =================
        private List<ProductDto> _allProducts = new();
        private int _currentPage = 1;
        private int _pageSize = 10;

        private Label lblPageInfo = new();
        private Button btnPreviousPage = new();
        private Button btnNextPage = new();
        private ComboBox cboPageSize = new();
        private FlowLayoutPanel paginationPanel = new();

        // Các controls chính
        private DataGridView dgvProducts;
        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnLoad;
        private TextBox txtId;
        private TextBox txtBarcode;
        private TextBox txtProductName;
        private TextBox txtPrice;
        private TextBox txtStock;
        private ComboBox cbCategory;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormProductManagement()
        {
            InitializeComponentCustom();
            InitializePagination();
            SessionManager.AddAuthorizationHeader(_client);
            ApplyPermissions();
        }

        private void InitializeComponentCustom()
        {
            Text = "KhaMiniMart | Quản lý sản phẩm";
            BackColor = darkBackground;
            ForeColor = textLight;
            Font = new Font("Segoe UI", 10F);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(15);

            // 1. Tiêu đề đầu Form
            var heading = new Label
            {
                Text = "QUẢN LÝ SẢN PHẨM",
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
                Text = "Tìm kiếm sản phẩm",
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
                Text = "Thông tin sản phẩm",
                Dock = DockStyle.Fill,
                ForeColor = textLight,
                BackColor = cardBackground,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(15)
            };

            // Các trường nhập liệu bên phải
            var lblId = new Label { Text = "Mã ID (Tự tăng)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtId = new TextBox { Dock = DockStyle.Top, ReadOnly = true, BackColor = Color.FromArgb(71, 85, 105), ForeColor = textLight, Margin = new Padding(0, 4, 0, 10), Height = 28 };

            var lblBarcode = new Label { Text = "Mã vạch", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtBarcode = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 10), Height = 28 };

            var lblName = new Label { Text = "Tên sản phẩm", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtProductName = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 10), Height = 28 };

            var lblPrice = new Label { Text = "Giá bán (VNĐ)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtPrice = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 10), Height = 28 };

            var lblStock = new Label { Text = "Số lượng tồn kho", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            txtStock = new TextBox { Dock = DockStyle.Top, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 10), Height = 28 };

            var lblCat = new Label { Text = "Danh mục", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), ForeColor = textMuted };
            cbCategory = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = inputBackground, ForeColor = textLight, Margin = new Padding(0, 4, 0, 15), Height = 28 };

            // Bảng chứa 3 nút Thêm, Cập nhật, Xóa ở dưới cùng của khung bên phải
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

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            // Thêm các control vào GroupBox (Thêm theo thứ tự từ dưới lên do dùng Dock.Top)
            gbDetail.Controls.Add(tableButtons);
            gbDetail.Controls.Add(cbCategory);
            gbDetail.Controls.Add(lblCat);
            gbDetail.Controls.Add(txtStock);
            gbDetail.Controls.Add(lblStock);
            gbDetail.Controls.Add(txtPrice);
            gbDetail.Controls.Add(lblPrice);
            gbDetail.Controls.Add(txtProductName);
            gbDetail.Controls.Add(lblName);
            gbDetail.Controls.Add(txtBarcode);
            gbDetail.Controls.Add(lblBarcode);
            gbDetail.Controls.Add(txtId);
            gbDetail.Controls.Add(lblId);

            pnlRight.Controls.Add(gbDetail);

            // 4. DataGridView ở giữa hiển thị danh sách sản phẩm
            dgvProducts = new DataGridView();
            StyleDataGridView(dgvProducts);

            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductId", HeaderText = "Mã SP", DataPropertyName = "ProductId", FillWeight = 50 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Barcode", HeaderText = "Mã vạch", DataPropertyName = "Barcode", FillWeight = 90 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Tên sản phẩm", DataPropertyName = "ProductName", FillWeight = 150 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Giá (VNĐ)", DataPropertyName = "Price", FillWeight = 80 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "StockQuantity", HeaderText = "Tồn kho", DataPropertyName = "StockQuantity", FillWeight = 60 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryName", HeaderText = "Tên danh mục", DataPropertyName = "CategoryName", FillWeight = 100 });

            dgvProducts.CellClick += dgvProducts_CellClick;

            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 5, 5, 0), BackColor = darkBackground };
            pnlCenter.Controls.Add(dgvProducts);

            // Thêm tất cả vào Form chính
            Controls.Add(pnlCenter);
            Controls.Add(pnlRight);
            Controls.Add(pnlSearch);
            Controls.Add(heading);

            // Style nút bấm màu sắc chuẩn
            StyleActionButton(btnAdd, btnGreen);
            StyleActionButton(btnUpdate, btnOrange);
            StyleActionButton(btnDelete, btnRed);
            StyleActionButton(btnLoad, accentColor);
            StyleActionButton(btnSearch, accentColor);

            Load += FormProductManagement_Load;
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

            Control parent = dgvProducts.Parent;
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

        private async void FormProductManagement_Load(object sender, EventArgs e)
        {
            await LoadCategoriesToComboBoxAsync();
            await LoadProductsAsync();
        }

        private async Task LoadCategoriesToComboBoxAsync()
        {
            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                var categories = await _client.GetFromJsonAsync<List<CategoryDto>>("categories");
                if (categories != null)
                {
                    cbCategory.DataSource = categories;
                    cbCategory.DisplayMember = "CategoryName";
                    cbCategory.ValueMember = "CategoryId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh mục:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                using var response = await _client.GetAsync("products");

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Không thể tải danh sách sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                var list = new List<ProductDto>();

                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var product in root.EnumerateArray())
                    {
                        string categoryName = string.Empty;

                        if (product.TryGetProperty("category", out var categoryObj) && categoryObj.ValueKind == JsonValueKind.Object)
                        {
                            if (categoryObj.TryGetProperty("categoryName", out var catNameProp))
                            {
                                categoryName = catNameProp.GetString() ?? string.Empty;
                            }
                        }

                        list.Add(new ProductDto
                        {
                            ProductId = product.TryGetProperty("productId", out var idProp) ? idProp.GetInt32() : 0,
                            Barcode = product.TryGetProperty("barcode", out var barProp) ? barProp.GetString() : null,
                            ProductName = product.TryGetProperty("productName", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty,
                            Price = product.TryGetProperty("price", out var priceProp) ? priceProp.GetDecimal() : 0,
                            StockQuantity = product.TryGetProperty("stockQuantity", out var stockProp) ? stockProp.GetInt32() : 0,
                            CategoryId = product.TryGetProperty("categoryId", out var catIdProp) ? catIdProp.GetInt32() : 0,
                            CategoryName = categoryName
                        });
                    }
                }

                SetProducts(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách sản phẩm:\n" + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetProducts(List<ProductDto>? products)
        {
            _allProducts = products ?? new List<ProductDto>();
            _currentPage = 1;
            DisplayCurrentPage();
        }

        private void DisplayCurrentPage()
        {
            int totalItems = _allProducts.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)_pageSize));

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageData = _allProducts
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            dgvProducts.DataSource = null;
            dgvProducts.DataSource = pageData;
            dgvProducts.ClearSelection();

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            int totalItems = _allProducts.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)_pageSize));

            int firstItem = totalItems == 0 ? 0 : (_currentPage - 1) * _pageSize + 1;
            int lastItem = Math.Min(_currentPage * _pageSize, totalItems);

            lblPageInfo.Text = $"Trang {_currentPage}/{totalPages}   |   {firstItem}-{lastItem} / {totalItems} sản phẩm";

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
            int totalPages = Math.Max(1, (int)Math.Ceiling(_allProducts.Count / (double)_pageSize));
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

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            txtKeyword.Clear();
            await LoadProductsAsync();
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvProducts.Rows[e.RowIndex];
            if (row.DataBoundItem is ProductDto p)
            {
                txtId.Text = p.ProductId.ToString();
                txtBarcode.Text = p.Barcode ?? string.Empty;
                txtProductName.Text = p.ProductName ?? string.Empty;
                txtPrice.Text = p.Price.ToString();
                txtStock.Text = p.StockQuantity.ToString();
                cbCategory.SelectedValue = p.CategoryId;
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductName.Focus();
                return;
            }

            var newProduct = new
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = decimal.TryParse(txtPrice.Text, out var p) ? p : 0,
                StockQuantity = int.TryParse(txtStock.Text, out var s) ? s : 0,
                CategoryId = cbCategory.SelectedValue != null ? (int)cbCategory.SelectedValue : 0
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                using var response = await _client.PostAsJsonAsync("products", newProduct);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm sản phẩm thành công!", "KhaMiniMart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Thêm thất bại: " + err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa từ bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductName.Focus();
                return;
            }

            var updateProduct = new
            {
                ProductId = id,
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = decimal.TryParse(txtPrice.Text, out var p) ? p : 0,
                StockQuantity = int.TryParse(txtStock.Text, out var s) ? s : 0,
                CategoryId = cbCategory.SelectedValue != null ? (int)cbCategory.SelectedValue : 0
            };

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                using var response = await _client.PutAsJsonAsync($"products/{id}", updateProduct);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!", "KhaMiniMart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Cập nhật thất bại: " + err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                using var response = await _client.DeleteAsync($"products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "KhaMiniMart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                await LoadProductsAsync();
                return;
            }

            try
            {
                SessionManager.AddAuthorizationHeader(_client);
                var result = await _client.GetFromJsonAsync<List<ProductDto>>($"products/search?keyword={Uri.EscapeDataString(keyword)}");
                SetProducts(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tìm thấy sản phẩm:\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            dgvProducts.ClearSelection();
        }

        private void ApplyPermissions()
        {
            bool canManage = SessionManager.IsAdmin || SessionManager.IsWarehouse;
            btnAdd.Enabled = canManage;
            btnUpdate.Enabled = canManage;
            btnDelete.Enabled = canManage;
        }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}