using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Add this for MySQL connection

namespace JTI_Payroll_System
{
    public partial class sssgovdues : Form
    {
        public sssgovdues()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.Columns.Add("salary1", "Salary 1");
            dataGridView.Columns.Add("salary2", "Salary 2");
            dataGridView.Columns.Add("salarycredit", "Salary Credit");
            dataGridView.Columns.Add("ERShare", "ER Share");
            dataGridView.Columns.Add("ERMPF", "ER MPF");
            dataGridView.Columns.Add("ERECC", "ER ECC");
            dataGridView.Columns.Add("EEShare", "EE Share");
            dataGridView.Columns.Add("EEMPF", "EE MPF");

            //Remove the code that adds 61 rows to the DataGridView
            for (int i = 0; i < 62; i++)
            {
                dataGridView.Rows.Add();
            }

            dataGridView.KeyDown += DataGridView_KeyDown;

            // Add context menu for right-click paste
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem pasteMenuItem = new ToolStripMenuItem("Paste");
            pasteMenuItem.Click += PasteMenuItem_Click;
            contextMenu.Items.Add(pasteMenuItem);
            dataGridView.ContextMenuStrip = contextMenu;
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteClipboardData();
            }
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardData();
        }

        private void PasteClipboardData()
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                string[] lines = clipboardText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                int startRow = dataGridView.CurrentCell.RowIndex;
                int startCol = dataGridView.CurrentCell.ColumnIndex;

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] cells = line.Split('\t');
                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (startRow >= dataGridView.Rows.Count)
                        {
                            dataGridView.Rows.Add();
                        }

                        dataGridView[startCol + i, startRow].Value = cells[i];
                    }
                    startRow++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting data: " + ex.Message);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            // Check if the DataGridView is empty
            bool hasData = dataGridView.Rows.Cast<DataGridViewRow>()
                .Any(row => !row.IsNewRow && row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value != null && cell.Value.ToString().Trim() != ""));

            if (!hasData)
            {
                MessageBox.Show("Cannot save data because the grid is empty.");
                return;
            }

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (DataGridViewRow row in dataGridView.Rows)
                        {
                            // Skip new row placeholder
                            if (row.IsNewRow) continue;

                            // Check if the row has data
                            bool rowHasData = row.Cells.Cast<DataGridViewCell>()
                                .Any(cell => cell.Value != null && cell.Value.ToString().Trim() != "");

                            if (!rowHasData) continue;

                            string query = @"
                        INSERT INTO sssgovdues (salary1, salary2, salarycredit, ERShare, ERMPF, ERECC, EEShare, EEMPF)
                        VALUES (@salary1, @salary2, @salarycredit, @ERShare, @ERMPF, @ERECC, @EEShare, @EEMPF)
                        ON DUPLICATE KEY UPDATE
                            salary2 = VALUES(salary2),
                            salarycredit = VALUES(salarycredit),
                            ERShare = VALUES(ERShare),
                            ERMPF = VALUES(ERMPF),
                            ERECC = VALUES(ERECC),
                            EEShare = VALUES(EEShare),
                            EEMPF = VALUES(EEMPF)";

                            using (MySqlCommand command = new MySqlCommand(query, conn, transaction))
                            {
                                command.Parameters.AddWithValue("@salary1", ConvertToDecimal(row.Cells["salary1"].Value));
                                command.Parameters.AddWithValue("@salary2", ConvertToDecimal(row.Cells["salary2"].Value));
                                command.Parameters.AddWithValue("@salarycredit", ConvertToDecimal(row.Cells["salarycredit"].Value));
                                command.Parameters.AddWithValue("@ERShare", ConvertToDecimal(row.Cells["ERShare"].Value));
                                command.Parameters.AddWithValue("@ERMPF", ConvertToDecimal(row.Cells["ERMPF"].Value));
                                command.Parameters.AddWithValue("@ERECC", ConvertToDecimal(row.Cells["ERECC"].Value));
                                command.Parameters.AddWithValue("@EEShare", ConvertToDecimal(row.Cells["EEShare"].Value));
                                command.Parameters.AddWithValue("@EEMPF", ConvertToDecimal(row.Cells["EEMPF"].Value));

                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                        MessageBox.Show("Data saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error saving data: " + ex.Message);
                    }
                }
            }
        }

        private decimal ConvertToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            string stringValue = value.ToString().Replace(",", "");
            if (decimal.TryParse(stringValue, out decimal result))
                return result;

            throw new FormatException($"Invalid decimal value: {value}");
        }

        private void load_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM sssgovdues";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        dataGridView.Rows.Clear();
                        while (reader.Read())
                        {
                            int rowIndex = dataGridView.Rows.Add();
                            DataGridViewRow row = dataGridView.Rows[rowIndex];
                            row.Cells["salary1"].Value = reader["salary1"];
                            row.Cells["salary2"].Value = reader["salary2"];
                            row.Cells["salarycredit"].Value = reader["salarycredit"];
                            row.Cells["ERShare"].Value = reader["ERShare"];
                            row.Cells["ERMPF"].Value = reader["ERMPF"];
                            row.Cells["ERECC"].Value = reader["ERECC"];
                            row.Cells["EEShare"].Value = reader["EEShare"];
                            row.Cells["EEMPF"].Value = reader["EEMPF"];
                        }
                    }
                }
            }
        }

    }
}


