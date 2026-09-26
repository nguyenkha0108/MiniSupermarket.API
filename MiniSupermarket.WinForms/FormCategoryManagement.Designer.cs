namespace MiniSupermarket.WinForms
{
    partial class FormCategoryManagement
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
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
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // dgvCategories
            // 
            dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategories.Location = new Point(6, 22);
            dgvCategories.Name = "dgvCategories";
            dgvCategories.Size = new Size(376, 286);
            dgvCategories.TabIndex = 0;
            dgvCategories.CellContentClick += dgvCategories_CellClick;
            // 
            // txtId
            // 
            txtId.Location = new Point(16, 48);
            txtId.Name = "txtId";
            txtId.Size = new Size(273, 23);
            txtId.TabIndex = 1;
            // 
            // txtCategoryName
            // 
            txtCategoryName.Location = new Point(16, 109);
            txtCategoryName.Name = "txtCategoryName";
            txtCategoryName.Size = new Size(273, 23);
            txtCategoryName.TabIndex = 2;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(16, 163);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(273, 23);
            txtDescription.TabIndex = 3;
            // 
            // txtKeyword
            // 
            txtKeyword.AccessibleDescription = "";
            txtKeyword.AccessibleName = "";
            txtKeyword.Location = new Point(6, 22);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.PlaceholderText = "Nhập từ khóa...";
            txtKeyword.Size = new Size(298, 23);
            txtKeyword.TabIndex = 4;
            txtKeyword.UseWaitCursor = true;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(403, 23);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 5;
            btnLoad.Text = "Tải lại";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(321, 22);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 6;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(453, 341);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Thêm mới";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(547, 341);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 8;
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(647, 341);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BorderStyle = BorderStyle.Fixed3D;
            label4.Location = new Point(16, 28);
            label4.Name = "label4";
            label4.Size = new Size(40, 17);
            label4.TabIndex = 13;
            label4.Text = "Mã ID";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BorderStyle = BorderStyle.Fixed3D;
            label5.Location = new Point(16, 89);
            label5.Name = "label5";
            label5.Size = new Size(94, 17);
            label5.TabIndex = 14;
            label5.Text = "Tên Nhóm hàng";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BorderStyle = BorderStyle.Fixed3D;
            label6.Location = new Point(16, 143);
            label6.Name = "label6";
            label6.Size = new Size(79, 17);
            label6.TabIndex = 15;
            label6.Text = "Mô tả chi tiết";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(txtId);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(txtCategoryName);
            groupBox1.Controls.Add(txtDescription);
            groupBox1.Location = new Point(433, 124);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(350, 198);
            groupBox1.TabIndex = 16;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thông tin nhóm hàng";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dgvCategories);
            groupBox2.Location = new Point(24, 124);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(388, 314);
            groupBox2.TabIndex = 17;
            groupBox2.TabStop = false;
            groupBox2.Text = "Danh sách Nhóm hàng";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(txtKeyword);
            groupBox3.Controls.Add(btnSearch);
            groupBox3.Controls.Add(btnLoad);
            groupBox3.Location = new Point(24, 55);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(498, 52);
            groupBox3.TabIndex = 18;
            groupBox3.TabStop = false;
            groupBox3.Text = "Tìm kiếm";
            // 
            // FormCategoryManagement
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnAdd);
            Name = "FormCategoryManagement";
            Text = "FormCategoryManagement";
            Load += FormCategoryManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
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
    }
}