using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class SelectEmployeeForm : Form
    {
        [Browsable(false)] // Prevents serialization errors
        public string SelectedID { get; private set; }
        public string SelectedFName { get; private set; }
        public string SelectedLName { get; private set; }

        public SelectEmployeeForm(DataTable employeeData)
        {
            InitializeComponent();
            dataGridView1.DataSource = employeeData;

            // Ensure selection behavior
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // Handle row double-click event
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure a valid row is selected
            {
                SelectedID = dataGridView1.Rows[e.RowIndex].Cells["id_no"].Value.ToString();
                SelectedFName = dataGridView1.Rows[e.RowIndex].Cells["fname"].Value.ToString();
                SelectedLName = dataGridView1.Rows[e.RowIndex].Cells["lname"].Value.ToString();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
