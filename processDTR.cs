using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
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
            dgvDTR.DataError += dgvDTR_DataError; // ✅ Attach DataError handler
        }

        private void SetupRateDropdown()
        {
            string rateColumnName = "Rate";

            // ✅ Check if Rate column already exists
            if (dgvDTR.Columns.Contains(rateColumnName))
            {
                dgvDTR.Columns.Remove(rateColumnName); // Remove existing column to re-add ComboBox
            }

            // ✅ Fetch rate values from database
            List<decimal> rateValues = GetRateValuesFromDatabase();

            // ✅ Create ComboBox Column with database rates
            DataGridViewComboBoxColumn rateColumn = new DataGridViewComboBoxColumn
            {
                Name = rateColumnName,
                HeaderText = "Rate",
                DataPropertyName = rateColumnName,  // ✅ Binds to Rate column
                DataSource = rateValues,  // ✅ Use dynamic database values
                AutoComplete = true
            };

            dgvDTR.Columns.Add(rateColumn);

            // ✅ Ensure existing database values match the DataSource
            foreach (DataGridViewRow row in dgvDTR.Rows)
            {
                if (row.IsNewRow) continue;

                object rateValue = row.Cells[rateColumnName].Value;

                // Convert to decimal to match DataSource
                if (rateValue == null || !rateValues.Contains(Convert.ToDecimal(rateValue)))
                {
                    row.Cells[rateColumnName].Value = rateValues.FirstOrDefault(); // ✅ Default valid value
                }
            }

            // ✅ Set Rate column's value type to decimal to avoid type mismatches
            dgvDTR.Columns[rateColumnName].ValueType = typeof(decimal);
        }

        // ✅ Method to fetch rates from the database
        private List<decimal> GetRateValuesFromDatabase()
        {
            List<decimal> rates = new List<decimal>();

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT rate_value FROM Rate ORDER BY rate_value ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rates.Add(reader.GetDecimal(0));
                        }
                    }
                }
            }

            return rates;
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
                SELECT e.id_no AS EmployeeID, 
                       CONCAT(e.fname, ' ', e.lname) AS EmployeeName, 
                       p.date, 
                       p.time_in AS TimeIn, 
                       p.time_out AS TimeOut, 
                       COALESCE(p.rate, 0.00) AS Rate  -- ✅ Ensures NULL values are replaced with 560.00
                FROM processedDTR p
                INNER JOIN employee e ON p.employee_id = e.id_no
                WHERE p.employee_id = @employeeID 
                      AND p.date BETWEEN @startDate AND @endDate
                ORDER BY p.date ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // ✅ Ensure "Rate" column exists and is correctly formatted as decimal
                            if (!dt.Columns.Contains("Rate"))
                            {
                                dt.Columns.Add("Rate", typeof(decimal)); // Ensure Rate is numeric
                            }

                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["Rate"] == DBNull.Value || string.IsNullOrEmpty(row["Rate"].ToString()))
                                {
                                    row["Rate"] = 0.00m; // ✅ Set default rate
                                }
                            }

                            if (dt.Rows.Count > 0)
                            {
                                textID.Text = dt.Rows[0]["EmployeeID"].ToString();
                                textName.Text = dt.Rows[0]["EmployeeName"].ToString();
                                dgvDTR.DataSource = dt;
                                SetupRateDropdown(); // ✅ Set up dropdown properly
                            }
                            else
                            {
                                LoadAttendanceData(employeeID, startDate, endDate); // ✅ Load attendance if no DTR found
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "DTR Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAttendanceData(string employeeID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT e.id_no AS EmployeeID, 
                       CONCAT(e.fname, ' ', e.lname) AS EmployeeName, 
                       a.date, 
                       MIN(a.time) AS TimeIn, 
                       MAX(a.time) AS TimeOut, 
                       0.00 AS Rate  -- ✅ Default rate when loading attendance
                FROM attendance a
                INNER JOIN employee e ON a.id = e.id_no
                WHERE a.id = @employeeID 
                      AND a.date BETWEEN @startDate AND @endDate
                GROUP BY a.date, e.id_no, e.fname, e.lname
                ORDER BY a.date ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // ✅ Ensure "Rate" column exists
                            if (!dt.Columns.Contains("Rate"))
                            {
                                dt.Columns.Add("Rate", typeof(decimal)); // Ensure Rate is numeric
                            }

                            if (dt.Rows.Count > 0)
                            {
                                textID.Text = dt.Rows[0]["EmployeeID"].ToString();
                                textName.Text = dt.Rows[0]["EmployeeName"].ToString();
                                dgvDTR.DataSource = dt;
                                SetupRateDropdown(); // ✅ Setup dropdown
                            }
                            else
                            {
                                MessageBox.Show("No attendance records found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDTR.DataSource = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Attendance Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnNext_Click(object sender, EventArgs e)
        {
            if (employeeIDs.Count == 0) return;

            if (currentEmployeeIndex < employeeIDs.Count - 1)
            {
                currentEmployeeIndex++;

                // ✅ Parse StartDate and EndDate before passing
                if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                    !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
                {
                    MessageBox.Show("Invalid date format. Please enter a valid date (YYYY-MM-DD).",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], startDate, endDate); // ✅ Refresh DataGridView
            }
            else
            {
                MessageBox.Show("No more employees.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (employeeIDs.Count == 0) return;

            if (currentEmployeeIndex > 0)
            {
                currentEmployeeIndex--;

                // ✅ Parse StartDate and EndDate before passing
                if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                    !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
                {
                    MessageBox.Show("Invalid date format. Please enter a valid date (YYYY-MM-DD).",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], startDate, endDate); // ✅ Refresh DataGridView
            }
            else
            {
                MessageBox.Show("This is the first employee.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSaveProcessedDTR_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    foreach (DataGridViewRow row in dgvDTR.Rows)
                    {
                        if (row.IsNewRow) continue; // Skip empty rows

                        string employeeID = textID.Text;
                        DateTime date = Convert.ToDateTime(row.Cells["date"].Value);
                        TimeSpan timeIn = TimeSpan.Parse(row.Cells["TimeIn"].Value.ToString());
                        TimeSpan timeOut = TimeSpan.Parse(row.Cells["TimeOut"].Value.ToString());
                        decimal rate = Convert.ToDecimal(row.Cells["Rate"].Value);

                        // ✅ First, check if an entry already exists for this employee on this date
                        string checkQuery = @"
                    SELECT COUNT(*) FROM processedDTR 
                    WHERE employee_id = @employeeID AND date = @date";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            checkCmd.Parameters.AddWithValue("@date", date);

                            int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (recordExists > 0)
                            {
                                // ✅ If a record exists, update only the Rate
                                string updateQuery = @"
                            UPDATE processedDTR 
                            SET rate = @rate 
                            WHERE employee_id = @employeeID AND date = @date";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@rate", rate);
                                    updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    updateCmd.Parameters.AddWithValue("@date", date);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // ✅ If no record exists, insert a new entry
                                string insertQuery = @"
                            INSERT INTO processedDTR (employee_id, date, time_in, time_out, rate) 
                            VALUES (@employeeID, @date, @timeIn, @timeOut, @rate)";

                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    insertCmd.Parameters.AddWithValue("@date", date);
                                    insertCmd.Parameters.AddWithValue("@timeIn", timeIn);
                                    insertCmd.Parameters.AddWithValue("@timeOut", timeOut);
                                    insertCmd.Parameters.AddWithValue("@rate", rate);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    MessageBox.Show("DTR saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving DTR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDTR_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == dgvDTR.Columns["Rate"].Index && e.Exception != null)
            {
                MessageBox.Show($"Invalid value in Rate column: {dgvDTR.Rows[e.RowIndex].Cells[e.ColumnIndex].Value}",
                                "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // ✅ Set a default value to prevent crashes
                dgvDTR.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0.00m;
                e.ThrowException = false;
            }
        }

        private void btnOpenDeleteDTR_Click(object sender, EventArgs e)
        {
            DeleteDTRForm deleteForm = new DeleteDTRForm();
            deleteForm.ShowDialog(); // Open as a modal dialog
        }
    }


}
