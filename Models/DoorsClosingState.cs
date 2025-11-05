using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// Concrete State: Doors Closing State
    /// Represents the elevator when doors are closing
    /// Transitions to Idle state after animation
    /// </summary>
    public class DoorsClosingState : IElevatorState
    {
        /// <summary>
        /// Handle a floor request while doors are closing
        /// Ignore requests during door operation
        /// </summary>
        public void HandleRequest(Elevator elevator, int targetFloor)
        {
            // Ignore requests while doors are closing
            System.Diagnostics.Debug.WriteLine($"Doors are closing. Request to floor {targetFloor} ignored.");
        }

        /// <summary>
        /// Get the status for this state
        /// </summary>
        public ElevatorStatus GetStatus()
        {
            return ElevatorStatus.DoorsClosing;
        }

        /// <summary>
        /// Get the state name for display
        /// </summary>
        public string GetStateName()
        {
            return "Doors Closing";
        }
    }
}
