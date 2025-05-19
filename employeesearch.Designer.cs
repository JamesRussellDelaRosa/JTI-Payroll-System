namespace JTI_Payroll_System
{
    partial class employeesearch
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
            searchbar = new TextBox();
            search = new Button();
            employee = new FlowLayoutPanel();
            label1 = new Label();
            SuspendLayout();
            // 
            // searchbar
            // 
            searchbar.Font = new Font("Segoe UI", 10.8F);
            searchbar.Location = new Point(373, 12);
            searchbar.Name = "searchbar";
            searchbar.Size = new Size(392, 31);
            searchbar.TabIndex = 1;
            // 
            // search
            // 
            search.Font = new Font("Segoe UI", 10.8F);
            search.Location = new Point(771, 12);
            search.Name = "search";
            search.Size = new Size(75, 31);
            search.TabIndex = 2;
            search.Text = "search";
            search.UseVisualStyleBackColor = true;
            // 
            // employee
            // 
            employee.AutoScroll = true;
            employee.Location = new Point(12, 56);
            employee.Name = "employee";
            employee.Size = new Size(837, 424);
            employee.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(355, 25);
            label1.TabIndex = 4;
            label1.Text = "ID_NO., FirstName, MiddleName, LastName";
            // 
            // employeesearch
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(861, 492);
            Controls.Add(label1);
            Controls.Add(employee);
            Controls.Add(search);
            Controls.Add(searchbar);
            Name = "employeesearch";
            Text = "employeesearch";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox searchbar;
        private Button search;
        private FlowLayoutPanel employee;
        private Label label1;
    }
}