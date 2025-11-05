using System;

namespace ElevatorControlSystem.Models
{
    /// <summary>
    /// State Design Pattern Interface
    /// Each concrete state will implement this interface to define behavior
    /// for different elevator states (Idle, Moving, DoorsOpening, etc.)
    /// This eliminates the need for switch/if statements in the main logic.
    /// </summary>
    public interface IElevatorState
    {
        /// <summary>
        /// Handle a floor request in the current state
        /// </summary>
        /// <param name="elevator">The elevator instance</param>
        /// <param name="targetFloor">The requested floor</param>
        void HandleRequest(Elevator elevator, int targetFloor);

        /// <summary>
        /// Get the current status of the elevator in this state
        /// </summary>
        /// <returns>ElevatorStatus enum value</returns>
        ElevatorStatus GetStatus();

        /// <summary>
        /// Get a string representation of the current state
        /// </summary>
        /// <returns>State name as string</returns>
        string GetStateName();
    }
}
