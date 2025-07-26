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
using System.Diagnostics;
using System.IO;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Controls.WinForms;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using JTI_Payroll_System;

namespace JTI_Payroll_System
{
    public partial class generatepayslip : Form
    {
        public generatepayslip()
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
                textBox.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void AddHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "MM/DD/YYYY";
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
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

        private void GeneratePayslipPdf(PayslipViewModel payslip)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"Payslip_{payslip.EmployeeId}_{Guid.NewGuid()}.pdf");
            var document = new PayslipDocument(payslip);
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                document.GeneratePdf(fs);
            }
            WaitForFileRelease(tempFile);
            // Prompt user to save the PDF
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf", FileName = Path.GetFileName(tempFile) })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(tempFile, sfd.FileName, true);
                    MessageBox.Show("PDF saved successfully.", "Save As", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Optionally open the PDF
                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
            }
        }

        // Generate a single PDF for multiple payslips using QuestPDF
        private void GeneratePayslipsPdf(List<PayslipViewModel> payslips)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"Payslips_{Guid.NewGuid()}.pdf");
            var document = new MultiPayslipDocument(payslips);
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                document.GeneratePdf(fs);
            }
            WaitForFileRelease(tempFile);
            // Prompt user to save the PDF
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf", FileName = Path.GetFileName(tempFile) })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(tempFile, sfd.FileName, true);
                    MessageBox.Show("PDF saved successfully.", "Save As", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Optionally open the PDF
                    try { Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true }); } catch { }
                }
            }
        }

        // Helper to wait for file to be released
        private void WaitForFileRelease(string filePath, int maxWaitMs = 1000)
        {
            int waited = 0;
            while (waited < maxWaitMs)
            {
                try
                {
                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        break;
                    }
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(50);
                    waited += 50;
                }
            }
        }

        private void UpdateNetPay(int month, int controlPeriod, int payrollYear, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string selectQuery = @"SELECT id, total_grosspay, sil, perfect_attendance, adjustment_total, reliever_total FROM payroll WHERE month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod AND pay_period_start = @fromDate AND pay_period_end = @toDate";
                    using (var cmd = new MySqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                        cmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var updates = new List<(int id, decimal netpay)>();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                decimal totalGrossPay = reader.IsDBNull(reader.GetOrdinal("total_grosspay")) ? 0 : reader.GetDecimal("total_grosspay");
                                decimal sil = reader.IsDBNull(reader.GetOrdinal("sil")) ? 0 : reader.GetDecimal("sil");
                                decimal perfectAttendance = reader.IsDBNull(reader.GetOrdinal("perfect_attendance")) ? 0 : reader.GetDecimal("perfect_attendance");
                                decimal adjustmentTotal = reader.IsDBNull(reader.GetOrdinal("adjustment_total")) ? 0 : reader.GetDecimal("adjustment_total");
                                decimal relieverTotal = reader.IsDBNull(reader.GetOrdinal("reliever_total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("reliever_total"));
                                decimal netpay = totalGrossPay + sil + perfectAttendance + adjustmentTotal + relieverTotal;
                                netpay = Math.Round(netpay, 2, MidpointRounding.AwayFromZero); // Round to 2 decimal places
                                updates.Add((id, netpay));
                            }
                            reader.Close();
                            foreach (var upd in updates)
                            {
                                string updateQuery = "UPDATE payroll SET netpay = @netpay WHERE id = @id";
                                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@netpay", upd.netpay);
                                    updateCmd.Parameters.AddWithValue("@id", upd.id);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating netpay: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<PayslipViewModel> GetPayslipsForPeriod(int month, int controlPeriod, int payrollYear, DateTime fromDate, DateTime toDate)
        {
            var payslips = new List<PayslipViewModel>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string selectQuery = @"SELECT p.*, e.ccode AS emp_ccode, e.client AS emp_client, e.bir_stat AS emp_bir_stat, e.atm_card_no AS emp_atm_card_no FROM payroll p LEFT JOIN employee e ON p.employee_id = e.id_no WHERE p.month = @month AND p.payrollyear = @payrollYear AND p.control_period = @controlPeriod AND p.pay_period_start = @fromDate AND p.pay_period_end = @toDate";
                    using (var cmd = new MySqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@payrollYear", payrollYear);
                        cmd.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var payslip = new PayslipViewModel
                                {
                                    EmployeeId = reader["employee_id"].ToString(),
                                    EmployeeName = $"{reader["lname"]}, {reader["fname"]} {reader["mname"]}",
                                    Department = reader["emp_ccode"]?.ToString() ?? "",
                                    Client = reader["emp_client"]?.ToString() ?? "",
                                    BirStat = reader["emp_bir_stat"]?.ToString() ?? "",
                                    AtmCardNo = reader["emp_atm_card_no"]?.ToString() ?? "",
                                    PeriodStart = reader.GetDateTime(reader.GetOrdinal("pay_period_start")),
                                    PeriodEnd = reader.GetDateTime(reader.GetOrdinal("pay_period_end")),
                                    RatePerDay = reader.IsDBNull(reader.GetOrdinal("rate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("rate")),
                                    BasicPay = reader.IsDBNull(reader.GetOrdinal("basicpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("basicpay")),
                                    LegalHolidayPay = reader.IsDBNull(reader.GetOrdinal("lhpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("lhpay")),
                                    TardyUndertimePay = reader.IsDBNull(reader.GetOrdinal("trdypay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("trdypay")),
                                    OvertimePay = reader.IsDBNull(reader.GetOrdinal("total_ot_pay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_ot_pay")),
                                    NightDifferentialPay = reader.IsDBNull(reader.GetOrdinal("ndpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndpay")),
                                    GrossPay = reader.IsDBNull(reader.GetOrdinal("gross_pay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("gross_pay")),
                                    SSS = reader.IsDBNull(reader.GetOrdinal("SSS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("SSS")),
                                    PhilHealth = reader.IsDBNull(reader.GetOrdinal("philhealth")) ? 0 : reader.GetDecimal(reader.GetOrdinal("philhealth")),
                                    HDMF = reader.IsDBNull(reader.GetOrdinal("hdmf")) ? 0 : reader.GetDecimal(reader.GetOrdinal("hdmf")),
                                    HMO = reader.IsDBNull(reader.GetOrdinal("hmo")) ? 0 : reader.GetDecimal(reader.GetOrdinal("hmo")),
                                    TotalDeductions = reader.IsDBNull(reader.GetOrdinal("total_deductions")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_deductions")),
                                    SIL = reader.IsDBNull(reader.GetOrdinal("sil")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sil")),
                                    PerfectAttendance = reader.IsDBNull(reader.GetOrdinal("perfect_attendance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("perfect_attendance")),
                                    Adjustment = reader.IsDBNull(reader.GetOrdinal("adjustment_total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("adjustment_total")),
                                    Reliever = reader.IsDBNull(reader.GetOrdinal("reliever_total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("reliever_total")),
                                    NetPay = reader.IsDBNull(reader.GetOrdinal("netpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("netpay")),
                                    CashAdvance = reader.IsDBNull(reader.GetOrdinal("cash_advance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cash_advance")),
                                    Uniform = reader.IsDBNull(reader.GetOrdinal("uniform")) ? 0 : reader.GetDecimal(reader.GetOrdinal("uniform")),
                                    AtmId = reader.IsDBNull(reader.GetOrdinal("atm_id")) ? 0 : reader.GetDecimal(reader.GetOrdinal("atm_id")),
                                    Medical = reader.IsDBNull(reader.GetOrdinal("medical")) ? 0 : reader.GetDecimal(reader.GetOrdinal("medical")),
                                    Grocery = reader.IsDBNull(reader.GetOrdinal("grocery")) ? 0 : reader.GetDecimal(reader.GetOrdinal("grocery")),
                                    Canteen = reader.IsDBNull(reader.GetOrdinal("canteen")) ? 0 : reader.GetDecimal(reader.GetOrdinal("canteen")),
                                    Damayan = reader.IsDBNull(reader.GetOrdinal("damayan")) ? 0 : reader.GetDecimal(reader.GetOrdinal("damayan")),
                                    Rice = reader.IsDBNull(reader.GetOrdinal("rice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("rice")),
                                    TotalDays = reader.IsDBNull(reader.GetOrdinal("total_days")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_days")),
                                    LegalHolidayCount = reader.IsDBNull(reader.GetOrdinal("legal_holiday_count")) ? 0 : reader.GetDecimal(reader.GetOrdinal("legal_holiday_count")),
                                    OvertimeHours = reader.IsDBNull(reader.GetOrdinal("overtime_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("overtime_hours")),
                                    RestdayHours = reader.IsDBNull(reader.GetOrdinal("restday_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("restday_hours")),
                                    RestdayOvertimeHours = reader.IsDBNull(reader.GetOrdinal("restday_overtime_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("restday_overtime_hours")),
                                    LegalHolidayHours = reader.IsDBNull(reader.GetOrdinal("legal_holiday_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("legal_holiday_hours")),
                                    LegalHolidayOvertimeHours = reader.IsDBNull(reader.GetOrdinal("legal_holiday_overtime_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("legal_holiday_overtime_hours")),
                                    LegalHolidayRestdayHours = reader.IsDBNull(reader.GetOrdinal("lhrd_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("lhrd_hours")),
                                    LegalHolidayRestdayOvertimeHours = reader.IsDBNull(reader.GetOrdinal("lhrd_overtime_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("lhrd_overtime_hours")),
                                    SpecialHolidayHours = reader.IsDBNull(reader.GetOrdinal("special_holiday_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("special_holiday_hours")),
                                    SpecialHolidayOvertimeHours = reader.IsDBNull(reader.GetOrdinal("special_holiday_overtime_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("special_holiday_overtime_hours")),
                                    SpecialHolidayRestdayHours = reader.IsDBNull(reader.GetOrdinal("special_holiday_restday_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("special_holiday_restday_hours")),
                                    SpecialHolidayRestdayOvertimeHours = reader.IsDBNull(reader.GetOrdinal("special_holiday_restday_overtime_hours")) ? 0 : reader.GetDecimal(reader.GetOrdinal("special_holiday_restday_overtime_hours")),
                                    NightDifferentialHours = reader.IsDBNull(reader.GetOrdinal("nd_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("nd_hrs")),
                                    NightDifferentialOvertimeHours = reader.IsDBNull(reader.GetOrdinal("ndot_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndot_hrs")),
                                    NightDifferentialRestdayHours = reader.IsDBNull(reader.GetOrdinal("ndrd_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndrd_hrs")),
                                    NightDifferentialSpecialHolidayHours = reader.IsDBNull(reader.GetOrdinal("ndsh_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndsh_hrs")),
                                    NightDifferentialSpecialHolidayRestdayHours = reader.IsDBNull(reader.GetOrdinal("ndshrd_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndshrd_hrs")),
                                    NightDifferentialLegalHolidayHours = reader.IsDBNull(reader.GetOrdinal("ndlh_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndlh_hrs")),
                                    NightDifferentialLegalHolidayRestdayHours = reader.IsDBNull(reader.GetOrdinal("ndlhrd_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndlhrd_hrs")),
                                    RestdayPay = reader.IsDBNull(reader.GetOrdinal("rdpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("rdpay")),
                                    RestdayOvertimePay = reader.IsDBNull(reader.GetOrdinal("rdotpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("rdotpay")),
                                    LegalHolidayOvertimePay = reader.IsDBNull(reader.GetOrdinal("lhothrspay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("lhothrspay")),
                                    LegalHolidayRestdayPay = reader.IsDBNull(reader.GetOrdinal("lhrdpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("lhrdpay")),
                                    LegalHolidayRestdayOvertimePay = reader.IsDBNull(reader.GetOrdinal("lhrdotpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("lhrdotpay")),
                                    SpecialHolidayPay = reader.IsDBNull(reader.GetOrdinal("shpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("shpay")),
                                    SpecialHolidayOvertimePay = reader.IsDBNull(reader.GetOrdinal("shotpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("shotpay")),
                                    SpecialHolidayRestdayPay = reader.IsDBNull(reader.GetOrdinal("shrdpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("shrdpay")),
                                    SpecialHolidayRestdayOvertimePay = reader.IsDBNull(reader.GetOrdinal("shrdotpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("shrdotpay")),
                                    NightDifferentialOvertimePay = reader.IsDBNull(reader.GetOrdinal("ndotpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndotpay")),
                                    NightDifferentialRestdayPay = reader.IsDBNull(reader.GetOrdinal("ndrdpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndrdpay")),
                                    NightDifferentialSpecialHolidayPay = reader.IsDBNull(reader.GetOrdinal("ndshpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndshpay")),
                                    NightDifferentialSpecialHolidayRestdayPay = reader.IsDBNull(reader.GetOrdinal("ndshrdpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndshrdpay")),
                                    NightDifferentialLegalHolidayPay = reader.IsDBNull(reader.GetOrdinal("ndlhpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndlhpay")),
                                    NightDifferentialLegalHolidayRestdayPay = reader.IsDBNull(reader.GetOrdinal("ndlhrdpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndlhrdpay")),
                                    TotalBasicPay = reader.IsDBNull(reader.GetOrdinal("total_basic_pay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_basic_pay")),
                                    RegOtPay = reader.IsDBNull(reader.GetOrdinal("regotpay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("regotpay"))
                                };
                                payslips.Add(payslip);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching payslips: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return payslips;
        }

        private void generate_Click(object sender, EventArgs e)
        {
            // Parse input fields
            if (!int.TryParse(month.Text, out int monthVal) ||
                !int.TryParse(controlPeriod.Text, out int controlPeriodVal) ||
                !int.TryParse(payrollyear.Text, out int payrollYearVal) ||
                !DateTime.TryParse(fromdate.Text, out DateTime fromDateVal) ||
                !DateTime.TryParse(todate.Text, out DateTime toDateVal))
            {
                MessageBox.Show("Invalid input. Please check your entries.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If regenerate is checked, update all rows; otherwise, only update where total_grosspay is NULL or 0
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string selectQuery = @"SELECT id, gross_pay, total_govdues, loan_deduction, total_deductions, total_grosspay FROM payroll WHERE month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod AND pay_period_start = @fromDate AND pay_period_end = @toDate";
                    using (var cmd = new MySqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@month", monthVal);
                        cmd.Parameters.AddWithValue("@payrollYear", payrollYearVal);
                        cmd.Parameters.AddWithValue("@controlPeriod", controlPeriodVal);
                        cmd.Parameters.AddWithValue("@fromDate", fromDateVal);
                        cmd.Parameters.AddWithValue("@toDate", toDateVal);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var updates = new List<(int id, decimal totalGrossPay, bool shouldUpdate)>();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                decimal grossPay = reader.IsDBNull(reader.GetOrdinal("gross_pay")) ? 0 : reader.GetDecimal(reader.GetOrdinal("gross_pay"));
                                decimal totalGovDues = reader.IsDBNull(reader.GetOrdinal("total_govdues")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_govdues"));
                                decimal loanDeduction = 0;
                                decimal totalDeductions = 0;
                                try { loanDeduction = reader.IsDBNull(reader.GetOrdinal("loan_deduction")) ? 0 : reader.GetDecimal(reader.GetOrdinal("loan_deduction")); } catch { }
                                try { totalDeductions = reader.IsDBNull(reader.GetOrdinal("total_deductions")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_deductions")); } catch { }
                                decimal totalGrossPay = grossPay - totalGovDues - loanDeduction - totalDeductions;
                                // If regenerate is checked, always update; otherwise, only update if total_grosspay is NULL or 0
                                bool isNull = reader.IsDBNull(reader.GetOrdinal("total_grosspay"));
                                decimal existing = isNull ? 0 : reader.GetDecimal("total_grosspay");
                                bool shouldUpdate = (regenerate != null && regenerate.Checked) ? true : (isNull || existing == 0);
                                updates.Add((id, totalGrossPay, shouldUpdate));
                            }
                            reader.Close();
                            // Update total_grosspay for each row if needed
                            foreach (var upd in updates)
                            {
                                if (upd.shouldUpdate)
                                {
                                    string updateQuery = "UPDATE payroll SET total_grosspay = @total_grosspay WHERE id = @id";
                                    using (var updateCmd = new MySqlCommand(updateQuery, conn))
                                    {
                                        updateCmd.Parameters.AddWithValue("@total_grosspay", upd.totalGrossPay);
                                        updateCmd.Parameters.AddWithValue("@id", upd.id);
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
                // After updating total_grosspay, update netpay
                UpdateNetPay(monthVal, controlPeriodVal, payrollYearVal, fromDateVal, toDateVal);
                MessageBox.Show("Payslip generation calculation and update complete.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Show payslips for the period
                var payslips = GetPayslipsForPeriod(monthVal, controlPeriodVal, payrollYearVal, fromDateVal, toDateVal);
                if (payslips.Count > 0)
                {
                    GeneratePayslipsPdf(payslips);
                }
                else
                {
                    MessageBox.Show("No payslips found for the selected period.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during payslip generation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // PayslipViewModel for displaying payslip data
    public class PayslipViewModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Client { get; set; } // Added for client from employee table
        public string BirStat { get; set; } // Added for BIR status from employee table
        public string AtmCardNo { get; set; } // Added for ATM card number from employee table
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal RatePerDay { get; set; }
        public decimal BasicPay { get; set; }
        public decimal LegalHolidayPay { get; set; }
        public decimal TardyUndertimePay { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal NightDifferentialPay { get; set; }
        public decimal GrossPay { get; set; }
        public decimal SSS { get; set; }
        public decimal PhilHealth { get; set; }
        public decimal HDMF { get; set; }
        public decimal HMO { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal SIL { get; set; }
        public decimal PerfectAttendance { get; set; }
        public decimal Adjustment { get; set; }
        public decimal Reliever { get; set; }
        public decimal NetPay { get; set; }
        public decimal CashAdvance { get; set; }
        public decimal Uniform { get; set; }
        public decimal AtmId { get; set; }
        public decimal Medical { get; set; }
        public decimal Grocery { get; set; }
        public decimal Canteen { get; set; }
        public decimal Damayan { get; set; }
        public decimal Rice { get; set; }
        public decimal TotalDays { get; set; }
        public decimal LegalHolidayCount { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal RestdayHours { get; set; }
        public decimal RestdayOvertimeHours { get; set; }
        public decimal LegalHolidayHours { get; set; }
        public decimal LegalHolidayOvertimeHours { get; set; }
        public decimal LegalHolidayRestdayHours { get; set; }
        public decimal LegalHolidayRestdayOvertimeHours { get; set; }
        public decimal SpecialHolidayHours { get; set; }
        public decimal SpecialHolidayOvertimeHours { get; set; }
        public decimal SpecialHolidayRestdayHours { get; set; }
        public decimal SpecialHolidayRestdayOvertimeHours { get; set; }
        public decimal NightDifferentialHours { get; set; }
        public decimal NightDifferentialOvertimeHours { get; set; }
        public decimal NightDifferentialRestdayHours { get; set; }
        public decimal NightDifferentialSpecialHolidayHours { get; set; }
        public decimal NightDifferentialSpecialHolidayRestdayHours { get; set; }
        public decimal NightDifferentialLegalHolidayHours { get; set; }
        public decimal NightDifferentialLegalHolidayRestdayHours { get; set; }
        public decimal RestdayPay { get; set; }
        public decimal RestdayOvertimePay { get; set; }
        public decimal LegalHolidayOvertimePay { get; set; }
        public decimal LegalHolidayRestdayPay { get; set; }
        public decimal LegalHolidayRestdayOvertimePay { get; set; }
        public decimal SpecialHolidayPay { get; set; }
        public decimal SpecialHolidayOvertimePay { get; set; }
        public decimal SpecialHolidayRestdayPay { get; set; }
        public decimal SpecialHolidayRestdayOvertimePay { get; set; }
        public decimal NightDifferentialOvertimePay { get; set; }
        public decimal NightDifferentialRestdayPay { get; set; }
        public decimal NightDifferentialSpecialHolidayPay { get; set; }
        public decimal NightDifferentialSpecialHolidayRestdayPay { get; set; }
        public decimal NightDifferentialLegalHolidayPay { get; set; }
        public decimal NightDifferentialLegalHolidayRestdayPay { get; set; }
        public decimal TotalBasicPay { get; set; }
        public decimal RegOtPay { get; set; }
    }
}

