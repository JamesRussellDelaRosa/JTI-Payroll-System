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
    public partial class User : Form
    {
        public User()
        {
            InitializeComponent();
            lblFullName.Text = $"{UserSession.FullName}";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            UserSession.Clear(); // Clear the session
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
