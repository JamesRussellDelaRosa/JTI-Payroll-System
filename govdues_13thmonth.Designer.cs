namespace JTI_Payroll_System
{
    partial class govdues_13thmonth
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
            reprocessdata = new CheckBox();
            process = new Button();
            label5 = new Label();
            todate = new TextBox();
            label4 = new Label();
            fromdate = new TextBox();
            label3 = new Label();
            controlperiod_textbox = new TextBox();
            label2 = new Label();
            payrollyear_textbox = new TextBox();
            label1 = new Label();
            month_textbox = new TextBox();
            SuspendLayout();
            // 
            // reprocessdata
            // 
            reprocessdata.AutoSize = true;
            reprocessdata.Location = new Point(245, 327);
            reprocessdata.Name = "reprocessdata";
            reprocessdata.Size = new Size(109, 24);
            reprocessdata.TabIndex = 23;
            reprocessdata.Text = "REPROCESS";
            reprocessdata.UseVisualStyleBackColor = true;
            // 
            // process
            // 
            process.Location = new Point(358, 324);
            process.Name = "process";
            process.Size = new Size(193, 29);
            process.TabIndex = 22;
            process.Text = "PROCESS";
            process.UseVisualStyleBackColor = true;
            process.Click += process_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(451, 234);
            label5.Name = "label5";
            label5.Size = new Size(25, 20);
            label5.TabIndex = 21;
            label5.Text = "To";
            // 
            // todate
            // 
            todate.Location = new Point(482, 231);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 20;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(271, 234);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 19;
            label4.Text = "From";
            // 
            // fromdate
            // 
            fromdate.Location = new Point(320, 231);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 18;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(358, 101);
            label3.Name = "label3";
            label3.Size = new Size(104, 20);
            label3.TabIndex = 17;
            label3.Text = "Control Period";
            // 
            // controlperiod_textbox
            // 
            controlperiod_textbox.Location = new Point(468, 98);
            controlperiod_textbox.Name = "controlperiod_textbox";
            controlperiod_textbox.Size = new Size(32, 27);
            controlperiod_textbox.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(229, 156);
            label2.Name = "label2";
            label2.Size = new Size(85, 20);
            label2.TabIndex = 15;
            label2.Text = "Payroll year";
            // 
            // payrollyear_textbox
            // 
            payrollyear_textbox.Location = new Point(320, 153);
            payrollyear_textbox.Name = "payrollyear_textbox";
            payrollyear_textbox.Size = new Size(125, 27);
            payrollyear_textbox.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(194, 101);
            label1.Name = "label1";
            label1.Size = new Size(120, 20);
            label1.TabIndex = 13;
            label1.Text = "For the Month of";
            // 
            // month_textbox
            // 
            month_textbox.Location = new Point(320, 98);
            month_textbox.Name = "month_textbox";
            month_textbox.Size = new Size(32, 27);
            month_textbox.TabIndex = 12;
            // 
            // govdues_13thmonth
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(reprocessdata);
            Controls.Add(process);
            Controls.Add(label5);
            Controls.Add(todate);
            Controls.Add(label4);
            Controls.Add(fromdate);
            Controls.Add(label3);
            Controls.Add(controlperiod_textbox);
            Controls.Add(label2);
            Controls.Add(payrollyear_textbox);
            Controls.Add(label1);
            Controls.Add(month_textbox);
            Name = "govdues_13thmonth";
            Text = "govdues_13thmonth";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox reprocessdata;
        private Button process;
        private Label label5;
        private TextBox todate;
        private Label label4;
        private TextBox fromdate;
        private Label label3;
        private TextBox controlperiod_textbox;
        private Label label2;
        private TextBox payrollyear_textbox;
        private Label label1;
        private TextBox month_textbox;
    }
}