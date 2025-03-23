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
    public partial class RateConfig : Form
    {
        public RateConfig()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void autoCompute_Click_1(object sender, EventArgs e)
        {
            if (decimal.TryParse(ratecompute.Text, out decimal rate))
            {
                rdpay.Text = (rate / 8 * 1.3m).ToString("F2");
                rdotpay.Text = (rate / 8 * 1.69m).ToString("F2");
                lhpay.Text = "520";
                regotpay.Text = (rate / 8 * 1.25m).ToString("F2");
                trdypay.Text = (-rate / 8).ToString("F2");
                lhhrspay.Text = (rate / 8).ToString("F2");
                lhothrspay.Text = (rate / 8 * 2.6m).ToString("F2");
                lhrdpay.Text = (rate / 8 * 2.6m).ToString("F2");
                lhrdotpay.Text = (rate / 8 * 3.38m).ToString("F2");
                shpay.Text = (rate / 8 * 1.3m).ToString("F2");
                shotpay.Text = (rate / 8 * 1.69m).ToString("F2");
                shrdpay.Text = (rate / 8 * 1.5m).ToString("F2");
                shrdotpay.Text = (rate / 8 * 1.95m).ToString("F2");
                ndpay.Text = (rate / 8 * 0.1m).ToString("F2");
                ndotpay.Text = (rate / 8 * 1.25m * 0.1m).ToString("F2");
                ndrdpay.Text = (rate / 8 * 1.3m * 0.1m).ToString("F2");
                ndshpay.Text = (rate / 8 * 1.3m * 0.1m).ToString("F2");
                ndshrdpay.Text = (rate / 8 * 1.5m * 0.1m).ToString("F2");
                ndlhpay.Text = (rate / 8 * 2.0m * 0.1m).ToString("F2");
                ndlhrdpay.Text = (rate / 8 * 2.6m * 0.1m).ToString("F2");
            }
            else
            {
                MessageBox.Show("Please enter a valid number in the RateCompute textbox.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
