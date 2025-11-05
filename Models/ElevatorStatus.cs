using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// Enum representing the various states/statuses of the elevator.
    /// Used for display and logging purposes.
    /// </summary>
    public enum ElevatorStatus
    {
        /// <summary>
        /// Elevator is stationary and waiting for requests
        /// </summary>
        Idle,

        /// <summary>
        /// Elevator is moving between floors
        /// </summary>
        Moving,

        /// <summary>
        /// Elevator doors are opening
        /// </summary>
        DoorsOpening,

        /// <summary>
        /// Elevator doors are open and waiting
        /// </summary>
        DoorsOpen,

        /// <summary>
        /// Elevator doors are closing
        /// </summary>
        DoorsClosing,

        /// <summary>
        /// Elevator has encountered an error
        /// </summary>
        Error
    }
}
