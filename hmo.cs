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
    public partial class hmo : Form
    {
        public hmo()
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

        /// <summary>
        /// Updates the payroll.hmo field to 132.5 for all employees with active_hmo = 1 for the specified pay period.
        /// </summary>
        /// <param name="payPeriodStart">The start date of the pay period.</param>
        /// <param name="payPeriodEnd">The end date of the pay period.</param>
        public void BatchUpdateHMO(DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    // Get all employees with active_hmo = 1
                    string employeeQuery = "SELECT id_no FROM employee WHERE active_hmo = 1";
                    List<string> employeeIds = new List<string>();
                    using (MySqlCommand employeeCmd = new MySqlCommand(employeeQuery, connection))
                    using (MySqlDataReader reader = employeeCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employeeIds.Add(reader["id_no"].ToString());
                        }
                    }

                    // Update payroll for each employee for the given period
                    foreach (string employeeId in employeeIds)
                    {
                        string updatePayrollQuery = "UPDATE payroll SET hmo = 132.5 WHERE employee_id = @employee_id AND pay_period_start = @from_date AND pay_period_end = @to_date AND reliever = 0";
                        using (MySqlCommand updatePayrollCmd = new MySqlCommand(updatePayrollQuery, connection))
                        {
                            updatePayrollCmd.Parameters.AddWithValue("@employee_id", employeeId);
                            updatePayrollCmd.Parameters.AddWithValue("@from_date", payPeriodStart.ToString("yyyy-MM-dd"));
                            updatePayrollCmd.Parameters.AddWithValue("@to_date", payPeriodEnd.ToString("yyyy-MM-dd"));
                            updatePayrollCmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("HMO batch update completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating HMO: {ex.Message}");
            }
        }

        private void process_Click(object sender, EventArgs e)
        {
            // Parse dates from fromdate and todate TextBoxes
            if (!DateTime.TryParse(fromdate.Text, out DateTime payPeriodStart))
            {
                MessageBox.Show("Please enter a valid From date.");
                return;
            }
            if (!DateTime.TryParse(todate.Text, out DateTime payPeriodEnd))
            {
                MessageBox.Show("Please enter a valid To date.");
                return;
            }

            // Check if the pay period exists in the payroll table
            bool periodExists = false;
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string checkQuery = "SELECT COUNT(*) FROM payroll WHERE pay_period_start = @from_date AND pay_period_end = @to_date";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@from_date", payPeriodStart.ToString("yyyy-MM-dd"));
                    checkCmd.Parameters.AddWithValue("@to_date", payPeriodEnd.ToString("yyyy-MM-dd"));
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    periodExists = count > 0;
                }
            }

            if (!periodExists)
            {
                MessageBox.Show("The selected date range does not match any payroll period. Please enter a correct range of dates.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BatchUpdateHMO(payPeriodStart, payPeriodEnd);
        }
    }
}
