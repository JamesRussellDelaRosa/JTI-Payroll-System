using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class PayrollPost : Form
    {
        public PayrollPost()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private bool allowRepost = false;

        private void InitializeCustomComponents()
        {
            fromdate.Enter += new EventHandler(RemoveHint);
            fromdate.Leave += new EventHandler(AddHint);
            fromdate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            todate.Enter += new EventHandler(RemoveHint);
            todate.Leave += new EventHandler(AddHint);
            todate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            AddHint(fromdate, null);
            AddHint(todate, null);
        }
        private void RemoveHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "MM/DD/YYYY")
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }
        private void AddHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "MM/DD/YYYY";
                textBox.ForeColor = Color.Gray;
            }
        }
        private void CalculateAndSavePayroll(DateTime startDate, DateTime endDate, int month, int payrollyear, int controlPeriod)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Check if payroll data already exists for the specified cutoff period, month, year, and control period
                    string checkExistingPayrollQuery = @"
                SELECT COUNT(*) 
                FROM payroll 
                WHERE pay_period_start = @startDate 
                AND pay_period_end = @endDate
                AND month = @month
                AND payrollyear = @payrollyear
                AND control_period = @controlPeriod";

                    using (MySqlCommand checkCmd = new MySqlCommand(checkExistingPayrollQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@startDate", startDate);
                        checkCmd.Parameters.AddWithValue("@endDate", endDate);
                        checkCmd.Parameters.AddWithValue("@month", month);
                        checkCmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                        checkCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);

                        int existingPayrollCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (existingPayrollCount > 0 && !allowRepost)
                        {
                            MessageBox.Show("Payroll data already exists for the specified cutoff period, month, year, and control period. No new data will be inserted.",
                                "Existing Payroll Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else if (existingPayrollCount > 0 && allowRepost)
                        {
                            string deleteQuery = @"
                        DELETE FROM payroll 
                        WHERE pay_period_start = @startDate 
                        AND pay_period_end = @endDate
                        AND month = @month
                        AND payrollyear = @payrollyear
                        AND control_period = @controlPeriod";

                            using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@startDate", startDate);
                                deleteCmd.Parameters.AddWithValue("@endDate", endDate);
                                deleteCmd.Parameters.AddWithValue("@month", month);
                                deleteCmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                                deleteCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);

                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Fetch employee IDs with attendance in the given date range
                    List<string> employeeIDs = new List<string>();
                    string query = @"
                SELECT DISTINCT e.id_no
                FROM employee e
                LEFT JOIN processedDTR p ON e.id_no = p.employee_id AND p.date BETWEEN @startDate AND @endDate
                LEFT JOIN attendance a ON e.id_no = a.id AND a.date BETWEEN @startDate AND @endDate
                WHERE p.employee_id IS NOT NULL OR a.id IS NOT NULL
                ORDER BY e.id_no ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employeeIDs.Add(reader["id_no"].ToString());
                            }
                        }
                    }

                    foreach (string employeeID in employeeIDs)
                    {
                        // Fetch employee name and ccode details
                        string lname = "", fname = "", mname = "", ccode = "";
                        string nameQuery = "SELECT lname, fname, mname, ccode FROM employee WHERE id_no = @employeeID LIMIT 1";
                        using (MySqlCommand nameCmd = new MySqlCommand(nameQuery, conn))
                        {
                            nameCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            using (MySqlDataReader nameReader = nameCmd.ExecuteReader())
                            {
                                if (nameReader.Read())
                                {
                                    lname = nameReader["lname"].ToString();
                                    fname = nameReader["fname"].ToString();
                                    mname = nameReader["mname"].ToString();
                                    ccode = nameReader["ccode"].ToString();
                                }
                            }
                        }

                        // Get unique rate and reliever combinations for this employee
                        var rateRelieverCombinations = new List<(decimal rate, bool reliever)>();
                        query = @"
                    SELECT DISTINCT p.rate, p.reliever
                    FROM processedDTR p
                    WHERE p.employee_id = @employeeID 
                      AND p.date BETWEEN @startDate AND @endDate
                      AND p.rate > 0
                    ORDER BY p.rate, p.reliever";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeID", employeeID);
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.Parameters.AddWithValue("@endDate", endDate);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    decimal rate = reader.GetDecimal("rate");
                                    bool isReliever = reader.GetBoolean("reliever");
                                    rateRelieverCombinations.Add((rate, isReliever));
                                }
                            }
                        }

                        foreach (var combination in rateRelieverCombinations)
                        {
                            decimal rate = combination.rate;
                            bool isReliever = combination.reliever;

                            if (rate <= 0)
                                continue;

                            // Initialize variables for different types of hours
                            decimal totalHours = 0;
                            decimal overtimeHours = 0;
                            decimal totalEarnings = 0;
                            decimal totalDays = 0;
                            decimal restdayHours = 0;
                            decimal restdayOvertimeHours = 0;
                            decimal legalHolidayHours = 0;
                            decimal legalHolidayOvertimeHours = 0;
                            decimal lhrdHours = 0;
                            decimal lhrdOvertimeHours = 0;
                            decimal specialHolidayHours = 0;
                            decimal specialHolidayOvertimeHours = 0;
                            decimal specialHolidayRestDayHours = 0;
                            decimal specialHolidayRestDayOvertimeHours = 0;
                            decimal nightDifferentialHours = 0;
                            decimal nightDifferentialOtHours = 0;
                            decimal nightDifferentialRestDayHours = 0;
                            decimal nightDifferentialRestDayOtHours = 0;
                            decimal nightDifferentialSpecialHolidayHours = 0;
                            decimal nightDifferentialSpecialHolidayOtHours = 0;
                            decimal nightDifferentialSpecialHolidayRestDayHours = 0;
                            decimal nightDifferentialSpecialHolidayRestDayOtHours = 0;
                            decimal nightDifferentialLegalHolidayHours = 0;
                            decimal nightDifferentialLegalHolidayOtHours = 0;
                            decimal nightDifferentialLegalHolidayRestDayHours = 0;
                            decimal nightDifferentialLegalHolidayRestDayOtHours = 0;
                            decimal totalTardinessUndertime = 0;
                            decimal totalWorkingHours = 0;
                            int legalHolidayCount = 0;
                            int nonWorkingDayCount = 0;

                            // Get data for this specific rate/reliever combination
                            query = @"
                        SELECT p.working_hours, p.ot_hrs, s.regular_hours, p.rest_day, p.legal_holiday, 
                               p.special_holiday, p.non_working_day, p.nd_hrs, p.ndot_hrs, p.tardiness_undertime
                        FROM processedDTR p
                        JOIN ShiftCodes s ON p.shift_code = s.shift_code
                        WHERE p.employee_id = @employeeID 
                          AND p.date BETWEEN @startDate AND @endDate
                          AND p.rate = @rate
                          AND p.reliever = @reliever";

                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@employeeID", employeeID);
                                cmd.Parameters.AddWithValue("@startDate", startDate);
                                cmd.Parameters.AddWithValue("@endDate", endDate);
                                cmd.Parameters.AddWithValue("@rate", rate);
                                cmd.Parameters.AddWithValue("@reliever", isReliever);

                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        decimal workingHours = reader.GetDecimal("working_hours");
                                        decimal otHours = reader.GetDecimal("ot_hrs");
                                        decimal regularHours = reader.GetDecimal("regular_hours");
                                        bool isRestDay = reader.GetBoolean("rest_day");
                                        bool isLegalHoliday = reader.GetBoolean("legal_holiday");
                                        bool isSpecialHoliday = reader.GetBoolean("special_holiday");
                                        bool isNonWorkingDay = reader.GetBoolean("non_working_day");
                                        decimal ndHours = reader.GetDecimal("nd_hrs");
                                        decimal ndOtHours = reader.GetDecimal("ndot_hrs");
                                        decimal tardinessUndertime = reader.GetDecimal("tardiness_undertime");

                                        totalEarnings += (workingHours + otHours) * rate;
                                        totalTardinessUndertime += tardinessUndertime;
                                        totalWorkingHours += workingHours;

                                        if (isNonWorkingDay) nonWorkingDayCount++;

                                        if (isRestDay && isLegalHoliday)
                                        {
                                            lhrdHours += workingHours;
                                            lhrdOvertimeHours += otHours;
                                            nightDifferentialLegalHolidayRestDayHours += ndHours;
                                            nightDifferentialLegalHolidayRestDayOtHours += ndOtHours;
                                        }
                                        else if (isRestDay && isSpecialHoliday)
                                        {
                                            specialHolidayRestDayHours += workingHours;
                                            specialHolidayRestDayOvertimeHours += otHours;
                                            nightDifferentialSpecialHolidayRestDayHours += ndHours;
                                            nightDifferentialSpecialHolidayRestDayOtHours += ndOtHours;
                                        }
                                        else if (isRestDay)
                                        {
                                            restdayHours += workingHours;
                                            restdayOvertimeHours += otHours;
                                            nightDifferentialRestDayHours += ndHours;
                                            nightDifferentialRestDayOtHours += ndOtHours;
                                        }
                                        else if (isLegalHoliday)
                                        {
                                            legalHolidayHours += workingHours;
                                            legalHolidayOvertimeHours += otHours;
                                            nightDifferentialLegalHolidayHours += ndHours;
                                            nightDifferentialLegalHolidayOtHours += ndOtHours;
                                            legalHolidayCount++;
                                        }
                                        else if (isSpecialHoliday)
                                        {
                                            specialHolidayHours += workingHours;
                                            specialHolidayOvertimeHours += otHours;
                                            nightDifferentialSpecialHolidayHours += ndHours;
                                            nightDifferentialSpecialHolidayOtHours += ndOtHours;
                                        }
                                        else
                                        {
                                            totalHours += workingHours;
                                            overtimeHours += otHours;
                                            totalHours += ndHours;
                                            overtimeHours += ndOtHours;
                                            nightDifferentialHours += ndHours;
                                            nightDifferentialOtHours += ndOtHours;
                                            if (regularHours > 0)
                                                totalDays += workingHours / regularHours;
                                        }
                                    }
                                }
                            }

                            // Check if payroll data already exists for this employee, rate, and reliever status
                            string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM payroll 
                        WHERE employee_id = @employeeID 
                          AND pay_period_start = @startDate 
                          AND pay_period_end = @endDate 
                          AND rate = @rate 
                          AND reliever = @reliever";

                            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                checkCmd.Parameters.AddWithValue("@startDate", startDate);
                                checkCmd.Parameters.AddWithValue("@endDate", endDate);
                                checkCmd.Parameters.AddWithValue("@rate", rate);
                                checkCmd.Parameters.AddWithValue("@reliever", isReliever);

                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    // Record already exists, perform update
                                    string updateQuery = @"
                                UPDATE payroll 
                                SET lname = @lname, fname = @fname, mname = @mname, ccode = @ccode,
                                    total_days = @totalDays, 
                                    overtime_hours = @overtimeHours, 
                                    total_earnings = @totalEarnings, 
                                    restday_hours = @restdayHours, 
                                    restday_overtime_hours = @restdayOvertimeHours, 
                                    legal_holiday_hours = @legalHolidayHours, 
                                    legal_holiday_overtime_hours = @legalHolidayOvertimeHours, 
                                    lhrd_hours = @lhrdHours, 
                                    lhrd_overtime_hours = @lhrdOvertimeHours, 
                                    special_holiday_hours = @specialHolidayHours, 
                                    special_holiday_overtime_hours = @specialHolidayOvertimeHours, 
                                    special_holiday_restday_hours = @specialHolidayRestDayHours, 
                                    special_holiday_restday_overtime_hours = @specialHolidayRestDayOvertimeHours, 
                                    nd_hrs = @nightDifferentialHours, 
                                    ndot_hrs = @nightDifferentialOtHours, 
                                    ndrd_hrs = @nightDifferentialRestDayHours, 
                                    ndrdot_hrs = @nightDifferentialRestDayOtHours, 
                                    ndsh_hrs = @nightDifferentialSpecialHolidayHours, 
                                    ndshot_hrs = @nightDifferentialSpecialHolidayOtHours, 
                                    ndshrd_hrs = @nightDifferentialSpecialHolidayRestDayHours, 
                                    ndshrdot_hrs = @nightDifferentialSpecialHolidayRestDayOtHours, 
                                    ndlh_hrs = @nightDifferentialLegalHolidayHours, 
                                    ndlhot_hrs = @nightDifferentialLegalHolidayOtHours, 
                                    ndlhrd_hrs = @nightDifferentialLegalHolidayRestDayHours, 
                                    ndlhrdot_hrs = @nightDifferentialLegalHolidayRestDayOtHours, 
                                    month = @month, 
                                    payrollyear = @payrollyear, 
                                    control_period = @controlPeriod, 
                                    td_ut = @totalTardinessUndertime, 
                                    working_hours = @totalWorkingHours,
                                    legal_holiday_count = @legalHolidayCount,
                                    non_working_day_count = @nonWorkingDayCount
                                WHERE employee_id = @employeeID 
                                  AND pay_period_start = @startDate 
                                  AND pay_period_end = @endDate
                                  AND rate = @rate
                                  AND reliever = @reliever";

                                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                    {
                                        updateCmd.Parameters.AddWithValue("@lname", lname);
                                        updateCmd.Parameters.AddWithValue("@fname", fname);
                                        updateCmd.Parameters.AddWithValue("@mname", mname);
                                        updateCmd.Parameters.AddWithValue("@ccode", ccode);
                                        updateCmd.Parameters.AddWithValue("@totalDays", totalDays);
                                        updateCmd.Parameters.AddWithValue("@overtimeHours", overtimeHours);
                                        updateCmd.Parameters.AddWithValue("@totalEarnings", totalEarnings);
                                        updateCmd.Parameters.AddWithValue("@restdayHours", restdayHours);
                                        updateCmd.Parameters.AddWithValue("@restdayOvertimeHours", restdayOvertimeHours);
                                        updateCmd.Parameters.AddWithValue("@legalHolidayHours", legalHolidayHours);
                                        updateCmd.Parameters.AddWithValue("@legalHolidayOvertimeHours", legalHolidayOvertimeHours);
                                        updateCmd.Parameters.AddWithValue("@lhrdHours", lhrdHours);
                                        updateCmd.Parameters.AddWithValue("@lhrdOvertimeHours", lhrdOvertimeHours);
                                        updateCmd.Parameters.AddWithValue("@specialHolidayHours", specialHolidayHours);
                                        updateCmd.Parameters.AddWithValue("@specialHolidayOvertimeHours", specialHolidayOvertimeHours);
                                        updateCmd.Parameters.AddWithValue("@specialHolidayRestDayHours", specialHolidayRestDayHours);
                                        updateCmd.Parameters.AddWithValue("@specialHolidayRestDayOvertimeHours", specialHolidayRestDayOvertimeHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialHours", nightDifferentialHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialOtHours", nightDifferentialOtHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialRestDayHours", nightDifferentialRestDayHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialRestDayOtHours", nightDifferentialRestDayOtHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayHours", nightDifferentialSpecialHolidayHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayOtHours", nightDifferentialSpecialHolidayOtHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayRestDayHours", nightDifferentialSpecialHolidayRestDayHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayRestDayOtHours", nightDifferentialSpecialHolidayRestDayOtHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayHours", nightDifferentialLegalHolidayHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayOtHours", nightDifferentialLegalHolidayOtHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayRestDayHours", nightDifferentialLegalHolidayRestDayHours);
                                        updateCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayRestDayOtHours", nightDifferentialLegalHolidayRestDayOtHours);
                                        updateCmd.Parameters.AddWithValue("@month", month);
                                        updateCmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                                        updateCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                                        updateCmd.Parameters.AddWithValue("@totalTardinessUndertime", totalTardinessUndertime);
                                        updateCmd.Parameters.AddWithValue("@totalWorkingHours", totalWorkingHours);
                                        updateCmd.Parameters.AddWithValue("@legalHolidayCount", legalHolidayCount);
                                        updateCmd.Parameters.AddWithValue("@nonWorkingDayCount", nonWorkingDayCount);
                                        updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                        updateCmd.Parameters.AddWithValue("@startDate", startDate);
                                        updateCmd.Parameters.AddWithValue("@endDate", endDate);
                                        updateCmd.Parameters.AddWithValue("@rate", rate);
                                        updateCmd.Parameters.AddWithValue("@reliever", isReliever);

                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Record does not exist, perform insert
                                    string insertQuery = @"
                                INSERT INTO payroll (
                                    employee_id, lname, fname, mname, ccode, pay_period_start, pay_period_end, total_days, overtime_hours, 
                                    total_earnings, restday_hours, restday_overtime_hours, legal_holiday_hours, 
                                    legal_holiday_overtime_hours, lhrd_hours, lhrd_overtime_hours, special_holiday_hours, 
                                    special_holiday_overtime_hours, special_holiday_restday_hours, special_holiday_restday_overtime_hours, 
                                    nd_hrs, ndot_hrs, ndrd_hrs, ndrdot_hrs, ndsh_hrs, ndshot_hrs, ndshrd_hrs, ndshrdot_hrs, 
                                    ndlh_hrs, ndlhot_hrs, ndlhrd_hrs, ndlhrdot_hrs, month, payrollyear, control_period, 
                                    td_ut, working_hours, legal_holiday_count, non_working_day_count, rate, reliever, SSS, philhealth, hdmf
                                )
                                VALUES (
                                    @employeeID, @lname, @fname, @mname, @ccode, @startDate, @endDate, @totalDays, @overtimeHours, 
                                    @totalEarnings, @restdayHours, @restdayOvertimeHours, @legalHolidayHours, 
                                    @legalHolidayOvertimeHours, @lhrdHours, @lhrdOvertimeHours, @specialHolidayHours, 
                                    @specialHolidayOvertimeHours, @specialHolidayRestDayHours, @specialHolidayRestDayOvertimeHours, 
                                    @nightDifferentialHours, @nightDifferentialOtHours, @nightDifferentialRestDayHours, @nightDifferentialRestDayOtHours, 
                                    @nightDifferentialSpecialHolidayHours, @nightDifferentialSpecialHolidayOtHours, 
                                    @nightDifferentialSpecialHolidayRestDayHours, @nightDifferentialSpecialHolidayRestDayOtHours, 
                                    @nightDifferentialLegalHolidayHours, @nightDifferentialLegalHolidayOtHours, 
                                    @nightDifferentialLegalHolidayRestDayHours, @nightDifferentialLegalHolidayRestDayOtHours, 
                                    @month, @payrollyear, @controlPeriod, @totalTardinessUndertime, @totalWorkingHours,
                                    @legalHolidayCount, @nonWorkingDayCount, @rate, @reliever, @sss, @philhealth, @hdmf
                                )";

                                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                    {
                                        insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
                                        insertCmd.Parameters.AddWithValue("@lname", lname);
                                        insertCmd.Parameters.AddWithValue("@fname", fname);
                                        insertCmd.Parameters.AddWithValue("@mname", mname);
                                        insertCmd.Parameters.AddWithValue("@ccode", ccode);
                                        insertCmd.Parameters.AddWithValue("@startDate", startDate);
                                        insertCmd.Parameters.AddWithValue("@endDate", endDate);
                                        insertCmd.Parameters.AddWithValue("@totalDays", totalDays);
                                        insertCmd.Parameters.AddWithValue("@overtimeHours", overtimeHours);
                                        insertCmd.Parameters.AddWithValue("@totalEarnings", totalEarnings);
                                        insertCmd.Parameters.AddWithValue("@restdayHours", restdayHours);
                                        insertCmd.Parameters.AddWithValue("@restdayOvertimeHours", restdayOvertimeHours);
                                        insertCmd.Parameters.AddWithValue("@legalHolidayHours", legalHolidayHours);
                                        insertCmd.Parameters.AddWithValue("@legalHolidayOvertimeHours", legalHolidayOvertimeHours);
                                        insertCmd.Parameters.AddWithValue("@lhrdHours", lhrdHours);
                                        insertCmd.Parameters.AddWithValue("@lhrdOvertimeHours", lhrdOvertimeHours);
                                        insertCmd.Parameters.AddWithValue("@specialHolidayHours", specialHolidayHours);
                                        insertCmd.Parameters.AddWithValue("@specialHolidayOvertimeHours", specialHolidayOvertimeHours);
                                        insertCmd.Parameters.AddWithValue("@specialHolidayRestDayHours", specialHolidayRestDayHours);
                                        insertCmd.Parameters.AddWithValue("@specialHolidayRestDayOvertimeHours", specialHolidayRestDayOvertimeHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialHours", nightDifferentialHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialOtHours", nightDifferentialOtHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialRestDayHours", nightDifferentialRestDayHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialRestDayOtHours", nightDifferentialRestDayOtHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayHours", nightDifferentialSpecialHolidayHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayOtHours", nightDifferentialSpecialHolidayOtHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayRestDayHours", nightDifferentialSpecialHolidayRestDayHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialSpecialHolidayRestDayOtHours", nightDifferentialSpecialHolidayRestDayOtHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayHours", nightDifferentialLegalHolidayHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayOtHours", nightDifferentialLegalHolidayOtHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayRestDayHours", nightDifferentialLegalHolidayRestDayHours);
                                        insertCmd.Parameters.AddWithValue("@nightDifferentialLegalHolidayRestDayOtHours", nightDifferentialLegalHolidayRestDayOtHours);
                                        insertCmd.Parameters.AddWithValue("@month", month);
                                        insertCmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                                        insertCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                                        insertCmd.Parameters.AddWithValue("@totalTardinessUndertime", totalTardinessUndertime);
                                        insertCmd.Parameters.AddWithValue("@totalWorkingHours", totalWorkingHours);
                                        insertCmd.Parameters.AddWithValue("@legalHolidayCount", legalHolidayCount);
                                        insertCmd.Parameters.AddWithValue("@nonWorkingDayCount", nonWorkingDayCount);
                                        insertCmd.Parameters.AddWithValue("@rate", rate);
                                        insertCmd.Parameters.AddWithValue("@reliever", isReliever);
                                        insertCmd.Parameters.AddWithValue("@sss", 0); // Initial value, will be updated by CalculatePayrollAmounts
                                        insertCmd.Parameters.AddWithValue("@philhealth", 0); // Initial value, will be updated by CalculatePayrollAmounts
                                        insertCmd.Parameters.AddWithValue("@hdmf", 0); // Initial value, will be updated by CalculatePayrollAmounts

                                        insertCmd.ExecuteNonQuery();
                                    }
                                }

                                // Now calculate and update the pay amounts based on the rate configuration
                                CalculatePayrollAmounts(employeeID, startDate, endDate, rate, isReliever);
                            }
                        }
                    }

                    MessageBox.Show("Payroll data saved and calculated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating and saving payroll: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CalculatePayrollAmounts(string employeeID, DateTime startDate, DateTime endDate, decimal rate, bool isReliever)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // First, get the payroll record
                    string payrollQuery = @"
                SELECT * FROM payroll 
                WHERE employee_id = @employeeID 
                AND pay_period_start = @startDate 
                AND pay_period_end = @endDate
                AND rate = @rate
                AND reliever = @reliever";

                    MySqlCommand payrollCmd = new MySqlCommand(payrollQuery, conn);
                    payrollCmd.Parameters.AddWithValue("@employeeID", employeeID);
                    payrollCmd.Parameters.AddWithValue("@startDate", startDate);
                    payrollCmd.Parameters.AddWithValue("@endDate", endDate);
                    payrollCmd.Parameters.AddWithValue("@rate", rate);
                    payrollCmd.Parameters.AddWithValue("@reliever", isReliever);

                    // Get the rate configuration for this rate
                    string rateQuery = @"
                SELECT * FROM rate 
                WHERE defaultrate = @rate 
                ORDER BY id DESC LIMIT 1";

                    MySqlCommand rateCmd = new MySqlCommand(rateQuery, conn);
                    rateCmd.Parameters.AddWithValue("@rate", rate);

                    decimal basicpay = 0, rdpay = 0, rdotpay = 0, lhpay = 0, regotpay = 0, trdypay = 0;
                    decimal lhhrspay = 0, lhothrspay = 0, lhrdpay = 0, lhrdotpay = 0;
                    decimal shpay = 0, shotpay = 0, shrdpay = 0, shrdotpay = 0;
                    decimal ndpay = 0, ndotpay = 0, ndrdpay = 0, ndshpay = 0, ndshrdpay = 0;
                    decimal ndlhpay = 0, ndlhrdpay = 0;
                    decimal totalBasicPay = 0, totalOTPay = 0, grossPay = 0;
                    decimal sss = 0, philhealth = 0, hdmf = 0;
                    int controlPeriod = 0;
                    int month = 0, payrollyear = 0;

                    using (MySqlDataReader rateReader = rateCmd.ExecuteReader())
                    {
                        if (rateReader.Read())
                        {
                            // Store rate values
                            decimal basic = rateReader.GetDecimal("basic");
                            decimal rd = rateReader.GetDecimal("rd");
                            decimal rdot = rateReader.GetDecimal("rdot");
                            decimal lh = rateReader.GetDecimal("lh");
                            decimal regot = rateReader.GetDecimal("regot");
                            decimal trdy = rateReader.GetDecimal("trdy");
                            decimal lhhrs = rateReader.GetDecimal("lhhrs");
                            decimal lhothrs = rateReader.GetDecimal("lhothrs");
                            decimal lhrd = rateReader.GetDecimal("lhrd");
                            decimal lhrdot = rateReader.GetDecimal("lhrdot");
                            decimal sh = rateReader.GetDecimal("sh");
                            decimal shot = rateReader.GetDecimal("shot");
                            decimal shrd = rateReader.GetDecimal("shrd");
                            decimal shrdot = rateReader.GetDecimal("shrdot");
                            decimal nd = rateReader.GetDecimal("nd");
                            decimal ndot = rateReader.GetDecimal("ndot");
                            decimal ndrd = rateReader.GetDecimal("ndrd");
                            decimal ndsh = rateReader.GetDecimal("ndsh");
                            decimal ndshrd = rateReader.GetDecimal("ndshrd");
                            decimal ndlh = rateReader.GetDecimal("ndlh");
                            decimal ndlhrd = rateReader.GetDecimal("ndlhrd");

                            // Close the rate reader before opening the payroll reader
                            rateReader.Close();

                            using (MySqlDataReader payrollReader = payrollCmd.ExecuteReader())
                            {
                                if (payrollReader.Read())
                                {
                                    // Get payroll values
                                    decimal total_days = payrollReader.GetDecimal("total_days");
                                    decimal restday_hours = payrollReader.GetDecimal("restday_hours");
                                    decimal restday_overtime_hours = payrollReader.GetDecimal("restday_overtime_hours");
                                    int legal_holiday_count = payrollReader.GetInt32("legal_holiday_count");
                                    decimal overtime_hours = payrollReader.GetDecimal("overtime_hours");
                                    decimal td_ut = payrollReader.GetDecimal("td_ut");
                                    decimal legal_holiday_hours = payrollReader.GetDecimal("legal_holiday_hours");
                                    decimal legal_holiday_overtime_hours = payrollReader.GetDecimal("legal_holiday_overtime_hours");
                                    decimal lhrd_hours = payrollReader.GetDecimal("lhrd_hours");
                                    decimal lhrd_overtime_hours = payrollReader.GetDecimal("lhrd_overtime_hours");
                                    decimal special_holiday_hours = payrollReader.GetDecimal("special_holiday_hours");
                                    decimal special_holiday_overtime_hours = payrollReader.GetDecimal("special_holiday_overtime_hours");
                                    decimal special_holiday_restday_hours = payrollReader.GetDecimal("special_holiday_restday_hours");
                                    decimal special_holiday_restday_overtime_hours = payrollReader.GetDecimal("special_holiday_restday_overtime_hours");
                                    decimal nd_hrs = payrollReader.GetDecimal("nd_hrs");
                                    decimal ndot_hrs = payrollReader.GetDecimal("ndot_hrs");
                                    decimal ndrd_hrs = payrollReader.GetDecimal("ndrd_hrs");
                                    decimal ndsh_hrs = payrollReader.GetDecimal("ndsh_hrs");
                                    decimal ndshrd_hrs = payrollReader.GetDecimal("ndshrd_hrs");
                                    decimal ndlh_hrs = payrollReader.GetDecimal("ndlh_hrs");
                                    decimal ndlhrd_hrs = payrollReader.GetDecimal("ndlhrd_hrs");
                                    controlPeriod = payrollReader.GetInt32("control_period");
                                    month = payrollReader.GetInt32("month");
                                    payrollyear = payrollReader.GetInt32("payrollyear");

                                    // Calculate pay amounts
                                    basicpay = total_days * basic;
                                    rdpay = restday_hours * rd;
                                    rdotpay = rdot * restday_overtime_hours;
                                    lhpay = lh * legal_holiday_count;
                                    regotpay = regot * overtime_hours;
                                    trdypay = trdy * td_ut;
                                    lhhrspay = lhhrs * legal_holiday_hours;
                                    lhothrspay = lhothrs * legal_holiday_overtime_hours;
                                    lhrdpay = lhrd * lhrd_hours;
                                    lhrdotpay = lhrdot * lhrd_overtime_hours;
                                    shpay = sh * special_holiday_hours;
                                    shotpay = shot * special_holiday_overtime_hours;
                                    shrdpay = shrd * special_holiday_restday_hours;
                                    shrdotpay = shrdot * special_holiday_restday_overtime_hours;
                                    ndpay = nd * nd_hrs;
                                    ndotpay = ndot * ndot_hrs;
                                    ndrdpay = ndrd * ndrd_hrs;
                                    ndshpay = ndsh * ndsh_hrs;
                                    ndshrdpay = ndshrd * ndshrd_hrs;
                                    ndlhpay = ndlh * ndlh_hrs;
                                    ndlhrdpay = ndlhrd * ndlhrd_hrs;

                                    // Calculate total pay components as specified
                                    totalBasicPay = basicpay + trdypay + lhpay;

                                    totalOTPay = rdpay + rdotpay + regotpay + lhhrspay + lhothrspay +
                                                lhrdpay + lhrdotpay + shpay + shotpay + shrdpay + shrdotpay +
                                                ndpay + ndotpay + ndrdpay + ndshpay + ndshrdpay + ndlhpay + ndlhrdpay;

                                    grossPay = totalBasicPay + totalOTPay;

                                    // Calculate SSS based on the gross pay only if not a reliever
                                    if (!isReliever)
                                    {
                                        sss = CalculateSSS(grossPay);
                                    }

                                    // Calculate PhilHealth based on the rate and control period only if not a reliever
                                    if (controlPeriod == 1 && !isReliever)
                                    {
                                        philhealth = rate * 313 / 12 * 0.05m / 2;
                                    }
                                    else
                                    {
                                        philhealth = 0; // Set to zero for relievers or when control period is not 1
                                    }

                                    // Calculate HDMF (Pag-IBIG)
                                    if (!isReliever)
                                    {
                                        if (controlPeriod == 1)
                                        {
                                            // For the first half, use the formula
                                            hdmf = totalBasicPay >= 10000 ? 200 : (totalBasicPay * 0.02m);
                                        }
                                        else if (controlPeriod == 2)
                                        {
                                            // For the second half, calculate the difference
                                            hdmf = CalculateSecondHalfHDMF(employeeID, month, payrollyear, totalBasicPay);
                                        }
                                    }
                                }
                            }

                            // Update the payroll record with the calculated amounts
                            string updateQuery = @"
                        UPDATE payroll SET
                            basicpay = @basicpay,
                            rdpay = @rdpay,
                            rdotpay = @rdotpay, 
                            lhpay = @lhpay,
                            regotpay = @regotpay,
                            trdypay = @trdypay,
                            lhhrspay = @lhhrspay,
                            lhothrspay = @lhothrspay,
                            lhrdpay = @lhrdpay,
                            lhrdotpay = @lhrdotpay,
                            shpay = @shpay,
                            shotpay = @shotpay,
                            shrdpay = @shrdpay,
                            shrdotpay = @shrdotpay,
                            ndpay = @ndpay,
                            ndotpay = @ndotpay,
                            ndrdpay = @ndrdpay,
                            ndshpay = @ndshpay,
                            ndshrdpay = @ndshrdpay,
                            ndlhpay = @ndlhpay,
                            ndlhrdpay = @ndlhrdpay,
                            total_basic_pay = @totalBasicPay,
                            total_ot_pay = @totalOTPay,
                            gross_pay = @grossPay,
                            SSS = @sss,
                            philhealth = @philhealth,
                            hdmf = @hdmf
                        WHERE employee_id = @employeeID 
                        AND pay_period_start = @startDate 
                        AND pay_period_end = @endDate
                        AND rate = @rate
                        AND reliever = @reliever";

                            MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                            updateCmd.Parameters.AddWithValue("@basicpay", basicpay);
                            updateCmd.Parameters.AddWithValue("@rdpay", rdpay);
                            updateCmd.Parameters.AddWithValue("@rdotpay", rdotpay);
                            updateCmd.Parameters.AddWithValue("@lhpay", lhpay);
                            updateCmd.Parameters.AddWithValue("@regotpay", regotpay);
                            updateCmd.Parameters.AddWithValue("@trdypay", trdypay);
                            updateCmd.Parameters.AddWithValue("@lhhrspay", lhhrspay);
                            updateCmd.Parameters.AddWithValue("@lhothrspay", lhothrspay);
                            updateCmd.Parameters.AddWithValue("@lhrdpay", lhrdpay);
                            updateCmd.Parameters.AddWithValue("@lhrdotpay", lhrdotpay);
                            updateCmd.Parameters.AddWithValue("@shpay", shpay);
                            updateCmd.Parameters.AddWithValue("@shotpay", shotpay);
                            updateCmd.Parameters.AddWithValue("@shrdpay", shrdpay);
                            updateCmd.Parameters.AddWithValue("@shrdotpay", shrdotpay);
                            updateCmd.Parameters.AddWithValue("@ndpay", ndpay);
                            updateCmd.Parameters.AddWithValue("@ndotpay", ndotpay);
                            updateCmd.Parameters.AddWithValue("@ndrdpay", ndrdpay);
                            updateCmd.Parameters.AddWithValue("@ndshpay", ndshpay);
                            updateCmd.Parameters.AddWithValue("@ndshrdpay", ndshrdpay);
                            updateCmd.Parameters.AddWithValue("@ndlhpay", ndlhpay);
                            updateCmd.Parameters.AddWithValue("@ndlhrdpay", ndlhrdpay);
                            updateCmd.Parameters.AddWithValue("@totalBasicPay", totalBasicPay);
                            updateCmd.Parameters.AddWithValue("@totalOTPay", totalOTPay);
                            updateCmd.Parameters.AddWithValue("@grossPay", grossPay);
                            updateCmd.Parameters.AddWithValue("@sss", sss);
                            updateCmd.Parameters.AddWithValue("@philhealth", philhealth);
                            updateCmd.Parameters.AddWithValue("@hdmf", hdmf);
                            updateCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            updateCmd.Parameters.AddWithValue("@startDate", startDate);
                            updateCmd.Parameters.AddWithValue("@endDate", endDate);
                            updateCmd.Parameters.AddWithValue("@rate", rate);
                            updateCmd.Parameters.AddWithValue("@reliever", isReliever);

                            updateCmd.ExecuteNonQuery();

                            // Sum gross pay for reliever and non-reliever days
                            string sumGrossPayQuery = @"
                        SELECT SUM(gross_pay) AS total_gross_pay
                        FROM payroll
                        WHERE employee_id = @employeeID
                        AND pay_period_start = @startDate
                        AND pay_period_end = @endDate";

                            MySqlCommand sumGrossPayCmd = new MySqlCommand(sumGrossPayQuery, conn);
                            sumGrossPayCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            sumGrossPayCmd.Parameters.AddWithValue("@startDate", startDate);
                            sumGrossPayCmd.Parameters.AddWithValue("@endDate", endDate);

                            decimal totalGrossPay = Convert.ToDecimal(sumGrossPayCmd.ExecuteScalar());

                            // Update netpay for non-reliever day
                            string updateNetPayQuery = @"
                        UPDATE payroll
                        SET netpay = @totalGrossPay
                        WHERE employee_id = @employeeID
                        AND pay_period_start = @startDate
                        AND pay_period_end = @endDate
                        AND reliever = 0";

                            MySqlCommand updateNetPayCmd = new MySqlCommand(updateNetPayQuery, conn);
                            updateNetPayCmd.Parameters.AddWithValue("@totalGrossPay", totalGrossPay);
                            updateNetPayCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            updateNetPayCmd.Parameters.AddWithValue("@startDate", startDate);
                            updateNetPayCmd.Parameters.AddWithValue("@endDate", endDate);

                            updateNetPayCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MessageBox.Show($"No rate configuration found for rate {rate}. Please configure the rate first.",
                                "Missing Rate Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating payroll amounts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private decimal CalculateSecondHalfHDMF(string employeeID, int month, int payrollyear, decimal currentBasicPay)
        {
            decimal hdmf = 0;
            decimal firstHalfHDMF = 0;

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // First get the first half's HDMF amount
                    string firstHalfQuery = @"
                SELECT hdmf
                FROM payroll
                WHERE employee_id = @employeeID
                AND month = @month
                AND payrollyear = @payrollyear
                AND control_period = 1
                AND reliever = 0";

                    using (MySqlCommand cmd = new MySqlCommand(firstHalfQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@payrollyear", payrollyear);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                firstHalfHDMF = reader.GetDecimal("hdmf");

                                // Simple calculation: 2nd half hdmf = 200 - 1st half hdmf
                                hdmf = 200 - firstHalfHDMF;

                                // Ensure we don't have negative HDMF
                                if (hdmf < 0) hdmf = 0;
                            }
                            else
                            {
                                // If no first half record found, calculate based on current basic pay
                                hdmf = currentBasicPay >= 10000 ? 200 : (currentBasicPay * 0.02m);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating HDMF for second half: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return hdmf;
        }
        private decimal CalculateSSS(decimal grossPay)
        {
            decimal sss = 0;

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT salary1, salary2, EETotal FROM sssgovdues";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal salary1 = reader.GetDecimal("salary1");
                            decimal salary2 = reader.GetDecimal("salary2");
                            decimal eeTotal = reader.GetDecimal("EETotal");

                            if (grossPay >= salary1 && grossPay <= salary2)
                            {
                                sss = eeTotal;
                                break;
                            }
                        }
                    }
                }
            }

            return sss;
        }
        private void btnSavePayroll_Click_1(object sender, EventArgs e)
        {
            if (!DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate) ||
                !int.TryParse(payrollyear.Text, out int parsedPayrollYear) ||
                !int.TryParse(month.Text, out int parsedMonth) ||
                !int.TryParse(controlPeriod.Text, out int parsedControlPeriod))
            {
                MessageBox.Show("Invalid input. Please enter valid dates (MM/DD/YYYY) and numeric values for month, payroll year, and control period.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate that the year of fromdate and todate matches payrollyear
            if (startDate.Year != parsedPayrollYear || endDate.Year != parsedPayrollYear)
            {
                MessageBox.Show("The payroll year must match the year of the From Date and To Date.",
                    "Year Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalculateAndSavePayroll(startDate, endDate, parsedMonth, parsedPayrollYear, parsedControlPeriod);
        }
        private void repost_CheckedChanged(object sender, EventArgs e)
        {
            allowRepost = repost.Checked;
        }
        private void AutoFormatDate(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Allow only digits and control keys (e.g., backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // Auto-insert slashes based on the MM/DD/YYYY format
            if (!char.IsControl(e.KeyChar))
            {
                int length = textBox.Text.Length;

                if (length == 2 || length == 5)
                {
                    textBox.Text += "/";
                    textBox.SelectionStart = textBox.Text.Length; // Move the caret to the end
                }
            }
        }
    }
}
