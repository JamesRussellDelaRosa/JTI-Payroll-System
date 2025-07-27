using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using System.Globalization;
using System.IO;

namespace JTI_Payroll_System
{
    public partial class payroll_list : Form
    {
        private string selectedCcode = null;

        public payroll_list()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadCcodeList();
        }

        private void InitializeCustomComponents()
        {
            fromdate.Enter += new EventHandler(RemoveHint);
            fromdate.Leave += new EventHandler(AddHint);
            fromdate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            todate.Enter += new EventHandler(RemoveHint);
            todate.Leave += new EventHandler(AddHint);
            todate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            AddHint(fromdate, null);
            AddHint(todate, null);
        }

        private void RemoveHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "MM/DD/YYYY")
            {
                textBox.Text = "";
                textBox.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void AddHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "MM/DD/YYYY";
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void AutoFormatDate(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Allow only digits and control keys (e.g., backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // Auto-insert slashes based on the MM/DD/YYYY format
            if (!char.IsControl(e.KeyChar))
            {
                int length = textBox.Text.Length;
                if (length == 2 || length == 5)
                {
                    textBox.Text += "/";
                    textBox.SelectionStart = textBox.Text.Length; // Move the caret to the end
                }
            }
        }

        private void LoadCcodeList()
        {
            flowCcodePanel.Controls.Clear();
            selectedCcode = null;
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT DISTINCT ccode FROM employee WHERE ccode IS NOT NULL AND ccode != '' ORDER BY ccode";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ccode = reader.GetString(0);
                            var panel = new Panel
                            {
                                Width = flowCcodePanel.Width - 25,
                                Height = 32,
                                BorderStyle = BorderStyle.FixedSingle,
                                Margin = new Padding(2),
                                Cursor = Cursors.Hand,
                                Tag = ccode
                            };
                            var lbl = new Label
                            {
                                AutoSize = false,
                                Width = panel.Width,
                                Height = panel.Height,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                                Text = ccode,
                                Cursor = Cursors.Hand
                            };
                            panel.Controls.Add(lbl);
                            panel.Click += (s, e) => SelectCcodePanel(panel, ccode);
                            lbl.Click += (s, e) => SelectCcodePanel(panel, ccode);
                            flowCcodePanel.Controls.Add(panel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ccode list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectCcodePanel(Panel panel, string ccode)
        {
            // Highlight selected panel
            foreach (Panel p in flowCcodePanel.Controls)
                p.BackColor = SystemColors.Control;
            panel.BackColor = Color.LightBlue;
            selectedCcode = ccode;
        }

        private void process_Click(object sender, EventArgs e)
        {
            // Validate date input
            if (!DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out DateTime endDate))
            {
                MessageBox.Show("Invalid date format. Please use MM/DD/YYYY.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected ccode
            if (string.IsNullOrEmpty(selectedCcode))
            {
                MessageBox.Show("Please select a CCode to process.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Check if the date range exactly matches any payroll period
                    string checkQuery = "SELECT COUNT(*) FROM payroll WHERE pay_period_start = @startDate AND pay_period_end = @endDate";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@startDate", startDate);
                        checkCmd.Parameters.AddWithValue("@endDate", endDate);
                        var count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            MessageBox.Show("No payroll records found for the exact date range. Please check your dates.", "No Payroll Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string query = @"SELECT e.id_no, e.lname, e.fname, e.mname, e.ccode, p.netpay
                                    FROM employee e
                                    JOIN payroll p ON e.id_no = p.employee_id
                                    WHERE p.pay_period_start >= @startDate AND p.pay_period_end <= @endDate
                                    AND e.ccode = @ccode
                                    ORDER BY e.lname, e.fname, e.id_no";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);
                        cmd.Parameters.AddWithValue("@ccode", selectedCcode);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show("No records found for the selected date range and CCode.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            using (SaveFileDialog sfd = new SaveFileDialog())
                            {
                                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                                sfd.FileName = $"payroll_list_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    using (var workbook = new XLWorkbook())
                                    {
                                        var worksheet = workbook.Worksheets.Add("Payroll List");
                                        // Write header
                                        worksheet.Cell(1, 1).Value = "ID No";
                                        worksheet.Cell(1, 2).Value = "Last Name";
                                        worksheet.Cell(1, 3).Value = "First Name";
                                        worksheet.Cell(1, 4).Value = "Middle Name";
                                        worksheet.Cell(1, 5).Value = "CCode";
                                        worksheet.Cell(1, 6).Value = "Net Pay";

                                        int row = 2;
                                        while (reader.Read())
                                        {
                                            worksheet.Cell(row, 1).Value = reader["id_no"]?.ToString();
                                            worksheet.Cell(row, 2).Value = reader["lname"]?.ToString();
                                            worksheet.Cell(row, 3).Value = reader["fname"]?.ToString();
                                            worksheet.Cell(row, 4).Value = reader["mname"]?.ToString();
                                            worksheet.Cell(row, 5).Value = reader["ccode"]?.ToString();
                                            worksheet.Cell(row, 6).Value = reader["netpay"]?.ToString();
                                            row++;
                                        }
                                        worksheet.Columns().AdjustToContents();
                                        workbook.SaveAs(sfd.FileName);
                                    }
                                    MessageBox.Show("Payroll list exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting payroll list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
