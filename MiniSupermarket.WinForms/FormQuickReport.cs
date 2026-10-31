
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormQuickReport : Form
    {
        private readonly DateTimePicker dtpReportDate = new();
        private readonly Label lblTotalOrders = new();
        private readonly Label lblTotalRevenue = new();
        private readonly Label lblBestSeller = new();

        private readonly Color navy = Color.FromArgb(17, 24, 39);
        private readonly Color blue = Color.FromArgb(37, 99, 235);
        private readonly Color background = Color.FromArgb(243, 246, 251);

        public FormQuickReport()
        {
            InitializeComponent();
            BuildReportUI();
        }

        private void BuildReportUI()
        {
            Controls.Clear();
            BackColor = background;
            Padding = new Padding(18);
            Font = new Font("Segoe UI", 10F);

            var title = new Label
            {
                Text = "BÁO CÁO DOANH THU",
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 45,
                ForeColor = navy
            };

            var subtitle = new Label
            {
                Text = "Theo dõi tình hình kinh doanh của KhaMiniMart",
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var dateLabel = new Label
            {
                Text = "Ngày báo cáo:",
                AutoSize = true,
                Location = new Point(15, 25),
                ForeColor = navy
            };

            dtpReportDate.Format = DateTimePickerFormat.Short;
            dtpReportDate.Width = 150;
            dtpReportDate.Location = new Point(125, 20);
            dtpReportDate.Font = new Font("Segoe UI", 10F);

            var btnView = new Button
            {
                Text = "Xem báo cáo",
                Width = 140,
                Height = 34,
                Location = new Point(295, 18)
            };

            StyleButton(btnView, blue);
            btnView.Click += (_, _) => ShowReport();

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
                BackColor = background
            };

            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            cards.Controls.Add(
                CreateCard("TỔNG SỐ HÓA ĐƠN", lblTotalOrders, "Chưa có dữ liệu"),
                0, 0);

            cards.Controls.Add(
                CreateCard("TỔNG DOANH THU", lblTotalRevenue, "Chưa có dữ liệu"),
                1, 0);

            cards.Controls.Add(
                CreateCard("SẢN PHẨM BÁN CHẠY", lblBestSeller, "Chưa có dữ liệu"),
                2, 0);

            var note = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(18)
            };

            var noteText = new Label
            {
                Text =
                    "LƯU Ý\n" +
                    "Đây là giao diện báo cáo mẫu. Cần kết nối API thống kê " +
                    "đơn hàng để hiển thị số liệu kinh doanh thực tế.",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 10F)
            };

            note.Controls.Add(noteText);

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
                BackColor = Color.White,
                Margin = new Padding(0, 0, 12, 0),
                Padding = new Padding(16)
            };

            var accent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = blue
            };

            var headingLabel = new Label
            {
                Text = heading,
                Dock = DockStyle.Top,
                Height = 38,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            valueLabel.Text = initialValue;
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            valueLabel.ForeColor = navy;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;

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
                ControlPaint.Dark(color, 0.12F);
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private void ShowReport()
        {
            MessageBox.Show(
                "Ngày báo cáo: " +
                dtpReportDate.Value.ToString("dd/MM/yyyy") +
                "\n\nGiao diện báo cáo đã sẵn sàng." +
                "\nĐể hiển thị số hóa đơn, doanh thu và sản phẩm bán chạy, " +
                "cần kết nối API thống kê đơn hàng.",
                "KhaMiniMart | Báo cáo doanh thu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}