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
            sssgovdues = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // add
            // 
            add.Location = new Point(15, 315);
            add.Name = "add";
            add.Size = new Size(94, 29);
            add.TabIndex = 1;
            add.Text = "ADD USER";
            add.UseVisualStyleBackColor = true;
            add.Click += add_Click;
            // 
            // search
            // 
            search.Location = new Point(115, 315);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 2;
            search.Text = "SEARCH";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // delete
            // 
            delete.Location = new Point(215, 315);
            delete.Name = "delete";
            delete.Size = new Size(94, 29);
            delete.TabIndex = 3;
            delete.Text = "DELETE";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // update
            // 
            update.Location = new Point(315, 315);
            update.Name = "update";
            update.Size = new Size(94, 29);
            update.TabIndex = 4;
            update.Text = "UPDATE";
            update.UseVisualStyleBackColor = true;
            update.Click += update_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(31, 187);
            label1.Name = "label1";
            label1.Size = new Size(115, 28);
            label1.TabIndex = 5;
            label1.Text = "USERNAME";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(34, 243);
            label2.Name = "label2";
            label2.Size = new Size(117, 28);
            label2.TabIndex = 6;
            label2.Text = "PASSWORD";
            // 
            // username
            // 
            username.Location = new Point(184, 188);
            username.Name = "username";
            username.Size = new Size(125, 27);
            username.TabIndex = 7;
            // 
            // password
            // 
            password.Location = new Point(184, 247);
            password.Name = "password";
            password.Size = new Size(125, 27);
            password.TabIndex = 8;
            // 
            // view
            // 
            view.Location = new Point(415, 315);
            view.Name = "view";
            view.Size = new Size(94, 29);
            view.TabIndex = 9;
            view.Text = "VIEW";
            view.UseVisualStyleBackColor = true;
            view.Click += view_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(449, 99);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(299, 188);
            dataGridView1.TabIndex = 10;
            // 
            // employee
            // 
            employee.Location = new Point(515, 315);
            employee.Name = "employee";
            employee.Size = new Size(94, 29);
            employee.TabIndex = 11;
            employee.Text = "EMPLOYEE";
            employee.UseVisualStyleBackColor = true;
            employee.Click += button1_Click;
            // 
            // uploadattlog
            // 
            uploadattlog.Location = new Point(615, 315);
            uploadattlog.Name = "uploadattlog";
            uploadattlog.Size = new Size(94, 29);
            uploadattlog.TabIndex = 12;
            uploadattlog.Text = "UploadATT";
            uploadattlog.UseVisualStyleBackColor = true;
            uploadattlog.Click += uploadattlog_Click;
            // 
            // processDtr
            // 
            processDtr.Location = new Point(15, 380);
            processDtr.Name = "processDtr";
            processDtr.Size = new Size(131, 29);
            processDtr.TabIndex = 13;
            processDtr.Text = "PROCESS DTR";
            processDtr.UseVisualStyleBackColor = true;
            processDtr.Click += processDtr_Click;
            // 
            // btnpayrollpost
            // 
            btnpayrollpost.Location = new Point(152, 380);
            btnpayrollpost.Name = "btnpayrollpost";
            btnpayrollpost.Size = new Size(139, 29);
            btnpayrollpost.TabIndex = 14;
            btnpayrollpost.Text = "PAYROLL POSTING";
            btnpayrollpost.UseVisualStyleBackColor = true;
            btnpayrollpost.Click += btnpayrollpost_Click;
            // 
            // rateConfig
            // 
            rateConfig.Location = new Point(298, 380);
            rateConfig.Margin = new Padding(3, 4, 3, 4);
            rateConfig.Name = "rateConfig";
            rateConfig.Size = new Size(123, 31);
            rateConfig.TabIndex = 15;
            rateConfig.Text = "RATECONFIG";
            rateConfig.UseVisualStyleBackColor = true;
            rateConfig.Click += rateConfig_Click;
            // 
            // sssgovdues
            // 
            sssgovdues.Location = new Point(427, 380);
            sssgovdues.Name = "sssgovdues";
            sssgovdues.Size = new Size(166, 29);
            sssgovdues.TabIndex = 16;
            sssgovdues.Text = "SSS CONTRIB TABLE";
            sssgovdues.UseVisualStyleBackColor = true;
            sssgovdues.Click += sssgovdues_Click;
            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 451);
            Controls.Add(sssgovdues);
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
        private Button sssgovdues;
    }
}