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

        private void InitializeCustomComponents()
        {
            // Assuming fromDate and toDate are already added to the form
            fromdate.Enter += new EventHandler(RemoveHint);
            fromdate.Leave += new EventHandler(AddHint);
            todate.Enter += new EventHandler(RemoveHint);
            todate.Leave += new EventHandler(AddHint);

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
                        decimal totalWorkingHours = 0; // New variable for total working hours
                        int legalHolidayCount = 0; // New variable for counting legal holidays
                        int nonWorkingDayCount = 0; // New variable for counting non-working days

                        query = @"
            SELECT p.working_hours, p.ot_hrs, p.rate, s.regular_hours, p.rest_day, p.legal_holiday, p.special_holiday, p.non_working_day, p.nd_hrs, p.ndot_hrs, p.tardiness_undertime
            FROM processedDTR p
            JOIN ShiftCodes s ON p.shift_code = s.shift_code
            WHERE p.employee_id = @employeeID AND p.date BETWEEN @startDate AND @endDate";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeID", employeeID);
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.Parameters.AddWithValue("@endDate", endDate);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    decimal workingHours = reader.GetDecimal("working_hours");
                                    decimal otHours = reader.GetDecimal("ot_hrs");
                                    decimal rate = reader.GetDecimal("rate");
                                    decimal regularHours = reader.GetDecimal("regular_hours");
                                    bool isRestDay = reader.GetBoolean("rest_day");
                                    bool isLegalHoliday = reader.GetBoolean("legal_holiday");
                                    bool isSpecialHoliday = reader.GetBoolean("special_holiday");
                                    bool isNonWorkingDay = reader.GetBoolean("non_working_day");
                                    decimal ndHours = reader.GetDecimal("nd_hrs");
                                    decimal ndOtHours = reader.GetDecimal("ndot_hrs");
                                    decimal tardinessUndertime = reader.GetDecimal("tardiness_undertime"); // Fetch tardiness/undertime

                                    // Always add to total earnings
                                    totalEarnings += (workingHours + otHours) * rate;

                                    // Add tardiness/undertime to total
                                    totalTardinessUndertime += tardinessUndertime;

                                    // Add working hours to total
                                    totalWorkingHours += workingHours;

                                    if (isNonWorkingDay)
                                    {
                                        // Count non-working days
                                        nonWorkingDayCount++;
                                    }

                                    if (isRestDay && isLegalHoliday)
                                    {
                                        // Legal Holiday Rest Day
                                        lhrdHours += workingHours;
                                        lhrdOvertimeHours += otHours;
                                        nightDifferentialLegalHolidayRestDayHours += ndHours;
                                        nightDifferentialLegalHolidayRestDayOtHours += ndOtHours;
                                    }
                                    else if (isRestDay && isSpecialHoliday)
                                    {
                                        // Special Holiday Rest Day
                                        specialHolidayRestDayHours += workingHours;
                                        specialHolidayRestDayOvertimeHours += otHours;
                                        nightDifferentialSpecialHolidayRestDayHours += ndHours;
                                        nightDifferentialSpecialHolidayRestDayOtHours += ndOtHours;
                                    }
                                    else if (isRestDay)
                                    {
                                        // Rest Day
                                        restdayHours += workingHours;
                                        restdayOvertimeHours += otHours;
                                        nightDifferentialRestDayHours += ndHours;
                                        nightDifferentialRestDayOtHours += ndOtHours;
                                    }
                                    else if (isLegalHoliday)
                                    {
                                        // Legal Holiday
                                        legalHolidayHours += workingHours;
                                        legalHolidayOvertimeHours += otHours;
                                        nightDifferentialLegalHolidayHours += ndHours;
                                        nightDifferentialLegalHolidayOtHours += ndOtHours;
                                        legalHolidayCount++; // Increment legal holiday count
                                    }
                                    else if (isSpecialHoliday)
                                    {
                                        // Special Holiday
                                        specialHolidayHours += workingHours;
                                        specialHolidayOvertimeHours += otHours;
                                        nightDifferentialSpecialHolidayHours += ndHours;
                                        nightDifferentialSpecialHolidayOtHours += ndOtHours;
                                    }
                                    else
                                    {
                                        // Regular day (no special flags)
                                        totalHours += workingHours;
                                        overtimeHours += otHours;

                                        // For regular days, include ND hours in the total hours
                                        totalHours += ndHours;
                                        overtimeHours += ndOtHours;

                                        // Also track ND hours separately
                                        nightDifferentialHours += ndHours;
                                        nightDifferentialOtHours += ndOtHours;

                                        if (regularHours > 0)
                                        {
                                            totalDays += workingHours / regularHours;
                                        }
                                    }
                                }
                            }
                        }

                        // Check if payroll data already exists for the employee and pay period
                        string checkQuery = @"
            SELECT COUNT(*) 
            FROM payroll 
            WHERE employee_id = @employeeID AND pay_period_start = @startDate AND pay_period_end = @endDate";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
                            checkCmd.Parameters.AddWithValue("@startDate", startDate);
                            checkCmd.Parameters.AddWithValue("@endDate", endDate);

                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (count > 0)
                            {
                                // Record already exists, perform update
                                string updateQuery = @"
                    UPDATE payroll 
                    SET total_days = @totalDays, 
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
                    WHERE employee_id = @employeeID AND pay_period_start = @startDate AND pay_period_end = @endDate";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
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

                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Record does not exist, perform insert
                                string insertQuery = @"
                    INSERT INTO payroll (employee_id, pay_period_start, pay_period_end, total_days, overtime_hours, total_earnings, restday_hours, restday_overtime_hours, legal_holiday_hours, legal_holiday_overtime_hours, lhrd_hours, lhrd_overtime_hours, special_holiday_hours, special_holiday_overtime_hours, special_holiday_restday_hours, special_holiday_restday_overtime_hours, nd_hrs, ndot_hrs, ndrd_hrs, ndrdot_hrs, ndsh_hrs, ndshot_hrs, ndshrd_hrs, ndshrdot_hrs, ndlh_hrs, ndlhot_hrs, ndlhrd_hrs, ndlhrdot_hrs, month, payrollyear, control_period, td_ut, working_hours, legal_holiday_count, non_working_day_count)
                    VALUES (@employeeID, @startDate, @endDate, @totalDays, @overtimeHours, @totalEarnings, @restdayHours, @restdayOvertimeHours, @legalHolidayHours, @legalHolidayOvertimeHours, @lhrdHours, @lhrdOvertimeHours, @specialHolidayHours, @specialHolidayOvertimeHours, @specialHolidayRestDayHours, @specialHolidayRestDayOvertimeHours, @nightDifferentialHours, @nightDifferentialOtHours, @nightDifferentialRestDayHours, @nightDifferentialRestDayOtHours, @nightDifferentialSpecialHolidayHours, @nightDifferentialSpecialHolidayOtHours, @nightDifferentialSpecialHolidayRestDayHours, @nightDifferentialSpecialHolidayRestDayOtHours, @nightDifferentialLegalHolidayHours, @nightDifferentialLegalHolidayOtHours, @nightDifferentialLegalHolidayRestDayHours, @nightDifferentialLegalHolidayRestDayOtHours, @month, @payrollyear, @controlPeriod, @totalTardinessUndertime, @totalWorkingHours, @legalHolidayCount, @nonWorkingDayCount)";

                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@employeeID", employeeID);
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

                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    MessageBox.Show("Payroll data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating and saving payroll: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSavePayroll_Click_1(object sender, EventArgs e)
        {
            if (!DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate) ||
                !int.TryParse(month.Text, out int parsedMonth) ||
                !int.TryParse(payrollyear.Text, out int parsedPayrollYear) ||
                !int.TryParse(controlPeriod.Text, out int parsedControlPeriod))
            {
                MessageBox.Show("Invalid input. Please enter valid dates (MM/DD/YYYY) and numeric values for month, payroll year, and control period.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalculateAndSavePayroll(startDate, endDate, parsedMonth, parsedPayrollYear, parsedControlPeriod);
        }
    }
}
