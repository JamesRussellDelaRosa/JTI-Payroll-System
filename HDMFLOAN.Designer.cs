namespace JTI_Payroll_System
{
    partial class HDMFLOAN
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            loandate = new TextBox();
            loanamt = new TextBox();
            monthamort = new TextBox();
            deductpay = new TextBox();
            firstcollect = new TextBox();
            lastcollect = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            update = new Button();
            save = new Button();
            cancel = new Button();
            empname = new Label();
            empid = new Label();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Font = new Font("Calibri", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            flowLayoutPanel1.Location = new Point(12, 65);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(736, 405);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // loandate
            // 
            loandate.Location = new Point(1210, 65);
            loandate.Name = "loandate";
            loandate.PlaceholderText = "MM/DD/YYYY";
            loandate.Size = new Size(125, 27);
            loandate.TabIndex = 1;
            // 
            // loanamt
            // 
            loanamt.Location = new Point(1210, 98);
            loanamt.Name = "loanamt";
            loanamt.Size = new Size(125, 27);
            loanamt.TabIndex = 2;
            // 
            // monthamort
            // 
            monthamort.Location = new Point(1210, 131);
            monthamort.Name = "monthamort";
            monthamort.Size = new Size(125, 27);
            monthamort.TabIndex = 3;
            // 
            // deductpay
            // 
            deductpay.Location = new Point(1210, 164);
            deductpay.Name = "deductpay";
            deductpay.Size = new Size(125, 27);
            deductpay.TabIndex = 4;
            // 
            // firstcollect
            // 
            firstcollect.Location = new Point(1210, 197);
            firstcollect.Name = "firstcollect";
            firstcollect.PlaceholderText = "MM/DD/YYYY";
            firstcollect.Size = new Size(125, 27);
            firstcollect.TabIndex = 5;
            // 
            // lastcollect
            // 
            lastcollect.Location = new Point(1210, 230);
            lastcollect.Name = "lastcollect";
            lastcollect.PlaceholderText = "MM/DD/YYYY";
            lastcollect.Size = new Size(125, 27);
            lastcollect.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1031, 37);
            label1.Name = "label1";
            label1.Size = new Size(152, 20);
            label1.TabIndex = 7;
            label1.Text = "HDMF LOAN DETAILS";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1010, 68);
            label2.Name = "label2";
            label2.Size = new Size(87, 20);
            label2.TabIndex = 8;
            label2.Text = "LOAN DATE";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1010, 101);
            label3.Name = "label3";
            label3.Size = new Size(114, 20);
            label3.TabIndex = 9;
            label3.Text = "LOAN AMOUNT";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1010, 134);
            label4.Name = "label4";
            label4.Size = new Size(187, 20);
            label4.TabIndex = 10;
            label4.Text = "MONTHLY AMORTIZATION";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1010, 167);
            label5.Name = "label5";
            label5.Size = new Size(194, 20);
            label5.TabIndex = 11;
            label5.Text = "DEDUCTION EVERY PAYDAY";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(1010, 204);
            label6.Name = "label6";
            label6.Size = new Size(134, 20);
            label6.TabIndex = 12;
            label6.Text = "FIRST COLLECTION";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(1010, 233);
            label7.Name = "label7";
            label7.Size = new Size(131, 20);
            label7.TabIndex = 13;
            label7.Text = "LAST COLLECTION";
            // 
            // update
            // 
            update.Location = new Point(1031, 309);
            update.Name = "update";
            update.Size = new Size(94, 29);
            update.TabIndex = 14;
            update.Text = "Update";
            update.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            save.Location = new Point(1131, 309);
            save.Name = "save";
            save.Size = new Size(94, 29);
            save.TabIndex = 15;
            save.Text = "Save";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
            // 
            // cancel
            // 
            cancel.Location = new Point(1241, 309);
            cancel.Name = "cancel";
            cancel.Size = new Size(94, 29);
            cancel.TabIndex = 16;
            cancel.Text = "Cancel";
            cancel.UseVisualStyleBackColor = true;
            // 
            // empname
            // 
            empname.AutoSize = true;
            empname.Location = new Point(153, 21);
            empname.Name = "empname";
            empname.Size = new Size(125, 20);
            empname.TabIndex = 0;
            empname.Text = "EMPLOYEE NAME";
            // 
            // empid
            // 
            empid.AutoSize = true;
            empid.Location = new Point(39, 21);
            empid.Name = "empid";
            empid.Size = new Size(53, 20);
            empid.TabIndex = 17;
            empid.Text = "ID NO.";
            // 
            // HDMFLOAN
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1402, 588);
            Controls.Add(empid);
            Controls.Add(empname);
            Controls.Add(cancel);
            Controls.Add(save);
            Controls.Add(update);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lastcollect);
            Controls.Add(firstcollect);
            Controls.Add(deductpay);
            Controls.Add(monthamort);
            Controls.Add(loanamt);
            Controls.Add(loandate);
            Controls.Add(flowLayoutPanel1);
            Name = "HDMFLOAN";
            Text = "HDMFLOAN";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox loandate;
        private TextBox loanamt;
        private TextBox monthamort;
        private TextBox deductpay;
        private TextBox firstcollect;
        private TextBox lastcollect;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Button update;
        private Button save;
        private Button cancel;
        private Label empname;
        private Label empid;
    }
}