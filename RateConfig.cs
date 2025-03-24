using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class RateConfig : Form
    {

        public RateConfig()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void autoCompute_Click_1(object sender, EventArgs e)
        {
            basicpay.Text = defaultrate.Text;

            if (decimal.TryParse(ratecompute.Text, out decimal rate))
            {
                rdpay.Text = (rate / 8 * 1.3m).ToString("F2");
                rdotpay.Text = (rate / 8 * 1.69m).ToString("F2");
                lhpay.Text = "520";
                regotpay.Text = (rate / 8 * 1.25m).ToString("F2");
                trdypay.Text = (-rate / 8).ToString("F2");
                lhhrspay.Text = (rate / 8).ToString("F2");
                lhothrspay.Text = (rate / 8 * 2.6m).ToString("F2");
                lhrdpay.Text = (rate / 8 * 2.6m).ToString("F2");
                lhrdotpay.Text = (rate / 8 * 3.38m).ToString("F2");
                shpay.Text = (rate / 8 * 1.3m).ToString("F2");
                shotpay.Text = (rate / 8 * 1.69m).ToString("F2");
                shrdpay.Text = (rate / 8 * 1.5m).ToString("F2");
                shrdotpay.Text = (rate / 8 * 1.95m).ToString("F2");
                ndpay.Text = (rate / 8 * 0.1m).ToString("F2");
                ndotpay.Text = (rate / 8 * 1.25m * 0.1m).ToString("F2");
                ndrdpay.Text = (rate / 8 * 1.3m * 0.1m).ToString("F2");
                ndshpay.Text = (rate / 8 * 1.3m * 0.1m).ToString("F2");
                ndshrdpay.Text = (rate / 8 * 1.5m * 0.1m).ToString("F2");
                ndlhpay.Text = (rate / 8 * 2.0m * 0.1m).ToString("F2");
                ndlhrdpay.Text = (rate / 8 * 2.6m * 0.1m).ToString("F2");
            }
            else
            {
                MessageBox.Show("Please enter a valid number in the RateCompute textbox.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void saveRate_Click_1(object sender, EventArgs e)
        {
            SaveRateToDatabase();
        }

        private void SaveRateToDatabase()
        {
            string query = @"
                INSERT INTO rate (
                    defaultrate, ratecompute, basicpay, rdpay, rdotpay, lhpay, regotpay, trdypay, lhhrspay, lhothrspay, lhrdpay, lhrdotpay, shpay, shotpay, shrdpay, shrdotpay, ndpay, ndotpay, ndrdpay, ndshpay, ndshrdpay, ndlhpay, ndlhrdpay
                ) VALUES (
                    @defaultrate, @ratecompute, @basicpay, @rdpay, @rdotpay, @lhpay, @regotpay, @trdypay, @lhhrspay, @lhothrspay, @lhrdpay, @lhrdotpay, @shpay, @shotpay, @shrdpay, @shrdotpay, @ndpay, @ndotpay, @ndrdpay, @ndshpay, @ndshrdpay, @ndlhpay, @ndlhrdpay
                )";

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@defaultrate", defaultrate.Text);
                        command.Parameters.AddWithValue("@ratecompute", ratecompute.Text);
                        command.Parameters.AddWithValue("@basicpay", basicpay.Text);
                        command.Parameters.AddWithValue("@rdpay", rdpay.Text);
                        command.Parameters.AddWithValue("@rdotpay", rdotpay.Text);
                        command.Parameters.AddWithValue("@lhpay", lhpay.Text);
                        command.Parameters.AddWithValue("@regotpay", regotpay.Text);
                        command.Parameters.AddWithValue("@trdypay", trdypay.Text);
                        command.Parameters.AddWithValue("@lhhrspay", lhhrspay.Text);
                        command.Parameters.AddWithValue("@lhothrspay", lhothrspay.Text);
                        command.Parameters.AddWithValue("@lhrdpay", lhrdpay.Text);
                        command.Parameters.AddWithValue("@lhrdotpay", lhrdotpay.Text);
                        command.Parameters.AddWithValue("@shpay", shpay.Text);
                        command.Parameters.AddWithValue("@shotpay", shotpay.Text);
                        command.Parameters.AddWithValue("@shrdpay", shrdpay.Text);
                        command.Parameters.AddWithValue("@shrdotpay", shrdotpay.Text);
                        command.Parameters.AddWithValue("@ndpay", ndpay.Text);
                        command.Parameters.AddWithValue("@ndotpay", ndotpay.Text);
                        command.Parameters.AddWithValue("@ndrdpay", ndrdpay.Text);
                        command.Parameters.AddWithValue("@ndshpay", ndshpay.Text);
                        command.Parameters.AddWithValue("@ndshrdpay", ndshrdpay.Text);
                        command.Parameters.AddWithValue("@ndlhpay", ndlhpay.Text);
                        command.Parameters.AddWithValue("@ndlhrdpay", ndlhrdpay.Text);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Rate saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to save rate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

