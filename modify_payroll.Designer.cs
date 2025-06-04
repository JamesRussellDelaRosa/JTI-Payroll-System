namespace JTI_Payroll_System
{
    partial class modify_payroll
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.TextBox txtFromDate;
        private System.Windows.Forms.TextBox txtToDate;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.Label lblPayrollYear;
        private System.Windows.Forms.Label lblControlPeriod;
        private System.Windows.Forms.TextBox txtMonth;
        private System.Windows.Forms.TextBox txtPayrollYear;
        private System.Windows.Forms.TextBox txtControlPeriod;

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
            lblFromDate = new Label();
            lblToDate = new Label();
            txtFromDate = new TextBox();
            txtToDate = new TextBox();
            btnModify = new Button();
            lblMonth = new Label();
            lblPayrollYear = new Label();
            lblControlPeriod = new Label();
            txtMonth = new TextBox();
            txtPayrollYear = new TextBox();
            txtControlPeriod = new TextBox();
            SuspendLayout();
            // 
            // lblFromDate
            // 
            lblFromDate.Location = new Point(45, 124);
            lblFromDate.Name = "lblFromDate";
            lblFromDate.Size = new Size(49, 23);
            lblFromDate.TabIndex = 0;
            lblFromDate.Text = "From Date:";
            // 
            // lblToDate
            // 
            lblToDate.Location = new Point(223, 123);
            lblToDate.Name = "lblToDate";
            lblToDate.Size = new Size(32, 23);
            lblToDate.TabIndex = 2;
            lblToDate.Text = "To Date:";
            // 
            // txtFromDate
            // 
            txtFromDate.Location = new Point(100, 121);
            txtFromDate.Name = "txtFromDate";
            txtFromDate.Size = new Size(120, 27);
            txtFromDate.TabIndex = 1;
            // 
            // txtToDate
            // 
            txtToDate.Location = new Point(261, 120);
            txtToDate.Name = "txtToDate";
            txtToDate.Size = new Size(120, 27);
            txtToDate.TabIndex = 3;
            // 
            // btnModify
            // 
            btnModify.Location = new Point(206, 283);
            btnModify.Name = "btnModify";
            btnModify.Size = new Size(100, 29);
            btnModify.TabIndex = 4;
            btnModify.Text = "MODIFY";
            btnModify.UseVisualStyleBackColor = true;
            btnModify.Click += btnModify_Click;
            // 
            // lblMonth
            // 
            lblMonth.Location = new Point(180, 190);
            lblMonth.Name = "lblMonth";
            lblMonth.Size = new Size(60, 23);
            lblMonth.TabIndex = 5;
            lblMonth.Text = "Month:";
            // 
            // lblPayrollYear
            // 
            lblPayrollYear.Location = new Point(57, 222);
            lblPayrollYear.Name = "lblPayrollYear";
            lblPayrollYear.Size = new Size(90, 23);
            lblPayrollYear.TabIndex = 7;
            lblPayrollYear.Text = "Payroll Year:";
            // 
            // lblControlPeriod
            // 
            lblControlPeriod.Location = new Point(255, 231);
            lblControlPeriod.Name = "lblControlPeriod";
            lblControlPeriod.Size = new Size(110, 23);
            lblControlPeriod.TabIndex = 9;
            lblControlPeriod.Text = "Control Period:";
            // 
            // txtMonth
            // 
            txtMonth.Location = new Point(246, 190);
            txtMonth.Name = "txtMonth";
            txtMonth.Size = new Size(60, 27);
            txtMonth.TabIndex = 6;
            // 
            // txtPayrollYear
            // 
            txtPayrollYear.Location = new Point(147, 222);
            txtPayrollYear.Name = "txtPayrollYear";
            txtPayrollYear.Size = new Size(80, 27);
            txtPayrollYear.TabIndex = 8;
            // 
            // txtControlPeriod
            // 
            txtControlPeriod.Location = new Point(371, 231);
            txtControlPeriod.Name = "txtControlPeriod";
            txtControlPeriod.Size = new Size(60, 27);
            txtControlPeriod.TabIndex = 10;
            // 
            // modify_payroll
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(647, 435);
            Controls.Add(lblFromDate);
            Controls.Add(txtFromDate);
            Controls.Add(lblToDate);
            Controls.Add(txtToDate);
            Controls.Add(btnModify);
            Controls.Add(lblMonth);
            Controls.Add(txtMonth);
            Controls.Add(lblPayrollYear);
            Controls.Add(txtPayrollYear);
            Controls.Add(lblControlPeriod);
            Controls.Add(txtControlPeriod);
            Name = "modify_payroll";
            Text = "Modify Payroll";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
