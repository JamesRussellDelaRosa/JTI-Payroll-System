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
            save = new Button();
            empname = new Label();
            empid = new Label();
            delete = new Button();
            calamitydelete = new Button();
            calamitysave = new Button();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            calamitylastcollect = new TextBox();
            calamityfirstcollect = new TextBox();
            calamitydeductpay = new TextBox();
            calamitymonthamort = new TextBox();
            calamityloanamt = new TextBox();
            calamityloandate = new TextBox();
            searchtxt = new TextBox();
            search = new Button();
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
            label2.Location = new Point(960, 68);
            label2.Name = "label2";
            label2.Size = new Size(193, 20);
            label2.TabIndex = 8;
            label2.Text = "LOAN DATE (MM/DD/YYYY)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(960, 98);
            label3.Name = "label3";
            label3.Size = new Size(114, 20);
            label3.TabIndex = 9;
            label3.Text = "LOAN AMOUNT";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(960, 131);
            label4.Name = "label4";
            label4.Size = new Size(187, 20);
            label4.TabIndex = 10;
            label4.Text = "MONTHLY AMORTIZATION";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(960, 171);
            label5.Name = "label5";
            label5.Size = new Size(194, 20);
            label5.TabIndex = 11;
            label5.Text = "DEDUCTION EVERY PAYDAY";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(960, 204);
            label6.Name = "label6";
            label6.Size = new Size(244, 20);
            label6.TabIndex = 12;
            label6.Text = "FIRST COLLECTION  (MM/DD/YYYY)";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(960, 233);
            label7.Name = "label7";
            label7.Size = new Size(237, 20);
            label7.TabIndex = 13;
            label7.Text = "LAST COLLECTION (MM/DD/YYYY)";
            // 
            // save
            // 
            save.Location = new Point(1158, 308);
            save.Name = "save";
            save.Size = new Size(94, 29);
            save.TabIndex = 15;
            save.Text = "Save";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
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
            // delete
            // 
            delete.Location = new Point(1053, 308);
            delete.Name = "delete";
            delete.Size = new Size(94, 29);
            delete.TabIndex = 18;
            delete.Text = "Delete";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // calamitydelete
            // 
            calamitydelete.Location = new Point(1044, 642);
            calamitydelete.Name = "calamitydelete";
            calamitydelete.Size = new Size(94, 29);
            calamitydelete.TabIndex = 33;
            calamitydelete.Text = "Delete";
            calamitydelete.UseVisualStyleBackColor = true;
            calamitydelete.Click += calamitydelete_Click;
            // 
            // calamitysave
            // 
            calamitysave.Location = new Point(1149, 642);
            calamitysave.Name = "calamitysave";
            calamitysave.Size = new Size(94, 29);
            calamitysave.TabIndex = 32;
            calamitysave.Text = "Save";
            calamitysave.UseVisualStyleBackColor = true;
            calamitysave.Click += calamitysave_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(951, 567);
            label8.Name = "label8";
            label8.Size = new Size(237, 20);
            label8.TabIndex = 31;
            label8.Text = "LAST COLLECTION (MM/DD/YYYY)";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(951, 538);
            label9.Name = "label9";
            label9.Size = new Size(244, 20);
            label9.TabIndex = 30;
            label9.Text = "FIRST COLLECTION  (MM/DD/YYYY)";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(951, 505);
            label10.Name = "label10";
            label10.Size = new Size(194, 20);
            label10.TabIndex = 29;
            label10.Text = "DEDUCTION EVERY PAYDAY";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(951, 465);
            label11.Name = "label11";
            label11.Size = new Size(187, 20);
            label11.TabIndex = 28;
            label11.Text = "MONTHLY AMORTIZATION";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(951, 432);
            label12.Name = "label12";
            label12.Size = new Size(114, 20);
            label12.TabIndex = 27;
            label12.Text = "LOAN AMOUNT";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(951, 402);
            label13.Name = "label13";
            label13.Size = new Size(193, 20);
            label13.TabIndex = 26;
            label13.Text = "LOAN DATE (MM/DD/YYYY)";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(1022, 371);
            label14.Name = "label14";
            label14.Size = new Size(225, 20);
            label14.TabIndex = 25;
            label14.Text = "HDMF CALAMITY LOAN DETAILS";
            // 
            // calamitylastcollect
            // 
            calamitylastcollect.Location = new Point(1201, 564);
            calamitylastcollect.Name = "calamitylastcollect";
            calamitylastcollect.PlaceholderText = "MM/DD/YYYY";
            calamitylastcollect.Size = new Size(125, 27);
            calamitylastcollect.TabIndex = 24;
            // 
            // calamityfirstcollect
            // 
            calamityfirstcollect.Location = new Point(1201, 531);
            calamityfirstcollect.Name = "calamityfirstcollect";
            calamityfirstcollect.PlaceholderText = "MM/DD/YYYY";
            calamityfirstcollect.Size = new Size(125, 27);
            calamityfirstcollect.TabIndex = 23;
            // 
            // calamitydeductpay
            // 
            calamitydeductpay.Location = new Point(1201, 498);
            calamitydeductpay.Name = "calamitydeductpay";
            calamitydeductpay.Size = new Size(125, 27);
            calamitydeductpay.TabIndex = 22;
            // 
            // calamitymonthamort
            // 
            calamitymonthamort.Location = new Point(1201, 465);
            calamitymonthamort.Name = "calamitymonthamort";
            calamitymonthamort.Size = new Size(125, 27);
            calamitymonthamort.TabIndex = 21;
            // 
            // calamityloanamt
            // 
            calamityloanamt.Location = new Point(1201, 432);
            calamityloanamt.Name = "calamityloanamt";
            calamityloanamt.Size = new Size(125, 27);
            calamityloanamt.TabIndex = 20;
            // 
            // calamityloandate
            // 
            calamityloandate.Location = new Point(1201, 399);
            calamityloandate.Name = "calamityloandate";
            calamityloandate.PlaceholderText = "MM/DD/YYYY";
            calamityloandate.Size = new Size(125, 27);
            calamityloandate.TabIndex = 19;
            // 
            // searchtxt
            // 
            searchtxt.Location = new Point(663, 14);
            searchtxt.Name = "searchtxt";
            searchtxt.Size = new Size(195, 27);
            searchtxt.TabIndex = 34;
            // 
            // search
            // 
            search.Location = new Point(864, 12);
            search.Name = "search";
            search.Size = new Size(94, 29);
            search.TabIndex = 35;
            search.Text = "Search";
            search.UseVisualStyleBackColor = true;
            search.Click += search_Click;
            // 
            // HDMFLOAN
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1442, 814);
            Controls.Add(search);
            Controls.Add(searchtxt);
            Controls.Add(calamitydelete);
            Controls.Add(calamitysave);
            Controls.Add(label8);
            Controls.Add(label9);
            Controls.Add(label10);
            Controls.Add(label11);
            Controls.Add(label12);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(calamitylastcollect);
            Controls.Add(calamityfirstcollect);
            Controls.Add(calamitydeductpay);
            Controls.Add(calamitymonthamort);
            Controls.Add(calamityloanamt);
            Controls.Add(calamityloandate);
            Controls.Add(delete);
            Controls.Add(empid);
            Controls.Add(empname);
            Controls.Add(save);
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
        private Button save;
        private Label empname;
        private Label empid;
        private Button delete;
        private Button calamitydelete;
        private Button calamitysave;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private TextBox calamitylastcollect;
        private TextBox calamityfirstcollect;
        private TextBox calamitydeductpay;
        private TextBox calamitymonthamort;
        private TextBox calamityloanamt;
        private TextBox calamityloandate;
        private TextBox searchtxt;
        private Button search;
    }
}