using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class modify_payroll : Form
    {
        private MySqlConnection conn;

        public modify_payroll()
        {
            InitializeComponent();
            conn = DatabaseHelper.GetConnection();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            txtFromDate.Enter += new EventHandler(RemoveHint);
            txtFromDate.Leave += new EventHandler(AddHint);
            txtFromDate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            txtToDate.Enter += new EventHandler(RemoveHint);
            txtToDate.Leave += new EventHandler(AddHint);
            txtToDate.KeyPress += new KeyPressEventHandler(AutoFormatDate);

            AddHint(txtFromDate, null);
            AddHint(txtToDate, null);
        }

        private void RemoveHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "MM/DD/YYYY")
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }

        private void AddHint(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "MM/DD/YYYY";
                textBox.ForeColor = Color.Gray;
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

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) ||
                !DateTime.TryParse(txtToDate.Text, out DateTime toDate) ||
                !int.TryParse(txtMonth.Text, out int month) ||
                !int.TryParse(txtPayrollYear.Text, out int payrollyear) ||
                !int.TryParse(txtControlPeriod.Text, out int controlPeriod))
            {
                MessageBox.Show("Please enter valid dates (MM/DD/YYYY) and numeric values for Month, Payroll Year, and Control Period.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var gridForm = new modify_payroll_grid(fromDate, toDate, month, payrollyear, controlPeriod))
            {
                if (!gridForm.HasData)
                {
                    MessageBox.Show("This payroll period does not exist.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                gridForm.ShowDialog();
            }
        }
    }
}
