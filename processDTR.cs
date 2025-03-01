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
            dgvDTR.DataError += dgvDTR_DataError;
            dgvDTR.CellEndEdit += dgvDTR_CellEndEdit;
            dgvDTR.EditingControlShowing += dgvDTR_EditingControlShowing;
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
        private void SetupShiftCodeDropdown()
        {
            string shiftCodeColumnName = "ShiftCode";

            // ✅ Check if ShiftCode column already exists
            if (dgvDTR.Columns.Contains(shiftCodeColumnName))
            {
                dgvDTR.Columns.Remove(shiftCodeColumnName); // Remove existing column to re-add ComboBox
            }

            // ✅ Fetch shift codes from database
            List<string> shiftCodes = GetShiftCodesFromDatabase();

            // ✅ Create ComboBox Column with database shift codes
            DataGridViewComboBoxColumn shiftCodeColumn = new DataGridViewComboBoxColumn
            {
                Name = shiftCodeColumnName,
                HeaderText = "Shift Code",
                DataPropertyName = shiftCodeColumnName,  // ✅ Binds to ShiftCode column
                DataSource = shiftCodes,  // ✅ Use dynamic database values
                AutoComplete = true,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Set to ComboBox to display dropdown immediately
            };

            dgvDTR.Columns.Add(shiftCodeColumn);

            // ✅ Ensure existing database values match the DataSource
            foreach (DataGridViewRow row in dgvDTR.Rows)
            {
                if (row.IsNewRow) continue;

                object shiftCodeValue = row.Cells[shiftCodeColumnName].Value;

                if (shiftCodeValue != null && !shiftCodes.Contains(shiftCodeValue.ToString()))
                {
                    row.Cells[shiftCodeColumnName].Value = shiftCodes.FirstOrDefault(); // ✅ Default valid value
                }
            }
        }
        private List<string> GetShiftCodesFromDatabase()
        {
            List<string> shiftCodes = new List<string>();

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT shift_code FROM ShiftCodes ORDER BY shift_code ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            shiftCodes.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return shiftCodes;
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
                LEFT JOIN processedDTR p ON e.id_no = p.employee_id AND p.date BETWEEN @startDate AND @endDate
                LEFT JOIN attendance a ON e.id_no = a.id AND a.date BETWEEN @startDate AND @endDate
                WHERE p.employee_id IS NOT NULL OR a.id IS NOT NULL
                ORDER BY e.id_no ASC;";  // ✅ Fetch from processedDTR first, fallback to attendance

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
                DataTable dt = LoadAttendanceData(employeeID, startDate, endDate);

                if (dt.Rows.Count > 0)
                {
                    textID.Text = dt.Rows[0]["EmployeeID"].ToString();
                    textName.Text = dt.Rows[0]["EmployeeName"].ToString();
                }
                else
                {
                    textID.Text = employeeID;
                    textName.Text = "Unknown Employee";
                }

                if (!dt.Columns.Contains("OTHours"))
                {
                    dt.Columns.Add("OTHours", typeof(decimal));
                }
                if (!dt.Columns.Contains("StartTime"))
                {
                    dt.Columns.Add("StartTime", typeof(TimeSpan));
                }
                if (!dt.Columns.Contains("EndTime"))
                {
                    dt.Columns.Add("EndTime", typeof(TimeSpan));
                }

                if (!dt.Columns.Contains("NightDifferentialHours"))
                {
                    dt.Columns.Add("NightDifferentialHours", typeof(decimal));
                }

                if (!dt.Columns.Contains("NightDifferentialOtHours"))
                {
                    dt.Columns.Add("NightDifferentialOtHours", typeof(decimal));
                }

                if (!dt.Columns.Contains("Remarks"))
                {
                    dt.Columns.Add("Remarks", typeof(string));
                }

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
                        newRow["WorkingHours"] = 0.00m;
                        newRow["OTHours"] = 0.00m;
                        newRow["ShiftCode"] = DBNull.Value;
                        newRow["StartTime"] = DBNull.Value;
                        newRow["EndTime"] = DBNull.Value;
                        newRow["NightDifferentialHours"] = 0.00m;
                        newRow["NightDifferentialOtHours"] = 0.00m;
                        newRow["Remarks"] = DBNull.Value;
                        dt.Rows.Add(newRow);
                    }
                }

                dt.DefaultView.Sort = "Date ASC";
                dgvDTR.DataSource = dt;
                SetupRateDropdown();
                SetupShiftCodeDropdown();

                if (!dgvDTR.Columns.Contains("WorkingHours"))
                {
                    dgvDTR.Columns.Add(new DataGridViewTextBoxColumn { Name = "WorkingHours", HeaderText = "Working Hours", ReadOnly = true });
                }
                if (!dgvDTR.Columns.Contains("OTHours"))
                {
                    dgvDTR.Columns.Add(new DataGridViewTextBoxColumn { Name = "OTHours", HeaderText = "OT Hours", ReadOnly = true });
                }
                if (!dgvDTR.Columns.Contains("StartTime"))
                {
                    dgvDTR.Columns.Add(new DataGridViewTextBoxColumn { Name = "StartTime", HeaderText = "Start Time", ReadOnly = true });
                }
                if (!dgvDTR.Columns.Contains("EndTime"))
                {
                    dgvDTR.Columns.Add(new DataGridViewTextBoxColumn { Name = "EndTime", HeaderText = "End Time", ReadOnly = true });
                }

                // Add New Column - NightDifferential
                if (!dgvDTR.Columns.Contains("NightDifferentialHours"))
                {
                    DataGridViewTextBoxColumn nightDifferentialColumn = new DataGridViewTextBoxColumn
                    {
                        Name = "NightDifferentialHours",
                        HeaderText = "Night Differential Hours",
                        ReadOnly = true
                    };
                    dgvDTR.Columns.Add(nightDifferentialColumn);
                }

                // Add New Column - NightDifferentialOT
                if (!dgvDTR.Columns.Contains("NightDifferentialOtHours"))
                {
                    DataGridViewTextBoxColumn nightDifferentialOTColumn = new DataGridViewTextBoxColumn
                    {
                        Name = "NightDifferentialOtHours",
                        HeaderText = "Night Differential OT Hours",
                        ReadOnly = true
                    };
                    dgvDTR.Columns.Add(nightDifferentialOTColumn);
                }

                // Add New Column - Remarks
                if (!dgvDTR.Columns.Contains("Remarks"))
                {
                    DataGridViewTextBoxColumn remarksColumn = new DataGridViewTextBoxColumn
                    {
                        Name = "Remarks",
                        HeaderText = "Remarks",
                        ReadOnly = true
                    };
                    dgvDTR.Columns.Add(remarksColumn);
                }

                dgvDTR.Columns["WorkingHours"].DefaultCellStyle.Format = "N2";
                dgvDTR.Columns["OTHours"].DefaultCellStyle.Format = "N2";
                dgvDTR.Columns["StartTime"].DefaultCellStyle.Format = "h\\:mm";
                dgvDTR.Columns["EndTime"].DefaultCellStyle.Format = "h\\:mm";
                dgvDTR.Columns["NightDifferentialHours"].DefaultCellStyle.Format = "N2";
                dgvDTR.Columns["NightDifferentialOtHours"].DefaultCellStyle.Format = "N2";

                // Hide EmployeeID and EmployeeName columns
                dgvDTR.Columns["EmployeeID"].Visible = false;
                dgvDTR.Columns["EmployeeName"].Visible = false;

                dgvDTR.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "DTR Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private class ShiftCodeData
        {
            public string ShiftCode { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public decimal RegularHours { get; set; }
            public decimal OtHours { get; set; }
            public decimal NightDifferentialHours { get; set; } //NEW
            public decimal NightDifferentialOtHours { get; set; }
        }
        private ShiftCodeData GetShiftCodeData(string shiftCode)
        {
            ShiftCodeData shiftData = null;

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT shift_code, start_time, end_time, regular_hours, ot_hours, 
                    night_differential_hours, night_differential_ot_hours
                FROM ShiftCodes WHERE shift_code = @shiftCode";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                shiftData = new ShiftCodeData
                                {
                                    ShiftCode = reader["shift_code"].ToString(),
                                    StartTime = reader.GetTimeSpan("start_time"),
                                    EndTime = reader.GetTimeSpan("end_time"),
                                    RegularHours = reader.GetDecimal("regular_hours"),
                                    OtHours = reader.GetDecimal("ot_hours"),
                                    NightDifferentialHours = reader.GetDecimal("night_differential_hours"),
                                    NightDifferentialOtHours = reader.GetDecimal("night_differential_ot_hours")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching Shift Code data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return shiftData;
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
            dt.Columns.Add("WorkingHours", typeof(decimal));
            dt.Columns.Add("OTHours", typeof(decimal));
            dt.Columns.Add("ShiftCode", typeof(string));
            dt.Columns.Add("StartTime", typeof(TimeSpan));
            dt.Columns.Add("EndTime", typeof(TimeSpan));
            dt.Columns.Add("NightDifferentialHours", typeof(decimal));
            dt.Columns.Add("NightDifferentialOtHours", typeof(decimal));
            dt.Columns.Add("Remarks", typeof(string)); // Add Remarks column

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
            WITH RECURSIVE DateRange AS (
                SELECT @startDate AS Date
                UNION ALL
                SELECT DATE_ADD(Date, INTERVAL 1 DAY)
                FROM DateRange
                WHERE Date < @endDate
            )
            SELECT
                e.id_no AS EmployeeID,
                CONCAT(e.fname, ' ', e.lname) AS EmployeeName,
                d.Date,
                COALESCE(p.time_in, MIN(a.time)) AS TimeIn,
                COALESCE(p.time_out, MAX(a.time)) AS TimeOut,
                COALESCE(p.rate, 0.00) AS Rate,
                COALESCE(sc.regular_hours, 0.00) AS WorkingHours,
                COALESCE(p.ot_hrs, sc.ot_hours, 0.00) AS OTHours,
                p.shift_code AS ShiftCode,
                sc.start_time AS StartTime,
                sc.end_time AS EndTime,
                sc.night_differential_hours AS NightDifferentialHours,
                sc.night_differential_ot_hours AS NightDifferentialOtHours,
                p.remarks AS Remarks
            FROM employee e
            JOIN DateRange d ON 1=1
            LEFT JOIN attendance a ON e.id_no = a.id AND a.date = d.Date
            LEFT JOIN processedDTR p ON e.id_no = p.employee_id AND p.date = d.Date
            LEFT JOIN ShiftCodes sc ON p.shift_code = sc.shift_code
            WHERE e.id_no = @employeeID
            GROUP BY e.id_no, e.fname, e.lname, d.Date, p.time_in, p.time_out, p.rate, p.shift_code, sc.start_time, sc.end_time, sc.regular_hours, p.ot_hrs, sc.ot_hours, sc.night_differential_hours, sc.night_differential_ot_hours, p.remarks
            ORDER BY d.Date ASC;";

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

                        string employeeID = row.Cells["EmployeeID"].Value.ToString();
                        DateTime date = Convert.ToDateTime(row.Cells["Date"].Value);
                        TimeSpan? timeIn = row.Cells["TimeIn"].Value != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row.Cells["TimeIn"].Value.ToString()) : null;
                        TimeSpan? timeOut = row.Cells["TimeOut"].Value != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row.Cells["TimeOut"].Value.ToString()) : null;
                        decimal rate = row.Cells["Rate"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Rate"].Value) : 0.00m;
                        decimal workingHours = row.Cells["WorkingHours"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["WorkingHours"].Value) : 0.00m;
                        decimal otHours = row.Cells["OTHours"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["OTHours"].Value) : 0.00m;
                        string shiftCode = row.Cells["ShiftCode"].Value?.ToString();
                        decimal ndHours = row.Cells["NightDifferentialHours"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["NightDifferentialHours"].Value) : 0.00m;
                        decimal ndOtHours = row.Cells["NightDifferentialOtHours"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["NightDifferentialOtHours"].Value) : 0.00m;
                        string remarks = row.Cells["Remarks"].Value?.ToString();

                        // Check if the record exists
                        string checkQuery = "SELECT COUNT(*) FROM processedDTR WHERE employee_id = @employeeID AND date = @date";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            checkCmd.Parameters.AddWithValue("@date", date);
                            int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (recordExists > 0)
                            {
                                // Update the existing record
                                string updateQuery = @"
                        UPDATE processedDTR 
                        SET rate = @rate, 
                            time_in = IFNULL(@timeIn, time_in), 
                            time_out = IFNULL(@timeOut, time_out), 
                            working_hours = @workingHours, 
                            ot_hrs = @otHours,
                            shift_code = @shiftCode,
                            nd_hrs = @ndHours,
                            ndot_hrs = @ndOtHours,
                            remarks = @remarks
                        WHERE employee_id = @employeeID AND date = @date";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@rate", rate);
                                    updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    updateCmd.Parameters.AddWithValue("@date", date);
                                    updateCmd.Parameters.AddWithValue("@timeIn", (object)timeIn ?? DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@timeOut", (object)timeOut ?? DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@workingHours", workingHours);
                                    updateCmd.Parameters.AddWithValue("@otHours", otHours);
                                    updateCmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                                    updateCmd.Parameters.AddWithValue("@ndHours", ndHours);
                                    updateCmd.Parameters.AddWithValue("@ndOtHours", ndOtHours);
                                    updateCmd.Parameters.AddWithValue("@remarks", remarks);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert a new record if it doesn't exist
                                string insertQuery = @"
                        INSERT INTO processedDTR (employee_id, date, time_in, time_out, rate, working_hours, ot_hrs, shift_code, nd_hrs, ndot_hrs, remarks)
                        VALUES (@employeeID, @date, @timeIn, @timeOut, @rate, @workingHours, @otHours, @shiftCode, @ndHours, @ndOtHours, @remarks)";

                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    insertCmd.Parameters.AddWithValue("@date", date);
                                    insertCmd.Parameters.AddWithValue("@timeIn", (object)timeIn ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@timeOut", (object)timeOut ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@rate", rate);
                                    insertCmd.Parameters.AddWithValue("@workingHours", workingHours);
                                    insertCmd.Parameters.AddWithValue("@otHours", otHours);
                                    insertCmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                                    insertCmd.Parameters.AddWithValue("@ndHours", ndHours);
                                    insertCmd.Parameters.AddWithValue("@ndOtHours", ndOtHours);
                                    insertCmd.Parameters.AddWithValue("@remarks", remarks);
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
            else if (e.ColumnIndex == dgvDTR.Columns["ShiftCode"].Index && e.Exception != null)
            {
                // Suppress the error for the ShiftCode column
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
            DataGridViewRow row = dgvDTR.Rows[e.RowIndex];

            if (e.ColumnIndex == dgvDTR.Columns["ShiftCode"].Index)
            {
                if (row.Cells["ShiftCode"].Value != null)
                {
                    string shiftCode = row.Cells["ShiftCode"].Value.ToString();
                    ShiftCodeData shiftData = GetShiftCodeData(shiftCode);

                    if (shiftData != null)
                    {
                        row.Cells["StartTime"].Value = shiftData.StartTime;
                        row.Cells["EndTime"].Value = shiftData.EndTime;
                        row.Cells["WorkingHours"].Value = shiftData.RegularHours;

                        // Use fixed values from ShiftCodeData instead of calculations
                        row.Cells["OTHours"].Value = shiftData.OtHours;
                        row.Cells["NightDifferentialHours"].Value = shiftData.NightDifferentialHours;
                        row.Cells["NightDifferentialOtHours"].Value = shiftData.NightDifferentialOtHours;

                        // Set default values for TimeIn and TimeOut to 0:00 if they are blank
                        if (row.Cells["TimeIn"].Value == DBNull.Value)
                        {
                            row.Cells["TimeIn"].Value = TimeSpan.Zero;
                        }
                        if (row.Cells["TimeOut"].Value == DBNull.Value)
                        {
                            row.Cells["TimeOut"].Value = TimeSpan.Zero;
                        }

                        // Update Remarks based on TimeIn and TimeOut values
                        row.Cells["Remarks"].Value = CalculateRemarks(row);
                    }
                    else
                    {
                        MessageBox.Show("Invalid Shift Code", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        row.Cells["ShiftCode"].Value = DBNull.Value;
                        row.Cells["StartTime"].Value = DBNull.Value;
                        row.Cells["EndTime"].Value = DBNull.Value;
                        row.Cells["WorkingHours"].Value = DBNull.Value;
                        row.Cells["OTHours"].Value = DBNull.Value;
                        row.Cells["NightDifferentialHours"].Value = DBNull.Value;
                        row.Cells["NightDifferentialOtHours"].Value = DBNull.Value;
                        row.Cells["Remarks"].Value = DBNull.Value;
                    }
                }
                else
                {
                    row.Cells["StartTime"].Value = DBNull.Value;
                    row.Cells["EndTime"].Value = DBNull.Value;
                    row.Cells["WorkingHours"].Value = DBNull.Value;
                    row.Cells["OTHours"].Value = DBNull.Value;
                    row.Cells["NightDifferentialHours"].Value = DBNull.Value;
                    row.Cells["NightDifferentialOtHours"].Value = DBNull.Value;
                    row.Cells["Remarks"].Value = DBNull.Value;
                }
            }
            else if (e.ColumnIndex == dgvDTR.Columns["Rate"].Index)
            {
                // Handle changes to the Rate column
                if (row.Cells["Rate"].Value != null)
                {
                    decimal rate = Convert.ToDecimal(row.Cells["Rate"].Value);
                    UpdateProcessedDTR(row.Cells["EmployeeID"].Value.ToString(),
                                       Convert.ToDateTime(row.Cells["Date"].Value),
                                       row.Cells["ShiftCode"].Value.ToString(),
                                       Convert.ToDecimal(row.Cells["WorkingHours"].Value),
                                       Convert.ToDecimal(row.Cells["OTHours"].Value),
                                       rate,
                                       row.Cells["TimeIn"].Value == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row.Cells["TimeIn"].Value.ToString()),
                                       row.Cells["TimeOut"].Value == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row.Cells["TimeOut"].Value.ToString()),
                                       Convert.ToDecimal(row.Cells["NightDifferentialHours"].Value),
                                       Convert.ToDecimal(row.Cells["NightDifferentialOtHours"].Value));
                }
            }
            else if (e.ColumnIndex == dgvDTR.Columns["TimeIn"].Index || e.ColumnIndex == dgvDTR.Columns["TimeOut"].Index)
            {
                if (row.Cells["ShiftCode"].Value != null)
                {
                    ShiftCodeData shiftData = GetShiftCodeData(row.Cells["ShiftCode"].Value.ToString());

                    if (shiftData != null)
                    {
                        // Use fixed values from ShiftCodeData for all hours
                        row.Cells["OTHours"].Value = shiftData.OtHours;
                        row.Cells["NightDifferentialHours"].Value = shiftData.NightDifferentialHours;
                        row.Cells["NightDifferentialOtHours"].Value = shiftData.NightDifferentialOtHours;
                    }
                }

                row.Cells["Remarks"].Value = CalculateRemarks(row);

                UpdateProcessedDTR(row.Cells["EmployeeID"].Value.ToString(),
                                   Convert.ToDateTime(row.Cells["Date"].Value),
                                   row.Cells["ShiftCode"].Value.ToString(),
                                   Convert.ToDecimal(row.Cells["WorkingHours"].Value),
                                   Convert.ToDecimal(row.Cells["OTHours"].Value),
                                   Convert.ToDecimal(row.Cells["Rate"].Value),
                                   row.Cells["TimeIn"].Value == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row.Cells["TimeIn"].Value.ToString()),
                                   row.Cells["TimeOut"].Value == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row.Cells["TimeOut"].Value.ToString()),
                                   Convert.ToDecimal(row.Cells["NightDifferentialHours"].Value),
                                   Convert.ToDecimal(row.Cells["NightDifferentialOtHours"].Value));
            }

            // Refresh the DataGridView to reflect changes
            dgvDTR.Refresh();
        }
        private void UpdateProcessedDTR(string employeeID, DateTime date, string shiftCode, decimal workingHours, decimal otHours, decimal rate, TimeSpan? timeIn = null, TimeSpan? timeOut = null, decimal ndHours = 0, decimal ndOtHours = 0, string remarks = null)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
            UPDATE processedDTR 
            SET time_in = @timeIn, 
                time_out = @timeOut, 
                working_hours = @workingHours, 
                ot_hrs = @otHours,
                rate = @rate,
                shift_code = @shiftCode,
                nd_hrs = @ndHours,
                ndot_hrs = @ndOtHours,
                remarks = @remarks
            WHERE employee_id = @employeeID AND date = @date";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@timeIn", (object)timeIn ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@timeOut", (object)timeOut ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@workingHours", workingHours);
                        cmd.Parameters.AddWithValue("@otHours", otHours);
                        cmd.Parameters.AddWithValue("@rate", rate);
                        cmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                        cmd.Parameters.AddWithValue("@ndHours", ndHours);
                        cmd.Parameters.AddWithValue("@ndOtHours", ndOtHours);
                        cmd.Parameters.AddWithValue("@remarks", remarks);
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating DTR: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvDTR_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvDTR.CurrentCell.ColumnIndex == dgvDTR.Columns["ShiftCode"].Index)
            {
                if (e.Control is ComboBox comboBox)
                {
                    comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged; // Remove any existing handler
                    comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged; // Add the new handler
                }
            }
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvDTR.CurrentCell != null && dgvDTR.CurrentCell.ColumnIndex == dgvDTR.Columns["ShiftCode"].Index)
            {
                DataGridViewRow row = dgvDTR.Rows[dgvDTR.CurrentCell.RowIndex];
                ComboBox comboBox = sender as ComboBox;

                if (comboBox != null && comboBox.SelectedItem != null)
                {
                    string shiftCode = comboBox.SelectedItem.ToString();
                    ShiftCodeData shiftData = GetShiftCodeData(shiftCode);

                    if (shiftData != null)
                    {
                        row.Cells["StartTime"].Value = shiftData.StartTime;
                        row.Cells["EndTime"].Value = shiftData.EndTime;
                        row.Cells["WorkingHours"].Value = shiftData.RegularHours;

                        // Use fixed values from ShiftCodeData instead of calculations
                        row.Cells["OTHours"].Value = shiftData.OtHours;
                        row.Cells["NightDifferentialHours"].Value = shiftData.NightDifferentialHours;
                        row.Cells["NightDifferentialOtHours"].Value = shiftData.NightDifferentialOtHours;

                        // Set default values for TimeIn and TimeOut to 0:00 if they are blank
                        if (row.Cells["TimeIn"].Value == DBNull.Value)
                        {
                            row.Cells["TimeIn"].Value = TimeSpan.Zero;
                        }
                        if (row.Cells["TimeOut"].Value == DBNull.Value)
                        {
                            row.Cells["TimeOut"].Value = TimeSpan.Zero;
                        }

                        // Update Remarks based on TimeIn and TimeOut values
                        row.Cells["Remarks"].Value = CalculateRemarks(row);

                        // Update the DataGridView to reflect changes
                        dgvDTR.Refresh();
                    }
                    else
                    {
                        // Handle invalid shift code gracefully
                        row.Cells["StartTime"].Value = DBNull.Value;
                        row.Cells["EndTime"].Value = DBNull.Value;
                        row.Cells["WorkingHours"].Value = DBNull.Value;
                        row.Cells["OTHours"].Value = DBNull.Value;
                        row.Cells["NightDifferentialHours"].Value = DBNull.Value;
                        row.Cells["NightDifferentialOtHours"].Value = DBNull.Value;
                        row.Cells["Remarks"].Value = "Invalid Shift Code";
                    }
                }
            }
        }
        private string CalculateRemarks(DataGridViewRow row)
        {
            if (row.Cells["TimeIn"].Value == DBNull.Value || row.Cells["TimeOut"].Value == DBNull.Value)
            {
                return "Absent";
            }

            if (row.Cells["StartTime"].Value == DBNull.Value || row.Cells["EndTime"].Value == DBNull.Value)
            {
                return "No Shift Data";
            }

            TimeSpan timeIn = (TimeSpan)row.Cells["TimeIn"].Value;
            TimeSpan timeOut = (TimeSpan)row.Cells["TimeOut"].Value;
            TimeSpan startTime = (TimeSpan)row.Cells["StartTime"].Value;
            TimeSpan endTime = (TimeSpan)row.Cells["EndTime"].Value;

            if (timeIn > startTime)
            {
                return "Late";
            }
            else if (timeOut < endTime)
            {
                return "Undertime";
            }
            else
            {
                return "Present";
            }
        }
    }


}
