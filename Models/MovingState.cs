using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// Concrete State: Moving State
    /// Represents the elevator while it's traveling between floors
    /// Handles the animation and transition to DoorsOpening state
    /// </summary>
    public class MovingState : IElevatorState
    {
        private int _targetFloor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetFloor">The floor to move to</param>
        public MovingState(int targetFloor)
        {
            _targetFloor = targetFloor;
        }

        /// <summary>
        /// Handle a floor request while moving
        /// Ignore requests while moving (could be extended to queue requests)
        /// </summary>
        public void HandleRequest(Elevator elevator, int targetFloor)
        {
            // Ignore requests while moving
            // In a real system, this would queue the request
            System.Diagnostics.Debug.WriteLine($"Elevator is moving. Request to floor {targetFloor} ignored.");
        }

        /// <summary>
        /// Get the status for this state
        /// </summary>
        public ElevatorStatus GetStatus()
        {
            return ElevatorStatus.Moving;
        }

        /// <summary>
        /// Get the state name for display
        /// </summary>
        public string GetStateName()
        {
            return "Moving";
        }

        /// <summary>
        /// Get the target floor
        /// </summary>
        public int TargetFloor
        {
            get { return _targetFloor; }
        }
    }
}
