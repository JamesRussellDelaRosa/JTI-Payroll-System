using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; // Add this line to resolve ambiguity
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using MySql.Data.MySqlClient;
using System.Linq;

namespace JTI_Payroll_System
{
    public partial class processDTR : Form
    {
        private List<string> employeeIDs = new List<string>(); // Store employee IDs with attendance
        private int currentEmployeeIndex = 0; // Track current employee position
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private string selectedCcode = null; // Add selectedCcode

        public processDTR()
        {
            InitializeComponent();
        }

        public processDTR(DateTime fromDate, DateTime toDate) : this()
        {
            _fromDate = fromDate;
            _toDate = toDate;
            UpdateDateRangeLabel();
        }

        public processDTR(DateTime fromDate, DateTime toDate, string ccode) : this()
        {
            _fromDate = fromDate;
            _toDate = toDate;
            selectedCcode = ccode;
            UpdateDateRangeLabel();
            // Auto-load attendance data for the selected ccode and date range
            FilterByCcode();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadCcodePanels();
            UpdateDateRangeLabel();
            // Do NOT show the filter dialog here, since it's already handled in User.cs
            search.Click += search_Click;
            // Attach EditingControlShowing for time auto-format
            dgvDTR.EditingControlShowing += dgvDTR_EditingControlShowing;
            // Attach CellValueChanged for TimeIn/TimeOut auto-calc
            dgvDTR.CellValueChanged += dgvDTR_CellValueChanged;
        }

