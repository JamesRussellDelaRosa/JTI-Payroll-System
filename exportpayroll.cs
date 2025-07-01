using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;

namespace JTI_Payroll_System
{
    public partial class exportpayroll : Form
    {
        public exportpayroll()
        {
            InitializeComponent();
            InitializeCustomComponents();
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
                textBox.ForeColor = Color.Black;
            }
        }

        private void AddHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "MM/DD/YYYY";
                textBox.ForeColor = Color.Gray;
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void export_Click(object sender, EventArgs e)
        {
            // Validate date input
            if (!DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                MessageBox.Show("Invalid date format. Please use MM/DD/YYYY.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!postedpayroll.Checked)
            {
                MessageBox.Show("Please check 'POSTED PAYROLL' to export posted payroll data.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT employee_id, lname, fname, mname, total_days, basicpay, td_ut, trdypay, legal_holiday_count, lhpay, overtime_hours, regotpay, restday_hours, rdpay, restday_overtime_hours, rdotpay, legal_holiday_hours, lhhrspay, legal_holiday_overtime_hours, lhothrspay, lhrd_hours, lhrdpay, lhrd_overtime_hours, lhrdotpay, special_holiday_hours, shpay, special_holiday_overtime_hours, shotpay, special_holiday_restday_hours, shrdpay, special_holiday_restday_overtime_hours, shrdotpay, nd_hrs, ndpay, ndot_hrs, ndotpay, ndrd_hrs, ndrdpay, ndsh_hrs, ndshpay, ndshrd_hrs, ndshrdpay, ndlh_hrs, ndlhpay, ndlhrd_hrs, ndlhrdpay, ndrdot_hrs, ndshot_hrs, ndshrdot_hrs, ndlhot_hrs, ndlhrdot_hrs, non_working_day_count, total_earnings, total_basic_pay, total_ot_pay, gross_pay FROM payroll WHERE pay_period_start = @startDate AND pay_period_end = @endDate";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show("No posted payroll data found for the selected date range.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            // SaveFileDialog for XLSX
                            using (SaveFileDialog sfd = new SaveFileDialog())
                            {
                                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                                sfd.FileName = $"payroll_export_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    using (var workbook = new XLWorkbook())
                                    {
                                        var worksheet = workbook.Worksheets.Add("Payroll");
                                        int colCount = reader.FieldCount;
                                        // Write header
                                        for (int i = 0; i < colCount; i++)
                                        {
                                            worksheet.Cell(1, i + 1).Value = reader.GetName(i);
                                        }
                                        int row = 2;
                                        // Write data
                                        while (reader.Read())
                                        {
                                            for (int i = 0; i < colCount; i++)
                                            {
                                                worksheet.Cell(row, i + 1).Value = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString();
                                            }
                                            row++;
                                        }
                                        worksheet.Columns().AdjustToContents();
                                        workbook.SaveAs(sfd.FileName);
                                    }
                                    MessageBox.Show("Payroll data exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting payroll data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
