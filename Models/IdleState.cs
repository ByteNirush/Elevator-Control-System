using System;

namespace ElevatorControlSystem.Models
{
    /// Concrete State: Idle State
    /// Represents the elevator when it's stationary and ready to accept requests
    public class IdleState : IElevatorState
    {
        /// Handle a floor request when idle
        /// If already at target floor, open doors. Otherwise, transition to Moving state.
        public void HandleRequest(Elevator elevator, int targetFloor)
        {
            if (elevator.CurrentFloor == targetFloor)
            {
                // Already at target floor - just open doors
                elevator.CurrentState = new DoorsOpeningState();
            }
            else
            {
                // Need to move - transition to Moving state
                elevator.CurrentState = new MovingState(targetFloor);
            }
        }

        /// Get the status for this state
        public ElevatorStatus GetStatus()
        {
            return ElevatorStatus.Idle;
        }

        /// Get the state name for display
        public string GetStateName()
        {
            return "Idle";
        }
    }
}
