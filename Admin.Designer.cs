namespace JTI_Payroll_System
{
    partial class Admin
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
            add = new Button();
            search = new Button();
            delete = new Button();
            update = new Button();
            label1 = new Label();
            label2 = new Label();
            username = new TextBox();
            password = new TextBox();
            view = new Button();
            dataGridView1 = new DataGridView();
            employee = new Button();
            uploadattlog = new Button();
            processDtr = new Button();
            btnpayrollpost = new Button();
            rateConfig = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // add
            // 
            add.Location = new Point(13, 236);
            add.Margin = new Padding(3, 2, 3, 2);
            add.Name = "add";
            add.Size = new Size(82, 22);
            add.TabIndex = 1;
            add.Text = "ADD USER";
            add.UseVisualStyleBackColor = true;
            add.Click += add_Click;
            // 
            // search
            // 
            search.Location = new Point(101, 236);
            search.Margin = new Padding(3, 2, 3, 2);
            search.Name = "search";
            search.Size = new Size(82, 22);
            search.TabIndex = 2;
            search.Text = "SEARCH";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // delete
            // 
            delete.Location = new Point(188, 236);
            delete.Margin = new Padding(3, 2, 3, 2);
            delete.Name = "delete";
            delete.Size = new Size(82, 22);
            delete.TabIndex = 3;
            delete.Text = "DELETE";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // update
            // 
            update.Location = new Point(276, 236);
            update.Margin = new Padding(3, 2, 3, 2);
            update.Name = "update";
            update.Size = new Size(82, 22);
            update.TabIndex = 4;
            update.Text = "UPDATE";
            update.UseVisualStyleBackColor = true;
            update.Click += update_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(27, 140);
            label1.Name = "label1";
            label1.Size = new Size(92, 21);
            label1.TabIndex = 5;
            label1.Text = "USERNAME";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(30, 182);
            label2.Name = "label2";
            label2.Size = new Size(94, 21);
            label2.TabIndex = 6;
            label2.Text = "PASSWORD";
            // 
            // username
            // 
            username.Location = new Point(161, 141);
            username.Margin = new Padding(3, 2, 3, 2);
            username.Name = "username";
            username.Size = new Size(110, 23);
            username.TabIndex = 7;
            // 
            // password
            // 
            password.Location = new Point(161, 185);
            password.Margin = new Padding(3, 2, 3, 2);
            password.Name = "password";
            password.Size = new Size(110, 23);
            password.TabIndex = 8;
            // 
            // view
            // 
            view.Location = new Point(363, 236);
            view.Margin = new Padding(3, 2, 3, 2);
            view.Name = "view";
            view.Size = new Size(82, 22);
            view.TabIndex = 9;
            view.Text = "VIEW";
            view.UseVisualStyleBackColor = true;
            view.Click += view_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(393, 74);
            dataGridView1.Margin = new Padding(3, 2, 3, 2);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(262, 141);
            dataGridView1.TabIndex = 10;
            // 
            // employee
            // 
            employee.Location = new Point(451, 236);
            employee.Margin = new Padding(3, 2, 3, 2);
            employee.Name = "employee";
            employee.Size = new Size(82, 22);
            employee.TabIndex = 11;
            employee.Text = "EMPLOYEE";
            employee.UseVisualStyleBackColor = true;
            employee.Click += button1_Click;
            // 
            // uploadattlog
            // 
            uploadattlog.Location = new Point(538, 236);
            uploadattlog.Margin = new Padding(3, 2, 3, 2);
            uploadattlog.Name = "uploadattlog";
            uploadattlog.Size = new Size(82, 22);
            uploadattlog.TabIndex = 12;
            uploadattlog.Text = "UploadATT";
            uploadattlog.UseVisualStyleBackColor = true;
            uploadattlog.Click += uploadattlog_Click;
            // 
            // processDtr
            // 
            processDtr.Location = new Point(13, 285);
            processDtr.Margin = new Padding(3, 2, 3, 2);
            processDtr.Name = "processDtr";
            processDtr.Size = new Size(115, 22);
            processDtr.TabIndex = 13;
            processDtr.Text = "PROCESS DTR";
            processDtr.UseVisualStyleBackColor = true;
            processDtr.Click += processDtr_Click;
            // 
            // btnpayrollpost
            // 
            btnpayrollpost.Location = new Point(133, 285);
            btnpayrollpost.Margin = new Padding(3, 2, 3, 2);
            btnpayrollpost.Name = "btnpayrollpost";
            btnpayrollpost.Size = new Size(122, 22);
            btnpayrollpost.TabIndex = 14;
            btnpayrollpost.Text = "PAYROLL POSTING";
            btnpayrollpost.UseVisualStyleBackColor = true;
            btnpayrollpost.Click += btnpayrollpost_Click;
            // 
            // rateConfig
            // 
            rateConfig.Location = new Point(261, 285);
            rateConfig.Name = "rateConfig";
            rateConfig.Size = new Size(108, 23);
            rateConfig.TabIndex = 15;
            rateConfig.Text = "RATECONFIG";
            rateConfig.UseVisualStyleBackColor = true;
            rateConfig.Click += rateConfig_Click;
            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(rateConfig);
            Controls.Add(btnpayrollpost);
            Controls.Add(processDtr);
            Controls.Add(uploadattlog);
            Controls.Add(employee);
            Controls.Add(dataGridView1);
            Controls.Add(view);
            Controls.Add(password);
            Controls.Add(username);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(update);
            Controls.Add(delete);
            Controls.Add(search);
            Controls.Add(add);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Admin";
            Text = "Admin";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button add;
        private Button search;
        private Button delete;
        private Button update;
        private Label label1;
        private Label label2;
        private TextBox username;
        private TextBox password;
        private Button view;
        private DataGridView dataGridView1;
        private Button employee;
        private Button uploadattlog;
        private Button processDtr;
        private Button btnpayrollpost;
        private Button rateConfig;
    }
}