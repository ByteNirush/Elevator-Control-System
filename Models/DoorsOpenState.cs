using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// Concrete State: Doors Open State
    /// Represents the elevator when doors are fully open
    /// Transitions to DoorsClosing state after a delay
    /// </summary>
    public class DoorsOpenState : IElevatorState
    {
        /// <summary>
        /// Handle a floor request while doors are open
        /// Ignore requests while doors are open
        /// </summary>
        public void HandleRequest(Elevator elevator, int targetFloor)
        {
            // Ignore requests while doors are open
            System.Diagnostics.Debug.WriteLine($"Doors are open. Request to floor {targetFloor} ignored.");
        }

        /// <summary>
        /// Get the status for this state
        /// </summary>
        public ElevatorStatus GetStatus()
        {
            return ElevatorStatus.DoorsOpen;
        }

        /// <summary>
        /// Get the state name for display
        /// </summary>
        public string GetStateName()
        {
            return "Doors Open";
        }
    }
}
