namespace JTI_Payroll_System
{
    partial class autodeductssshdmfloan
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
            label1 = new Label();
            month = new TextBox();
            year = new TextBox();
            label2 = new Label();
            controlperiod = new TextBox();
            label3 = new Label();
            fromdate = new TextBox();
            label5 = new Label();
            todate = new TextBox();
            label6 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(39, 124);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 0;
            label1.Text = "Month";
            // 
            // month
            // 
            month.Location = new Point(97, 124);
            month.Name = "month";
            month.Size = new Size(40, 27);
            month.TabIndex = 1;
            // 
            // year
            // 
            year.Location = new Point(186, 125);
            year.Name = "year";
            year.Size = new Size(98, 27);
            year.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(143, 128);
            label2.Name = "label2";
            label2.Size = new Size(37, 20);
            label2.TabIndex = 2;
            label2.Text = "Year";
            // 
            // controlperiod
            // 
            controlperiod.Location = new Point(396, 125);
            controlperiod.Name = "controlperiod";
            controlperiod.Size = new Size(40, 27);
            controlperiod.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(290, 127);
            label3.Name = "label3";
            label3.Size = new Size(100, 20);
            label3.TabIndex = 4;
            label3.Text = "ControlPeriod";
            // 
            // fromdate
            // 
            fromdate.Location = new Point(88, 201);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(98, 27);
            fromdate.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(39, 204);
            label5.Name = "label5";
            label5.Size = new Size(43, 20);
            label5.TabIndex = 8;
            label5.Text = "From";
            // 
            // todate
            // 
            todate.Location = new Point(223, 201);
            todate.Name = "todate";
            todate.Size = new Size(98, 27);
            todate.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(192, 204);
            label6.Name = "label6";
            label6.Size = new Size(25, 20);
            label6.TabIndex = 10;
            label6.Text = "To";
            // 
            // button1
            // 
            button1.Location = new Point(211, 295);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 12;
            button1.Text = "Process";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // autodeductssshdmfloan
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(548, 414);
            Controls.Add(button1);
            Controls.Add(todate);
            Controls.Add(label6);
            Controls.Add(fromdate);
            Controls.Add(label5);
            Controls.Add(controlperiod);
            Controls.Add(label3);
            Controls.Add(year);
            Controls.Add(label2);
            Controls.Add(month);
            Controls.Add(label1);
            Name = "autodeductssshdmfloan";
            Text = "autodeductssshdmfloan";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox month;
        private TextBox year;
        private Label label2;
        private TextBox controlperiod;
        private Label label3;
        private TextBox fromdate;
        private Label label5;
        private TextBox todate;
        private Label label6;
        private Button button1;
    }
}