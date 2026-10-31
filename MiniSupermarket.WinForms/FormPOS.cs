
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormPOS : Form
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color background = Color.FromArgb(243, 246, 251);
        private readonly Color green = Color.FromArgb(22, 163, 74);
        private readonly Color red = Color.FromArgb(220, 38, 38);

        private TextBox txtSearch = null!;
        private DataGridView gridProducts = null!;
        private DataGridView gridCart = null!;
        private Label lblTotal = null!;
        private Label lblUser = null!;
        private Label lblProductCount = null!;
        private Button btnSearch = null!;
        private Button btnAdd = null!;
        private Button btnRemove = null!;

        private readonly BindingSource productSource = new();
        private readonly BindingSource cartSource = new();
        private readonly BindingList<CartItem> cart = new();

        public FormPOS()
        {
            InitializeComponent();

            SuspendLayout();
            Controls.Clear();

            BuildPOSUI();

            ResumeLayout(true);
        }

        private void BuildPOSUI()
        {
            Text = "KhaMiniMart | Quầy bán hàng";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(850, 600);
            Size = new Size(1150, 760);
            BackColor = background;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Font;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(14),
                BackColor = background
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

            // HEADER
            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = background,
                Margin = new Padding(0, 0, 0, 8)
            };

            header.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 58));
            header.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 42));

            header.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblTitle = new Label
            {
                Text = "KhaMiniMart",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = blue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblUser = new Label
            {
                Text = $"Nhân viên: {SessionManager.Username} | " +
                       $"Quyền: {SessionManager.Role}",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = navy,
                TextAlign = ContentAlignment.MiddleRight,
                AutoEllipsis = true
            };

            var searchPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 4, 0, 0)
            };

            searchPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            searchPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 145));

            txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = "Quét mã vạch hoặc nhập tên sản phẩm",
                Margin = new Padding(0, 3, 8, 3)
            };

            // Máy quét mã vạch thường hoạt động như bàn phím
            // và gửi Enter sau khi quét.
            txtSearch.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    await SearchProductsAsync();
                }
            };

            btnSearch = CreateButton("TÌM SẢN PHẨM", blue);
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Margin = new Padding(0);
            btnSearch.Click += async (s, e) =>
                await SearchProductsAsync();

            searchPanel.Controls.Add(txtSearch, 0, 0);
            searchPanel.Controls.Add(btnSearch, 1, 0);

            var searchRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = background
            };

            btnAdd = CreateButton("THÊM VÀO GIỎ", green);
            btnAdd.Dock = DockStyle.Right;
            btnAdd.Width = 145;
            btnAdd.Click += (s, e) => AddSelectedProduct();

            searchRight.Controls.Add(btnAdd);

            header.Controls.Add(lblTitle, 0, 0);
            header.Controls.Add(lblUser, 1, 0);
            header.Controls.Add(searchPanel, 0, 1);
            header.Controls.Add(searchRight, 1, 1);

            // DANH SÁCH SẢN PHẨM
            var productPanel = CreateSectionPanel("DANH SÁCH SẢN PHẨM");

            lblProductCount = new Label
            {
                Text = "0 sản phẩm",
                Dock = DockStyle.Right,
                Width = 130,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Gray
            };

            productPanel.Controls[0].Controls.Add(lblProductCount);
            lblProductCount.BringToFront();

            gridProducts = CreateGrid();
            gridProducts.AutoGenerateColumns = false;

            gridProducts.Columns.Add(
                CreateTextColumn("Barcode", "Mã vạch", 150));

            gridProducts.Columns.Add(
                CreateTextColumn("ProductName", "Tên sản phẩm", 250));

            gridProducts.Columns.Add(
                CreateTextColumn("Price", "Đơn giá", 120, "N0"));

            gridProducts.Columns.Add(
                CreateTextColumn("StockQuantity", "Tồn kho", 100));

            productSource.DataSource = new BindingList<ProductDto>();
            gridProducts.DataSource = productSource;

            productPanel.Controls.Add(gridProducts);
            gridProducts.BringToFront();
            productPanel.Controls[0].BringToFront();

            // GIỎ HÀNG
            var cartPanel = CreateSectionPanel("GIỎ HÀNG");

            gridCart = CreateGrid();
            gridCart.AutoGenerateColumns = false;

            gridCart.Columns.Add(
                CreateTextColumn("Barcode", "Mã vạch", 140));

            gridCart.Columns.Add(
                CreateTextColumn("ProductName", "Tên sản phẩm", 250));

            gridCart.Columns.Add(
                CreateTextColumn("Price", "Đơn giá", 110, "N0"));

            gridCart.Columns.Add(
                CreateTextColumn("Quantity", "Số lượng", 90));

            gridCart.Columns.Add(
                CreateTextColumn("Amount", "Thành tiền", 140, "N0"));

            cartSource.DataSource = cart;
            gridCart.DataSource = cartSource;

            cartPanel.Controls.Add(gridCart);
            gridCart.BringToFront();
            cartPanel.Controls[0].BringToFront();

            // FOOTER
            var footer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 8, 0, 0)
            };

            footer.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));
            footer.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            btnRemove = CreateButton("XÓA SẢN PHẨM", red);
            btnRemove.Width = 165;
            btnRemove.Dock = DockStyle.Left;
            btnRemove.Click += (s, e) => RemoveSelectedItem();

            lblTotal = new Label
            {
                Text = "Tổng tiền: 0 VNĐ",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = blue,
                AutoEllipsis = true
            };

            footer.Controls.Add(btnRemove, 0, 0);
            footer.Controls.Add(lblTotal, 1, 0);

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(productPanel, 0, 1);
            layout.Controls.Add(cartPanel, 0, 2);
            layout.Controls.Add(footer, 0, 3);

            Controls.Add(layout);

            AcceptButton = btnSearch;
        }

        private Panel CreateSectionPanel(string title)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10),
                Margin = new Padding(0, 5, 0, 5)
            };

            var heading = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = navy,
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(heading);
            return panel;
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 30 }
            };
        }

        private DataGridViewTextBoxColumn CreateTextColumn(
            string property,
            string title,
            int minimumWidth,
            string? format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = property,
                HeaderText = title,
                MinimumWidth = minimumWidth,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            if (format != null)
                column.DefaultCellStyle.Format = format;

            if (property == "Price" || property == "Amount")
            {
                column.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            return column;
        }

        private Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Margin = new Padding(3),
                Height = 38
            };
        }

        // TÌM SẢN PHẨM BẰNG MÃ VẠCH HOẶC TÊN
        private async Task SearchProductsAsync()
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Vui lòng quét mã vạch hoặc nhập tên sản phẩm.");
                txtSearch.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(SessionManager.Token))
            {
                MessageBox.Show(
                    "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn.");
                return;
            }

            try
            {
                btnSearch.Enabled = false;
                btnSearch.Text = "ĐANG TÌM...";

                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "Products/search?keyword=" +
                    Uri.EscapeDataString(keyword));

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer", SessionManager.Token);

                using HttpResponseMessage response =
                    await _client.SendAsync(request);

                string json =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    string message = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized =>
                            "Token không hợp lệ hoặc đã hết hạn. Hãy đăng nhập lại.",

                        System.Net.HttpStatusCode.Forbidden =>
                            "Tài khoản không có quyền tìm sản phẩm.",

                        _ => $"API trả về lỗi {(int)response.StatusCode}:\n{json}"
                    };

                    MessageBox.Show(
                        message,
                        "Lỗi API",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result =
                    JsonSerializer.Deserialize<List<ProductDto>>(
                        json, options) ?? new List<ProductDto>();

                gridProducts.CurrentCell = null;
                gridProducts.ClearSelection();

                productSource.DataSource =
                    new BindingList<ProductDto>(result);

                productSource.ResetBindings(false);

                lblProductCount.Text = $"{result.Count} sản phẩm";

                // Nếu mã vừa quét khớp chính xác Barcode,
                // tự động thêm sản phẩm vào giỏ.
                ProductDto? barcodeProduct = result.Find(p =>
                    !string.IsNullOrWhiteSpace(p.Barcode) &&
                    string.Equals(
                        p.Barcode.Trim(),
                        keyword,
                        StringComparison.OrdinalIgnoreCase));

                if (barcodeProduct != null)
                {
                    AddProductToCart(barcodeProduct);

                    // Xóa ô tìm kiếm để sẵn sàng quét mã tiếp theo.
                    txtSearch.Clear();
                    txtSearch.Focus();
                    return;
                }

                if (result.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm phù hợp.");
                    txtSearch.SelectAll();
                    txtSearch.Focus();
                }
                else
                {
                    // Tìm theo tên hoặc từ khóa không trùng chính xác
                    // với mã vạch: người dùng chọn sản phẩm để thêm.
                    gridProducts.ClearSelection();
                    txtSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể kết nối API:\n" + ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnSearch.Enabled = true;
                btnSearch.Text = "TÌM SẢN PHẨM";
            }
        }

        // THÊM SẢN PHẨM ĐANG ĐƯỢC CHỌN TRÊN BẢNG
        private void AddSelectedProduct()
        {
            if (gridProducts.CurrentRow?.DataBoundItem
                is not ProductDto product)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm trong danh sách.");
                return;
            }

            AddProductToCart(product);
        }

        // DÙNG CHUNG CHO QUÉT MÃ VẠCH VÀ BẤM THÊM VÀO GIỎ
        private void AddProductToCart(ProductDto product)
        {
            if (product.StockQuantity <= 0)
            {
                MessageBox.Show("Sản phẩm đã hết hàng.");
                return;
            }

            CartItem? existing = null;

            foreach (CartItem item in cart)
            {
                if (item.ProductId == product.ProductId)
                {
                    existing = item;
                    break;
                }
            }

            if (existing != null)
            {
                if (existing.Quantity >= product.StockQuantity)
                {
                    MessageBox.Show("Số lượng vượt quá tồn kho.");
                    return;
                }

                existing.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    Barcode = product.Barcode,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Quantity = 1
                });
            }

            cartSource.ResetBindings(false);
            UpdateTotal();
        }

        private void RemoveSelectedItem()
        {
            if (gridCart.CurrentRow?.DataBoundItem is not CartItem item)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm trong giỏ hàng.");
                return;
            }

            cart.Remove(item);
            cartSource.ResetBindings(false);
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = 0;

            foreach (CartItem item in cart)
            {
                total += item.Price * item.Quantity;
            }

            lblTotal.Text = "Tổng tiền: " +
                total.ToString(
                    "N0",
                    CultureInfo.GetCultureInfo("vi-VN")) +
                " VNĐ";
        }

        public class ProductDto
        {
            public int ProductId { get; set; }
            public string Barcode { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public int CategoryId { get; set; }
        }

        public class CartItem
        {
            public int ProductId { get; set; }
            public string Barcode { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public int Quantity { get; set; }

            public decimal Amount => Price * Quantity;
        }
    }
}
