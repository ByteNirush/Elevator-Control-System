using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ElevatorControlSystem.Controllers;

namespace ElevatorControlSystem.Forms
{
    /// <summary>
    /// Log Form - Professional elevator operation history viewer
    /// TASK 3: View Log button shows all stored operations in DataGridView
    /// Uses BackgroundWorker for non-blocking database queries
    /// Features: Search, Filter, Export, Sort, Statistics
    /// </summary>
    public partial class LogForm : Form
    {
        private BackgroundWorker _dbWorker;
        private DataTable _originalData;
        private string _currentFilter = "All";

        /// <summary>
        /// Constructor
        /// </summary>
        public LogForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            InitializeCustomControls();
            
            // Load logs when form opens
            this.Load += LogForm_Load;
            
            // Add cell formatting event for timestamp conversion to 12-hour format
            dgvLogs.CellFormatting += DgvLogs_CellFormatting;
        }

        /// <summary>
        /// Initialize custom controls and styling
        /// </summary>
        private void InitializeCustomControls()
        {
            // Set form icon and styling
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.FromArgb(240, 240, 240);
            
            // Configure DataGridView styling
            dgvLogs.EnableHeadersVisualStyles = false;
            dgvLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvLogs.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvLogs.ColumnHeadersHeight = 35;
            
            // Set default filter to "All"
            if (cmbFilter != null && cmbFilter.Items.Count > 0)
            {
                cmbFilter.SelectedIndex = 0; // Select "All"
            }
            
            // Set tooltip for search
            if (txtSearch != null)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(txtSearch, "Search in all columns (Status, Floor, etc.)");
            }
        }

        /// <summary>
        /// Initialize BackgroundWorker for database operations
        /// TASK 5: Concurrency - prevent UI freezing
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            _dbWorker = new BackgroundWorker();
            _dbWorker.DoWork += DbWorker_DoWork;
            _dbWorker.RunWorkerCompleted += DbWorker_RunWorkerCompleted;
        }

        /// <summary>
        /// Form load event - load logs from database
        /// </summary>
        private void LogForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadLogs();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                // TASK 5: Exception handling
                MessageBox.Show($"Error loading logs: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load logs from database
        /// TASK 3: Display all stored operations
        /// </summary>
        private void LoadLogs()
        {
            try
            {
                // Show loading indicator
                lblStatus.Text = "⏳ Loading logs...";
                lblStatus.ForeColor = Color.Blue;
                lblStatus.Visible = true;
                btnRefresh.Enabled = false;
                if (btnExport != null) btnExport.Enabled = false;
                if (btnClearAll != null) btnClearAll.Enabled = false;

                // Load logs in background thread
                if (!_dbWorker.IsBusy)
                {
                    _dbWorker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initiating log load: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// BackgroundWorker DoWork - retrieve logs on background thread
        /// </summary>
        private void DbWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Get all logs from database
                DataTable logs = DatabaseHelper.Instance.GetAllLogs();
                e.Result = logs;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        /// <summary>
        /// BackgroundWorker Completed - update UI on main thread
        /// TASK 5: Use Invoke() for thread-safe GUI updates
        /// </summary>
        private void DbWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result is Exception ex)
                {
                    MessageBox.Show($"Error loading logs: {ex.Message}", 
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "❌ Error loading logs";
                    lblStatus.ForeColor = Color.Red;
                }
                else if (e.Result is DataTable logs)
                {
                    // Store original data for filtering
                    _originalData = logs.Copy();
                    
                    // Bind data to DataGridView
                    dgvLogs.DataSource = logs;
                    
                    // Format DataGridView
                    FormatDataGridView();
                    
                    // Update status and statistics
                    lblStatus.Text = $"✓ Loaded {logs.Rows.Count} log entries";
                    lblStatus.ForeColor = Color.Green;
                    UpdateStatistics();
                }

                btnRefresh.Enabled = true;
                if (btnExport != null) btnExport.Enabled = true;
                if (btnClearAll != null) btnClearAll.Enabled = dgvLogs.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying logs: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Format DataGridView for better appearance
        /// TASK 1: Professional GUI
        /// </summary>
        private void FormatDataGridView()
        {
            try
            {
                if (dgvLogs.Columns.Count > 0)
                {
                    // Database returns 4 columns: Log ID, Floor, Status, Timestamp
                    // Set column widths based on actual column count
                    if (dgvLogs.Columns.Count >= 1)
                    {
                        dgvLogs.Columns[0].Width = 80;  // Log ID
                        dgvLogs.Columns[0].HeaderText = "ID";
                    }
                    
                    if (dgvLogs.Columns.Count >= 2)
                    {
                        dgvLogs.Columns[1].Width = 100; // Floor
                        dgvLogs.Columns[1].HeaderText = "Floor";
                    }
                    
                    if (dgvLogs.Columns.Count >= 3)
                    {
                        dgvLogs.Columns[2].Width = 250; // Status
                        dgvLogs.Columns[2].HeaderText = "Status / Operation";
                    }
                    
                    if (dgvLogs.Columns.Count >= 4)
                    {
                        dgvLogs.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Timestamp
                        dgvLogs.Columns[3].HeaderText = "Timestamp";
                        // Format timestamp in 12-hour format
                        dgvLogs.Columns[3].DefaultCellStyle.Format = "MM/dd/yyyy hh:mm:ss tt";
                    }

                    // Alternate row colors for better readability
                    dgvLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                    dgvLogs.DefaultCellStyle.BackColor = Color.White;
                    dgvLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
                    dgvLogs.DefaultCellStyle.SelectionForeColor = Color.White;
                    dgvLogs.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
                    dgvLogs.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);
                    
                    // Center align numeric columns (Log ID and Floor)
                    if (dgvLogs.Columns.Count >= 1)
                        dgvLogs.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    
                    if (dgvLogs.Columns.Count >= 2)
                        dgvLogs.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Make read-only
                    dgvLogs.ReadOnly = true;
                    dgvLogs.AllowUserToAddRows = false;
                    dgvLogs.AllowUserToDeleteRows = false;
                    
                    // Additional formatting
                    dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvLogs.MultiSelect = false;
                    dgvLogs.RowHeadersVisible = false;
                    dgvLogs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvLogs.BorderStyle = BorderStyle.None;
                    dgvLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    dgvLogs.GridColor = Color.FromArgb(224, 224, 224);
                    dgvLogs.RowTemplate.Height = 32;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Format timestamp cells to 12-hour format
        /// </summary>
        private void DgvLogs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // Check if this is the Timestamp column (column index 3)
                if (e.ColumnIndex == 3 && e.Value != null)
                {
                    // Try to parse and format the timestamp
                    if (DateTime.TryParse(e.Value.ToString(), out DateTime timestamp))
                    {
                        // Format to 12-hour time with AM/PM
                        e.Value = timestamp.ToString("MM/dd/yyyy hh:mm:ss tt");
                        e.FormattingApplied = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting cell: {ex.Message}");
            }
        }

        /// <summary>
        /// Update statistics panel with log metrics
        /// </summary>
        private void UpdateStatistics()
        {
            try
            {
                if (_originalData == null || _originalData.Rows.Count == 0)
                {
                    if (lblTotalLogs != null) lblTotalLogs.Text = "Total: 0";
                    if (lblFloor1Count != null) lblFloor1Count.Text = "Floor 1: 0";
                    if (lblFloor2Count != null) lblFloor2Count.Text = "Floor 2: 0";
                    return;
                }

                int totalLogs = _originalData.Rows.Count;
                int floor1Count = _originalData.AsEnumerable()
                    .Count(row => row.Field<long>("Floor") == 1);
                int floor2Count = _originalData.AsEnumerable()
                    .Count(row => row.Field<long>("Floor") == 2);

                if (lblTotalLogs != null) lblTotalLogs.Text = $"📊 Total: {totalLogs}";
                if (lblFloor1Count != null) lblFloor1Count.Text = $"🔸 Floor 1: {floor1Count}";
                if (lblFloor2Count != null) lblFloor2Count.Text = $"🔹 Floor 2: {floor2Count}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating statistics: {ex.Message}");
            }
        }

        /// <summary>
        /// Refresh button click event
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
        }

        /// <summary>
        /// Search text changed - filter logs
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_originalData == null) return;

                string searchText = txtSearch.Text.Trim().ToLower();
                
                if (string.IsNullOrEmpty(searchText))
                {
                    dgvLogs.DataSource = _originalData;
                    lblStatus.Text = $"✓ Showing all {_originalData.Rows.Count} log entries";
                }
                else
                {
                    DataTable filteredData = _originalData.Clone();
                    foreach (DataRow row in _originalData.Rows)
                    {
                        bool match = false;
                        foreach (DataColumn col in _originalData.Columns)
                        {
                            if (row[col].ToString().ToLower().Contains(searchText))
                            {
                                match = true;
                                break;
                            }
                        }
                        if (match) filteredData.ImportRow(row);
                    }
                    
                    dgvLogs.DataSource = filteredData;
                    lblStatus.Text = $"🔍 Found {filteredData.Rows.Count} matching entries";
                    lblStatus.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching: {ex.Message}");
            }
        }

        /// <summary>
        /// Filter by floor
        /// </summary>
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_originalData == null || cmbFilter == null) return;

                _currentFilter = cmbFilter.SelectedItem?.ToString() ?? "All";
                
                if (_currentFilter == "All")
                {
                    dgvLogs.DataSource = _originalData;
                    lblStatus.Text = $"✓ Showing all {_originalData.Rows.Count} log entries";
                }
                else if (_currentFilter == "Floor 1")
                {
                    var filtered = _originalData.AsEnumerable()
                        .Where(row => row.Field<long>("Floor") == 1);
                    DataTable dt = filtered.Any() ? filtered.CopyToDataTable() : _originalData.Clone();
                    dgvLogs.DataSource = dt;
                    lblStatus.Text = $"🔸 Showing {dt.Rows.Count} Floor 1 entries";
                }
                else if (_currentFilter == "Floor 2")
                {
                    var filtered = _originalData.AsEnumerable()
                        .Where(row => row.Field<long>("Floor") == 2);
                    DataTable dt = filtered.Any() ? filtered.CopyToDataTable() : _originalData.Clone();
                    dgvLogs.DataSource = dt;
                    lblStatus.Text = $"🔹 Showing {dt.Rows.Count} Floor 2 entries";
                }
                
                lblStatus.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering: {ex.Message}");
            }
        }

        /// <summary>
        /// Export logs to CSV file
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvLogs.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export.", "Export", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    sfd.FileName = $"ElevatorLogs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        
                        // Headers
                        for (int i = 0; i < dgvLogs.Columns.Count; i++)
                        {
                            sb.Append(dgvLogs.Columns[i].HeaderText);
                            if (i < dgvLogs.Columns.Count - 1) sb.Append(",");
                        }
                        sb.AppendLine();
                        
                        // Data
                        foreach (DataGridViewRow row in dgvLogs.Rows)
                        {
                            for (int i = 0; i < dgvLogs.Columns.Count; i++)
                            {
                                sb.Append(row.Cells[i].Value?.ToString() ?? "");
                                if (i < dgvLogs.Columns.Count - 1) sb.Append(",");
                            }
                            sb.AppendLine();
                        }
                        
                        System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                        MessageBox.Show($"Exported {dgvLogs.Rows.Count} records successfully!\n\n{sfd.FileName}", 
                            "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", 
                    "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clear all logs from database
        /// </summary>
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete ALL log entries?\n\nThis action cannot be undone!",
                    "Confirm Delete All",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DatabaseHelper.Instance.ClearAllLogs();
                    MessageBox.Show("All logs have been cleared successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadLogs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing logs: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Close button click event
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Clean up resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbWorker?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
