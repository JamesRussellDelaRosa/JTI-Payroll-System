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
    public partial class semimonthlywtax : Form
    {
        private DataTable taxTable = new DataTable();

        public semimonthlywtax()
        {
            InitializeComponent();
            InitializeTaxTableUI();
            LoadTaxTableFromDatabase();
        }

        private void InitializeTaxTableUI()
        {
            // Setup DataTable columns
            taxTable.Columns.Add("ID", typeof(int));
            taxTable.Columns.Add("MinIncome", typeof(decimal));
            taxTable.Columns.Add("MaxIncome", typeof(decimal));
            taxTable.Columns.Add("BaseTax", typeof(decimal));
            taxTable.Columns.Add("ExcessRate", typeof(decimal));
            taxTable.Columns.Add("ExcessOver", typeof(decimal));
            taxTable.Columns.Add("EffectiveDate", typeof(DateTime));
            taxTable.PrimaryKey = new DataColumn[] { taxTable.Columns["ID"] };
            taxTable.Columns["ID"].AutoIncrement = true;
            taxTable.Columns["ID"].AutoIncrementSeed = 1;

            // Prevent blank row for new record
            dgvTaxTable.AllowUserToAddRows = false;

            // Setup DataGridView columns formatting (after DataSource is set in Designer)
            dgvTaxTable.DataSource = taxTable;
            if (dgvTaxTable.Columns.Contains("ID"))
                dgvTaxTable.Columns["ID"].ReadOnly = true;
            if (dgvTaxTable.Columns.Contains("MinIncome"))
                dgvTaxTable.Columns["MinIncome"].DefaultCellStyle.Format = "N2";
            if (dgvTaxTable.Columns.Contains("MaxIncome"))
                dgvTaxTable.Columns["MaxIncome"].DefaultCellStyle.Format = "N2";
            if (dgvTaxTable.Columns.Contains("BaseTax"))
                dgvTaxTable.Columns["BaseTax"].DefaultCellStyle.Format = "N2";
            if (dgvTaxTable.Columns.Contains("ExcessRate"))
                dgvTaxTable.Columns["ExcessRate"].DefaultCellStyle.Format = "P2";
            if (dgvTaxTable.Columns.Contains("ExcessOver"))
                dgvTaxTable.Columns["ExcessOver"].DefaultCellStyle.Format = "N2";
            if (dgvTaxTable.Columns.Contains("EffectiveDate"))
                dgvTaxTable.Columns["EffectiveDate"].DefaultCellStyle.Format = "yyyy-MM-dd";

            // Attach event handlers
            btnCalculate.Click += BtnCalculate_Click;
            btnSave.Click += BtnSave_Click;
        }

        private void LoadTaxTableFromDatabase()
        {
            taxTable.Clear();
            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT ID, MinIncome, MaxIncome, BaseTax, ExcessRate, ExcessOver, EffectiveDate FROM SemiMonthlyWithholdingTax", conn))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(taxTable);
                }
            }
        }

        private void SaveTaxTableToDatabase()
        {
            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                foreach (DataRow row in taxTable.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("INSERT INTO SemiMonthlyWithholdingTax (MinIncome, MaxIncome, BaseTax, ExcessRate, ExcessOver, EffectiveDate) VALUES (@MinIncome, @MaxIncome, @BaseTax, @ExcessRate, @ExcessOver, @EffectiveDate)", conn))
                        {
                            cmd.Parameters.AddWithValue("@MinIncome", row["MinIncome"]);
                            cmd.Parameters.AddWithValue("@MaxIncome", row["MaxIncome"]);
                            cmd.Parameters.AddWithValue("@BaseTax", row["BaseTax"]);
                            cmd.Parameters.AddWithValue("@ExcessRate", row["ExcessRate"]);
                            cmd.Parameters.AddWithValue("@ExcessOver", row["ExcessOver"]);
                            cmd.Parameters.AddWithValue("@EffectiveDate", row["EffectiveDate"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("UPDATE SemiMonthlyWithholdingTax SET MinIncome=@MinIncome, MaxIncome=@MaxIncome, BaseTax=@BaseTax, ExcessRate=@ExcessRate, ExcessOver=@ExcessOver, EffectiveDate=@EffectiveDate WHERE ID=@ID", conn))
                        {
                            cmd.Parameters.AddWithValue("@MinIncome", row["MinIncome"]);
                            cmd.Parameters.AddWithValue("@MaxIncome", row["MaxIncome"]);
                            cmd.Parameters.AddWithValue("@BaseTax", row["BaseTax"]);
                            cmd.Parameters.AddWithValue("@ExcessRate", row["ExcessRate"]);
                            cmd.Parameters.AddWithValue("@ExcessOver", row["ExcessOver"]);
                            cmd.Parameters.AddWithValue("@EffectiveDate", row["EffectiveDate"]);
                            cmd.Parameters.AddWithValue("@ID", row["ID"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (row.RowState == DataRowState.Deleted)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM SemiMonthlyWithholdingTax WHERE ID=@ID", conn))
                        {
                            cmd.Parameters.AddWithValue("@ID", row["ID", DataRowVersion.Original]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveTaxTableToDatabase();
                taxTable.AcceptChanges();
                MessageBox.Show("Tax table changes saved to database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtIncome.Text, out decimal income))
            {
                decimal tax = CalculateWithholdingTax(income);
                lblResult.Text = $"Withholding Tax: PHP {tax:N2}";
            }
            else
            {
                MessageBox.Show("Please enter a valid income amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public decimal CalculateWithholdingTax(decimal taxableIncome)
        {
            DataRow[] brackets = taxTable.Select($"MinIncome <= {taxableIncome} AND MaxIncome >= {taxableIncome}", "EffectiveDate DESC");
            if (brackets.Length == 0)
                return 0;
            DataRow bracket = brackets[0];
            decimal baseTax = Convert.ToDecimal(bracket["BaseTax"]);
            decimal excessRate = Convert.ToDecimal(bracket["ExcessRate"]);
            decimal excessOver = Convert.ToDecimal(bracket["ExcessOver"]);
            decimal excess = Math.Max(0, taxableIncome - excessOver);
            decimal taxOnExcess = excess * excessRate;
            decimal totalTax = baseTax + taxOnExcess;
            return Math.Round(totalTax, 2);
        }
    }
}
