namespace JTI_Payroll_System
{
    partial class PayrollPost
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
            month = new TextBox();
            label1 = new Label();
            label2 = new Label();
            payrollyear = new TextBox();
            label3 = new Label();
            controlPeriod = new TextBox();
            label4 = new Label();
            fromdate = new TextBox();
            label5 = new Label();
            todate = new TextBox();
            btnSavePayroll = new Button();
            repost = new CheckBox();
            SuspendLayout();
            // 
            // month
            // 
            month.Location = new Point(163, 81);
            month.Name = "month";
            month.Size = new Size(32, 27);
            month.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 84);
            label1.Name = "label1";
            label1.Size = new Size(120, 20);
            label1.TabIndex = 1;
            label1.Text = "For the Month of";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(72, 139);
            label2.Name = "label2";
            label2.Size = new Size(85, 20);
            label2.TabIndex = 3;
            label2.Text = "Payroll year";
            // 
            // payrollyear
            // 
            payrollyear.Location = new Point(163, 136);
            payrollyear.Name = "payrollyear";
            payrollyear.Size = new Size(125, 27);
            payrollyear.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(201, 84);
            label3.Name = "label3";
            label3.Size = new Size(104, 20);
            label3.TabIndex = 5;
            label3.Text = "Control Period";
            // 
            // controlPeriod
            // 
            controlPeriod.Location = new Point(311, 81);
            controlPeriod.Name = "controlPeriod";
            controlPeriod.Size = new Size(32, 27);
            controlPeriod.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(114, 217);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 7;
            label4.Text = "From";
            // 
            // fromdate
            // 
            fromdate.Location = new Point(163, 214);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(294, 217);
            label5.Name = "label5";
            label5.Size = new Size(25, 20);
            label5.TabIndex = 9;
            label5.Text = "To";
            // 
            // todate
            // 
            todate.Location = new Point(325, 214);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 8;
            // 
            // btnSavePayroll
            // 
            btnSavePayroll.Location = new Point(201, 307);
            btnSavePayroll.Name = "btnSavePayroll";
            btnSavePayroll.Size = new Size(193, 29);
            btnSavePayroll.TabIndex = 10;
            btnSavePayroll.Text = "POST PAYROLL";
            btnSavePayroll.UseVisualStyleBackColor = true;
            btnSavePayroll.Click += btnSavePayroll_Click_1;
            // 
            // repost
            // 
            repost.AutoSize = true;
            repost.Location = new Point(112, 310);
            repost.Name = "repost";
            repost.Size = new Size(83, 24);
            repost.TabIndex = 11;
            repost.Text = "REPOST";
            repost.UseVisualStyleBackColor = true;
            repost.CheckedChanged += repost_CheckedChanged;
            // 
            // PayrollPost
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(repost);
            Controls.Add(btnSavePayroll);
            Controls.Add(label5);
            Controls.Add(todate);
            Controls.Add(label4);
            Controls.Add(fromdate);
            Controls.Add(label3);
            Controls.Add(controlPeriod);
            Controls.Add(label2);
            Controls.Add(payrollyear);
            Controls.Add(label1);
            Controls.Add(month);
            Name = "PayrollPost";
            Text = "PayrollPost";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox month;
        private Label label1;
        private Label label2;
        private TextBox payrollyear;
        private Label label3;
        private TextBox controlPeriod;
        private Label label4;
        private TextBox fromdate;
        private Label label5;
        private TextBox todate;
        private Button btnSavePayroll;
        private CheckBox repost;
    }
}