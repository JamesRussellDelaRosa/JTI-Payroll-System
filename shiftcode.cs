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
    public partial class shiftcode : Form
    {
        public shiftcode()
        {
            InitializeComponent();
            dgvShiftCodes.KeyDown += dgvShiftCodes_KeyDown;
        }

        private void load_Click(object sender, EventArgs e)
        {
            LoadShiftCodes();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveShiftCodes();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            AddNewShiftCodeRow();
        }

        private void LoadShiftCodes()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT shift_code, start_time, end_time, regular_hours, ot_hours, night_differential_hours, night_differential_ot_hours FROM ShiftCodes";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvShiftCodes.DataSource = dt;
                        dgvShiftCodes.AllowUserToAddRows = false; // Prevent last empty row
                        dgvShiftCodes.ReadOnly = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading shift codes: " + ex.Message);
            }
        }

        private void SaveShiftCodes()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var dt = dgvShiftCodes.DataSource as DataTable;
                    if (dt == null) return;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted) continue;
                        string shiftCode = row["shift_code"].ToString();
                        string startTime = row["start_time"].ToString();
                        string endTime = row["end_time"].ToString();
                        decimal regularHours = Convert.ToDecimal(row["regular_hours"]);
                        decimal otHours = Convert.ToDecimal(row["ot_hours"]);
                        decimal ndHours = Convert.ToDecimal(row["night_differential_hours"]);
                        decimal ndOtHours = Convert.ToDecimal(row["night_differential_ot_hours"]);
                        // Check if exists
                        string checkQuery = "SELECT COUNT(*) FROM ShiftCodes WHERE shift_code = @shiftCode";
                        using (var checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (exists > 0)
                            {
                                // Update
                                string updateQuery = @"UPDATE ShiftCodes SET start_time=@startTime, end_time=@endTime, regular_hours=@regularHours, ot_hours=@otHours, night_differential_hours=@ndHours, night_differential_ot_hours=@ndOtHours WHERE shift_code=@shiftCode";
                                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                                    updateCmd.Parameters.AddWithValue("@startTime", startTime);
                                    updateCmd.Parameters.AddWithValue("@endTime", endTime);
                                    updateCmd.Parameters.AddWithValue("@regularHours", regularHours);
                                    updateCmd.Parameters.AddWithValue("@otHours", otHours);
                                    updateCmd.Parameters.AddWithValue("@ndHours", ndHours);
                                    updateCmd.Parameters.AddWithValue("@ndOtHours", ndOtHours);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert
                                string insertQuery = @"INSERT INTO ShiftCodes (shift_code, start_time, end_time, regular_hours, ot_hours, night_differential_hours, night_differential_ot_hours) VALUES (@shiftCode, @startTime, @endTime, @regularHours, @otHours, @ndHours, @ndOtHours)";
                                using (var insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@shiftCode", shiftCode);
                                    insertCmd.Parameters.AddWithValue("@startTime", startTime);
                                    insertCmd.Parameters.AddWithValue("@endTime", endTime);
                                    insertCmd.Parameters.AddWithValue("@regularHours", regularHours);
                                    insertCmd.Parameters.AddWithValue("@otHours", otHours);
                                    insertCmd.Parameters.AddWithValue("@ndHours", ndHours);
                                    insertCmd.Parameters.AddWithValue("@ndOtHours", ndOtHours);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("Shift codes saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving shift codes: " + ex.Message);
            }
        }

        private void AddNewShiftCodeRow()
        {
            var dt = dgvShiftCodes.DataSource as DataTable;
            if (dt != null)
            {
                dt.Rows.Add(dt.NewRow());
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {

        }

        private void dgvShiftCodes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedRows();
            }
        }

        private void DeleteSelectedRows()
        {
            var dt = dgvShiftCodes.DataSource as DataTable;
            if (dt == null) return;
            foreach (DataGridViewRow row in dgvShiftCodes.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    dgvShiftCodes.Rows.Remove(row);
                }
            }
        }
    }
}
