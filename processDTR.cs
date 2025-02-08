using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class processDTR : Form
    {
        private List<string> employeeIDs = new List<string>(); // Store employee IDs with attendance
        private int currentEmployeeIndex = 0; // Track current employee position

        public processDTR()
        {
            InitializeComponent();
        }

        private void filter_Click(object sender, EventArgs e)
        {
            // ✅ Convert TextBox values to DateTime
            if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
            {
                MessageBox.Show("Invalid date format. Please enter a valid date (YYYY-MM-DD).",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Load Employees for Navigation
            LoadEmployeesForNavigation(startDate, endDate);
        }

        private void LoadEmployeesForNavigation(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT e.id_no
                        FROM employee e
                        INNER JOIN attendance a ON e.id_no = a.id
                        WHERE a.date BETWEEN @startDate AND @endDate
                        ORDER BY e.id_no ASC;";  // Ensure ordered list

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            employeeIDs.Clear();  // Reset list
                            while (reader.Read())
                            {
                                employeeIDs.Add(reader["id_no"].ToString());
                            }
                        }
                    }

                    if (employeeIDs.Count > 0)
                    {
                        currentEmployeeIndex = 0;  // Start at the first employee
                        LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], startDate, endDate);
                    }
                    else
                    {
                        MessageBox.Show("No employees found in the selected date range.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Navigation Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployeeDTR(string employeeID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT e.id_no, e.fname, e.lname, a.date, 
                               MIN(a.time) AS TimeIn, MAX(a.time) AS TimeOut
                        FROM employee e
                        INNER JOIN attendance a ON e.id_no = a.id
                        WHERE e.id_no = @employeeID AND a.date BETWEEN @startDate AND @endDate
                        GROUP BY e.id_no, a.date
                        ORDER BY a.date ASC;";

                    DataTable dt = new DataTable(); // ✅ Create DataTable to store results

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textID.Text = reader["id_no"].ToString();
                                textName.Text = reader["fname"].ToString() + " " + reader["lname"].ToString();
                            }
                        }

                        // ✅ Reset the command for the adapter
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd)) // ✅ Fix: New MySqlCommand instance is used
                        {
                            adapter.Fill(dt);
                        }
                    }

                    dgvDTR.DataSource = dt;  // ✅ Load DataGridView
                }

                // Add dropdown column after loading data
                AddRateDropdownToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "DTR Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (employeeIDs.Count == 0)
            {
                MessageBox.Show("No employees found. Click Filter first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentEmployeeIndex < employeeIDs.Count - 1)
            {
                currentEmployeeIndex++;

                if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                    !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
                {
                    MessageBox.Show("Invalid date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], startDate, endDate);
            }
            else
            {
                MessageBox.Show("This is the last employee.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (employeeIDs.Count == 0)
            {
                MessageBox.Show("No employees found. Click Filter first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentEmployeeIndex > 0)
            {
                currentEmployeeIndex--;

                if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                    !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
                {
                    MessageBox.Show("Invalid date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], startDate, endDate);
            }
            else
            {
                MessageBox.Show("This is the first employee.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AddRateDropdownToGrid()
        {
            if (dgvDTR.Columns["Rate"] == null) // Avoid duplicate columns
            {
                DataGridViewComboBoxColumn rateColumn = new DataGridViewComboBoxColumn
                {
                    Name = "Rate",
                    HeaderText = "Rate",
                    DropDownWidth = 100,
                    Width = 120,
                    FlatStyle = FlatStyle.Flat
                };

                // Add predefined rates
                rateColumn.Items.Add("780");
                rateColumn.Items.Add("817");
                rateColumn.Items.Add("520");

                // Set a default display text
                rateColumn.DefaultCellStyle.NullValue = "Select Rate";

                // Add column to DataGridView
                dgvDTR.Columns.Add(rateColumn);
            }
        }

        private void btnSaveProcessedDTR_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string employeeID = textID.Text; // Get current employee ID

                    // 🔹 Loop through only the current employee's rows in DataGridView
                    foreach (DataGridViewRow row in dgvDTR.Rows)
                    {
                        if (row.IsNewRow) continue; // Skip empty new row

                        string date = row.Cells["date"].Value?.ToString() ?? "";
                        string timeIn = row.Cells["TimeIn"].Value?.ToString() ?? "NULL";
                        string timeOut = row.Cells["TimeOut"].Value?.ToString() ?? "NULL";
                        string rate = row.Cells["Rate"].Value?.ToString() ?? "0"; // Default rate if empty

                        if (string.IsNullOrEmpty(date))
                        {
                            MessageBox.Show("Missing date. Skipping row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        // ✅ Check if the record already exists for this employee & date
                        string checkQuery = @"
                    SELECT COUNT(*) FROM processedDTR 
                    WHERE employee_id = @employeeID AND date = @date;";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            checkCmd.Parameters.AddWithValue("@date", DateTime.Parse(date).ToString("yyyy-MM-dd"));

                            int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (recordExists > 0)
                            {
                                // ✅ If record exists, only update the rate
                                string updateQuery = @"
                            UPDATE processedDTR 
                            SET rate = @rate
                            WHERE employee_id = @employeeID AND date = @date;";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    updateCmd.Parameters.AddWithValue("@date", DateTime.Parse(date).ToString("yyyy-MM-dd"));
                                    updateCmd.Parameters.AddWithValue("@rate", rate);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // 🔹 If record does not exist, insert new attendance
                                string insertQuery = @"
                            INSERT INTO processedDTR (employee_id, date, time_in, time_out, rate) 
                            VALUES (@employeeID, @date, @timeIn, @timeOut, @rate);";

                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    insertCmd.Parameters.AddWithValue("@date", DateTime.Parse(date).ToString("yyyy-MM-dd"));
                                    insertCmd.Parameters.AddWithValue("@timeIn", timeIn == "NULL" ? DBNull.Value : (object)timeIn);
                                    insertCmd.Parameters.AddWithValue("@timeOut", timeOut == "NULL" ? DBNull.Value : (object)timeOut);
                                    insertCmd.Parameters.AddWithValue("@rate", rate);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    MessageBox.Show("Data saved successfully for Employee ID: " + employeeID, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
