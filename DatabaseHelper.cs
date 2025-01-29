using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace JTI_Payroll_System
{
    internal class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["JTI_Payroll_System.Properties.Settings.JtiPayrollSystem"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
