using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using System.Reflection.Emit;

namespace JTI_Payroll_System
{
    public partial class employee : Form
    {
        private void SetControlsReadOnly(bool isReadOnly)
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox tb)
                    tb.ReadOnly = isReadOnly;
                else if (c is ComboBox cb)
                    cb.Enabled = !isReadOnly;
                else if (c is CheckBox chk)
                    chk.Enabled = !isReadOnly;
            }
        }
        private bool isNewEmployee = false;
        private List<string> employeeIdList = new();
        private int currentEmployeeIndex = 0;

        public employee()
        {
            InitializeComponent();
            SetControlsReadOnly(true);
            this.Load += employee_Load;
            cancel.Enabled = false;
            dt_birth.KeyPress += AutoFormatDate;
            dt_issued.KeyPress += AutoFormatDate;
            cont_date.KeyPress += AutoFormatDate;
            cont_end.KeyPress += AutoFormatDate;
            dt_expired.KeyPress += AutoFormatDate;

            title_code.Items.Clear();
            title_code.Items.AddRange(new string[] { "Mr", "Ms", "Mrs" });
            edu_attaint.Items.Clear();
            edu_attaint.Items.AddRange(new string[] {
                "COLLEGE GRADUATE",
                "COLLEGE LEVEL",
                "HIGH SCHOOL GRADE",
                "SENIOR HIGH GRADE",
                "VOCATIONAL GRADE"
            });
            bir_cd.Items.Clear();
            bir_cd.Items.AddRange(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" });

        }

        private void employee_Load(object sender, EventArgs e)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT id_no FROM employee ORDER BY fileno ASC";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    employeeIdList.Clear();
                    while (reader.Read())
                    {
                        employeeIdList.Add(reader["id_no"].ToString());
                    }
                }
            }

            if (employeeIdList.Count > 0)
            {
                currentEmployeeIndex = 0;
                LoadEmployeeData(employeeIdList[currentEmployeeIndex]);
            }
        }

        private void saveEmp_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("You are not allowed to save employee records.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(id_no.Text))
            {
                MessageBox.Show("Please enter an Employee ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query;

                if (isNewEmployee)
                {
                    query = @"
                INSERT INTO employee (
                    id_no, fname, mname, lname, sex, dt_birth, civil_stat, sssnum, tin, hdmfnum, phnum, bir_cd, bir_stat, acct_no, atm_card_no, dt_issued, enable_atm, atm_status, ccode, client, dep_code, department, line_cd, line, cont_date, cont_end, rate_month, rate_day, cont_rate, meal_rate, allowance, position, sil_amt, street, barangay, city, province, title_code, mfname, mmname, mlname, spou_name, edu_attaint, dt_expired, contact_no, zipcode, staff, active, active_hmo, active_sil
                ) VALUES (
                    @id_no, @fname, @mname, @lname, @sex, @dt_birth, @civil_stat, @sssnum, @tin, @hdmfnum, @phnum, @bir_cd, @bir_stat, @acct_no, @atm_card_no, @dt_issued, @enable_atm, @atm_status, @ccode, @client, @dep_code, @department, @line_cd, @line, @cont_date, @cont_end, @rate_month, @rate_day, @cont_rate, @meal_rate, @allowance, @position, @sil_amt, @street, @barangay, @city, @province, @title_code, @mfname, @mmname, @mlname, @spou_name, @edu_attaint, @dt_expired, @contact_no, @zipcode, @staff, @active, @active_hmo, @active_sil
                )
            ";
                }
                else
                {
                    query = @"
                UPDATE employee SET
                    fname = @fname,
                    mname = @mname,
                    lname = @lname,
                    sex = @sex,
                    dt_birth = @dt_birth,
                    civil_stat = @civil_stat,
                    sssnum = @sssnum,
                    tin = @tin,
                    hdmfnum = @hdmfnum,
                    phnum = @phnum,
                    bir_cd = @bir_cd,
                    bir_stat = @bir_stat,
                    acct_no = @acct_no,
                    atm_card_no = @atm_card_no,
                    dt_issued = @dt_issued,
                    enable_atm = @enable_atm,
                    atm_status = @atm_status,
                    ccode = @ccode,
                    client = @client,
                    dep_code = @dep_code,
                    department = @department,
                    line_cd = @line_cd,
                    line = @line,
                    cont_date = @cont_date,
                    cont_end = @cont_end,
                    rate_month = @rate_month,
                    rate_day = @rate_day,
                    cont_rate = @cont_rate,
                    meal_rate = @meal_rate,
                    allowance = @allowance,
                    position = @position,
                    sil_amt = @sil_amt,
                    street = @street,
                    barangay = @barangay,
                    city = @city,
                    province = @province,
                    title_code = @title_code,
                    mfname = @mfname,
                    mmname = @mmname,
                    mlname = @mlname,
                    spou_name = @spou_name,
                    edu_attaint = @edu_attaint,
                    dt_expired = @dt_expired,
                    contact_no = @contact_no,
                    zipcode = @zipcode,
                    enable_atm = @enable_atm,
                    staff = @staff,
                    active = @active,
                    active_hmo = @active_hmo,
                    active_sil = @active_sil
                WHERE id_no = @id_no
            ";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    // (parameters code unchanged)
                    cmd.Parameters.AddWithValue("@id_no", id_no.Text.Trim());
                    cmd.Parameters.AddWithValue("@fname", fname.Text.Trim());
                    cmd.Parameters.AddWithValue("@mname", mname.Text.Trim());
                    cmd.Parameters.AddWithValue("@lname", lname.Text.Trim());
                    cmd.Parameters.AddWithValue("@mfname", mfname.Text.Trim());
                    cmd.Parameters.AddWithValue("@mmname", mmname.Text.Trim());
                    cmd.Parameters.AddWithValue("@mlname", mlname.Text.Trim());
                    cmd.Parameters.AddWithValue("@title_code", title_code.Text.Trim());
                    cmd.Parameters.AddWithValue("@sex", sex.Text.Trim());
                    cmd.Parameters.AddWithValue("@spou_name", spou_name.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt_birth", DateTime.TryParse(dt_birth.Text, out DateTime dtb) ? dtb.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@civil_stat", civil_stat.Text.Trim());
                    cmd.Parameters.AddWithValue("@sssnum", sssnum.Text.Trim());
                    cmd.Parameters.AddWithValue("@tin", tin.Text.Trim());
                    cmd.Parameters.AddWithValue("@hdmfnum", hdmfnum.Text.Trim());
                    cmd.Parameters.AddWithValue("@phnum", phnum.Text.Trim());
                    cmd.Parameters.AddWithValue("@bir_cd", bir_cd.Text.Trim());
                    cmd.Parameters.AddWithValue("@bir_stat", bir_stat.Text.Trim());
                    cmd.Parameters.AddWithValue("@acct_no", acct_no.Text.Trim());
                    cmd.Parameters.AddWithValue("@atm_card_no", atm_card_no.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt_issued", DateTime.TryParse(dt_issued.Text, out DateTime dti) ? dti.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@atm_status", atm_status.Text.Trim());
                    cmd.Parameters.AddWithValue("@ccode", ccode.Text.Trim());
                    cmd.Parameters.AddWithValue("@client", client.Text.Trim());
                    cmd.Parameters.AddWithValue("@dep_code", dep_code.Text.Trim());
                    cmd.Parameters.AddWithValue("@department", department.Text.Trim());
                    cmd.Parameters.AddWithValue("@line_cd", line_cd.Text.Trim());
                    cmd.Parameters.AddWithValue("@line", line.Text.Trim());
                    cmd.Parameters.AddWithValue("@cont_date", DateTime.TryParse(cont_date.Text, out DateTime ctd) ? ctd.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@cont_end", DateTime.TryParse(cont_end.Text, out DateTime cte) ? cte.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@rate_month", decimal.TryParse(rate_month.Text, out decimal rm) ? rm : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@rate_day", decimal.TryParse(rate_day.Text, out decimal rd) ? rd : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@cont_rate", decimal.TryParse(cont_rate.Text, out decimal cr) ? cr : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@meal_rate", decimal.TryParse(meal_rate.Text, out decimal mr) ? mr : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@allowance", decimal.TryParse(allowance.Text, out decimal alw) ? alw : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@position", position.Text.Trim());
                    cmd.Parameters.AddWithValue("@sil_amt", decimal.TryParse(sil_amt.Text, out decimal sa) ? sa : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@street", street.Text.Trim());
                    cmd.Parameters.AddWithValue("@barangay", barangay.Text.Trim());
                    cmd.Parameters.AddWithValue("@city", city.Text.Trim());
                    cmd.Parameters.AddWithValue("@province", province.Text.Trim());
                    cmd.Parameters.AddWithValue("@edu_attaint", edu_attaint.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt_expired", DateTime.TryParse(dt_expired.Text, out DateTime dte) ? dte.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@contact_no", contact_no.Text.Trim());
                    cmd.Parameters.AddWithValue("@zipcode", zipcode.Text.Trim());

                    // CheckBoxes
                    cmd.Parameters.AddWithValue("@enable_atm", enable_atm.Checked);
                    cmd.Parameters.AddWithValue("@staff", staff.Checked);
                    cmd.Parameters.AddWithValue("@active", active.Checked);
                    cmd.Parameters.AddWithValue("@active_hmo", active_hmo.Checked);
                    cmd.Parameters.AddWithValue("@active_sil", active_sil.Checked);

                    cmd.ExecuteNonQuery();
                }
            }

            if (isNewEmployee)
            {
                // Add new ID to list and set as current
                employeeIdList.Add(id_no.Text.Trim());
                currentEmployeeIndex = employeeIdList.Count - 1;
                isNewEmployee = false;
                MessageBox.Show("New employee added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Employee data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            SetControlsReadOnly(true);
            cancel.Enabled = false;
        }

        private void import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Excel File",
                Filter = "Excel Files|*.xlsx;*.xls",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string excelFilePath = openFileDialog.FileName;

                using (var workbook = new XLWorkbook(excelFilePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().ToList();

                    // Define expected columns (must match your DB and parameter names)
                    var expectedColumns = new List<string>
            {
                "fileno", "id_no", "old_id", "rfid", "lname", "fname", "mname", "sex", "dt_birth", "civil_stat", "sssnum", "tin",
                "hdmfnum", "phnum", "bir_cd", "bir_stat", "acct_no", "atm_card_no", "dt_issued", "enable_atm", "atm_status",
                "ccode", "client", "dep_code", "department", "line_cd", "line", "cont_date", "cont_end", "rate_month", "rate_day",
                "cont_rate", "meal_rate", "allowance", "position", "active", "emp_rem", "photo", "staff", "active_hmo",
                "active_sil", "sil_amt", "street", "barangay", "poblacion", "city", "province", "date_file", "date_delet",
                "file_sss", "file_ph", "file_hdmf", "ocn", "edu_attaint", "dt_expired", "contact_no", "title_code", "mlname",
                "mfname", "mmname", "spou_name", "zipcode"
            };

                    // Read header row and map column indices
                    var headerRow = rows[0];
                    var headerMap = new Dictionary<string, int>();
                    for (int i = 1; i <= headerRow.CellCount(); i++)
                    {
                        string header = headerRow.Cell(i).GetValue<string>().Trim();
                        if (expectedColumns.Contains(header))
                        {
                            headerMap[header] = i;
                        }
                    }

                    using (MySqlConnection connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();

                        foreach (var row in rows.Skip(1)) // Skip header
                        {
                            string query = @"INSERT INTO employee (
                        " + string.Join(", ", headerMap.Keys) + @"
                    ) VALUES (
                        " + string.Join(", ", headerMap.Keys.Select(k => "@" + k)) + @"
                    )
                    ON DUPLICATE KEY UPDATE
                        " + string.Join(", ", headerMap.Keys.Select(k => $"{k} = VALUES({k})")) + ";";

                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                foreach (var col in headerMap)
                                {
                                    object value = row.Cell(col.Value).Value;
                                    // Handle special types if needed
                                    if (col.Key == "dt_birth" || col.Key == "dt_issued" || col.Key == "cont_date" || col.Key == "cont_end" || col.Key == "dt_expired" || col.Key == "date_file" || col.Key == "date_delet")
                                    {
                                        if (DateTime.TryParse(value?.ToString(), out DateTime dt))
                                            value = dt.ToString("yyyy-MM-dd");
                                        else
                                            value = DBNull.Value;
                                    }
                                    else if (col.Key == "rate_month" || col.Key == "rate_day" || col.Key == "cont_rate" || col.Key == "meal_rate" || col.Key == "allowance" || col.Key == "sil_amt")
                                    {
                                        if (decimal.TryParse(value?.ToString(), out decimal dec))
                                            value = dec;
                                        else
                                            value = DBNull.Value;
                                    }
                                    else if (col.Key == "enable_atm" || col.Key == "active" || col.Key == "staff" || col.Key == "active_hmo" || col.Key == "active_sil")
                                    {
                                        if (bool.TryParse(value?.ToString(), out bool b))
                                            value = b;
                                        else
                                            value = false;
                                    }
                                    else
                                    {
                                        value = value?.ToString() ?? "";
                                    }
                                    cmd.Parameters.AddWithValue("@" + col.Key, value);
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show("Data Imported Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    using (MySqlConnection connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string query = "SELECT id_no FROM employee ORDER BY fileno ASC";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            employeeIdList.Clear();
                            while (reader.Read())
                            {
                                employeeIdList.Add(reader["id_no"].ToString());
                            }
                        }
                    }

                    if (employeeIdList.Count > 0)
                    {
                        currentEmployeeIndex = 0;
                        LoadEmployeeData(employeeIdList[currentEmployeeIndex]);
                    }
                }
            }
        }

        public void LoadEmployeeData(string idNo)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM employee WHERE id_no = @id_no";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_no", idNo);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // TextBoxes
                            id_no.Text = reader["id_no"]?.ToString();
                            fname.Text = reader["fname"]?.ToString();
                            mname.Text = reader["mname"]?.ToString();
                            lname.Text = reader["lname"]?.ToString();
                            sex.Text = reader["sex"]?.ToString();
                            title_code.Text = reader["title_code"]?.ToString();
                            dt_birth.Text = reader["dt_birth"] != DBNull.Value ? Convert.ToDateTime(reader["dt_birth"]).ToString("MM/dd/yyyy") : string.Empty;
                            civil_stat.Text = reader["civil_stat"]?.ToString();
                            sssnum.Text = reader["sssnum"]?.ToString();
                            tin.Text = reader["tin"]?.ToString();
                            hdmfnum.Text = reader["hdmfnum"]?.ToString();
                            phnum.Text = reader["phnum"]?.ToString();
                            bir_cd.Text = reader["bir_cd"]?.ToString();
                            bir_stat.Text = reader["bir_stat"]?.ToString();
                            acct_no.Text = reader["acct_no"]?.ToString();
                            atm_card_no.Text = reader["atm_card_no"]?.ToString();
                            dt_issued.Text = reader["dt_issued"] is DateTime dti ? dti.ToString("MM/dd/yyyy") : reader["dt_issued"]?.ToString();
                            atm_status.Text = reader["atm_status"]?.ToString();
                            ccode.Text = reader["ccode"]?.ToString();
                            client.Text = reader["client"]?.ToString();
                            dep_code.Text = reader["dep_code"]?.ToString();
                            department.Text = reader["department"]?.ToString();
                            line_cd.Text = reader["line_cd"]?.ToString();
                            line.Text = reader["line"]?.ToString();
                            cont_date.Text = reader["cont_date"] is DateTime ctd ? ctd.ToString("MM/dd/yyyy") : reader["cont_date"]?.ToString();
                            cont_end.Text = reader["cont_end"] is DateTime cte ? cte.ToString("MM/dd/yyyy") : reader["cont_end"]?.ToString();
                            rate_month.Text = reader["rate_month"]?.ToString();
                            rate_day.Text = reader["rate_day"]?.ToString();
                            cont_rate.Text = reader["cont_rate"]?.ToString();
                            meal_rate.Text = reader["meal_rate"]?.ToString();
                            allowance.Text = reader["allowance"]?.ToString();
                            position.Text = reader["position"]?.ToString();
                            sil_amt.Text = reader["sil_amt"]?.ToString();
                            street.Text = reader["street"]?.ToString();
                            barangay.Text = reader["barangay"]?.ToString();
                            city.Text = reader["city"]?.ToString();
                            province.Text = reader["province"]?.ToString();
                            edu_attaint.Text = reader["edu_attaint"]?.ToString();
                            dt_expired.Text = reader["dt_expired"] is DateTime dte ? dte.ToString("MM/dd/yyyy") : reader["dt_expired"]?.ToString();
                            contact_no.Text = reader["contact_no"]?.ToString();
                            zipcode.Text = reader["zipcode"]?.ToString();
                            mfname.Text = reader["mfname"]?.ToString();
                            mmname.Text = reader["mmname"]?.ToString();
                            mlname.Text = reader["mlname"]?.ToString();
                            spou_name.Text = reader["spou_name"]?.ToString();

                            // CheckBoxes
                            enable_atm.Checked = reader["enable_atm"] != DBNull.Value && Convert.ToBoolean(reader["enable_atm"]);
                            staff.Checked = reader["staff"] != DBNull.Value && Convert.ToBoolean(reader["staff"]);
                            active.Checked = reader["active"] != DBNull.Value && Convert.ToBoolean(reader["active"]);
                            active_hmo.Checked = reader["active_hmo"] != DBNull.Value && Convert.ToBoolean(reader["active_hmo"]);
                            active_sil.Checked = reader["active_sil"] != DBNull.Value && Convert.ToBoolean(reader["active_sil"]);

                            // ComboBoxes
                            if (ccode.Items.Contains(reader["ccode"]?.ToString()))
                                ccode.SelectedItem = reader["ccode"]?.ToString();
                            if (dep_code.Items.Contains(reader["dep_code"]?.ToString()))
                                dep_code.SelectedItem = reader["dep_code"]?.ToString();
                            if (line_cd.Items.Contains(reader["line_cd"]?.ToString()))
                                line_cd.SelectedItem = reader["line_cd"]?.ToString();
                            if (edu_attaint.Items.Contains(reader["edu_attaint"]?.ToString()))
                                edu_attaint.SelectedItem = reader["edu_attaint"]?.ToString();
                            if (bir_cd.Items.Contains(reader["bir_cd"]?.ToString()))
                                bir_cd.SelectedItem = reader["bir_cd"]?.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Employee not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            // Update current index if found in the list
            int idx = employeeIdList.IndexOf(idNo);
            if (idx >= 0)
                currentEmployeeIndex = idx;
        }

        private void edit_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("You are not allowed to edit employee records.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetControlsReadOnly(false);
            cancel.Enabled = true;
        }

        private void back_Click(object sender, EventArgs e)
        {
            if (employeeIdList.Count == 0) return;
            if (currentEmployeeIndex > 0)
            {
                currentEmployeeIndex--;
                LoadEmployeeData(employeeIdList[currentEmployeeIndex]);
            }
        }

        private void next_Click(object sender, EventArgs e)
        {
            if (employeeIdList.Count == 0) return;
            if (currentEmployeeIndex < employeeIdList.Count - 1)
            {
                currentEmployeeIndex++;
                LoadEmployeeData(employeeIdList[currentEmployeeIndex]);
            }
        }

        private void empsearch_Click(object sender, EventArgs e)
        {
            employeesearch employeesearchForm = new employeesearch();
            employeesearchForm.Show();
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

        private void cancel_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(true);
            cancel.Enabled = false; // Disable cancel after canceling
                                    // Optionally reload the current employee data to revert changes
            if (employeeIdList.Count > 0)
                LoadEmployeeData(employeeIdList[currentEmployeeIndex]);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("You are not allowed to delete employee records.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(id_no.Text))
            {
                MessageBox.Show("No employee selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirmResult = MessageBox.Show(
                "Are you sure you want to delete this employee?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM employee WHERE id_no = @id_no";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_no", id_no.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                // Remove from local list and update UI
                int removedIndex = currentEmployeeIndex;
                employeeIdList.RemoveAt(currentEmployeeIndex);

                if (employeeIdList.Count > 0)
                {
                    // Show next employee, or previous if last was deleted
                    if (removedIndex >= employeeIdList.Count)
                        removedIndex = employeeIdList.Count - 1;
                    currentEmployeeIndex = removedIndex;
                    LoadEmployeeData(employeeIdList[currentEmployeeIndex]);
                }
                else
                {
                    // No employees left, clear fields
                    foreach (Control c in Controls)
                    {
                        if (c is TextBox tb) tb.Text = "";
                        else if (c is ComboBox cb) cb.SelectedIndex = -1;
                        else if (c is CheckBox chk) chk.Checked = false;
                    }
                    currentEmployeeIndex = 0;
                }

                SetControlsReadOnly(true);
                cancel.Enabled = false;
                MessageBox.Show("Employee deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void empnew_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("You are not allowed to add new employees.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Clear all input fields
            foreach (Control c in Controls)
            {
                if (c is TextBox tb) tb.Text = "";
                else if (c is ComboBox cb) cb.SelectedIndex = -1;
                else if (c is CheckBox chk) chk.Checked = false;
            }

            SetControlsReadOnly(false);
            cancel.Enabled = true;
            isNewEmployee = true;
            id_no.ReadOnly = false; // Allow entering new ID
        }
    }
}
