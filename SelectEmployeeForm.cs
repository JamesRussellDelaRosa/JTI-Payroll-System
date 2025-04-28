using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace JTI_Payroll_System
{
    public partial class SelectEmployeeForm : Form
    {
        private string selectedID;

        public string GetSelectedID()
        {
            return selectedID;
        }

        private void SetSelectedID(string value)
        {
            selectedID = value;
        }

        private string selectedFName;

        public string GetSelectedFName()
        {
            return selectedFName;
        }

        private void SetSelectedFName(string value)
        {
            selectedFName = value;
        }

        private string selectedLName;

        public string GetSelectedLName()
        {
            return selectedLName;
        }

        private void SetSelectedLName(string value)
        {
            selectedLName = value;
        }

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
                SetSelectedID(dataGridView1.Rows[e.RowIndex].Cells["id_no"].Value.ToString());
                SetSelectedFName(dataGridView1.Rows[e.RowIndex].Cells["fname"].Value.ToString());
                SetSelectedLName(dataGridView1.Rows[e.RowIndex].Cells["lname"].Value.ToString());

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
