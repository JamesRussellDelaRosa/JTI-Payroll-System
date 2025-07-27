namespace JTI_Payroll_System
{
    partial class payroll_list
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
            process = new Button();
            label5 = new Label();
            todate = new TextBox();
            label4 = new Label();
            fromdate = new TextBox();
            flowCcodePanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // process
            // 
            process.Location = new Point(352, 12);
            process.Name = "process";
            process.Size = new Size(193, 29);
            process.TabIndex = 45;
            process.Text = "PROCESS";
            process.UseVisualStyleBackColor = true;
            process.Click += process_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(190, 15);
            label5.Name = "label5";
            label5.Size = new Size(25, 20);
            label5.TabIndex = 44;
            label5.Text = "To";
            // 
            // todate
            // 
            todate.Location = new Point(221, 12);
            todate.Name = "todate";
            todate.Size = new Size(125, 27);
            todate.TabIndex = 43;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(10, 15);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 42;
            label4.Text = "From";
            // 
            // fromdate
            // 
            fromdate.Location = new Point(59, 12);
            fromdate.Name = "fromdate";
            fromdate.Size = new Size(125, 27);
            fromdate.TabIndex = 41;
            // 
            // flowCcodePanel
            // 
            flowCcodePanel.AutoScroll = true;
            flowCcodePanel.FlowDirection = FlowDirection.TopDown;
            flowCcodePanel.Location = new Point(12, 69);
            flowCcodePanel.Name = "flowCcodePanel";
            flowCcodePanel.Size = new Size(554, 369);
            flowCcodePanel.TabIndex = 46;
            flowCcodePanel.WrapContents = false;
            // 
            // payroll_list
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(578, 450);
            Controls.Add(process);
            Controls.Add(label5);
            Controls.Add(todate);
            Controls.Add(label4);
            Controls.Add(fromdate);
            Controls.Add(flowCcodePanel);
            Name = "payroll_list";
            Text = "payroll_list";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button process;
        private Label label5;
        private TextBox todate;
        private Label label4;
        private TextBox fromdate;
        private System.Windows.Forms.FlowLayoutPanel flowCcodePanel;
    }
}