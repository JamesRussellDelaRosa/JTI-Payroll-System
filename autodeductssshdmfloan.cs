using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class autodeductssshdmfloan : Form
    {
        public autodeductssshdmfloan()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string month = this.month.Text;
            string year = this.year.Text; 
            string controlPeriod = controlperiod.Text;
            string fromDate = fromdate.Text; 
            string toDate = todate.Text;

            if (string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year) || string.IsNullOrEmpty(controlPeriod) ||
                string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Retrieve matching payroll records
                    string payrollQuery = "SELECT employee_id FROM payroll WHERE month = @month AND year = @year " +
                                          "AND control_period = @control_period AND from_date = @from_date AND to_date = @to_date";
                    using (MySqlCommand payrollCommand = new MySqlCommand(payrollQuery, connection))
                    {
                        payrollCommand.Parameters.AddWithValue("@month", month);
                        payrollCommand.Parameters.AddWithValue("@year", year);
                        payrollCommand.Parameters.AddWithValue("@control_period", controlPeriod);
                        payrollCommand.Parameters.AddWithValue("@from_date", fromDate);
                        payrollCommand.Parameters.AddWithValue("@to_date", toDate);

                        using (MySqlDataReader reader = payrollCommand.ExecuteReader())
                        {
                            List<string> employeeIds = new List<string>();

                            while (reader.Read())
                            {
                                employeeIds.Add(reader["employee_id"].ToString());
                            }

                            if (employeeIds.Count == 0)
                            {
                                MessageBox.Show("No matching payroll records found.");
                                return;
                            }

                            reader.Close();

                            // Update hdmf_loan for each employee
                            foreach (string employeeId in employeeIds)
                            {
                                UpdateHdmfLoan(connection, employeeId);
                            }

                            MessageBox.Show("HDMF Loan deductions updated successfully.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateHdmfLoan(MySqlConnection connection, string employeeId)
        {
            try
            {
                // Retrieve deduction_pay from HDMFLOAN table
                string getDeductionQuery = "SELECT deduction_pay FROM hdmfloan WHERE employee_id = @employee_id";
                decimal deductionPay = 0;

                using (MySqlCommand getDeductionCommand = new MySqlCommand(getDeductionQuery, connection))
                {
                    getDeductionCommand.Parameters.AddWithValue("@employee_id", employeeId);
                    object result = getDeductionCommand.ExecuteScalar();
                    if (result != null)
                    {
                        deductionPay = Convert.ToDecimal(result);
                    }
                    else
                    {
                        // Skip if no HDMF Loan data is found for the employee
                        return;
                    }
                }

                // Update hdmf_loan in payroll table
                string updateQuery = "UPDATE payroll SET hdmf_loan = @hdmf_loan WHERE employee_id = @employee_id";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@hdmf_loan", deductionPay);
                    updateCommand.Parameters.AddWithValue("@employee_id", employeeId);

                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating HDMF Loan for Employee ID {employeeId}: {ex.Message}");
            }
        }

    }
}
