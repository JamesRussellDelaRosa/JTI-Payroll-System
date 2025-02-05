using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JTI_Payroll_System
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int value, int max, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(value, max, message)));
            }
            else
            {
                progressBar1.Maximum = max;
                progressBar1.Value = value;
                progressLabel.Text = message;
            }
        }

        private void progressLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
