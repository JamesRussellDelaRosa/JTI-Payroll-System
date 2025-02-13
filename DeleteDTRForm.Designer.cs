namespace JTI_Payroll_System
{
    partial class DeleteDTRForm
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
            dgvAvailableEmployees = new DataGridView();
            btnDeleteDTR = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvAvailableEmployees).BeginInit();
            SuspendLayout();
            // 
            // dgvAvailableEmployees
            // 
            dgvAvailableEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAvailableEmployees.Location = new Point(12, 92);
            dgvAvailableEmployees.Name = "dgvAvailableEmployees";
            dgvAvailableEmployees.RowHeadersWidth = 51;
            dgvAvailableEmployees.Size = new Size(1011, 460);
            dgvAvailableEmployees.TabIndex = 0;
            // 
            // btnDeleteDTR
            // 
            btnDeleteDTR.Location = new Point(249, 35);
            btnDeleteDTR.Name = "btnDeleteDTR";
            btnDeleteDTR.Size = new Size(94, 29);
            btnDeleteDTR.TabIndex = 1;
            btnDeleteDTR.Text = "DELETE";
            btnDeleteDTR.UseVisualStyleBackColor = true;
            btnDeleteDTR.Click += btnDeleteDTR_Click;
            // 
            // DeleteDTRForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1035, 564);
            Controls.Add(btnDeleteDTR);
            Controls.Add(dgvAvailableEmployees);
            Name = "DeleteDTRForm";
            Text = "DeleteDTRForm";
            ((System.ComponentModel.ISupportInitialize)dgvAvailableEmployees).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvAvailableEmployees;
        private Button btnDeleteDTR;
    }
}