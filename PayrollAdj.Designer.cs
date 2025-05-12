namespace JTI_Payroll_System
{
    partial class PayrollAdj
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
            label2 = new Label();
            label1 = new Label();
            textEndDate = new TextBox();
            textStartDate = new TextBox();
            filter = new Button();
            dgvDTR = new DataGridView();
            post = new Button();
            refresh = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvDTR).BeginInit();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(208, 15);
            label2.Name = "label2";
            label2.Size = new Size(27, 20);
            label2.TabIndex = 7;
            label2.Text = "TO";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 15);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 6;
            label1.Text = "FROM";
            // 
            // textEndDate
            // 
            textEndDate.Location = new Point(241, 12);
            textEndDate.Name = "textEndDate";
            textEndDate.Size = new Size(125, 27);
            textEndDate.TabIndex = 5;
            // 
            // textStartDate
            // 
            textStartDate.Location = new Point(77, 12);
            textStartDate.Name = "textStartDate";
            textStartDate.Size = new Size(125, 27);
            textStartDate.TabIndex = 4;
            // 
            // filter
            // 
            filter.Location = new Point(385, 11);
            filter.Name = "filter";
            filter.Size = new Size(94, 29);
            filter.TabIndex = 8;
            filter.Text = "FILTER";
            filter.UseVisualStyleBackColor = true;
            filter.Click += filter_Click;
            // 
            // dgvDTR
            // 
            dgvDTR.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDTR.Location = new Point(12, 47);
            dgvDTR.Name = "dgvDTR";
            dgvDTR.RowHeadersWidth = 51;
            dgvDTR.Size = new Size(1493, 546);
            dgvDTR.TabIndex = 9;
            // 
            // post
            // 
            post.Location = new Point(485, 12);
            post.Name = "post";
            post.Size = new Size(94, 29);
            post.TabIndex = 10;
            post.Text = "POST";
            post.UseVisualStyleBackColor = true;
            post.Click += post_Click;
            // 
            // refresh
            // 
            refresh.Location = new Point(585, 12);
            refresh.Name = "refresh";
            refresh.Size = new Size(94, 29);
            refresh.TabIndex = 11;
            refresh.Text = "REFRESH";
            refresh.UseVisualStyleBackColor = true;
            refresh.Click += refresh_Click;
            // 
            // PayrollAdj
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1517, 605);
            Controls.Add(refresh);
            Controls.Add(post);
            Controls.Add(dgvDTR);
            Controls.Add(filter);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textEndDate);
            Controls.Add(textStartDate);
            Name = "PayrollAdj";
            Text = "PayrollAdj";
            ((System.ComponentModel.ISupportInitialize)dgvDTR).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label1;
        private TextBox textEndDate;
        private TextBox textStartDate;
        private Button filter;
        private DataGridView dgvDTR;
        private Button post;
        private Button refresh;
    }
}