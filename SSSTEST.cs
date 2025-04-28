using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class SSSTEST : Form
    {
        public SSSTEST()
        {
            InitializeComponent();
        }

        private void compute_Click(object sender, EventArgs e)
        {
            // Read the grossPay value from the TextBox
            if (decimal.TryParse(grossPay.Text, out decimal parsedGrossPay))
            {
                // Calculate SSS based on the parsedGrossPay
                decimal sss = CalculateSSS(parsedGrossPay);

                // Display the result in the result TextBox
                result.Text = sss.ToString("F2");
            }
            else
            {
                MessageBox.Show("Please enter a valid gross pay amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private decimal CalculateSSS(decimal grossPay)
        {
            decimal sss = 0;

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT salary1, salary2, EETotal FROM sssgovdues";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
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
            }

            return sss;
        }
    }
}
