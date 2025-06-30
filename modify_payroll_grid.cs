using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class modify_payroll_grid : Form
    {
        private DataTable payrollTable;
        private MySqlDataAdapter adapter;
        private MySqlConnection conn;
        private BindingSource bindingSource = new BindingSource();
        private Label noDataLabel;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool HasData { get; private set; } = false;

        public modify_payroll_grid(DateTime fromDate, DateTime toDate, int month, int payrollyear, int controlPeriod)
        {
            InitializeComponent();
            conn = DatabaseHelper.GetConnection();
            LoadPayrollData(fromDate, toDate, month, payrollyear, controlPeriod);
        }

        private void LoadPayrollData(DateTime fromDate, DateTime toDate, int month, int payrollyear, int controlPeriod)
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM payroll WHERE pay_period_start >= @fromDate AND pay_period_end <= @toDate AND month = @month AND payrollyear = @payrollyear AND control_period = @controlPeriod";
                adapter = new MySqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@fromDate", fromDate);
                adapter.SelectCommand.Parameters.AddWithValue("@toDate", toDate);
                adapter.SelectCommand.Parameters.AddWithValue("@month", month);
                adapter.SelectCommand.Parameters.AddWithValue("@payrollyear", payrollyear);
                adapter.SelectCommand.Parameters.AddWithValue("@controlPeriod", controlPeriod);
                MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                payrollTable = new DataTable();
                adapter.Fill(payrollTable);
                HasData = payrollTable.Rows.Count > 0;
                if (!HasData)
                {
                    // Do not show or update UI if no data
                    return;
                }
                bindingSource.DataSource = payrollTable;
                dataGridView1.DataSource = bindingSource;
                dataGridView1.Visible = true;
                btnSaveChanges.Visible = true;
                HideNoDataLabel();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payroll data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void ShowNoDataLabel()
        {
            if (noDataLabel == null)
            {
                noDataLabel = new Label();
                noDataLabel.Text = "This payroll period does not exist.";
                noDataLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                noDataLabel.ForeColor = Color.Red;
                noDataLabel.AutoSize = true;
                noDataLabel.Location = new Point(30, 30);
                this.Controls.Add(noDataLabel);
            }
            noDataLabel.Visible = true;
        }

        private void HideNoDataLabel()
        {
            if (noDataLabel != null)
                noDataLabel.Visible = false;
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                adapter.Update(payrollTable);
                MessageBox.Show("Payroll changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn col in this.dataGridView1.Columns)
            {
                if (col.Name == "id" ||
                    col.Name == "employee_id" ||
                    col.Name == "lname" ||
                    col.Name == "fname" ||
                    col.Name == "mname" ||
                    col.Name == "ccode" ||
                    col.Name == "pay_period_start" ||
                    col.Name == "pay_period_end" ||
                    col.Name == "month" ||
                    col.Name == "payrollyear" ||
                    col.Name == "control_period")
                {
                    col.ReadOnly = true;
                    col.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                }
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            string searchTerm = searchtxt.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                // Clear selection if search is empty
                dataGridView1.ClearSelection();
                return;
            }

            bool found = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                string empId = row.Cells["employee_id"]?.Value?.ToString() ?? string.Empty;
                string lname = row.Cells["lname"]?.Value?.ToString() ?? string.Empty;
                string fname = row.Cells["fname"]?.Value?.ToString() ?? string.Empty;
                string mname = row.Cells["mname"]?.Value?.ToString() ?? string.Empty;

                if (empId.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    lname.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    fname.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    mname.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    found = true;
                    break; // Only highlight the first match
                }
            }
            if (!found)
            {
                MessageBox.Show("No employee found matching your search.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataGridView1.ClearSelection();
            }
        }
    }
}
