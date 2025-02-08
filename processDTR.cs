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

                    // 🔹 First, check for existing processedDTR data
                    string query = @"
                SELECT e.id_no AS EmployeeID, 
                       CONCAT(e.fname, ' ', e.lname) AS EmployeeName, 
                       p.date, p.time_in AS TimeIn, p.time_out AS TimeOut, p.rate
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

                            if (dt.Rows.Count > 0)
                            {
                                // ✅ Processed DTR exists, load it
                                textID.Text = dt.Rows[0]["EmployeeID"].ToString();
                                textName.Text = dt.Rows[0]["EmployeeName"].ToString();
                                dgvDTR.DataSource = dt; // ✅ Load into DataGridView
                            }
                            else
                            {
                                // 🚨 No processedDTR exists, load attendance data instead
                                LoadAttendanceData(employeeID, startDate, endDate);
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
                NULL AS Rate
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

                            if (dt.Rows.Count > 0)
                            {
                                textID.Text = dt.Rows[0]["EmployeeID"].ToString();
                                textName.Text = dt.Rows[0]["EmployeeName"].ToString();
                                dgvDTR.DataSource = dt; // ✅ Load attendance into DataGridView
                            }
                            else
                            {
                                MessageBox.Show("No attendance records found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDTR.DataSource = null; // Clear DataGridView if no records found
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
