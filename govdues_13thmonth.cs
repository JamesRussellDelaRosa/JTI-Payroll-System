using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class govdues_13thmonth : Form
    {
        public govdues_13thmonth()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

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

        private void AutoFormatDate(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (!char.IsControl(e.KeyChar))
            {
                int length = textBox.Text.Length;
                if (length == 2 || length == 5)
                {
                    textBox.Text += "/";
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        private void process_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(month_textbox.Text, out int month) ||
                !int.TryParse(controlperiod_textbox.Text, out int controlPeriod) ||
                !int.TryParse(payrollyear_textbox.Text, out int payrollYear) ||
                !DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fromDate) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime toDate))
            {
                MessageBox.Show("Invalid input. Please enter valid dates (MM/DD/YYYY) and numeric values for month, payroll year, and control period.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool reprocess = reprocessdata.Checked;
            ProcessGovDuesAnd13thMonthMerged(month, controlPeriod, payrollYear, fromDate, toDate, reprocess);
        }

        private void ProcessGovDuesAnd13thMonthMerged(int month, int controlPeriod, int payrollYear, DateTime fromDate, DateTime toDate, bool reprocess)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    if (!reprocess)
                    {
                        string checkExistingQuery = @"
                            SELECT COUNT(*) FROM payroll
                            WHERE month = @month
                            AND payrollyear = @payrollYear
                            AND control_period = @controlPeriod
                            AND pay_period_start = @fromDate
                            AND pay_period_end = @toDate
                            AND (basic != 0 OR gross != 0)";
                        using (MySqlCommand checkCmd = new MySqlCommand(checkExistingQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@month", month);
                            checkCmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                            checkCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                            checkCmd.Parameters.AddWithValue("@fromDate", fromDate);
                            checkCmd.Parameters.AddWithValue("@toDate", toDate);
                            long existingCount = (long)checkCmd.ExecuteScalar();
                            if (existingCount > 0)
                            {
                                MessageBox.Show("Data for this period has already been processed. To reprocess, please check the 'REPROCESS' box.", "Data Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                    if (reprocess)
                    {
                        string clearQuery = @"
                            UPDATE payroll 
                            SET basic = 0, gross = 0, SSS = 0, philhealth = 0, hdmf = 0
                            WHERE month = @month 
                            AND payrollyear = @payrollYear 
                            AND control_period = @controlPeriod
                            AND pay_period_start = @fromDate
                            AND pay_period_end = @toDate";
                        using (MySqlCommand clearCmd = new MySqlCommand(clearQuery, conn))
                        {
                            clearCmd.Parameters.AddWithValue("@month", month);
                            clearCmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                            clearCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                            clearCmd.Parameters.AddWithValue("@fromDate", fromDate);
                            clearCmd.Parameters.AddWithValue("@toDate", toDate);
                            clearCmd.ExecuteNonQuery();
                        }
                    }

                    string employeeIdQuery = @"
                        SELECT DISTINCT employee_id FROM payroll 
                        WHERE pay_period_start = @fromDate AND pay_period_end = @toDate AND month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod
                        UNION
                        SELECT DISTINCT employee_id FROM payroll_reliever 
                        WHERE pay_period_start = @fromDate AND pay_period_end = @toDate AND month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod
                        UNION
                        SELECT DISTINCT employee_id FROM payroll_adjustment 
                        WHERE pay_period_start = @fromDate AND pay_period_end = @toDate AND month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod";

                    List<string> employeeIds = new List<string>();
                    using (MySqlCommand empIdCmd = new MySqlCommand(employeeIdQuery, conn))
                    {
                        empIdCmd.Parameters.AddWithValue("@fromDate", fromDate);
                        empIdCmd.Parameters.AddWithValue("@toDate", toDate);
                        empIdCmd.Parameters.AddWithValue("@month", month);
                        empIdCmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                        empIdCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                        using (MySqlDataReader reader = empIdCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employeeIds.Add(reader.GetString("employee_id"));
                            }
                        }
                    }

                    foreach (string empId in employeeIds)
                    {
                        decimal totalBasicPaySum = 0;
                        decimal totalGrossPaySum = 0;

                        string payrollQuery = @"
                            SELECT SUM(total_basic_pay) as sum_basic, SUM(gross_pay) as sum_gross 
                            FROM payroll 
                            WHERE employee_id = @employeeId 
                            AND pay_period_start = @fromDate AND pay_period_end = @toDate 
                            AND month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod";
                        using (MySqlCommand cmd = new MySqlCommand(payrollQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeId", empId);
                            cmd.Parameters.AddWithValue("@fromDate", fromDate);
                            cmd.Parameters.AddWithValue("@toDate", toDate);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                            cmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal("sum_basic")))
                                        totalBasicPaySum += reader.GetDecimal("sum_basic");
                                    if (!reader.IsDBNull(reader.GetOrdinal("sum_gross")))
                                        totalGrossPaySum += reader.GetDecimal("sum_gross");
                                }
                            }
                        }

                        string relieverQuery = @"
                            SELECT SUM(total_basic_pay) as sum_basic, SUM(gross_pay) as sum_gross 
                            FROM payroll_reliever 
                            WHERE employee_id = @employeeId 
                            AND pay_period_start = @fromDate AND pay_period_end = @toDate 
                            AND month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod";
                        using (MySqlCommand cmd = new MySqlCommand(relieverQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeId", empId);
                            cmd.Parameters.AddWithValue("@fromDate", fromDate);
                            cmd.Parameters.AddWithValue("@toDate", toDate);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                            cmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal("sum_basic")))
                                        totalBasicPaySum += reader.GetDecimal("sum_basic");
                                    if (!reader.IsDBNull(reader.GetOrdinal("sum_gross")))
                                        totalGrossPaySum += reader.GetDecimal("sum_gross");
                                }
                            }
                        }

                        string adjustmentQuery = @"
                            SELECT SUM(total_basic_pay) as sum_basic, SUM(gross_pay) as sum_gross 
                            FROM payroll_adjustment 
                            WHERE employee_id = @employeeId 
                            AND pay_period_start = @fromDate AND pay_period_end = @toDate 
                            AND month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod";
                        using (MySqlCommand cmd = new MySqlCommand(adjustmentQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeId", empId);
                            cmd.Parameters.AddWithValue("@fromDate", fromDate);
                            cmd.Parameters.AddWithValue("@toDate", toDate);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                            cmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal("sum_basic")))
                                        totalBasicPaySum += reader.GetDecimal("sum_basic");
                                    if (!reader.IsDBNull(reader.GetOrdinal("sum_gross")))
                                        totalGrossPaySum += reader.GetDecimal("sum_gross");
                                }
                            }
                        }

                        string selectPayrollQuery = @"
                            SELECT id, rate, reliever, SSS, philhealth, hdmf
                            FROM payroll
                            WHERE employee_id = @employeeId
                            AND month = @month
                            AND payrollyear = @payrollYear
                            AND control_period = @controlPeriod
                            AND pay_period_start = @fromDate
                            AND pay_period_end = @toDate";
                        int payrollId = -1;
                        decimal rate = 0;
                        bool reliever = false;
                        decimal existingSSS = 0, existingPhilhealth = 0, existingHDMF = 0;
                        using (MySqlCommand selectCmd = new MySqlCommand(selectPayrollQuery, conn))
                        {
                            selectCmd.Parameters.AddWithValue("@employeeId", empId);
                            selectCmd.Parameters.AddWithValue("@month", month);
                            selectCmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                            selectCmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                            selectCmd.Parameters.AddWithValue("@fromDate", fromDate);
                            selectCmd.Parameters.AddWithValue("@toDate", toDate);
                            using (var reader = selectCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    payrollId = reader.GetInt32("id");
                                    rate = reader.GetDecimal("rate");
                                    reliever = reader.GetBoolean("reliever");
                                    existingSSS = reader.IsDBNull(reader.GetOrdinal("SSS")) ? 0m : reader.GetDecimal("SSS");
                                    existingPhilhealth = reader.IsDBNull(reader.GetOrdinal("philhealth")) ? 0m : reader.GetDecimal("philhealth");
                                    existingHDMF = reader.IsDBNull(reader.GetOrdinal("hdmf")) ? 0m : reader.GetDecimal("hdmf");
                                }
                            }
                        }

                        if (payrollId != -1 && (reprocess || (existingSSS == 0m && existingPhilhealth == 0m && existingHDMF == 0m)))
                        {
                            decimal sss = CalculateSSS(totalGrossPaySum, reliever);
                            decimal philhealth = CalculatePhilHealth(rate, controlPeriod, reliever);
                            decimal hdmf = CalculateHDMF(totalBasicPaySum, controlPeriod, reliever, empId, month, payrollYear);

                            string updateQuery = @"
                                UPDATE payroll 
                                SET basic = @totalBasicPay, 
                                    gross = @totalGrossPay,
                                    SSS = @sss,
                                    philhealth = @philhealth,
                                    hdmf = @hdmf
                                WHERE id = @id";
                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@totalBasicPay", totalBasicPaySum);
                                updateCmd.Parameters.AddWithValue("@totalGrossPay", totalGrossPaySum);
                                updateCmd.Parameters.AddWithValue("@sss", sss);
                                updateCmd.Parameters.AddWithValue("@philhealth", philhealth);
                                updateCmd.Parameters.AddWithValue("@hdmf", hdmf);
                                updateCmd.Parameters.AddWithValue("@id", payrollId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show("Government Dues and 13th Month data processed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal CalculateSSS(decimal grossPay, bool isReliever)
        {
            if (isReliever)
            {
                return 0;
            }

            decimal sss = 0;
            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT salary1, salary2, EETotal FROM sssgovdues";
                using (MySqlCommand command = new MySqlCommand(query, conn))
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
            return sss;
        }

        private decimal CalculatePhilHealth(decimal rate, int controlPeriod, bool isReliever)
        {
            if (isReliever)
            {
                return 0;
            }
            if (controlPeriod == 1)
            {
                return rate * 313 / 12 * 0.05m / 2;
            }
            return 0;
        }

        private decimal CalculateHDMF(decimal totalBasicPay, int controlPeriod, bool isReliever, string employeeID, int month, int payrollyear)
        {
            if (isReliever)
            {
                return 0;
            }

            if (controlPeriod == 1)
            {
                return totalBasicPay >= 10000 ? 200 : (totalBasicPay * 0.02m);
            }
            else if (controlPeriod == 2)
            {
                return CalculateSecondHalfHDMF(employeeID, month, payrollyear, totalBasicPay);
            }
            return 0;
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
                                hdmf = 200 - firstHalfHDMF;
                                if (hdmf < 0) hdmf = 0;
                            }
                            else
                            {
                                hdmf = currentBasicPay >= 10000 ? 200 : (currentBasicPay * 0.02m);
                            }
                        }
                    }
                }
            }
            catch
            {
                hdmf = currentBasicPay >= 10000 ? 200 : (currentBasicPay * 0.02m);
            }
            return hdmf;
        }
    }
}
