
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly FormLogin ownerLogin;

        private static readonly Color Navy = Color.FromArgb(17, 24, 39);
        private static readonly Color NavyLight = Color.FromArgb(30, 41, 59);
        private static readonly Color Blue = Color.FromArgb(37, 99, 235);
        private static readonly Color BlueHover = Color.FromArgb(29, 78, 216);
        private static readonly Color PageBackground = Color.FromArgb(243, 246, 251);
        private static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
        private static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);

        public FormMainShell(FormLogin loginForm)
        {
            ownerLogin = loginForm;

            InitializeShell();
            ApplyRolePermissions();

            Load += (_, _) => OpenDefaultScreen();

            FormClosed += (_, _) =>
            {
                activeChild?.Dispose();
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
            BackColor = PageBackground;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Font;

            // SIDEBAR
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Width = 245;
            panelSidebar.BackColor = Navy;
            panelSidebar.Padding = new Padding(12, 12, 12, 10);

            var logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 76,
                BackColor = Navy
            };

            var logo = new Label
            {
                Text = "◆  KhaMiniMart",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var logoLine = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 3,
                BackColor = Blue
            };

            logoPanel.Controls.Add(logo);
            logoPanel.Controls.Add(logoLine);
            panelSidebar.Controls.Add(logoPanel);

            // NÚT ĐĂNG XUẤT
            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                Padding = new Padding(0, 8, 0, 0),
                BackColor = Navy
            };

            var btnLogout = CreateMenuButton("Đăng xuất");
            btnLogout.Dock = DockStyle.Fill;
            btnLogout.ForeColor = Color.FromArgb(248, 113, 113);
            btnLogout.Click += Logout_Click;

            footer.Controls.Add(btnLogout);
            panelSidebar.Controls.Add(footer);

            // MENU - GIỮ NGUYÊN PHÂN QUYỀN
            AddMenu("POS", "Bán hàng (POS)",
                "BÁN HÀNG (POS)", Roles("Admin", "Cashier"));

            AddMenu("Category", "Quản lý danh mục",
                "QUẢN LÝ DANH MỤC NHÓM HÀNG", Roles("Admin", "Warehouse"));

            AddMenu("Product", "Quản lý sản phẩm",
                "QUẢN LÝ SẢN PHẨM & KHO HÀNG", Roles("Admin", "Warehouse"));

            AddMenu("Customer", "Quản lý khách hàng",
                "QUẢN LÝ KHÁCH HÀNG", Roles("Admin", "Cashier"));

            AddMenu("Orders", "Quản lý đơn hàng",
                "QUẢN LÝ ĐƠN HÀNG", Roles("Admin", "Cashier"));

            AddMenu("Reports", "Báo cáo doanh thu",
                "BÁO CÁO DOANH THU", Roles("Admin"));

            AddMenu("Users", "Quản trị tài khoản",
                "QUẢN TRỊ TÀI KHOẢN", Roles("Admin"));

            // HEADER
            panelTopHeader.Dock = DockStyle.Top;
            panelTopHeader.Height = 72;
            panelTopHeader.BackColor = Color.White;
            panelTopHeader.Padding = new Padding(26, 0, 24, 0);

            lblTitle.Text = "BÀN LÀM VIỆC HỆ THỐNG";
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Width = 520;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = TextPrimary;

            lblUserInfo.Dock = DockStyle.Fill;
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;
            lblUserInfo.ForeColor = TextSecondary;
            lblUserInfo.Font = new Font("Segoe UI", 9.5F);

            panelTopHeader.Controls.Add(lblUserInfo);
            panelTopHeader.Controls.Add(lblTitle);

            // VÙNG NỘI DUNG
            panelMainContent.Dock = DockStyle.Fill;
            panelMainContent.BackColor = PageBackground;
            panelMainContent.Padding = new Padding(18);

            // THỨ TỰ DOCK
            Controls.Add(panelMainContent);
            Controls.Add(panelTopHeader);
            Controls.Add(panelSidebar);

            lblUserInfo.Text =
                $"Nhân viên: {SessionManager.Username}   |   Vai trò: {SessionManager.Role}";
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
            var button = CreateMenuButton(text);

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
                Text = "   " + text,
                Height = 48,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(226, 232, 240),
                BackColor = Navy,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand,
                Padding = new Padding(8, 0, 0, 0),
                UseVisualStyleBackColor = false,
                Margin = new Padding(0, 3, 0, 3)
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = NavyLight;
            button.FlatAppearance.MouseDownBackColor = BlueHover;

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
            string key = SessionManager.Role.ToUpperInvariant() switch
            {
                "CASHIER" => "POS",
                "WAREHOUSE" => "Product",
                _ => "Category"
            };

            if (menuButtons.TryGetValue(key, out var button))
            {
                button.PerformClick();
            }
        }

        private void OpenScreen(string key, string title, Button selectedButton)
        {
            // KIỂM TRA QUYỀN TRUY CẬP
            var allowed = key switch
            {
                "POS" => SessionManager.IsAdmin || SessionManager.IsCashier,
                "Category" or "Product" =>
                    SessionManager.IsAdmin || SessionManager.IsWarehouse,
                "Customer" or "Orders" =>
                    SessionManager.IsAdmin || SessionManager.IsCashier,
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
                activeChild.Close();
                activeChild.Dispose();
                activeChild = null;
            }

            // TRẠNG THÁI MENU
            foreach (var btn in menuButtons.Values)
            {
                btn.BackColor = Navy;
                btn.ForeColor = Color.FromArgb(226, 232, 240);
            }

            selectedButton.BackColor = Blue;
            selectedButton.ForeColor = Color.White;

            lblTitle.Text = title;

            // MỞ CÁC FORM HIỆN CÓ
            if (key == "Category")
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
                ShowChild(new FormPOS());
            }
            else
            {
                ShowChild(new SimpleWorkspaceForm(
                    title, GetDescription(key)));
            }
        }

        private static string GetDescription(string key) => key switch
        {
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
            child.BackColor = PageBackground;
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

    internal sealed class SimpleWorkspaceForm : Form
    {
        public SimpleWorkspaceForm(string title, string description)
        {
            BackColor = Color.FromArgb(243, 246, 251);
            Padding = new Padding(18);

            var card = new Panel
            {
                Dock = DockStyle.Top,
                Height = 210,
                BackColor = Color.White,
                Padding = new Padding(28),
                Margin = new Padding(10)
            };

            var heading = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 52,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };

            var accent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 3,
                BackColor = Color.FromArgb(37, 99, 235)
            };

            var message = new Label
            {
                Text = description,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(71, 85, 105),
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