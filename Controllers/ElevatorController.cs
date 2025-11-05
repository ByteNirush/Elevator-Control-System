using System;
using System.ComponentModel;
using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Controllers
{
    /// <summary>
    /// Elevator Controller - Orchestrates elevator operations
    /// Separates business logic from GUI (TASK 2 REQUIREMENT)
    /// Handles state transitions, animations, and database logging
    /// Uses BackgroundWorker for concurrent database operations (TASK 5)
    /// </summary>
    public class ElevatorController
    {
        private Elevator _elevator;
        private System.Windows.Forms.Timer _movementTimer;
        private System.Windows.Forms.Timer _doorTimer;
        private BackgroundWorker _dbWorker;

        // Animation parameters
        private const int MOVEMENT_DURATION = 2000; // 2 seconds to move between floors
        private const int DOOR_OPERATION_DURATION = 1000; // 1 second for door operations
        private const int DOOR_OPEN_DURATION = 1500; // 1.5 seconds doors stay open

        /// <summary>
        /// Event fired when GUI needs to be updated
        /// Delegates are used for event-driven programming (TASK 4)
        /// </summary>
        public event Action<int, string, string> OnDisplayUpdate;

        /// <summary>
        /// Event fired when database operation completes
        /// </summary>
        public event Action<bool, string> OnDatabaseOperationComplete;

        /// <summary>
        /// Gets the elevator instance
        /// </summary>
        public Elevator Elevator
        {
            get { return _elevator; }
        }

        /// <summary>
        /// Constructor - initializes controller and elevator
        /// </summary>
        public ElevatorController()
        {
            try
            {
                // Initialize elevator with state pattern
                _elevator = new Elevator();

                // Subscribe to elevator events
                _elevator.OnFloorChanged += OnFloorChanged;
                _elevator.OnStateChanged += OnStateChanged;

                // Initialize timers for animation (TASK 4)
                InitializeTimers();

                // Initialize BackgroundWorker for database operations (TASK 5)
                InitializeBackgroundWorker();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize elevator controller: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Initialize timers for animation
        /// TASK 4: Timer and Delegates for animation
        /// </summary>
        private void InitializeTimers()
        {
            // Movement timer for elevator animation
            _movementTimer = new System.Windows.Forms.Timer();
            _movementTimer.Interval = MOVEMENT_DURATION;
            _movementTimer.Tick += MovementTimer_Tick;

            // Door timer for door operations
            _doorTimer = new System.Windows.Forms.Timer();
            _doorTimer.Tick += DoorTimer_Tick;
        }

        /// <summary>
        /// Initialize BackgroundWorker for concurrent database operations
        /// TASK 5: Concurrency using BackgroundWorker
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            _dbWorker = new BackgroundWorker();
            _dbWorker.DoWork += DbWorker_DoWork;
            _dbWorker.RunWorkerCompleted += DbWorker_RunWorkerCompleted;
        }

        /// <summary>
        /// Handle floor request from GUI
        /// TASK 2: Separate GUI logic from control logic
        /// </summary>
        /// <param name="targetFloor">Requested floor</param>
        public void RequestFloor(int targetFloor)
        {
            try
            {
                // Validate floor
                if (targetFloor < 1 || targetFloor > 2)
                {
                    throw new ArgumentException("Invalid floor. Must be 1 or 2.");
                }

                // Check if already at target floor
                if (_elevator.CurrentFloor == targetFloor && 
                    _elevator.GetStatus() == ElevatorStatus.Idle)
                {
                    // Just open doors
                    _elevator.RequestFloor(targetFloor);
                    StartDoorOpeningSequence();
                }
                else if (_elevator.CurrentFloor != targetFloor &&
                         _elevator.GetStatus() == ElevatorStatus.Idle)
                {
                    // Need to move
                    _elevator.RequestFloor(targetFloor);
                    StartMovementAnimation(targetFloor);
                }
                // Else: Elevator is busy, request is ignored by state pattern
            }
            catch (Exception ex)
            {
                // TASK 5: Exception handling
                System.Diagnostics.Debug.WriteLine($"Error in RequestFloor: {ex.Message}");
                OnDatabaseOperationComplete?.Invoke(false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Start elevator movement animation
        /// TASK 4: Animation using Timer
        /// </summary>
        private void StartMovementAnimation(int targetFloor)
        {
            try
            {
                // Log operation to database (background operation)
                LogToDatabaseAsync("Moving", _elevator.CurrentFloor, targetFloor, 
                    $"Moving from floor {_elevator.CurrentFloor} to floor {targetFloor}");

                // Start movement timer
                _movementTimer.Tag = targetFloor; // Store target floor
                _movementTimer.Start();

                // Update display
                UpdateDisplay();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in StartMovementAnimation: {ex.Message}");
            }
        }

        /// <summary>
        /// Movement timer tick event - elevator reaches destination
        /// </summary>
        private void MovementTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _movementTimer.Stop();

                // Get target floor from timer tag
                int targetFloor = (int)_movementTimer.Tag;

                // Update elevator floor
                _elevator.SetFloor(targetFloor);

                // Log arrival
                LogToDatabaseAsync("Arrived", targetFloor, targetFloor, 
                    $"Arrived at floor {targetFloor}");

                // Transition to door opening
                _elevator.CurrentState = new DoorsOpeningState();
                StartDoorOpeningSequence();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MovementTimer_Tick: {ex.Message}");
            }
        }

        /// <summary>
        /// Start door opening sequence
        /// </summary>
        private void StartDoorOpeningSequence()
        {
            try
            {
                // Log door opening
                LogToDatabaseAsync("DoorsOpening", _elevator.CurrentFloor, _elevator.CurrentFloor,
                    $"Doors opening at floor {_elevator.CurrentFloor}");

                // Update display
                UpdateDisplay();

                // Start door timer
                _doorTimer.Interval = DOOR_OPERATION_DURATION;
                _doorTimer.Tag = "Opening";
                _doorTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in StartDoorOpeningSequence: {ex.Message}");
            }
        }

        /// <summary>
        /// Door timer tick event - handles door state transitions
        /// </summary>
        private void DoorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _doorTimer.Stop();

                string doorState = _doorTimer.Tag as string;

                if (doorState == "Opening")
                {
                    // Doors finished opening
                    _elevator.CurrentState = new DoorsOpenState();
                    LogToDatabaseAsync("DoorsOpen", _elevator.CurrentFloor, _elevator.CurrentFloor,
                        $"Doors open at floor {_elevator.CurrentFloor}");
                    UpdateDisplay();

                    // Wait, then start closing
                    _doorTimer.Interval = DOOR_OPEN_DURATION;
                    _doorTimer.Tag = "Open";
                    _doorTimer.Start();
                }
                else if (doorState == "Open")
                {
                    // Start closing doors
                    _elevator.CurrentState = new DoorsClosingState();
                    LogToDatabaseAsync("DoorsClosing", _elevator.CurrentFloor, _elevator.CurrentFloor,
                        $"Doors closing at floor {_elevator.CurrentFloor}");
                    UpdateDisplay();

                    _doorTimer.Interval = DOOR_OPERATION_DURATION;
                    _doorTimer.Tag = "Closing";
                    _doorTimer.Start();
                }
                else if (doorState == "Closing")
                {
                    // Doors finished closing - return to idle
                    _elevator.CurrentState = new IdleState();
                    LogToDatabaseAsync("Idle", _elevator.CurrentFloor, _elevator.CurrentFloor,
                        $"Elevator idle at floor {_elevator.CurrentFloor}");
                    UpdateDisplay();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DoorTimer_Tick: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle floor changed event from elevator
        /// </summary>
        private void OnFloorChanged(int newFloor)
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Handle state changed event from elevator
        /// </summary>
        private void OnStateChanged(ElevatorStatus status, string stateName)
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Update display through event delegation
        /// TASK 4: Delegates for updating display
        /// </summary>
        private void UpdateDisplay()
        {
            try
            {
                string status = _elevator.GetStateName();
                string floorInfo = $"Floor {_elevator.CurrentFloor}";
                
                // Fire event to update GUI
                OnDisplayUpdate?.Invoke(_elevator.CurrentFloor, status, floorInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateDisplay: {ex.Message}");
            }
        }

        /// <summary>
        /// Log operation to database asynchronously
        /// TASK 5: BackgroundWorker for concurrency
        /// </summary>
        private void LogToDatabaseAsync(string status, int currentFloor, int targetFloor, string description)
        {
            try
            {
                // Only start new operation if worker is not busy
                if (!_dbWorker.IsBusy)
                {
                    var logData = new
                    {
                        Status = status,
                        CurrentFloor = currentFloor,
                        TargetFloor = targetFloor,
                        Description = description
                    };

                    _dbWorker.RunWorkerAsync(logData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LogToDatabaseAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// BackgroundWorker DoWork event - performs database operation on background thread
        /// TASK 5: Concurrency implementation
        /// </summary>
        private void DbWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                dynamic logData = e.Argument;
                
                // Perform database operation on background thread
                DatabaseHelper.Instance.LogOperation(
                    logData.Status,
                    logData.CurrentFloor,
                    logData.TargetFloor,
                    logData.Description
                );

                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = false;
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }
        }

        /// <summary>
        /// BackgroundWorker RunWorkerCompleted event - called on UI thread
        /// </summary>
        private void DbWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                bool success = e.Result != null && (bool)e.Result;
                string message = success ? "Operation logged" : "Failed to log operation";
                
                // Fire event on UI thread
                OnDatabaseOperationComplete?.Invoke(success, message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DbWorker_RunWorkerCompleted: {ex.Message}");
            }
        }

        /// <summary>
        /// Clean up resources
        /// </summary>
        public void Dispose()
        {
            _movementTimer?.Dispose();
            _doorTimer?.Dispose();
            _dbWorker?.Dispose();
        }
    }
}
