using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTI_Payroll_System
{
    public static class UserSession
    {
        public static int UserID { get; set; }
        public static string Username { get; set; }
        public static string FullName { get; set; }
        public static string UserType { get; set; }

        public static bool IsAdmin => UserType?.ToLower() == "admin";

        public static void Clear()
        {
            UserID = 0;
            Username = null;
            FullName = null;
            UserType = null;
        }
    }
}

