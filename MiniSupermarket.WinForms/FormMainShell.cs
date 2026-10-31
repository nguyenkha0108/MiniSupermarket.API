using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public sealed class FormMainShell : Form
    {
        private readonly Panel panelSidebar = new();
        private readonly Panel panelTopHeader = new();
        private readonly Panel panelMainContent = new();
        private readonly Label lblTitle = new();
        private readonly Label lblUserInfo = new();
        private readonly Dictionary<string, Button> menuButtons = new();
        private Form? activeChild;
        private FormPOS? posScreen; // Giữ lại để không mất giỏ hàng khi chuyển menu
        private readonly FormLogin ownerLogin;

        // BẢNG MÀU DARK MODE ĐỒNG BỘ VỚI FORM LOGIN
        private static readonly Color DarkBackground = Color.FromArgb(15, 23, 42);   // Slate 900 (Nền chính)
        private static readonly Color SidebarBackground = Color.FromArgb(15, 23, 42); // Slate 900
        private static readonly Color CardBackground = Color.FromArgb(30, 41, 59);    // Slate 800 (Nền header/card)
        private static readonly Color AccentColor = Color.FromArgb(168, 85, 247);     // Tím Violet hiện đại
        private static readonly Color AccentHover = Color.FromArgb(147, 51, 234);     // Tím đậm khi hover
        private static readonly Color NavHover = Color.FromArgb(51, 65, 85);          // Slate 700
        private static readonly Color TextLight = Color.FromArgb(248, 250, 252);      // Chữ sáng
        private static readonly Color TextMuted = Color.FromArgb(148, 163, 184);      // Chữ phụ

        public FormMainShell(FormLogin loginForm)
        {
            ownerLogin = loginForm;

            InitializeShell();
            ApplyRolePermissions();

            Load += (_, _) => OpenDefaultScreen();

            FormClosed += (_, _) =>
            {
                activeChild?.Dispose();
                posScreen?.Dispose();
                SessionManager.ClearSession();
                ownerLogin.Show();
            };
        }

        private void InitializeShell()
        {
            Text = "KhaMiniMart | Hệ thống quản lý bán lẻ";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1280, 760);
            MinimumSize = new Size(1000, 600);
            BackColor = DarkBackground;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Font;

            // 1. SIDEBAR (BẢNG ĐIỀU HƯỚNG TỐI CAO CẤP)
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Width = 260;
            panelSidebar.BackColor = SidebarBackground;
            panelSidebar.Padding = new Padding(12, 12, 12, 10);

            var logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SidebarBackground
            };

            var logo = new Label
            {
                Text = "⚡ KHA MINI MART",
                Dock = DockStyle.Fill,
                ForeColor = AccentColor,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var logoLine = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 2,
                BackColor = Color.FromArgb(51, 65, 85)
            };

            logoPanel.Controls.Add(logo);
            logoPanel.Controls.Add(logoLine);
            panelSidebar.Controls.Add(logoPanel);

            // NÚT ĐĂNG XUẤT Ở CUỐI SIDEBAR
            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(0, 8, 0, 0),
                BackColor = SidebarBackground
            };

            var btnLogout = CreateMenuButton("🚪 Đăng xuất");
            btnLogout.Dock = DockStyle.Fill;
            btnLogout.ForeColor = Color.FromArgb(248, 113, 113); // Màu đỏ cảnh báo nổi bật
            btnLogout.Click += Logout_Click;

            footer.Controls.Add(btnLogout);
            panelSidebar.Controls.Add(footer);

            // MENU CHỨC NĂNG - TÍCH HỢP DASHBOARD VÀ PHÂN QUYỀN
            AddMenu("Dashboard", "Trang chủ / Tổng quan",
                "DASHBOARD TỔNG QUAN HỆ THỐNG", Roles("Admin", "Cashier", "Warehouse"));

            AddMenu("POS", "Bán hàng (POS)",
                "BÁN HÀNG (POS)", Roles("Admin", "Cashier"));

            AddMenu("Category", "Quản lý danh mục",
                "QUẢN LÝ DANH MỤC NHÓM HÀNG", Roles("Admin", "Warehouse"));

            AddMenu("Product", "Quản lý sản phẩm",
                "QUẢN LÝ SẢN PHẨM & KHO HÀNG", Roles("Admin", "Warehouse"));

            AddMenu("Customer", "Quản lý khách hàng",
                "QUẢN LÝ KHÁCH HÀNG", Roles("Admin"));

            AddMenu("Orders", "Quản lý đơn hàng",
                "QUẢN LÝ ĐƠN HÀNG", Roles("Admin"));

            AddMenu("Reports", "Báo cáo doanh thu",
                "BÁO CÁO DOANH THU", Roles("Admin"));

            AddMenu("Users", "Quản trị tài khoản",
                "QUẢN TRỊ TÀI KHOẢN", Roles("Admin"));

            // Đưa logo lên trên cùng của sidebar (sau khi các nút menu đã được thêm)
            panelSidebar.Controls.SetChildIndex(logoPanel, panelSidebar.Controls.Count - 1);

            // 2. HEADER PHÍA TRÊN
            panelTopHeader.Dock = DockStyle.Top;
            panelTopHeader.Height = 72;
            panelTopHeader.BackColor = CardBackground;
            panelTopHeader.Padding = new Padding(26, 0, 24, 0);

            lblTitle.Text = "BÀN LÀM VIỆC HỆ THỐNG";
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Width = 520;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = TextLight;

            lblUserInfo.Dock = DockStyle.Fill;
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;
            lblUserInfo.ForeColor = TextMuted;
            lblUserInfo.Font = new Font("Segoe UI", 9.5F);

            panelTopHeader.Controls.Add(lblUserInfo);
            panelTopHeader.Controls.Add(lblTitle);

            // 3. VÙNG NỘI DUNG CHÍNH
            panelMainContent.Dock = DockStyle.Fill;
            panelMainContent.BackColor = DarkBackground;
            panelMainContent.Padding = new Padding(20);

            // THỨ TỰ DOCK
            Controls.Add(panelMainContent);
            Controls.Add(panelTopHeader);
            Controls.Add(panelSidebar);

            lblUserInfo.Text =
                $"Nhân viên: {SessionManager.Username}    |    Vai trò: {SessionManager.Role}";
        }

        private static HashSet<string> Roles(params string[] roles)
        {
            return new HashSet<string>(
                roles,
                StringComparer.OrdinalIgnoreCase);
        }

        private void AddMenu(
            string key,
            string text,
            string title,
            HashSet<string> allowedRoles)
        {
            var button = CreateMenuButton("  " + text);

            button.Dock = DockStyle.Top;
            button.Visible = allowedRoles.Contains(SessionManager.Role);
            button.Click += (_, _) => OpenScreen(key, title, button);

            menuButtons[key] = button;

            panelSidebar.Controls.Add(button);
            panelSidebar.Controls.SetChildIndex(button, 1);
        }

        private static Button CreateMenuButton(string text)
        {
            var button = new Button
            {
                Text = text,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(226, 232, 240),
                BackColor = SidebarBackground,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 10F),
                Cursor = Cursors.Hand,
                Padding = new Padding(12, 0, 0, 0),
                UseVisualStyleBackColor = false,
                Margin = new Padding(0, 4, 0, 4)
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = NavHover;
            button.FlatAppearance.MouseDownBackColor = AccentHover;

            return button;
        }

        private void ApplyRolePermissions()
        {
            if (SessionManager.Role.Equals(
                    "Admin", StringComparison.OrdinalIgnoreCase) ||
                SessionManager.Role.Equals(
                    "Cashier", StringComparison.OrdinalIgnoreCase) ||
                SessionManager.Role.Equals(
                    "Warehouse", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            MessageBox.Show(
                "Tài khoản chưa được cấp quyền hợp lệ!",
                "Cảnh báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            BeginInvoke(new Action(Close));
        }

        private void OpenDefaultScreen()
        {
            // Mặc định tất cả các vai trò khi mở phần mềm đều vào trang Dashboard tổng quan trước
            string key = "Dashboard";

            if (menuButtons.TryGetValue(key, out var button))
            {
                button.PerformClick();
            }
        }

        private void OpenScreen(string key, string title, Button selectedButton)
        {
            var allowed = key switch
            {
                "Dashboard" => true,
                "POS" => SessionManager.IsAdmin || SessionManager.IsCashier,
                "Category" or "Product" =>
                    SessionManager.IsAdmin || SessionManager.IsWarehouse,
                "Customer" or "Orders" => SessionManager.IsAdmin,
                "Reports" or "Users" => SessionManager.IsAdmin,
                _ => false
            };

            if (!allowed)
            {
                MessageBox.Show(
                    "Bạn không có quyền truy cập chức năng này!",
                    "Từ chối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);

                return;
            }

            if (activeChild != null)
            {
                panelMainContent.Controls.Remove(activeChild);

                // Form POS không đóng/hủy để giữ nguyên giỏ hàng
                if (activeChild is not FormPOS)
                {
                    activeChild.Close();
                    activeChild.Dispose();
                }

                activeChild = null;
            }

            // ĐẶT LẠI TRẠNG THÁI MÀU MENU
            foreach (var btn in menuButtons.Values)
            {
                btn.BackColor = SidebarBackground;
                btn.ForeColor = Color.FromArgb(226, 232, 240);
            }

            // MENU ĐANG ĐƯỢC CHỌN (HIGHLIGHT MÀU TÍM VIOLET)
            selectedButton.BackColor = AccentHover;
            selectedButton.ForeColor = Color.White;

            lblTitle.Text = title;

            // XỬ LÝ GỌI FORM TƯƠNG ỨNG
            if (key == "Dashboard")
            {
                ShowChild(new FormDashboard()); // Gọi Form Dashboard tổng quan chứa 4 thẻ KPI
            }
            else if (key == "Category")
            {
                ShowChild(new FormCategoryManagement());
            }
            else if (key == "Product")
            {
                ShowChild(new FormProductManagement());
            }
            else if (key == "Customer")
            {
                ShowChild(new FormCustomerManagement());
            }
            else if (key == "Orders")
            {
                ShowChild(new FormOrders());
            }
            else if (key == "Users")
            {
                ShowChild(new FormUserManagement());
            }
            else if (key == "Reports")
            {
                ShowChild(new FormQuickReport());
            }
            else if (key == "POS")
            {
                posScreen ??= new FormPOS();
                ShowChild(posScreen);
            }
            else
            {
                ShowChild(new SimpleWorkspaceForm(
                    title, GetDescription(key)));
            }
        }

        private static string GetDescription(string key) => key switch
        {
            "Dashboard" => "Trang tổng quan hệ thống quản lý bán hàng.",
            "POS" => "Khung giao diện quầy bán hàng.",
            "Product" => "Quản lý sản phẩm và tồn kho.",
            "Customer" => "Quản lý khách hàng thành viên và tích điểm.",
            "Orders" => "Tra cứu hóa đơn đã thanh toán và xem chi tiết sản phẩm.",
            "Reports" => "Khu vực báo cáo doanh thu dành riêng cho Admin.",
            "Users" => "Khu vực quản trị tài khoản và phân quyền dành riêng cho Admin.",
            _ => ""
        };

        private void ShowChild(Form child)
        {
            activeChild = child;
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            child.BackColor = DarkBackground;
            panelMainContent.Controls.Clear();
            panelMainContent.Controls.Add(child);
            child.Show();
            child.BringToFront();
        }

        private void Logout_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            SessionManager.ClearSession();
            Close();
        }
    }

    // MÀN HÌNH WORKSPACE TẠM THỜI GIAO DIỆN THẺ CARD HIỆN ĐẠI (DARK MODE)
    internal sealed class SimpleWorkspaceForm : Form
    {
        public SimpleWorkspaceForm(string title, string description)
        {
            BackColor = Color.FromArgb(15, 23, 42);
            Padding = new Padding(18);

            var card = new Panel
            {
                Dock = DockStyle.Top,
                Height = 210,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(28),
                Margin = new Padding(10)
            };

            var heading = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 52,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(248, 250, 252)
            };

            var accent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 3,
                BackColor = Color.FromArgb(168, 85, 247)
            };

            var message = new Label
            {
                Text = description,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = false,
                Padding = new Padding(0, 12, 0, 0)
            };

            card.Controls.Add(message);
            card.Controls.Add(accent);
            card.Controls.Add(heading);
            Controls.Add(card);
        }
    }
}