using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class SSSLOAN : Form
    {
        private string selectedEmployeeId; // To store the selected employee's ID
        private Panel highlightedPanel = null;

        public SSSLOAN()
        {
            InitializeComponent();
            LoadEmployees();

            // Attach AutoFormatDate to the KeyPress event of the textboxes
            loandate.KeyPress += AutoFormatDate;
            firstcollect.KeyPress += AutoFormatDate;
            lastcollect.KeyPress += AutoFormatDate;
            calamityloandate.KeyPress += AutoFormatDate;
            calamityfirstcollect.KeyPress += AutoFormatDate;
            calamitylastcollect.KeyPress += AutoFormatDate;
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
                                // Add Click events to each label
                                idLabel.Click += Label_Click;
                                idLabel.DoubleClick += Label_DoubleClick;
                                firstNameLabel.Click += Label_Click;
                                firstNameLabel.DoubleClick += Label_DoubleClick;
                                middleNameLabel.Click += Label_Click;
                                middleNameLabel.DoubleClick += Label_DoubleClick;
                                lastNameLabel.Click += Label_Click;
                                lastNameLabel.DoubleClick += Label_DoubleClick;
                                ccodeLabel.Click += Label_Click;
                                ccodeLabel.DoubleClick += Label_DoubleClick;

                                // Add labels to the panel
                                employeePanel.Controls.Add(idLabel);
                                employeePanel.Controls.Add(firstNameLabel);
                                employeePanel.Controls.Add(middleNameLabel);
                                employeePanel.Controls.Add(lastNameLabel);
                                employeePanel.Controls.Add(ccodeLabel);

                                // Add Click and DoubleClick events to the panel
                                employeePanel.Click += Panel_Click;
                                employeePanel.DoubleClick += Panel_DoubleClick;
                                employeePanel.DoubleClick += (s, e) => { }; // Ensures DoubleClick is recognized
                                employeePanel.TabStop = true;
                                employeePanel.KeyDown += EmployeePanel_KeyDown;

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
            HighlightPanel(((Control)sender).Parent as Panel);
        }
        private void Label_DoubleClick(object sender, EventArgs e)
        {
            // Load and display data
            Panel parentPanel = ((Control)sender).Parent as Panel;
            if (parentPanel != null)
            {
                var employeeData = (dynamic)parentPanel.Tag;
                if (employeeData != null)
                {
                    selectedEmployeeId = employeeData.Id;
                    empid.Text = $"ID NO.: {employeeData.Id}";
                    empname.Text = $"EMPLOYEE NAME: {employeeData.Name}";
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
            HighlightPanel(sender as Panel ?? ((Control)sender).Parent as Panel);
        }
        private void Panel_DoubleClick(object sender, EventArgs e)
        {
            // Load and display data
            Panel clickedPanel = sender as Panel ?? ((Control)sender).Parent as Panel;
            if (clickedPanel != null)
            {
                var employeeData = (dynamic)clickedPanel.Tag;
                if (employeeData != null)
                {
                    selectedEmployeeId = employeeData.Id;
                    empid.Text = $"ID NO.: {employeeData.Id}";
                    empname.Text = $"EMPLOYEE NAME: {employeeData.Name}";
                    LoadEmployeeData(selectedEmployeeId);
                    LoadEmployeeDataCalamity(selectedEmployeeId);
                }
                else
                {
                    MessageBox.Show("Employee data is missing in the panel's Tag property.");
                }
            }
        }

        private void HighlightPanel(Panel panel)
        {
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is Panel p)
                    p.BackColor = SystemColors.Control;
            }
            if (panel != null)
            {
                panel.BackColor = Color.LightBlue;
                panel.Focus();
                highlightedPanel = panel;
            }
        }
        private void EmployeePanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (highlightedPanel == null) return;

            int idx = flowLayoutPanel1.Controls.GetChildIndex(highlightedPanel);
            if (e.KeyCode == Keys.Down && idx < flowLayoutPanel1.Controls.Count - 1)
            {
                HighlightPanel((Panel)flowLayoutPanel1.Controls[idx + 1]);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && idx > 0)
            {
                HighlightPanel((Panel)flowLayoutPanel1.Controls[idx - 1]);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                // Simulate double-click to load data
                Panel_DoubleClick(highlightedPanel, EventArgs.Empty);
                e.Handled = true;
            }
        }

        //SSS LOAN

        private void LoadEmployeeData(string employeeId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection " +
                                   "FROM sssloan WHERE employee_id = @employee_id";
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

        private void save_Click_1(object sender, EventArgs e)
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
                    string checkQuery = "SELECT COUNT(*) FROM sssloan WHERE employee_id = @employee_id";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                        int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (recordCount > 0)
                        {
                            // Update existing record
                            string updateQuery = "UPDATE sssloan SET loan_date = @loan_date, loan_amount = @loan_amount, " +
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
                                MessageBox.Show("sss Loan updated successfully.");
                            }
                        }
                        else
                        {
                            // Insert new record
                            string insertQuery = "INSERT INTO sssloan (employee_id, loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection) " +
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
                                MessageBox.Show("sss Loan saved successfully.");
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

        private void delete_Click_1(object sender, EventArgs e)
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
                        string deleteQuery = "DELETE FROM sssloan WHERE employee_id = @employee_id";
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

        //SSS CALAMITY LOAN

        private void LoadEmployeeDataCalamity(string employeeId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection " +
                                   "FROM ssscalamityloan WHERE employee_id = @employee_id";
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

        private void calamitysave_Click_1(object sender, EventArgs e)
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
                    string checkQuery = "SELECT COUNT(*) FROM ssscalamityloan WHERE employee_id = @employee_id";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@employee_id", selectedEmployeeId);
                        int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (recordCount > 0)
                        {
                            // Update existing record
                            string updateQuery = "UPDATE ssscalamityloan SET loan_date = @loan_date, loan_amount = @loan_amount, " +
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
                                MessageBox.Show("sss Calamity Loan updated successfully.");
                            }
                        }
                        else
                        {
                            // Insert new record
                            string insertQuery = "INSERT INTO ssscalamityloan (employee_id, loan_date, loan_amount, monthly_amortization, deduction_pay, first_collection, last_collection) " +
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
                                MessageBox.Show("sss Calamity Loan saved successfully.");
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

        private void calamitydelete_Click_1(object sender, EventArgs e)
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
                        string deleteQuery = "DELETE FROM ssscalamityloan WHERE employee_id = @employee_id";
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

        private void AutoFormatDate(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Allow only digits and control keys (e.g., backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // Auto-insert slashes based on the MM/DD/YYYY format
            if (!char.IsControl(e.KeyChar))
            {
                int length = textBox.Text.Length;

                if (length == 2 || length == 5)
                {
                    textBox.Text += "/";
                    textBox.SelectionStart = textBox.Text.Length; // Move the caret to the end
                }
            }
        }

    }
}
