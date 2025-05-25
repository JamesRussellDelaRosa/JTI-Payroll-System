using MySql.Data.MySqlClient;
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
    public partial class PayrollAdj : Form
    {
        private Panel selectedPanel = null;

        public PayrollAdj()
        {
            InitializeComponent();

            // Add event handlers for placeholder text
            textStartDate.Enter += TextBox_Enter;
            textStartDate.Leave += TextBox_Leave;
            textStartDate.TextChanged += TextBox_TextChanged;
            textStartDate.KeyPress += AutoFormatDate;

            textEndDate.Enter += TextBox_Enter;
            textEndDate.Leave += TextBox_Leave;
            textEndDate.KeyPress += AutoFormatDate;

            // Add Paint event handlers for custom drawing
            textStartDate.Paint += TextBox_Paint;
            textEndDate.Paint += TextBox_Paint;

            // Set initial placeholder text
            SetPlaceholderText(textStartDate, "MM/DD/YYYY");
            SetPlaceholderText(textEndDate, "MM/DD/YYYY");
        }
        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "MM/DD/YYYY")
            {
                textBox.Text = "";
                textBox.ForeColor = SystemColors.WindowText;
            }
            textBox.Invalidate();
        }
        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholderText(textBox, "MM/DD/YYYY");
            }
            textBox.Invalidate();
        }
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Invalidate();
        }
        private void SetPlaceholderText(TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.ForeColor = SystemColors.GrayText;
        }
        private void AutoFormatDate(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (!char.IsControl(e.KeyChar))
            {
                int length = textBox.Text.Length;

                if (length == 2 || length == 5)
                {
                    textBox.Text += "/";
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }
        private void TextBox_Paint(object sender, PaintEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                using (Brush brush = new SolidBrush(SystemColors.GrayText))
                {
                    e.Graphics.DrawString("MM/DD/YYYY", textBox.Font, brush, new PointF(0, 0));
                }
            }
        }
        private void filter_Click(object sender, EventArgs e)
        {
            employees.Controls.Clear();
            if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
            {
                MessageBox.Show("Please enter valid start and end dates.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            LoadEmployeesByDateRange(startDate, endDate);
        }
        private void LoadEmployeesByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT employee_id, lname, fname, mname, ccode, pay_period_start, pay_period_end FROM payroll WHERE pay_period_start = @start AND pay_period_end = @end GROUP BY employee_id, lname, fname, mname, ccode, pay_period_start, pay_period_end ORDER BY lname, fname, mname";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", endDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Panel panel = new Panel
                                {
                                    Height = 32,
                                    Width = employees.Width - 25,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(2),
                                    BackColor = Color.White
                                };
                                int x = 5;
                                int[] widths = { 80, 100, 100, 80, 60, 110, 110 };
                                string[] values = {
                                    reader["employee_id"].ToString(),
                                    reader["lname"].ToString(),
                                    reader["fname"].ToString(),
                                    reader["mname"].ToString(),
                                    reader["ccode"].ToString(),
                                    Convert.ToDateTime(reader["pay_period_start"]).ToString("MM/dd/yyyy"),
                                    Convert.ToDateTime(reader["pay_period_end"]).ToString("MM/dd/yyyy")
                                };
                                for (int i = 0; i < values.Length; i++)
                                {
                                    Label lbl = new Label
                                    {
                                        Text = values[i],
                                        Location = new Point(x, 7),
                                        Width = widths[i],
                                        AutoSize = false,
                                        BackColor = Color.Transparent
                                    };
                                    lbl.Click += (s, e) => EmployeePanel_Click(panel, e); // Make label clickable
                                    panel.Controls.Add(lbl);
                                    x += widths[i] + 5;
                                }
                                panel.Tag = new {
                                    employee_id = reader["employee_id"].ToString(),
                                    lname = reader["lname"].ToString(),
                                    fname = reader["fname"].ToString(),
                                    mname = reader["mname"].ToString(),
                                    ccode = reader["ccode"].ToString(),
                                    pay_period_start = Convert.ToDateTime(reader["pay_period_start"]),
                                    pay_period_end = Convert.ToDateTime(reader["pay_period_end"])
                                };
                                panel.Cursor = Cursors.Hand;
                                panel.Click += EmployeePanel_Click;
                                employees.Controls.Add(panel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EmployeePanel_Click(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null && sender is Label lbl && lbl.Parent is Panel p) panel = p;
            if (panel?.Tag == null) return;
            // Highlight selection
            if (selectedPanel != null) selectedPanel.BackColor = Color.White;
            panel.BackColor = Color.LightBlue;
            selectedPanel = panel;
            dynamic tag = panel.Tag;
            var adjForm = new adjustment_input(
                tag.employee_id,
                tag.lname,
                tag.fname,
                tag.mname,
                tag.ccode,
                tag.pay_period_start,
                tag.pay_period_end
            );
            adjForm.ShowDialog();
            // Optionally, remove highlight after closing
            panel.BackColor = Color.White;
            selectedPanel = null;
        }
    }
}