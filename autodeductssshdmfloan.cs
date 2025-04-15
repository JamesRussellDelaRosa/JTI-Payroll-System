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

            fromdate.PlaceholderText = "MM/DD/YYYY";
            todate.PlaceholderText = "MM/DD/YYYY";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve input values from textboxes
            string month = this.month.Text; // Replace with the actual textbox name for month
            string year = this.year.Text; // Replace with the actual textbox name for year
            string controlPeriod = controlperiod.Text; // Replace with the actual textbox name for control period
            string fromDateInput = fromdate.Text; // Replace with the actual textbox name for from date
            string toDateInput = todate.Text; // Replace with the actual textbox name for to date

            if (string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year) || string.IsNullOrEmpty(controlPeriod) ||
                string.IsNullOrEmpty(fromDateInput) || string.IsNullOrEmpty(toDateInput))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            // Validate and format dates
            if (!DateTime.TryParseExact(fromDateInput, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fromDate))
            {
                MessageBox.Show("Please enter a valid From Date in MM/DD/YYYY format.");
                return;
            }

            if (!DateTime.TryParseExact(toDateInput, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime toDate))
            {
                MessageBox.Show("Please enter a valid To Date in MM/DD/YYYY format.");
                return;
            }

            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Check if there is already data in hdmf_loan for the specified cutoff period
                    string checkQuery = "SELECT COUNT(*) FROM payroll WHERE month = @month AND payrollyear = @year " +
                                        "AND control_period = @control_period AND pay_period_start = @from_date " +
                                        "AND pay_period_end = @to_date AND hdmf_loan > 0";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@month", month);
                        checkCommand.Parameters.AddWithValue("@year", year);
                        checkCommand.Parameters.AddWithValue("@control_period", controlPeriod);
                        checkCommand.Parameters.AddWithValue("@from_date", fromDate.ToString("yyyy-MM-dd")); // Format for SQL
                        checkCommand.Parameters.AddWithValue("@to_date", toDate.ToString("yyyy-MM-dd")); // Format for SQL

                        int existingRecords = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingRecords > 0)
                        {
                            // If data already exists, show a message and stop processing
                            MessageBox.Show("This cutoff has already been deducted. No further processing is allowed.",
                                            "Cutoff Already Processed",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Retrieve matching payroll records excluding relievers
                    string payrollQuery = "SELECT employee_id FROM payroll WHERE month = @month AND payrollyear = @year " +
                                          "AND control_period = @control_period AND pay_period_start = @from_date AND pay_period_end = @to_date " +
                                          "AND reliever = 0"; // Assuming 'reliever' is the column indicating reliever status
                    using (MySqlCommand payrollCommand = new MySqlCommand(payrollQuery, connection))
                    {
                        payrollCommand.Parameters.AddWithValue("@month", month);
                        payrollCommand.Parameters.AddWithValue("@year", year);
                        payrollCommand.Parameters.AddWithValue("@control_period", controlPeriod);
                        payrollCommand.Parameters.AddWithValue("@from_date", fromDate.ToString("yyyy-MM-dd")); // Format for SQL
                        payrollCommand.Parameters.AddWithValue("@to_date", toDate.ToString("yyyy-MM-dd")); // Format for SQL

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
                // Check if the employee is marked as a reliever
                string checkRelieverQuery = "SELECT reliever FROM payroll WHERE employee_id = @employee_id";
                bool isReliever = false;

                using (MySqlCommand checkRelieverCommand = new MySqlCommand(checkRelieverQuery, connection))
                {
                    checkRelieverCommand.Parameters.AddWithValue("@employee_id", employeeId);
                    object result = checkRelieverCommand.ExecuteScalar();
                    if (result != null)
                    {
                        isReliever = Convert.ToBoolean(result);
                    }
                }

                // If the employee is a reliever, skip updating the hdmf_loan
                if (isReliever)
                {
                    return;
                }

                // Retrieve deduction_pay and loan_amount from HDMFLOAN table
                string getLoanDataQuery = "SELECT deduction_pay, loan_amount FROM hdmfloan WHERE employee_id = @employee_id";
                decimal deductionPay = 0;
                decimal loanAmount = 0;

                using (MySqlCommand getLoanDataCommand = new MySqlCommand(getLoanDataQuery, connection))
                {
                    getLoanDataCommand.Parameters.AddWithValue("@employee_id", employeeId);
                    using (MySqlDataReader reader = getLoanDataCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            deductionPay = Convert.ToDecimal(reader["deduction_pay"]);
                            loanAmount = Convert.ToDecimal(reader["loan_amount"]);
                        }
                        else
                        {
                            // Skip if no HDMF Loan data is found for the employee
                            return;
                        }
                    }
                }

                // Deduct the monthly amortization from the loan amount
                decimal newLoanAmount = loanAmount - deductionPay;

                // Ensure the loan amount does not go below zero
                if (newLoanAmount < 0)
                {
                    newLoanAmount = 0;
                }

                // Update the loan amount in the HDMFLOAN table
                string updateLoanAmountQuery = "UPDATE hdmfloan SET loan_amount = @new_loan_amount WHERE employee_id = @employee_id";
                using (MySqlCommand updateLoanAmountCommand = new MySqlCommand(updateLoanAmountQuery, connection))
                {
                    updateLoanAmountCommand.Parameters.AddWithValue("@new_loan_amount", newLoanAmount);
                    updateLoanAmountCommand.Parameters.AddWithValue("@employee_id", employeeId);

                    updateLoanAmountCommand.ExecuteNonQuery();
                }

                // Update hdmf_loan in payroll table, ensuring relievers are excluded
                string updatePayrollQuery = "UPDATE payroll SET hdmf_loan = @hdmf_loan WHERE employee_id = @employee_id AND reliever = 0";
                using (MySqlCommand updatePayrollCommand = new MySqlCommand(updatePayrollQuery, connection))
                {
                    updatePayrollCommand.Parameters.AddWithValue("@hdmf_loan", deductionPay);
                    updatePayrollCommand.Parameters.AddWithValue("@employee_id", employeeId);

                    updatePayrollCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating HDMF Loan for Employee ID {employeeId}: {ex.Message}");
            }
        }


    }
}
