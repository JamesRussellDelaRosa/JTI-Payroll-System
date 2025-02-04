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

        private void UploadDatFileToDatabase(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int totalLines = lines.Length;

                progressBar1.Minimum = 0;
                progressBar1.Maximum = totalLines;
                progressBar1.Value = 0;
                progressBar1.Step = 1;

                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // 🟢 Step 1: Load existing records into HashSet for fast lookup
                    HashSet<string> existingRecords = new HashSet<string>();
                    string loadExistingQuery = "SELECT CONCAT(id, '|', date, '|', time) FROM attendance";
                    using (MySqlCommand loadCmd = new MySqlCommand(loadExistingQuery, conn))
                    {
                        using (MySqlDataReader reader = loadCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existingRecords.Add(reader.GetString(0)); // Add "id|date|time" as key
                            }
                        }
                    }

                    int successfulInserts = 0;
                    int skippedLines = 0;
                    int duplicateRecords = 0;

                    List<MySqlCommand> insertCommands = new List<MySqlCommand>(); // Batch insert commands

                    // 🟢 Step 2: Process each line from the file
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length >= 6)
                        {
                            try
                            {
                                string id = values[0];
                                string date = values[1];
                                string time = values[2];
                                string recordKey = $"{id}|{date}|{time}"; // Unique key for checking duplicates

                                // Check in-memory first before inserting
                                if (existingRecords.Contains(recordKey))
                                {
                                    duplicateRecords++;
                                    continue; // Skip exact duplicate records
                                }

                                // 🟢 Step 3: Add insert query to batch
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

                                // Add to existing records to prevent duplicate insertions in the same run
                                existingRecords.Add(recordKey);

                                successfulInserts++;
                            }
                            catch (Exception sqlEx)
                            {
                                Console.WriteLine($"SQL Error on line: \"{line}\" - {sqlEx.Message}");
                                skippedLines++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Skipping line due to incorrect format: \"{line}\" (Found {values.Length} values)");
                            skippedLines++;
                        }

                        progressBar1.PerformStep();
                        Application.DoEvents();
                    }

                    // 🟢 Step 4: Execute all insert queries in a batch (faster)
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        foreach (var cmd in insertCommands)
                        {
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }

                    MessageBox.Show($"Import Completed!\nSuccessful Inserts: {successfulInserts}\nSkipped Lines: {skippedLines}\nDuplicate Records Skipped: {duplicateRecords}",
                        "Import Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                progressBar1.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar1.Value = 0;
            }
        }
    }
}
