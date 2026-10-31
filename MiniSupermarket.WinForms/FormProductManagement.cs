
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormProductManagement : Form
    {
        private readonly DataGridView grid = new();
        private readonly Label lblStatus = new();
        private readonly Button btnReload = new();

        private readonly Panel pnlActions = new();
        private readonly Button btnAdd = new();
        private readonly Button btnEdit = new();
        private readonly Button btnDelete = new();

        private const string ApiUrl =
            "https://localhost:7044/api/Products";

        private const string CategoryApiUrl =
            "https://localhost:7044/api/Categories";

        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color background = Color.FromArgb(243, 246, 251);
        private readonly Color green = Color.FromArgb(22, 163, 74);
        private readonly Color red = Color.FromArgb(220, 38, 38);

        public FormProductManagement()
        {
            Text = "KhaMiniMart | Quản lý sản phẩm";
            BackColor = background;
            Font = new Font("Segoe UI", 10F);
            Padding = new Padding(18);
            MinimumSize = new Size(850, 500);
            StartPosition = FormStartPosition.CenterScreen;

            var heading = new Label
            {
                Text = "QUẢN LÝ SẢN PHẨM",
                Dock = DockStyle.Top,
                Height = 42,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                ForeColor = navy
            };

            var subtitle = new Label
            {
                Text = "Theo dõi danh sách sản phẩm, giá bán và tồn kho",
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                Padding = new Padding(0, 8, 0, 8),
                BackColor = background
            };

            btnReload.Text = "↻  Tải lại sản phẩm";
            btnReload.Dock = DockStyle.Left;
            btnReload.Width = 175;
            StyleButton(btnReload, blue);
            btnReload.Click += async (_, _) => await LoadProductsAsync();
            toolbar.Controls.Add(btnReload);

            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Height = 32;
            lblStatus.Text = "Đang chờ tải dữ liệu...";
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            StyleGrid();

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductId",
                HeaderText = "Mã SP",
                DataPropertyName = nameof(ProductRow.ProductId),
                FillWeight = 65
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Barcode",
                HeaderText = "Mã vạch",
                DataPropertyName = nameof(ProductRow.Barcode),
                FillWeight = 105
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = nameof(ProductRow.ProductName),
                FillWeight = 180
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Giá (VNĐ)",
                DataPropertyName = nameof(ProductRow.Price),
                FillWeight = 100
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StockQuantity",
                HeaderText = "Tồn kho",
                DataPropertyName = nameof(ProductRow.StockQuantity),
                FillWeight = 75
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Danh mục",
                DataPropertyName = nameof(ProductRow.CategoryName),
                FillWeight = 120
            });

            // Khung chức năng bên phải
            pnlActions.Dock = DockStyle.Right;
            pnlActions.Width = 175;
            pnlActions.Padding = new Padding(12);
            pnlActions.BackColor = Color.White;

            var lblActionsTitle = new Label
            {
                Text = "CHỨC NĂNG",
                Dock = DockStyle.Top,
                Height = 42,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = navy
            };

            ConfigureActionButton(btnAdd, "+  Thêm sản phẩm", green);
            ConfigureActionButton(btnEdit, "✎  Sửa sản phẩm", blue);
            ConfigureActionButton(btnDelete, "X  Xóa sản phẩm", red);

            btnAdd.Click += async (_, _) => await AddProductAsync();
            btnEdit.Click += async (_, _) => await EditProductAsync();
            btnDelete.Click += async (_, _) => await DeleteProductAsync();

            pnlActions.Controls.Add(btnDelete);
            pnlActions.Controls.Add(btnEdit);
            pnlActions.Controls.Add(btnAdd);
            pnlActions.Controls.Add(lblActionsTitle);

            // Bảng và khung chức năng nằm cạnh nhau
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = background
            };

            contentPanel.Controls.Add(grid);
            contentPanel.Controls.Add(pnlActions);

            Controls.Add(contentPanel);
            Controls.Add(lblStatus);
            Controls.Add(toolbar);
            Controls.Add(subtitle);
            Controls.Add(heading);

            Load += async (_, _) => await LoadProductsAsync();
        }

        private void ConfigureActionButton(
            Button button, string text, Color color)
        {
            button.Text = text;
            button.Dock = DockStyle.Top;
            button.Height = 44;
            StyleButton(button, color);
        }

        private void StyleGrid()
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoGenerateColumns = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.BackgroundColor = Color.White;
            grid.RowHeadersVisible = false;
            grid.BorderStyle = BorderStyle.None;
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

        private static void StyleButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                ControlPaint.Dark(color, 0.12F);
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
        }

        // ==================== TẢI SẢN PHẨM ====================

        private async Task LoadProductsAsync()
        {
            btnReload.Enabled = false;
            lblStatus.Text = "Đang tải dữ liệu...";
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);

            try
            {
                using var client = new HttpClient();
                SessionManager.AddAuthorizationHeader(client);

                using var response = await client.GetAsync(ApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    lblStatus.Text =
                        $"Không tải được sản phẩm (HTTP {(int)response.StatusCode}).";
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Array)
                {
                    lblStatus.Text = "Dữ liệu API không đúng định dạng.";
                    return;
                }

                var rows = new List<ProductRow>();

                foreach (var product in root.EnumerateArray())
                {
                    string categoryName = "";
                    string categoryId = GetValue(product, "categoryId");

                    if (product.TryGetProperty("category", out var category) &&
                        category.ValueKind == JsonValueKind.Object)
                    {
                        categoryName = GetValue(category, "categoryName");

                        if (string.IsNullOrWhiteSpace(categoryId))
                        {
                            categoryId = GetValue(category, "categoryId");
                        }
                    }

                    rows.Add(new ProductRow
                    {
                        ProductId = GetValue(product, "productId"),
                        Barcode = GetValue(product, "barcode"),
                        ProductName = GetValue(product, "productName"),
                        Price = FormatPrice(GetValue(product, "price")),
                        StockQuantity = GetValue(product, "stockQuantity"),
                        CategoryId = categoryId,
                        CategoryName = categoryName
                    });
                }

                grid.DataSource = null;
                grid.DataSource = rows;

                lblStatus.Text = $"Đã tải {rows.Count} sản phẩm.";
                lblStatus.ForeColor = green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Không tải được danh sách sản phẩm.";

                MessageBox.Show(
                    ex.Message,
                    "Lỗi tải dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnReload.Enabled = true;
            }
        }

        // ==================== TẢI DANH MỤC ====================

        private async Task<List<CategoryOption>> LoadCategoriesAsync()
        {
            using var client = new HttpClient();
            SessionManager.AddAuthorizationHeader(client);

            using var response = await client.GetAsync(CategoryApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Không tải được danh mục. HTTP {(int)response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<CategoryOption>>(
                json, options) ?? new List<CategoryOption>();
        }

        // ==================== THÊM SẢN PHẨM ====================

        private async Task AddProductAsync()
        {
            ProductRequest? input =
                await ShowProductDialogAsync("Thêm sản phẩm");

            if (input == null)
                return;

            try
            {
                using var client = new HttpClient();
                SessionManager.AddAuthorizationHeader(client);

                using var response =
                    await client.PostAsJsonAsync(ApiUrl, input);

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Thêm sản phẩm thất bại.\nHTTP {(int)response.StatusCode}\n{error}",
                        "Lỗi API",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Thêm sản phẩm thành công!");
                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi thêm sản phẩm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==================== SỬA SẢN PHẨM ====================

        private async Task EditProductAsync()
        {
            if (grid.CurrentRow?.DataBoundItem is not ProductRow selected)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.");
                return;
            }

            ProductRequest? input =
                await ShowProductDialogAsync("Sửa sản phẩm", selected);

            if (input == null)
                return;

            try
            {
                using var client = new HttpClient();
                SessionManager.AddAuthorizationHeader(client);

                using var response = await client.PutAsJsonAsync(
                    $"{ApiUrl}/{selected.ProductId}", input);

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Sửa sản phẩm thất bại.\nHTTP {(int)response.StatusCode}\n{error}",
                        "Lỗi API",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Cập nhật sản phẩm thành công!");
                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi sửa sản phẩm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==================== XÓA SẢN PHẨM ====================

        private async Task DeleteProductAsync()
        {
            if (grid.CurrentRow?.DataBoundItem is not ProductRow selected)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa sản phẩm \"{selected.ProductName}\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using var client = new HttpClient();
                SessionManager.AddAuthorizationHeader(client);

                using var response = await client.DeleteAsync(
                    $"{ApiUrl}/{selected.ProductId}");

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Xóa sản phẩm thất bại.\nHTTP {(int)response.StatusCode}\n{error}",
                        "Lỗi API",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Xóa sản phẩm thành công!");
                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi xóa sản phẩm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==================== FORM THÊM / SỬA ====================

        private async Task<ProductRequest?> ShowProductDialogAsync(
            string title, ProductRow? existing = null)
        {
            using var form = new Form
            {
                Text = title,
                Size = new Size(440, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                ColumnCount = 2,
                RowCount = 6
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 125));
            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            var barcode = new TextBox { Dock = DockStyle.Fill };
            var productName = new TextBox { Dock = DockStyle.Fill };
            var price = new TextBox { Dock = DockStyle.Fill };
            var stock = new TextBox { Dock = DockStyle.Fill };

            var categoryCombo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(CategoryOption.CategoryName),
                ValueMember = nameof(CategoryOption.CategoryId)
            };

            if (existing != null)
            {
                barcode.Text = existing.Barcode;
                productName.Text = existing.ProductName;
                price.Text = existing.Price;
                stock.Text = existing.StockQuantity;
            }

            string[] labels =
            {
                "Mã vạch:",
                "Tên sản phẩm:",
                "Giá (VNĐ):",
                "Số lượng tồn:",
                "Danh mục:"
            };

            Control[] inputs =
            {
                barcode, productName, price, stock, categoryCombo
            };

            for (int i = 0; i < labels.Length; i++)
            {
                layout.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 43));

                layout.Controls.Add(new Label
                {
                    Text = labels[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                }, 0, i);

                layout.Controls.Add(inputs[i], 1, i);
            }

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };

            var btnSave = new Button
            {
                Text = "Lưu",
                Width = 100,
                Height = 36
            };
            StyleButton(btnSave, blue);

            var btnCancel = new Button
            {
                Text = "Hủy",
                Width = 100,
                Height = 36,
                DialogResult = DialogResult.Cancel
            };
            StyleButton(btnCancel, Color.FromArgb(100, 116, 139));

            buttons.Controls.Add(btnCancel);
            buttons.Controls.Add(btnSave);

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));
            layout.Controls.Add(buttons, 0, 5);
            layout.SetColumnSpan(buttons, 2);

            form.Controls.Add(layout);
            form.CancelButton = btnCancel;

            try
            {
                // Tải danh mục và hiển thị tên để chọn
                List<CategoryOption> categories =
                    await LoadCategoriesAsync();

                categoryCombo.DataSource = categories;

                if (categories.Count == 0)
                {
                    MessageBox.Show(
                        "Chưa có danh mục nào. Hãy thêm danh mục trước.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return null;
                }

                // Khi sửa, chọn lại danh mục hiện tại nếu API có trả CategoryId
                if (existing != null &&
                    int.TryParse(existing.CategoryId, out int oldCategoryId))
                {
                    categoryCombo.SelectedValue = oldCategoryId;
                }

                if (categoryCombo.SelectedIndex < 0)
                    categoryCombo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tải được danh mục.\n" + ex.Message,
                    "Lỗi danh mục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }

            ProductRequest? result = null;

            btnSave.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(productName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sản phẩm.");
                    productName.Focus();
                    return;
                }

                string priceText = price.Text.Trim();

                // Chấp nhận giá nhập theo kiểu 15000 hoặc 15.000
                priceText = priceText.Replace(".", "").Replace(",", "");

                if (!decimal.TryParse(
                    priceText,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal parsedPrice) ||
                    parsedPrice < 0)
                {
                    MessageBox.Show("Giá sản phẩm không hợp lệ.");
                    price.Focus();
                    return;
                }

                if (!int.TryParse(stock.Text.Trim(), out int parsedStock) ||
                    parsedStock < 0)
                {
                    MessageBox.Show(
                        "Số lượng tồn phải là số nguyên không âm.");
                    stock.Focus();
                    return;
                }

                if (categoryCombo.SelectedItem is not CategoryOption category)
                {
                    MessageBox.Show("Vui lòng chọn danh mục.");
                    return;
                }

                result = new ProductRequest
                {
                    Barcode = barcode.Text.Trim(),
                    ProductName = productName.Text.Trim(),
                    Price = parsedPrice,
                    StockQuantity = parsedStock,
                    CategoryId = category.CategoryId
                };

                form.DialogResult = DialogResult.OK;
                form.Close();
            };

            return form.ShowDialog(this) == DialogResult.OK
                ? result
                : null;
        }

        // ==================== HÀM HỖ TRỢ ====================

        private static string GetValue(
            JsonElement element, string property)
        {
            return element.TryGetProperty(property, out var value)
                ? value.ToString()
                : "";
        }

        private static string FormatPrice(string value)
        {
            if (decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal price))
            {
                return price.ToString(
                    "N0",
                    CultureInfo.GetCultureInfo("vi-VN"));
            }

            return value;
        }
    }

    public class ProductRow
    {
        public string ProductId { get; set; } = "";
        public string Barcode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string Price { get; set; } = "";
        public string StockQuantity { get; set; } = "";
        public string CategoryId { get; set; } = "";
        public string CategoryName { get; set; } = "";
    }

    public class CategoryOption
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";

        public override string ToString()
        {
            return CategoryName;
        }
    }

    public class ProductRequest
    {
        public string Barcode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
    }
}
