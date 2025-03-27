using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class RateConfig : Form
    {
        // Use a static field to ensure event handlers are only attached once across all instances
        private static bool eventHandlersInitialized = false;

        public RateConfig()
        {
            InitializeComponent();

            // First, remove any existing handlers to prevent duplicates
            deleteRateButton.Click -= deleteRateButton_Click;
            dropdownRate.SelectedIndexChanged -= dropdownRate_SelectedIndexChanged;

            // Then add the handlers
            deleteRateButton.Click += deleteRateButton_Click;
            dropdownRate.SelectedIndexChanged += dropdownRate_SelectedIndexChanged;

            // Load the data
            LoadRatesIntoDropdown();
        }

        private void LoadRatesIntoDropdown()
        {
            string query = "SELECT id, defaultrate FROM rate ORDER BY id DESC";

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dropdownRate.Items.Clear();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string rate = reader.GetDecimal("defaultrate").ToString("F2");
                                dropdownRate.Items.Add(new RateItem(id, rate));
                            }

                            if (dropdownRate.Items.Count > 0)
                                dropdownRate.SelectedIndex = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading rates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dropdownRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropdownRate.SelectedItem is RateItem selectedRate)
            {
                LoadRateDetails(selectedRate.Id);
            }
        }

        private void LoadRateDetails(int rateId)
        {
            string query = "SELECT * FROM rate WHERE id = @id LIMIT 1";

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", rateId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                defaultrate.Text = reader["defaultrate"].ToString();
                                ratecompute.Text = reader["ratecompute"].ToString();
                                basicpay.Text = reader["basic"].ToString();
                                rdpay.Text = reader["rd"].ToString();
                                rdotpay.Text = reader["rdot"].ToString();
                                lhpay.Text = reader["lh"].ToString();
                                regotpay.Text = reader["regot"].ToString();
                                trdypay.Text = reader["trdy"].ToString();
                                lhhrspay.Text = reader["lhhrs"].ToString();
                                lhothrspay.Text = reader["lhothrs"].ToString();
                                lhrdpay.Text = reader["lhrd"].ToString();
                                lhrdotpay.Text = reader["lhrdot"].ToString();
                                shpay.Text = reader["sh"].ToString();
                                shotpay.Text = reader["shot"].ToString();
                                shrdpay.Text = reader["shrd"].ToString();
                                shrdotpay.Text = reader["shrdot"].ToString();
                                ndpay.Text = reader["nd"].ToString();
                                ndotpay.Text = reader["ndot"].ToString();
                                ndrdpay.Text = reader["ndrd"].ToString();
                                ndshpay.Text = reader["ndsh"].ToString();
                                ndshrdpay.Text = reader["ndshrd"].ToString();
                                ndlhpay.Text = reader["ndlh"].ToString();
                                ndlhrdpay.Text = reader["ndlhrd"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading rate details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

        private void saveRate_Click_1(object sender, EventArgs e)
        {
            SaveRateToDatabase();
            LoadRatesIntoDropdown(); // Refresh the dropdown after saving
        }

        private void SaveRateToDatabase()
        {
            string query = @"
                INSERT INTO rate (
                    defaultrate, ratecompute, basic, rd, rdot, lh, regot, trdy, lhhrs, lhothrs, lhrd, lhrdot, sh, shot, shrd, shrdot, nd, ndot, ndrd, ndsh, ndshrd, ndlh, ndlhrd
                ) VALUES (
                    @defaultrate, @ratecompute, @basic, @rd, @rdot, @lh, @regot, @trdy, @lhhrs, @lhothrs, @lhrd, @lhrdot, @sh, @shot, @shrd, @shrdot, @nd, @ndot, @ndrd, @ndsh, @ndshrd, @ndlh, @ndlhrd
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
                        command.Parameters.AddWithValue("@basic", basicpay.Text);
                        command.Parameters.AddWithValue("@rd", rdpay.Text);
                        command.Parameters.AddWithValue("@rdot", rdotpay.Text);
                        command.Parameters.AddWithValue("@lh", lhpay.Text);
                        command.Parameters.AddWithValue("@regot", regotpay.Text);
                        command.Parameters.AddWithValue("@trdy", trdypay.Text);
                        command.Parameters.AddWithValue("@lhhrs", lhhrspay.Text);
                        command.Parameters.AddWithValue("@lhothrs", lhothrspay.Text);
                        command.Parameters.AddWithValue("@lhrd", lhrdpay.Text);
                        command.Parameters.AddWithValue("@lhrdot", lhrdotpay.Text);
                        command.Parameters.AddWithValue("@sh", shpay.Text);
                        command.Parameters.AddWithValue("@shot", shotpay.Text);
                        command.Parameters.AddWithValue("@shrd", shrdpay.Text);
                        command.Parameters.AddWithValue("@shrdot", shrdotpay.Text);
                        command.Parameters.AddWithValue("@nd", ndpay.Text);
                        command.Parameters.AddWithValue("@ndot", ndotpay.Text);
                        command.Parameters.AddWithValue("@ndrd", ndrdpay.Text);
                        command.Parameters.AddWithValue("@ndsh", ndshpay.Text);
                        command.Parameters.AddWithValue("@ndshrd", ndshrdpay.Text);
                        command.Parameters.AddWithValue("@ndlh", ndlhpay.Text);
                        command.Parameters.AddWithValue("@ndlhrd", ndlhrdpay.Text);

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

        private void deleteRateButton_Click(object sender, EventArgs e)
        {
            // Prevent the handler from being called multiple times
            deleteRateButton.Click -= deleteRateButton_Click;

            try
            {
                // Disable the button to prevent multiple clicks
                deleteRateButton.Enabled = false;

                if (dropdownRate.SelectedItem is RateItem selectedRate)
                {
                    // Store the ID locally to ensure it doesn't change during the operation
                    int rateIdToDelete = selectedRate.Id;

                    // Add confirmation dialog
                    DialogResult result = MessageBox.Show(
                        $"Are you sure you want to delete rate {selectedRate.Rate}?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        DeleteRateFromDatabase(rateIdToDelete);
                        LoadRatesIntoDropdown(); // Refresh the dropdown after deleting
                    }
                }
                else
                {
                    MessageBox.Show("Please select a rate to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                // Re-enable the button and re-attach the handler
                deleteRateButton.Enabled = true;
                deleteRateButton.Click += deleteRateButton_Click;
            }
        }

        private void DeleteRateFromDatabase(int rateId)
        {
            // Use LIMIT 1 to ensure only one record is deleted
            string query = "DELETE FROM rate WHERE id = @id LIMIT 1";

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", rateId);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Rate deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete rate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private class RateItem
        {
            public int Id { get; set; }
            public string Rate { get; set; }

            public RateItem(int id, string rate)
            {
                Id = id;
                Rate = rate;
            }

            public override string ToString()
            {
                return Rate;
            }
        }
    }
}
