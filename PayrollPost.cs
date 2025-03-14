using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class PayrollPost : Form
    {
        public PayrollPost()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Assuming fromDate and toDate are already added to the form
            fromdate.Enter += new EventHandler(RemoveHint);
            fromdate.Leave += new EventHandler(AddHint);
            todate.Enter += new EventHandler(RemoveHint);
            todate.Leave += new EventHandler(AddHint);

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

        private void CalculateAndSavePayroll(DateTime startDate, DateTime endDate, int month, int payrollyear, int controlPeriod)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Fetch employee IDs with attendance in the given date range
                    List<string> employeeIDs = new List<string>();
                    string query = @"
            SELECT DISTINCT e.id_no
            FROM employee e
            LEFT JOIN processedDTR p ON e.id_no = p.employee_id AND p.date BETWEEN @startDate AND @endDate
            LEFT JOIN attendance a ON e.id_no = a.id AND a.date BETWEEN @startDate AND @endDate
            WHERE p.employee_id IS NOT NULL OR a.id IS NOT NULL
            ORDER BY e.id_no ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employeeIDs.Add(reader["id_no"].ToString());
                            }
                        }
                    }

                    foreach (string employeeID in employeeIDs)
                    {
                        // Calculate total hours, overtime hours, and earnings
                        decimal totalHours = 0;
                        decimal overtimeHours = 0;
                        decimal totalEarnings = 0;
                        decimal totalDays = 0;

                        query = @"
                SELECT p.working_hours, p.ot_hrs, p.rate, s.regular_hours
                FROM processedDTR p
                JOIN ShiftCodes s ON p.shift_code = s.shift_code
                WHERE p.employee_id = @employeeID AND p.date BETWEEN @startDate AND @endDate";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeID", employeeID);
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.Parameters.AddWithValue("@endDate", endDate);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    decimal workingHours = reader.GetDecimal("working_hours");
                                    decimal otHours = reader.GetDecimal("ot_hrs");
                                    decimal rate = reader.GetDecimal("rate");
                                    decimal regularHours = reader.GetDecimal("regular_hours");

                                    totalHours += workingHours;
                                    overtimeHours += otHours;
                                    totalEarnings += (workingHours + otHours) * rate;

                                    // Convert working hours to days based on regular hours for each day
                                    if (regularHours > 0)
                                    {
                                        totalDays += workingHours / regularHours;
                                    }
                                }
                            }
                        }

                        // Insert payroll data into payroll table
                        string insertQuery = @"
                INSERT INTO payroll (employee_id, pay_period_start, pay_period_end, total_days, overtime_hours, total_earnings, month, payrollyear, control_period)
                VALUES (@employeeID, @startDate, @endDate, @totalDays, @overtimeHours, @totalEarnings, @month, @payrollyear, @controlPeriod)";

                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            insertCmd.Parameters.AddWithValue("@startDate", startDate);
                            insertCmd.Parameters.AddWithValue("@endDate", endDate);
                            insertCmd.Parameters.AddWithValue("@totalDays", totalDays);
                            insertCmd.Parameters.AddWithValue("@overtimeHours", overtimeHours);
                            insertCmd.Parameters.AddWithValue("@totalEarnings", totalEarnings);
                            insertCmd.Parameters.AddWithValue("@month", month);
                            insertCmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                            insertCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);

                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Payroll data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating and saving payroll: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSavePayroll_Click_1(object sender, EventArgs e)
        {
            if (!DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate) ||
                !int.TryParse(month.Text, out int parsedMonth) ||
                !int.TryParse(payrollyear.Text, out int parsedPayrollYear) ||
                !int.TryParse(controlPeriod.Text, out int parsedControlPeriod))
            {
                MessageBox.Show("Invalid input. Please enter valid dates (MM/DD/YYYY) and numeric values for month, payroll year, and control period.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalculateAndSavePayroll(startDate, endDate, parsedMonth, parsedPayrollYear, parsedControlPeriod);
        }
    }
}
