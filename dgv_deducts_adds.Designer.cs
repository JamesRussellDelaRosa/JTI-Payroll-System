namespace JTI_Payroll_System
{
    partial class dgv_deducts_adds
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
            dataGridView1 = new DataGridView();
            SaveButton = new Button();
            search = new Button();
            searchtxt = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 53);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1122, 447);
            dataGridView1.TabIndex = 0;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(0, 0);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(94, 29);
            SaveButton.TabIndex = 1;
            SaveButton.Text = "save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // search
            // 
            search.Location = new Point(1038, 10);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 72;
            search.Text = "Search";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // searchtxt
            // 
            searchtxt.Location = new Point(837, 12);
            searchtxt.Name = "searchtxt";
            searchtxt.Size = new Size(195, 27);
            searchtxt.TabIndex = 71;
            // 
            // dgv_deducts_adds
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1137, 512);
            Controls.Add(search);
            Controls.Add(searchtxt);
            Controls.Add(SaveButton);
            Controls.Add(dataGridView1);
            Name = "dgv_deducts_adds";
            Text = "Deductions and Additionals";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private DataGridView dataGridView1;
        private Button SaveButton;
        private Button search;
        private TextBox searchtxt;
    }
}