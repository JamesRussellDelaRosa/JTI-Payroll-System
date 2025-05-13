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
        public PayrollAdj()
        {
            InitializeComponent();

            // Add event handlers for placeholder text
            textStartDate.Enter += TextBox_Enter;
            textStartDate.Leave += TextBox_Leave;
            textStartDate.TextChanged += TextBox_TextChanged; // Add this line
            textStartDate.KeyPress += AutoFormatDate; // Attach AutoFormatDate

            textEndDate.Enter += TextBox_Enter;
            textEndDate.Leave += TextBox_Leave;
            textEndDate.KeyPress += AutoFormatDate; // Attach AutoFormatDate

            // Add Paint event handlers for custom drawing
            textStartDate.Paint += TextBox_Paint;
            textEndDate.Paint += TextBox_Paint;

            // Set initial placeholder text
            SetPlaceholderText(textStartDate, "MM/DD/YYYY");
            SetPlaceholderText(textEndDate, "MM/DD/YYYY");
        }

        private void LoadPayrollAdjustments()
        {
            if (!DateTime.TryParse(textStartDate.Text, out DateTime startDate) ||
                !DateTime.TryParse(textEndDate.Text, out DateTime endDate))
            {
                MessageBox.Show("Please enter valid start and end dates (YYYY-MM-DD).");
                return;
            }

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT 
                employee_id,
                mname,
                fname,
                lname,
                adj_days,
                adj_tdut,
                adj_lh,
                adj_ot,
                adj_rd,
                adj_rdot,
                adj_lhhrs,
                adj_lhot,
                adj_lhrd,
                adj_lhrdot,
                adj_sh,
                adj_shot,
                adj_shrd,
                adj_shrdot,
                adj_nd,
                adj_ndot,
                adj_ndrd,
                adj_ndsh,
                adj_ndshrd,
                adj_ndlh,
                adj_ndlhrd
            FROM payroll
            WHERE pay_period_start >= @startDate 
              AND pay_period_end <= @endDate
              AND reliever = 0
        ";

                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    var adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    dgvDTR.DataSource = dt;

                    // Make grid editable except for id and name columns
                    dgvDTR.ReadOnly = false;
                    if (dgvDTR.Columns["id"] != null) dgvDTR.Columns["id"].ReadOnly = true;
                    if (dgvDTR.Columns["mname"] != null) dgvDTR.Columns["mname"].ReadOnly = true;
                    if (dgvDTR.Columns["fname"] != null) dgvDTR.Columns["fname"].ReadOnly = true;
                    if (dgvDTR.Columns["lname"] != null) dgvDTR.Columns["lname"].ReadOnly = true;
                }
            }
        }
        private void filter_Click(object sender, EventArgs e)
        {
            LoadPayrollAdjustments();
        }
        private void refresh_Click(object sender, EventArgs e)
        {
            LoadPayrollAdjustments();
        }
        private void post_Click(object sender, EventArgs e)
        {
            if (dgvDTR.DataSource == null)
            {
                MessageBox.Show("No data to post.");
                return;
            }

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                foreach (DataGridViewRow row in dgvDTR.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the new row placeholder

                    // Get the id for the current row
                    if (row.Cells["id"].Value == null) continue;
                    int id = Convert.ToInt32(row.Cells["id"].Value);

                    // Prepare the update command
                    string updateQuery = @"
                UPDATE payroll SET
                    adj_days = @adj_days,
                    adj_tdut = @adj_tdut,
                    adj_lh = @adj_lh,
                    adj_ot = @adj_ot,
                    adj_rd = @adj_rd,
                    adj_rdot = @adj_rdot,
                    adj_lhhrs = @adj_lhhrs,
                    adj_lhot = @adj_lhot,
                    adj_lhrd = @adj_lhrd,
                    adj_lhrdot = @adj_lhrdot,
                    adj_sh = @adj_sh,
                    adj_shot = @adj_shot,
                    adj_shrd = @adj_shrd,
                    adj_shrdot = @adj_shrdot,
                    adj_nd = @adj_nd,
                    adj_ndot = @adj_ndot,
                    adj_ndrd = @adj_ndrd,
                    adj_ndsh = @adj_ndsh,
                    adj_ndshrd = @adj_ndshrd,
                    adj_ndlh = @adj_ndlh,
                    adj_ndlhrd = @adj_ndlhrd
                WHERE id = @id
            ";

                    using (var cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@adj_days", row.Cells["adj_days"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_tdut", row.Cells["adj_tdut"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_lh", row.Cells["adj_lh"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ot", row.Cells["adj_ot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_rd", row.Cells["adj_rd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_rdot", row.Cells["adj_rdot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_lhhrs", row.Cells["adj_lhhrs"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_lhot", row.Cells["adj_lhot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_lhrd", row.Cells["adj_lhrd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_lhrdot", row.Cells["adj_lhrdot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_sh", row.Cells["adj_sh"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_shot", row.Cells["adj_shot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_shrd", row.Cells["adj_shrd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_shrdot", row.Cells["adj_shrdot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_nd", row.Cells["adj_nd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ndot", row.Cells["adj_ndot"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ndrd", row.Cells["adj_ndrd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ndsh", row.Cells["adj_ndsh"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ndshrd", row.Cells["adj_ndshrd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ndlh", row.Cells["adj_ndlh"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@adj_ndlhrd", row.Cells["adj_ndlhrd"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Adjustments have been posted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "MM/DD/YYYY")
            {
                textBox.Text = "";
                textBox.ForeColor = SystemColors.WindowText;
            }
            textBox.Invalidate(); // Force repaint to remove placeholder text
        }
        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholderText(textBox, "MM/DD/YYYY");
            }
            textBox.Invalidate(); // Force repaint to show placeholder text
        }
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Invalidate(); // Force repaint to show or hide placeholder text
        }
        private void SetPlaceholderText(TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.ForeColor = SystemColors.GrayText;
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
    }
}
