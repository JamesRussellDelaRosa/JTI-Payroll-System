using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

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

            // Database connection string
            string connectionString = ConfigurationManager.ConnectionStrings["JTI_Payroll_System.Properties.Settings.JtiPayrollSystem"].ConnectionString;

            // SQL query to check user credentials
            string query = "SELECT UserType FROM Users WHERE Username = @Username AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();

                        // Execute the query and read the result
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Login successful, retrieve UserType
                                string userType = reader.GetString(0);

                                MessageBox.Show("Login successful!");

                                if (userType == "Admin")
                                {
                                    // Redirect to Admin form
                                    Admin adminForm = new Admin();
                                    adminForm.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    // Redirect to User form (or another form)
                                    User userForm = new User();
                                    userForm.Show();
                                    this.Hide();
                                }
                            }
                            else
                            {
                                // Login failed
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
