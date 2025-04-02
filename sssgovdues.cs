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

            // Add 61 rows to the DataGridView
            for (int i = 0; i < 61; i++)
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
    }
}
