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
                string relieverCondition = chkReliever.Checked ? "reliever = 1" : "reliever = 0";
                string query = $@"
            SELECT 
                id,
                employee_id,
                mname,
                fname,
                lname,
                rate,
                adj_rate,
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
              AND {relieverCondition}
        ";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    dgvDTR.DataSource = dt;

                    // Make grid editable except for id and name columns
                    dgvDTR.ReadOnly = false;
                    if (dgvDTR.Columns["id"] != null) dgvDTR.Columns["id"].ReadOnly = true;
                    if (dgvDTR.Columns["mname"] != null) dgvDTR.Columns["mname"].ReadOnly = true;
                    if (dgvDTR.Columns["fname"] != null) dgvDTR.Columns["fname"].ReadOnly = true;
                    if (dgvDTR.Columns["lname"] != null) dgvDTR.Columns["lname"].ReadOnly = true;

                    // Setup adj_rate ComboBox column
                    SetupAdjRateDropdown();
                }
            }
        }
        private void SetupAdjRateDropdown()
        {
            // Remove existing adj_rate column if present
            if (dgvDTR.Columns.Contains("adj_rate"))
                dgvDTR.Columns.Remove("adj_rate");

            // Fetch rate values from the database
            List<decimal> rateValues = GetRateValuesFromDatabase();
            rateValues.Insert(0, 0.00m);

            // Create ComboBox Column for adj_rate
            DataGridViewComboBoxColumn adjRateColumn = new DataGridViewComboBoxColumn
            {
                Name = "adj_rate",
                HeaderText = "Adj. Rate",
                DataPropertyName = "adj_rate",
                DataSource = rateValues,
                AutoComplete = true,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };

            // Insert after the "rate" column
            int rateColIndex = dgvDTR.Columns["rate"].Index;
            dgvDTR.Columns.Insert(rateColIndex + 1, adjRateColumn);
        }
        private List<decimal> GetRateValuesFromDatabase()
        {
            List<decimal> rates = new List<decimal>();
            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT defaultrate FROM Rate ORDER BY defaultrate ASC";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rates.Add(reader.GetDecimal(0));
                        }
                    }
                }
            }
            return rates;
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
                    if (row.IsNewRow) continue;
                    if (row.Cells["id"].Value == null) continue;
                    int id = Convert.ToInt32(row.Cells["id"].Value);

                    // 1. Update adjustment columns as before, including adj_rate
                    string updateAdjQuery = @"
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
                    adj_ndlhrd = @adj_ndlhrd,
                    adj_rate = @adj_rate
                WHERE id = @id
            ";
                    using (var cmd = new MySqlCommand(updateAdjQuery, conn))
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
                        cmd.Parameters.AddWithValue("@adj_rate", row.Cells["adj_rate"].Value ?? 0);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Fetch the adj_rate for this row
                    decimal rate = Convert.ToDecimal(row.Cells["adj_rate"].Value ?? 0);
                    string rateQuery = "SELECT * FROM rate WHERE defaultrate = @rate ORDER BY id DESC LIMIT 1";
                    decimal basic = 0, rd = 0, rdot = 0, lh = 0, regot = 0, trdy = 0, lhhrs = 0, lhothrs = 0, lhrd = 0, lhrdot = 0;
                    decimal sh = 0, shot = 0, shrd = 0, shrdot = 0, nd = 0, ndot = 0, ndrd = 0, ndsh = 0, ndshrd = 0, ndlh = 0, ndlhrd = 0;
                    using (var rateCmd = new MySqlCommand(rateQuery, conn))
                    {
                        rateCmd.Parameters.AddWithValue("@rate", rate);
                        using (var reader = rateCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                basic = reader.GetDecimal("basic");
                                rd = reader.GetDecimal("rd");
                                rdot = reader.GetDecimal("rdot");
                                lh = reader.GetDecimal("lh");
                                regot = reader.GetDecimal("regot");
                                trdy = reader.GetDecimal("trdy");
                                lhhrs = reader.GetDecimal("lhhrs");
                                lhothrs = reader.GetDecimal("lhothrs");
                                lhrd = reader.GetDecimal("lhrd");
                                lhrdot = reader.GetDecimal("lhrdot");
                                sh = reader.GetDecimal("sh");
                                shot = reader.GetDecimal("shot");
                                shrd = reader.GetDecimal("shrd");
                                shrdot = reader.GetDecimal("shrdot");
                                nd = reader.GetDecimal("nd");
                                ndot = reader.GetDecimal("ndot");
                                ndrd = reader.GetDecimal("ndrd");
                                ndsh = reader.GetDecimal("ndsh");
                                ndshrd = reader.GetDecimal("ndshrd");
                                ndlh = reader.GetDecimal("ndlh");
                                ndlhrd = reader.GetDecimal("ndlhrd");
                            }
                        }
                    }

                    // 3. Compute adjustment pay amounts using adj_rate
                    decimal adj_days = Convert.ToDecimal(row.Cells["adj_days"].Value ?? 0);
                    decimal adj_tdut = Convert.ToDecimal(row.Cells["adj_tdut"].Value ?? 0);
                    decimal adj_lh = Convert.ToDecimal(row.Cells["adj_lh"].Value ?? 0);
                    decimal adj_ot = Convert.ToDecimal(row.Cells["adj_ot"].Value ?? 0);
                    decimal adj_rd = Convert.ToDecimal(row.Cells["adj_rd"].Value ?? 0);
                    decimal adj_rdot = Convert.ToDecimal(row.Cells["adj_rdot"].Value ?? 0);
                    decimal adj_lhhrs = Convert.ToDecimal(row.Cells["adj_lhhrs"].Value ?? 0);
                    decimal adj_lhot = Convert.ToDecimal(row.Cells["adj_lhot"].Value ?? 0);
                    decimal adj_lhrd = Convert.ToDecimal(row.Cells["adj_lhrd"].Value ?? 0);
                    decimal adj_lhrdot = Convert.ToDecimal(row.Cells["adj_lhrdot"].Value ?? 0);
                    decimal adj_sh = Convert.ToDecimal(row.Cells["adj_sh"].Value ?? 0);
                    decimal adj_shot = Convert.ToDecimal(row.Cells["adj_shot"].Value ?? 0);
                    decimal adj_shrd = Convert.ToDecimal(row.Cells["adj_shrd"].Value ?? 0);
                    decimal adj_shrdot = Convert.ToDecimal(row.Cells["adj_shrdot"].Value ?? 0);
                    decimal adj_nd = Convert.ToDecimal(row.Cells["adj_nd"].Value ?? 0);
                    decimal adj_ndot = Convert.ToDecimal(row.Cells["adj_ndot"].Value ?? 0);
                    decimal adj_ndrd = Convert.ToDecimal(row.Cells["adj_ndrd"].Value ?? 0);
                    decimal adj_ndsh = Convert.ToDecimal(row.Cells["adj_ndsh"].Value ?? 0);
                    decimal adj_ndshrd = Convert.ToDecimal(row.Cells["adj_ndshrd"].Value ?? 0);
                    decimal adj_ndlh = Convert.ToDecimal(row.Cells["adj_ndlh"].Value ?? 0);
                    decimal adj_ndlhrd = Convert.ToDecimal(row.Cells["adj_ndlhrd"].Value ?? 0);

                    decimal adj_basicpay = adj_days * basic;
                    decimal adj_rdpay = adj_rd * rd;
                    decimal adj_rdotpay = adj_rdot * rdot;
                    decimal adj_lhpay = adj_lh * lh;
                    decimal adj_regotpay = adj_ot * regot;
                    decimal adj_trdypay = adj_tdut * trdy;
                    decimal adj_lhhrspay = adj_lhhrs * lhhrs;
                    decimal adj_lhothrspay = adj_lhot * lhothrs;
                    decimal adj_lhrdpay = adj_lhrd * lhrd;
                    decimal adj_lhrdotpay = adj_lhrdot * lhrdot;
                    decimal adj_shpay = adj_sh * sh;
                    decimal adj_shotpay = adj_shot * shot;
                    decimal adj_shrdpay = adj_shrd * shrd;
                    decimal adj_shrdotpay = adj_shrdot * shrdot;
                    decimal adj_ndpay = adj_nd * nd;
                    decimal adj_ndotpay = adj_ndot * ndot;
                    decimal adj_ndrdpay = adj_ndrd * ndrd;
                    decimal adj_ndshpay = adj_ndsh * ndsh;
                    decimal adj_ndshrdpay = adj_ndshrd * ndshrd;
                    decimal adj_ndlhpay = adj_ndlh * ndlh;
                    decimal adj_ndlhrdpay = adj_ndlhrd * ndlhrd;

                    decimal adj_totalBasicPay = adj_basicpay + adj_trdypay + adj_lhpay;
                    decimal adj_totalOTPay = adj_rdpay + adj_rdotpay + adj_regotpay + adj_lhhrspay + adj_lhothrspay +
                                             adj_lhrdpay + adj_lhrdotpay + adj_shpay + adj_shotpay + adj_shrdpay + adj_shrdotpay +
                                             adj_ndpay + adj_ndotpay + adj_ndrdpay + adj_ndshpay + adj_ndshrdpay + adj_ndlhpay + adj_ndlhrdpay;
                    decimal adj_grossPay = adj_totalBasicPay + adj_totalOTPay;

                    // 4. Update payroll with adjustment pay fields
                    string updatePayQuery = @"
                UPDATE payroll SET
                    adj_basicpay = @adj_basicpay,
                    adj_rdpay = @adj_rdpay,
                    adj_rdotpay = @adj_rdotpay,
                    adj_lhpay = @adj_lhpay,
                    adj_regotpay = @adj_regotpay,
                    adj_trdypay = @adj_trdypay,
                    adj_lhhrspay = @adj_lhhrspay,
                    adj_lhothrspay = @adj_lhothrspay,
                    adj_lhrdpay = @adj_lhrdpay,
                    adj_lhrdotpay = @adj_lhrdotpay,
                    adj_shpay = @adj_shpay,
                    adj_shotpay = @adj_shotpay,
                    adj_shrdpay = @adj_shrdpay,
                    adj_shrdotpay = @adj_shrdotpay,
                    adj_ndpay = @adj_ndpay,
                    adj_ndotpay = @adj_ndotpay,
                    adj_ndrdpay = @adj_ndrdpay,
                    adj_ndshpay = @adj_ndshpay,
                    adj_ndshrdpay = @adj_ndshrdpay,
                    adj_ndlhpay = @adj_ndlhpay,
                    adj_ndlhrdpay = @adj_ndlhrdpay,
                    adj_total_basic_pay = @adj_totalBasicPay,
                    adj_total_ot_pay = @adj_totalOTPay,
                    adj_gross_pay = @adj_grossPay
                WHERE id = @id
            ";
                    using (var payCmd = new MySqlCommand(updatePayQuery, conn))
                    {
                        payCmd.Parameters.AddWithValue("@adj_basicpay", adj_basicpay);
                        payCmd.Parameters.AddWithValue("@adj_rdpay", adj_rdpay);
                        payCmd.Parameters.AddWithValue("@adj_rdotpay", adj_rdotpay);
                        payCmd.Parameters.AddWithValue("@adj_lhpay", adj_lhpay);
                        payCmd.Parameters.AddWithValue("@adj_regotpay", adj_regotpay);
                        payCmd.Parameters.AddWithValue("@adj_trdypay", adj_trdypay);
                        payCmd.Parameters.AddWithValue("@adj_lhhrspay", adj_lhhrspay);
                        payCmd.Parameters.AddWithValue("@adj_lhothrspay", adj_lhothrspay);
                        payCmd.Parameters.AddWithValue("@adj_lhrdpay", adj_lhrdpay);
                        payCmd.Parameters.AddWithValue("@adj_lhrdotpay", adj_lhrdotpay);
                        payCmd.Parameters.AddWithValue("@adj_shpay", adj_shpay);
                        payCmd.Parameters.AddWithValue("@adj_shotpay", adj_shotpay);
                        payCmd.Parameters.AddWithValue("@adj_shrdpay", adj_shrdpay);
                        payCmd.Parameters.AddWithValue("@adj_shrdotpay", adj_shrdotpay);
                        payCmd.Parameters.AddWithValue("@adj_ndpay", adj_ndpay);
                        payCmd.Parameters.AddWithValue("@adj_ndotpay", adj_ndotpay);
                        payCmd.Parameters.AddWithValue("@adj_ndrdpay", adj_ndrdpay);
                        payCmd.Parameters.AddWithValue("@adj_ndshpay", adj_ndshpay);
                        payCmd.Parameters.AddWithValue("@adj_ndshrdpay", adj_ndshrdpay);
                        payCmd.Parameters.AddWithValue("@adj_ndlhpay", adj_ndlhpay);
                        payCmd.Parameters.AddWithValue("@adj_ndlhrdpay", adj_ndlhrdpay);
                        payCmd.Parameters.AddWithValue("@adj_totalBasicPay", adj_totalBasicPay);
                        payCmd.Parameters.AddWithValue("@adj_totalOTPay", adj_totalOTPay);
                        payCmd.Parameters.AddWithValue("@adj_grossPay", adj_grossPay);
                        payCmd.Parameters.AddWithValue("@id", id);
                        payCmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Adjustments have been posted and calculated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}