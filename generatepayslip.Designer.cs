namespace JTI_Payroll_System
{
    partial class generatepayslip
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
            generate = new Button();
            label5 = new Label();
            todate = new TextBox();
            label4 = new Label();
            fromdate = new TextBox();
            label3 = new Label();
            controlPeriod = new TextBox();
            label2 = new Label();
            payrollyear = new TextBox();
            label1 = new Label();
            month = new TextBox();
            SuspendLayout();
            // 
            // generate
            // 
            generate.Location = new Point(358, 324);
            generate.Name = "generate";
            generate.Size = new Size(193, 29);
            generate.TabIndex = 34;
            generate.Text = "GENERATE";
            generate.UseVisualStyleBackColor = true;
            generate.Click += generate_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(451, 234);
            label5.Name = "label5";
            label5.Size = new Size(25, 20);
            label5.TabIndex = 33;
            label5.Text = "To";
            // 
            // todate
            // 
            todate.Location = new Point(482, 231);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 32;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(271, 234);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 31;
            label4.Text = "From";
            // 
            // fromdate
            // 
            fromdate.Location = new Point(320, 231);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 30;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(358, 101);
            label3.Name = "label3";
            label3.Size = new Size(104, 20);
            label3.TabIndex = 29;
            label3.Text = "Control Period";
            // 
            // controlPeriod
            // 
            controlPeriod.Location = new Point(468, 98);
            controlPeriod.Name = "controlPeriod";
            controlPeriod.Size = new Size(32, 27);
            controlPeriod.TabIndex = 28;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(229, 156);
            label2.Name = "label2";
            label2.Size = new Size(85, 20);
            label2.TabIndex = 27;
            label2.Text = "Payroll year";
            // 
            // payrollyear
            // 
            payrollyear.Location = new Point(320, 153);
            payrollyear.Name = "payrollyear";
            payrollyear.Size = new Size(125, 27);
            payrollyear.TabIndex = 26;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(194, 101);
            label1.Name = "label1";
            label1.Size = new Size(120, 20);
            label1.TabIndex = 25;
            label1.Text = "For the Month of";
            // 
            // month
            // 
            month.Location = new Point(320, 98);
            month.Name = "month";
            month.Size = new Size(32, 27);
            month.TabIndex = 24;
            // 
            // generatepayslip
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(generate);
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
            Name = "generatepayslip";
            Text = "generatepayslip";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button generate;
        private Label label5;
        private TextBox todate;
        private Label label4;
        private TextBox fromdate;
        private Label label3;
        private TextBox controlPeriod;
        private Label label2;
        private TextBox payrollyear;
        private Label label1;
        private TextBox month;
    }
}