namespace JTI_Payroll_System
{
    partial class SSSTEST
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
            grossPay = new TextBox();
            compute = new Button();
            result = new TextBox();
            SuspendLayout();
            // 
            // grossPay
            // 
            grossPay.Location = new Point(21, 45);
            grossPay.Name = "grossPay";
            grossPay.Size = new Size(125, 27);
            grossPay.TabIndex = 0;
            // 
            // compute
            // 
            compute.Location = new Point(152, 45);
            compute.Name = "compute";
            compute.Size = new Size(94, 29);
            compute.TabIndex = 1;
            compute.Text = "compute";
            compute.UseVisualStyleBackColor = true;
            compute.Click += compute_Click;
            // 
            // result
            // 
            result.Location = new Point(252, 47);
            result.Name = "result";
            result.Size = new Size(125, 27);
            result.TabIndex = 2;
            // 
            // SSSTEST
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(402, 96);
            Controls.Add(result);
            Controls.Add(compute);
            Controls.Add(grossPay);
            Name = "SSSTEST";
            Text = "SSSTEST";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox grossPay;
        private Button compute;
        private TextBox result;
    }
}