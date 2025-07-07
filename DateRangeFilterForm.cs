using System;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class DateRangeFilterForm : Form
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public string SelectedCcode { get; private set; }
        private System.Windows.Forms.Panel selectedPanel = null;

        public DateRangeFilterForm()
        {
            InitializeComponent();
            LoadCcodePanels();
        }

        private void LoadCcodePanels()
        {
            flowCcodePanels.Controls.Clear();
            List<string> ccodeList = new List<string>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT DISTINCT ccode FROM employee WHERE ccode IS NOT NULL AND ccode != '' ORDER BY ccode", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ccodeList.Add(reader.GetString(0));
                    }
                }
            }
            foreach (var ccode in ccodeList)
            {
                var panel = new System.Windows.Forms.Panel
                {
                    Width = 100,
                    Height = 40,
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                    Margin = new System.Windows.Forms.Padding(3),
                    Cursor = System.Windows.Forms.Cursors.Hand,
                    Tag = ccode
                };
                var lbl = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Width = 100,
                    Height = 40,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular),
                    Text = ccode,
                    Cursor = System.Windows.Forms.Cursors.Hand
                };
                panel.Controls.Add(lbl);
                panel.Click += (s, e) => SelectCcodePanel(panel, ccode);
                lbl.Click += (s, e) => SelectCcodePanel(panel, ccode);
                flowCcodePanels.Controls.Add(panel);
            }
        }

        private void SelectCcodePanel(System.Windows.Forms.Panel panel, string ccode)
        {
            // Highlight selected panel
            foreach (System.Windows.Forms.Panel p in flowCcodePanels.Controls)
                p.BackColor = System.Drawing.SystemColors.Control;
            panel.BackColor = System.Drawing.Color.LightBlue;
            selectedPanel = panel;
            SelectedCcode = ccode;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validate and parse the date inputs
            if (!DateTime.TryParse(txtFrom.Text, out DateTime fromDate))
            {
                MessageBox.Show("Invalid 'From' date. Please use MM/DD/YYYY format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFrom.Focus();
                return;
            }
            if (!DateTime.TryParse(txtTo.Text, out DateTime toDate))
            {
                MessageBox.Show("Invalid 'To' date. Please use MM/DD/YYYY format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTo.Focus();
                return;
            }
            if (fromDate > toDate)
            {
                MessageBox.Show("'From' date must be before or equal to 'To' date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFrom.Focus();
                return;
            }
            if (string.IsNullOrEmpty(SelectedCcode))
            {
                MessageBox.Show("Please select a Cost Center (ccode) before proceeding.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FromDate = fromDate;
            ToDate = toDate;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
