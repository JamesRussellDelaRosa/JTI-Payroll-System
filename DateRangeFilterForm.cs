using System;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class DateRangeFilterForm : Form
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        public DateRangeFilterForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validate and parse the date inputs
            if (!DateTime.TryParse(txtFrom.Text, out DateTime fromDate))
            {
                MessageBox.Show("Invalid 'From' date. Please use MM/DD/YYYY format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFrom.Focus();
                return;
            }
            if (!DateTime.TryParse(txtTo.Text, out DateTime toDate))
            {
                MessageBox.Show("Invalid 'To' date. Please use MM/DD/YYYY format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTo.Focus();
                return;
            }
            if (fromDate > toDate)
            {
                MessageBox.Show("'From' date must be before or equal to 'To' date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFrom.Focus();
                return;
            }
            FromDate = fromDate;
            ToDate = toDate;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
