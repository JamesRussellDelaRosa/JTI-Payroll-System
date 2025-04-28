namespace JTI_Payroll_System
{
    partial class processDTR
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
            textStartDate = new TextBox();
            textEndDate = new TextBox();
            label1 = new Label();
            label2 = new Label();
            filter = new Button();
            dgvDTR = new DataGridView();
            textID = new Label();
            textName = new Label();
            btnBack = new Button();
            btnNext = new Button();
            btnSaveProcessedDTR = new Button();
            btnOpenDeleteDTR = new Button();
            btnAutoAssignShift = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvDTR).BeginInit();
            SuspendLayout();
            // 
            // textStartDate
            // 
            textStartDate.Location = new Point(67, 6);
            textStartDate.Name = "textStartDate";
            textStartDate.Size = new Size(125, 27);
            textStartDate.TabIndex = 0;
            // 
            // textEndDate
            // 
            textEndDate.Location = new Point(231, 6);
            textEndDate.Name = "textEndDate";
            textEndDate.Size = new Size(125, 27);
            textEndDate.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 2;
            label1.Text = "FROM";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(198, 9);
            label2.Name = "label2";
            label2.Size = new Size(27, 20);
            label2.TabIndex = 3;
            label2.Text = "TO";
            // 
            // filter
            // 
            filter.Location = new Point(362, 6);
            filter.Name = "filter";
            filter.Size = new Size(94, 29);
            filter.TabIndex = 4;
            filter.Text = "FILTER";
            filter.UseVisualStyleBackColor = true;
            filter.Click += filter_Click;
            // 
            // dgvDTR
            // 
            dgvDTR.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDTR.Location = new Point(12, 53);
            dgvDTR.Name = "dgvDTR";
            dgvDTR.RowHeadersWidth = 51;
            dgvDTR.Size = new Size(1738, 517);
            dgvDTR.TabIndex = 5;
            // 
            // textID
            // 
            textID.AutoSize = true;
            textID.Location = new Point(1301, 17);
            textID.Name = "textID";
            textID.Size = new Size(53, 20);
            textID.TabIndex = 6;
            textID.Text = "ID NO.";
            // 
            // textName
            // 
            textName.AutoSize = true;
            textName.Location = new Point(1376, 17);
            textName.Name = "textName";
            textName.Size = new Size(119, 20);
            textName.TabIndex = 7;
            textName.Text = "Employee Name";
            // 
            // btnBack
            // 
            btnBack.Location = new Point(460, 6);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(94, 29);
            btnBack.TabIndex = 8;
            btnBack.Text = "BACK";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(560, 6);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(94, 29);
            btnNext.TabIndex = 9;
            btnNext.Text = "NEXT";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnSaveProcessedDTR
            // 
            btnSaveProcessedDTR.Location = new Point(1192, 12);
            btnSaveProcessedDTR.Name = "btnSaveProcessedDTR";
            btnSaveProcessedDTR.Size = new Size(94, 29);
            btnSaveProcessedDTR.TabIndex = 10;
            btnSaveProcessedDTR.Text = "SAVE";
            btnSaveProcessedDTR.UseVisualStyleBackColor = true;
            btnSaveProcessedDTR.Click += btnSaveProcessedDTR_Click;
            // 
            // btnOpenDeleteDTR
            // 
            btnOpenDeleteDTR.Location = new Point(1033, 12);
            btnOpenDeleteDTR.Name = "btnOpenDeleteDTR";
            btnOpenDeleteDTR.Size = new Size(153, 29);
            btnOpenDeleteDTR.TabIndex = 11;
            btnOpenDeleteDTR.Text = "DELETE SAVED DTR";
            btnOpenDeleteDTR.UseVisualStyleBackColor = true;
            btnOpenDeleteDTR.Click += btnOpenDeleteDTR_Click;
            // 
            // btnAutoAssignShift
            // 
            btnAutoAssignShift.Location = new Point(820, 13);
            btnAutoAssignShift.Name = "btnAutoAssignShift";
            btnAutoAssignShift.Size = new Size(194, 29);
            btnAutoAssignShift.TabIndex = 12;
            btnAutoAssignShift.Text = "AUTO ASSIGN SHIFTCODE";
            btnAutoAssignShift.UseVisualStyleBackColor = true;
            btnAutoAssignShift.Click += btnAutoAssignShift_Click;
            // 
            // processDTR
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1762, 582);
            Controls.Add(btnAutoAssignShift);
            Controls.Add(btnOpenDeleteDTR);
            Controls.Add(btnSaveProcessedDTR);
            Controls.Add(btnNext);
            Controls.Add(btnBack);
            Controls.Add(textName);
            Controls.Add(textID);
            Controls.Add(dgvDTR);
            Controls.Add(filter);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textEndDate);
            Controls.Add(textStartDate);
            Name = "processDTR";
            Text = "processDTR";
            ((System.ComponentModel.ISupportInitialize)dgvDTR).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textStartDate;
        private TextBox textEndDate;
        private Label label1;
        private Label label2;
        private Button filter;
        private DataGridView dgvDTR;
        private Label textID;
        private Label textName;
        private Button btnBack;
        private Button btnNext;
        private Button btnSaveProcessedDTR;
        private Button btnOpenDeleteDTR;
        private Button btnAutoAssignShift;
    }
}