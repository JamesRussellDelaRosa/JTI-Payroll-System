namespace JTI_Payroll_System
{
    partial class shiftcode
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
            dgvShiftCodes = new DataGridView();
            load = new Button();
            buttonSave = new Button();
            buttonNew = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvShiftCodes).BeginInit();
            SuspendLayout();
            // 
            // dgvShiftCodes
            // 
            dgvShiftCodes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvShiftCodes.Location = new Point(12, 68);
            dgvShiftCodes.Name = "dgvShiftCodes";
            dgvShiftCodes.RowHeadersWidth = 51;
            dgvShiftCodes.Size = new Size(776, 370);
            dgvShiftCodes.TabIndex = 0;
            // 
            // load
            // 
            load.Location = new Point(12, 21);
            load.Name = "load";
            load.Size = new Size(94, 29);
            load.TabIndex = 1;
            load.Text = "LOAD";
            load.UseVisualStyleBackColor = true;
            load.Click += load_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(112, 21);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(94, 29);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "SAVE";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonNew
            // 
            buttonNew.Location = new Point(212, 21);
            buttonNew.Name = "buttonNew";
            buttonNew.Size = new Size(94, 29);
            buttonNew.TabIndex = 3;
            buttonNew.Text = "NEW";
            buttonNew.UseVisualStyleBackColor = true;
            buttonNew.Click += buttonNew_Click;
            // 
            // shiftcode
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonNew);
            Controls.Add(buttonSave);
            Controls.Add(load);
            Controls.Add(dgvShiftCodes);
            Name = "shiftcode";
            Text = "shiftcode";
            ((System.ComponentModel.ISupportInitialize)dgvShiftCodes).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvShiftCodes;
        private Button load;
        private Button buttonSave;
        private Button buttonNew;
    }
}