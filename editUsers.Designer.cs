namespace JTI_Payroll_System
{
    partial class editUsers
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
            dataGridView1 = new DataGridView();
            view = new Button();
            password = new TextBox();
            username = new TextBox();
            label2 = new Label();
            label1 = new Label();
            update = new Button();
            delete = new Button();
            search = new Button();
            add = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(468, 103);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(299, 188);
            dataGridView1.TabIndex = 20;
            // 
            // view
            // 
            view.Location = new Point(434, 319);
            view.Name = "view";
            view.Size = new Size(94, 29);
            view.TabIndex = 19;
            view.Text = "VIEW";
            view.UseVisualStyleBackColor = true;
            view.Click += view_Click;
            // 
            // password
            // 
            password.Location = new Point(203, 251);
            password.Name = "password";
            password.Size = new Size(125, 27);
            password.TabIndex = 18;
            // 
            // username
            // 
            username.Location = new Point(203, 192);
            username.Name = "username";
            username.Size = new Size(125, 27);
            username.TabIndex = 17;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(53, 247);
            label2.Name = "label2";
            label2.Size = new Size(117, 28);
            label2.TabIndex = 16;
            label2.Text = "PASSWORD";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(50, 191);
            label1.Name = "label1";
            label1.Size = new Size(115, 28);
            label1.TabIndex = 15;
            label1.Text = "USERNAME";
            // 
            // update
            // 
            update.Location = new Point(334, 319);
            update.Name = "update";
            update.Size = new Size(94, 29);
            update.TabIndex = 14;
            update.Text = "UPDATE";
            update.UseVisualStyleBackColor = true;
            update.Click += update_Click;
            // 
            // delete
            // 
            delete.Location = new Point(234, 319);
            delete.Name = "delete";
            delete.Size = new Size(94, 29);
            delete.TabIndex = 13;
            delete.Text = "DELETE";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // search
            // 
            search.Location = new Point(134, 319);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 12;
            search.Text = "SEARCH";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // add
            // 
            add.Location = new Point(34, 319);
            add.Name = "add";
            add.Size = new Size(94, 29);
            add.TabIndex = 11;
            add.Text = "ADD USER";
            add.UseVisualStyleBackColor = true;
            add.Click += add_Click;
            // 
            // editUsers
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
            Name = "editUsers";
            Text = "editUsers";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button view;
        private TextBox password;
        private TextBox username;
        private Label label2;
        private Label label1;
        private Button update;
        private Button delete;
        private Button search;
        private Button add;
    }
}