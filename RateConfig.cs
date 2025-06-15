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
                                string rate = reader.GetDecimal("defaultrate").ToString("F5"); // Changed F2 to F5
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
                                basicpay.Text = reader["basicpay"].ToString();
                                rdpay.Text = reader["rdpay"].ToString();
                                rdotpay.Text = reader["rdotpay"].ToString();
                                lhpay.Text = reader["lhpay"].ToString();
                                regotpay.Text = reader["regotpay"].ToString();
                                trdypay.Text = reader["trdypay"].ToString();
                                lhhrspay.Text = reader["lhhrspay"].ToString();
                                lhothrspay.Text = reader["lhothrspay"].ToString();
                                lhrdpay.Text = reader["lhrdpay"].ToString();
                                lhrdotpay.Text = reader["lhrdotpay"].ToString();
                                shpay.Text = reader["shpay"].ToString();
                                shotpay.Text = reader["shotpay"].ToString();
                                shrdpay.Text = reader["shrdpay"].ToString();
                                shrdotpay.Text = reader["shrdotpay"].ToString();
                                ndpay.Text = reader["ndpay"].ToString();
                                ndotpay.Text = reader["ndotpay"].ToString();
                                ndrdpay.Text = reader["ndrdpay"].ToString();
                                ndshpay.Text = reader["ndshpay"].ToString();
                                ndshrdpay.Text = reader["ndshrdpay"].ToString();
                                ndlhpay.Text = reader["ndlhpay"].ToString();
                                ndlhrdpay.Text = reader["ndlhrdpay"].ToString();
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
                rdpay.Text = (rate / 8 * 1.3m).ToString("F5");
                rdotpay.Text = (rate / 8 * 1.69m).ToString("F5");
                lhpay.Text = "520.00000";
                regotpay.Text = (rate / 8 * 1.25m).ToString("F5");
                trdypay.Text = (-rate / 8).ToString("F5");
                lhhrspay.Text = (rate / 8).ToString("F5");
                lhothrspay.Text = (rate / 8 * 2.6m).ToString("F5");
                lhrdpay.Text = (rate / 8 * 2.6m).ToString("F5");
                lhrdotpay.Text = (rate / 8 * 3.38m).ToString("F5");
                shpay.Text = (rate / 8 * 1.3m).ToString("F5");
                shotpay.Text = (rate / 8 * 1.69m).ToString("F5");
                shrdpay.Text = (rate / 8 * 1.5m).ToString("F5");
                shrdotpay.Text = (rate / 8 * 1.95m).ToString("F5");
                ndpay.Text = (rate / 8 * 0.1m).ToString("F5");
                ndotpay.Text = (rate / 8 * 1.25m * 0.1m).ToString("F5");
                ndrdpay.Text = (rate / 8 * 1.3m * 0.1m).ToString("F5");
                ndshpay.Text = (rate / 8 * 1.3m * 0.1m).ToString("F5");
                ndshrdpay.Text = (rate / 8 * 1.5m * 0.1m).ToString("F5");
                ndlhpay.Text = (rate / 8 * 2.0m * 0.1m).ToString("F5");
                ndlhrdpay.Text = (rate / 8 * 2.6m * 0.1m).ToString("F5");
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
