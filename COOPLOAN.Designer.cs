namespace JTI_Payroll_System
{
    partial class COOPLOAN
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
            upload = new Button();
            fromdate = new TextBox();
            todate = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // upload
            // 
            upload.Location = new Point(193, 176);
            upload.Name = "upload";
            upload.Size = new Size(236, 29);
            upload.TabIndex = 0;
            upload.Text = "UPLOAD COOP EXCEL FILE";
            upload.UseVisualStyleBackColor = true;
            upload.Click += upload_Click;
            // 
            // fromdate
            // 
            fromdate.Location = new Point(193, 110);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 1;
            // 
            // todate
            // 
            todate.Location = new Point(349, 111);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(142, 114);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 3;
            label1.Text = "FROM";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(320, 115);
            label2.Name = "label2";
            label2.Size = new Size(27, 20);
            label2.TabIndex = 4;
            label2.Text = "TO";
            // 
            // COOPLOAN
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(646, 301);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(todate);
            Controls.Add(fromdate);
            Controls.Add(upload);
            Margin = new Padding(4, 5, 4, 5);
            Name = "COOPLOAN";
            Text = "COOPLOAN";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Button upload;
        private TextBox fromdate;
        private TextBox todate;
        private Label label1;
        private Label label2;
    }
}
