using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace ElevatorControlSystem.Controllers
{
    /// <summary>
    /// Database Helper class implementing SQLite database for elevator logging.
    /// UPDATED: Uses DISCONNECTED MODEL with SQLiteDataAdapter and DataSet
    /// Implements thread-safe operations suitable for BackgroundWorker usage.
    /// Uses relative paths for portability.
    /// 
    /// TASK 3 REQUIREMENT: Disconnected model, relative paths, no code duplication
    /// </summary>
    public class DatabaseHelper
    {
        private string _connectionString;
        private string _databasePath;
        private static DatabaseHelper _instance;
        private static readonly object _lock = new object();
        
        // Disconnected model components
        private SQLiteDataAdapter _dataAdapter;
        private DataSet _dataSet;
        private DataTable _logsTable;

        /// <summary>
        /// Singleton pattern to ensure only one database helper instance
        /// </summary>
        public static DatabaseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private constructor - initializes SQLite database connection
        /// Uses relative path: ./Database/ElevatorLogs.db
        /// Automatically creates database and table if they don't exist
        /// </summary>
        private DatabaseHelper()
        {
            try
            {
                // Use relative path from application directory
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string databaseFolder = Path.Combine(appPath, "Database");
                
                // Ensure Database folder exists
                if (!Directory.Exists(databaseFolder))
                {
                    Directory.CreateDirectory(databaseFolder);
                }

                // Set database path
                _databasePath = Path.Combine(databaseFolder, "ElevatorLogs.db");

                // Create SQLite connection string
                _connectionString = $"Data Source={_databasePath};Version=3;";

                // Initialize database (create if doesn't exist)
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to initialize SQLite database:\n\n{ex.Message}",
                    "Database Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                throw;
            }
        }

        /// <summary>
        /// Initialize database file and create table if they don't exist
        /// Creates SQLite database file automatically
        /// Thread-safe operation
        /// </summary>
        private void InitializeDatabase()
        {
            lock (_lock)
            {
                try
                {
                    // Check if database file exists, if not create it
                    bool isNewDatabase = !File.Exists(_databasePath);

                    if (isNewDatabase)
                    {
                        // Create new SQLite database file
                        SQLiteConnection.CreateFile(_databasePath);
                    }

                    // Ensure table exists (create if missing)
                    CreateTableIfNotExists();
                    
                    // Initialize disconnected model components
                    InitializeDisconnectedModel();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to initialize database structure: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Create the Logs table if it doesn't exist
        /// Schema: Id (INTEGER PRIMARY KEY AUTOINCREMENT), Floor (INT), Status (TEXT), Timestamp (DATETIME)
        /// </summary>
        private void CreateTableIfNotExists()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Logs (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Floor INTEGER NOT NULL,
                            Status TEXT NOT NULL,
                            Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                    using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Logs table: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Initialize DataAdapter and DataSet for DISCONNECTED MODEL
        /// TASK 3 REQUIREMENT: Use DataAdapter.Update() instead of ExecuteNonQuery()
        /// </summary>
        private void InitializeDisconnectedModel()
        {
            try
            {
                _dataSet = new DataSet("ElevatorData");
                
                // Create DataAdapter with SELECT command
                // Connection string is used, not a connection object
                string selectQuery = "SELECT * FROM Logs";
                _dataAdapter = new SQLiteDataAdapter(selectQuery, _connectionString);
                
                // Create CommandBuilder to auto-generate INSERT, UPDATE, DELETE commands
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(_dataAdapter);
                
                // Fill the DataSet with existing data
                _dataAdapter.Fill(_dataSet, "Logs");
                _logsTable = _dataSet.Tables["Logs"];
                
                System.Diagnostics.Debug.WriteLine("[DISCONNECTED MODEL] DataAdapter and DataSet initialized successfully");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize disconnected model: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Insert a log entry for an elevator operation
        /// TASK 3 REQUIREMENT: Uses DISCONNECTED MODEL with DataAdapter.Update()
        /// Thread-safe operation suitable for BackgroundWorker
        /// </summary>
        /// <param name="status">Elevator status/operation description</param>
        /// <param name="floor">Current floor number</param>
        public void InsertLog(string status, int floor)
        {
            lock (_lock)
            {
                try
                {
                    // Refresh DataSet from database to ensure we have latest data
                    _dataSet.Tables["Logs"].Clear();
                    _dataAdapter.Fill(_dataSet, "Logs");
                    
                    // Create new row in DataTable (in-memory operation)
                    DataRow newRow = _logsTable.NewRow();
                    newRow["Floor"] = floor;
                    newRow["Status"] = status;
                    newRow["Timestamp"] = DateTime.Now;
                    
                    // Add row to DataTable
                    _logsTable.Rows.Add(newRow);

                    // DISCONNECTED MODEL: Update database using DataAdapter.Update()
                    // This is the KEY requirement - NOT using ExecuteNonQuery()
                    _dataAdapter.Update(_dataSet, "Logs");
                    
                    System.Diagnostics.Debug.WriteLine($"[DISCONNECTED MODEL] Log inserted: {status} at floor {floor}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to log operation: {ex.Message}");
                    throw new Exception($"Failed to log operation: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Compatibility method for legacy code
        /// Maps to InsertLog with appropriate parameters
        /// </summary>
        /// <param name="status">Current elevator status</param>
        /// <param name="currentFloor">Current floor</param>
        /// <param name="targetFloor">Target floor (not used in new schema)</param>
        /// <param name="description">Operation description</param>
        public void LogOperation(string status, int currentFloor, int targetFloor, string description)
        {
            // Use the new InsertLog method with combined status description
            string fullStatus = $"{status} - {description}";
            InsertLog(fullStatus, currentFloor);
        }

        /// <summary>
        /// Retrieve all elevator logs from database
        /// TASK 3 REQUIREMENT: Uses DISCONNECTED MODEL with DataAdapter
        /// Returns a DataTable suitable for binding to DataGridView
        /// Thread-safe operation
        /// </summary>
        /// <returns>DataTable containing all log entries ordered by timestamp (newest first)</returns>
        public DataTable GetLogs()
        {
            lock (_lock)
            {
                try
                {
                    // ✅ DISCONNECTED MODEL: Refresh DataSet from database
                    _dataSet.Tables["Logs"].Clear();
                    _dataAdapter.Fill(_dataSet, "Logs");
                    
                    // Create a sorted view for display (newest first)
                    DataView view = _logsTable.DefaultView;
                    view.Sort = "Timestamp DESC";
                    
                    // Return a copy of the DataTable
                    return view.ToTable();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to retrieve logs: {ex.Message}");
                    throw new Exception($"Failed to retrieve logs: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Compatibility method for legacy code
        /// Maps to GetLogs
        /// </summary>
        /// <returns>DataTable containing all log entries</returns>
        public DataTable GetAllLogs()
        {
            return GetLogs();
        }

        /// <summary>
        /// Clear all logs from the database
        /// TASK 3 REQUIREMENT: Uses DISCONNECTED MODEL with DataAdapter
        /// Thread-safe operation
        /// </summary>
        public void ClearAllLogs()
        {
            lock (_lock)
            {
                try
                {
                    // Refresh DataSet to get latest data
                    _dataSet.Tables["Logs"].Clear();
                    _dataAdapter.Fill(_dataSet, "Logs");
                    
                    // ✅ DISCONNECTED MODEL: Mark all rows for deletion
                    foreach (DataRow row in _logsTable.Rows)
                    {
                        row.Delete();
                    }
                    
                    // ✅ DISCONNECTED MODEL: Update database using DataAdapter.Update()
                    _dataAdapter.Update(_dataSet, "Logs");
                    
                    // Accept changes to clean up deleted rows
                    _dataSet.AcceptChanges();
                    
                    System.Diagnostics.Debug.WriteLine("[DISCONNECTED MODEL] All logs cleared successfully");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to clear logs: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Get the database file path
        /// </summary>
        public string DatabasePath
        {
            get { return _databasePath; }
        }
    }
}
