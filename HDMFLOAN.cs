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

                    // Load saved data for the selected employee
                    LoadEmployeeData(selectedEmployeeId);
                    LoadEmployeeDataCalamity(selectedEmployeeId);
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

                    // Load saved data for the selected employee
                    LoadEmployeeData(selectedEmployeeId);
                    LoadEmployeeDataCalamity(selectedEmployeeId);
                }
                else
                {
                    MessageBox.Show("Employee data is missing in the panel's Tag property.");
                }
            }
        }

        //HDMF LOAN

        private void LoadEmployeeData(string employeeId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection " +
                                   "FROM hdmfloan WHERE employee_id = @employee_id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@employee_id", employeeId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate textboxes with the retrieved data
                                loandate.Text = Convert.ToDateTime(reader["loan_date"]).ToString("MM/dd/yyyy");
                                loanamt.Text = reader["loan_amount"].ToString();
                                monthamort.Text = reader["monthly_amortization"].ToString();
                                deductpay.Text = reader["deduction_pay"].ToString();
                                firstcollect.Text = Convert.ToDateTime(reader["first_collection"]).ToString("MM/dd/yyyy");
                                lastcollect.Text = Convert.ToDateTime(reader["last_collection"]).ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                // Clear textboxes if no data is found
                                loandate.Clear();
                                loanamt.Clear();
                                monthamort.Clear();
                                deductpay.Clear();
                                firstcollect.Clear();
                                lastcollect.Clear();
                                MessageBox.Show("No saved data found for the selected employee.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading employee data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedEmployeeId))
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

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

                    // Check if a record already exists for the selected employee
                    string checkQuery = "SELECT COUNT(*) FROM hdmfloan WHERE employee_id = @employee_id";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                        int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (recordCount > 0)
                        {
                            // Update existing record
                            string updateQuery = "UPDATE hdmfloan SET loan_date = @loan_date, loan_amount = @loan_amount, " +
                                                 "monthly_amortization = @monthly_amortization, deduction_pay = @deduction_pay, " +
                                                 "first_collection = @first_collection, last_collection = @last_collection " +
                                                 "WHERE employee_id = @employee_id";
                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                                updateCommand.Parameters.AddWithValue("@loan_date", loanDate.ToString("yyyy-MM-dd"));
                                updateCommand.Parameters.AddWithValue("@loan_amount", loanAmount);
                                updateCommand.Parameters.AddWithValue("@monthly_amortization", monthlyAmortization);
                                updateCommand.Parameters.AddWithValue("@deduction_pay", deductionPay);
                                updateCommand.Parameters.AddWithValue("@first_collection", firstCollection.ToString("yyyy-MM-dd"));
                                updateCommand.Parameters.AddWithValue("@last_collection", lastCollection.ToString("yyyy-MM-dd"));

                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("HDMF Loan updated successfully.");
                            }
                        }
                        else
                        {
                            // Insert new record
                            string insertQuery = "INSERT INTO hdmfloan (employee_id, loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection) " +
                                                 "VALUES (@employee_id, @loan_date, @loan_amount, @monthly_amortization, @deduction_pay, @first_collection, @last_collection)";
                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                                insertCommand.Parameters.AddWithValue("@loan_date", loanDate.ToString("yyyy-MM-dd"));
                                insertCommand.Parameters.AddWithValue("@loan_amount", loanAmount);
                                insertCommand.Parameters.AddWithValue("@monthly_amortization", monthlyAmortization);
                                insertCommand.Parameters.AddWithValue("@deduction_pay", deductionPay);
                                insertCommand.Parameters.AddWithValue("@first_collection", firstCollection.ToString("yyyy-MM-dd"));
                                insertCommand.Parameters.AddWithValue("@last_collection", lastCollection.ToString("yyyy-MM-dd"));

                                insertCommand.ExecuteNonQuery();
                                MessageBox.Show("HDMF Loan saved successfully.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedEmployeeId))
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            // Confirm deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this employee's loan data?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        connection.Open();

                        // Delete the loan data for the selected employee
                        string deleteQuery = "DELETE FROM hdmfloan WHERE employee_id = @employee_id";
                        using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Employee loan data deleted successfully.");

                                // Clear the textboxes after deletion
                                loandate.Clear();
                                loanamt.Clear();
                                monthamort.Clear();
                                deductpay.Clear();
                                firstcollect.Clear();
                                lastcollect.Clear();
                            }
                            else
                            {
                                MessageBox.Show("No loan data found for the selected employee.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting the loan data: {ex.Message}",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

        // HDMF CALAMITY LOAN

        private void LoadEmployeeDataCalamity(string employeeId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection " +
                                   "FROM hdmfcalamityloan WHERE employee_id = @employee_id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@employee_id", employeeId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate textboxes with the retrieved data
                                calamityloandate.Text = Convert.ToDateTime(reader["loan_date"]).ToString("MM/dd/yyyy");
                                calamityloanamt.Text = reader["loan_amount"].ToString();
                                calamitymonthamort.Text = reader["monthly_amortization"].ToString();
                                calamitydeductpay.Text = reader["deduction_pay"].ToString();
                                calamityfirstcollect.Text = Convert.ToDateTime(reader["first_collection"]).ToString("MM/dd/yyyy");
                                calamitylastcollect.Text = Convert.ToDateTime(reader["last_collection"]).ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                // Clear textboxes if no data is found
                                calamityloandate.Clear();
                                calamityloanamt.Clear();
                                calamitymonthamort.Clear();
                                calamitydeductpay.Clear();
                                calamityfirstcollect.Clear();
                                calamitylastcollect.Clear();
                                MessageBox.Show("No saved data found for the selected employee.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading employee data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void calamitysave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedEmployeeId))
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            // Validate and parse date fields
            if (!DateTime.TryParse(calamityloandate.Text, out DateTime loanDate))
            {
                MessageBox.Show("Please enter a valid Loan Date.");
                return;
            }

            if (!DateTime.TryParse(calamityfirstcollect.Text, out DateTime firstCollection))
            {
                MessageBox.Show("Please enter a valid First Collection Date.");
                return;
            }

            if (!DateTime.TryParse(calamitylastcollect.Text, out DateTime lastCollection))
            {
                MessageBox.Show("Please enter a valid Last Collection Date.");
                return;
            }

            string loanAmount = calamityloanamt.Text;
            string monthlyAmortization = calamitymonthamort.Text;
            string deductionPay = deductpay.Text;

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Check if a record already exists for the selected employee
                    string checkQuery = "SELECT COUNT(*) FROM hdmfcalamityloan WHERE employee_id = @employee_id";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                        int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (recordCount > 0)
                        {
                            // Update existing record
                            string updateQuery = "UPDATE hdmfcalamityloan SET loan_date = @loan_date, loan_amount = @loan_amount, " +
                                                 "monthly_amortization = @monthly_amortization, deduction_pay = @deduction_pay, " +
                                                 "first_collection = @first_collection, last_collection = @last_collection " +
                                                 "WHERE employee_id = @employee_id";
                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                                updateCommand.Parameters.AddWithValue("@loan_date", loanDate.ToString("yyyy-MM-dd"));
                                updateCommand.Parameters.AddWithValue("@loan_amount", loanAmount);
                                updateCommand.Parameters.AddWithValue("@monthly_amortization", monthlyAmortization);
                                updateCommand.Parameters.AddWithValue("@deduction_pay", deductionPay);
                                updateCommand.Parameters.AddWithValue("@first_collection", firstCollection.ToString("yyyy-MM-dd"));
                                updateCommand.Parameters.AddWithValue("@last_collection", lastCollection.ToString("yyyy-MM-dd"));

                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("HDMF Calamity Loan updated successfully.");
                            }
                        }
                        else
                        {
                            // Insert new record
                            string insertQuery = "INSERT INTO hdmfcalamityloan (employee_id, loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection) " +
                                                 "VALUES (@employee_id, @loan_date, @loan_amount, @monthly_amortization, @deduction_pay, @first_collection, @last_collection)";
                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                                insertCommand.Parameters.AddWithValue("@loan_date", loanDate.ToString("yyyy-MM-dd"));
                                insertCommand.Parameters.AddWithValue("@loan_amount", loanAmount);
                                insertCommand.Parameters.AddWithValue("@monthly_amortization", monthlyAmortization);
                                insertCommand.Parameters.AddWithValue("@deduction_pay", deductionPay);
                                insertCommand.Parameters.AddWithValue("@first_collection", firstCollection.ToString("yyyy-MM-dd"));
                                insertCommand.Parameters.AddWithValue("@last_collection", lastCollection.ToString("yyyy-MM-dd"));

                                insertCommand.ExecuteNonQuery();
                                MessageBox.Show("HDMF Calamity Loan saved successfully.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void calamitydelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedEmployeeId))
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            // Confirm deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this employee's calamity loan data?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        connection.Open();

                        // Delete the loan data for the selected employee
                        string deleteQuery = "DELETE FROM hdmfcalamityloan WHERE employee_id = @employee_id";
                        using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Employee calamity loan data deleted successfully.");

                                // Clear the textboxes after deletion
                                loandate.Clear();
                                loanamt.Clear();
                                monthamort.Clear();
                                deductpay.Clear();
                                firstcollect.Clear();
                                lastcollect.Clear();
                            }
                            else
                            {
                                MessageBox.Show("No calamity loan data found for the selected employee.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting the calamity loan data: {ex.Message}",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}
