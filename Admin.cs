using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = username.Text;
                string passWord = password.Text;

                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Username, Password, UserType) VALUES (@Username, @Password, 'User')";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", userName);
                        command.Parameters.AddWithValue("@Password", passWord);
                        command.ExecuteNonQuery();

                        MessageBox.Show("User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void search_Click(object sender, EventArgs e)
        {

        }

        private void delete_Click(object sender, EventArgs e)
        {
            try
            {
                string usernameToDelete = username.Text;

                if (string.IsNullOrEmpty(usernameToDelete))
                {
                    MessageBox.Show("Please enter the username to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (MySqlConnection connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string deleteQuery = "DELETE FROM Users WHERE Username = @UsernameToDelete";
                        using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@UsernameToDelete", usernameToDelete);
                            deleteCommand.ExecuteNonQuery();

                            MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            try
            {

                string userName = username.Text;
                string newPassword = password.Text;

                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE Users SET Password = @NewPassword WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", userName);
                        command.Parameters.AddWithValue("@NewPassword", newPassword);
                        command.ExecuteNonQuery();

                        MessageBox.Show("User updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserData(); // Refresh the DataGridView
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void view_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Username, Password, UserType FROM Users";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Redirect to Admin form
            employee employeeForm = new employee();
            employeeForm.Show();
            this.Hide();
        }

        private void uploadattlog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DAT Files (*.dat)|*.dat|All Files (*.*)|*.*";
                openFileDialog.Title = "Select Attendance Data File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UploadDatFileToDatabase(openFileDialog.FileName);
                }
            }
        }

        private async void UploadDatFileToDatabase(string filePath)
        {
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show(); // Show progress window

            int successfulInserts = 0;
            int skippedLines = 0;
            int duplicateRecords = 0;
            int missingEmployees = 0;

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int totalLines = lines.Length;

                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // 🔹 Load existing attendance records to prevent duplicates
                    HashSet<string> existingRecords = new HashSet<string>();
                    string loadExistingQuery = "SELECT CONCAT(id, '|', date, '|', time) FROM attendance";
                    using (MySqlCommand loadCmd = new MySqlCommand(loadExistingQuery, conn))
                    {
                        using (MySqlDataReader reader = loadCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existingRecords.Add(reader.GetString(0));
                            }
                        }
                    }

                    // 🔹 Load valid employee IDs to check before inserting attendance
                    HashSet<string> validEmployeeIds = new HashSet<string>();
                    string loadEmployeesQuery = "SELECT id_no FROM employee";
                    using (MySqlCommand loadEmpCmd = new MySqlCommand(loadEmployeesQuery, conn))
                    {
                        using (MySqlDataReader reader = loadEmpCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                validEmployeeIds.Add(reader.GetString(0));
                            }
                        }
                    }

                    List<MySqlCommand> insertCommands = new List<MySqlCommand>();

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length >= 6)
                        {
                            try
                            {
                                string id = values[0];
                                string date = values[1];
                                string time = values[2];
                                string recordKey = $"{id}|{date}|{time}";

                                // 🔹 Skip if the record already exists
                                if (existingRecords.Contains(recordKey))
                                {
                                    duplicateRecords++;
                                    continue;
                                }

                                // 🔹 Skip if the employee does not exist in the `employee` table
                                if (!validEmployeeIds.Contains(id))
                                {
                                    missingEmployees++;
                                    continue;
                                }

                                // 🔹 Insert attendance record
                                string insertQuery = "INSERT INTO attendance (id, date, time, col1, col2, col3, col4) " +
                                                     "VALUES (@id, @date, @time, @col1, @col2, @col3, @col4)";
                                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.Parameters.AddWithValue("@date", date);
                                cmd.Parameters.AddWithValue("@time", time);
                                cmd.Parameters.AddWithValue("@col1", values[3]);
                                cmd.Parameters.AddWithValue("@col2", values[4]);
                                cmd.Parameters.AddWithValue("@col3", values[5]);
                                cmd.Parameters.AddWithValue("@col4", values.Length > 6 ? values[6] : DBNull.Value);
                                insertCommands.Add(cmd);

                                existingRecords.Add(recordKey);
                                successfulInserts++;
                            }
                            catch
                            {
                                skippedLines++;
                            }
                        }
                        else
                        {
                            skippedLines++;
                        }

                        // 🔹 Update progress form UI
                        progressForm.UpdateProgress(i + 1, totalLines, $"Processing {i + 1}/{totalLines}...");
                        await Task.Delay(10); // Simulate processing delay
                    }

                    // 🔹 Execute all insert queries within a transaction
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        foreach (var cmd in insertCommands)
                        {
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }

                MessageBox.Show($"Import Completed!\n" +
                                $"✅ Successful Inserts: {successfulInserts}\n" +
                                $"❌ Skipped Lines: {skippedLines}\n" +
                                $"⚠️ Duplicate Records Skipped: {duplicateRecords}\n" +
                                $"🚫 Missing Employee Records Skipped: {missingEmployees}",
                    "Import Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 🔹 Close progress form after upload completes
                progressForm.Close();
            }
        }

        private void processDtr_Click(object sender, EventArgs e)
        {
            // Redirect to ProcessDTR form
            processDTR processDTRForm = new processDTR();
            processDTRForm.Show();
        }

        private void btnpayrollpost_Click(object sender, EventArgs e)
        {
            // Redirect to PayrollPost form
            PayrollPost payrollPostForm = new PayrollPost();
            payrollPostForm.Show();
        }

        private void rateConfig_Click(object sender, EventArgs e)
        {
            RateConfig rateConfigForm = new RateConfig();
            rateConfigForm.Show();
        }

        private void sssgovdues_Click(object sender, EventArgs e)
        {
            sssgovdues sssgovduesForm = new sssgovdues();
            sssgovduesForm.Show();
        }

        private void ssstest_Click(object sender, EventArgs e)
        {
            SSSTEST SSSTESTForm = new SSSTEST();
            SSSTESTForm.Show();
        }

        private void sssloan_Click(object sender, EventArgs e)
        {
            SSSLOAN SSSLOANForm = new SSSLOAN();
            SSSLOANForm.Show();
        }

        private void hdmfloan_Click(object sender, EventArgs e)
        {
            HDMFLOAN HDMFLOANForm = new HDMFLOAN();
            HDMFLOANForm.Show();
        }

        private void autodeducthdmfsssloan_Click(object sender, EventArgs e)
        {
            autodeductssshdmfloan autodeductssshdmfloanForm = new autodeductssshdmfloan();
            autodeductssshdmfloanForm.Show();
        }

        private void editUsers_Click(object sender, EventArgs e)
        {
            editUsers editUsersForm = new editUsers();
            editUsersForm.Show();
        }
    }
}
