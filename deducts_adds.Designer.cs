namespace JTI_Payroll_System
{
    partial class deducts_adds
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox fromDateTextBox;
        private System.Windows.Forms.TextBox toDateTextBox;
        private System.Windows.Forms.Button openFormButton;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
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
            fromDateTextBox = new TextBox();
            toDateTextBox = new TextBox();
            openFormButton = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // fromDateTextBox
            // 
            fromDateTextBox.Location = new Point(139, 123);
            fromDateTextBox.Name = "fromDateTextBox";
            fromDateTextBox.PlaceholderText = "MM/DD/YYYY";
            fromDateTextBox.Size = new Size(100, 27);
            fromDateTextBox.TabIndex = 0;
            fromDateTextBox.KeyPress += AutoFormatDate;
            // 
            // toDateTextBox
            // 
            toDateTextBox.Location = new Point(273, 123);
            toDateTextBox.Name = "toDateTextBox";
            toDateTextBox.PlaceholderText = "MM/DD/YYYY";
            toDateTextBox.Size = new Size(100, 27);
            toDateTextBox.TabIndex = 1;
            toDateTextBox.KeyPress += AutoFormatDate;
            // 
            // openFormButton
            // 
            openFormButton.Location = new Point(118, 175);
            openFormButton.Name = "openFormButton";
            openFormButton.Size = new Size(260, 29);
            openFormButton.TabIndex = 2;
            openFormButton.Text = "Open Deductions/Additionals";
            openFormButton.UseVisualStyleBackColor = true;
            openFormButton.Click += OpenFormButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(87, 127);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 3;
            label1.Text = "FROM";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(243, 127);
            label2.Name = "label2";
            label2.Size = new Size(27, 20);
            label2.TabIndex = 4;
            label2.Text = "TO";
            label2.Click += label2_Click;
            // 
            // deducts_adds
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(489, 326);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(fromDateTextBox);
            Controls.Add(toDateTextBox);
            Controls.Add(openFormButton);
            Name = "deducts_adds";
            Text = "deducts_adds";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
    }
}