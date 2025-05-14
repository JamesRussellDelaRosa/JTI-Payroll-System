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

        #endregion

        private FlowLayoutPanel menuFlowLayoutPanel;
        private Label label1;
        private Label lblFullName;
        private Button btnLogout;
        private Button buttonClose;
    }
}