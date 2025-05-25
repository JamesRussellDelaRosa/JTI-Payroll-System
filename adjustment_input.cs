using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JTI_Payroll_System
{
    public partial class adjustment_input : Form
    {
        private string employee_id, lname, fname, mname, ccode;
        private DateTime pay_period_start, pay_period_end;
        private int month, payrollyear, control_period;
        private DataGridView dgv;
        private Button btnAdd;
        private Button btnAddRate;
        private Button btnDeleteRow;
        private Button btnRefresh;

        public adjustment_input(string employee_id, string lname, string fname, string mname, string ccode, DateTime pay_period_start, DateTime pay_period_end)
        {
            this.employee_id = employee_id;
            this.lname = lname;
            this.fname = fname;
            this.mname = mname;
            this.ccode = ccode;
            this.pay_period_start = pay_period_start;
            this.pay_period_end = pay_period_end;
            this.Text = $"Adjustments for {lname}, {fname} ({employee_id})";
            this.Size = new Size(950, 480);
            LoadPayrollInfo();
            SetupGrid();
            SetupAddButton();
        }

        private void LoadPayrollInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string q = @"SELECT month, payrollyear, control_period, rate, reliever FROM payroll WHERE employee_id=@id AND pay_period_start=@start AND pay_period_end=@end LIMIT 1";
                using (var cmd = new MySqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@id", employee_id);
                    cmd.Parameters.AddWithValue("@start", pay_period_start);
                    cmd.Parameters.AddWithValue("@end", pay_period_end);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            month = Convert.ToInt32(r["month"]);
                            payrollyear = Convert.ToInt32(r["payrollyear"]);
                            control_period = Convert.ToInt32(r["control_period"]);
                        }
                    }
                }
            }
        }

        private List<decimal> GetRateValuesFromDatabase()
        {
            var rates = new List<decimal>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string q = "SELECT DISTINCT defaultrate FROM rate ORDER BY defaultrate DESC";
                using (var cmd = new MySqlCommand(q, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        if (decimal.TryParse(r["defaultrate"].ToString(), out decimal rate))
                            rates.Add(rate);
                    }
                }
            }
            return rates;
        }

        private void SetupGrid()
        {
            dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(900, 350),
                AllowUserToAddRows = false, // Prevent empty last row
                AllowUserToDeleteRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };
            Controls.Add(dgv);

            dgv.Columns.Add("non_working_day_count", "Non-Working Day Count");

            var rateValues = GetRateValuesFromDatabase();
            rateValues.Insert(0, 0.00m);
            DataGridViewComboBoxColumn rateColumn = new DataGridViewComboBoxColumn
            {
                Name = "rate",
                HeaderText = "Rate",
                DataPropertyName = "rate",
                DataSource = new List<decimal>(rateValues),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            dgv.Columns.Add(rateColumn);

            dgv.Columns.Add("reliever", "Reliever");
            dgv.Columns.Add("working_hours", "Working Hours");
            dgv.Columns.Add("total_days", "Total Days");
            dgv.Columns.Add("basicpay", "Basic Pay");
            dgv.Columns.Add("td_ut", "Tardiness/Undertime");
            dgv.Columns.Add("trdypay", "Tardy Pay");
            dgv.Columns.Add("legal_holiday_count", "Legal Holiday Count");
            dgv.Columns.Add("lhpay", "Legal Holiday Pay");
            dgv.Columns.Add("overtime_hours", "Overtime Hours");
            dgv.Columns.Add("regotpay", "Regular OT Pay");
            dgv.Columns.Add("restday_hours", "Restday Hours");
            dgv.Columns.Add("rdpay", "Restday Pay");
            dgv.Columns.Add("restday_overtime_hours", "Restday OT Hours");
            dgv.Columns.Add("rdotpay", "Restday OT Pay");
            dgv.Columns.Add("legal_holiday_hours", "Legal Holiday Hours");
            dgv.Columns.Add("lhhrspay", "Legal Holiday Hours Pay");
            dgv.Columns.Add("legal_holiday_overtime_hours", "Legal Holiday OT Hours");
            dgv.Columns.Add("lhothrspay", "Legal Holiday OT Pay");
            dgv.Columns.Add("lhrd_hours", "Legal Holiday Restday Hours");
            dgv.Columns.Add("lhrdpay", "Legal Holiday Restday Pay");
            dgv.Columns.Add("lhrd_overtime_hours", "Legal Holiday Restday OT Hours");
            dgv.Columns.Add("lhrdotpay", "Legal Holiday Restday OT Pay");
            dgv.Columns.Add("special_holiday_hours", "Special Holiday Hours");
            dgv.Columns.Add("shpay", "Special Holiday Pay");
            dgv.Columns.Add("special_holiday_overtime_hours", "Special Holiday OT Hours");
            dgv.Columns.Add("shotpay", "Special Holiday OT Pay");
            dgv.Columns.Add("special_holiday_restday_hours", "Special Holiday Restday Hours");
            dgv.Columns.Add("shrdpay", "Special Holiday Restday Pay");
            dgv.Columns.Add("special_holiday_restday_overtime_hours", "Special Holiday Restday OT Hours");
            dgv.Columns.Add("shrdotpay", "Special Holiday Restday OT Pay");
            dgv.Columns.Add("nd_hrs", "Night Diff Hours");
            dgv.Columns.Add("ndpay", "Night Diff Pay");
            dgv.Columns.Add("ndot_hrs", "Night Diff OT Hours");
            dgv.Columns.Add("ndotpay", "Night Diff OT Pay");
            dgv.Columns.Add("ndrd_hrs", "Night Diff Restday Hours");
            dgv.Columns.Add("ndrdpay", "Night Diff Restday Pay");
            dgv.Columns.Add("ndsh_hrs", "Night Diff SH Hours");
            dgv.Columns.Add("ndshpay", "Night Diff SH Pay");
            dgv.Columns.Add("ndshrd_hrs", "Night Diff SH Restday Hours");
            dgv.Columns.Add("ndshrdpay", "Night Diff SH Restday Pay");
            dgv.Columns.Add("ndlh_hrs", "Night Diff LH Hours");
            dgv.Columns.Add("ndlhpay", "Night Diff LH Pay");
            dgv.Columns.Add("ndlhrd_hrs", "Night Diff LH Restday Hours");
            dgv.Columns.Add("ndlhrdpay", "Night Diff LH Restday Pay");
            dgv.Columns.Add("ndrdot_hrs", "Night Diff Restday OT Hours");
            dgv.Columns.Add("ndshot_hrs", "Night Diff SH OT Hours");
            dgv.Columns.Add("ndshrdot_hrs", "Night Diff SH Restday OT Hours");
            dgv.Columns.Add("ndlhot_hrs", "Night Diff LH OT Hours");
            dgv.Columns.Add("ndlhrdot_hrs", "Night Diff LH Restday OT Hours");
            dgv.Columns.Add("total_earnings", "Total Earnings");
            dgv.Columns.Add("total_basic_pay", "Total Basic Pay");
            dgv.Columns.Add("total_ot_pay", "Total OT Pay");
            dgv.Columns.Add("gross_pay", "Gross Pay");

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string q = @"SELECT * FROM payroll_adjustment WHERE employee_id=@id AND pay_period_start=@start AND pay_period_end=@end AND month=@month AND payrollyear=@payrollyear AND control_period=@control_period";
                using (var cmd = new MySqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@id", employee_id);
                    cmd.Parameters.AddWithValue("@start", pay_period_start);
                    cmd.Parameters.AddWithValue("@end", pay_period_end);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                    cmd.Parameters.AddWithValue("@control_period", control_period);
                    using (var r = cmd.ExecuteReader())
                    {
                        bool hasRows = false;
                        while (r.Read())
                        {
                            hasRows = true;
                            object[] values = new object[dgv.Columns.Count];
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                if (dgv.Columns[i].Name == "rate")
                                {
                                    decimal dbRate = 0.00m;
                                    decimal.TryParse(r["rate"].ToString(), out dbRate);
                                    var combo = (DataGridViewComboBoxColumn)dgv.Columns["rate"];
                                    var comboList = (List<decimal>)combo.DataSource;
                                    if (!comboList.Contains(dbRate))
                                        comboList.Add(dbRate);
                                    values[i] = dbRate;
                                }
                                else
                                {
                                    values[i] = r[dgv.Columns[i].Name];
                                }
                            }
                            dgv.Rows.Add(values);
                        }
                    }
                }
            }

            dgv.EditingControlShowing += (sender, e) =>
            {
                if (dgv.CurrentCell != null && dgv.CurrentCell.ColumnIndex == dgv.Columns["rate"].Index)
                {
                    if (e.Control is ComboBox comboBox)
                    {
                        comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                        comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                    }
                }
            };

            dgv.DataError += (sender, e) =>
            {
                e.ThrowException = false;
            };
        }

        private void SetupAddButton()
        {
            btnAdd = new Button
            {
                Text = "Add Adjustment",
                Location = new Point(10, 370),
                Size = new Size(150, 35)
            };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            btnAddRate = new Button
            {
                Text = "Add Rate Row",
                Location = new Point(170, 370),
                Size = new Size(150, 35)
            };
            btnAddRate.Click += BtnAddRate_Click;
            Controls.Add(btnAddRate);

            btnDeleteRow = new Button
            {
                Text = "Delete Row",
                Location = new Point(330, 370),
                Size = new Size(150, 35)
            };
            btnDeleteRow.Click += BtnDeleteRow_Click;
            Controls.Add(btnDeleteRow);

            btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(490, 370),
                Size = new Size(150, 35)
            };
            btnRefresh.Click += BtnRefresh_Click;
            Controls.Add(btnRefresh);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadPayrollInfo();
            SetupGrid();
        }

        private void CalculateAdjustmentAmounts()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    // Get rate value
                    if (!decimal.TryParse(row.Cells["rate"].Value?.ToString(), out decimal rate))
                        continue;
                    // Get rate config from rate table
                    string rateQuery = @"SELECT * FROM rate WHERE defaultrate = @rate ORDER BY id DESC LIMIT 1";
                    using (var rateCmd = new MySqlCommand(rateQuery, conn))
                    {
                        rateCmd.Parameters.AddWithValue("@rate", rate);
                        using (var rateReader = rateCmd.ExecuteReader())
                        {
                            if (rateReader.Read())
                            {
                                // Get config values
                                decimal basic = rateReader.GetDecimal("basic");
                                decimal rd = rateReader.GetDecimal("rd");
                                decimal rdot = rateReader.GetDecimal("rdot");
                                decimal lh = rateReader.GetDecimal("lh");
                                decimal regot = rateReader.GetDecimal("regot");
                                decimal trdy = rateReader.GetDecimal("trdy");
                                decimal lhhrs = rateReader.GetDecimal("lhhrs");
                                decimal lhothrs = rateReader.GetDecimal("lhothrs");
                                decimal lhrd = rateReader.GetDecimal("lhrd");
                                decimal lhrdot = rateReader.GetDecimal("lhrdot");
                                decimal sh = rateReader.GetDecimal("sh");
                                decimal shot = rateReader.GetDecimal("shot");
                                decimal shrd = rateReader.GetDecimal("shrd");
                                decimal shrdot = rateReader.GetDecimal("shrdot");
                                decimal nd = rateReader.GetDecimal("nd");
                                decimal ndot = rateReader.GetDecimal("ndot");
                                decimal ndrd = rateReader.GetDecimal("ndrd");
                                decimal ndsh = rateReader.GetDecimal("ndsh");
                                decimal ndshrd = rateReader.GetDecimal("ndshrd");
                                decimal ndlh = rateReader.GetDecimal("ndlh");
                                decimal ndlhrd = rateReader.GetDecimal("ndlhrd");
                                rateReader.Close();
                                // Get adjustment row values
                                decimal total_days = Convert.ToDecimal(row.Cells["total_days"].Value ?? 0);
                                decimal restday_hours = Convert.ToDecimal(row.Cells["restday_hours"].Value ?? 0);
                                decimal restday_overtime_hours = Convert.ToDecimal(row.Cells["restday_overtime_hours"].Value ?? 0);
                                int legal_holiday_count = Convert.ToInt32(row.Cells["legal_holiday_count"].Value ?? 0);
                                decimal overtime_hours = Convert.ToDecimal(row.Cells["overtime_hours"].Value ?? 0);
                                decimal td_ut = Convert.ToDecimal(row.Cells["td_ut"].Value ?? 0);
                                decimal legal_holiday_hours = Convert.ToDecimal(row.Cells["legal_holiday_hours"].Value ?? 0);
                                decimal legal_holiday_overtime_hours = Convert.ToDecimal(row.Cells["legal_holiday_overtime_hours"].Value ?? 0);
                                decimal lhrd_hours = Convert.ToDecimal(row.Cells["lhrd_hours"].Value ?? 0);
                                decimal lhrd_overtime_hours = Convert.ToDecimal(row.Cells["lhrd_overtime_hours"].Value ?? 0);
                                decimal special_holiday_hours = Convert.ToDecimal(row.Cells["special_holiday_hours"].Value ?? 0);
                                decimal special_holiday_overtime_hours = Convert.ToDecimal(row.Cells["special_holiday_overtime_hours"].Value ?? 0);
                                decimal special_holiday_restday_hours = Convert.ToDecimal(row.Cells["special_holiday_restday_hours"].Value ?? 0);
                                decimal special_holiday_restday_overtime_hours = Convert.ToDecimal(row.Cells["special_holiday_restday_overtime_hours"].Value ?? 0);
                                decimal nd_hrs = Convert.ToDecimal(row.Cells["nd_hrs"].Value ?? 0);
                                decimal ndot_hrs = Convert.ToDecimal(row.Cells["ndot_hrs"].Value ?? 0);
                                decimal ndrd_hrs = Convert.ToDecimal(row.Cells["ndrd_hrs"].Value ?? 0);
                                decimal ndsh_hrs = Convert.ToDecimal(row.Cells["ndsh_hrs"].Value ?? 0);
                                decimal ndshrd_hrs = Convert.ToDecimal(row.Cells["ndshrd_hrs"].Value ?? 0);
                                decimal ndlh_hrs = Convert.ToDecimal(row.Cells["ndlh_hrs"].Value ?? 0);
                                decimal ndlhrd_hrs = Convert.ToDecimal(row.Cells["ndlhrd_hrs"].Value ?? 0);
                                // Compute
                                decimal basicpay = total_days * basic;
                                decimal rdpay = restday_hours * rd;
                                decimal rdotpay = rdot * restday_overtime_hours;
                                decimal lhpay = lh * legal_holiday_count;
                                decimal regotpay = regot * overtime_hours;
                                decimal trdypay = trdy * td_ut;
                                decimal lhhrspay = lhhrs * legal_holiday_hours;
                                decimal lhothrspay = lhothrs * legal_holiday_overtime_hours;
                                decimal lhrdpay = lhrd * lhrd_hours;
                                decimal lhrdotpay = lhrdot * lhrd_overtime_hours;
                                decimal shpay = sh * special_holiday_hours;
                                decimal shotpay = shot * special_holiday_overtime_hours;
                                decimal shrdpay = shrd * special_holiday_restday_hours;
                                decimal shrdotpay = shrdot * special_holiday_restday_overtime_hours;
                                decimal ndpay = nd * nd_hrs;
                                decimal ndotpay = ndot * ndot_hrs;
                                decimal ndrdpay = ndrd * ndrd_hrs;
                                decimal ndshpay = ndsh * ndsh_hrs;
                                decimal ndshrdpay = ndshrd * ndshrd_hrs;
                                decimal ndlhpay = ndlh * ndlh_hrs;
                                decimal ndlhrdpay = ndlhrd * ndlhrd_hrs;
                                decimal totalBasicPay = basicpay + trdypay + lhpay;
                                decimal totalOTPay = rdpay + rdotpay + regotpay + lhhrspay + lhothrspay +
                                                     lhrdpay + lhrdotpay + shpay + shotpay + shrdpay + shrdotpay +
                                                     ndpay + ndotpay + ndrdpay + ndshpay + ndshrdpay + ndlhpay + ndlhrdpay;
                                decimal grossPay = totalBasicPay + totalOTPay;
                                // Set computed values in grid
                                row.Cells["basicpay"].Value = basicpay;
                                row.Cells["rdpay"].Value = rdpay;
                                row.Cells["rdotpay"].Value = rdotpay;
                                row.Cells["lhpay"].Value = lhpay;
                                row.Cells["regotpay"].Value = regotpay;
                                row.Cells["trdypay"].Value = trdypay;
                                row.Cells["lhhrspay"].Value = lhhrspay;
                                row.Cells["lhothrspay"].Value = lhothrspay;
                                row.Cells["lhrdpay"].Value = lhrdpay;
                                row.Cells["lhrdotpay"].Value = lhrdotpay;
                                row.Cells["shpay"].Value = shpay;
                                row.Cells["shotpay"].Value = shotpay;
                                row.Cells["shrdpay"].Value = shrdpay;
                                row.Cells["shrdotpay"].Value = shrdotpay;
                                row.Cells["ndpay"].Value = ndpay;
                                row.Cells["ndotpay"].Value = ndotpay;
                                row.Cells["ndrdpay"].Value = ndrdpay;
                                row.Cells["ndshpay"].Value = ndshpay;
                                row.Cells["ndshrdpay"].Value = ndshrdpay;
                                row.Cells["ndlhpay"].Value = ndlhpay;
                                row.Cells["ndlhrdpay"].Value = ndlhrdpay;
                                row.Cells["total_basic_pay"].Value = totalBasicPay;
                                row.Cells["total_ot_pay"].Value = totalOTPay;
                                row.Cells["gross_pay"].Value = grossPay;
                            }
                        }
                    }
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0)
                dgv.Rows.Add();
            // Calculate adjustment amounts before saving
            CalculateAdjustmentAmounts();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string insert = @"REPLACE INTO payroll_adjustment (
                            employee_id, lname, fname, mname, ccode, pay_period_start, pay_period_end, month, payrollyear, control_period,
                            non_working_day_count, rate, reliever, working_hours, total_days, basicpay, td_ut, trdypay, legal_holiday_count, lhpay, overtime_hours, regotpay, restday_hours, rdpay, restday_overtime_hours, rdotpay, legal_holiday_hours, lhhrspay, legal_holiday_overtime_hours, lhothrspay, lhrd_hours, lhrdpay, lhrd_overtime_hours, lhrdotpay, special_holiday_hours, shpay, special_holiday_overtime_hours, shotpay, special_holiday_restday_hours, shrdpay, special_holiday_restday_overtime_hours, shrdotpay, nd_hrs, ndpay, ndot_hrs, ndotpay, ndrd_hrs, ndrdpay, ndsh_hrs, ndshpay, ndshrd_hrs, ndshrdpay, ndlh_hrs, ndlhpay, ndlhrd_hrs, ndlhrdpay, ndrdot_hrs, ndshot_hrs, ndshrdot_hrs, ndlhot_hrs, ndlhrdot_hrs, total_earnings, total_basic_pay, total_ot_pay, gross_pay
                        ) VALUES (
                            @employee_id, @lname, @fname, @mname, @ccode, @pay_period_start, @pay_period_end, @month, @payrollyear, @control_period,
                            @non_working_day_count, @rate, @reliever, @working_hours, @total_days, @basicpay, @td_ut, @trdypay, @legal_holiday_count, @lhpay, @overtime_hours, @regotpay, @restday_hours, @rdpay, @restday_overtime_hours, @rdotpay, @legal_holiday_hours, @lhhrspay, @legal_holiday_overtime_hours, @lhothrspay, @lhrd_hours, @lhrdpay, @lhrd_overtime_hours, @lhrdotpay, @special_holiday_hours, @shpay, @special_holiday_overtime_hours, @shotpay, @special_holiday_restday_hours, @shrdpay, @special_holiday_restday_overtime_hours, @shrdotpay, @nd_hrs, @ndpay, @ndot_hrs, @ndotpay, @ndrd_hrs, @ndrdpay, @ndsh_hrs, @ndshpay, @ndshrd_hrs, @ndshrdpay, @ndlh_hrs, @ndlhpay, @ndlhrd_hrs, @ndlhrdpay, @ndrdot_hrs, @ndshot_hrs, @ndshrdot_hrs, @ndlhot_hrs, @ndlhrdot_hrs, @total_earnings, @total_basic_pay, @total_ot_pay, @gross_pay
                        )";
                        using (var cmd = new MySqlCommand(insert, conn))
                        {
                            cmd.Parameters.AddWithValue("@employee_id", employee_id);
                            cmd.Parameters.AddWithValue("@lname", lname);
                            cmd.Parameters.AddWithValue("@fname", fname);
                            cmd.Parameters.AddWithValue("@mname", mname);
                            cmd.Parameters.AddWithValue("@ccode", ccode);
                            cmd.Parameters.AddWithValue("@pay_period_start", pay_period_start);
                            cmd.Parameters.AddWithValue("@pay_period_end", pay_period_end);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                            cmd.Parameters.AddWithValue("@control_period", control_period);
                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                var val = row.Cells[col.Index].Value ?? 0;
                                cmd.Parameters.AddWithValue($"@{col.Name}", val);
                            }
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("Adjustments saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Reload the DataGridView instead of closing the form
                SetupGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving adjustment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddRate_Click(object sender, EventArgs e)
        {
            dgv.Rows.Add();
        }

        private void BtnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow != null && !dgv.CurrentRow.IsNewRow)
            {
                // Optionally, delete from database if the row exists there
                var row = dgv.CurrentRow;
                if (row.Cells["rate"].Value != null && row.Cells["rate"].Value.ToString() != "")
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string del = @"DELETE FROM payroll_adjustment WHERE employee_id=@id AND pay_period_start=@start AND pay_period_end=@end AND month=@month AND payrollyear=@payrollyear AND control_period=@control_period AND rate=@rate AND reliever=@reliever";
                        using (var cmd = new MySqlCommand(del, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", employee_id);
                            cmd.Parameters.AddWithValue("@start", pay_period_start);
                            cmd.Parameters.AddWithValue("@end", pay_period_end);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@payrollyear", payrollyear);
                            cmd.Parameters.AddWithValue("@control_period", control_period);
                            cmd.Parameters.AddWithValue("@rate", row.Cells["rate"].Value);
                            cmd.Parameters.AddWithValue("@reliever", row.Cells["reliever"].Value ?? 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                dgv.Rows.Remove(row);
            }
        }
    }
}
