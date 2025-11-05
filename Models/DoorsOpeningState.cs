using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// Concrete State: Doors Opening State
    /// Represents the elevator when doors are opening
    /// Transitions to DoorsOpen state after animation
    /// </summary>
    public class DoorsOpeningState : IElevatorState
    {
        /// <summary>
        /// Handle a floor request while doors are opening
        /// Ignore requests during door operation
        /// </summary>
        public void HandleRequest(Elevator elevator, int targetFloor)
        {
            // Ignore requests while doors are opening
            System.Diagnostics.Debug.WriteLine($"Doors are opening. Request to floor {targetFloor} ignored.");
        }

        /// <summary>
        /// Get the status for this state
        /// </summary>
        public ElevatorStatus GetStatus()
        {
            return ElevatorStatus.DoorsOpening;
        }

        /// <summary>
        /// Get the state name for display
        /// </summary>
        public string GetStateName()
        {
            return "Doors Opening";
        }
    }
}
