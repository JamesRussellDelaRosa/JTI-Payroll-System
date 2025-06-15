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
    public partial class dgv_deducts_adds : Form
    {
        private DateTime fromDate;
        private DateTime toDate;
        private MySqlConnection conn;

        public dgv_deducts_adds(DateTime fromDate, DateTime toDate)
        {
            InitializeComponent();
            this.fromDate = fromDate;
            this.toDate = toDate;
            conn = DatabaseHelper.GetConnection();
            InitializeDataGridView(); // Call this before LoadData
            LoadData();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false; // Prevent the empty new row from appearing
            dataGridView1.AutoGenerateColumns = false; // Prevent auto-generation of columns

            // Set up columns
            dataGridView1.Columns.Add("employee_id", "Employee ID");
            dataGridView1.Columns.Add("lname", "Last Name");
            dataGridView1.Columns.Add("fname", "First Name");
            dataGridView1.Columns.Add("mname", "Middle Name");
            dataGridView1.Columns.Add("ccode", "Cost Center");

            // Add editable deduction columns
            dataGridView1.Columns.Add("cash_advance", "Cash Advance");
            dataGridView1.Columns.Add("hmo", "HMO");
            dataGridView1.Columns.Add("uniform", "Uniform");
            dataGridView1.Columns.Add("atm_id", "ATM ID");
            dataGridView1.Columns.Add("medical", "Medical");
            dataGridView1.Columns.Add("grocery", "Grocery");
            dataGridView1.Columns.Add("canteen", "Canteen");
            dataGridView1.Columns.Add("damayan", "Damayan");
            dataGridView1.Columns.Add("rice", "Rice");

            // Add editable other earnings columns
            dataGridView1.Columns.Add("sil", "SIL");
            dataGridView1.Columns.Add("perfect_attendance", "Perfect Attendance");

            // Make specified columns read-only and set DataPropertyName
            // The DataPropertyName must match the column names from your SQL query
            dataGridView1.Columns["employee_id"].DataPropertyName = "employee_id";
            dataGridView1.Columns["employee_id"].ReadOnly = true;
            dataGridView1.Columns["lname"].DataPropertyName = "lname";
            dataGridView1.Columns["lname"].ReadOnly = true;
            dataGridView1.Columns["fname"].DataPropertyName = "fname";
            dataGridView1.Columns["fname"].ReadOnly = true;
            dataGridView1.Columns["mname"].DataPropertyName = "mname";
            dataGridView1.Columns["mname"].ReadOnly = true;
            dataGridView1.Columns["ccode"].DataPropertyName = "ccode";
            dataGridView1.Columns["ccode"].ReadOnly = true;

            dataGridView1.Columns["cash_advance"].DataPropertyName = "cash_advance";
            dataGridView1.Columns["hmo"].DataPropertyName = "hmo";
            dataGridView1.Columns["uniform"].DataPropertyName = "uniform";
            dataGridView1.Columns["atm_id"].DataPropertyName = "atm_id";
            dataGridView1.Columns["medical"].DataPropertyName = "medical";
            dataGridView1.Columns["grocery"].DataPropertyName = "grocery";
            dataGridView1.Columns["canteen"].DataPropertyName = "canteen";
            dataGridView1.Columns["damayan"].DataPropertyName = "damayan";
            dataGridView1.Columns["rice"].DataPropertyName = "rice";
            dataGridView1.Columns["sil"].DataPropertyName = "sil";
            dataGridView1.Columns["perfect_attendance"].DataPropertyName = "perfect_attendance";
        }

        private void LoadData()
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                string query = @"
                    SELECT employee_id, lname, fname, mname, ccode,
                           cash_advance, hmo, uniform, atm_id, medical, grocery, canteen, damayan, rice,
                           sil, perfect_attendance
                    FROM payroll
                    WHERE pay_period_start = @fromDate AND pay_period_end = @toDate";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fromDate", this.fromDate);
                    cmd.Parameters.AddWithValue("@toDate", this.toDate);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the new row if any

                    // Check if the row has been changed
                    bool rowChanged = false;
                    if (row.DataBoundItem is DataRowView drv)
                    {
                        // For DataBound DataGridView
                        DataRow dataRow = drv.Row;
                        if (dataRow.RowState == DataRowState.Modified)
                        {
                            rowChanged = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.OwningColumn.ReadOnly == false && cell.Value != null && cell.Value != DBNull.Value)
                            {
                                rowChanged = true;
                                break;
                            }
                        }
                    }

                    if (rowChanged)
                    {
                        string employeeId = row.Cells["employee_id"].Value.ToString();

                        decimal cashAdvance = Convert.ToDecimal(row.Cells["cash_advance"].Value ?? 0);
                        decimal hmo = Convert.ToDecimal(row.Cells["hmo"].Value ?? 0);
                        decimal uniform = Convert.ToDecimal(row.Cells["uniform"].Value ?? 0);
                        decimal atmId = Convert.ToDecimal(row.Cells["atm_id"].Value ?? 0);
                        decimal medical = Convert.ToDecimal(row.Cells["medical"].Value ?? 0);
                        decimal grocery = Convert.ToDecimal(row.Cells["grocery"].Value ?? 0);
                        decimal canteen = Convert.ToDecimal(row.Cells["canteen"].Value ?? 0);
                        decimal damayan = Convert.ToDecimal(row.Cells["damayan"].Value ?? 0);
                        decimal rice = Convert.ToDecimal(row.Cells["rice"].Value ?? 0);
                        decimal sil = Convert.ToDecimal(row.Cells["sil"].Value ?? 0);
                        decimal perfectAttendance = Convert.ToDecimal(row.Cells["perfect_attendance"].Value ?? 0);

                        string updateQuery = @"
                            UPDATE payroll SET
                                cash_advance = @cash_advance, hmo = @hmo, uniform = @uniform, atm_id = @atm_id,
                                medical = @medical, grocery = @grocery, canteen = @canteen, damayan = @damayan,
                                rice = @rice, sil = @sil, perfect_attendance = @perfect_attendance
                            WHERE employee_id = @employee_id AND pay_period_start = @fromDate AND pay_period_end = @toDate";

                        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@cash_advance", cashAdvance);
                            updateCmd.Parameters.AddWithValue("@hmo", hmo);
                            updateCmd.Parameters.AddWithValue("@uniform", uniform);
                            updateCmd.Parameters.AddWithValue("@atm_id", atmId);
                            updateCmd.Parameters.AddWithValue("@medical", medical);
                            updateCmd.Parameters.AddWithValue("@grocery", grocery);
                            updateCmd.Parameters.AddWithValue("@canteen", canteen);
                            updateCmd.Parameters.AddWithValue("@damayan", damayan);
                            updateCmd.Parameters.AddWithValue("@rice", rice);
                            updateCmd.Parameters.AddWithValue("@sil", sil);
                            updateCmd.Parameters.AddWithValue("@perfect_attendance", perfectAttendance);
                            updateCmd.Parameters.AddWithValue("@employee_id", employeeId);
                            updateCmd.Parameters.AddWithValue("@fromDate", this.fromDate);
                            updateCmd.Parameters.AddWithValue("@toDate", this.toDate);

                            updateCmd.ExecuteNonQuery();
                        }

                        // Calculate total deductions and update total_deductions column
                        decimal totalDeductions = cashAdvance + hmo + uniform + atmId + medical + grocery + canteen + damayan + rice;
                        string updateTotalDeductionsQuery = @"
                            UPDATE payroll SET total_deductions = @total_deductions
                            WHERE employee_id = @employee_id AND pay_period_start = @fromDate AND pay_period_end = @toDate";
                        using (MySqlCommand updateTotalCmd = new MySqlCommand(updateTotalDeductionsQuery, conn))
                        {
                            updateTotalCmd.Parameters.AddWithValue("@total_deductions", totalDeductions);
                            updateTotalCmd.Parameters.AddWithValue("@employee_id", employeeId);
                            updateTotalCmd.Parameters.AddWithValue("@fromDate", this.fromDate);
                            updateTotalCmd.Parameters.AddWithValue("@toDate", this.toDate);
                            updateTotalCmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Refresh data to reflect changes and reset row states
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
