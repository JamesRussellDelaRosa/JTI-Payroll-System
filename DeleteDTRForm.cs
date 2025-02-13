using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.ExtendedProperties;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class DeleteDTRForm : Form
    {
        public DeleteDTRForm()
        {
            InitializeComponent();
            LoadAvailableEmployeesWithDTR();
        }

        private void LoadAvailableEmployeesWithDTR()
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT DISTINCT e.id_no AS EmployeeID, 
                       e.lname, e.fname,  
                       CONCAT(e.fname, ' ', e.lname) AS EmployeeName,
                       MIN(p.date) AS StartDate,  -- ✅ Earliest DTR date
                       MAX(p.date) AS EndDate     -- ✅ Latest DTR date
                FROM processedDTR p
                INNER JOIN employee e ON p.employee_id = e.id_no
                GROUP BY e.id_no, e.lname, e.fname  -- ✅ Group by EmployeeID
                ORDER BY e.lname ASC, e.fname ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvAvailableEmployees.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading available employees: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ✅ Delete Selected Employee's Processed DTR
        private void btnDeleteDTR_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvAvailableEmployees.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select an employee.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string employeeID = dgvAvailableEmployees.SelectedRows[0].Cells["EmployeeID"].Value.ToString();

                DialogResult confirmDelete = MessageBox.Show(
                    "Are you sure you want to delete all processed DTR records for this employee?",
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmDelete == DialogResult.Yes)
                {
                    using (MySqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string deleteQuery = @"DELETE FROM processedDTR WHERE employee_id = @employeeID";

                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@employeeID", employeeID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Processed DTR deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ✅ Refresh available employees list
                    LoadAvailableEmployeesWithDTR();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting DTR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
