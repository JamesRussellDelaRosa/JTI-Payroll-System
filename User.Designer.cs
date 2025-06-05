namespace JTI_Payroll_System
{
    partial class User
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
            menuFlowLayoutPanel = new FlowLayoutPanel();
            label1 = new Label();
            lblFullName = new Label();
            btnLogout = new Button();
            buttonClose = new Button();
            SuspendLayout();
            // 
            // menuFlowLayoutPanel
            // 
            menuFlowLayoutPanel.AutoScroll = true;
            menuFlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            menuFlowLayoutPanel.Location = new Point(12, 49);
            menuFlowLayoutPanel.Name = "menuFlowLayoutPanel";
            menuFlowLayoutPanel.Size = new Size(442, 531);
            menuFlowLayoutPanel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 28.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(472, 49);
            label1.Name = "label1";
            label1.Size = new Size(220, 62);
            label1.TabIndex = 4;
            label1.Text = "Welcome";
            // 
            // lblFullName
            // 
            lblFullName.AutoSize = true;
            lblFullName.Font = new Font("Segoe UI", 28.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFullName.Location = new Point(472, 111);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(175, 62);
            lblFullName.TabIndex = 3;
            lblFullName.Text = "sample";
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(460, 551);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(94, 29);
            btnLogout.TabIndex = 5;
            btnLogout.Text = "LOGOUT";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // buttonClose
            // 
            buttonClose.Location = new Point(560, 551);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(94, 29);
            buttonClose.TabIndex = 6;
            buttonClose.Text = "CLOSE";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // User
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1007, 600);
            Controls.Add(buttonClose);
            Controls.Add(btnLogout);
            Controls.Add(label1);
            Controls.Add(lblFullName);
            Controls.Add(menuFlowLayoutPanel);
            Name = "User";
            Text = "User";
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeCustomPanels()
        {
            // Initialize Panels
            Panel employeePanel = CreateClickablePanel("EMPLOYEE", button1_Click);
            Panel uploadattlogPanel = CreateClickablePanel("UploadATT", uploadattlog_Click);
            Panel processDtrPanel = CreateClickablePanel("PROCESS DTR", processDtr_Click);
            Panel btnpayrollpostPanel = CreateClickablePanel("PAYROLL POSTING", btnpayrollpost_Click);
            Panel sssloanPanel = CreateClickablePanel("SSS LOAN", sssloan_Click);
            Panel hdmfloanPanel = CreateClickablePanel("HDMF LOAN", hdmfloan_Click);
            Panel cooploanPanel = CreateClickablePanel("COOP LOAN", cooploan_Click);
            Panel deducts_addsPanel = CreateClickablePanel("Deductions/Additionals", deducts_adds_Click);
            Panel govdues13thmonthPanel = CreateClickablePanel("SUMMATION OF PAYROLL BASIC/GROSS & GOV DUES", govdues_13thmonth_Click);
            Panel semimonthlywtaxtablePanel = CreateClickablePanel("Semi Monthly Withholding Tax Table", semimonthlywtax_Click);
            Panel autodeducthdmfsssloanPanel = CreateClickablePanel("Auto Deduct SSS/HDMF Loan", autodeducthdmfsssloan_Click);
            Panel payrollAdjPanel = CreateClickablePanel("Payroll Adjustments", payrollAdj_Click);
            Panel modifyPayrollPanel = CreateClickablePanel("Modify Payroll", modifyPayroll_Click);
            Panel computewtaxPanel = CreateClickablePanel("Compute WTax", computewtax_Click);

            // Add Panels to FlowLayoutPanel
            menuFlowLayoutPanel.Controls.Add(employeePanel);
            menuFlowLayoutPanel.Controls.Add(uploadattlogPanel);
            menuFlowLayoutPanel.Controls.Add(processDtrPanel);
            menuFlowLayoutPanel.Controls.Add(btnpayrollpostPanel);
            menuFlowLayoutPanel.Controls.Add(sssloanPanel);
            menuFlowLayoutPanel.Controls.Add(hdmfloanPanel);
            menuFlowLayoutPanel.Controls.Add(cooploanPanel);
            menuFlowLayoutPanel.Controls.Add(deducts_addsPanel);
            menuFlowLayoutPanel.Controls.Add(govdues13thmonthPanel);
            menuFlowLayoutPanel.Controls.Add(autodeducthdmfsssloanPanel);
            menuFlowLayoutPanel.Controls.Add(semimonthlywtaxtablePanel);
            menuFlowLayoutPanel.Controls.Add(payrollAdjPanel);
            menuFlowLayoutPanel.Controls.Add(modifyPayrollPanel);
            menuFlowLayoutPanel.Controls.Add(computewtaxPanel);
        }

        private Panel CreateClickablePanel(string text, EventHandler functionHandler)
        {
            Panel panel = new Panel();
            panel.Size = new Size(442, 29);
            panel.TabStop = true; // Enable focus for the panel to capture key events

            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            panel.Controls.Add(label);

            // Add single-click event to highlight the panel
            panel.Click += (s, e) =>
            {
                HighlightPanel(panel); // Highlight the clicked panel
                panel.Focus(); // Set focus to the panel to capture key events
            };

            // Add double-click event to execute the function
            panel.DoubleClick += (s, e) =>
            {
                functionHandler(s, e); // Trigger the original double-click handler
            };

            // Add key press event to execute the function when Enter is pressed or navigate with Up/Down keys
            panel.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    functionHandler(s, e); // Trigger the function handler
                }
                else if (e.KeyCode == Keys.Up)
                {
                    NavigateToPreviousPanel(panel); // Navigate to the previous panel
                }
                else if (e.KeyCode == Keys.Down)
                {
                    NavigateToNextPanel(panel); // Navigate to the next panel
                }
            };

            // Ensure label click and double-click trigger the same behavior
            label.Click += (s, e) =>
            {
                HighlightPanel(panel);
                panel.Focus();
            };

            label.DoubleClick += (s, e) =>
            {
                functionHandler(s, e);
            };

            return panel;
        }

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

        private void NavigateToPreviousPanel(Panel currentPanel)
        {
            int currentIndex = menuFlowLayoutPanel.Controls.GetChildIndex(currentPanel);
            if (currentIndex > 0)
            {
                Panel previousPanel = menuFlowLayoutPanel.Controls[currentIndex - 1] as Panel;
                if (previousPanel != null)
                {
                    HighlightPanel(previousPanel);
                    previousPanel.Focus();
                }
            }
        }

        private void NavigateToNextPanel(Panel currentPanel)
        {
            int currentIndex = menuFlowLayoutPanel.Controls.GetChildIndex(currentPanel);
            if (currentIndex < menuFlowLayoutPanel.Controls.Count - 1)
            {
                Panel nextPanel = menuFlowLayoutPanel.Controls[currentIndex + 1] as Panel;
                if (nextPanel != null)
                {
                    HighlightPanel(nextPanel);
                    nextPanel.Focus();
                }
            }
        }

        #endregion

        private FlowLayoutPanel menuFlowLayoutPanel;
        private Label label1;
        private Label lblFullName;
        private Button btnLogout;
        private Button buttonClose;
    }
}