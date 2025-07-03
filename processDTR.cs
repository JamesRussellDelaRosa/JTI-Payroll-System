using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; // Add this line to resolve ambiguity
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class processDTR : Form
    {
        private List<string> employeeIDs = new List<string>(); // Store employee IDs with attendance
        private int currentEmployeeIndex = 0; // Track current employee position
        private bool rateDropdownHandlerAttached = false;
        private bool shiftCodeDropdownHandlerAttached = false;

        public processDTR()
        {
            InitializeComponent();
            dgvDTR.DataError += dgvDTR_DataError;
            dgvDTR.CellEndEdit += dgvDTR_CellEndEdit;
            dgvDTR.EditingControlShowing += dgvDTR_EditingControlShowing;
            dgvDTR.CellValueChanged += dgvDTR_CellValueChanged; // Add this line
            dgvDTR.CurrentCellDirtyStateChanged += dgvDTR_CurrentCellDirtyStateChanged; // Add this line
            dgvDTR.CellEnter += dgvDTR_CellEnter;

            // Add row highlighting for current row
            dgvDTR.RowPrePaint += dgvDTR_RowPrePaint;

            // --- Optimization: Set CheckBox column style only once ---
            SetCheckBoxColumnStyles();
            // --------------------------------------------------------

            // Add context menu for Fill Down
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Fill Down", null, (s, e) => FillDownCurrentCell());
            dgvDTR.ContextMenuStrip = menu;

            // Add event handlers for placeholder text
            textStartDate.Enter += TextBox_Enter;
            textStartDate.Leave += TextBox_Leave;
            textStartDate.TextChanged += TextBox_TextChanged; // Add this line
            textStartDate.KeyPress += AutoFormatDate; // Attach AutoFormatDate

            textEndDate.Enter += TextBox_Enter;
            textEndDate.Leave += TextBox_Leave;
            textEndDate.KeyPress += AutoFormatDate; // Attach AutoFormatDate

            // Add Paint event handlers for custom drawing
            textStartDate.Paint += TextBox_Paint;
            textEndDate.Paint += TextBox_Paint;

            // Set initial placeholder text
            SetPlaceholderText(textStartDate, "MM/DD/YYYY");
            SetPlaceholderText(textEndDate, "MM/DD/YYYY");
        }

        // Optimization: Set CheckBox column style only once
        private void SetCheckBoxColumnStyles()
        {
            string[] checkBoxColumns = { "RestDay", "LegalHoliday", "SpecialHoliday", "NonWorkingDay", "Reliever" };
            foreach (var colName in checkBoxColumns)
            {
                if (dgvDTR.Columns.Contains(colName) && dgvDTR.Columns[colName] is DataGridViewCheckBoxColumn col)
                {
                    col.FlatStyle = FlatStyle.Standard;
                    col.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 255, 224); // Light yellow
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        private void SetupRateDropdown()
        {
            string rateColumnName = "Rate";

            // Check if Rate column already exists
            if (dgvDTR.Columns.Contains(rateColumnName))
            {
                dgvDTR.Columns.Remove(rateColumnName); // Remove existing column to re-add ComboBox
            }

            // Fetch rate values from the database
            List<decimal> rateValues = GetRateValuesFromDatabase();

            // Add default value 0.00 at the beginning
            rateValues.Insert(0, 0.00m);

            // Create ComboBox Column with database rates
            DataGridViewComboBoxColumn rateColumn = new DataGridViewComboBoxColumn
            {
                Name = rateColumnName,
                HeaderText = "Rate",
                DataPropertyName = rateColumnName, // Binds to Rate column
                DataSource = rateValues, // Use dynamic database values
                AutoComplete = true,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Set to ComboBox to display dropdown immediately
            };

            dgvDTR.Columns.Add(rateColumn);
        }

        private void ComboBox_Validating(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBoxrate)
            {
                string input = comboBoxrate.Text;

                // Validate if the input is a valid decimal
                if (decimal.TryParse(input, out decimal newRate))
                {
                    // Check if the value already exists in the dropdown
                    var rateColumn = (DataGridViewComboBoxColumn)dgvDTR.Columns["Rate"];
                    var currentDataSource = (List<decimal>)rateColumn.DataSource;

                    if (!currentDataSource.Contains(newRate))
                    {
                        // Silently reset to default if invalid
                        comboBoxrate.Text = "0.00";
                    }
                }
                else
                {
                    // Silently reset to default if invalid
                        comboBoxrate.Text = "0.00";
                }
            }
        }

        private List<decimal> GetRateValuesFromDatabase()
        {
            List<decimal> rates = new List<decimal>();

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT defaultrate FROM Rate ORDER BY defaultrate ASC";

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

            // Check if ShiftCode column already exists
            if (dgvDTR.Columns.Contains(shiftCodeColumnName))
            {
                dgvDTR.Columns.Remove(shiftCodeColumnName); // Remove existing column to re-add ComboBox
            }

            // Fetch shift codes from the database
            List<string> shiftCodes = GetShiftCodesFromDatabase();

            // Create ComboBox Column with database shift codes
            DataGridViewComboBoxColumn shiftCodeColumn = new DataGridViewComboBoxColumn
            {
                Name = shiftCodeColumnName,
                HeaderText = "Shift Code",
                DataPropertyName = shiftCodeColumnName, // Binds to ShiftCode column
                DataSource = shiftCodes, // Use dynamic database values
                AutoComplete = true,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Set to ComboBox to display dropdown immediately
            };

            dgvDTR.Columns.Add(shiftCodeColumn);
        }

        private void ShiftCodeComboBox_Validating(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBoxshiftcode)
            {
                string input = comboBoxshiftcode.Text;

                // Validate if the input is a valid shift code
                if (!string.IsNullOrWhiteSpace(input))
                {
                    // Check if the value already exists in the dropdown
                    var shiftCodeColumn = (DataGridViewComboBoxColumn)dgvDTR.Columns["ShiftCode"];
                    var currentDataSource = (List<string>)shiftCodeColumn.DataSource;

                    if (!currentDataSource.Contains(input))
                    {
                        // Silently reset to default if invalid
                        comboBoxshiftcode.Text = "";
                    }
                }
                else
                {
                    // Silently reset to default if invalid
                    comboBoxshiftcode.Text = "";
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
            // ✅ Convert TextBox values to DateTime in MM/DD/YYYY format
            if (!DateTime.TryParseExact(textStartDate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(textEndDate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                MessageBox.Show("Invalid date format. Please enter a valid date (MM/DD/YYYY).",
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
                // Setup columns once
                SetupRateDropdown();
                SetupShiftCodeDropdown();

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
                        newRow["TardinessUndertime"] = 0.00m;
                        newRow["RestDay"] = false;
                        newRow["LegalHoliday"] = false;
                        newRow["SpecialHoliday"] = false;
                        newRow["NonWorkingDay"] = false;
                        newRow["Reliever"] = false;
                        dt.Rows.Add(newRow);
                    }
                }

                dt.DefaultView.Sort = "Date ASC";
                dgvDTR.DataSource = dt;
                dgvDTR.AllowUserToAddRows = false;

                // Update column headers
                dgvDTR.Columns["WorkingHours"].HeaderText = "WorkHrs";
                dgvDTR.Columns["OTHours"].HeaderText = "OTHrs";
                dgvDTR.Columns["StartTime"].HeaderText = "Start";
                dgvDTR.Columns["EndTime"].HeaderText = "End";
                dgvDTR.Columns["NightDifferentialHours"].HeaderText = "NDHrs";
                dgvDTR.Columns["NightDifferentialOtHours"].HeaderText = "NDOTHrs";
                dgvDTR.Columns["Remarks"].HeaderText = "Rmrks";
                dgvDTR.Columns["TardinessUndertime"].HeaderText = "Tard/UT";
                dgvDTR.Columns["RestDay"].HeaderText = "RD";
                dgvDTR.Columns["LegalHoliday"].HeaderText = "LH";
                dgvDTR.Columns["SpecialHoliday"].HeaderText = "SH";
                dgvDTR.Columns["NonWorkingDay"].HeaderText = "NWS";
                dgvDTR.Columns["Reliever"].HeaderText = "RLVR";

                dgvDTR.Columns["WorkingHours"].DefaultCellStyle.Format = "N2";
                dgvDTR.Columns["OTHours"].DefaultCellStyle.Format = "N2";
                dgvDTR.Columns["NightDifferentialHours"].DefaultCellStyle.Format = "N2";
                dgvDTR.Columns["NightDifferentialOtHours"].DefaultCellStyle.Format = "N2";

                dgvDTR.Columns["TimeIn"].DefaultCellStyle.Format = "hh\\:mm";
                dgvDTR.Columns["TimeOut"].DefaultCellStyle.Format = "hh\\:mm";
                dgvDTR.Columns["StartTime"].DefaultCellStyle.Format = "hh\\:mm";
                dgvDTR.Columns["EndTime"].DefaultCellStyle.Format = "hh\\:mm";

                dgvDTR.Columns["EmployeeID"].Visible = false;
                dgvDTR.Columns["EmployeeName"].Visible = false;

                dgvDTR.Columns["Rate"].DisplayIndex = 3;
                dgvDTR.Columns["ShiftCode"].DisplayIndex = 3;

                HighlightRestDaysAndUpdateRemarks(dgvDTR);
                SetCheckBoxColumnStyles(); // <-- Ensure styles are set after columns are created
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
            dt.Columns.Add("Remarks", typeof(string));
            dt.Columns.Add("TardinessUndertime", typeof(decimal));
            dt.Columns.Add("RestDay", typeof(bool));
            dt.Columns.Add("LegalHoliday", typeof(bool));
            dt.Columns.Add("SpecialHoliday", typeof(bool));
            dt.Columns.Add("NonWorkingDay", typeof(bool));
            dt.Columns.Add("Reliever", typeof(bool)); // Add Reliever column

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
            COALESCE(p.nd_hrs, sc.night_differential_hours, 0.00) AS NightDifferentialHours,
            COALESCE(p.ndot_hrs, sc.night_differential_ot_hours, 0.00) AS NightDifferentialOtHours,
            p.remarks AS Remarks,
            p.tardiness_undertime AS TardinessUndertime,
            p.rest_day AS RestDay,
            p.legal_holiday AS LegalHoliday,
            p.special_holiday AS SpecialHoliday,
            p.non_working_day AS NonWorkingDay,
            p.reliever AS Reliever
        FROM DateRange d
        LEFT JOIN employee e ON e.id_no = @employeeID
        LEFT JOIN processedDTR p ON e.id_no = p.employee_id AND p.date = d.Date
        LEFT JOIN attendance a ON e.id_no = a.id AND a.date = d.Date
        LEFT JOIN ShiftCodes sc ON p.shift_code = sc.shift_code
        GROUP BY d.Date;";

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

        // Add this overload to ensure it exists for DataGridView
        private void HighlightRestDaysAndUpdateRemarks(DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                bool isRestDay = Convert.ToBoolean(row.Cells["RestDay"].Value);

                // Set text color to red for rest days
                if (isRestDay)
                {
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.Red; // Set text color to red
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black; // Set text color to black for non-rest days
                }
            }
        }

        // Add stubs for all event handler methods referenced in the constructor
        private void dgvDTR_DataError(object sender, DataGridViewDataErrorEventArgs e) { }
        private void dgvDTR_CellEndEdit(object sender, DataGridViewCellEventArgs e) { }
        private void dgvDTR_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) { }
        private void dgvDTR_CellValueChanged(object sender, DataGridViewCellEventArgs e) { }
        private void dgvDTR_CurrentCellDirtyStateChanged(object sender, EventArgs e) { }
        private void dgvDTR_CellEnter(object sender, DataGridViewCellEventArgs e) { }
        private void dgvDTR_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) { }
        private void FillDownCurrentCell() { }
        private void TextBox_Enter(object sender, EventArgs e) { }
        private void TextBox_Leave(object sender, EventArgs e) { }
        private void TextBox_TextChanged(object sender, EventArgs e) { }
        private void AutoFormatDate(object sender, KeyPressEventArgs e) { }
        private void TextBox_Paint(object sender, PaintEventArgs e) { }
        private void SetPlaceholderText(TextBox textBox, string placeholderText) { }
    }
}
