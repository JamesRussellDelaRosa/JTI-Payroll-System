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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;

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
                                decimal relieverTotal = reader.IsDBNull(reader.GetOrdinal("reliever_total")) ? 0 : reader.GetDecimal("reliever_total");
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
                    string selectQuery = @"SELECT * FROM payroll WHERE month = @month AND payrollyear = @payrollYear AND control_period = @controlPeriod AND pay_period_start = @fromDate AND pay_period_end = @toDate";
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
                                    Department = reader["ccode"].ToString(),
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
                                    NightDifferentialLegalHolidayRestdayHours = reader.IsDBNull(reader.GetOrdinal("ndlhrd_hrs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ndlhrd_hrs"))
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

        private void GeneratePayslipPdf(PayslipViewModel payslip)
        {
            using (PdfDocument document = new PdfDocument())
            {
                document.Info.Title = "Payslip";
                PdfPage page = document.AddPage();
                double margin = 40;
                double pageWidth = page.Width.Point;
                double usableWidth = pageWidth - 2 * margin;
                int y = 40, lineHeight = 14;
                int earningsCol1 = (int)margin, earningsCol2 = (int)(margin + 90), earningsCol3 = (int)(margin + 170);
                int dedCol1 = (int)(margin + usableWidth / 2 + 10), dedCol2 = (int)(margin + usableWidth / 2 + 160);

                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 8);
                var fontStyleExType = typeof(XFont).Assembly.GetType("PdfSharp.Drawing.XFontStyleEx");
                object boldStyle = fontStyleExType != null ? Enum.Parse(fontStyleExType, "Bold", true) : null;
                XFont boldFont = boldStyle != null ? (XFont)Activator.CreateInstance(typeof(XFont), new object[] { "Arial", 8, boldStyle }) : new XFont("Arial", 8);
                XFont headerFont = boldStyle != null ? (XFont)Activator.CreateInstance(typeof(XFont), new object[] { "Arial", 10, boldStyle }) : new XFont("Arial", 10);
                XFont redFont = boldStyle != null ? (XFont)Activator.CreateInstance(typeof(XFont), new object[] { "Arial", 8, boldStyle }) : new XFont("Arial", 8);
                XBrush redBrush = XBrushes.Red;
                XBrush blackBrush = XBrushes.Black;

                // Header
                gfx.DrawString("JEANNIE'S TOUCH MANPOWER SOLUTIONS INC.", headerFont, blackBrush, new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
                y += lineHeight + 2;
                gfx.DrawString("****PAYSLIP****", redFont, redBrush, page.Width.Point - margin - 100, y);
                y += lineHeight + 2;

                // Employee Info
                gfx.DrawString("ID NO :", font, blackBrush, earningsCol1, y);
                gfx.DrawRectangle(XBrushes.Yellow, earningsCol1 + 45, y - 2, 50, lineHeight - 2);
                gfx.DrawString(payslip.EmployeeId, boldFont, blackBrush, earningsCol1 + 48, y);
                gfx.DrawString("BIR CODE :", font, blackBrush, dedCol1, y);
                gfx.DrawString("S", font, blackBrush, dedCol1 + 60, y);
                y += lineHeight;
                gfx.DrawString("NAME :", font, blackBrush, earningsCol1, y);
                gfx.DrawString(payslip.EmployeeName.ToUpper(), boldFont, blackBrush, earningsCol1 + 48, y);
                gfx.DrawString("RATE/DAY :", font, blackBrush, dedCol1, y);
                gfx.DrawString($"{payslip.RatePerDay:N2}", font, blackBrush, dedCol1 + 60, y);
                y += lineHeight;
                gfx.DrawString("JTI-MANOS -JTI INTERNATIONAL ASIA MANUFACTURING CORP.", font, blackBrush, earningsCol1 + 48, y);
                gfx.DrawString("TYPE-ATM :", font, blackBrush, dedCol1, y);
                gfx.DrawString("428303263825768", font, blackBrush, dedCol1 + 60, y);
                y += lineHeight;
                gfx.DrawString($"PAYROLL PERIOD {payslip.PeriodStart:MM/dd/yyyy} TO {payslip.PeriodEnd:MM/dd/yyyy}", font, blackBrush, earningsCol1, y);
                y += lineHeight + 2;

                // Table headers
                int tableStartY = y;
                gfx.DrawString("EARNINGS", boldFont, blackBrush, new XRect(earningsCol1, y, earningsCol2 - earningsCol1, lineHeight), XStringFormats.TopLeft);
                gfx.DrawString("CURRENT", boldFont, blackBrush, new XRect(earningsCol2, y, earningsCol3 - earningsCol2, lineHeight), XStringFormats.TopCenter);
                gfx.DrawString("AMOUNT", boldFont, blackBrush, new XRect(earningsCol3, y, 60, lineHeight), XStringFormats.TopRight);
                gfx.DrawString("DEDUCTIONS", boldFont, blackBrush, new XRect(dedCol1, y, dedCol2 - dedCol1, lineHeight), XStringFormats.TopLeft);
                gfx.DrawString("AMOUNT", boldFont, blackBrush, new XRect(dedCol2, y, 60, lineHeight), XStringFormats.TopRight);
                y += lineHeight;
                gfx.DrawLine(XPens.Black, earningsCol1, y, earningsCol3 + 60, y);
                gfx.DrawLine(XPens.Black, dedCol1, y, dedCol2 + 60, y);
                y += 2;

                // EARNINGS TABLE (detailed rows)
                var earningsRows = new (string label, string current, decimal amount, bool bold)[] {
                    ("Basic Pay (No. of regular Days)", payslip.TotalDays.ToString("N2"), payslip.BasicPay, false),
                    ("Legal Holiday w/ Pay", payslip.LegalHolidayCount.ToString(), payslip.LegalHolidayPay, false),
                    ("Less:Tardy/Undertime", payslip.TardyUndertimePay.ToString("N2"), payslip.TardyUndertimePay, false),
                    ("Total Basic Pay", "", payslip.BasicPay + payslip.LegalHolidayPay - payslip.TardyUndertimePay, true),
                    ("Overtime Pay", "", payslip.OvertimePay, true),
                    ("Regular OT Hrs.", payslip.OvertimeHours.ToString("N2"), payslip.OvertimePay, false),
                    ("Rest Day Hrs.", payslip.RestdayHours.ToString("N2"), payslip.RestdayPay, false),
                    ("Rest Day OT Hrs.", payslip.RestdayOvertimeHours.ToString("N2"), payslip.RestdayOvertimePay, false),
                    ("Legal Holiday Hrs.", payslip.LegalHolidayHours.ToString("N2"), payslip.LegalHolidayPay, false),
                    ("Legal Holiday OT Hrs.", payslip.LegalHolidayOvertimeHours.ToString("N2"), payslip.LegalHolidayOvertimePay, false),
                    ("Legal Hol. Rest Day Hrs.", payslip.LegalHolidayRestdayHours.ToString("N2"), payslip.LegalHolidayRestdayPay, false),
                    ("Legal Hol. Rest Day OT Hrs.", payslip.LegalHolidayRestdayOvertimeHours.ToString("N2"), payslip.LegalHolidayRestdayOvertimePay, false),
                    ("Special Holiday Hrs.", payslip.SpecialHolidayHours.ToString("N2"), payslip.SpecialHolidayPay, false),
                    ("Special Holiday OT Hrs.", payslip.SpecialHolidayOvertimeHours.ToString("N2"), payslip.SpecialHolidayOvertimePay, false),
                    ("Spl Holiday on a Rest Day", payslip.SpecialHolidayRestdayHours.ToString("N2"), payslip.SpecialHolidayRestdayPay, false),
                    ("Spl Hol/Rest Day OT", payslip.SpecialHolidayRestdayOvertimeHours.ToString("N2"), payslip.SpecialHolidayRestdayOvertimePay, false),
                    ("Night Differential", payslip.NightDifferentialHours.ToString("N2"), payslip.NightDifferentialPay, false),
                    ("Night Differential OT", payslip.NightDifferentialOvertimeHours.ToString("N2"), payslip.NightDifferentialOvertimePay, false),
                    ("Night Diff. Rest Day", payslip.NightDifferentialRestdayHours.ToString("N2"), payslip.NightDifferentialRestdayPay, false),
                    ("Night Diff. SH.", payslip.NightDifferentialSpecialHolidayHours.ToString("N2"), payslip.NightDifferentialSpecialHolidayPay, false),
                    ("Night Diff. SH/RD.", payslip.NightDifferentialSpecialHolidayRestdayHours.ToString("N2"), payslip.NightDifferentialSpecialHolidayRestdayPay, false),
                    ("Night Diff. Leg. Hol.", payslip.NightDifferentialLegalHolidayHours.ToString("N2"), payslip.NightDifferentialLegalHolidayPay, false),
                    ("Night Diff. LH/RD.", payslip.NightDifferentialLegalHolidayRestdayHours.ToString("N2"), payslip.NightDifferentialLegalHolidayRestdayPay, false),
                    ("Total Overtime Pay", "", payslip.OvertimePay, true),
                    ("GROSS PAY", "", payslip.GrossPay, true)
                };
                int earningsY = tableStartY + lineHeight + 2;
                foreach (var row in earningsRows)
                {
                    gfx.DrawString(row.label, row.bold ? boldFont : font, blackBrush, new XRect(earningsCol1, earningsY, earningsCol2 - earningsCol1, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString(row.current, font, blackBrush, new XRect(earningsCol2, earningsY, earningsCol3 - earningsCol2, lineHeight), XStringFormats.TopCenter);
                    gfx.DrawString($"{row.amount:N2}", row.bold ? boldFont : font, blackBrush, new XRect(earningsCol3, earningsY, 60, lineHeight), XStringFormats.TopRight);
                    earningsY += lineHeight;
                }

                // Deductions Table (right side, unchanged)
                int deductionsY = tableStartY + lineHeight + 2;
                string[] deductionLabels = new[] {
                    "GOVERNMENT DUES DEDUCTION",
                    "SSS Contribution",
                    "Phil-Health Contribution",
                    "HDMF Contribution",
                    "Loan Deduction",
                    "SSS Loan",
                    "HDMF Loan",
                    "SSS Calamity Loan",
                    "HDMF Calamity Loan",
                    "Withholding Tax",
                    "Cash Advance",
                    "HMO",
                    "Uniform",
                    "ATM ID",
                    "Medical",
                    "Grocery",
                    "Canteen",
                    "Damayan",
                    "Rice",
                    "Total Deduction"
                };
                decimal[] deductionAmounts = new[] {
                    0,
                    payslip.SSS,
                    payslip.PhilHealth,
                    payslip.HDMF,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    payslip.CashAdvance,
                    payslip.HMO,
                    payslip.Uniform,
                    payslip.AtmId,
                    payslip.Medical,
                    payslip.Grocery,
                    payslip.Canteen,
                    payslip.Damayan,
                    payslip.Rice,
                    payslip.TotalDeductions
                };
                bool[] deductionBold = new[] { true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true };
                for (int i = 0; i < deductionLabels.Length; i++)
                {
                    gfx.DrawString(deductionLabels[i], deductionBold[i] ? boldFont : font, blackBrush, new XRect(dedCol1, deductionsY, dedCol2 - dedCol1, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString($"{deductionAmounts[i]:N2}", deductionBold[i] ? boldFont : font, blackBrush, new XRect(dedCol2, deductionsY, 60, lineHeight), XStringFormats.TopRight);
                    deductionsY += lineHeight;
                }

                // Other Earnings (on the right, below deductions)
                int otherY = deductionsY;
                gfx.DrawString("OTHER EARNINGS", boldFont, blackBrush, dedCol1, otherY);
                otherY += lineHeight;
                void DrawOtherEarningRow(string label, decimal amount, bool red = false)
                {
                    XFont useFont = red ? redFont : font;
                    XBrush useBrush = red ? redBrush : blackBrush;
                    gfx.DrawString(label, useFont, useBrush, new XRect(dedCol1, otherY, dedCol2 - dedCol1, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString($"{amount:N2}", useFont, useBrush, new XRect(dedCol2, otherY, 60, lineHeight), XStringFormats.TopRight);
                    otherY += lineHeight;
                }
                DrawOtherEarningRow("SIL-SERVICE INCENTIVE LEAVE", payslip.SIL, true);
                DrawOtherEarningRow("PERFECT ATTENDANCE", payslip.PerfectAttendance, true);
                DrawOtherEarningRow("ADJUSTMENT (LH NO WORK)", payslip.Adjustment, true);
                DrawOtherEarningRow("RELIEVER", payslip.Reliever, true);
                otherY += lineHeight;
                DrawOtherEarningRow("TAKE HOME PAY", payslip.NetPay, false);

                using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf", FileName = $"Payslip_{payslip.EmployeeId}.pdf" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        document.Save(sfd.FileName);
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
            }
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
                                decimal grossPay = reader.IsDBNull(reader.GetOrdinal("gross_pay")) ? 0 : reader.GetDecimal("gross_pay");
                                decimal totalGovDues = reader.IsDBNull(reader.GetOrdinal("total_govdues")) ? 0 : reader.GetDecimal("total_govdues");
                                decimal loanDeduction = 0;
                                decimal totalDeductions = 0;
                                try { loanDeduction = reader.IsDBNull(reader.GetOrdinal("loan_deduction")) ? 0 : reader.GetDecimal("loan_deduction"); } catch { }
                                try { totalDeductions = reader.IsDBNull(reader.GetOrdinal("total_deductions")) ? 0 : reader.GetDecimal("total_deductions"); } catch { }
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
                foreach (var payslip in payslips)
                {
                    GeneratePayslipPdf(payslip);
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
    }
}
