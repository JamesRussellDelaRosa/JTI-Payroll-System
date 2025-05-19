using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class employeesearch : Form
    {
        private Panel selectedPanel = null;
        private List<Panel> panelList = new List<Panel>();

        public employeesearch()
        {
            InitializeComponent();
            search.Click += search_Click;
            searchbar.KeyDown += searchbar_KeyDown;
            this.KeyPreview = true;
            this.KeyDown += employeesearch_KeyDown;
        }

        private void search_Click(object sender, EventArgs e)
        {
            string searchText = searchbar.Text.Trim();
            employee.Controls.Clear(); // Clear previous results
            selectedPanel = null; // Reset selection
            panelList.Clear(); // Clear previous panel list

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
                                Text = $"{idNo} | {fname} {mname} {lname}",
                                Cursor = Cursors.Hand
                            };

                            panel.Controls.Add(lbl);

                            // Highlight on single click
                            panel.Click += (s, args) => HighlightPanel(panel);
                            lbl.Click += (s, args) => HighlightPanel(panel);

                            // Open employee form on double click
                            panel.DoubleClick += (s, args) => OpenEmployeeForm(idNo);
                            lbl.DoubleClick += (s, args) => OpenEmployeeForm(idNo);

                            employee.Controls.Add(panel);
                            panelList.Add(panel); // Add to navigation list
                        }
                    }
                }
            }
        }

        private void HighlightPanel(Panel panel)
        {
            // Reset previous selection
            if (selectedPanel != null)
                selectedPanel.BackColor = SystemColors.Control;

            // Highlight new selection
            panel.BackColor = Color.LightBlue;
            selectedPanel = panel;
        }

        private void employeesearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (panelList.Count == 0)
                return;

            int index = selectedPanel != null ? panelList.IndexOf(selectedPanel) : -1;

            if (e.KeyCode == Keys.Down)
            {
                if (index < panelList.Count - 1)
                {
                    HighlightPanel(panelList[index + 1]);
                    e.Handled = true;
                }
                else if (index == -1 && panelList.Count > 0)
                {
                    HighlightPanel(panelList[0]);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (index > 0)
                {
                    HighlightPanel(panelList[index - 1]);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (selectedPanel != null && selectedPanel.Tag is string idNo)
                {
                    OpenEmployeeForm(idNo);
                    e.Handled = true;
                }
            }
        }

        private void searchbar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                search_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
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
