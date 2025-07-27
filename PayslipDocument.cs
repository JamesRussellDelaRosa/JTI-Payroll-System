using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace JTI_Payroll_System
{
    public class PayslipComponent : IComponent
    {
        private readonly PayslipViewModel payslip;
        public PayslipComponent(PayslipViewModel payslip)
        {
            this.payslip = payslip;
        }
        public void Compose(IContainer container)
        {
            container.Padding(0).ShowEntire().Column(col =>
            {
                col.Item().Text("JEANNIE'S TOUCH MANPOWER SOLUTIONS INC.").Bold().FontSize(11).AlignCenter();
                col.Item().Text("****PAYSLIP****").FontColor(Colors.Red.Medium).FontSize(9).AlignRight();
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text($"ID NO: {payslip.EmployeeId}").FontSize(8);
                    row.RelativeItem().Text($"NAME: {payslip.EmployeeName}").FontSize(8);
                });
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text($"DEPARTMENT: {payslip.Department}").FontSize(8);
                    row.RelativeItem().Text($"CLIENT: {payslip.Client}").FontSize(8);
                });
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text($"BIR CODE: {payslip.BirStat}").FontSize(8);
                    row.RelativeItem().Text($"ATM CARD NO: {payslip.AtmCardNo}").FontSize(8);
                });
                col.Item().Text($"PERIOD: {payslip.PeriodStart:MM/dd/yyyy} - {payslip.PeriodEnd:MM/dd/yyyy}").FontSize(8);
                col.Item().Text($"RATE/DAY: {payslip.RatePerDay:N2}").FontSize(8);
                col.Item().Text("");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("EARNINGS").Bold().FontSize(8);
                        header.Cell().Element(CellStyle).Text("CURRENT").Bold().FontSize(8);
                        header.Cell().Element(CellStyle).Text("AMOUNT").Bold().FontSize(8);
                    });
                    void AddRow(string label, string current, decimal amount, bool bold = false)
                    {
                        var textLabel = table.Cell().Element(CellStyle).Text(label).FontSize(7);
                        var textCurrent = table.Cell().Element(CellStyle).Text(current).FontSize(7);
                        var textAmount = table.Cell().Element(CellStyle).Text($"{amount:N2}").FontSize(7);
                        if (bold)
                        {
                            textLabel.Bold();
                            textAmount.Bold();
                        }
                    }
                    AddRow("Basic Pay (No. of regular Days)", payslip.TotalDays.ToString("N2"), payslip.BasicPay);
                    AddRow("Legal Holiday w/ Pay", payslip.LegalHolidayCount.ToString(), payslip.LegalHolidayPay);
                    AddRow("Less:Tardy/Undertime", payslip.TardyUndertime.ToString("N2"), payslip.TardyUndertimePay);
                    AddRow("Total Basic Pay", "", payslip.TotalBasicPay, true);
                    // Overtime Pay header
                    var textLabel = table.Cell().Element(CellStyle).Text("OVERTIME PAY").Bold().FontSize(7);
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    AddRow("Regular OT Hrs.", payslip.OvertimeHours.ToString("N2"), payslip.RegOtPay);
                    AddRow("Rest Day Hrs.", payslip.RestdayHours.ToString("N2"), payslip.RestdayPay);
                    AddRow("Rest Day OT Hrs.", payslip.RestdayOvertimeHours.ToString("N2"), payslip.RestdayOvertimePay);
                    AddRow("Legal Holiday Hrs.", payslip.LegalHolidayHours.ToString("N2"), payslip.LegalHolidayPay);
                    AddRow("Legal Holiday OT Hrs.", payslip.LegalHolidayOvertimeHours.ToString("N2"), payslip.LegalHolidayOvertimePay);
                    AddRow("Legal Hol. Rest Day Hrs.", payslip.LegalHolidayRestdayHours.ToString("N2"), payslip.LegalHolidayRestdayPay);
                    AddRow("Legal Hol. Rest Day OT Hrs.", payslip.LegalHolidayRestdayOvertimeHours.ToString("N2"), payslip.LegalHolidayRestdayOvertimePay);
                    AddRow("Special Holiday Hrs.", payslip.SpecialHolidayHours.ToString("N2"), payslip.SpecialHolidayPay);
                    AddRow("Special Holiday OT Hrs.", payslip.SpecialHolidayOvertimeHours.ToString("N2"), payslip.SpecialHolidayOvertimePay);
                    AddRow("Spl Holiday on a Rest Day", payslip.SpecialHolidayRestdayHours.ToString("N2"), payslip.SpecialHolidayRestdayPay);
                    AddRow("Spl Hol/Rest Day OT", payslip.SpecialHolidayRestdayOvertimeHours.ToString("N2"), payslip.SpecialHolidayRestdayOvertimePay);
                    AddRow("Night Differential", payslip.NightDifferentialHours.ToString("N2"), payslip.NightDifferentialPay);
                    AddRow("Night Differential OT", payslip.NightDifferentialOvertimeHours.ToString("N2"), payslip.NightDifferentialOvertimePay);
                    AddRow("Night Diff. Rest Day", payslip.NightDifferentialRestdayHours.ToString("N2"), payslip.NightDifferentialRestdayPay);
                    AddRow("Night Diff. SH.", payslip.NightDifferentialSpecialHolidayHours.ToString("N2"), payslip.NightDifferentialSpecialHolidayPay);
                    AddRow("Night Diff. SH/RD.", payslip.NightDifferentialSpecialHolidayRestdayHours.ToString("N2"), payslip.NightDifferentialSpecialHolidayRestdayPay);
                    AddRow("Night Diff. Leg. Hol.", payslip.NightDifferentialLegalHolidayHours.ToString("N2"), payslip.NightDifferentialLegalHolidayPay);
                    AddRow("Night Diff. LH/RD.", payslip.NightDifferentialLegalHolidayRestdayHours.ToString("N2"), payslip.NightDifferentialLegalHolidayRestdayPay);
                    AddRow("Total Overtime Pay", "", payslip.OvertimePay, true);
                    AddRow("GROSS PAY", "", payslip.GrossPay, true);
                });
                col.Item().Text("");
                // Deductions and Other Earnings side by side
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(dedCol =>
                    {
                        dedCol.Item().Text("DEDUCTIONS").Bold().FontColor(Colors.Red.Medium).FontSize(9);
                        dedCol.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            void AddDeduction(string label, decimal amount)
                            {
                                table.Cell().Element(CellStyle).Text(label).FontSize(7);
                                table.Cell().Element(CellStyle).Text($"{amount:N2}").FontSize(7);
                            }
                            AddDeduction("SSS Contribution", payslip.SSS);
                            AddDeduction("PhilHealth Contribution", payslip.PhilHealth);
                            AddDeduction("HDMF Contribution", payslip.HDMF);
                            AddDeduction("Withholding Tax", payslip.WTax);
                            AddDeduction("Cash Advance", payslip.CashAdvance);
                            AddDeduction("HMO", payslip.HMO);
                            AddDeduction("Uniform", payslip.Uniform);
                            AddDeduction("ID/ATM Replacement", payslip.AtmId);
                            AddDeduction("Medical", payslip.Medical);
                            AddDeduction("Grocery", payslip.Grocery);
                            AddDeduction("Canteen", payslip.Canteen);
                            AddDeduction("Damayan", payslip.Damayan);
                            AddDeduction("Rice", payslip.Rice);
                            AddDeduction("Coop Loan", payslip.CoopLoanDeduction);
                            AddDeduction("Coop Share Capital", payslip.CoopShareCapital);
                            AddDeduction("Coop Saving Deposit", payslip.CoopSavingsDeposit);
                            AddDeduction("Coop Membership Fee", payslip.CoopMembershipFee);
                        });
                    });
                    row.RelativeItem().Column(earnCol =>
                    {
                        earnCol.Item().Text("OTHER EARNINGS").Bold().FontColor(Colors.Green.Medium).FontSize(9);
                        earnCol.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            void AddEarning(string label, decimal amount)
                            {
                                table.Cell().Element(CellStyle).Text(label).FontSize(7);
                                table.Cell().Element(CellStyle).Text($"{amount:N2}").FontSize(7);
                            }
                            AddEarning("SIL", payslip.SIL);
                            AddEarning("Perfect Attendance", payslip.PerfectAttendance);
                            AddEarning("Adjustment", payslip.Adjustment);
                            AddEarning("Reliever", payslip.Reliever);
                        });
                    });
                });
                col.Item().Text("");
                col.Item().Text($"TAKE HOME PAY: {payslip.NetPay:N2}").Bold().FontSize(11).FontColor(Colors.Blue.Medium).AlignCenter();
            });
        }
        private IContainer CellStyle(IContainer container)
        {
            return container.PaddingVertical(1).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
        }
    }

    public class PayslipDocument : IDocument
    {
        private readonly PayslipViewModel payslip;
        public PayslipDocument(PayslipViewModel payslip)
        {
            this.payslip = payslip;
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                // Removed header pagination, moved to footer
                page.Content().Component(new PayslipComponent(payslip));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                });
            });
        }
    }

    public class MultiPayslipDocument : IDocument
    {
        private readonly List<PayslipViewModel> payslips;
        public MultiPayslipDocument(List<PayslipViewModel> payslips)
        {
            this.payslips = payslips;
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            foreach (var payslip in payslips)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    // Removed header pagination, moved to footer
                    page.Content().Component(new PayslipComponent(payslip));
                    page.Header().AlignRight().Text(text =>
                    {
                        text.CurrentPageNumber();
                    });
                });
            }
        }
    }
}
