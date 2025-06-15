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

namespace JTI_Payroll_System
{
    public partial class generatepayslip : Form
    {
        public generatepayslip()
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

        private void UpdateNetPay(int month, int controlPeriod, int payrollYear, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string selectQuery = @"SELECT id, total_grosspay, sil, perfect_attendance, adjustment_total, reliever_total FROM payroll WHERE month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod AND pay_period_start = @fromDate AND pay_period_end = @toDate";
                    using (var cmd = new MySqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                        cmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var updates = new List<(int id, decimal netpay)>();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                decimal totalGrossPay = reader.IsDBNull(reader.GetOrdinal("total_grosspay")) ? 0 : reader.GetDecimal("total_grosspay");
                                decimal sil = reader.IsDBNull(reader.GetOrdinal("sil")) ? 0 : reader.GetDecimal("sil");
                                decimal perfectAttendance = reader.IsDBNull(reader.GetOrdinal("perfect_attendance")) ? 0 : reader.GetDecimal("perfect_attendance");
                                decimal adjustmentTotal = reader.IsDBNull(reader.GetOrdinal("adjustment_total")) ? 0 : reader.GetDecimal("adjustment_total");
                                decimal relieverTotal = reader.IsDBNull(reader.GetOrdinal("reliever_total")) ? 0 : reader.GetDecimal("reliever_total");
                                decimal netpay = totalGrossPay + sil + perfectAttendance + adjustmentTotal + relieverTotal;
                                netpay = Math.Round(netpay, 2, MidpointRounding.AwayFromZero); // Round to 2 decimal places
                                updates.Add((id, netpay));
                            }
                            reader.Close();
                            foreach (var upd in updates)
                            {
                                string updateQuery = "UPDATE payroll SET netpay = @netpay WHERE id = @id";
                                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@netpay", upd.netpay);
                                    updateCmd.Parameters.AddWithValue("@id", upd.id);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating netpay: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void generate_Click(object sender, EventArgs e)
        {
            // Parse input fields
            if (!int.TryParse(month.Text, out int monthVal) ||
                !int.TryParse(controlPeriod.Text, out int controlPeriodVal) ||
                !int.TryParse(payrollyear.Text, out int payrollYearVal) ||
                !DateTime.TryParse(fromdate.Text, out DateTime fromDateVal) ||
                !DateTime.TryParse(todate.Text, out DateTime toDateVal))
            {
                MessageBox.Show("Invalid input. Please check your entries.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If regenerate is checked, update all rows; otherwise, only update where total_grosspay is NULL or 0
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string selectQuery = @"SELECT id, gross_pay, total_govdues, loan_deduction, total_deductions, total_grosspay FROM payroll WHERE month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod AND pay_period_start = @fromDate AND pay_period_end = @toDate";
                    using (var cmd = new MySqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@month", monthVal);
                        cmd.Parameters.AddWithValue("@payrollYear", payrollYearVal);
                        cmd.Parameters.AddWithValue("@controlPeriod", controlPeriodVal);
                        cmd.Parameters.AddWithValue("@fromDate", fromDateVal);
                        cmd.Parameters.AddWithValue("@toDate", toDateVal);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var updates = new List<(int id, decimal totalGrossPay, bool shouldUpdate)>();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                decimal grossPay = reader.IsDBNull(reader.GetOrdinal("gross_pay")) ? 0 : reader.GetDecimal("gross_pay");
                                decimal totalGovDues = reader.IsDBNull(reader.GetOrdinal("total_govdues")) ? 0 : reader.GetDecimal("total_govdues");
                                decimal loanDeduction = 0;
                                decimal totalDeductions = 0;
                                try { loanDeduction = reader.IsDBNull(reader.GetOrdinal("loan_deduction")) ? 0 : reader.GetDecimal("loan_deduction"); } catch { }
                                try { totalDeductions = reader.IsDBNull(reader.GetOrdinal("total_deductions")) ? 0 : reader.GetDecimal("total_deductions"); } catch { }
                                decimal totalGrossPay = grossPay - totalGovDues - loanDeduction - totalDeductions;
                                // If regenerate is checked, always update; otherwise, only update if total_grosspay is NULL or 0
                                bool isNull = reader.IsDBNull(reader.GetOrdinal("total_grosspay"));
                                decimal existing = isNull ? 0 : reader.GetDecimal("total_grosspay");
                                bool shouldUpdate = (regenerate != null && regenerate.Checked) ? true : (isNull || existing == 0);
                                updates.Add((id, totalGrossPay, shouldUpdate));
                            }
                            reader.Close();
                            // Update total_grosspay for each row if needed
                            foreach (var upd in updates)
                            {
                                if (upd.shouldUpdate)
                                {
                                    string updateQuery = "UPDATE payroll SET total_grosspay = @total_grosspay WHERE id = @id";
                                    using (var updateCmd = new MySqlCommand(updateQuery, conn))
                                    {
                                        updateCmd.Parameters.AddWithValue("@total_grosspay", upd.totalGrossPay);
                                        updateCmd.Parameters.AddWithValue("@id", upd.id);
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
                // After updating total_grosspay, update netpay
                UpdateNetPay(monthVal, controlPeriodVal, payrollYearVal, fromDateVal, toDateVal);
                MessageBox.Show("Payslip generation calculation and update complete.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during payslip generation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
