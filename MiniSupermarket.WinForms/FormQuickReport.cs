using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormQuickReport : Form
    {
        private static readonly HttpClient ApiClient = new()
        {
            BaseAddress = new Uri("https://localhost:7044/api/")
        };

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI HỆ THỐNG
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800 (Nền khung/panel)
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet (Điểm nhấn chính)
        private readonly Color inputBackground = Color.FromArgb(51, 65, 85);  // Slate 700 (Ô nhập liệu)
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Chữ sáng (Slate 50)
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Chữ phụ (Slate 400)

        private readonly DateTimePicker dtpReportDate = new();
        private readonly Label lblTotalOrders = new();
        private readonly Label lblTotalRevenue = new();
        private readonly Label lblBestSeller = new();

        private Button? btnView;
        private Label? lblStatus;

        private sealed class QuickReportDto
        {
            public bool Success { get; set; }
            public string Date { get; set; } = "";
            public int TotalOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public string BestSeller { get; set; } = "Chưa có dữ liệu";
        }

        public FormQuickReport()
        {
            InitializeComponent();
            BuildReportUI();
        }

        private void BuildReportUI()
        {
            Controls.Clear();

            BackColor = darkBackground;
            Padding = new Padding(18);
            Font = new Font("Segoe UI", 10F);

            var title = new Label
            {
                Text = "BÁO CÁO DOANH THU",
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

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = cardBackground,
                Padding = new Padding(15)
            };

            var dateLabel = new Label
            {
                Text = "Ngày báo cáo:",
                AutoSize = true,
                Location = new Point(15, 25),
                ForeColor = textMuted
            };

            dtpReportDate.Format = DateTimePickerFormat.Short;
            dtpReportDate.Width = 150;
            dtpReportDate.Location = new Point(125, 20);
            dtpReportDate.Font = new Font("Segoe UI", 10F);
            dtpReportDate.CalendarForeColor = textLight;
            dtpReportDate.CalendarMonthBackground = inputBackground;

            btnView = new Button
            {
                Text = "Xem báo cáo",
                Width = 140,
                Height = 36,
                Location = new Point(295, 18)
            };

            StyleButton(btnView, accentColor);

            btnView.Click += async (_, _) =>
            {
                await LoadReportSafelyAsync();
            };

            toolbar.Controls.Add(dateLabel);
            toolbar.Controls.Add(dtpReportDate);
            toolbar.Controls.Add(btnView);

            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 175,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0, 15, 0, 15),
                BackColor = darkBackground
            };

            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));
            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));
            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.34F));

            cards.Controls.Add(
                CreateCard("TỔNG SỐ HÓA ĐƠN", lblTotalOrders,
                    "Chưa có dữ liệu"), 0, 0);

            cards.Controls.Add(
                CreateCard("TỔNG DOANH THU", lblTotalRevenue,
                    "Chưa có dữ liệu"), 1, 0);

            cards.Controls.Add(
                CreateCard("SẢN PHẨM BÁN CHẠY", lblBestSeller,
                    "Chưa có dữ liệu"), 2, 0);

            var note = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = cardBackground,
                Padding = new Padding(18)
            };

            lblStatus = new Label
            {
                Text =
                    "BÁO CÁO DOANH THU\n" +
                    "Chọn ngày và nhấn Xem báo cáo để tải số liệu từ hệ thống.",
                Dock = DockStyle.Fill,
                ForeColor = textMuted,
                Font = new Font("Segoe UI", 10F)
            };

            note.Controls.Add(lblStatus);

            // Thứ tự thêm control để giữ bố cục giao diện
            Controls.Add(note);
            Controls.Add(cards);
            Controls.Add(toolbar);
            Controls.Add(subtitle);
            Controls.Add(title);
        }

        private Panel CreateCard(
            string heading,
            Label valueLabel,
            string initialValue)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = cardBackground,
                Margin = new Padding(0, 0, 12, 0),
                Padding = new Padding(16)
            };

            var accent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = accentColor
            };

            var headingLabel = new Label
            {
                Text = heading,
                Dock = DockStyle.Top,
                Height = 38,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = textMuted
            };

            valueLabel.Text = initialValue;
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            valueLabel.ForeColor = textLight;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            valueLabel.AutoEllipsis = true;

            card.Controls.Add(valueLabel);
            card.Controls.Add(headingLabel);
            card.Controls.Add(accent);

            return card;
        }

        private static void StyleButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(color, 0.15F);
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private async Task LoadReportSafelyAsync()
        {
            if (btnView == null)
                return;

            btnView.Enabled = false;
            btnView.Text = "Đang tải...";

            try
            {
                await ShowReportAsync();
            }
            finally
            {
                btnView.Enabled = true;
                btnView.Text = "Xem báo cáo";
            }
        }

        private async Task ShowReportAsync()
        {
            try
            {
                string date =
                    dtpReportDate.Value.ToString("yyyy-MM-dd");

                var report =
                    await ApiClient.GetFromJsonAsync<QuickReportDto>(
                        $"Reports/quick-report?date={date}");

                if (report == null || !report.Success)
                {
                    MessageBox.Show(
                        "API không trả về dữ liệu báo cáo hợp lệ.",
                        "KhaMiniMart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                lblTotalOrders.Text =
                    report.TotalOrders.ToString("N0");

                lblTotalRevenue.Text =
                    report.TotalRevenue.ToString("N0") + " VNĐ";

                lblBestSeller.Text =
                    report.TotalOrders == 0
                        ? "Chưa có dữ liệu"
                        : report.BestSeller;

                if (lblStatus != null)
                {
                    lblStatus.Text =
                        $"BÁO CÁO NGÀY {dtpReportDate.Value:dd/MM/yyyy}\n" +
                        "Dữ liệu đã được tải từ hệ thống đơn hàng.";
                }
            }
            catch (HttpRequestException ex)
            {
                if (lblStatus != null)
                    lblStatus.Text = "Không thể kết nối API báo cáo.";

                MessageBox.Show(
                    "Không kết nối được API báo cáo.\n\n" +
                    "Hãy kiểm tra API đang chạy tại " +
                    "https://localhost:7044.\n\n" +
                    ex.Message,
                    "Lỗi kết nối API",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show(
                    "Yêu cầu báo cáo bị quá thời gian chờ.",
                    "KhaMiniMart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải báo cáo:\n\n" + ex.Message,
                    "Lỗi báo cáo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}