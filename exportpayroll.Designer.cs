namespace JTI_Payroll_System
{
    partial class exportpayroll
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
            label5 = new Label();
            todate = new TextBox();
            label4 = new Label();
            fromdate = new TextBox();
            export = new Button();
            postedpayroll = new CheckBox();
            overallpayroll = new CheckBox();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(385, 30);
            label5.Name = "label5";
            label5.Size = new Size(25, 20);
            label5.TabIndex = 13;
            label5.Text = "To";
            // 
            // todate
            // 
            todate.Location = new Point(416, 27);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(205, 30);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 11;
            label4.Text = "From";
            label4.Click += label4_Click;
            // 
            // fromdate
            // 
            fromdate.Location = new Point(254, 27);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 10;
            // 
            // export
            // 
            export.Location = new Point(285, 194);
            export.Name = "export";
            export.Size = new Size(193, 29);
            export.TabIndex = 14;
            export.Text = "EXPORT";
            export.UseVisualStyleBackColor = true;
            export.Click += export_Click;
            // 
            // postedpayroll
            // 
            postedpayroll.AutoSize = true;
            postedpayroll.Location = new Point(173, 128);
            postedpayroll.Name = "postedpayroll";
            postedpayroll.Size = new Size(147, 24);
            postedpayroll.TabIndex = 15;
            postedpayroll.Text = "POSTED PAYROLL";
            postedpayroll.UseVisualStyleBackColor = true;
            // 
            // overallpayroll
            // 
            overallpayroll.AutoSize = true;
            overallpayroll.Location = new Point(341, 128);
            overallpayroll.Name = "overallpayroll";
            overallpayroll.Size = new Size(150, 24);
            overallpayroll.TabIndex = 16;
            overallpayroll.Text = "OVERALLPAYROLL";
            overallpayroll.UseVisualStyleBackColor = true;
            // 
            // exportpayroll
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(729, 275);
            Controls.Add(overallpayroll);
            Controls.Add(postedpayroll);
            Controls.Add(export);
            Controls.Add(label5);
            Controls.Add(todate);
            Controls.Add(label4);
            Controls.Add(fromdate);
            Name = "exportpayroll";
            Text = "exportpayroll";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label5;
        private TextBox todate;
        private Label label4;
        private TextBox fromdate;
        private Button export;
        private CheckBox postedpayroll;
        private CheckBox overallpayroll;
    }
}