using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MiniSupermarket.WinForms
{
    public partial class FormQuickReport : Form
    {
        private const int MaxReportDays = 366;

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/panel)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)
        private readonly Color warningColor = Color.FromArgb(251, 146, 60);   // Cam (cảnh báo)

        private readonly DateTimePicker dtpFrom = new();
        private readonly DateTimePicker dtpTo = new();
        private readonly Label lblTodayRevenue = new();
        private readonly Label lblTotalInvoices = new();
        private readonly Label lblNewCustomers = new();
        private readonly Label lblLowStock = new();
        private readonly Chart chartRevenue = new();
        private readonly DataGridView gridTop5 = new();
        private readonly Label lblStatus = new();

        private Button btnLoad = new();
        private Button btnExport = new();
        private List<DailyRow> _dailyRows = new();

        private sealed record DailyRow(DateTime Date, int TotalOrders, decimal TotalRevenue);

        public FormQuickReport()
        {
            InitializeComponent();
            BuildReportUI();
            Shown += async (_, _) => await LoadAllAsync();
        }

        private void BuildReportUI()
        {
            Controls.Clear();

            BackColor = darkBackground;
            Padding = new Padding(18);
            Font = new Font("Segoe UI", 10F);

            var title = new Label
            {
                Text = "BÁO CÁO & THỐNG KÊ",
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 45,
                ForeColor = textLight,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var subtitle = new Label
            {
                Text = "Theo dõi tình hình kinh doanh của KhaMiniMart",
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = textMuted,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ----- Thanh chọn khoảng ngày -----
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = cardBackground,
                Padding = new Padding(15)
            };

            var lblFrom = new Label { Text = "Từ ngày:", AutoSize = true, Location = new Point(15, 25), ForeColor = textMuted };
            ConfigureDatePicker(dtpFrom, new Point(85, 20));
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var lblTo = new Label { Text = "Đến ngày:", AutoSize = true, Location = new Point(230, 25), ForeColor = textMuted };
            ConfigureDatePicker(dtpTo, new Point(315, 20));
            dtpTo.Value = DateTime.Today;

            btnLoad = new Button { Text = "Xem báo cáo", Width = 150, Height = 36, Location = new Point(460, 18) };
            StyleButton(btnLoad, accentColor);
            btnLoad.Click += async (_, _) => await LoadAllAsync();

            btnExport = new Button { Text = "Xuất CSV", Width = 150, Height = 36, Location = new Point(625, 18) };
            StyleButton(btnExport, Color.FromArgb(14, 116, 144)); // Xanh ngọc
            btnExport.Click += btnExport_Click;

            toolbar.Controls.Add(lblFrom);
            toolbar.Controls.Add(dtpFrom);
            toolbar.Controls.Add(lblTo);
            toolbar.Controls.Add(dtpTo);
            toolbar.Controls.Add(btnLoad);
            toolbar.Controls.Add(btnExport);

            // ----- Bốn thẻ KPI -----
            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0, 15, 0, 0),
                BackColor = darkBackground
            };
            for (int i = 0; i < 4; i++)
                cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            cards.Controls.Add(CreateCard("DOANH THU HÔM NAY (VNĐ)", lblTodayRevenue), 0, 0);
            cards.Controls.Add(CreateCard("TỔNG SỐ HÓA ĐƠN", lblTotalInvoices), 1, 0);
            cards.Controls.Add(CreateCard("KHÁCH HÀNG MỚI (30 NGÀY)", lblNewCustomers), 2, 0);
            cards.Controls.Add(CreateCard("SẮP HẾT HÀNG (TỒN ≤ 10)", lblLowStock), 3, 0);

            // ----- Vùng nội dung: biểu đồ (trái) + Top 5 (phải) -----
            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = darkBackground
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            var chartPanel = new Panel { Dock = DockStyle.Fill, BackColor = cardBackground, Padding = new Padding(8), Margin = new Padding(0, 0, 12, 0) };
            BuildChart();
            chartPanel.Controls.Add(chartRevenue);

            var topPanel = new Panel { Dock = DockStyle.Fill, BackColor = cardBackground, Padding = new Padding(8) };
            BuildTop5Grid();
            var topHeading = new Label
            {
                Text = "TOP 5 SẢN PHẨM BÁN CHẠY",
                Dock = DockStyle.Top,
                Height = 36,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = textLight,
                TextAlign = ContentAlignment.MiddleLeft
            };
            topPanel.Controls.Add(gridTop5);
            topPanel.Controls.Add(topHeading);

            body.Controls.Add(chartPanel, 0, 0);
            body.Controls.Add(topPanel, 1, 0);

            // ----- Thanh trạng thái -----
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Height = 30;
            lblStatus.ForeColor = textMuted;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Text = "Đang tải dữ liệu...";

            Controls.Add(body);
            Controls.Add(lblStatus);
            Controls.Add(cards);
            Controls.Add(toolbar);
            Controls.Add(subtitle);
            Controls.Add(title);
        }

        private void ConfigureDatePicker(DateTimePicker picker, Point location)
        {
            picker.Format = DateTimePickerFormat.Short;
            picker.Width = 130;
            picker.Location = location;
            picker.Font = new Font("Segoe UI", 10F);
            picker.CalendarForeColor = textLight;
            picker.CalendarMonthBackground = inputBackground;
        }

        private Panel CreateCard(string heading, Label valueLabel)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = cardBackground,
                Margin = new Padding(0, 0, 12, 0),
                Padding = new Padding(16)
            };

            var accent = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accentColor };

            var headingLabel = new Label
            {
                Text = heading,
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textMuted
            };

            valueLabel.Text = "—";
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            valueLabel.ForeColor = textLight;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            valueLabel.AutoEllipsis = true;

            card.Controls.Add(valueLabel);
            card.Controls.Add(headingLabel);
            card.Controls.Add(accent);
            return card;
        }

        private void BuildChart()
        {
            chartRevenue.Dock = DockStyle.Fill;
            chartRevenue.BackColor = cardBackground;
            chartRevenue.Legends.Clear();

            chartRevenue.Titles.Add(new Title
            {
                Text = "DOANH THU THEO NGÀY (VNĐ)",
                ForeColor = textLight,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Docking = Docking.Top
            });

            var area = new ChartArea("RevenueArea") { BackColor = cardBackground };
            area.AxisX.LabelStyle.ForeColor = textMuted;
            area.AxisY.LabelStyle.ForeColor = textMuted;
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisX.MajorGrid.LineColor = inputBackground;
            area.AxisY.MajorGrid.LineColor = inputBackground;
            area.AxisX.LineColor = inputBackground;
            area.AxisY.LineColor = inputBackground;
            chartRevenue.ChartAreas.Add(area);

            var series = new Series("Doanh thu")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                ChartArea = area.Name,
                Color = accentColor,
                ToolTip = "#VALX: #VALY{N0} VNĐ"
            };
            chartRevenue.Series.Add(series);
        }

        private void BuildTop5Grid()
        {
            gridTop5.Dock = DockStyle.Fill;
            gridTop5.ReadOnly = true;
            gridTop5.AllowUserToAddRows = false;
            gridTop5.AllowUserToDeleteRows = false;
            gridTop5.AllowUserToResizeRows = false;
            gridTop5.RowHeadersVisible = false;
            gridTop5.BorderStyle = BorderStyle.None;
            gridTop5.BackgroundColor = cardBackground;
            gridTop5.EnableHeadersVisualStyles = false;
            gridTop5.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridTop5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            gridTop5.ColumnHeadersDefaultCellStyle.BackColor = darkBackground;
            gridTop5.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            gridTop5.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gridTop5.DefaultCellStyle.BackColor = cardBackground;
            gridTop5.DefaultCellStyle.ForeColor = textLight;
            gridTop5.DefaultCellStyle.SelectionBackColor = accentColor;
            gridTop5.DefaultCellStyle.SelectionForeColor = Color.White;
            gridTop5.AlternatingRowsDefaultCellStyle.BackColor = inputBackground;

            gridTop5.Columns.Add(new DataGridViewTextBoxColumn { Name = "Barcode", HeaderText = "Mã vạch", FillWeight = 25 });
            gridTop5.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Tên sản phẩm", FillWeight = 40 });
            gridTop5.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Đã bán", FillWeight = 15 });
            gridTop5.Columns.Add(new DataGridViewTextBoxColumn { Name = "Revenue", HeaderText = "Doanh thu", FillWeight = 30 });
        }

        private static void StyleButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.15F);
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private static string Money(decimal value)
        {
            return value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }

        private void SetBusy(bool busy)
        {
            btnLoad.Enabled = !busy;
            btnExport.Enabled = !busy;
            btnLoad.Text = busy ? "Đang tải..." : "Xem báo cáo";
        }

        private async Task LoadAllAsync()
        {
            if (dtpFrom.Value.Date > dtpTo.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.", "Khoảng ngày không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int days = (int)(dtpTo.Value.Date - dtpFrom.Value.Date).TotalDays + 1;
            if (days > MaxReportDays)
            {
                MessageBox.Show($"Khoảng ngày tối đa là {MaxReportDays} ngày.", "Khoảng ngày quá dài",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string from = dtpFrom.Value.Date.ToString("yyyy-MM-dd");
            string to = dtpTo.Value.Date.ToString("yyyy-MM-dd");

            SetBusy(true);
            try
            {
                var dashboardTask = ApiClientService.GetAsync<DashboardSummaryDto>("Reports/dashboard-summary");
                var dailyTask = ApiClientService.GetAsync<DailyRevenueResponse>($"Reports/daily-revenue?fromDate={from}&toDate={to}");
                var topTask = ApiClientService.GetAsync<Top5Response>("Reports/top5-selling-products");

                await Task.WhenAll(dashboardTask, dailyTask, topTask);

                ShowDashboard(dashboardTask.Result);
                ShowDailyRevenue(dailyTask.Result, dtpFrom.Value.Date, dtpTo.Value.Date);
                ShowTop5(topTask.Result);

                lblStatus.Text = $"Dữ liệu từ {dtpFrom.Value:dd/MM/yyyy} đến {dtpTo.Value:dd/MM/yyyy}. Cập nhật lúc {DateTime.Now:HH:mm:ss}.";
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ShowDashboard(new DashboardSummaryDto { TodayRevenue = 0, TotalInvoices = 0, NewCustomers30Days = 0, LowStockProducts = 0 });
                    ShowDailyRevenue(new DailyRevenueResponse { Data = new List<DailyRevenueItem>() }, dtpFrom.Value.Date, dtpTo.Value.Date);
                    ShowTop5(new Top5Response { Data = new List<TopProductItem>() });

                    lblStatus.Text = $"Không có dữ liệu phát sinh từ {dtpFrom.Value:dd/MM/yyyy} đến {dtpTo.Value:dd/MM/yyyy}.";
                }
                else
                {
                    lblStatus.Text = "Không tải được dữ liệu báo cáo.";
                    MessageBox.Show(ex.Message, "Lỗi tải báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Không tải được dữ liệu báo cáo.";
                MessageBox.Show("Không thể tải báo cáo:\n\n" + ex.Message, "Lỗi báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ShowDashboard(DashboardSummaryDto? data)
        {
            if (data == null)
            {
                lblTodayRevenue.Text = "0";
                lblTotalInvoices.Text = "0";
                lblNewCustomers.Text = "0";
                lblLowStock.Text = "0";
                return;
            }

            lblTodayRevenue.Text = Money(data.TodayRevenue);
            lblTotalInvoices.Text = data.TotalInvoices.ToString("N0", CultureInfo.InvariantCulture);
            lblNewCustomers.Text = data.NewCustomers30Days.ToString("N0", CultureInfo.InvariantCulture);
            lblLowStock.Text = data.LowStockProducts.ToString("N0", CultureInfo.InvariantCulture);
            lblLowStock.ForeColor = data.LowStockProducts > 0 ? warningColor : textLight;
        }

        private void ShowDailyRevenue(DailyRevenueResponse? response, DateTime from, DateTime to)
        {
            var byDate = (response?.Data ?? new List<DailyRevenueItem>())
                .ToDictionary(item => item.Date, item => item);

            _dailyRows = new List<DailyRow>();
            for (DateTime day = from; day <= to; day = day.AddDays(1))
            {
                string key = day.ToString("yyyy-MM-dd");
                if (byDate.TryGetValue(key, out var found))
                    _dailyRows.Add(new DailyRow(day, found.TotalOrders, found.TotalRevenue));
                else
                    _dailyRows.Add(new DailyRow(day, 0, 0m));
            }

            Series series = chartRevenue.Series[0];
            series.Points.Clear();
            foreach (DailyRow row in _dailyRows)
            {
                series.Points.AddXY(row.Date.ToString("dd/MM"), (double)row.TotalRevenue);
            }
            chartRevenue.Invalidate();
        }

        private void ShowTop5(Top5Response? response)
        {
            gridTop5.Rows.Clear();
            foreach (TopProductItem item in response?.Data ?? new List<TopProductItem>())
            {
                gridTop5.Rows.Add(
                    item.Barcode,
                    item.ProductName,
                    item.TotalQuantitySold.ToString("N0", CultureInfo.InvariantCulture),
                    Money(item.TotalRevenue));
            }
        }

        private void btnExport_Click(object? sender, EventArgs e)
        {
            if (_dailyRows.Count == 0)
            {
                MessageBox.Show("Hãy xem báo cáo trước khi xuất.", "Xuất CSV",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var rows = new List<IEnumerable<string>>
            {
                new[] { "Ngày", "Số hóa đơn", "Doanh thu (VNĐ)" }
            };

            rows.AddRange(_dailyRows.Select(row => (IEnumerable<string>)new[]
            {
                row.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                row.TotalOrders.ToString(CultureInfo.InvariantCulture),
                row.TotalRevenue.ToString("0", CultureInfo.InvariantCulture)
            }));

            string fileName = $"doanh-thu_{dtpFrom.Value:yyyyMMdd}_{dtpTo.Value:yyyyMMdd}.csv";
            if (CsvExport.SaveWithDialog(this, fileName, rows))
            {
                lblStatus.Text = $"Đã xuất {_dailyRows.Count} ngày doanh thu ra file CSV.";
            }
        }
    }
}