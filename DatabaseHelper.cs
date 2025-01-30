using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace JTI_Payroll_System
{
    internal class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["JTI_Payroll_System.Properties.Settings.JtiPayrollSystem"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
