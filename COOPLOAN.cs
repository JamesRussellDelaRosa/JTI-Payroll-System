using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel; // Replaced EPPlus with ClosedXML
using System.IO;

namespace JTI_Payroll_System
{
    public partial class COOPLOAN : Form
    {
        public COOPLOAN()
        {
            InitializeComponent();
        }

        private void upload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                openFileDialog.Title = "Select an Excel File (.xlsx)";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook(filePath))
                        {
                            IXLWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null)
                            {
                                MessageBox.Show("No worksheet found in the Excel file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            using (MySqlConnection conn = DatabaseHelper.GetConnection())
                            {
                                conn.Open();
                                int successCount = 0;
                                int failCount = 0;
                                List<string> errors = new List<string>(); // Errors are still collected but won't be displayed in detail

                                int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                                if (lastRow == 0 && worksheet.RowsUsed().Any())
                                {
                                    lastRow = worksheet.RowsUsed().Last().RowNumber();
                                }

                                // Revised loop to correctly identify and skip non-data rows
                                for (int currentRowNum = 1; currentRowNum <= lastRow; currentRowNum++)
                                {
                                    // --- Cell Value Extraction for Decisions ---
                                    string idCellValue = worksheet.Cell(currentRowNum, 2).GetString().Trim();    // Column B (ID No.)
                                    string hcCellValue = worksheet.Cell(currentRowNum, 1).GetString().Trim();    // Column A (HC / Title)
                                    string nameCellValue = worksheet.Cell(currentRowNum, 3).GetString().Trim();  // Column C (Name / Title)

                                    // --- Skip Conditions ---
                                    // 1. Skip actual header rows (where ID column itself has "ID No.")
                                    if (idCellValue.Equals("ID No.", StringComparison.OrdinalIgnoreCase))
                                    {
                                        continue;
                                    }

                                    // 2. Skip section title rows (e.g., "JTI - STAFF (MANUAL)" in Col A, and ID in Col B is blank)
                                    if ((hcCellValue.StartsWith("JTI - STAFF (MANUAL)", StringComparison.OrdinalIgnoreCase) ||
                                         hcCellValue.StartsWith("JTI - OP (MANUAL)", StringComparison.OrdinalIgnoreCase)) &&
                                        string.IsNullOrWhiteSpace(idCellValue))
                                    {
                                        continue;
                                    }

                                    // 3. Skip "TOTAL" rows (where "TOTAL" is in Col A or C, and ID in Col B is blank)
                                    if (string.IsNullOrWhiteSpace(idCellValue) &&
                                        (hcCellValue.Equals("TOTAL", StringComparison.OrdinalIgnoreCase) ||
                                         nameCellValue.Equals("TOTAL", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        continue;
                                    }

                                    // 4. Skip any other row where the ID (Column B) is blank.
                                    // This is a crucial catch-all for other non-data rows like main titles (e.g., rows 1-3 of your example), 
                                    // empty formatting rows (e.g., row 12), or sub-headers that don't have an ID (e.g., row 6).
                                    if (string.IsNullOrWhiteSpace(idCellValue))
                                    {
                                        continue;
                                    }

                                    // --- If we reach here, it's considered a data row ---
                                    string employeeId = idCellValue; // Already trimmed.
                                    if (employeeId.StartsWith("'")) { employeeId = employeeId.Substring(1); }


                                    Func<int, decimal> getDecimalValue = (colIdx) =>
                                    {
                                        IXLCell dataCell = worksheet.Cell(currentRowNum, colIdx); // Use currentRowNum
                                        if (string.IsNullOrWhiteSpace(dataCell.GetString())) return 0;
                                        string sVal = dataCell.GetString().Replace(",", "");
                                        decimal.TryParse(sVal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal valResult);
                                        return valResult;
                                    };

                                    decimal totalLoansDeduction = getDecimalValue(20); // Column T
                                    decimal shareCapital = getDecimalValue(21);       // Column U
                                    decimal savingsDeposit = getDecimalValue(22);     // Column V
                                    decimal membershipFee = getDecimalValue(23);        // Column W

                                    string query = @"UPDATE payroll 
                                                     SET coop_loan_deduction = @coopLoan, 
                                                         coop_share_capital = @shareCapital, 
                                                         coop_savings_deposit = @savingsDeposit,
                                                         coop_membership_fee = @membershipFee
                                                     WHERE employee_id = @employeeId 
                                                     ORDER BY pay_period_end DESC LIMIT 1";

                                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@coopLoan", totalLoansDeduction);
                                        cmd.Parameters.AddWithValue("@shareCapital", shareCapital);
                                        cmd.Parameters.AddWithValue("@savingsDeposit", savingsDeposit);
                                        cmd.Parameters.AddWithValue("@membershipFee", membershipFee);
                                        cmd.Parameters.AddWithValue("@employeeId", employeeId);

                                        try
                                        {
                                            int rowsAffected = cmd.ExecuteNonQuery();
                                            if (rowsAffected > 0)
                                            {
                                                successCount++;
                                            }
                                            else
                                            {
                                                failCount++;
                                                errors.Add($"No payroll record found or updated for Employee ID: {employeeId} (Row {currentRowNum})");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            failCount++;
                                            errors.Add($"Error updating Employee ID: {employeeId} (Row {currentRowNum}): {ex.Message}");
                                        }
                                    }
                                }

                                StringBuilder resultMessage = new StringBuilder();
                                resultMessage.AppendLine($"Successfully updated {successCount} records.");
                                if (failCount > 0)
                                {
                                    resultMessage.AppendLine($"Failed to update {failCount} records.");
                                    // Detailed error list removed as per user request
                                }
                                MessageBox.Show(resultMessage.ToString(), "Upload Result", MessageBoxButtons.OK, failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}