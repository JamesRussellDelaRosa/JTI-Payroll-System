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
        private Panel selectedPanel = null;

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
            if (textBox != null)
            {
                string currentText = textBox.Text;

                if (string.IsNullOrWhiteSpace(currentText))
                {
                    // If user cleared the text or entered only spaces, reset to placeholder.
                    SetPlaceholderText(textBox, "MM/DD/YYYY");
                }
                // Only validate if the text color indicates it's user input (not the gray placeholder).
                else if (textBox.ForeColor == SystemColors.WindowText)
                {
                    // The placeholder string "MM/DD/YYYY" itself is not a valid date for processing.
                    // Also, any other text that doesn't parse correctly into a date.
                    if (currentText == "MM/DD/YYYY" || 
                        !DateTime.TryParseExact(currentText, "MM/dd/yyyy", 
                                               System.Globalization.CultureInfo.InvariantCulture, 
                                               System.Globalization.DateTimeStyles.None, 
                                               out _))
                    {
                        MessageBox.Show($"The date '{currentText}' is invalid. Please use MM/DD/YYYY format and ensure it's a real date (e.g., not 02/30/2023).",
                                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                textBox.Invalidate(); // Redraw for placeholder or other visual updates.
            }
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
        private void filter_Click(object sender, EventArgs e)
        {
            employees.Controls.Clear();
            if (!DateTime.TryParseExact(textStartDate.Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(textEndDate.Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endDate) ||
                textStartDate.Text == "MM/DD/YYYY" || // Explicitly disallow placeholder text
                textEndDate.Text == "MM/DD/YYYY")
            {
                MessageBox.Show("Please enter valid start and end dates using MM/DD/YYYY format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (startDate > endDate)
            {
                MessageBox.Show("The start date cannot be after the end date. Please enter a valid date range.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadEmployeesByDateRange(startDate, endDate);
        }
        private void LoadEmployeesByDateRange(DateTime startDate, DateTime endDate)
        {
            // Define original fixed widths as base for proportions
            int[] columnBaseWidths = { 80, 100, 100, 80, 60, 110, 110 };
            double totalBaseWidthUnits = columnBaseWidths.Sum(w => (double)w); // Sum of base units for proportion calculation
            int numberOfColumns = columnBaseWidths.Length;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT employee_id, lname, fname, mname, ccode, pay_period_start, pay_period_end FROM payroll WHERE pay_period_start <= @end AND pay_period_end >= @start GROUP BY employee_id, lname, fname, mname, ccode, pay_period_start, pay_period_end ORDER BY lname, fname, mname";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", endDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Panel panel = new Panel
                                {
                                    Height = 32,
                                    // Width will be set below based on parent
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(2), // Existing margin
                                    BackColor = Color.White
                                };

                                // Set panel width to be responsive to the 'employees' container width
                                int targetPanelWidth = employees.ClientRectangle.Width - panel.Margin.Horizontal;
                                // Ensure a minimum sensible width for the panel
                                int minPanelWidth = Math.Max(50, (int)(totalBaseWidthUnits * 0.4)); // e.g., min 40% of original total, or 50px
                                panel.Width = Math.Max(minPanelWidth, targetPanelWidth);


                                // Calculate available width for labels inside this panel
                                int initialXOffset = 5;    // Left padding inside the panel for the first label
                                int gapBetweenLabels = 5;  // Gap between labels

                                // panel.ClientRectangle.Width is the inner width, accounts for panel's own borders
                                int internalPanelClientAreaWidth = panel.ClientRectangle.Width;
                                
                                // Total width needed for all gaps between labels
                                int totalGapSpace = (numberOfColumns > 1 ? (numberOfColumns - 1) * gapBetweenLabels : 0);

                                // Net width available for the sum of all label widths
                                int netWidthForAllLabels = internalPanelClientAreaWidth - initialXOffset - totalGapSpace;
                                if (netWidthForAllLabels < 0) netWidthForAllLabels = 0; // Cannot be negative

                                string[] values = {
                                    reader["employee_id"].ToString(),
                                    reader["lname"].ToString(),
                                    reader["fname"].ToString(),
                                    reader["mname"].ToString(),
                                    reader["ccode"].ToString(),
                                    Convert.ToDateTime(reader["pay_period_start"]).ToString("MM/dd/yyyy"),
                                    Convert.ToDateTime(reader["pay_period_end"]).ToString("MM/dd/yyyy")
                                };
                                
                                int currentX = initialXOffset;
                                for (int i = 0; i < numberOfColumns; i++)
                                {
                                    int calculatedLabelWidth;
                                    // Check if there's meaningful space to distribute and base units exist
                                    if (totalBaseWidthUnits > 0 && netWidthForAllLabels > (numberOfColumns * 10) ) 
                                    {
                                        double proportion = (double)columnBaseWidths[i] / totalBaseWidthUnits;
                                        calculatedLabelWidth = (int)(proportion * netWidthForAllLabels);
                                        // Ensure a minimum width for labels to be somewhat visible and clickable
                                        calculatedLabelWidth = Math.Max(10, calculatedLabelWidth); 
                                    }
                                    else
                                    {
                                        // Fallback: if panel is too narrow or no base units.
                                        // Distribute remaining space or use a small fixed minimum.
                                        if (netWidthForAllLabels > 0 && numberOfColumns > 0) {
                                             calculatedLabelWidth = Math.Max(10, netWidthForAllLabels / numberOfColumns);
                                        } else {
                                             calculatedLabelWidth = Math.Max(10, columnBaseWidths[i] / 3); // Small portion of original
                                        }
                                    }

                                    Label lbl = new Label
                                    {
                                        Text = values[i],
                                        Location = new Point(currentX, 7), // Y position is fixed
                                        Width = calculatedLabelWidth,      // Use calculated responsive width
                                        AutoSize = false,                  // Important for fixed Width and AutoEllipsis
                                        AutoEllipsis = true,               // Show "..." if text is too long
                                        TextAlign = ContentAlignment.MiddleLeft, // Align text
                                        BackColor = Color.Transparent
                                    };
                                    lbl.Click += (s, ev) => EmployeePanel_Click(panel, ev); 
                                    lbl.DoubleClick += (s, ev) => EmployeePanel_DoubleClick(panel, ev); 
                                    panel.Controls.Add(lbl);
                                    currentX += calculatedLabelWidth + gapBetweenLabels; // Move X for next label
                                }
                                
                                panel.Tag = new {
                                    employee_id = reader["employee_id"].ToString(),
                                    lname = reader["lname"].ToString(),
                                    fname = reader["fname"].ToString(),
                                    mname = reader["mname"].ToString(),
                                    ccode = reader["ccode"].ToString(),
                                    pay_period_start = Convert.ToDateTime(reader["pay_period_start"]),
                                    pay_period_end = Convert.ToDateTime(reader["pay_period_end"])
                                };
                                panel.Cursor = Cursors.Hand;
                                panel.Click += EmployeePanel_Click;
                                panel.DoubleClick += EmployeePanel_DoubleClick;
                                employees.Controls.Add(panel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EmployeePanel_Click(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null && sender is Label lbl && lbl.Parent is Panel p) panel = p;
            if (panel?.Tag == null) return;

            // Highlight selection
            if (selectedPanel != null && selectedPanel != panel)
            {
                selectedPanel.BackColor = Color.White;
            }

            if (panel.BackColor == Color.LightBlue) // If already selected, deselect on click
            {
                panel.BackColor = Color.White;
                selectedPanel = null;
            }
            else
            {
                panel.BackColor = Color.LightBlue;
                selectedPanel = panel;
            }
        }
        private void EmployeePanel_DoubleClick(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null && sender is Label lbl && lbl.Parent is Panel p) panel = p;
            if (panel?.Tag == null) return;

            // Ensure the panel is highlighted before opening the form
            if (selectedPanel != panel)
            {
                if (selectedPanel != null)
                {
                    selectedPanel.BackColor = Color.White;
                }
                panel.BackColor = Color.LightBlue;
                selectedPanel = panel;
            }
            
            dynamic tag = panel.Tag;
            var adjForm = new adjustment_input(
                tag.employee_id,
                tag.lname,
                tag.fname,
                tag.mname,
                tag.ccode,
                tag.pay_period_start,
                tag.pay_period_end
            );
            adjForm.ShowDialog();
        }
    }
}