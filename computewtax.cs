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
    public partial class computewtax : Form
    {
        public computewtax()
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
        private DataTable LoadTaxTable(MySqlConnection conn)
        {
            var taxTable = new DataTable();
            using (var cmd = new MySqlCommand("SELECT ID, MinIncome, MaxIncome, BaseTax, ExcessRate, ExcessOver, EffectiveDate FROM SemiMonthlyWithholdingTax", conn))
            using (var adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(taxTable);
            }
            return taxTable;
        }
        private decimal CalculateWithholdingTax(decimal taxableIncome, DataTable taxTable)
        {
            var brackets = taxTable.Select($"MinIncome <= {taxableIncome} AND MaxIncome >= {taxableIncome}", "EffectiveDate DESC");
            if (brackets.Length == 0)
                return 0;
            var bracket = brackets[0];
            decimal baseTax = Convert.ToDecimal(bracket["BaseTax"]);
            decimal excessRate = Convert.ToDecimal(bracket["ExcessRate"]);
            decimal excessOver = Convert.ToDecimal(bracket["ExcessOver"]);
            decimal excess = Math.Max(0, taxableIncome - excessOver);
            decimal taxOnExcess = excess * excessRate;
            decimal totalTax = baseTax + taxOnExcess;
            return Math.Round(totalTax, 2);
        }
        private void btncomputewtax_Click(object sender, EventArgs e)
        {
            // Parse input values
            if (!int.TryParse(month.Text, out int monthVal) ||
                !int.TryParse(controlPeriod.Text, out int controlPeriodVal) ||
                !int.TryParse(payrollyear.Text, out int payrollYearVal) ||
                !DateTime.TryParseExact(fromdate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fromDateVal) ||
                !DateTime.TryParseExact(todate.Text, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime toDateVal))
            {
                MessageBox.Show("Invalid input. Please enter valid dates (MM/DD/YYYY) and numeric values for month, payroll year, and control period.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var taxTable = LoadTaxTable(conn);
                    string selectQuery = @"SELECT id, SSS, philhealth, hdmf, gross FROM payroll WHERE month = @month AND payrollyear = @payrollyear AND control_period = @control_period AND pay_period_start = @fromdate AND pay_period_end = @todate";
                    using (var selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@month", monthVal);
                        selectCmd.Parameters.AddWithValue("@payrollyear", payrollYearVal);
                        selectCmd.Parameters.AddWithValue("@control_period", controlPeriodVal);
                        selectCmd.Parameters.AddWithValue("@fromdate", fromDateVal);
                        selectCmd.Parameters.AddWithValue("@todate", toDateVal);
                        using (var reader = selectCmd.ExecuteReader())
                        {
                            var updates = new List<(int id, decimal gpay_tax, decimal wtax)>();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                decimal sss = reader.IsDBNull(reader.GetOrdinal("SSS")) ? 0 : reader.GetDecimal("SSS");
                                decimal philhealth = reader.IsDBNull(reader.GetOrdinal("philhealth")) ? 0 : reader.GetDecimal("philhealth");
                                decimal hdmf = reader.IsDBNull(reader.GetOrdinal("hdmf")) ? 0 : reader.GetDecimal("hdmf");
                                decimal gross = reader.IsDBNull(reader.GetOrdinal("gross")) ? 0 : reader.GetDecimal("gross");
                                decimal gpay_tax = gross - (sss + philhealth + hdmf);
                                decimal wtax = CalculateWithholdingTax(gpay_tax, taxTable);
                                updates.Add((id, gpay_tax, wtax));
                            }
                            reader.Close();
                            // Update gpay_tax and wtax for each row
                            foreach (var upd in updates)
                            {
                                string updateQuery = "UPDATE payroll SET gpay_tax = @gpay_tax, wtax = @wtax WHERE id = @id";
                                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@gpay_tax", upd.gpay_tax);
                                    updateCmd.Parameters.AddWithValue("@wtax", upd.wtax);
                                    updateCmd.Parameters.AddWithValue("@id", upd.id);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("gpay_tax and wtax successfully computed and updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
