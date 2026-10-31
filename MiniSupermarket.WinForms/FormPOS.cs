using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
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

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/panel)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color accentHover = Color.FromArgb(147, 51, 234);    // Tím đậm khi hover
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color btnGreen = Color.FromArgb(22, 163, 74);        // Xanh lá (Thêm/Thanh toán)
        private readonly Color btnRed = Color.FromArgb(220, 38, 38);          // Đỏ (Xóa)

        private TextBox txtSearch = null!;
        private DataGridView gridProducts = null!;
        private DataGridView gridCart = null!;
        private Label lblTotal = null!;
        private Label lblUser = null!;
        private Label lblProductCount = null!;
        private Button btnSearch = null!;
        private Button btnAdd = null!;
        private Button btnRemove = null!;
        private TextBox txtCustomerId = null!;
        private Button btnCheckout = null!;
        private TextBox txtRedeemPoints = null!;
        private TextBox txtCashReceived = null!;
        private Label lblChange = null!;
        private int? _customerPoints;

        private const decimal PointValue = 1000m; // 1 điểm thưởng = 1.000 VNĐ
        private const string RedeemPlaceholder = "Điểm dùng (1 điểm = 1.000đ)";

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
            MinimumSize = new Size(1000, 650);
            Size = new Size(1150, 760);
            BackColor = darkBackground;
            ForeColor = textLight;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Font;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(15),
                BackColor = darkBackground
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));

            // HEADER
            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = darkBackground,
                Margin = new Padding(0, 0, 0, 8)
            };

            header.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 58));
            header.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 42));

            header.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblTitle = new Label
            {
                Text = "⚡ KHA MINI MART — POS",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblUser = new Label
            {
                Text = $"Nhân viên: {SessionManager.Username}    |    Quyền: {SessionManager.Role}",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = textMuted,
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
                new ColumnStyle(SizeType.Absolute, 155));

            txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = "Quét mã vạch hoặc nhập tên sản phẩm",
                Margin = new Padding(0, 3, 8, 3),
                BackColor = inputBackground,
                ForeColor = textLight
            };

            txtSearch.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    await SearchProductsAsync();
                }
            };

            btnSearch = CreateButton("TÌM SẢN PHẨM", accentColor);
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Margin = new Padding(0);
            btnSearch.Click += async (s, e) =>
                await SearchProductsAsync();

            searchPanel.Controls.Add(txtSearch, 0, 0);
            searchPanel.Controls.Add(btnSearch, 1, 0);

            var searchRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = darkBackground
            };

            btnAdd = CreateButton("THÊM VÀO GIỎ", btnGreen);
            btnAdd.Dock = DockStyle.Right;
            btnAdd.Width = 155;
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
                Width = 140,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
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

            // FOOTER: chia 4 khu vực riêng
            var footer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 6, 0, 0),
                Padding = new Padding(0),
                BackColor = darkBackground,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            footer.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 20F));

            footer.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 26F));

            footer.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 30F));

            footer.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 24F));

            footer.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            // 1. NÚT XÓA SẢN PHẨM
            btnRemove = CreateButton("XÓA SẢN PHẨM", btnRed);
            btnRemove.Dock = DockStyle.Fill;
            btnRemove.Margin = new Padding(0, 5, 8, 5);
            btnRemove.MinimumSize = new Size(0, 42);
            btnRemove.Font = new Font(
                "Segoe UI", 9.5F, FontStyle.Bold);

            btnRemove.Click += (s, e) => RemoveSelectedItem();

            // 2. MÃ KHÁCH HÀNG + ĐIỂM ĐỔI (chiều cao cố định để không bị bóp)
            var customerPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Margin = new Padding(4, 0, 8, 0),
                Padding = new Padding(0),
                BackColor = darkBackground
            };

            customerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            customerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            customerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            customerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            var lblCustomer = new Label
            {
                Text = "Mã khách hàng",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(2, 0, 0, 0)
            };

            txtCustomerId = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Nhập mã KH",
                Font = new Font("Segoe UI", 10F),
                BackColor = inputBackground,
                ForeColor = textLight,
                Margin = new Padding(0)
            };

            // Khi rời ô mã KH: tải số điểm hiện có để hiển thị gợi ý.
            txtCustomerId.Leave += async (s, e) => await LoadCustomerPointsAsync();

            var lblRedeem = new Label
            {
                Text = "Điểm đổi (1 điểm = 1.000đ)",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(2, 0, 0, 0)
            };

            txtRedeemPoints = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = RedeemPlaceholder,
                Font = new Font("Segoe UI", 10F),
                BackColor = inputBackground,
                ForeColor = textLight,
                Margin = new Padding(0)
            };
            txtRedeemPoints.TextChanged += (s, e) => UpdateTotal();

            customerPanel.Controls.Add(lblCustomer, 0, 0);
            customerPanel.Controls.Add(txtCustomerId, 0, 1);
            customerPanel.Controls.Add(lblRedeem, 0, 2);
            customerPanel.Controls.Add(txtRedeemPoints, 0, 3);

            // 3. TỔNG TIỀN
            var totalPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 5, 8, 5),
                BackColor = cardBackground,
                Padding = new Padding(10, 0, 8, 0)
            };

            lblTotal = new Label
            {
                Text = "Tổng tiền: 0 VNĐ",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = textLight,
                AutoEllipsis = true,
                UseMnemonic = false,
                Margin = new Padding(0)
            };

            totalPanel.Controls.Add(lblTotal);

            txtCashReceived = new TextBox
            {
                Dock = DockStyle.Bottom,
                PlaceholderText = "Tiền khách đưa (VNĐ)",
                Font = new Font("Segoe UI", 10F),
                BackColor = inputBackground,
                ForeColor = textLight,
                Height = 28
            };
            txtCashReceived.TextChanged += (s, e) => UpdateTotal();

            lblChange = new Label
            {
                Text = "Tiền thối: 0 VNĐ",
                Dock = DockStyle.Bottom,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(134, 239, 172)
            };

            totalPanel.Controls.Add(txtCashReceived);
            totalPanel.Controls.Add(lblChange);

            // 4. NÚT THANH TOÁN
            btnCheckout = CreateButton("THANH TOÁN", btnGreen);
            btnCheckout.Dock = DockStyle.Fill;
            btnCheckout.Margin = new Padding(0, 5, 0, 5);
            btnCheckout.MinimumSize = new Size(0, 42);
            btnCheckout.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            btnCheckout.Click += async (s, e) =>
                await CheckoutAsync();

            footer.Controls.Add(btnRemove, 0, 0);
            footer.Controls.Add(customerPanel, 1, 0);
            footer.Controls.Add(totalPanel, 2, 0);
            footer.Controls.Add(btnCheckout, 3, 0);

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(productPanel, 0, 1);
            layout.Controls.Add(cartPanel, 0, 2);
            layout.Controls.Add(footer, 0, 3);

            Controls.Add(layout);

        }

        private Panel CreateSectionPanel(string title)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = cardBackground,
                Padding = new Padding(12),
                Margin = new Padding(0, 5, 0, 5)
            };

            var heading = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = textLight,
                BackColor = cardBackground,
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
                BackgroundColor = cardBackground,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 38 },
                GridColor = Color.FromArgb(51, 65, 85),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(15, 23, 42),
                    ForeColor = textLight,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = cardBackground,
                    ForeColor = textLight,
                    SelectionBackColor = accentColor,
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5F)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(35, 47, 68),
                    ForeColor = textLight
                }
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
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Margin = new Padding(3),
                Height = 40,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = ControlPaint.Light(color, 0.15F) }
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

                ProductDto? barcodeProduct = result.Find(p =>
                    !string.IsNullOrWhiteSpace(p.Barcode) &&
                    string.Equals(
                        p.Barcode.Trim(),
                        keyword,
                        StringComparison.OrdinalIgnoreCase));

                if (barcodeProduct != null)
                {
                    AddProductToCart(barcodeProduct);

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

        private decimal CalculateGrossTotal()
        {
            return cart.Sum(item => item.Price * item.Quantity);
        }

        // Chỉ giữ chữ số (người dùng có thể gõ dấu chấm/phẩy phân cách hàng nghìn).
        private static string DigitsOnly(string? text)
        {
            return new string((text ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        private static int ParseDigitsAsInt(string? text)
        {
            string digits = DigitsOnly(text);
            if (digits.Length == 0) return 0;
            return int.TryParse(digits, out int value) ? value : int.MaxValue; // quá lớn -> sẽ bị từ chối khi kiểm tra
        }

        private static decimal ParseDigitsAsDecimal(string? text)
        {
            string digits = DigitsOnly(text);
            return decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out decimal value) ? value : 0m;
        }

        private static string Money(decimal value)
        {
            return value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }

        private void UpdateTotal()
        {
            if (lblTotal == null || lblChange == null || txtRedeemPoints == null || txtCashReceived == null)
                return;

            decimal gross = CalculateGrossTotal();
            int redeemPoints = ParseDigitsAsInt(txtRedeemPoints.Text);
            decimal discount = Math.Min(redeemPoints * PointValue, gross);
            decimal payable = gross - discount;

            string totalText = "Tổng tiền: " + Money(gross) + " VNĐ";
            if (discount > 0)
            {
                totalText += "\nĐổi " + redeemPoints + " điểm: -" + Money(discount) + " VNĐ";
                totalText += "\nKhách phải trả: " + Money(payable) + " VNĐ";
            }
            lblTotal.Text = totalText;

            decimal cash = ParseDigitsAsDecimal(txtCashReceived.Text);
            if (cash <= 0)
            {
                lblChange.Text = "Tiền thối: nhập tiền khách đưa";
                lblChange.ForeColor = Color.FromArgb(148, 163, 184);
            }
            else if (cash < payable)
            {
                lblChange.Text = "Chưa đủ tiền, còn thiếu " + Money(payable - cash) + " VNĐ";
                lblChange.ForeColor = Color.FromArgb(252, 165, 165);
            }
            else
            {
                lblChange.Text = "Tiền thối: " + Money(cash - payable) + " VNĐ";
                lblChange.ForeColor = Color.FromArgb(134, 239, 172);
            }
        }

        // Tải số điểm của khách hàng để gợi ý và kiểm tra trước khi thanh toán.
        private async Task LoadCustomerPointsAsync()
        {
            _customerPoints = null;
            txtRedeemPoints.PlaceholderText = RedeemPlaceholder;

            if (!int.TryParse(txtCustomerId.Text.Trim(), out int customerId) || customerId <= 0)
                return;
            if (string.IsNullOrWhiteSpace(SessionManager.Token))
                return;

            try
            {
                var customer = await ApiClientService.GetAsync<CustomerLookupDto>($"customers/{customerId}");
                if (customer == null) return;

                _customerPoints = customer.RewardPoints;
                txtRedeemPoints.PlaceholderText = $"Điểm dùng (đang có {customer.RewardPoints} điểm)";
            }
            catch (ApiException)
            {
                // Không chặn thao tác: API sẽ kiểm tra lại khi thanh toán.
                _customerPoints = null;
            }
        }

        private async Task CheckoutAsync()
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thanh toán",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(txtCustomerId.Text.Trim(), out int customerId) || customerId <= 0)
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng hợp lệ.", "Thiếu mã khách hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(SessionManager.Token))
            {
                MessageBox.Show("Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SessionManager.Username))
            {
                MessageBox.Show("Không xác định được nhân viên đang đăng nhập.");
                return;
            }

            // Kiểm tra đổi điểm và tiền khách đưa trước khi gửi lên API.
            int redeemPoints = ParseDigitsAsInt(txtRedeemPoints.Text);
            decimal gross = CalculateGrossTotal();
            decimal discount = redeemPoints * PointValue;

            if (_customerPoints.HasValue && redeemPoints > _customerPoints.Value)
            {
                MessageBox.Show($"Khách hàng chỉ có {_customerPoints.Value} điểm, không đủ để đổi {redeemPoints} điểm.",
                    "Không đủ điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRedeemPoints.Focus();
                return;
            }

            if (discount > gross)
            {
                MessageBox.Show("Số điểm đổi vượt quá giá trị hóa đơn.", "Đổi điểm",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRedeemPoints.Focus();
                return;
            }

            decimal payable = gross - discount;
            decimal cash = ParseDigitsAsDecimal(txtCashReceived.Text);
            if (cash > 0 && cash < payable)
            {
                MessageBox.Show("Tiền khách đưa chưa đủ. Còn thiếu " + Money(payable - cash) + " VNĐ.",
                    "Chưa đủ tiền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCashReceived.Focus();
                return;
            }

            var cartSnapshot = cart.Select(item => new
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            }).ToList();

            var payload = new
            {
                CashierUsername = SessionManager.Username.Trim(),
                CustomerId = customerId,
                RedeemPoints = redeemPoints,
                Items = cartSnapshot
            };

            try
            {
                btnCheckout.Enabled = false;
                btnCheckout.Text = "ĐANG XỬ LÝ...";

                ApiRawResponse raw = await ApiClientService.PostRawAsync("checkout", payload);
                string responseJson = raw.Body;

                bool success = false;
                string message = responseJson;
                string orderCode = string.Empty;
                decimal finalAmount = 0m;
                decimal discountApplied = 0m;
                int rewardPoints = 0;

                try
                {
                    using JsonDocument document = JsonDocument.Parse(responseJson);
                    JsonElement root = document.RootElement;
                    if (root.TryGetProperty("success", out JsonElement successElement))
                        success = successElement.ValueKind == JsonValueKind.True;
                    if (root.TryGetProperty("message", out JsonElement messageElement))
                        message = messageElement.GetString() ?? message;
                    if (root.TryGetProperty("orderCode", out JsonElement codeElement))
                        orderCode = codeElement.GetString() ?? string.Empty;
                    if (root.TryGetProperty("finalAmount", out JsonElement amountElement) && amountElement.TryGetDecimal(out decimal amount))
                        finalAmount = amount;
                    if (root.TryGetProperty("discountAmount", out JsonElement discountElement) && discountElement.TryGetDecimal(out decimal discountValue))
                        discountApplied = discountValue;
                    if (root.TryGetProperty("rewardPointsAdded", out JsonElement pointsElement) && pointsElement.TryGetInt32(out int points))
                        rewardPoints = points;
                    if (root.TryGetProperty("errorDetail", out JsonElement errorElement) && !string.IsNullOrWhiteSpace(errorElement.GetString()))
                        message += "\nChi tiết: " + errorElement.GetString();
                }
                catch (JsonException)
                {
                    // Giữ nguyên nội dung nếu phản hồi không phải JSON.
                }

                if (!raw.IsSuccess || !success)
                {
                    if (raw.StatusCode == HttpStatusCode.Unauthorized)
                        message = "Phiên đăng nhập hết hạn hoặc token không hợp lệ. Hãy đăng nhập lại.\n" + message;
                    else if (raw.StatusCode == HttpStatusCode.Forbidden)
                        message = "Tài khoản không có quyền thanh toán.\n" + message;

                    MessageBox.Show(message, "Thanh toán thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var summary = new StringBuilder();
                summary.AppendLine(message);
                summary.AppendLine("Mã hóa đơn: " + orderCode);
                summary.AppendLine("Tổng tiền hàng: " + Money(gross) + " VNĐ");
                if (discountApplied > 0)
                    summary.AppendLine("Đổi " + redeemPoints + " điểm: -" + Money(discountApplied) + " VNĐ");
                summary.AppendLine("Khách phải trả: " + Money(finalAmount) + " VNĐ");
                if (cash > 0)
                {
                    summary.AppendLine("Khách đưa: " + Money(cash) + " VNĐ");
                    summary.AppendLine("Tiền thối: " + Money(cash - finalAmount) + " VNĐ");
                }
                summary.Append("Điểm thưởng cộng: " + rewardPoints);

                MessageBox.Show(summary.ToString(), "Thanh toán thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form cho khách tiếp theo.
                cart.Clear();
                cartSource.ResetBindings(false);
                txtCustomerId.Clear();
                txtRedeemPoints.Clear();
                txtCashReceived.Clear();
                _customerPoints = null;
                txtRedeemPoints.PlaceholderText = RedeemPlaceholder;
                UpdateTotal();

                await RefreshProductsAsync();
                txtSearch.Focus();
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kết nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể hoàn tất thanh toán:\n" + ex.Message, "Lỗi thanh toán",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckout.Enabled = true;
                btnCheckout.Text = "THANH TOÁN";
            }
        }

        private async Task RefreshProductsAsync()
        {
            if (string.IsNullOrWhiteSpace(SessionManager.Token)) return;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "Products");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
                using HttpResponseMessage response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return;

                string json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<ProductDto>>(json, options) ?? new List<ProductDto>();
                productSource.DataSource = new BindingList<ProductDto>(result);
                productSource.ResetBindings(false);
                lblProductCount.Text = $"{result.Count} sản phẩm";
                gridProducts.ClearSelection();
            }
            catch
            {
                // Bỏ qua lỗi làm mới khi thanh toán thành công
            }
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