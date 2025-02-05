namespace JTI_Payroll_System
{
    partial class ProgressForm
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
            progressBar1 = new ProgressBar();
            progressLabel = new Label();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 54);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(639, 29);
            progressBar1.TabIndex = 0;
            // 
            // progressLabel
            // 
            progressLabel.AutoSize = true;
            progressLabel.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            progressLabel.Location = new Point(12, 9);
            progressLabel.Name = "progressLabel";
            progressLabel.Size = new Size(139, 31);
            progressLabel.TabIndex = 1;
            progressLabel.Text = "Processing...";
            progressLabel.Click += progressLabel_Click;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(663, 118);
            ControlBox = false;
            Controls.Add(progressLabel);
            Controls.Add(progressBar1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar1;
        private Label progressLabel;
    }
}