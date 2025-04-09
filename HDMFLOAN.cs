using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class HDMFLOAN : Form
    {
        private string selectedEmployeeId; // To store the selected employee's ID

        public HDMFLOAN()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id_no, fname, lname, mname, ccode FROM employee";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            flowLayoutPanel1.Controls.Clear(); // Clear existing controls

                            while (reader.Read())
                            {
                                string id = reader["id_no"].ToString();
                                string firstName = reader["fname"].ToString();
                                string middleName = reader["mname"].ToString();
                                string lastName = reader["lname"].ToString();
                                string Ccode = reader["ccode"].ToString();

                                // Create a panel for each employee
                                Panel employeePanel = new Panel
                                {
                                    Size = new Size(700, 40),
                                    Tag = new { Id = id, Name = $"{firstName} {middleName} {lastName}", ccode = Ccode } // Store employee data in Tag
                                };

                                // Add labels for each column
                                Label idLabel = new Label
                                {
                                    Text = id,
                                    AutoSize = true,
                                    Location = new Point(10, 10), // Column 1
                                    Tag = employeePanel.Tag
                                };

                                Label firstNameLabel = new Label
                                {
                                    Text = firstName,
                                    AutoSize = true,
                                    Location = new Point(150, 10), // Column 2
                                    Tag = employeePanel.Tag
                                };

                                Label middleNameLabel = new Label
                                {
                                    Text = middleName,
                                    AutoSize = true,
                                    Location = new Point(300, 10), // Column 3
                                    Tag = employeePanel.Tag
                                };

                                Label lastNameLabel = new Label
                                {
                                    Text = lastName,
                                    AutoSize = true,
                                    Location = new Point(450, 10), // Column 4
                                    Tag = employeePanel.Tag
                                };

                                Label ccodeLabel = new Label
                                {
                                    Text = Ccode,
                                    AutoSize = true,
                                    Location = new Point(600, 10), // Column 5
                                    Tag = employeePanel.Tag
                                };

                                // Add Click events to each label
                                idLabel.Click += Label_Click;
                                firstNameLabel.Click += Label_Click;
                                middleNameLabel.Click += Label_Click;
                                lastNameLabel.Click += Label_Click;
                                ccodeLabel.Click += Label_Click;

                                // Add labels to the panel
                                employeePanel.Controls.Add(idLabel);
                                employeePanel.Controls.Add(firstNameLabel);
                                employeePanel.Controls.Add(middleNameLabel);
                                employeePanel.Controls.Add(lastNameLabel);
                                employeePanel.Controls.Add(ccodeLabel);

                                // Add Click event to the panel
                                employeePanel.Click += Panel_Click;

                                flowLayoutPanel1.Controls.Add(employeePanel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedEmployeeId))
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            MessageBox.Show($"Selected Employee ID: {selectedEmployeeId}"); // Debugging log

            // Validate and parse date fields
            if (!DateTime.TryParse(loandate.Text, out DateTime loanDate))
            {
                MessageBox.Show("Please enter a valid Loan Date.");
                return;
            }

            if (!DateTime.TryParse(firstcollect.Text, out DateTime firstCollection))
            {
                MessageBox.Show("Please enter a valid First Collection Date.");
                return;
            }

            if (!DateTime.TryParse(lastcollect.Text, out DateTime lastCollection))
            {
                MessageBox.Show("Please enter a valid Last Collection Date.");
                return;
            }

            string loanAmount = loanamt.Text;
            string monthlyAmortization = monthamort.Text;
            string deductionPay = deductpay.Text;

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO hdmf (employee_id, loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection) " +
                                "VALUES (@employee_id, @loan_date, @loan_amount, @monthly_amortization, @deduction_pay, @first_collection, @last_collection)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                        command.Parameters.AddWithValue("@loan_date", loanDate.ToString("yyyy-MM-dd")); // Format as YYYY-MM-DD
                        command.Parameters.AddWithValue("@loan_amount", loanAmount);
                        command.Parameters.AddWithValue("@monthly_amortization", monthlyAmortization);
                        command.Parameters.AddWithValue("@deduction_pay", deductionPay);
                        command.Parameters.AddWithValue("@first_collection", firstCollection.ToString("yyyy-MM-dd")); // Format as YYYY-MM-DD
                        command.Parameters.AddWithValue("@last_collection", lastCollection.ToString("yyyy-MM-dd")); // Format as YYYY-MM-DD

                        command.ExecuteNonQuery();
                        MessageBox.Show("HDMF Loan saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            // Reset the background color of all panels
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is Panel panel)
                {
                    panel.BackColor = SystemColors.Control; // Default panel color
                }
            }

            // Determine the parent panel of the clicked label
            Panel parentPanel = ((Control)sender).Parent as Panel;

            if (parentPanel != null)
            {
                // Highlight the parent panel
                parentPanel.BackColor = Color.LightBlue;

                // Set empid and empname fields
                var employeeData = (dynamic)parentPanel.Tag;
                if (employeeData != null)
                {
                    selectedEmployeeId = employeeData.Id; // Set the selected employee ID
                    empid.Text = $"ID NO.: {employeeData.Id}";
                    empname.Text = $"EMPLOYEE NAME: {employeeData.Name}";
                }
                else
                {
                    MessageBox.Show("Employee data is missing in the label's parent panel's Tag property.");
                }
            }
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            // Reset the background color of all panels
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is Panel panel)
                {
                    panel.BackColor = SystemColors.Control; // Default panel color
                }
            }

            // Determine the clicked panel
            Panel clickedPanel = sender as Panel ?? ((Control)sender).Parent as Panel;

            if (clickedPanel != null)
            {
                // Highlight the clicked panel
                clickedPanel.BackColor = Color.LightBlue;

                // Set empid and empname fields
                var employeeData = (dynamic)clickedPanel.Tag;
                if (employeeData != null)
                {
                    selectedEmployeeId = employeeData.Id; // Set the selected employee ID
                    empid.Text = $"ID NO.: {employeeData.Id}";
                    empname.Text = $"EMPLOYEE NAME: {employeeData.Name}";
                }
                else
                {
                    MessageBox.Show("Employee data is missing in the panel's Tag property.");
                }
            }
        }

    }
}
