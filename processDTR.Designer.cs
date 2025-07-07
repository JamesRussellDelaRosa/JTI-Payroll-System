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
        /// Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelDateRange = new Label();
            dgvDTR = new DataGridView();
            textID = new Label();
            textName = new Label();
            btnBack = new Button();
            btnNext = new Button();
            btnSaveProcessedDTR = new Button();
            btnOpenDeleteDTR = new Button();
            btnAutoAssignShift = new Button();
            search = new Button();
            flowCcodePanels = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dgvDTR).BeginInit();
            SuspendLayout();
            // 
            // labelDateRange
            // 
            labelDateRange.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelDateRange.Location = new Point(12, 9);
            labelDateRange.Name = "labelDateRange";
            labelDateRange.Size = new Size(350, 20);
            labelDateRange.TabIndex = 2;
            labelDateRange.Text = "Date Range: MM/DD/YYYY - MM/DD/YYYY";
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
            // search
            // 
            search.Location = new Point(660, 6);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 13;
            search.Text = "SEARCH";
            search.UseVisualStyleBackColor = true;
            // 
            // flowCcodePanels
            // 
            flowCcodePanels.AutoScroll = true;
            flowCcodePanels.Location = new Point(12, 580);
            flowCcodePanels.Name = "flowCcodePanels";
            flowCcodePanels.Size = new Size(600, 60);
            flowCcodePanels.TabIndex = 14;
            // 
            // processDTR
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1762, 650);
            Controls.Add(labelDateRange);
            Controls.Add(flowCcodePanels);
            Controls.Add(search);
            Controls.Add(btnAutoAssignShift);
            Controls.Add(btnOpenDeleteDTR);
            Controls.Add(btnSaveProcessedDTR);
            Controls.Add(btnNext);
            Controls.Add(btnBack);
            Controls.Add(textName);
            Controls.Add(textID);
            Controls.Add(dgvDTR);
            Name = "processDTR";
            Text = "processDTR";
            ((System.ComponentModel.ISupportInitialize)dgvDTR).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelDateRange;
        private DataGridView dgvDTR;
        private Label textID;
        private Label textName;
        private Button btnBack;
        private Button btnNext;
        private Button btnSaveProcessedDTR;
        private Button btnOpenDeleteDTR;
        private Button btnAutoAssignShift;
        private Button search;
        private FlowLayoutPanel flowCcodePanels;
    }
}