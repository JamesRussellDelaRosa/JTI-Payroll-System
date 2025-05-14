using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get user input
            string username = textBox1.Text;
            string password = textBox2.Text;

            string query = "SELECT UserID, FullName, UserType FROM Users WHERE Username = @Username AND Password = @Password";

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();

                        // Execute the query and read the result
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Store user info in session
                                UserSession.UserID = reader.GetInt32(0);
                                UserSession.FullName = reader.GetString(1);
                                UserSession.UserType = reader.GetString(2);
                                UserSession.Username = username;

                                MessageBox.Show("Login successful!");

                                if (UserSession.IsAdmin)
                                {
                                    Admin adminForm = new Admin();
                                    adminForm.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    User userForm = new User();
                                    userForm.Show();
                                    this.Hide();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }
    }
}
