using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormDashboard : Form
    {
        // Bảng màu Dark Mode đồng bộ hệ thống Kha Mini Mart
        private readonly Color darkBackground = Color.FromArgb(15, 23, 42);   // Slate 900
        private readonly Color cardBackground = Color.FromArgb(30, 41, 59);   // Slate 800
        private readonly Color accentColor = Color.FromArgb(168, 85, 247);    // Tím Violet
        private readonly Color textLight = Color.FromArgb(248, 250, 252);     // Slate 50
        private readonly Color textMuted = Color.FromArgb(148, 163, 184);     // Slate 400
        private readonly Color warningColor = Color.FromArgb(251, 146, 60);   // Cam cảnh báo

        private readonly Label lblTodayRevenue = new();
        private readonly Label lblTotalInvoices = new();
        private readonly Label lblNewCustomers = new();
        private readonly Label lblLowStock = new();
        private readonly Label lblWelcome = new();
        private readonly Button btnRefresh = new();

        public FormDashboard()
        {
            InitializeComponent();
            BuildDashboardUI();
            Shown += async (_, _) => await LoadDashboardDataAsync();
        }

        private void BuildDashboardUI()
        {
            BackColor = darkBackground;
            Padding = new Padding(28);
            Font = new Font("Segoe UI", 10F);
            Text = "Dashboard Tổng Quan";
            AutoScroll = true;

            // ----- HEADER: TIÊU ĐỀ & NÚT LÀM MỚI -----
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = darkBackground,
                Margin = new Padding(0, 0, 0, 10)
            };

            lblWelcome.Text = "⚡ TỔNG QUAN HỆ THỐNG KHA MINI MART";
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.ForeColor = textLight;
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(0, 5);

            var lblSubWelcome = new Label
            {
                Text = "Cập nhật nhanh các chỉ số kinh doanh trong ngày hôm nay.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = textMuted,
                AutoSize = true,
                Location = new Point(2, 35)
            };

            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.Width = 125;
            btnRefresh.Height = 36;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Location = new Point(headerPanel.Width - 130, 12);
            // Canh chỉnh vị trí nút khi form thay đổi kích thước
            headerPanel.Resize += (s, e) => { btnRefresh.Left = headerPanel.Width - btnRefresh.Width; };

            StyleButton(btnRefresh, accentColor);
            btnRefresh.Click += async (_, _) => await LoadDashboardDataAsync();

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblSubWelcome);
            headerPanel.Controls.Add(btnRefresh);

            // ----- BỐ CỤC 4 THẺ KPI (TableLayoutPanel 4 cột, 1 hàng) -----
            var cardsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 150,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 15, 0, 0),
                BackColor = darkBackground
            };
            for (int i = 0; i < 4; i++)
                cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            cardsPanel.Controls.Add(CreateCard("DOANH THU HÔM NAY", lblTodayRevenue, "💰"), 0, 0);
            cardsPanel.Controls.Add(CreateCard("TỔNG HÓA ĐƠN", lblTotalInvoices, "🧾"), 1, 0);
            cardsPanel.Controls.Add(CreateCard("KHÁCH HÀNG MỚI (30N)", lblNewCustomers, "👥"), 2, 0);
            cardsPanel.Controls.Add(CreateCard("SẢN PHẨM SẮP HẾT", lblLowStock, "⚠️"), 3, 0);

            // ----- KHU VỰC THÔNG TIN BỔ SUNG TRANG TRÍ DƯỚI ĐÁY -----
            var infoCard = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = cardBackground,
                Padding = new Padding(24),
                Margin = new Padding(0, 24, 0, 0)
            };

            var accentBarInfo = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accentColor };

            var infoTitle = new Label
            {
                Text = "HƯỚNG DẪN NHANH & TRẠNG THÁI HỆ THỐNG",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = textLight,
                Dock = DockStyle.Top,
                Height = 30
            };

            var infoContent = new Label
            {
                Text = "• Sử dụng thanh menu bên trái để chuyển đổi nhanh giữa các phân hệ: Bán hàng POS, Quản lý sản phẩm, Hóa đơn và Báo cáo.\n" +
                       "• Hệ thống tự động đồng bộ dữ liệu kho hàng khi phát sinh giao dịch thanh toán thành công.\n" +
                       "• Bấm nút 'Làm mới' ở góc trên để cập nhật dữ liệu biểu đồ và số liệu KPI mới nhất từ máy chủ.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = textMuted,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };

            infoCard.Controls.Add(infoContent);
            infoCard.Controls.Add(infoTitle);
            infoCard.Controls.Add(accentBarInfo);

            // Thêm các thành phần vào Form theo thứ tự
            Controls.Add(infoCard);
            Controls.Add(cardsPanel);
            Controls.Add(headerPanel);
        }

        private Panel CreateCard(string heading, Label valueLabel, string icon)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = cardBackground,
                Margin = new Padding(0, 0, 14, 0),
                Padding = new Padding(18)
            };

            // Thanh điểm nhấn màu tím bên trái thẻ
            var accentBar = new Panel { Dock = DockStyle.Left, Width = 4, BackColor = accentColor };

            var headingLabel = new Label
            {
                Text = $"{icon}  {heading}",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textMuted
            };

            valueLabel.Text = "—";
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            valueLabel.ForeColor = textLight;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            valueLabel.AutoEllipsis = true;

            card.Controls.Add(valueLabel);
            card.Controls.Add(headingLabel);
            card.Controls.Add(accentBar);
            return card;
        }

        private static void StyleButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private static string Money(decimal value)
        {
            return value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }

        private async Task LoadDashboardDataAsync()
        {
            btnRefresh.Enabled = false;
            btnRefresh.Text = "Đang tải...";

            try
            {
                var summary = await ApiClientService.GetAsync<DashboardSummaryDto>("Reports/dashboard-summary");

                if (summary != null)
                {
                    lblTodayRevenue.Text = Money(summary.TodayRevenue);
                    lblTotalInvoices.Text = summary.TotalInvoices.ToString("N0", CultureInfo.InvariantCulture);
                    lblNewCustomers.Text = summary.NewCustomers30Days.ToString("N0", CultureInfo.InvariantCulture);
                    lblLowStock.Text = summary.LowStockProducts.ToString("N0", CultureInfo.InvariantCulture);

                    // Đổi màu cảnh báo màu cam nổi bật nếu tồn kho <= 10
                    lblLowStock.ForeColor = summary.LowStockProducts > 0 ? warningColor : textLight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu Dashboard:\n" + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Text = "🔄 Làm mới";
            }
        }
    }
}