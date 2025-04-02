namespace JTI_Payroll_System
{
    partial class sssgovdues
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
            load = new Button();
            save = new Button();
            dataGridView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // load
            // 
            load.Location = new Point(12, 12);
            load.Name = "load";
            load.Size = new Size(94, 29);
            load.TabIndex = 1;
            load.Text = "LOAD";
            load.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            save.Location = new Point(112, 12);
            save.Name = "save";
            save.Size = new Size(94, 29);
            save.TabIndex = 2;
            save.Text = "SAVE";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(12, 65);
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidth = 51;
            dataGridView.Size = new Size(1656, 785);
            dataGridView.TabIndex = 3;
            // 
            // sssgovdues
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1680, 862);
            Controls.Add(dataGridView);
            Controls.Add(save);
            Controls.Add(load);
            Name = "sssgovdues";
            Text = "sssgovdues";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button load;
        private Button save;
        private DataGridView dataGridView;
    }
}