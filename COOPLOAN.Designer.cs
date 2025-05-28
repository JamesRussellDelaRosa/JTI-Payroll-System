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
            SuspendLayout();
            // 
            // upload
            // 
            upload.Location = new Point(193, 74);
            upload.Name = "upload";
            upload.Size = new Size(236, 29);
            upload.TabIndex = 0;
            upload.Text = "UPLOAD COOP EXCEL FILE";
            upload.UseVisualStyleBackColor = true;
            upload.Click += upload_Click;
            // 
            // COOPLOAN
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(623, 182);
            Controls.Add(upload);
            Margin = new Padding(4, 5, 4, 5);
            Name = "COOPLOAN";
            Text = "COOPLOAN";
            ResumeLayout(false);

        }

        #endregion

        private Button upload;
    }
}
