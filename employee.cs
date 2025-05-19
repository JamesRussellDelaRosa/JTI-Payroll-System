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

        private List<string> employeeIdList = new();
        private int currentEmployeeIndex = 0;
        public employee()
        {
            InitializeComponent();
            SetControlsReadOnly(true);
            this.Load += employee_Load;

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
            if (string.IsNullOrWhiteSpace(id_no.Text))
            {
                MessageBox.Show("No employee selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
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

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    // TextBoxes
                    cmd.Parameters.AddWithValue("@id_no", id_no.Text.Trim());
                    cmd.Parameters.AddWithValue("@fname", fname.Text.Trim());
                    cmd.Parameters.AddWithValue("@mname", mname.Text.Trim());
                    cmd.Parameters.AddWithValue("@lname", lname.Text.Trim());
                    cmd.Parameters.AddWithValue("@sex", sex.Text.Trim());
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

            SetControlsReadOnly(true);
            MessageBox.Show("Employee data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void import_Click(object sender, EventArgs e)
        {
            // Open file dialog for selecting the Excel file
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
                    var rows = worksheet.RangeUsed().RowsUsed();

                    using (MySqlConnection connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();

                        foreach (var row in rows.Skip(1)) // Skip the header row
                        {
                            string query = @"INSERT INTO employee (
                        fileno, id_no, old_id, rfid, lname, fname, mname, sex, dt_birth, civil_stat, sssnum, tin, 
                        hdmfnum, phnum, bir_cd, bir_stat, acct_no, atm_card_no, dt_issued, enable_atm, atm_status, 
                        ccode, client, dep_code, department, line_cd, line, cont_date, cont_end, rate_month, rate_day, 
                        cont_rate, meal_rate, allowance, position, active, emp_rem, photo, staff, active_hmo, 
                        active_sil, sil_amt, street, barangay, poblacion, city, province, date_file, date_delet, 
                        file_sss, file_ph, file_hdmf, ocn, edu_attaint, dt_expired, contact_no, title_code, mlname, 
                        mfname, mmname, spou_name, zipcode
                    ) VALUES (
                        @fileno, @id_no, @old_id, @rfid, @lname, @fname, @mname, @sex, @dt_birth, @civil_stat, @sssnum, @tin, 
                        @hdmfnum, @phnum, @bir_cd, @bir_stat, @acct_no, @atm_card_no, @dt_issued, @enable_atm, @atm_status, 
                        @ccode, @client, @dep_code, @department, @line_cd, @line, @cont_date, @cont_end, @rate_month, @rate_day, 
                        @cont_rate, @meal_rate, @allowance, @position, @active, @emp_rem, @photo, @staff, @active_hmo, 
                        @active_sil, @sil_amt, @street, @barangay, @poblacion, @city, @province, @date_file, @date_delet, 
                        @file_sss, @file_ph, @file_hdmf, @ocn, @edu_attaint, @dt_expired, @contact_no, @title_code, @mlname, 
                        @mfname, @mmname, @spou_name, @zipcode
                    )
                    ON DUPLICATE KEY UPDATE
                    old_id = VALUES(old_id),
                    rfid = VALUES(rfid),
                    lname = VALUES(lname),
                    fname = VALUES(fname),
                    mname = VALUES(mname),
                    sex = VALUES(sex),
                    dt_birth = VALUES(dt_birth),
                    civil_stat = VALUES(civil_stat),
                    sssnum = VALUES(sssnum),
                    tin = VALUES(tin),
                    hdmfnum = VALUES(hdmfnum),
                    phnum = VALUES(phnum),
                    bir_cd = VALUES(bir_cd),
                    bir_stat = VALUES(bir_stat),
                    acct_no = VALUES(acct_no),
                    atm_card_no = VALUES(atm_card_no),
                    dt_issued = VALUES(dt_issued),
                    enable_atm = VALUES(enable_atm),
                    atm_status = VALUES(atm_status),
                    ccode = VALUES(ccode),
                    client = VALUES(client),
                    dep_code = VALUES(dep_code),
                    department = VALUES(department),
                    line_cd = VALUES(line_cd),
                    line = VALUES(line),
                    cont_date = VALUES(cont_date),
                    cont_end = VALUES(cont_end),
                    rate_month = VALUES(rate_month),
                    rate_day = VALUES(rate_day),
                    cont_rate = VALUES(cont_rate),
                    meal_rate = VALUES(meal_rate),
                    allowance = VALUES(allowance),
                    position = VALUES(position),
                    active = VALUES(active),
                    emp_rem = VALUES(emp_rem),
                    photo = VALUES(photo),
                    staff = VALUES(staff),
                    active_hmo = VALUES(active_hmo),
                    active_sil = VALUES(active_sil),
                    sil_amt = VALUES(sil_amt),
                    street = VALUES(street),
                    barangay = VALUES(barangay),
                    poblacion = VALUES(poblacion),
                    city = VALUES(city),
                    province = VALUES(province),
                    date_file = VALUES(date_file),
                    date_delet = VALUES(date_delet),
                    file_sss = VALUES(file_sss),
                    file_ph = VALUES(file_ph),
                    file_hdmf = VALUES(file_hdmf),
                    ocn = VALUES(ocn),
                    edu_attaint = VALUES(edu_attaint),
                    dt_expired = VALUES(dt_expired),
                    contact_no = VALUES(contact_no),
                    title_code = VALUES(title_code),
                    mlname = VALUES(mlname),
                    mfname = VALUES(mfname),
                    mmname = VALUES(mmname),
                    spou_name = VALUES(spou_name),
                    zipcode = VALUES(zipcode);
                    ";

                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@fileno", row.Cell(1).GetValue<int>());
                                cmd.Parameters.AddWithValue("@id_no", row.Cell(2).GetValue<string>());
                                cmd.Parameters.AddWithValue("@old_id", row.Cell(3).GetValue<string>());
                                cmd.Parameters.AddWithValue("@rfid", row.Cell(4).GetValue<string>());
                                cmd.Parameters.AddWithValue("@lname", row.Cell(5).GetValue<string>());
                                cmd.Parameters.AddWithValue("@fname", row.Cell(6).GetValue<string>());
                                cmd.Parameters.AddWithValue("@mname", row.Cell(7).GetValue<string>());
                                cmd.Parameters.AddWithValue("@sex", row.Cell(8).GetValue<string>());
                                cmd.Parameters.AddWithValue("@dt_birth",
                                    DateTime.TryParseExact(row.Cell(9).GetValue<string>(), "dd-MMM-yy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime dtBirth)
                                    ? dtBirth.ToString("yyyy-MM-dd")
                                    : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@civil_stat", row.Cell(10).GetValue<string>());
                                cmd.Parameters.AddWithValue("@sssnum", row.Cell(11).GetValue<string>());
                                cmd.Parameters.AddWithValue("@tin", row.Cell(12).GetValue<string>());
                                cmd.Parameters.AddWithValue("@hdmfnum", row.Cell(13).GetValue<string>());
                                cmd.Parameters.AddWithValue("@phnum", row.Cell(14).GetValue<string>());
                                cmd.Parameters.AddWithValue("@bir_cd", row.Cell(15).GetValue<string>());
                                cmd.Parameters.AddWithValue("@bir_stat", row.Cell(16).GetValue<string>());
                                cmd.Parameters.AddWithValue("@acct_no", row.Cell(17).GetValue<string>());
                                cmd.Parameters.AddWithValue("@atm_card_no", row.Cell(18).GetValue<string>());
                                cmd.Parameters.AddWithValue("@dt_issued",
                                    DateTime.TryParseExact(row.Cell(19).GetValue<string>(), "dd-MMM-yy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime dtIssued)
                                    ? dtIssued.ToString("yyyy-MM-dd")
                                    : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@enable_atm", row.Cell(20).GetBoolean());
                                cmd.Parameters.AddWithValue("@atm_status", row.Cell(21).GetValue<string>());
                                cmd.Parameters.AddWithValue("@ccode", row.Cell(22).GetValue<string>());
                                cmd.Parameters.AddWithValue("@client", row.Cell(23).GetValue<string>());
                                cmd.Parameters.AddWithValue("@dep_code", row.Cell(24).GetValue<string>());
                                cmd.Parameters.AddWithValue("@department", row.Cell(25).GetValue<string>());
                                cmd.Parameters.AddWithValue("@line_cd", row.Cell(26).GetValue<string>());
                                cmd.Parameters.AddWithValue("@line", row.Cell(27).GetValue<string>());
                                cmd.Parameters.AddWithValue("@cont_date",
                                    DateTime.TryParseExact(row.Cell(28).GetValue<string>(), "dd-MMM-yy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime contDate)
                                    ? contDate.ToString("yyyy-MM-dd")
                                    : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@cont_end",
                                    DateTime.TryParseExact(row.Cell(29).GetValue<string>(), "dd-MMM-yy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime contEnd)
                                    ? contEnd.ToString("yyyy-MM-dd")
                                    : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@rate_month",
                                    decimal.TryParse(row.Cell(30).GetValue<string>(), out decimal rateMonth) ? rateMonth : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@rate_day",
                                    decimal.TryParse(row.Cell(31).GetValue<string>(), out decimal rateDay) ? rateDay : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@cont_rate",
                                    decimal.TryParse(row.Cell(32).GetValue<string>(), out decimal contRate) ? contRate : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@meal_rate",
                                    decimal.TryParse(row.Cell(33).GetValue<string>(), out decimal mealRate) ? mealRate : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@allowance",
                                    decimal.TryParse(row.Cell(34).GetValue<string>(), out decimal allowance) ? allowance : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@position", row.Cell(35).GetValue<string>());
                                cmd.Parameters.AddWithValue("@active", row.Cell(36).GetBoolean());
                                cmd.Parameters.AddWithValue("@emp_rem", row.Cell(37).GetValue<string>());
                                cmd.Parameters.AddWithValue("@photo", row.Cell(38).GetValue<string>());
                                cmd.Parameters.AddWithValue("@staff", row.Cell(39).GetBoolean());
                                cmd.Parameters.AddWithValue("@active_hmo", row.Cell(40).GetBoolean());
                                cmd.Parameters.AddWithValue("@active_sil", row.Cell(41).GetBoolean());
                                cmd.Parameters.AddWithValue("@sil_amt",
                                    decimal.TryParse(row.Cell(42).GetValue<string>(), out decimal silAmt) ? silAmt : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@street", row.Cell(43).GetValue<string>());
                                cmd.Parameters.AddWithValue("@barangay", row.Cell(44).GetValue<string>());
                                cmd.Parameters.AddWithValue("@poblacion", row.Cell(45).GetValue<string>());
                                cmd.Parameters.AddWithValue("@city", row.Cell(46).GetValue<string>());
                                cmd.Parameters.AddWithValue("@province", row.Cell(47).GetValue<string>());
                                cmd.Parameters.AddWithValue("@date_file",
                                    DateTime.TryParse(row.Cell(48).GetValue<string>(), out DateTime dateFile) ? dateFile : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@date_delet",
                                    DateTime.TryParse(row.Cell(49).GetValue<string>(), out DateTime dateDelet) ? dateDelet : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@file_sss", row.Cell(50).GetValue<string>());
                                cmd.Parameters.AddWithValue("@file_ph", row.Cell(51).GetValue<string>());
                                cmd.Parameters.AddWithValue("@file_hdmf", row.Cell(52).GetValue<string>());
                                cmd.Parameters.AddWithValue("@ocn", row.Cell(53).GetValue<string>());
                                cmd.Parameters.AddWithValue("@edu_attaint", row.Cell(54).GetValue<string>());
                                cmd.Parameters.AddWithValue("@dt_expired",
                                    DateTime.TryParseExact(row.Cell(55).GetValue<string>(), "dd-MMM-yy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime dtExpired)
                                    ? dtExpired.ToString("yyyy-MM-dd") // Convert to yyyy-MM-dd format
                                    : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@contact_no", row.Cell(56).GetValue<string>());
                                cmd.Parameters.AddWithValue("@title_code", row.Cell(57).GetValue<string>());
                                cmd.Parameters.AddWithValue("@mlname", row.Cell(58).GetValue<string>());
                                cmd.Parameters.AddWithValue("@mfname", row.Cell(59).GetValue<string>());
                                cmd.Parameters.AddWithValue("@mmname", row.Cell(60).GetValue<string>());
                                cmd.Parameters.AddWithValue("@spou_name", row.Cell(61).GetValue<string>());
                                cmd.Parameters.AddWithValue("@zipcode", row.Cell(62).GetValue<string>());



                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show("Data Imported Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            dt_birth.Text = reader["dt_birth"] is DateTime dtb ? dtb.ToString("yyyy-MM-dd") : reader["dt_birth"]?.ToString();
                            civil_stat.Text = reader["civil_stat"]?.ToString();
                            sssnum.Text = reader["sssnum"]?.ToString();
                            tin.Text = reader["tin"]?.ToString();
                            hdmfnum.Text = reader["hdmfnum"]?.ToString();
                            phnum.Text = reader["phnum"]?.ToString();
                            bir_cd.Text = reader["bir_cd"]?.ToString();
                            bir_stat.Text = reader["bir_stat"]?.ToString();
                            acct_no.Text = reader["acct_no"]?.ToString();
                            atm_card_no.Text = reader["atm_card_no"]?.ToString();
                            dt_issued.Text = reader["dt_issued"] is DateTime dti ? dti.ToString("yyyy-MM-dd") : reader["dt_issued"]?.ToString();
                            atm_status.Text = reader["atm_status"]?.ToString();
                            ccode.Text = reader["ccode"]?.ToString();
                            client.Text = reader["client"]?.ToString();
                            dep_code.Text = reader["dep_code"]?.ToString();
                            department.Text = reader["department"]?.ToString();
                            line_cd.Text = reader["line_cd"]?.ToString();
                            line.Text = reader["line"]?.ToString();
                            cont_date.Text = reader["cont_date"] is DateTime ctd ? ctd.ToString("yyyy-MM-dd") : reader["cont_date"]?.ToString();
                            cont_end.Text = reader["cont_end"] is DateTime cte ? cte.ToString("yyyy-MM-dd") : reader["cont_end"]?.ToString();
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
                            dt_expired.Text = reader["dt_expired"] is DateTime dte ? dte.ToString("yyyy-MM-dd") : reader["dt_expired"]?.ToString();
                            contact_no.Text = reader["contact_no"]?.ToString();
                            zipcode.Text = reader["zipcode"]?.ToString();

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
            SetControlsReadOnly(false);
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
    }
}
