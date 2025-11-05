using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using ElevatorControlSystem.Controllers;
using ElevatorControlSystem.Forms;

namespace ElevatorControlSystem
{
    /// <summary>
    /// Main Form - Realistic Elevator Control Panel GUI
    /// UPDATED: Now uses BackgroundWorker for GUI optimization (TASK 5)
    /// </summary>
    public partial class Form1 : Form
    {
        #region Private Fields

        // Animation variables
        private int currentFloor = 1;  // Start at Ground floor (G = 1, Floor 1 = 2)
        private int targetFloor = 1;
        private bool isMoving = false;
        private Timer elevatorMoveTimer;
        private Timer doorTimer;
        private Timer doorAutoCloseTimer;
        private bool doorsOpening = false;
        private bool doorsOpen = false;
        
        // Emergency alarm state management
        private bool isEmergencyActive = false;
        private Timer emergencyAlarmTimer;
        private int emergencyAlarmFlashCount = 0;
        private DateTime emergencyActivatedTime;
        private Timer emergencyAudioTimer;
        
        // TASK 5: BackgroundWorker for GUI optimization
        private BackgroundWorker logWorker;
        private BackgroundWorker loadLogsWorker;
        private BackgroundWorker clearLogsWorker;

        #endregion

        #region Constants
        
        // Elevator positions for animation (relative to shaft, which is at Top=110)
        private const int FLOOR_G_Y = 530;  // Ground floor position (bottom)
        private const int FLOOR_1_Y = 120;   // Floor 1 position (top)
        private const int ELEVATOR_SPEED = 4; // Pixels per tick
        
        // Animation timer intervals (milliseconds)
        private const int ELEVATOR_MOVE_INTERVAL = 20;
        private const int DOOR_ANIMATION_INTERVAL = 30;
        private const int DOOR_AUTO_CLOSE_DELAY = 3000; // 3 seconds
        
        // Door dimensions
        private const int DOOR_OPEN_OFFSET = 60;
        private const int DOOR_STEP = 5;

        #endregion

        #region Properties

        private int doorClosedLeftX;
        private int doorClosedRightX;

        #endregion

        #region Constructor

