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
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            SuspendLayout();
            // 
            // add
            // 
            add.Location = new Point(169, 310);
            add.Name = "add";
            add.Size = new Size(94, 29);
            add.TabIndex = 1;
            add.Text = "ADD USER";
            add.UseVisualStyleBackColor = true;
            add.Click += add_Click;
            // 
            // search
            // 
            search.Location = new Point(269, 310);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 2;
            search.Text = "SEARCH";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // delete
            // 
            delete.Location = new Point(369, 310);
            delete.Name = "delete";
            delete.Size = new Size(94, 29);
            delete.TabIndex = 3;
            delete.Text = "DELETE";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // update
            // 
            update.Location = new Point(469, 310);
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
            label1.Location = new Point(169, 151);
            label1.Name = "label1";
            label1.Size = new Size(115, 28);
            label1.TabIndex = 5;
            label1.Text = "USERNAME";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(172, 207);
            label2.Name = "label2";
            label2.Size = new Size(117, 28);
            label2.TabIndex = 6;
            label2.Text = "PASSWORD";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(322, 152);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 7;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(322, 211);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(125, 27);
            textBox2.TabIndex = 8;
            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(update);
            Controls.Add(delete);
            Controls.Add(search);
            Controls.Add(add);
            Name = "Admin";
            Text = "Admin";
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
        private TextBox textBox1;
        private TextBox textBox2;
    }
}