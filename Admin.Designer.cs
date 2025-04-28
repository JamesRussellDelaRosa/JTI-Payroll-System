namespace JTI_Payroll_System
{
    partial class Admin
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
        /// 

        private void InitializeComponent()
        {
            add = new Button();
            search = new Button();
            delete = new Button();
            update = new Button();
            label1 = new Label();
            label2 = new Label();
            username = new TextBox();
            password = new TextBox();
            view = new Button();
            dataGridView1 = new DataGridView();
            employee = new Button();
            uploadattlog = new Button();
            processDtr = new Button();
            btnpayrollpost = new Button();
            rateConfig = new Button();
            sssgovdues = new Button();
            ssstest = new Button();
            sssloan = new Button();
            hdmfloan = new Button();
            autodeducthdmfsssloan = new Button();
            menuFlowLayoutPanel = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // add
            // 
            add.Location = new Point(15, 315);
            add.Name = "add";
            add.Size = new Size(94, 29);
            add.TabIndex = 1;
            add.Text = "ADD USER";
            add.UseVisualStyleBackColor = true;
            add.Click += add_Click;
            // 
            // search
            // 
            search.Location = new Point(115, 315);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 2;
            search.Text = "SEARCH";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // delete
            // 
            delete.Location = new Point(215, 315);
            delete.Name = "delete";
            delete.Size = new Size(94, 29);
            delete.TabIndex = 3;
            delete.Text = "DELETE";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // update
            // 
            update.Location = new Point(315, 315);
            update.Name = "update";
            update.Size = new Size(94, 29);
            update.TabIndex = 4;
            update.Text = "UPDATE";
            update.UseVisualStyleBackColor = true;
            update.Click += update_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(31, 187);
            label1.Name = "label1";
            label1.Size = new Size(115, 28);
            label1.TabIndex = 5;
            label1.Text = "USERNAME";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(34, 243);
            label2.Name = "label2";
            label2.Size = new Size(117, 28);
            label2.TabIndex = 6;
            label2.Text = "PASSWORD";
            // 
            // username
            // 
            username.Location = new Point(184, 188);
            username.Name = "username";
            username.Size = new Size(125, 27);
            username.TabIndex = 7;
            // 
            // password
            // 
            password.Location = new Point(184, 247);
            password.Name = "password";
            password.Size = new Size(125, 27);
            password.TabIndex = 8;
            // 
            // view
            // 
            view.Location = new Point(415, 315);
            view.Name = "view";
            view.Size = new Size(94, 29);
            view.TabIndex = 9;
            view.Text = "VIEW";
            view.UseVisualStyleBackColor = true;
            view.Click += view_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(449, 99);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(299, 188);
            dataGridView1.TabIndex = 10;
            // 
            // employee
            // 
            employee.Location = new Point(515, 315);
            employee.Name = "employee";
            employee.Size = new Size(94, 29);
            employee.TabIndex = 11;
            employee.Text = "EMPLOYEE";
            employee.UseVisualStyleBackColor = true;
            employee.Click += button1_Click;
            // 
            // uploadattlog
            // 
            uploadattlog.Location = new Point(615, 315);
            uploadattlog.Name = "uploadattlog";
            uploadattlog.Size = new Size(94, 29);
            uploadattlog.TabIndex = 12;
            uploadattlog.Text = "UploadATT";
            uploadattlog.UseVisualStyleBackColor = true;
            uploadattlog.Click += uploadattlog_Click;
            // 
            // processDtr
            // 
            processDtr.Location = new Point(15, 380);
            processDtr.Name = "processDtr";
            processDtr.Size = new Size(131, 29);
            processDtr.TabIndex = 13;
            processDtr.Text = "PROCESS DTR";
            processDtr.UseVisualStyleBackColor = true;
            processDtr.Click += processDtr_Click;
            // 
            // btnpayrollpost
            // 
            btnpayrollpost.Location = new Point(152, 380);
            btnpayrollpost.Name = "btnpayrollpost";
            btnpayrollpost.Size = new Size(139, 29);
            btnpayrollpost.TabIndex = 14;
            btnpayrollpost.Text = "PAYROLL POSTING";
            btnpayrollpost.UseVisualStyleBackColor = true;
            btnpayrollpost.Click += btnpayrollpost_Click;
            // 
            // rateConfig
            // 
            rateConfig.Location = new Point(298, 380);
            rateConfig.Margin = new Padding(3, 4, 3, 4);
            rateConfig.Name = "rateConfig";
            rateConfig.Size = new Size(123, 31);
            rateConfig.TabIndex = 15;
            rateConfig.Text = "RATECONFIG";
            rateConfig.UseVisualStyleBackColor = true;
            rateConfig.Click += rateConfig_Click;
            // 
            // sssgovdues
            // 
            sssgovdues.Location = new Point(427, 380);
            sssgovdues.Name = "sssgovdues";
            sssgovdues.Size = new Size(166, 29);
            sssgovdues.TabIndex = 16;
            sssgovdues.Text = "SSS CONTRIB TABLE";
            sssgovdues.UseVisualStyleBackColor = true;
            sssgovdues.Click += sssgovdues_Click;
            // 
            // ssstest
            // 
            ssstest.Location = new Point(615, 380);
            ssstest.Name = "ssstest";
            ssstest.Size = new Size(94, 29);
            ssstest.TabIndex = 17;
            ssstest.Text = "SSSTEST";
            ssstest.UseVisualStyleBackColor = true;
            ssstest.Click += ssstest_Click;
            // 
            // sssloan
            // 
            sssloan.Location = new Point(715, 315);
            sssloan.Name = "sssloan";
            sssloan.Size = new Size(94, 29);
            sssloan.TabIndex = 18;
            sssloan.Text = "SSS LOAN";
            sssloan.UseVisualStyleBackColor = true;
            sssloan.Click += sssloan_Click;
            // 
            // hdmfloan
            // 
            hdmfloan.Location = new Point(715, 381);
            hdmfloan.Name = "hdmfloan";
            hdmfloan.Size = new Size(119, 29);
            hdmfloan.TabIndex = 19;
            hdmfloan.Text = "HDMF LOAN";
            hdmfloan.UseVisualStyleBackColor = true;
            hdmfloan.Click += hdmfloan_Click;
            // 
            // autodeducthdmfsssloan
            // 
            autodeducthdmfsssloan.Location = new Point(826, 315);
            autodeducthdmfsssloan.Name = "autodeducthdmfsssloan";
            autodeducthdmfsssloan.Size = new Size(217, 29);
            autodeducthdmfsssloan.TabIndex = 20;
            autodeducthdmfsssloan.Text = "Auto Deduct SSS/HDMF Loan";
            autodeducthdmfsssloan.UseVisualStyleBackColor = true;
            autodeducthdmfsssloan.Click += autodeducthdmfsssloan_Click;
            // 
            // menuFlowLayoutPanel
            // 
            menuFlowLayoutPanel.Location = new Point(1105, 55);
            menuFlowLayoutPanel.Name = "menuFlowLayoutPanel";
            menuFlowLayoutPanel.Size = new Size(442, 568);
            menuFlowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            menuFlowLayoutPanel.WrapContents = true;
            menuFlowLayoutPanel.AutoScroll = true; // Enable scrolling
            menuFlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle; // Optional: Add a border for better visibility


            // Initialize Panels
            addPanel = CreateClickablePanel("ADD USER", add_Click);
            searchPanel = CreateClickablePanel("SEARCH", search_Click);
            deletePanel = CreateClickablePanel("DELETE", delete_Click);
            updatePanel = CreateClickablePanel("UPDATE", update_Click);
            viewPanel = CreateClickablePanel("VIEW", view_Click);
            employeePanel = CreateClickablePanel("EMPLOYEE", button1_Click);
            uploadattlogPanel = CreateClickablePanel("UploadATT", uploadattlog_Click);
            processDtrPanel = CreateClickablePanel("PROCESS DTR", processDtr_Click);
            btnpayrollpostPanel = CreateClickablePanel("PAYROLL POSTING", btnpayrollpost_Click);
            rateConfigPanel = CreateClickablePanel("RATECONFIG", rateConfig_Click);
            sssgovduesPanel = CreateClickablePanel("SSS CONTRIB TABLE", sssgovdues_Click);
            ssstestPanel = CreateClickablePanel("SSSTEST", ssstest_Click);
            sssloanPanel = CreateClickablePanel("SSS LOAN", sssloan_Click);
            hdmfloanPanel = CreateClickablePanel("HDMF LOAN", hdmfloan_Click);
            autodeducthdmfsssloanPanel = CreateClickablePanel("Auto Deduct SSS/HDMF Loan", autodeducthdmfsssloan_Click);

            // Add Panels to FlowLayoutPanel
            menuFlowLayoutPanel.Controls.Add(addPanel);
            menuFlowLayoutPanel.Controls.Add(searchPanel);
            menuFlowLayoutPanel.Controls.Add(deletePanel);
            menuFlowLayoutPanel.Controls.Add(updatePanel);
            menuFlowLayoutPanel.Controls.Add(viewPanel);
            menuFlowLayoutPanel.Controls.Add(employeePanel);
            menuFlowLayoutPanel.Controls.Add(uploadattlogPanel);
            menuFlowLayoutPanel.Controls.Add(processDtrPanel);
            menuFlowLayoutPanel.Controls.Add(btnpayrollpostPanel);
            menuFlowLayoutPanel.Controls.Add(rateConfigPanel);
            menuFlowLayoutPanel.Controls.Add(sssgovduesPanel);
            menuFlowLayoutPanel.Controls.Add(ssstestPanel);
            menuFlowLayoutPanel.Controls.Add(sssloanPanel);
            menuFlowLayoutPanel.Controls.Add(hdmfloanPanel);
            menuFlowLayoutPanel.Controls.Add(autodeducthdmfsssloanPanel);
            // Add FlowLayoutPanel to Form
            Controls.Add(menuFlowLayoutPanel);
            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1575, 635);
            Controls.Add(menuFlowLayoutPanel);
            Controls.Add(autodeducthdmfsssloan);
            Controls.Add(hdmfloan);
            Controls.Add(sssloan);
            Controls.Add(ssstest);
            Controls.Add(sssgovdues);
            Controls.Add(rateConfig);
            Controls.Add(btnpayrollpost);
            Controls.Add(processDtr);
            Controls.Add(uploadattlog);
            Controls.Add(employee);
            Controls.Add(dataGridView1);
            Controls.Add(view);
            Controls.Add(password);
            Controls.Add(username);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(update);
            Controls.Add(delete);
            Controls.Add(search);
            Controls.Add(add);
            Name = "Admin";
            Text = "Admin";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel CreateClickablePanel(string text, EventHandler clickHandler)
        {
            Panel panel = new Panel();
            panel.Size = new Size(400, 29);

            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            panel.Controls.Add(label);

            // Add click event to highlight the panel
            panel.Click += (s, e) =>
            {
                HighlightPanel(panel); // Highlight the clicked panel
                clickHandler(s, e);    // Trigger the original click handler
            };

            // Ensure label click triggers the same behavior
            label.Click += (s, e) =>
            {
                HighlightPanel(panel);
                clickHandler(s, e);
            };

            return panel;
        }

        // Method to highlight the selected panel
        private void HighlightPanel(Panel selectedPanel)
        {
            // Reset all panels' background color to default
            foreach (Control control in menuFlowLayoutPanel.Controls)
            {
                if (control is Panel panel)
                {
                    panel.BackColor = Color.Transparent; // Default color
                }
            }

            // Set the selected panel's background color to blue
            selectedPanel.BackColor = Color.LightBlue;
        }

        #endregion
        private Button add;
        private Button search;
        private Button delete;
        private Button update;
        private Label label1;
        private Label label2;
        private TextBox username;
        private TextBox password;
        private Button view;
        private DataGridView dataGridView1;
        private Button employee;
        private Button uploadattlog;
        private Button processDtr;
        private Button btnpayrollpost;
        private Button rateConfig;
        private Button sssgovdues;
        private Button ssstest;
        private Button sssloan;
        private Button hdmfloan;
        private Button autodeducthdmfsssloan;
        private FlowLayoutPanel menuFlowLayoutPanel;
        private Panel addPanel;
        private Panel searchPanel;
        private Panel deletePanel;
        private Panel updatePanel;
        private Panel viewPanel;
        private Panel employeePanel;
        private Panel uploadattlogPanel;
        private Panel processDtrPanel;
        private Panel btnpayrollpostPanel;
        private Panel rateConfigPanel;
        private Panel sssgovduesPanel;
        private Panel ssstestPanel;
        private Panel sssloanPanel;
        private Panel hdmfloanPanel;
        private Panel autodeducthdmfsssloanPanel;
    }
}