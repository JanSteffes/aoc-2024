# Advent of Code 2024

![ScreenShot](https://github.com/user-attachments/assets/59fd2185-9d84-4bd6-9cd2-bd9dce7f71a4)

A .NET 9 Console Application designed to streamline your Advent of Code 2024 experience. This toolkit helps you fetch daily inputs automatically and provides a framework to solve puzzles with ease.

## Features

- Automatically fetch daily inputs from the Advent of Code website.
- Organize solutions for each day in a structured format.
- Simple configuration to set up your session cookie.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) installed on your machine.
- A valid Advent of Code account with an active session cookie.

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/GessioMori/aoc-2024.git
cd aoc-2024
```

### 2. Configure Your Session Cookie

To fetch your daily inputs, the app requires your Advent of Code session cookie.

1. Create a file named `session-cookie.txt` in the directory "ProgramUtils" of the project.
2. Paste your session cookie into this file. You can find your session cookie by inspecting your browser's cookies while logged into the [Advent of Code website](https://adventofcode.com).

### 3. Build and Run the App

Use the .NET CLI to build and run the application.

```bash
dotnet build
dotnet run
```

The application will automatically fetch the input for the selected day if it is available and save it in the appropriate folder.

---

## Usage

1. **Daily Input Fetching**:  
   When you run the application, it will fetch the input for the selected day's puzzle and save it to `Inputs/input-XX.txt` (where `XX` is the day number).

2. **Solving Puzzles**:  
   Each day's solution can be implemented in the `Solutions` folder, under `SolutionXX.cs`. Use the fetched input file for processing.

---

## Project Structure

```plaintext
│   aoc-2024.csproj          # The project file defining dependencies and build settings.
│   aoc-2024.sln             # The solution file for the project.
│   Consts.cs                # Contains constants used throughout the application.
│   Program.cs               # The main entry point of the application.
│
├───AocClient                # Handles interactions with the Advent of Code website.
│       AocHttpClient.cs     # Implements HTTP client for fetching inputs from the API.
│       ClientResponse.cs    # Defines structures for API responses.
│
├───Classes                  # Core classes for application logic and utilities.
│       ConsoleController.cs # Manages console inputs/outputs.
│       ConsoleLogger.cs     # Handles logging to the console.
│       ConsoleRunner.cs     # Executes solutions via the console.
│       ExecutionResult.cs   # Represents the results of solution execution.
│       FileUtils.cs         # Utility methods for file management.
│       LastExecutionManager.cs  # Tracks details about the last execution.
│       SolutionManager.cs   # Handles loading and execution of solutions.
│       TestCase.cs          # Represents test cases for solutions.
│       TestManager.cs       # Manages test case execution.
│
├───Inputs                   # Contains input files for each day's puzzle.
│
├───Interfaces               # Interfaces defining key abstractions for the project.
│       IAocClient.cs        # Interface for the HTTP client.
│       IController.cs       # Interface for console controllers.
│       ILastExecutionManager.cs # Interface for managing execution details.
│       ILogger.cs           # Interface for logging implementations.
│       IRunner.cs           # Interface for executing solutions.
│       ISolution.cs         # Interface for puzzle solutions.
│       ISolutionManager.cs  # Interface for managing solutions.
│       ITestManager.cs      # Interface for managing test cases.
│
├───ProgramUtils             # Configuration and utility files.
│       last-choice.txt      # Tracks the last selected solution for quick access.
│       session-cookie.txt   # Stores the Advent of Code session cookie securely.
│
├───Solutions                # Folder for solutions to daily puzzles.
│       (e.g., Solution01.cs for Day 1, Solution02.cs for Day 2, etc.)
│
├───Templates                # Templates for creating new solutions and test cases.
│       solution-template.txt # Template for new solution files.
│       test-template.txt    # Template for new test case files.
│
└───Tests                    # Folder for test-related files.
```

---

## Notes

- **Session Cookie Security**:  
  Do **not** share your `session-cookie.txt` file. It grants access to your Advent of Code account and its data.

- **Input Fetching Errors**:  
  Ensure your session cookie is valid and that the puzzle for the current day is unlocked. Inputs will only be fetched if the puzzle is available.

---

## Contributions

Feel free to fork the repository and submit pull requests with improvements or new features. Contributions are welcome!

---

## License

This project is licensed under the **The Unlicense**. For more details, refer to the [LICENSE](./LICENSE.txt) file in the repository.

---

Enjoy solving the Advent of Code 2024 puzzles! 🎄

---
