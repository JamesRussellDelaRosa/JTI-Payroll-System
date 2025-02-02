namespace JTI_Payroll_System
{
    partial class employee
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
            label1 = new Label();
            textEmpID = new TextBox();
            textFirstName = new TextBox();
            labelFirstName = new Label();
            textMiddleName = new TextBox();
            label2 = new Label();
            textLastName = new TextBox();
            lastname = new Label();
            textEmail = new TextBox();
            label3 = new Label();
            textPhone = new TextBox();
            label4 = new Label();
            birthday = new DateTimePicker();
            label5 = new Label();
            saveEmp = new Button();
            import = new Button();
            search = new Button();
            searchbar = new TextBox();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 18);
            label1.Name = "label1";
            label1.Size = new Size(94, 20);
            label1.TabIndex = 0;
            label1.Text = "EMPLOYEEID";
            // 
            // textEmpID
            // 
            textEmpID.Location = new Point(112, 15);
            textEmpID.Name = "textEmpID";
            textEmpID.Size = new Size(125, 27);
            textEmpID.TabIndex = 1;
            // 
            // textFirstName
            // 
            textFirstName.Location = new Point(112, 48);
            textFirstName.Name = "textFirstName";
            textFirstName.Size = new Size(125, 27);
            textFirstName.TabIndex = 3;
            // 
            // labelFirstName
            // 
            labelFirstName.AutoSize = true;
            labelFirstName.Location = new Point(12, 51);
            labelFirstName.Name = "labelFirstName";
            labelFirstName.Size = new Size(76, 20);
            labelFirstName.TabIndex = 2;
            labelFirstName.Text = "FirstName";
            // 
            // textMiddleName
            // 
            textMiddleName.Location = new Point(112, 81);
            textMiddleName.Name = "textMiddleName";
            textMiddleName.Size = new Size(125, 27);
            textMiddleName.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 84);
            label2.Name = "label2";
            label2.Size = new Size(96, 20);
            label2.TabIndex = 4;
            label2.Text = "MiddleName";
            // 
            // textLastName
            // 
            textLastName.Location = new Point(112, 114);
            textLastName.Name = "textLastName";
            textLastName.Size = new Size(125, 27);
            textLastName.TabIndex = 7;
            // 
            // lastname
            // 
            lastname.AutoSize = true;
            lastname.Location = new Point(12, 117);
            lastname.Name = "lastname";
            lastname.Size = new Size(75, 20);
            lastname.TabIndex = 6;
            lastname.Text = "LastName";
            // 
            // textEmail
            // 
            textEmail.Location = new Point(112, 147);
            textEmail.Name = "textEmail";
            textEmail.Size = new Size(125, 27);
            textEmail.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 150);
            label3.Name = "label3";
            label3.Size = new Size(51, 20);
            label3.TabIndex = 8;
            label3.Text = "EMAIL";
            // 
            // textPhone
            // 
            textPhone.Location = new Point(112, 180);
            textPhone.Name = "textPhone";
            textPhone.Size = new Size(125, 27);
            textPhone.TabIndex = 11;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 183);
            label4.Name = "label4";
            label4.Size = new Size(104, 20);
            label4.TabIndex = 10;
            label4.Text = "PhoneNumber";
            // 
            // birthday
            // 
            birthday.CustomFormat = "MM/dd/yyyy";
            birthday.Format = DateTimePickerFormat.Custom;
            birthday.Location = new Point(112, 213);
            birthday.Name = "birthday";
            birthday.Size = new Size(125, 27);
            birthday.TabIndex = 12;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(25, 218);
            label5.Name = "label5";
            label5.Size = new Size(64, 20);
            label5.TabIndex = 13;
            label5.Text = "Birthday";
            // 
            // saveEmp
            // 
            saveEmp.Location = new Point(112, 315);
            saveEmp.Name = "saveEmp";
            saveEmp.Size = new Size(94, 29);
            saveEmp.TabIndex = 16;
            saveEmp.Text = "save";
            saveEmp.UseVisualStyleBackColor = true;
            saveEmp.Click += saveEmp_Click;
            // 
            // import
            // 
            import.Location = new Point(227, 315);
            import.Name = "import";
            import.Size = new Size(94, 29);
            import.TabIndex = 17;
            import.Text = "import";
            import.UseVisualStyleBackColor = true;
            import.Click += import_Click;
            // 
            // search
            // 
            search.Location = new Point(694, 14);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 18;
            search.Text = "search";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // searchbar
            // 
            searchbar.Location = new Point(492, 14);
            searchbar.Name = "searchbar";
            searchbar.Size = new Size(196, 27);
            searchbar.TabIndex = 19;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // employee
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(searchbar);
            Controls.Add(search);
            Controls.Add(import);
            Controls.Add(saveEmp);
            Controls.Add(label5);
            Controls.Add(birthday);
            Controls.Add(textPhone);
            Controls.Add(label4);
            Controls.Add(textEmail);
            Controls.Add(label3);
            Controls.Add(textLastName);
            Controls.Add(lastname);
            Controls.Add(textMiddleName);
            Controls.Add(label2);
            Controls.Add(textFirstName);
            Controls.Add(labelFirstName);
            Controls.Add(textEmpID);
            Controls.Add(label1);
            Name = "employee";
            Text = "employee";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textEmpID;
        private TextBox textFirstName;
        private Label labelFirstName;
        private TextBox textMiddleName;
        private Label label2;
        private TextBox textLastName;
        private Label lastname;
        private TextBox textEmail;
        private Label label3;
        private TextBox textPhone;
        private Label label4;
        private DateTimePicker birthday;
        private Label label5;
        private Button saveEmp;
        private Button import;
        private Button search;
        private TextBox searchbar;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
    }
}