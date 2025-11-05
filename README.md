# Elevator Control System - CIS016-2 Assignment 1

## Project Overview

A complete Windows Forms application demonstrating Object-Oriented Programming principles and software engineering best practices through an elevator simulation system.

## Project Architecture

### Folder Structure

```
ElevatorControlSystem/
├── Models/                     # Domain models and state pattern
│   ├── Elevator.cs            # Core elevator model
│   ├── ElevatorStatus.cs      # Status enumeration
│   ├── IElevatorState.cs      # State pattern interface
│   ├── IdleState.cs           # Concrete state
│   ├── MovingState.cs         # Concrete state
│   ├── DoorsOpeningState.cs   # Concrete state
│   ├── DoorsOpenState.cs      # Concrete state
│   └── DoorsClosingState.cs   # Concrete state
├── Controllers/               # Business logic controllers
│   ├── ElevatorController.cs  # Main controller
│   └── DatabaseHelper.cs      # Database operations (Singleton)
├── Utils/                     # Utility classes
│   └── AnimationHelper.cs     # Animation utilities
├── Forms/                     # UI forms
│   ├── LogForm.cs            # Log viewer form
│   └── LogForm.Designer.cs
├── Form1.cs                   # Main form
├── Form1.Designer.cs
└── Program.cs                 # Application entry point
```

## Design Patterns Used

1. **State Pattern**: Elevator states (Idle, Moving, etc.) with dynamic dispatch
2. **Singleton Pattern**: DatabaseHelper ensures single database connection
3. **Observer Pattern**: Event-driven architecture with delegates
4. **MVC Separation**: Model (Elevator), View (Forms), Controller (ElevatorController)

## OOP Principles Demonstrated

- **Encapsulation**: Private fields with public properties
- **Abstraction**: IElevatorState interface
- **Polymorphism**: State pattern implementations
- **Inheritance**: Form inheritance, state implementations
- **Modularity**: Separated concerns across classes

## Database Schema

### ElevatorLog Table

| Column       | Type       | Description            |
| ------------ | ---------- | ---------------------- |
| LogID        | AutoNumber | Primary key            |
| LogTime      | DateTime   | Timestamp of operation |
| Status       | Text(50)   | Elevator status        |
| CurrentFloor | Integer    | Current floor number   |
| TargetFloor  | Integer    | Requested floor number |
| Description  | Text(255)  | Operation description  |

## Installation & Setup

### Prerequisites

- Windows OS (7/8/10/11)
- .NET Framework 4.7.2 or higher
- Microsoft Access Database Engine (for .mdb files)
  - Download: Microsoft Access Database Engine 2016 Redistributable

### Running the Application

1. Open `ElevatorControlSystem.sln` in Visual Studio
2. Build the solution (F6)
3. Run the application (F5)
4. Database file will be automatically created on first run

### Database Setup

- Database file: `bin/Debug/ElevatorLog.mdb`
- Automatically created on first run
- Uses relative path for portability


## Technical Specifications

- **Language**: C# (.NET Framework 4.7.2)
- **IDE**: Visual Studio 2019/2022
- **UI Framework**: Windows Forms
- **Database**: MS Access (.mdb)
- **Data Access**: ADO.NET (OleDb)
- **Concurrency**: BackgroundWorker
- **Animation**: System.Windows.Forms.Timer

## Dependencies

- System.Data (ADO.NET)
- System.Windows.Forms
- System.Drawing
- ADOX (Microsoft ActiveX Data Objects Extensions)

## Contact & Support

For questions or issues related to this assignment submission, please contact through the course management system.

## Academic Integrity Statement

This project was created as original work for CIS016-2 Assignment 1. All code is original unless otherwise noted in comments. The design patterns and architectural decisions were made to demonstrate understanding of OOP principles and software engineering best practices.

---

**End of Documentation**
