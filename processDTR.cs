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
    public partial class processDTR : Form
    {
        public processDTR()
        {
            InitializeComponent();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
     
        }

        private void processDTR_Load(object sender, EventArgs e)
        {

        }

        private void LoadDTR(DateTime startDate, DateTime endDate, string employeeID)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT date, 
                       MIN(time) AS TimeIn, 
                       MAX(time) AS TimeOut
                FROM attendance
                WHERE id = @employeeID AND date BETWEEN @startDate AND @endDate
                GROUP BY date
                ORDER BY date ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvDTR.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "DTR Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void LoadFirstEmployeeDTR()
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT e.id_no, e.fname, e.lname
                FROM employee e
                INNER JOIN attendance a ON e.id_no = a.id
                WHERE a.date BETWEEN @startDate AND @endDate
                GROUP BY e.id_no
                ORDER BY MIN(a.date) ASC
                LIMIT 1;"; // ✅ Gets the first employee with attendance

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // ✅ Convert TextBox values to DateTime
                        if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                            !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
                        {
                            MessageBox.Show("Invalid date format. Please enter a valid date (YYYY-MM-DD).",
                                "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string employeeID = reader["id_no"].ToString();
                                string fullName = reader["fname"].ToString() + " " + reader["lname"].ToString();

                                // 🔹 Set labels
                                textID.Text = employeeID;
                                textName.Text = fullName;

                                // ✅ Load the first employee’s DTR
                                LoadDTR(startDate, endDate, employeeID);
                            }
                            else
                            {
                                MessageBox.Show("No attendance records found for the selected date range.",
                                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void filter_Click(object sender, EventArgs e)
        {
          LoadFirstEmployeeDTR();
        }
    }
}
