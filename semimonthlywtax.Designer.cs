namespace JTI_Payroll_System
{
    partial class semimonthlywtax
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvTaxTable;
        private System.Windows.Forms.TextBox txtIncome;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblIncome;

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
            this.components = new System.ComponentModel.Container();
            this.dgvTaxTable = new System.Windows.Forms.DataGridView();
            this.txtIncome = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblIncome = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaxTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTaxTable
            // 
            this.dgvTaxTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvTaxTable.Height = 220;
            this.dgvTaxTable.AllowUserToAddRows = true;
            this.dgvTaxTable.AllowUserToDeleteRows = true;
            this.dgvTaxTable.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvTaxTable.Location = new System.Drawing.Point(0, 0);
            this.dgvTaxTable.Name = "dgvTaxTable";
            this.dgvTaxTable.Size = new System.Drawing.Size(800, 220);
            this.dgvTaxTable.TabIndex = 0;
            // 
            // lblIncome
            // 
            this.lblIncome.AutoSize = true;
            this.lblIncome.Location = new System.Drawing.Point(10, 240);
            this.lblIncome.Name = "lblIncome";
            this.lblIncome.Size = new System.Drawing.Size(47, 15);
            this.lblIncome.TabIndex = 1;
            this.lblIncome.Text = "Income:";
            // 
            // txtIncome
            // 
            this.txtIncome.Location = new System.Drawing.Point(70, 237);
            this.txtIncome.Name = "txtIncome";
            this.txtIncome.Size = new System.Drawing.Size(120, 23);
            this.txtIncome.TabIndex = 2;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(200, 235);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(110, 27);
            this.btnCalculate.TabIndex = 3;
            this.btnCalculate.Text = "Calculate Tax";
            this.btnCalculate.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(320, 235);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 27);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save Changes";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.ForeColor = System.Drawing.Color.Blue;
            this.lblResult.Location = new System.Drawing.Point(10, 270);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 15);
            this.lblResult.TabIndex = 5;
            // 
            // semimonthlywtax
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.txtIncome);
            this.Controls.Add(this.lblIncome);
            this.Controls.Add(this.dgvTaxTable);
            this.Name = "semimonthlywtax";
            this.Text = "semimonthlywtax";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaxTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}