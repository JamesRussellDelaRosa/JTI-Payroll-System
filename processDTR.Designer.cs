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
            topPanel = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dgvDTR).BeginInit();
            SuspendLayout();
            // 
            // topPanel
            // 
            topPanel.ColumnCount = 10;
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // labelDateRange
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // btnBack
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // btnNext
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // search
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // btnAutoAssignShift
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // btnOpenDeleteDTR
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // btnSaveProcessedDTR
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // textID
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // textName
            topPanel.Dock = DockStyle.Top;
            topPanel.RowCount = 1;
            topPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            // Add controls to topPanel
            topPanel.Controls.Add(labelDateRange, 0, 0);
            topPanel.Controls.Add(btnBack, 1, 0);
            topPanel.Controls.Add(btnNext, 2, 0);
            topPanel.Controls.Add(search, 3, 0);
            topPanel.Controls.Add(btnAutoAssignShift, 4, 0);
            topPanel.Controls.Add(btnOpenDeleteDTR, 5, 0);
            topPanel.Controls.Add(btnSaveProcessedDTR, 6, 0);
            topPanel.Controls.Add(textID, 7, 0);
            topPanel.Controls.Add(textName, 8, 0);
            // 
            // labelDateRange
            // 
            labelDateRange.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelDateRange.Text = "Date Range: MM/DD/YYYY - MM/DD/YYYY";
            labelDateRange.Anchor = AnchorStyles.Left;
            labelDateRange.AutoSize = true;
            // 
            // btnBack, btnNext, search, btnAutoAssignShift, btnOpenDeleteDTR, btnSaveProcessedDTR
            // 
            btnBack.Text = "BACK";
            btnBack.Click += btnBack_Click;
            btnNext.Text = "NEXT";
            btnNext.Click += btnNext_Click;
            search.Text = "SEARCH";
            btnAutoAssignShift.Text = "AUTO ASSIGN SHIFTCODE";
            btnAutoAssignShift.Click += btnAutoAssignShift_Click;
            btnOpenDeleteDTR.Text = "DELETE SAVED DTR";
            btnOpenDeleteDTR.Click += btnOpenDeleteDTR_Click;
            btnSaveProcessedDTR.Text = "SAVE";
            btnSaveProcessedDTR.Click += btnSaveProcessedDTR_Click;
            // 
            // textID
            // 
            textID.Text = "ID NO.";
            textID.Anchor = AnchorStyles.Left;
            textID.AutoSize = true;
            // 
            // textName
            // 
            textName.Text = "Employee Name";
            textName.Anchor = AnchorStyles.Left;
            textName.AutoSize = true;
            // 
            // dgvDTR
            // 
            dgvDTR.Dock = DockStyle.Fill;
            dgvDTR.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDTR.RowHeadersWidth = 51;
            dgvDTR.TabIndex = 5;
            // 
            // flowCcodePanels
            // 
            flowCcodePanels.Dock = DockStyle.Bottom;
            flowCcodePanels.AutoScroll = true;
            flowCcodePanels.Height = 60;
            flowCcodePanels.TabIndex = 14;
            // 
            // processDTR
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1762, 650);
            Controls.Add(dgvDTR);
            Controls.Add(topPanel);
            Controls.Add(flowCcodePanels);
            MinimumSize = new Size(800, 600);
            Name = "processDTR";
            Text = "processDTR";
            this.Resize += processDTR_Resize;
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
        private TableLayoutPanel topPanel;
    }
}