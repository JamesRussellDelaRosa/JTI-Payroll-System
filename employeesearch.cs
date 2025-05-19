using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class employeesearch : Form
    {
        public employeesearch()
        {
            InitializeComponent();
            search.Click += search_Click;
        }

        private void search_Click(object sender, EventArgs e)
        {
            string searchText = searchbar.Text.Trim();
            employee.Controls.Clear(); // Clear previous results

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a search keyword!", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT id_no, lname, fname, mname
                    FROM employee
                    WHERE id_no LIKE @search
                       OR lname LIKE @search
                       OR fname LIKE @search
                       OR mname LIKE @search";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string idNo = reader["id_no"].ToString();
                            string fname = reader["fname"].ToString();
                            string mname = reader["mname"].ToString();
                            string lname = reader["lname"].ToString();

                            Panel panel = new Panel
                            {
                                Width = 800,
                                Height = 40,
                                BorderStyle = BorderStyle.FixedSingle,
                                Margin = new Padding(3),
                                Cursor = Cursors.Hand,
                                Tag = idNo // Store id_no for later use
                            };

                            Label lbl = new Label
                            {
                                AutoSize = false,
                                Width = 780,
                                Height = 40,
                                TextAlign = ContentAlignment.MiddleLeft,
                                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                                Text = $"{idNo} | {fname} {mname} {lname}"
                            };

                            panel.Controls.Add(lbl);

                            // Add click event to panel
                            panel.Click += (s, args) => OpenEmployeeForm(idNo);
                            // Also make label clickable
                            lbl.Click += (s, args) => OpenEmployeeForm(idNo);

                            employee.Controls.Add(panel);
                        }
                    }
                }
            }
        }

        private void OpenEmployeeForm(string idNo)
        {
            // Check if an employee form is already open
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is employee empForm)
                {
                    empForm.LoadEmployeeData(idNo);
                    empForm.WindowState = FormWindowState.Normal;
                    empForm.BringToFront();
                    this.Close(); // Optionally close the search form
                    return;
                }
            }

            // If not open, create a new one
            employee newEmpForm = new employee();
            newEmpForm.Show();
            newEmpForm.LoadEmployeeData(idNo);
            this.Close(); // Optionally close the search form
        }
    }
}