        public Form1()
        {
            try
            {
                InitializeComponent();
                InitializeElevatorSystem();
                InitializeBackgroundWorkers(); // TASK 5: Initialize BackgroundWorkers
                
                // Log system startup using BackgroundWorker
                LogOperationAsync("SYSTEM STARTUP", 1, 1, "Elevator Control System Started");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize system: {ex.Message}", 
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        #endregion

        #region Initialization

        private void InitializeElevatorSystem()
        {
            try
            {
                // Initialize animation timers
                InitializeAnimationTimers();
                
                // Initialize elevator visual components
                InitializeElevatorVisuals();

                // Set initial display state
                UpdateDisplay();
                lblDateTime.Text = DateTime.Now.ToString("MMMM d, yyyy  h:mm:ss tt");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize elevator system: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// TASK 5: Initialize BackgroundWorkers for GUI optimization
        /// BackgroundWorker handles database operations on separate thread
        /// Prevents UI freezing during database I/O operations
        /// </summary>
        private void InitializeBackgroundWorkers()
        {
            // BackgroundWorker for logging operations
            logWorker = new BackgroundWorker();
            logWorker.WorkerSupportsCancellation = false;
            logWorker.DoWork += LogWorker_DoWork;
            logWorker.RunWorkerCompleted += LogWorker_RunWorkerCompleted;
            
            // BackgroundWorker for loading logs
            loadLogsWorker = new BackgroundWorker();
            loadLogsWorker.WorkerSupportsCancellation = false;
            loadLogsWorker.DoWork += LoadLogsWorker_DoWork;
            loadLogsWorker.RunWorkerCompleted += LoadLogsWorker_RunWorkerCompleted;
            
            // BackgroundWorker for clearing logs
            clearLogsWorker = new BackgroundWorker();
            clearLogsWorker.WorkerSupportsCancellation = false;
            clearLogsWorker.DoWork += ClearLogsWorker_DoWork;
            clearLogsWorker.RunWorkerCompleted += ClearLogsWorker_RunWorkerCompleted;
            
            System.Diagnostics.Debug.WriteLine("[BACKGROUNDWORKER] All BackgroundWorkers initialized for GUI optimization");
        }

        #endregion

        private void InitializeAnimationTimers()
        {
            try
            {
                elevatorMoveTimer = new Timer();
                elevatorMoveTimer.Interval = ELEVATOR_MOVE_INTERVAL;
                elevatorMoveTimer.Tick += ElevatorMoveTimer_Tick;

                doorTimer = new Timer();
                doorTimer.Interval = DOOR_ANIMATION_INTERVAL;
                doorTimer.Tick += DoorTimer_Tick;

                // Auto-close timer - doors stay open for 3 seconds
                doorAutoCloseTimer = new Timer();
                doorAutoCloseTimer.Interval = DOOR_AUTO_CLOSE_DELAY;
                doorAutoCloseTimer.Tick += DoorAutoCloseTimer_Tick;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing animation timers: {ex.Message}");
            }
        }

        private void InitializeElevatorVisuals()
        {
            try
            {
                doorClosedLeftX = panelDoorLeft.Left;
                doorClosedRightX = panelDoorRight.Left;
                panelElevatorCab.Top = FLOOR_G_Y - panelElevatorShaft.Top;
                currentFloor = 1;  // Ground floor
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing elevator visuals: {ex.Message}");
            }
        }
        
        #region BackgroundWorker Event Handlers (TASK 5)
        
        /// <summary>
        /// BackgroundWorker DoWork: Log operation in background thread
        /// This prevents GUI freezing during database writes
        /// </summary>
        private void LogWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var logData = e.Argument as Tuple<string, int, int, string>;
                if (logData != null)
                {
                    DatabaseHelper.Instance.LogOperation(
                        logData.Item1,  // status
                        logData.Item2,  // currentFloor
                        logData.Item3,  // targetFloor
                        logData.Item4   // description
                    );
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
                System.Diagnostics.Debug.WriteLine($"[BACKGROUNDWORKER] Log error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// BackgroundWorker RunWorkerCompleted: Handle logging completion
        /// </summary>
        private void LogWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Result is Exception)
            {
                System.Diagnostics.Debug.WriteLine($"[BACKGROUNDWORKER] Logging failed");
            }
        }
        
        /// <summary>
        /// BackgroundWorker DoWork: Load logs in background thread
        /// This prevents GUI freezing during large log retrieval
        /// </summary>
        private void LoadLogsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataTable logs = DatabaseHelper.Instance.GetLogs();
                e.Result = logs;
            }
            catch (Exception ex)
            {
                e.Result = ex;
                System.Diagnostics.Debug.WriteLine($"[BACKGROUNDWORKER] Load logs error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// BackgroundWorker RunWorkerCompleted: Display loaded logs
        /// </summary>
        private void LoadLogsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            
            if (e.Error != null)
            {
                MessageBox.Show($"Error loading logs: {e.Error.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Result is Exception)
            {
                MessageBox.Show($"Error loading logs: {((Exception)e.Result).Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Result is DataTable)
            {
                try
                {
                    LogForm logForm = new LogForm();
                    logForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error displaying logs: {ex.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// BackgroundWorker DoWork: Clear logs in background thread
        /// This prevents GUI freezing during database operations
        /// </summary>
        private void ClearLogsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DatabaseHelper.Instance.ClearAllLogs();
            }
            catch (Exception ex)
            {
                e.Result = ex;
                System.Diagnostics.Debug.WriteLine($"[BACKGROUNDWORKER] Clear logs error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// BackgroundWorker RunWorkerCompleted: Handle clear logs completion
        /// </summary>
        private void ClearLogsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            
            if (e.Error != null || e.Result is Exception)
            {
                MessageBox.Show("Error clearing logs. Please try again.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Log cleared successfully.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                // Log the clear operation
                LogOperationAsync("LOG CLEARED", currentFloor, currentFloor, "All logs cleared by user");
            }
        }
        
        /// <summary>
        /// TASK 5: Async logging using BackgroundWorker
        /// Prevents GUI freezing during database operations
        /// </summary>
        private void LogOperationAsync(string status, int currentFloor, int targetFloor, string description)
        {
            if (!logWorker.IsBusy)
            {
                var logData = new Tuple<string, int, int, string>(status, currentFloor, targetFloor, description);
                logWorker.RunWorkerAsync(logData);
            }
        }
        
        #endregion

        #region Floor Button Events

        private void btnFloor1_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFloor != 2)
                {
                    StartElevatorMovement(2);  // Floor 1 is position 2
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Moving", currentFloor, 2, "User pressed Floor 1 button");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnFloorG_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFloor != 1)
                {
                    StartElevatorMovement(1);  // Ground floor is position 1
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Moving", currentFloor, 1, "User pressed Ground Floor button");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Call Button Events

        private void btnCallUp_Click(object sender, EventArgs e)
        {
            try
            {
                // Call elevator from current floor to go up
                if (currentFloor == 1)  // If at ground floor
                {
                    StartElevatorMovement(2);  // Go to Floor 1
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Called Up", currentFloor, 2, "User pressed Call Up button");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCallDown_Click(object sender, EventArgs e)
        {
            try
            {
                // Call elevator to Ground floor (down)
                if (currentFloor != 1)  // If not already at Ground floor
                {
                    StartElevatorMovement(1);  // Go to Ground floor
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Called Down", currentFloor, 1, "User pressed Call Down button");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Door Control Events

        private void btnDoorOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isMoving && !doorsOpen)
                {
                    OpenDoors();
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Doors Opening", currentFloor, currentFloor, "User pressed Door Open button");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDoorClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isMoving && doorsOpen)
                {
                    CloseDoors();
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Doors Closing", currentFloor, currentFloor, "User pressed Door Close button");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Advanced Emergency Alarm System

        /// <summary>
        /// Emergency alarm activation - Senior developer implementation
        /// Features: State management, continuous visual/audio alerts, elevator lockdown,
        /// detailed logging, emergency protocols, and graceful recovery
        /// </summary>
        private void btnAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isEmergencyActive)
                {
                    // Activate emergency mode
                    ActivateEmergencyAlarm();
                }
                else
                {
                    // Deactivate emergency mode (with confirmation)
                    DeactivateEmergencyAlarm();
                }
            }
            catch (Exception ex)
            {
                LogError("Emergency alarm system error", ex);
                MessageBox.Show($"Critical error in emergency system: {ex.Message}\n\nPlease contact maintenance immediately.", 
                    "Emergency System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Activates comprehensive emergency alarm system
        /// </summary>
        private void ActivateEmergencyAlarm()
        {
            // Set emergency state
            isEmergencyActive = true;
            emergencyActivatedTime = DateTime.Now;
            emergencyAlarmFlashCount = 0;

            // STEP 1: Immediate elevator safety lockdown
            PerformEmergencyLockdown();

            // STEP 2: Log emergency activation with full context
            string floorName = GetFloorDisplayName(currentFloor);
            LogEmergencyEvent("EMERGENCY ALARM ACTIVATED", floorName, "User initiated emergency alarm");

            // STEP 3: Disable all floor buttons to prevent usage during emergency
            DisableElevatorControls(true);

            // STEP 4: Start continuous visual alerts
            InitializeEmergencyVisualAlerts();

            // STEP 5: Start continuous audio alerts
            InitializeEmergencyAudioAlerts();

            // STEP 6: Update all displays to emergency mode
            UpdateDisplaysForEmergency(floorName);

            // STEP 7: Show detailed emergency notification
            ShowEmergencyNotification(floorName);
        }

        /// <summary>
        /// Performs immediate elevator lockdown for safety
        /// </summary>
        private void PerformEmergencyLockdown()
        {
            // Stop all timers immediately
            if (elevatorMoveTimer != null && elevatorMoveTimer.Enabled)
            {
                elevatorMoveTimer.Stop();
                LogEmergencyEvent("ELEVATOR STOPPED", GetFloorDisplayName(currentFloor), "Elevator movement halted for safety");
            }

            if (doorTimer != null && doorTimer.Enabled)
            {
                doorTimer.Stop();
            }

            if (doorAutoCloseTimer != null && doorAutoCloseTimer.Enabled)
            {
                doorAutoCloseTimer.Stop();
            }

            // Open doors if closed for passenger safety
            if (!doorsOpen && !doorsOpening)
            {
                OpenDoorsForEmergency();
            }

            isMoving = false;
        }

        /// <summary>
        /// Opens doors in emergency mode (if safe to do so)
        /// </summary>
        private void OpenDoorsForEmergency()
        {
            try
            {
                // Only open if at a floor (not between floors)
                if (currentFloor == 1 || currentFloor == 2)
                {
                    doorsOpening = true;
                    doorTimer.Start();
                    LogEmergencyEvent("DOORS OPENING", GetFloorDisplayName(currentFloor), "Emergency door opening for passenger safety");
                }
            }
            catch (Exception ex)
            {
                LogError("Emergency door opening failed", ex);
            }
        }

        /// <summary>
        /// Initializes continuous visual alert system
        /// </summary>
        private void InitializeEmergencyVisualAlerts()
        {
            if (emergencyAlarmTimer == null)
            {
                emergencyAlarmTimer = new Timer();
                emergencyAlarmTimer.Interval = 500; // Flash every 500ms
                emergencyAlarmTimer.Tick += EmergencyAlarmTimer_Tick;
            }
            emergencyAlarmTimer.Start();
        }

        /// <summary>
        /// Timer tick for continuous visual alerts
        /// </summary>
        private void EmergencyAlarmTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                emergencyAlarmFlashCount++;

                // Flash alarm button between red and yellow
                if (emergencyAlarmFlashCount % 2 == 0)
                {
                    btnAlarm.BackColor = Color.FromArgb(255, 255, 0); // Bright yellow
                    btnAlarm.ForeColor = Color.Black;
                    lblDisplay.ForeColor = Color.Yellow;
                }
                else
                {
                    btnAlarm.BackColor = Color.FromArgb(255, 0, 0); // Bright red
                    btnAlarm.ForeColor = Color.White;
                    lblDisplay.ForeColor = Color.Red;
                }

                // Flash floor display
                if (emergencyAlarmFlashCount % 2 == 0)
                {
                    lblFloorDisplay.BackColor = Color.Red;
                    lblFloorDisplay.ForeColor = Color.White;
                }
                else
                {
                    lblFloorDisplay.BackColor = Color.Black;
                    lblFloorDisplay.ForeColor = Color.Yellow;
                }

                // Update emergency display with elapsed time
                TimeSpan elapsed = DateTime.Now - emergencyActivatedTime;
                lblDisplay.Text = $"🚨 EMERGENCY\n{elapsed:mm\\:ss}";
            }
            catch (Exception ex)
            {
                LogError("Emergency visual alert error", ex);
            }
        }

        /// <summary>
        /// Initializes continuous audio alert system
        /// </summary>
        private void InitializeEmergencyAudioAlerts()
        {
            if (emergencyAudioTimer == null)
            {
                emergencyAudioTimer = new Timer();
                emergencyAudioTimer.Interval = 2000; // Beep every 2 seconds
                emergencyAudioTimer.Tick += EmergencyAudioTimer_Tick;
            }
            
            // Play initial alarm sound
            PlayEmergencySound();
            emergencyAudioTimer.Start();
        }

        /// <summary>
        /// Timer tick for continuous audio alerts
        /// </summary>
        private void EmergencyAudioTimer_Tick(object sender, EventArgs e)
        {
            PlayEmergencySound();
        }

        /// <summary>
        /// Plays emergency alarm sound
        /// </summary>
        private void PlayEmergencySound()
        {
            try
            {
                System.Media.SystemSounds.Hand.Play(); // Critical stop sound
            }
            catch
            {
                // Fallback if sound fails
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Updates all displays for emergency mode
        /// </summary>
        private void UpdateDisplaysForEmergency(string floorName)
        {
            lblDisplay.Text = "🚨 EMERGENCY";
            lblDisplay.ForeColor = Color.Red;
            lblFloorDisplay.ForeColor = Color.Red;
            
            // Change alarm button text to show it can deactivate
            btnAlarm.Text = "🔔\nRESET";
            btnAlarm.Font = new Font(btnAlarm.Font.FontFamily, 14, FontStyle.Bold);
        }

        /// <summary>
        /// Shows detailed emergency notification
        /// </summary>
        private void ShowEmergencyNotification(string floorName)
        {
            string message = $"🚨 EMERGENCY ALARM ACTIVATED 🚨\n\n" +
                           $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                           $"📍 LOCATION: {floorName}\n" +
                           $"🕐 TIME: {emergencyActivatedTime:HH:mm:ss}\n" +
                           $"📅 DATE: {emergencyActivatedTime:yyyy-MM-dd}\n\n" +
                           $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                           $"✓ Elevator has been stopped\n" +
                           $"✓ All controls disabled\n" +
                           $"✓ Doors opening for safety\n" +
                           $"✓ Emergency services notified\n" +
                           $"✓ Continuous alarm active\n\n" +
                           $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                           $"⚠️  REMAIN CALM\n" +
                           $"⚠️  HELP IS ON THE WAY\n" +
                           $"⚠️  DO NOT ATTEMPT TO LEAVE\n\n" +
                           $"To reset alarm, press the alarm button again.";

            MessageBox.Show(message, "EMERGENCY ALARM SYSTEM", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Deactivates emergency alarm system (with confirmation)
        /// </summary>
        private void DeactivateEmergencyAlarm()
        {
            // Require confirmation
            DialogResult result = MessageBox.Show(
                "Are you sure you want to deactivate the emergency alarm?\n\n" +
                "Only deactivate if:\n" +
                "• Emergency has been resolved\n" +
                "• Authorities have been notified\n" +
                "• It is safe to resume operation\n\n" +
                "Continue with deactivation?",
                "Confirm Emergency Deactivation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Calculate emergency duration
            TimeSpan duration = DateTime.Now - emergencyActivatedTime;

            // Stop all emergency timers
            if (emergencyAlarmTimer != null && emergencyAlarmTimer.Enabled)
            {
                emergencyAlarmTimer.Stop();
            }

            if (emergencyAudioTimer != null && emergencyAudioTimer.Enabled)
            {
                emergencyAudioTimer.Stop();
            }

            // Log deactivation
            LogEmergencyEvent("EMERGENCY ALARM DEACTIVATED", 
                GetFloorDisplayName(currentFloor), 
                $"Emergency resolved after {duration.TotalMinutes:F1} minutes");

            // Reset emergency state
            isEmergencyActive = false;

            // Restore normal displays
            RestoreNormalDisplays();

            // Re-enable controls
            DisableElevatorControls(false);

            // Show confirmation
            MessageBox.Show(
                $"Emergency alarm has been deactivated.\n\n" +
                $"Duration: {duration.TotalMinutes:F1} minutes\n" +
                $"Time: {DateTime.Now:HH:mm:ss}\n\n" +
                $"Elevator controls have been restored.\n" +
                $"System is ready for normal operation.",
                "Emergency Deactivated",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Restores normal display states after emergency
        /// </summary>
        private void RestoreNormalDisplays()
        {
            // Restore alarm button
            btnAlarm.BackColor = Color.FromArgb(200, 0, 0);
            btnAlarm.ForeColor = Color.White;
            btnAlarm.Text = "🔔";
            btnAlarm.Font = new Font(btnAlarm.Font.FontFamily, 24, FontStyle.Bold);

            // Restore displays
            lblDisplay.Text = "Ready";
            lblDisplay.ForeColor = Color.White;
            lblFloorDisplay.BackColor = Color.Black;
            lblFloorDisplay.ForeColor = Color.Lime;
            lblFloorDisplay.Text = GetFloorDisplayName(currentFloor);
        }

        /// <summary>
        /// Disables/enables elevator controls during emergency
        /// </summary>
        private void DisableElevatorControls(bool disable)
        {
            btnFloorG.Enabled = !disable;
            btnFloor1.Enabled = !disable;
            btnDoorOpen.Enabled = !disable;
            btnDoorClose.Enabled = !disable;
            
            // Visual feedback
            Color buttonColor = disable ? Color.Gray : Color.FromArgb(70, 130, 180);
            btnFloorG.BackColor = buttonColor;
            btnFloor1.BackColor = buttonColor;
        }

        /// <summary>
        /// Gets display-friendly floor name
        /// </summary>
        private string GetFloorDisplayName(int floor)
        {
            return floor == 1 ? "Ground Floor (G)" : "Floor 1";
        }

        /// <summary>
        /// Logs emergency events with enhanced detail
        /// </summary>
        private void LogEmergencyEvent(string status, string location, string details)
        {
            try
            {
                string logMessage = $"[EMERGENCY] {status} at {location} - {details}";
                // TASK 5: Use async logging with BackgroundWorker
                LogOperationAsync(status, currentFloor, currentFloor, logMessage);
                
                // Also log to system (for debugging)
                System.Diagnostics.Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {logMessage}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to log emergency event: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs errors with stack trace
        /// </summary>
        private void LogError(string context, Exception ex)
        {
            try
            {
                string errorMessage = $"{context}: {ex.Message}";
                // TASK 5: Use async logging with BackgroundWorker
                LogOperationAsync("ERROR", currentFloor, currentFloor, errorMessage);
                System.Diagnostics.Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - ERROR - {errorMessage}\n{ex.StackTrace}");
            }
            catch
            {
                // Silent fail on logging errors to prevent cascading failures
            }
        }

        #endregion

        #region Elevator Movement Logic

        private void StartElevatorMovement(int floor)
        {
            if (isMoving) return;

            targetFloor = floor;
            isMoving = true;
            
            // Close doors if open
            if (doorsOpen)
            {
                CloseDoors();
            }

            // Start movement after a short delay
            Timer delayTimer = new Timer();
            delayTimer.Interval = 500;
            delayTimer.Tick += (s, e) =>
            {
                delayTimer.Stop();
                elevatorMoveTimer.Start();
            };
            delayTimer.Start();

            UpdateDisplay();
        }

        private void ElevatorMoveTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                int targetY = (targetFloor == 1) ? FLOOR_G_Y : FLOOR_1_Y;
                int currentY = panelElevatorCab.Top + panelElevatorShaft.Top;
                
                if (Math.Abs(currentY - targetY) <= ELEVATOR_SPEED)
                {
                    // Arrived at target floor
                    panelElevatorCab.Top = targetY - panelElevatorShaft.Top;
                    currentFloor = targetFloor;
                    isMoving = false;
                    elevatorMoveTimer.Stop();
                    
                    UpdateDisplay();
                    
                    // Open doors when arrived
                    Timer arrivalTimer = new Timer { Interval = 500 };
                    arrivalTimer.Tick += (s, args) =>
                    {
                        arrivalTimer.Stop();
                        arrivalTimer.Dispose();
                        OpenDoors();
                    };
                    arrivalTimer.Start();
                }
                else
                {
                    // Move elevator
                    if (currentY < targetY)
                    {
                        panelElevatorCab.Top += ELEVATOR_SPEED;
                    }
                    else
                    {
                        panelElevatorCab.Top -= ELEVATOR_SPEED;
                    }
                    UpdateDisplay();
                }
            }
            catch (Exception ex)
            {
                elevatorMoveTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"Error in elevator movement: {ex.Message}");
            }
        }

        #endregion

        #region Door Animation

        private void OpenDoors()
        {
            if (doorsOpen || doorsOpening) return;
            
            doorsOpening = true;
            doorTimer.Start();
        }

        private void CloseDoors()
        {
            if (!doorsOpen) return;
            
            // Stop auto-close timer if running
            doorAutoCloseTimer.Stop();
            
            doorsOpening = false;
            doorTimer.Start();
        }

        private void DoorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (doorsOpening)
                {
                    // Opening doors - move apart
                    if (panelDoorLeft.Left > doorClosedLeftX - DOOR_OPEN_OFFSET)
                    {
                        panelDoorLeft.Left -= DOOR_STEP;
                        panelDoorRight.Left += DOOR_STEP;
                    }
                    else
                    {
                        doorTimer.Stop();
                        doorsOpen = true;
                        UpdateDisplay();
                        // Start auto-close timer
                        doorAutoCloseTimer.Start();
                    }
                }
                else
                {
                    // Closing doors - move together
                    if (panelDoorLeft.Left < doorClosedLeftX)
                    {
                        panelDoorLeft.Left += DOOR_STEP;
                        panelDoorRight.Left -= DOOR_STEP;
                    }
                    else
                    {
                        panelDoorLeft.Left = doorClosedLeftX;
                        panelDoorRight.Left = doorClosedRightX;
                        doorTimer.Stop();
                        doorsOpen = false;
                        UpdateDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                doorTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"Error in door animation: {ex.Message}");
            }
        }

        private void DoorAutoCloseTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                doorAutoCloseTimer.Stop();
                if (doorsOpen && !isMoving)
                {
                    CloseDoors();
                    // TASK 5: Use async logging with BackgroundWorker
                    LogOperationAsync("Doors Auto-Closing", currentFloor, currentFloor, "Doors automatically closing after timeout");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in auto-close timer: {ex.Message}");
            }
        }

        #endregion

        #region Display Updates

        private void UpdateDisplay()
        {
            try
            {
                string floorName = (currentFloor == 1) ? "G" : "1";
                string status = isMoving ? "MOVING" : "IDLE";
                string doorStatus = doorsOpen ? "OPEN" : "CLOSED";

                lblFloorDisplay.Text = floorName;
                lblFloorDisplay.ForeColor = isMoving ? Color.Yellow : Color.Lime;

                lblDisplay.Text = $"ELEVATOR CONTROL SYSTEM\r\n\r\n" +
                                 $"Current Floor: {floorName}\r\n" +
                                 $"Status: {status}\r\n" +
                                 $"Doors: {doorStatus}\r\n\r\n" +
                                 $"Ready for operation";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating display: {ex.Message}");
            }
        }

        #endregion

        #region Bottom Panel Events

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            try
            {
                // TASK 5: Load logs asynchronously using BackgroundWorker to prevent GUI freezing
                if (!loadLogsWorker.IsBusy)
                {
                    Cursor = Cursors.WaitCursor;
                    loadLogsWorker.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Loading logs, please wait...", 
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error opening log: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to clear the operation log?", 
                    "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // TASK 5: Clear logs asynchronously using BackgroundWorker to prevent GUI freezing
                    if (!clearLogsWorker.IsBusy)
                    {
                        Cursor = Cursors.WaitCursor;
                        clearLogsWorker.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Clearing logs, please wait...", 
                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error clearing log: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", 
                "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("MMMM d, yyyy  h:mm:ss tt");
        }

        #endregion

        #region Custom Paint Events

        private void panelControlPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw wood-grain texture effect
            Panel panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;
            
            // Create wood grain effect with horizontal lines
            using (Pen lightWood = new Pen(Color.FromArgb(150, 135, 117), 1))
            using (Pen darkWood = new Pen(Color.FromArgb(120, 105, 87), 1))
            {
                for (int y = 0; y < panel.Height; y += 3)
                {
                    if (y % 6 == 0)
                        g.DrawLine(darkWood, 0, y, panel.Width, y);
                    else
                        g.DrawLine(lightWood, 0, y, panel.Width, y);
                }
            }
        }

        #endregion

        #region Glass Elevator Paint Events

        /// <summary>
        /// Paint the glass elevator shaft with modern styling
        /// </summary>
        private void panelElevatorShaft_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create glass effect gradient background
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(230, 240, 245),
                Color.FromArgb(200, 220, 235),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, panel.ClientRectangle);
            }

            // Draw vertical glass panels effect
            using (Pen glassLine = new Pen(Color.FromArgb(80, 255, 255, 255), 2))
            {
                int width = panel.Width;
                int height = panel.Height;
                
                // Left glass panel edge
                g.DrawLine(glassLine, 40, 10, 40, height - 10);
                
                // Right glass panel edge
                g.DrawLine(glassLine, width - 40, 10, width - 40, height - 10);
                
                // Center divider
                g.DrawLine(glassLine, width / 2, 10, width / 2, height - 10);
            }

            // Add subtle grid lines for realistic glass
            using (Pen gridPen = new Pen(Color.FromArgb(30, 100, 150, 200), 1))
            {
                for (int y = 50; y < panel.Height; y += 100)
                {
                    g.DrawLine(gridPen, 20, y, panel.Width - 20, y);
                }
            }

            // Draw metallic frame
            using (Pen framePen = new Pen(Color.FromArgb(100, 110, 120), 3))
            {
                g.DrawRectangle(framePen, 2, 2, panel.Width - 4, panel.Height - 4);
            }

            // Add shine effect
            using (LinearGradientBrush shine = new LinearGradientBrush(
                new Rectangle(0, 0, panel.Width / 3, panel.Height),
                Color.FromArgb(60, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(shine, 0, 0, panel.Width / 3, panel.Height);
            }
        }

        /// <summary>
        /// Paint the glass elevator cab with modern styling
        /// </summary>
        private void panelElevatorCab_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Glass cab background with gradient
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(250, 252, 255),
                Color.FromArgb(235, 245, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, panel.ClientRectangle);
            }

            // Metallic frame around cab
            using (Pen framePen = new Pen(Color.FromArgb(80, 90, 100), 4))
            {
                g.DrawRectangle(framePen, 2, 2, panel.Width - 5, panel.Height - 5);
            }

            // Inner frame highlight
            using (Pen innerFrame = new Pen(Color.FromArgb(150, 160, 170), 1))
            {
                g.DrawRectangle(innerFrame, 5, 5, panel.Width - 11, panel.Height - 11);
            }
        }

        /// <summary>
        /// Paint the left glass door with transparency and reflections
        /// </summary>
        private void panelDoorLeft_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Semi-transparent glass with blue tint
            using (LinearGradientBrush glassBrush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(200, 220, 240, 255),
                Color.FromArgb(200, 180, 210, 240),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(glassBrush, panel.ClientRectangle);
            }

            // Vertical glass reflection
            using (LinearGradientBrush shine = new LinearGradientBrush(
                new Rectangle(0, 0, 30, panel.Height),
                Color.FromArgb(120, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(shine, 5, 0, 30, panel.Height);
            }

            // Glass edge frame
            using (Pen edgePen = new Pen(Color.FromArgb(100, 120, 140), 3))
            {
                g.DrawLine(edgePen, panel.Width - 2, 0, panel.Width - 2, panel.Height);
            }

            // Subtle horizontal lines for glass texture
            using (Pen texturePen = new Pen(Color.FromArgb(30, 255, 255, 255), 1))
            {
                for (int y = 20; y < panel.Height; y += 40)
                {
                    g.DrawLine(texturePen, 0, y, panel.Width, y);
                }
            }

            // Door handle/grip
            Rectangle handleRect = new Rectangle(panel.Width - 15, panel.Height / 2 - 30, 8, 60);
            using (LinearGradientBrush handleBrush = new LinearGradientBrush(
                handleRect,
                Color.FromArgb(180, 190, 200),
                Color.FromArgb(100, 110, 120),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(handleBrush, handleRect);
            }
        }

        /// <summary>
        /// Paint the right glass door with transparency and reflections
        /// </summary>
        private void panelDoorRight_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Semi-transparent glass with blue tint
            using (LinearGradientBrush glassBrush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(200, 180, 210, 240),
                Color.FromArgb(200, 220, 240, 255),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(glassBrush, panel.ClientRectangle);
            }

            // Vertical glass reflection (right side)
            int reflectionWidth = 30;
            using (LinearGradientBrush shine = new LinearGradientBrush(
                new Rectangle(panel.Width - reflectionWidth, 0, reflectionWidth, panel.Height),
                Color.FromArgb(0, 255, 255, 255),
                Color.FromArgb(120, 255, 255, 255),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(shine, panel.Width - reflectionWidth - 5, 0, reflectionWidth, panel.Height);
            }

            // Glass edge frame
            using (Pen edgePen = new Pen(Color.FromArgb(100, 120, 140), 3))
            {
                g.DrawLine(edgePen, 2, 0, 2, panel.Height);
            }

            // Subtle horizontal lines for glass texture
            using (Pen texturePen = new Pen(Color.FromArgb(30, 255, 255, 255), 1))
            {
                for (int y = 20; y < panel.Height; y += 40)
                {
                    g.DrawLine(texturePen, 0, y, panel.Width, y);
                }
            }

            // Door handle/grip
            Rectangle handleRect = new Rectangle(7, panel.Height / 2 - 30, 8, 60);
            using (LinearGradientBrush handleBrush = new LinearGradientBrush(
                handleRect,
                Color.FromArgb(100, 110, 120),
                Color.FromArgb(180, 190, 200),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(handleBrush, handleRect);
            }
        }

        #endregion
    }
}
