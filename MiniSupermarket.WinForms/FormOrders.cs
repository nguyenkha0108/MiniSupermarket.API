using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public sealed class FormOrders : Form
    {
        private static readonly HttpClient Client = new()
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/panel/grid)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color accentHover = Color.FromArgb(147, 51, 234);    // Tím đậm khi hover
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color btnGreen = Color.FromArgb(22, 163, 74);        // Xanh lá (Làm mới)

        private readonly TextBox txtSearch = new();
        private readonly DataGridView gridOrders = new();
        private readonly DataGridView gridDetails = new();
        private readonly Label lblStatus = new();
        private readonly Label lblSelectedOrder = new();
        private readonly Button btnSearch = new();
        private readonly Button btnRefresh = new();

        private bool _loadingOrders;
        private int _lastLoadedOrderId = -1;

        public FormOrders()
        {
            Text = "Quản lý đơn hàng";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(950, 600);
            Size = new Size(1200, 750);
            BackColor = darkBackground;
            ForeColor = textLight;
            Padding = new Padding(15);
            Font = new Font("Segoe UI", 9.5F);

            BuildUi();

            Load += async (_, _) => await LoadOrdersAsync();
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = darkBackground,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45));

            // Thanh tìm kiếm
            var searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = darkBackground,
                Margin = new Padding(0)
            };

            var heading = new Label
            {
                Text = "DANH SÁCH HÓA ĐƠN",
                Location = new Point(0, 4),
                Size = new Size(245, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = textLight
            };

            txtSearch.PlaceholderText = "Mã hóa đơn, nhân viên hoặc mã khách hàng";
            txtSearch.SetBounds(250, 8, 330, 34);
            txtSearch.BackColor = inputBackground;
            txtSearch.ForeColor = textLight;
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            btnSearch.Text = "TÌM KIẾM";
            btnSearch.SetBounds(590, 7, 100, 36);
            StyleActionButton(btnSearch, accentColor);
            btnSearch.Click += async (_, _) => await LoadOrdersAsync();

            btnRefresh.Text = "LÀM MỚI";
            btnRefresh.SetBounds(698, 7, 100, 36);
            StyleActionButton(btnRefresh, btnGreen);
            btnRefresh.Click += async (_, _) =>
            {
                txtSearch.Clear();
                await LoadOrdersAsync();
            };

            searchPanel.Controls.AddRange(new Control[]
            {
                heading, txtSearch, btnSearch, btnRefresh
            });

            root.Controls.Add(searchPanel, 0, 0);

            // Danh sách hóa đơn
            ConfigureGrid(gridOrders);

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrderId",
                HeaderText = "ID",
                DataPropertyName = "OrderId",
                Width = 55,
                MinimumWidth = 50
            });

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrderCode",
                HeaderText = "Mã hóa đơn",
                DataPropertyName = "OrderCode",
                Width = 145,
                MinimumWidth = 120
            });

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreatedDate",
                HeaderText = "Ngày bán",
                DataPropertyName = "CreatedDate",
                Width = 145,
                MinimumWidth = 125,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy HH:mm"
                }
            });

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CashierUsername",
                HeaderText = "Nhân viên",
                DataPropertyName = "CashierUsername",
                Width = 110,
                MinimumWidth = 90
            });

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerId",
                HeaderText = "Mã KH",
                DataPropertyName = "CustomerId",
                Width = 75,
                MinimumWidth = 65,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalItems",
                HeaderText = "Tổng SL",
                DataPropertyName = "TotalItems",
                Width = 95,
                MinimumWidth = 95,
                Resizable = DataGridViewTriState.False,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            gridOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FinalAmount",
                HeaderText = "Tổng tiền (VNĐ)",
                DataPropertyName = "FinalAmount",
                MinimumWidth = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            gridOrders.SelectionChanged += async (_, _) =>
            {
                if (_loadingOrders)
                    return;

                await LoadSelectedOrderDetailsAsync();
            };

            root.Controls.Add(gridOrders, 0, 1);

            // Thanh tiêu đề chi tiết hóa đơn
            var detailHeader = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = darkBackground,
                Margin = new Padding(0)
            };

            lblSelectedOrder.Text = "CHI TIẾT HÓA ĐƠN";
            lblSelectedOrder.Dock = DockStyle.Fill;
            lblSelectedOrder.TextAlign = ContentAlignment.MiddleLeft;
            lblSelectedOrder.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSelectedOrder.ForeColor = textLight;
            lblSelectedOrder.AutoEllipsis = true;
            lblSelectedOrder.Padding = new Padding(0, 0, 8, 0);

            lblStatus.Text = "";
            lblStatus.Dock = DockStyle.Right;
            lblStatus.Width = 220;
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            lblStatus.ForeColor = textMuted;
            lblStatus.AutoEllipsis = true;

            detailHeader.Controls.Add(lblSelectedOrder);
            detailHeader.Controls.Add(lblStatus);

            root.Controls.Add(detailHeader, 0, 2);

            // Chi tiết sản phẩm trong hóa đơn
            ConfigureGrid(gridDetails);

            gridDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductId",
                HeaderText = "Mã SP",
                DataPropertyName = "ProductId",
                Width = 85,
                MinimumWidth = 70
            });

            gridDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "ProductName",
                MinimumWidth = 180,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            gridDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Số lượng",
                DataPropertyName = "Quantity",
                Width = 100,
                MinimumWidth = 85,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            gridDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UnitPrice",
                HeaderText = "Đơn giá (VNĐ)",
                DataPropertyName = "UnitPrice",
                Width = 145,
                MinimumWidth = 125,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            gridDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LineTotal",
                HeaderText = "Thành tiền (VNĐ)",
                DataPropertyName = "LineTotal",
                Width = 165,
                MinimumWidth = 145,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            root.Controls.Add(gridDetails, 0, 3);

            Controls.Add(root);
        }

        private void ConfigureGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.Margin = new Padding(0, 2, 0, 2);
            grid.BackgroundColor = cardBackground;
            grid.BorderStyle = BorderStyle.None;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            grid.RowTemplate.Height = 38;
            grid.EnableHeadersVisualStyles = false;

            // Tiêu đề cột Dark Mode
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            grid.ColumnHeadersHeight = 42;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Dòng dữ liệu Dark Mode
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            grid.DefaultCellStyle.BackColor = cardBackground;
            grid.DefaultCellStyle.ForeColor = textLight;
            grid.DefaultCellStyle.SelectionBackColor = accentColor;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 47, 68);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = textLight;
            grid.GridColor = Color.FromArgb(51, 65, 85);
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

        private async Task LoadOrdersAsync()
        {
            if (_loadingOrders)
                return;

            _loadingOrders = true;
            SetBusy(true);

            try
            {
                string search = txtSearch.Text.Trim();
                string url = "checkout/orders";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    url += "?search=" + Uri.EscapeDataString(search);
                }

                var orders =
                    await Client.GetFromJsonAsync<List<OrderRow>>(url)
                    ?? new List<OrderRow>();

                gridOrders.DataSource = null;
                gridDetails.DataSource = null;
                _lastLoadedOrderId = -1;

                gridOrders.DataSource = orders;

                lblStatus.Text = $"Tìm thấy {orders.Count} hóa đơn";

                if (orders.Count == 0)
                {
                    lblSelectedOrder.Text = "CHI TIẾT HÓA ĐƠN";
                    return;
                }

                gridOrders.ClearSelection();

                if (gridOrders.Rows.Count > 0)
                {
                    gridOrders.CurrentCell =
                        gridOrders.Rows[0].Cells[0];
                    gridOrders.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tải được danh sách đơn hàng.\n\n" +
                    "Hãy kiểm tra API, cổng kết nối và SQL Server.\n\n" +
                    "Chi tiết: " + ex.Message,
                    "Lỗi tải đơn hàng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _loadingOrders = false;
                SetBusy(false);
            }

            await LoadSelectedOrderDetailsAsync();
        }

        private async Task LoadSelectedOrderDetailsAsync()
        {
            if (_loadingOrders)
                return;

            if (gridOrders.CurrentRow?.DataBoundItem
                is not OrderRow order)
                return;

            if (_lastLoadedOrderId == order.OrderId)
                return;

            _lastLoadedOrderId = order.OrderId;

            try
            {
                lblSelectedOrder.Text =
                    $"CHI TIẾT: {order.OrderCode} — " +
                    $"Tổng tiền {order.FinalAmount:N0} VNĐ";

                gridDetails.DataSource = null;

                var details =
                    await Client.GetFromJsonAsync<List<OrderDetailRow>>(
                        $"checkout/orders/{order.OrderId}")
                    ?? new List<OrderDetailRow>();

                if (gridOrders.CurrentRow?.DataBoundItem
                    is not OrderRow currentOrder
                    || currentOrder.OrderId != order.OrderId)
                {
                    return;
                }

                gridDetails.DataSource = details;
            }
            catch (Exception ex)
            {
                _lastLoadedOrderId = -1;
                lblStatus.Text = "Không tải được chi tiết hóa đơn";

                MessageBox.Show(
                    "Không tải được chi tiết hóa đơn.\n\n" +
                    "Chi tiết: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void SetBusy(bool busy)
        {
            btnSearch.Enabled = !busy;
            btnRefresh.Enabled = !busy;
            txtSearch.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }
    }

    public sealed class OrderRow
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public string CashierUsername { get; set; } = "";
        public int? CustomerId { get; set; }
        public int TotalItems { get; set; }
        public decimal FinalAmount { get; set; }
        public int RewardPoints { get; set; }
    }

    public sealed class OrderDetailRow
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}