# Advent of Code 2024 Toolkit

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
.
├── aoc-2024/
│   ├── AocClient/
│   │   ├── AocHttpClient.cs (HTTP client for Advent of Code API)
│   │   ├── ClientResponse.cs (Handles API responses)
│   │   └── IAocClient.cs (Interface for the HTTP client)
│   ├── Controller/
│   │   ├── ConsoleController.cs (Handles console inputs/outputs)
│   │   ├── IController.cs (Interface for controllers)
│   │   ├── ILogger.cs (Logging interface)
│   │   ├── LastExecutionManager.cs (Tracks last execution details)
│   │   └── SolutionManager.cs (Manages solution execution)
│   ├── Inputs/
│   │   └── ISolution.cs (Solution interface definition)
│   ├── MessageWriter/
│   │   ├── ConsoleMessageWriter.cs (Console message implementation)
│   │   └── IMessageWriter.cs (Message writer interface)
│   ├── ProgramUtils/
│   │   ├── last-choice.txt (Tracks last selected solution)
│   │   └── session-cookie.txt (Stores Advent of Code session cookie)
│   ├── Runner/
│   │   ├── ConsoleRunner.cs (Handles running solutions via console)
│   │   └── IRunner.cs (Runner interface)
│   ├── Solutions/
│   │   ├── Solution01.cs (Solution for Day 1)
│   │   └── Solution02.cs (Solution for Day 2)
│   ├── Templates/
│   │   ├── solution-template.txt (Template for new solutions)
│   │   └── test-template.txt (Template for test cases)
│   ├── Consts.cs (Constants used across the project)
│   └── Program.cs (Main entry point)

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

Enjoy solving the Advent of Code puzzles! 🎄