        private void ShowDateRangeFilterAndLoad()
        {
            using (var filterForm = new DateRangeFilterForm())
            {
                if (filterForm.ShowDialog() == DialogResult.OK)
                {
                    _fromDate = filterForm.FromDate;
                    _toDate = filterForm.ToDate;
                    selectedCcode = filterForm.SelectedCcode;
                    UpdateDateRangeLabel();
                    FilterByCcode();
                }
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            using (var filterForm = new DateRangeFilterForm())
            {
                if (filterForm.ShowDialog() == DialogResult.OK)
                {
                    _fromDate = filterForm.FromDate;
                    _toDate = filterForm.ToDate;
                    selectedCcode = filterForm.SelectedCcode;
                    UpdateDateRangeLabel();
                    FilterByCcode();
                }
            }
        }

        private void UpdateDateRangeLabel()
        {
            if (_fromDate.HasValue && _toDate.HasValue)
            {
                labelDateRange.Text = $"Date Range: {_fromDate:MM/dd/yyyy} - {_toDate:MM/dd/yyyy}";
            }
            else
            {
                labelDateRange.Text = "Date Range: MM/DD/YYYY - MM/DD/YYYY";
            }
        }

        private void LoadCcodePanels()
        {
            flowCcodePanels.Controls.Clear();
            List<string> ccodeList = new List<string>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT DISTINCT ccode FROM employee WHERE ccode IS NOT NULL AND ccode != '' ORDER BY ccode", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ccodeList.Add(reader.GetString(0));
                    }
                }
            }
            foreach (var ccode in ccodeList)
            {
                var panel = new System.Windows.Forms.Panel
                {
                    Width = 100,
                    Height = 40,
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                    Margin = new System.Windows.Forms.Padding(3),
                    Cursor = System.Windows.Forms.Cursors.Hand,
                    Tag = ccode
                };
                var lbl = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Width = 100,
                    Height = 40,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular),
                    Text = ccode,
                    Cursor = System.Windows.Forms.Cursors.Hand
                };
                panel.Controls.Add(lbl);
                panel.Click += (s, e) => SelectCcodePanel(panel, ccode);
                lbl.Click += (s, e) => SelectCcodePanel(panel, ccode);
                flowCcodePanels.Controls.Add(panel);
            }
        }

        private void SelectCcodePanel(System.Windows.Forms.Panel panel, string ccode)
        {
            // Highlight selected panel
            foreach (System.Windows.Forms.Panel p in flowCcodePanels.Controls)
                p.BackColor = System.Drawing.SystemColors.Control;
            panel.BackColor = System.Drawing.Color.LightBlue;
            selectedCcode = ccode;
            // Trigger filter
            FilterByCcode();
        }

        private void FilterByCcode()
        {
            if (string.IsNullOrEmpty(selectedCcode)) return;
            if (!_fromDate.HasValue || !_toDate.HasValue)
            {
                MessageBox.Show("Invalid date range. Please select a valid date range.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime startDate = _fromDate.Value;
            DateTime endDate = _toDate.Value;

            // Get employee IDs for selected ccode (regardless of attendance)
            employeeIDs.Clear();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT id_no, CONCAT(fname, ' ', lname) AS EmployeeName FROM employee WHERE ccode = @ccode ORDER BY id_no", conn))
                {
                    cmd.Parameters.AddWithValue("@ccode", selectedCcode);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employeeIDs.Add(reader.GetString(0));
                        }
                    }
                }
            }
            if (employeeIDs.Count > 0)
            {
                currentEmployeeIndex = 0;
                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], startDate, endDate);
            }
            else
            {
                dgvDTR.DataSource = null;
                textID.Text = "";
                textName.Text = "";
                MessageBox.Show($"No employees found for ccode: {selectedCcode}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // Attach EditingControlShowing event to handle user-typed input
            dgvDTR.EditingControlShowing += (sender, e) =>
            {
                if (dgvDTR.CurrentCell != null && dgvDTR.CurrentCell.ColumnIndex == dgvDTR.Columns[rateColumnName].Index)
                {
                    if (e.Control is ComboBox comboBox)
                    {
                        comboBox.DropDownStyle = ComboBoxStyle.DropDown; // Allow typing
                        comboBox.Validating -= ComboBox_Validating; // Remove any existing handler
                        comboBox.Validating += ComboBox_Validating; // Add new handler
                    }
                }
            };
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

            // Attach EditingControlShowing event to handle user-typed input
            dgvDTR.EditingControlShowing += (sender, e) =>
            {
                if (dgvDTR.CurrentCell != null && dgvDTR.CurrentCell.ColumnIndex == dgvDTR.Columns[shiftCodeColumnName].Index)
                {
                    if (e.Control is ComboBox comboBox)
                    {
                        comboBox.DropDownStyle = ComboBoxStyle.DropDown; // Allow typing
                        comboBox.Validating -= ShiftCodeComboBox_Validating; // Remove any existing handler
                        comboBox.Validating += ShiftCodeComboBox_Validating; // Add new handler
                    }
                }
            };
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
        private void LoadEmployeesForNavigation(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT e.id_no
                    FROM employee e
                    ORDER BY e.id_no ASC;";

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

                // Setup columns once
                SetupRateDropdown();
                SetupShiftCodeDropdown();

                // Ensure checkbox columns are properly configured
                SetupCheckBoxColumns();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "DTR Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Setup checkbox columns to ensure they're properly configured
        private void SetupCheckBoxColumns()
        {
            string[] checkBoxColumns = { "RestDay", "LegalHoliday", "SpecialHoliday", "NonWorkingDay", "Reliever" };

            foreach (string colName in checkBoxColumns)
            {
                if (dgvDTR.Columns.Contains(colName))
                {
                    var column = dgvDTR.Columns[colName];

                    // If it's not already a checkbox column, convert it
                    if (!(column is DataGridViewCheckBoxColumn))
                    {
                        int columnIndex = column.Index;
                        string headerText = column.HeaderText;
                        bool visible = column.Visible;
                        int displayIndex = column.DisplayIndex;

                        // Remove the existing column
                        dgvDTR.Columns.Remove(column);

                        // Create a new checkbox column
                        DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn
                        {
                            Name = colName,
                            HeaderText = headerText,
                            DataPropertyName = colName,
                            Visible = visible,
                            TrueValue = true,
                            FalseValue = false,
                            IndeterminateValue = false,
                            ThreeState = false
                        };

                        // Insert at the same position
                        dgvDTR.Columns.Insert(columnIndex, checkBoxColumn);
                        checkBoxColumn.DisplayIndex = displayIndex;
                    }
                }
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
            dt.Columns.Add("Reliever", typeof(bool));

            // Get employee name for manual row creation
            string employeeName = "Unknown Employee";
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT CONCAT(fname, ' ', lname) FROM employee WHERE id_no = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", employeeID);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        employeeName = result.ToString();
                }
            }

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
            COALESCE(p.tardiness_undertime, 0.00) AS TardinessUndertime,
            COALESCE(p.rest_day, false) AS RestDay,
            COALESCE(p.legal_holiday, false) AS LegalHoliday,
            COALESCE(p.special_holiday, false) AS SpecialHoliday,
            COALESCE(p.non_working_day, false) AS NonWorkingDay,
            COALESCE(p.reliever, false) AS Reliever
        FROM employee e
        JOIN DateRange d ON 1=1
        LEFT JOIN attendance a
            ON e.id_no = a.id AND a.date = d.Date
        LEFT JOIN processedDTR p
            ON e.id_no = p.employee_id AND p.date = d.Date
        LEFT JOIN ShiftCodes sc
            ON p.shift_code = sc.shift_code
        WHERE e.id_no = @employeeID
        GROUP BY
            e.id_no, e.fname, e.lname, d.Date,
            p.time_in, p.time_out, p.rate,
            p.shift_code, sc.start_time, sc.end_time,
            sc.regular_hours, p.ot_hrs, sc.ot_hours,
            p.nd_hrs, sc.night_differential_hours, p.ndot_hrs, sc.night_differential_ot_hours,
            p.remarks, p.tardiness_undertime,
            p.rest_day, p.legal_holiday, p.special_holiday, p.non_working_day, p.reliever
        ORDER BY d.Date ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["EmployeeID"] = reader["EmployeeID"];
                                row["EmployeeName"] = reader["EmployeeName"];
                                row["Date"] = reader["Date"];
                                // Convert varchar HHMM time to proper TimeSpan
                                if (reader["TimeIn"] != DBNull.Value)
                                {
                                    string timeInStr = reader["TimeIn"].ToString();
                                    row["TimeIn"] = ConvertHHMMToTimeSpan(timeInStr);
                                }

                                if (reader["TimeOut"] != DBNull.Value)
                                {
                                    string timeOutStr = reader["TimeOut"].ToString();
                                    row["TimeOut"] = ConvertHHMMToTimeSpan(timeOutStr);
                                }

                                row["Rate"] = reader["Rate"];
                                row["WorkingHours"] = reader["WorkingHours"];
                                row["OTHours"] = reader["OTHours"];
                                row["ShiftCode"] = reader["ShiftCode"];

                                // Handle StartTime and EndTime from database
                                if (reader["StartTime"] != DBNull.Value)
                                {
                                    row["StartTime"] = reader.GetTimeSpan(reader.GetOrdinal("StartTime"));
                                }

                                if (reader["EndTime"] != DBNull.Value)
                                {
                                    row["EndTime"] = reader.GetTimeSpan(reader.GetOrdinal("EndTime"));
                                }

                                row["NightDifferentialHours"] = reader["NightDifferentialHours"];
                                row["NightDifferentialOtHours"] = reader["NightDifferentialOtHours"];
                                row["Remarks"] = reader["Remarks"];
                                row["TardinessUndertime"] = reader["TardinessUndertime"];
                                row["RestDay"] = reader["RestDay"];
                                row["LegalHoliday"] = reader["LegalHoliday"];
                                row["SpecialHoliday"] = reader["SpecialHoliday"];
                                row["NonWorkingDay"] = reader["NonWorkingDay"];
                                row["Reliever"] = reader["Reliever"];

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Ensure all dates in range are present, even if no attendance data exists
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (!dt.AsEnumerable().Any(row => Convert.ToDateTime(row["Date"]) == date))
                {
                    DataRow newRow = dt.NewRow();
                    newRow["EmployeeID"] = employeeID;
                    newRow["EmployeeName"] = employeeName;
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

            return dt;
        }
        private TimeSpan ConvertHHMMToTimeSpan(string hhmmString)
        {
            if (string.IsNullOrEmpty(hhmmString))
                return TimeSpan.Zero;

            // Clean up the string to ensure it only contains digits
            string cleanedInput = new string(hhmmString.Where(char.IsDigit).ToArray());

            if (cleanedInput.Length < 3)
                return TimeSpan.Zero;

            // Ensure we have at least 3-4 digits
            while (cleanedInput.Length < 4)
                cleanedInput = "0" + cleanedInput;

            // Extract hours and minutes from the HHMM format
            int hours = int.Parse(cleanedInput.Substring(0, 2));
            int minutes = int.Parse(cleanedInput.Substring(2, 2));

            // Validate and cap hours/minutes to valid ranges
            hours = Math.Min(hours, 23);
            minutes = Math.Min(minutes, 59);

            return new TimeSpan(hours, minutes, 0);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (employeeIDs.Count == 0) return;

            if (currentEmployeeIndex < employeeIDs.Count - 1)
            {
                currentEmployeeIndex++;

                if (!_fromDate.HasValue || !_toDate.HasValue)
                {
                    MessageBox.Show("Invalid date range. Please select a valid date range.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], _fromDate.Value, _toDate.Value);
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

                if (!_fromDate.HasValue || !_toDate.HasValue)
                {
                    MessageBox.Show("Invalid date range. Please select a valid date range.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadEmployeeDTR(employeeIDs[currentEmployeeIndex], _fromDate.Value, _toDate.Value);
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
                        TimeSpan? timeIn = row.Cells["TimeIn"].Value != DBNull.Value
                            ? (TimeSpan?)TimeSpan.Parse(row.Cells["TimeIn"].Value.ToString())
                            : null;
                        TimeSpan? timeOut = row.Cells["TimeOut"].Value != DBNull.Value
                            ? (TimeSpan?)TimeSpan.Parse(row.Cells["TimeOut"].Value.ToString())
                            : null;
                        decimal rate = row.Cells["Rate"].Value != DBNull.Value
                            ? Convert.ToDecimal(row.Cells["Rate"].Value)
                            : 0.00m;
                        decimal workingHours = row.Cells["WorkingHours"].Value != DBNull.Value
                            ? Convert.ToDecimal(row.Cells["WorkingHours"].Value)
                            : 0.00m;
                        decimal otHours = row.Cells["OTHours"].Value != DBNull.Value
                            ? Convert.ToDecimal(row.Cells["OTHours"].Value)
                            : 0.00m;
                        string shiftCode = row.Cells["ShiftCode"].Value?.ToString();
                        decimal ndHours = row.Cells["NightDifferentialHours"].Value != DBNull.Value
                            ? Convert.ToDecimal(row.Cells["NightDifferentialHours"].Value)
                            : 0.00m;
                        decimal ndOtHours = row.Cells["NightDifferentialOtHours"].Value != DBNull.Value
                            ? Convert.ToDecimal(row.Cells["NightDifferentialOtHours"].Value)
                            : 0.00m;
                        string remarks = row.Cells["Remarks"].Value?.ToString();
                        decimal tardinessUndertime = 0.00m;

                        // Retrieve checkbox columns
                        bool restDay = row.Cells["RestDay"].Value != DBNull.Value
                            && Convert.ToBoolean(row.Cells["RestDay"].Value);
                        bool legalHoliday = row.Cells["LegalHoliday"].Value != DBNull.Value
                            && Convert.ToBoolean(row.Cells["LegalHoliday"].Value);
                        bool specialHoliday = row.Cells["SpecialHoliday"].Value != DBNull.Value
                            && Convert.ToBoolean(row.Cells["SpecialHoliday"].Value);
                        bool nonWorkingDay = row.Cells["NonWorkingDay"].Value != DBNull.Value
                            && Convert.ToBoolean(row.Cells["NonWorkingDay"].Value);
                        bool reliever = row.Cells["Reliever"].Value != DBNull.Value
                            && Convert.ToBoolean(row.Cells["Reliever"].Value);

                        if (!string.IsNullOrEmpty(shiftCode) && shiftCode != "00000")
                        {
                            tardinessUndertime = row.Cells["TardinessUndertime"].Value != DBNull.Value
                                ? Convert.ToDecimal(row.Cells["TardinessUndertime"].Value)
                                : 0.00m;
                        }

                        // Check if record already exists
                        string checkQuery = "SELECT COUNT(*) FROM processedDTR WHERE employee_id = @employeeID AND date = @date";
                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            checkCmd.Parameters.AddWithValue("@date", date);
                            int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (recordExists > 0)
                            {
                                // Update existing record
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
                        remarks = @remarks,
                        tardiness_undertime = @tardinessUndertime,
                        rest_day = @restDay,
                        legal_holiday = @legalHoliday,
                        special_holiday = @specialHoliday,
                        non_working_day = @nonWorkingDay,
                        reliever = @reliever
                    WHERE employee_id = @employeeID AND date = @date";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@rate", rate);
                                    updateCmd.Parameters.AddWithValue("@timeIn", (object)timeIn ?? DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@timeOut", (object)timeOut ?? DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@workingHours", workingHours);
                                    updateCmd.Parameters.AddWithValue("@otHours", otHours);
                                    updateCmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                                    updateCmd.Parameters.AddWithValue("@ndHours", ndHours);
                                    updateCmd.Parameters.AddWithValue("@ndOtHours", ndOtHours);
                                    updateCmd.Parameters.AddWithValue("@remarks", remarks);
                                    updateCmd.Parameters.AddWithValue("@tardinessUndertime", tardinessUndertime);
                                    updateCmd.Parameters.AddWithValue("@restDay", restDay);
                                    updateCmd.Parameters.AddWithValue("@legalHoliday", legalHoliday);
                                    updateCmd.Parameters.AddWithValue("@specialHoliday", specialHoliday);
                                    updateCmd.Parameters.AddWithValue("@nonWorkingDay", nonWorkingDay);
                                    updateCmd.Parameters.AddWithValue("@reliever", reliever);
                                    updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                    updateCmd.Parameters.AddWithValue("@date", date);

                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert a new record
                                string insertQuery = @"
                    INSERT INTO processedDTR (
                        employee_id, date, time_in, time_out, rate, 
                        working_hours, ot_hrs, shift_code, nd_hrs, ndot_hrs, 
                        remarks, tardiness_undertime, rest_day, legal_holiday, 
                        special_holiday, non_working_day, reliever
                    )
                    VALUES (
                        @employeeID, @date, @timeIn, @timeOut, @rate,
                        @workingHours, @otHours, @shiftCode, @ndHours, @ndOtHours,
                        @remarks, @tardinessUndertime, @restDay, @legalHoliday, 
                        @specialHoliday, @nonWorkingDay, @reliever
                    )";

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
                                    insertCmd.Parameters.AddWithValue("@tardinessUndertime", tardinessUndertime);
                                    insertCmd.Parameters.AddWithValue("@restDay", restDay);
                                    insertCmd.Parameters.AddWithValue("@legalHoliday", legalHoliday);
                                    insertCmd.Parameters.AddWithValue("@specialHoliday", specialHoliday);
                                    insertCmd.Parameters.AddWithValue("@nonWorkingDay", nonWorkingDay);
                                    insertCmd.Parameters.AddWithValue("@reliever", reliever);

                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Processed DTR saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving processed DTR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Auto-format time input for TimeIn, TimeOut, StartTime, EndTime columns
        private void dgvDTR_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvDTR.CurrentCell != null)
            {
                string colName = dgvDTR.Columns[dgvDTR.CurrentCell.ColumnIndex].Name;
                if (colName == "TimeIn" || colName == "TimeOut" || colName == "StartTime" || colName == "EndTime")
                {
                    if (e.Control is TextBox tb)
                    {
                        tb.KeyPress -= TimeCell_KeyPress; // Remove previous handler if any
                        tb.KeyPress += TimeCell_KeyPress;
                    }
                }
                else
                {
                    if (e.Control is TextBox tb)
                    {
                        tb.KeyPress -= TimeCell_KeyPress;
                    }
                }
            }
        }
        private void TimeCell_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Allow control keys (backspace, delete, arrows)
            if (char.IsControl(e.KeyChar))
                return;
            // Allow only digits
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            // Insert colon after two digits if not already present
            string text = tb.Text;
            int selStart = tb.SelectionStart;
            // Only auto-insert if length is 2 and no colon yet
            if (text.Length == 2 && !text.Contains(":"))
            {
                tb.Text = text + ":";
                tb.SelectionStart = tb.Text.Length;
            }
        }

        // Calculate remarks and tardiness/undertime when TimeIn, TimeOut, or ShiftCode changes
        private void dgvDTR_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var colName = dgvDTR.Columns[e.ColumnIndex].Name;
            var row = dgvDTR.Rows[e.RowIndex];

            if (colName == "ShiftCode")
            {
                // Update row values based on selected ShiftCode
                var shiftCode = row.Cells["ShiftCode"].Value?.ToString();
                if (!string.IsNullOrEmpty(shiftCode) && shiftCode != "00000")
                {
                    var shiftData = GetShiftCodeData(shiftCode);
                    if (shiftData != null)
                    {
                        row.Cells["StartTime"].Value = shiftData.StartTime;
                        row.Cells["EndTime"].Value = shiftData.EndTime;
                        row.Cells["WorkingHours"].Value = shiftData.RegularHours;
                        row.Cells["OTHours"].Value = shiftData.OtHours;
                        row.Cells["NightDifferentialHours"].Value = shiftData.NightDifferentialHours;
                        row.Cells["NightDifferentialOtHours"].Value = shiftData.NightDifferentialOtHours;
                    }
                }
                else
                {
                    // Clear shift-related fields if shift code is empty or 00000
                    row.Cells["StartTime"].Value = DBNull.Value;
                    row.Cells["EndTime"].Value = DBNull.Value;
                    row.Cells["WorkingHours"].Value = 0.00m;
                    row.Cells["OTHours"].Value = 0.00m;
                    row.Cells["NightDifferentialHours"].Value = 0.00m;
                    row.Cells["NightDifferentialOtHours"].Value = 0.00m;
                }
            }

            if (colName == "TimeIn" || colName == "TimeOut" || colName == "ShiftCode")
            {
                // Calculate and update Remarks
                string remarks = CalculateRemarks(row);
                row.Cells["Remarks"].Value = remarks;

                // Also update TardinessUndertime using DataRow logic if bound
                if (row.DataBoundItem is DataRowView drv)
                {
                    CalculateTardinessUndertime(drv.Row);
                    // Sync DataRow value back to DataGridView cell (in case logic differs)
                    row.Cells["TardinessUndertime"].Value = drv.Row["TardinessUndertime"];
                }
            }
        }
        private string CalculateRemarks(DataGridViewRow row)
        {
            // Defensive: Check for null or DBNull for all required cells
            if (row.Cells["TimeIn"].Value == null || row.Cells["TimeOut"].Value == null ||
                row.Cells["TimeIn"].Value == DBNull.Value || row.Cells["TimeOut"].Value == DBNull.Value)
            {
                row.Cells["TardinessUndertime"].Value = 0.00m;
                return "Absent";
            }

            if (row.Cells["StartTime"].Value == null || row.Cells["EndTime"].Value == null ||
                row.Cells["StartTime"].Value == DBNull.Value || row.Cells["EndTime"].Value == DBNull.Value)
            {
                row.Cells["TardinessUndertime"].Value = 0.00m;
                return "No Shift Data";
            }

            // Defensive: TryParse for TimeSpan
            if (!TimeSpan.TryParse(row.Cells["TimeIn"].Value.ToString(), out TimeSpan timeIn) ||
                !TimeSpan.TryParse(row.Cells["TimeOut"].Value.ToString(), out TimeSpan timeOut) ||
                !TimeSpan.TryParse(row.Cells["StartTime"].Value.ToString(), out TimeSpan startTime) ||
                !TimeSpan.TryParse(row.Cells["EndTime"].Value.ToString(), out TimeSpan endTime))
            {
                row.Cells["TardinessUndertime"].Value = 0.00m;
                return "Invalid Time Data";
            }

            double tardiness = 0;
            double undertime = 0;

            if (timeIn > startTime)
            {
                TimeSpan tardinessDifference = timeIn - startTime;
                tardiness = tardinessDifference.TotalHours;
            }

            if (timeOut < endTime)
            {
                TimeSpan undertimeDifference = endTime - timeOut;
                undertime = undertimeDifference.TotalHours;
            }

            double total = tardiness + undertime;
            row.Cells["TardinessUndertime"].Value = Math.Round((decimal)total, 2);

            if (total > 0)
            {
                return "Late or Undertime";
            }
            else
            {
                return "Present";
            }
        }
        private void CalculateTardinessUndertime(DataRow row)
        {
            if (row["ShiftCode"] == null || row["ShiftCode"] == DBNull.Value ||
                string.IsNullOrEmpty(row["ShiftCode"].ToString()) || row["ShiftCode"].ToString() == "00000")
            {
                row["TardinessUndertime"] = 0.00m;
                return;
            }

            if (row["TimeIn"] == null || row["TimeOut"] == null ||
                row["TimeIn"] == DBNull.Value || row["TimeOut"] == DBNull.Value)
            {
                row["TardinessUndertime"] = 0.00m;
                return;
            }

            if (row["StartTime"] == null || row["EndTime"] == null ||
                row["StartTime"] == DBNull.Value || row["EndTime"] == DBNull.Value)
            {
                row["TardinessUndertime"] = 0.00m;
                return;
            }

            if (!TimeSpan.TryParse(row["TimeIn"].ToString(), out TimeSpan timeIn) ||
                !TimeSpan.TryParse(row["TimeOut"].ToString(), out TimeSpan timeOut) ||
                !TimeSpan.TryParse(row["StartTime"].ToString(), out TimeSpan startTime) ||
                !TimeSpan.TryParse(row["EndTime"].ToString(), out TimeSpan endTime))
            {
                row["TardinessUndertime"] = 0.00m;
                return;
            }

            double tardiness = 0;
            double undertime = 0;

            if (timeIn > startTime)
            {
                TimeSpan tardinessDifference = timeIn - startTime;
                double tardyHours = tardinessDifference.TotalHours;
                tardiness = tardyHours;
            }

            if (timeOut < endTime)
            {
                TimeSpan undertimeDifference = endTime - timeOut;
                double undertimeHours = undertimeDifference.TotalHours;
                undertime = undertimeHours;
            }

            double total = tardiness + undertime;
            row["TardinessUndertime"] = Math.Round((decimal)total, 2);
        }

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
        private void HighlightRestDaysAndUpdateRemarks(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                DateTime date = Convert.ToDateTime(row["Date"]);
                DayOfWeek dayOfWeek = date.DayOfWeek;

                // Mark Sundays as rest days
                if (dayOfWeek == DayOfWeek.Sunday)
                {
                    row["RestDay"] = true; // Mark as rest day
                    row["Remarks"] = "Rest Day"; // Indicate rest day in remarks
                }
            }
        }
    }
}
