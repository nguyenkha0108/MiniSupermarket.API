
namespace MiniSupermarket.WinForms
{
    partial class FormCategoryManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            dgvCategories = new DataGridView();
            txtId = new TextBox();
            txtCategoryName = new TextBox();
            txtDescription = new TextBox();
            txtKeyword = new TextBox();
            btnLoad = new Button();
            btnSearch = new Button();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            groupBox1 = new GroupBox();
            infoLayout = new TableLayoutPanel();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            searchLayout = new TableLayoutPanel();
            mainLayout = new TableLayoutPanel();
            contentLayout = new TableLayoutPanel();
            actionPanel = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();
            groupBox1.SuspendLayout();
            infoLayout.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            searchLayout.SuspendLayout();
            mainLayout.SuspendLayout();
            contentLayout.SuspendLayout();
            actionPanel.SuspendLayout();
            SuspendLayout();
            // 
            // dgvCategories
            // 
            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.AllowUserToDeleteRows = false;
            dgvCategories.AllowUserToResizeRows = false;
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.BackgroundColor = Color.White;
            dgvCategories.BorderStyle = BorderStyle.None;
            dgvCategories.Dock = DockStyle.Fill;
            dgvCategories.Location = new Point(8, 24);
            dgvCategories.Margin = new Padding(0);
            dgvCategories.MultiSelect = false;
            dgvCategories.Name = "dgvCategories";
            dgvCategories.ReadOnly = true;
            dgvCategories.RowHeadersVisible = false;
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.Size = new Size(691, 468);
            dgvCategories.TabIndex = 0;
            dgvCategories.CellClick += dgvCategories_CellClick;
            // 
            // txtId
            // 
            txtId.Dock = DockStyle.Fill;
            txtId.Location = new Point(0, 29);
            txtId.Margin = new Padding(0, 3, 0, 6);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(337, 23);
            txtId.TabIndex = 1;
            // 
            // txtCategoryName
            // 
            txtCategoryName.Dock = DockStyle.Fill;
            txtCategoryName.Location = new Point(0, 91);
            txtCategoryName.Margin = new Padding(0, 3, 0, 6);
            txtCategoryName.Name = "txtCategoryName";
            txtCategoryName.Size = new Size(337, 23);
            txtCategoryName.TabIndex = 3;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Fill;
            txtDescription.Location = new Point(0, 153);
            txtDescription.Margin = new Padding(0, 3, 0, 0);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.Size = new Size(337, 303);
            txtDescription.TabIndex = 5;
            // 
            // txtKeyword
            // 
            txtKeyword.Dock = DockStyle.Fill;
            txtKeyword.Location = new Point(2, 2);
            txtKeyword.Margin = new Padding(2);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.PlaceholderText = "Nhập tên nhóm hàng cần tìm...";
            txtKeyword.Size = new Size(835, 23);
            txtKeyword.TabIndex = 0;
            // 
            // btnLoad
            // 
            btnLoad.Dock = DockStyle.Fill;
            btnLoad.Location = new Point(962, 2);
            btnLoad.Margin = new Padding(3, 2, 3, 2);
            btnLoad.MinimumSize = new Size(0, 30);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(99, 31);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "Tải lại";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSearch
            // 
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Location = new Point(842, 2);
            btnSearch.Margin = new Padding(3, 2, 3, 2);
            btnSearch.MinimumSize = new Size(0, 30);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(114, 31);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(733, 6);
            btnAdd.Margin = new Padding(5, 3, 0, 3);
            btnAdd.MinimumSize = new Size(110, 38);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(115, 38);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Thêm mới";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(856, 6);
            btnUpdate.Margin = new Padding(8, 3, 0, 3);
            btnUpdate.MinimumSize = new Size(110, 38);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(115, 38);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(979, 6);
            btnDelete.Margin = new Padding(8, 3, 0, 3);
            btnDelete.MinimumSize = new Size(100, 38);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(105, 38);
            btnDelete.TabIndex = 0;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // label4
            // 
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(0, 0);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(337, 26);
            label4.TabIndex = 0;
            label4.Text = "Mã ID";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(0, 62);
            label5.Margin = new Padding(0);
            label5.Name = "label5";
            label5.Size = new Size(337, 26);
            label5.TabIndex = 2;
            label5.Text = "Tên nhóm hàng";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.Dock = DockStyle.Fill;
            label6.Location = new Point(0, 124);
            label6.Margin = new Padding(0);
            label6.Name = "label6";
            label6.Size = new Size(337, 26);
            label6.TabIndex = 4;
            label6.Text = "Mô tả chi tiết";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(infoLayout);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(723, 0);
            groupBox1.Margin = new Padding(8, 0, 0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(12, 18, 12, 10);
            groupBox1.Size = new Size(361, 500);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thông tin nhóm hàng";
            // 
            // infoLayout
            // 
            infoLayout.ColumnCount = 1;
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            infoLayout.Controls.Add(label4, 0, 0);
            infoLayout.Controls.Add(txtId, 0, 1);
            infoLayout.Controls.Add(label5, 0, 2);
            infoLayout.Controls.Add(txtCategoryName, 0, 3);
            infoLayout.Controls.Add(label6, 0, 4);
            infoLayout.Controls.Add(txtDescription, 0, 5);
            infoLayout.Dock = DockStyle.Fill;
            infoLayout.Location = new Point(12, 34);
            infoLayout.Margin = new Padding(0);
            infoLayout.Name = "infoLayout";
            infoLayout.RowCount = 6;
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            infoLayout.Size = new Size(337, 456);
            infoLayout.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dgvCategories);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 0);
            groupBox2.Margin = new Padding(0, 0, 8, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(8);
            groupBox2.Size = new Size(707, 500);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Danh sách nhóm hàng";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(searchLayout);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(8, 8);
            groupBox3.Margin = new Padding(0, 0, 0, 8);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(10, 18, 10, 5);
            groupBox3.Size = new Size(1084, 74);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "Tìm kiếm danh mục";
            // 
            // searchLayout
            // 
            searchLayout.ColumnCount = 3;
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105F));
            searchLayout.Controls.Add(txtKeyword, 0, 0);
            searchLayout.Controls.Add(btnSearch, 1, 0);
            searchLayout.Controls.Add(btnLoad, 2, 0);
            searchLayout.Dock = DockStyle.Fill;
            searchLayout.Location = new Point(10, 34);
            searchLayout.Margin = new Padding(0);
            searchLayout.Name = "searchLayout";
            searchLayout.RowCount = 1;
            searchLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            searchLayout.Size = new Size(1064, 35);
            searchLayout.TabIndex = 0;
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(groupBox3, 0, 0);
            mainLayout.Controls.Add(contentLayout, 0, 1);
            mainLayout.Controls.Add(actionPanel, 0, 2);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Margin = new Padding(0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(8);
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            mainLayout.Size = new Size(1100, 650);
            mainLayout.TabIndex = 0;
            // 
            // contentLayout
            // 
            contentLayout.ColumnCount = 2;
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            contentLayout.Controls.Add(groupBox2, 0, 0);
            contentLayout.Controls.Add(groupBox1, 1, 0);
            contentLayout.Dock = DockStyle.Fill;
            contentLayout.Location = new Point(8, 90);
            contentLayout.Margin = new Padding(0);
            contentLayout.Name = "contentLayout";
            contentLayout.RowCount = 1;
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contentLayout.Size = new Size(1084, 500);
            contentLayout.TabIndex = 1;
            // 
            // actionPanel
            // 
            actionPanel.Controls.Add(btnDelete);
            actionPanel.Controls.Add(btnUpdate);
            actionPanel.Controls.Add(btnAdd);
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.FlowDirection = FlowDirection.RightToLeft;
            actionPanel.Location = new Point(8, 590);
            actionPanel.Margin = new Padding(0);
            actionPanel.Name = "actionPanel";
            actionPanel.Padding = new Padding(0, 3, 0, 0);
            actionPanel.Size = new Size(1084, 52);
            actionPanel.TabIndex = 2;
            actionPanel.WrapContents = false;
            // 
            // FormCategoryManagement
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 650);
            Controls.Add(mainLayout);
            MinimumSize = new Size(850, 500);
            Name = "FormCategoryManagement";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KhaMiniMart | Quản lý danh mục";
            Load += FormCategoryManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();
            groupBox1.ResumeLayout(false);
            infoLayout.ResumeLayout(false);
            infoLayout.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            searchLayout.ResumeLayout(false);
            searchLayout.PerformLayout();
            mainLayout.ResumeLayout(false);
            contentLayout.ResumeLayout(false);
            actionPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvCategories;

        private TextBox txtId;
        private TextBox txtCategoryName;
        private TextBox txtDescription;
        private TextBox txtKeyword;

        private Button btnLoad;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        private Label label4;
        private Label label5;
        private Label label6;

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;

        private TableLayoutPanel infoLayout;
        private TableLayoutPanel searchLayout;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel contentLayout;

        private FlowLayoutPanel actionPanel;
    }
}
