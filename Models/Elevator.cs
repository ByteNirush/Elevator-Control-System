using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// Core Elevator model class using State Design Pattern.
    /// Manages the current state and delegates behavior to state objects.
    /// Encapsulates elevator properties like current floor and state.
    /// </summary>
    public class Elevator
    {
        // Private fields - encapsulation
        private int _currentFloor;
        private IElevatorState _currentState;

        /// <summary>
        /// Event fired when the elevator's floor changes
        /// Used for GUI updates via delegates
        /// </summary>
        public event Action<int> OnFloorChanged;

        /// <summary>
        /// Event fired when the elevator's state changes
        /// Used for status display updates
        /// </summary>
        public event Action<ElevatorStatus, string> OnStateChanged;

        /// <summary>
        /// Gets the current floor (1 or 2)
        /// </summary>
        public int CurrentFloor 
        { 
            get { return _currentFloor; }
            private set 
            { 
                _currentFloor = value;
                OnFloorChanged?.Invoke(_currentFloor);
            }
        }

        /// <summary>
        /// Gets or sets the current state (State Design Pattern)
        /// </summary>
        public IElevatorState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                if (_currentState != null)
                {
                    OnStateChanged?.Invoke(_currentState.GetStatus(), _currentState.GetStateName());
                }
            }
        }

        /// <summary>
        /// Constructor - initializes elevator at Floor 1 in Idle state
        /// </summary>
        public Elevator()
        {
            _currentFloor = 1;
            _currentState = new IdleState();
        }

        /// <summary>
        /// Request the elevator to move to a specific floor
        /// Delegates to the current state's HandleRequest method
        /// </summary>
        /// <param name="targetFloor">The floor to move to (1 or 2)</param>
        public void RequestFloor(int targetFloor)
        {
            if (targetFloor < 1 || targetFloor > 2)
            {
                throw new ArgumentException("Invalid floor. Must be 1 or 2.");
            }

            // Delegate to current state
            _currentState.HandleRequest(this, targetFloor);
        }

        /// <summary>
        /// Update the elevator's current floor
        /// Called by state objects during movement
        /// </summary>
        /// <param name="floor">New floor number</param>
        public void SetFloor(int floor)
        {
            CurrentFloor = floor;
        }

        /// <summary>
        /// Get the current status of the elevator
        /// </summary>
        /// <returns>Current ElevatorStatus</returns>
        public ElevatorStatus GetStatus()
        {
            return _currentState.GetStatus();
        }

        /// <summary>
        /// Get the current state name for display
        /// </summary>
        /// <returns>State name as string</returns>
        public string GetStateName()
        {
            return _currentState.GetStateName();
        }
    }
}
