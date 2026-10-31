using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormUserManagement : Form
    {
        private readonly DataGridView dgvUsers = new();

    public FormUserManagement()
        {
            Text = "Quản trị tài khoản";
            BackColor = Color.White;

            var title = new Label
            {
                Text = "QUẢN TRỊ TÀI KHOẢN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(20, 15, 0, 0)
            };

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 55,
                Padding = new Padding(12, 8, 0, 0)
            };

            var btnAdd = new Button
            {
                Text = "Thêm tài khoản",
                AutoSize = true
            };

            var btnReset = new Button
            {
                Text = "Đặt lại mật khẩu",
                AutoSize = true
            };

            var btnLock = new Button
            {
                Text = "Khóa / mở khóa",
                AutoSize = true
            };

            btnAdd.Click += (_, _) =>
                MessageBox.Show(
                    "Bước tiếp theo sẽ kết nối chức năng thêm tài khoản với API.");

            btnReset.Click += (_, _) =>
                MessageBox.Show(
                    "Bước tiếp theo sẽ kết nối chức năng đặt lại mật khẩu với API.");

            btnLock.Click += (_, _) =>
                MessageBox.Show(
                    "Bước tiếp theo sẽ kết nối chức năng khóa tài khoản với API.");

            toolbar.Controls.AddRange(
                new Control[] { btnAdd, btnReset, btnLock });

            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.ReadOnly = true;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;

            dgvUsers.Columns.Add("Username", "Tên đăng nhập");
            dgvUsers.Columns.Add("Role", "Vai trò");
            dgvUsers.Columns.Add("Status", "Trạng thái");

            LoadSampleUsers();

            Controls.Add(dgvUsers);
            Controls.Add(toolbar);
            Controls.Add(title);
        }

        private void LoadSampleUsers()
        {
            string[] admins =
            {
            "admin01", "admin02", "admin_backup", "supervisor"
        };

            string[] cashiers =
            {
            "cashier01", "cashier02", "cashier03",
            "cashier04", "cashier05", "cashier06"
        };

            string[] warehouseUsers =
            {
            "ware01", "ware02", "ware03", "ware04", "ware05"
        };

            foreach (string username in admins)
                dgvUsers.Rows.Add(username, "Admin", "Hoạt động");

            foreach (string username in cashiers)
                dgvUsers.Rows.Add(username, "Cashier", "Hoạt động");

            foreach (string username in warehouseUsers)
                dgvUsers.Rows.Add(username, "Warehouse", "Hoạt động");
        }
    }

}
