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
            menuFlowLayoutPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // menuFlowLayoutPanel
            // 
            menuFlowLayoutPanel.AutoScroll = true;
            menuFlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            menuFlowLayoutPanel.Location = new Point(12, 55);
            menuFlowLayoutPanel.Name = "menuFlowLayoutPanel";
            menuFlowLayoutPanel.Size = new Size(442, 568);
            menuFlowLayoutPanel.TabIndex = 0;

            // Initialize Panels
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
            editUsersPanel = CreateClickablePanel("EditUsers", editUsers_Click);

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
            menuFlowLayoutPanel.Controls.Add(editUsersPanel);

            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(923, 635);
            Controls.Add(menuFlowLayoutPanel);
            Name = "Admin";
            Text = "Admin";
            ResumeLayout(false);
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
        private Panel editUsersPanel;
    }
}