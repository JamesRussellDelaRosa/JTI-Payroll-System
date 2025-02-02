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

namespace JTI_Payroll_System
{
    public partial class employee : Form
    {
        public employee()
        {
            InitializeComponent();
        }

        private void saveEmp_Click(object sender, EventArgs e)
        {

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

        private void search_Click(object sender, EventArgs e)
        {
            string searchText = searchbar.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a search keyword!", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT id_no, fname, lname
                    FROM employee 
                    WHERE id_no LIKE @search OR fname LIKE @search OR lname LIKE @search";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable employeeData = new DataTable();
                        adapter.Fill(employeeData);

                        if (employeeData.Rows.Count == 1)  // If only one match is found
                        {
                            DataRow row = employeeData.Rows[0];
                            textEmpID.Text = row["id_no"].ToString();
                            textFirstName.Text = row["fname"].ToString();
                            textLastName.Text = row["lname"].ToString();
                        }
                        else if (employeeData.Rows.Count > 1)  // If multiple matches exist
                        {
                            using (SelectEmployeeForm selectForm = new SelectEmployeeForm(employeeData))
                            {
                                if (selectForm.ShowDialog() == DialogResult.OK)
                                {
                                    textEmpID.Text = selectForm.SelectedID;
                                    textFirstName.Text = selectForm.SelectedFName;
                                    textLastName.Text = selectForm.SelectedLName; 
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No matching records found!", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void btnOpenSelectEmployeeForm_Click(object sender, EventArgs e)
        {
            // Fetch employee data (Replace with actual database query)
            DataTable employeeData = GetEmployeeData();

            using (SelectEmployeeForm selectForm = new SelectEmployeeForm(employeeData))
            {
                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    textEmpID.Text = selectForm.SelectedID;
                    textFirstName.Text = selectForm.SelectedFName;
                    textLastName.Text = selectForm.SelectedLName;
                }
            }
        }

        private DataTable GetEmployeeData()
        {
            DataTable dt = new DataTable();

            // Define your SQL query
            string query = "SELECT id_no, fname, lname FROM employee";

            // Connect to the database
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                dataAdapter.Fill(dt);  // Fill the DataTable with data from the database
            }

            return dt;
        }

        private void textLastName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
