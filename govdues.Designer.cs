namespace JTI_Payroll_System
{
    partial class govdues
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
            btngovdues = new Button();
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
            // btngovdues
            // 
            btngovdues.Location = new Point(342, 318);
            btngovdues.Name = "btngovdues";
            btngovdues.Size = new Size(193, 29);
            btngovdues.TabIndex = 22;
            btngovdues.Text = "POST GOV DUES";
            btngovdues.UseVisualStyleBackColor = true;
            btngovdues.Click += btngovdues_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(435, 228);
            label5.Name = "label5";
            label5.Size = new Size(25, 20);
            label5.TabIndex = 21;
            label5.Text = "To";
            // 
            // todate
            // 
            todate.Location = new Point(466, 225);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 20;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(255, 228);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 19;
            label4.Text = "From";
            // 
            // fromdate
            // 
            fromdate.Location = new Point(304, 225);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 18;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(342, 95);
            label3.Name = "label3";
            label3.Size = new Size(104, 20);
            label3.TabIndex = 17;
            label3.Text = "Control Period";
            // 
            // controlPeriod
            // 
            controlPeriod.Location = new Point(452, 92);
            controlPeriod.Name = "controlPeriod";
            controlPeriod.Size = new Size(32, 27);
            controlPeriod.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(213, 150);
            label2.Name = "label2";
            label2.Size = new Size(85, 20);
            label2.TabIndex = 15;
            label2.Text = "Payroll year";
            // 
            // payrollyear
            // 
            payrollyear.Location = new Point(304, 147);
            payrollyear.Name = "payrollyear";
            payrollyear.Size = new Size(125, 27);
            payrollyear.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(178, 95);
            label1.Name = "label1";
            label1.Size = new Size(120, 20);
            label1.TabIndex = 13;
            label1.Text = "For the Month of";
            // 
            // month
            // 
            month.Location = new Point(304, 92);
            month.Name = "month";
            month.Size = new Size(32, 27);
            month.TabIndex = 12;
            // 
            // govdues
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btngovdues);
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
            Name = "govdues";
            Text = "govdues";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btngovdues;
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