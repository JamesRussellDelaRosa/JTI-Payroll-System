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
    public partial class deducts_adds : Form
    {
        public deducts_adds()
        {
            InitializeComponent();
        }

        private void OpenFormButton_Click(object sender, EventArgs e)
        {
            // Validate and parse dates
            if (!DateTime.TryParse(fromDateTextBox.Text, out DateTime fromDate) ||
                !DateTime.TryParse(toDateTextBox.Text, out DateTime toDate))
            {
                MessageBox.Show("Please enter valid dates in MM/DD/YYYY format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgv_deducts_adds newForm = new dgv_deducts_adds(fromDate, toDate);
            newForm.Show();
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

        private void toDateTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
