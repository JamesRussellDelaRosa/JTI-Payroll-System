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
            dgvDTR.CellEndEdit += dgvDTR_CellEndEdit;
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
                DataTable dt = LoadAttendanceData(employeeID, startDate, endDate); // ✅ Load Attendance Data

                if (dt.Rows.Count > 0)
                {
                    // ✅ Set Employee ID and Name
                    textID.Text = dt.Rows[0]["EmployeeID"].ToString();
                    textName.Text = dt.Rows[0]["EmployeeName"].ToString();
                }
                else
                {
                    // Handle cases where no attendance data is found
                    textID.Text = employeeID;
                    textName.Text = "Unknown Employee";
                }

                // ✅ Ensure all dates between startDate and endDate are present
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (!dt.AsEnumerable().Any(row => Convert.ToDateTime(row["Date"]) == date))
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["EmployeeID"] = employeeID;
                        newRow["EmployeeName"] = textName.Text;
                        newRow["Date"] = date;
                        newRow["TimeIn"] = DBNull.Value;
                        newRow["TimeOut"] = DBNull.Value;
                        newRow["Rate"] = 0.00m;
                        newRow["WorkingHours"] = 0.00m; // ✅ Default to 0
                        dt.Rows.Add(newRow);
                    }
                }

                // ✅ Compute Working Hours for each row
                foreach (DataRow row in dt.Rows)
                {
                    if (row["TimeIn"] != DBNull.Value && row["TimeOut"] != DBNull.Value)
                    {
                        TimeSpan timeIn = (TimeSpan)row["TimeIn"];
                        TimeSpan timeOut = (TimeSpan)row["TimeOut"];
                        row["WorkingHours"] = (decimal)(timeOut - timeIn).TotalHours;
                    }
                    else
                    {
                        row["WorkingHours"] = 0.00m; // ✅ Default to 0 if missing data
                    }
                }

                dt.DefaultView.Sort = "Date ASC"; // ✅ Ensure sorting by date

                dgvDTR.DataSource = dt;
                SetupRateDropdown();

                // ✅ Ensure Working Hours column exists in dgvDTR
                if (!dgvDTR.Columns.Contains("WorkingHours"))
                {
                    DataGridViewTextBoxColumn workingHoursColumn = new DataGridViewTextBoxColumn
                    {
                        Name = "WorkingHours",
                        HeaderText = "Working Hours",
                        ReadOnly = true
                    };
                    dgvDTR.Columns.Add(workingHoursColumn);
                }

                // ✅ Format Working Hours column
                dgvDTR.Columns["WorkingHours"].DefaultCellStyle.Format = "N2"; // Two decimal places
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "DTR Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable LoadAttendanceData(string employeeID, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeID", typeof(string));
            dt.Columns.Add("EmployeeName", typeof(string));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("TimeIn", typeof(TimeSpan));
            dt.Columns.Add("TimeOut", typeof(TimeSpan));
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("WorkingHours", typeof(decimal)); // ✅ Add Working Hours column

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
                   COALESCE(p.rate, 0.00) AS Rate 
            FROM employee e
            LEFT JOIN attendance a ON e.id_no = a.id
            LEFT JOIN processedDTR p ON a.id = p.employee_id AND a.date = p.date
            WHERE e.id_no = @employeeID 
                  AND a.date BETWEEN @startDate AND @endDate
            GROUP BY e.id_no, e.fname, e.lname, a.date, p.rate
            ORDER BY a.date ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                // ✅ Compute Working Hours
                foreach (DataRow row in dt.Rows)
                {
                    if (row["TimeIn"] != DBNull.Value && row["TimeOut"] != DBNull.Value)
                    {
                        TimeSpan timeIn = (TimeSpan)row["TimeIn"];
                        TimeSpan timeOut = (TimeSpan)row["TimeOut"];
                        row["WorkingHours"] = (decimal)(timeOut - timeIn).TotalHours; // ✅ Compute hours
                    }
                    else
                    {
                        row["WorkingHours"] = 0.00m; // ✅ Default to 0 if missing data
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
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
                        if (row.IsNewRow) continue;

                        string employeeID = textID.Text;
                        DateTime date = Convert.ToDateTime(row.Cells["Date"].Value);
                        TimeSpan? timeIn = row.Cells["TimeIn"].Value != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row.Cells["TimeIn"].Value.ToString()) : null;
                        TimeSpan? timeOut = row.Cells["TimeOut"].Value != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row.Cells["TimeOut"].Value.ToString()) : null;
                        decimal rate = row.Cells["Rate"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Rate"].Value) : 0.00m;
                        decimal workingHours = row.Cells["WorkingHours"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["WorkingHours"].Value) : 0.00m;

                        // Check if the record exists
                        string checkQuery = "SELECT COUNT(*) FROM processedDTR WHERE employee_id = @employeeID AND date = @date";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            checkCmd.Parameters.AddWithValue("@date", date);
                            int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (recordExists > 0)
                            {
                                // **Update the existing record**
                                string updateQuery = @"
                        UPDATE processedDTR 
                        SET rate = @rate, 
                            time_in = IFNULL(@timeIn, time_in), 
                            time_out = IFNULL(@timeOut, time_out), 
                            working_hours = @workingHours 
                        WHERE employee_id = @employeeID AND date = @date";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@rate", rate);
                                    updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    updateCmd.Parameters.AddWithValue("@date", date);
                                    updateCmd.Parameters.AddWithValue("@timeIn", (object)timeIn ?? DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@timeOut", (object)timeOut ?? DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@workingHours", workingHours);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // **Insert a new record if it doesn't exist**
                                string insertQuery = @"
                        INSERT INTO processedDTR (employee_id, date, time_in, time_out, rate, working_hours)
                        VALUES (@employeeID, @date, @timeIn, @timeOut, @rate, @workingHours)";

                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    insertCmd.Parameters.AddWithValue("@date", date);
                                    insertCmd.Parameters.AddWithValue("@timeIn", (object)timeIn ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@timeOut", (object)timeOut ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@rate", rate);
                                    insertCmd.Parameters.AddWithValue("@workingHours", workingHours);
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

        private void dgvDTR_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvDTR.Columns["TimeIn"].Index || e.ColumnIndex == dgvDTR.Columns["TimeOut"].Index)
            {
                DataGridViewRow row = dgvDTR.Rows[e.RowIndex];

                if (row.Cells["TimeIn"].Value != DBNull.Value && row.Cells["TimeOut"].Value != DBNull.Value)
                {
                    if (TimeSpan.TryParse(row.Cells["TimeIn"].Value.ToString(), out TimeSpan timeIn) &&
                        TimeSpan.TryParse(row.Cells["TimeOut"].Value.ToString(), out TimeSpan timeOut))
                    {
                        row.Cells["WorkingHours"].Value = (decimal)(timeOut - timeIn).TotalHours;
                    }
                    else
                    {
                        row.Cells["WorkingHours"].Value = 0.00m;
                    }
                }
                else
                {
                    row.Cells["WorkingHours"].Value = 0.00m;
                }
            }
        }
    }


}
