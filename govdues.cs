using System;
using System.Collections.Generic; // Required for List
using System.Linq; // Required for Any()
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class govdues : Form
    {
        public govdues()
        {
            InitializeComponent();

            fromdate.Enter += new EventHandler(RemoveHint);
            fromdate.Leave += new EventHandler(AddHint);
            fromdate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            todate.Enter += new EventHandler(RemoveHint);
            todate.Leave += new EventHandler(AddHint);
            todate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            AddHint(fromdate, null);
            AddHint(todate, null);
        }

        private void btngovdues_Click(object sender, EventArgs e)
        {
            // Get user input
            if (!int.TryParse(month.Text, out int Month) ||
                !int.TryParse(controlPeriod.Text, out int ControlPeriod) ||
                !int.TryParse(payrollyear.Text, out int PayrollYear) ||
                !DateTime.TryParse(fromdate.Text, out DateTime fromDate) ||
                !DateTime.TryParse(todate.Text, out DateTime toDate))
            {
                MessageBox.Show("Please enter valid values for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int updatedRecords = 0;
            int skippedRecords = 0;
            var recordsToProcess = new List<(int id, string employeeId, decimal grossPay, decimal basicPay, decimal rate, bool reliever, decimal existingSSS, decimal existingPhilhealth, decimal existingHDMF)>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Select all payroll records for the given period, including existing dues
                    string selectQuery = @"
                        SELECT id, employee_id, basic, gross, rate, reliever,
                               SSS, philhealth, hdmf
                        FROM payroll
                        WHERE pay_period_start = @fromDate
                          AND pay_period_end = @toDate
                          AND month = @month
                          AND payrollyear = @payrollYear
                          AND control_period = @controlPeriod";

                    using (var selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@fromDate", fromDate);
                        selectCmd.Parameters.AddWithValue("@toDate", toDate);
                        selectCmd.Parameters.AddWithValue("@month", Month);
                        selectCmd.Parameters.AddWithValue("@payrollYear", PayrollYear);
                        selectCmd.Parameters.AddWithValue("@controlPeriod", ControlPeriod);

                        using (var reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string employeeId = reader.GetString("employee_id");
                                decimal grossPay = reader.GetDecimal("gross");
                                decimal basicPay = reader.GetDecimal("basic");
                                decimal rate = reader.GetDecimal("rate");
                                bool reliever = reader.GetBoolean("reliever");

                                // Safely read existing dues, treating DBNull as 0
                                decimal existingSSS = reader.IsDBNull(reader.GetOrdinal("SSS")) ? 0m : reader.GetDecimal("SSS");
                                decimal existingPhilhealth = reader.IsDBNull(reader.GetOrdinal("philhealth")) ? 0m : reader.GetDecimal("philhealth");
                                decimal existingHDMF = reader.IsDBNull(reader.GetOrdinal("hdmf")) ? 0m : reader.GetDecimal("hdmf");

                                recordsToProcess.Add((id, employeeId, grossPay, basicPay, rate, reliever, existingSSS, existingPhilhealth, existingHDMF));
                            }
                            reader.Close(); // Close reader before potentially showing MessageBox or starting updates
                        }
                    }

                    // Check if any records have existing dues and notify the user
                    if (recordsToProcess.Any(rec => rec.existingSSS != 0m || rec.existingPhilhealth != 0m || rec.existingHDMF != 0m))
                    {
                        DialogResult dialogResult = MessageBox.Show(
                            "Some records already have existing government dues (SSS, PhilHealth, or HDMF). These records will be skipped. Do you want to proceed with updating the other records?",
                            "Existing Dues Found",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (dialogResult == DialogResult.No)
                        {
                            MessageBox.Show("Operation cancelled by user.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return; // Exit the method if user chooses not to proceed
                        }
                    }

                    // Proceed with updates after potential user confirmation
                    foreach (var rec in recordsToProcess)
                    {
                        // Check if government dues already exist (are non-zero) for this specific record
                        if (rec.existingSSS != 0m || rec.existingPhilhealth != 0m || rec.existingHDMF != 0m)
                        {
                            skippedRecords++;
                            continue; // Skip this record
                        }

                        // Calculate new values
                        decimal sss = CalculateSSS(rec.grossPay, rec.reliever);
                        decimal philhealth = CalculatePhilHealth(rec.rate, ControlPeriod, rec.reliever);
                        decimal hdmf = CalculateHDMF(rec.basicPay, ControlPeriod, rec.reliever, rec.employeeId, Month, PayrollYear);

                        // Update the payroll record
                        string updateQuery = @"
                            UPDATE payroll
                            SET SSS = @sss, philhealth = @philhealth, hdmf = @hdmf
                            WHERE id = @id";
                        using (var updateCmd = new MySqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@sss", sss);
                            updateCmd.Parameters.AddWithValue("@philhealth", philhealth);
                            updateCmd.Parameters.AddWithValue("@hdmf", hdmf);
                            updateCmd.Parameters.AddWithValue("@id", rec.id);
                            updateCmd.ExecuteNonQuery();
                            updatedRecords++;
                        }
                    }
                } // Connection is closed here

                string summaryMessage;
                if (updatedRecords == 0 && skippedRecords == 0)
                {
                    summaryMessage = "No payroll records found for the selected criteria to process.";
                }
                else
                {
                    summaryMessage = $"Government dues processing complete. {updatedRecords} record(s) updated. {skippedRecords} record(s) were skipped as dues already existed or they were relievers (for SSS).";
                }
                MessageBox.Show(summaryMessage, "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing government dues: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private decimal CalculateSSS(decimal grossPay, bool isReliever)
        {
            if (isReliever)
            {
                return 0; // SSS not computed for relievers
            }

            decimal sss = 0;
            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT salary1, salary2, EETotal FROM sssgovdues";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal salary1 = reader.GetDecimal("salary1");
                        decimal salary2 = reader.GetDecimal("salary2");
                        decimal eeTotal = reader.GetDecimal("EETotal");
                        if (grossPay >= salary1 && grossPay <= salary2)
                        {
                            sss = eeTotal;
                            break;
                        }
                    }
                }
            }
            return sss;
        }
        private decimal CalculatePhilHealth(decimal rate, int controlPeriod, bool isReliever)
        {
            if (isReliever) // PhilHealth might also not be applicable to relievers, adjust if needed
            {
                return 0;
            }
            if (controlPeriod == 1) // PhilHealth only for the first control period and non-relievers
            {
                return rate * 313 / 12 * 0.05m / 2;
            }
            return 0;
        }
        private decimal CalculateHDMF(decimal totalBasicPay, int controlPeriod, bool isReliever, string employeeID, int month, int payrollyear)
        {
            if (isReliever) // HDMF not computed for relievers
            {
                return 0;
            }

            if (controlPeriod == 1)
            {
                return totalBasicPay >= 10000 ? 200 : (totalBasicPay * 0.02m);
            }
            else if (controlPeriod == 2)
            {
                return CalculateSecondHalfHDMF(employeeID, month, payrollyear, totalBasicPay);
            }
            return 0;
        }
        private decimal CalculateSecondHalfHDMF(string employeeID, int month, int payrollyear, decimal currentBasicPay)
        {
            // This method already correctly handles non-relievers due to the check in CalculateHDMF
            decimal hdmf = 0;
            decimal firstHalfHDMF = 0;
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // The query for firstHalfHDMF already filters for reliever = 0
                    string firstHalfQuery = @"
                        SELECT hdmf
                        FROM payroll
                        WHERE employee_id = @employeeID
                        AND month = @month
                        AND payrollyear = @payrollyear
                        AND control_period = 1
                        AND reliever = 0"; // Ensures we only get first half HDMF for non-relievers
                    using (MySqlCommand cmd = new MySqlCommand(firstHalfQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                firstHalfHDMF = reader.GetDecimal("hdmf");
                                hdmf = 200 - firstHalfHDMF; // Assuming 200 is the max monthly HDMF
                                if (hdmf < 0) hdmf = 0;
                            }
                            else
                            {
                                // If no first half record, calculate fresh for current basic pay
                                hdmf = currentBasicPay >= 10000 ? 200 : (currentBasicPay * 0.02m);
                            }
                        }
                    }
                }
            }
            catch
            {
                // In case of error, default to a safe value or re-evaluate based on current basic pay
                hdmf = currentBasicPay >= 10000 ? 200 : (currentBasicPay * 0.02m); // Fallback, or simply return 0
            }
            return hdmf;
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

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (!char.IsControl(e.KeyChar))
            {
                int length = textBox.Text.Length;
                if (length == 2 || length == 5)
                {
                    textBox.Text += "/";
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }
    }
}