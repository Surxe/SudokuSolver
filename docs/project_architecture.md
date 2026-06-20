\# Sudoku Solver Project Architecture



\## Project Goals



This project is primarily a learning exercise for:



\* C#

\* ASP.NET Core Web APIs

\* Dependency Injection

\* LINQ

\* Angular

\* Full-stack application architecture



The goal is NOT to build a feature-complete Sudoku game. The project intentionally avoids unnecessary complexity to focus on learning the technologies above.



\---



\# Current Scope (MVP)



The application will:



1\. Load Sudoku puzzles from an imported dataset.

2\. Display a puzzle in an Angular frontend.

3\. Send the puzzle to an ASP.NET Core Web API.

4\. Solve the puzzle using C# logic.

5\. Return the solved board.

6\. Display the solution.



The application will NOT initially support:



\* User accounts

\* Databases

\* Puzzle generation

\* Difficulty calculation

\* Manual Sudoku gameplay

\* Step-by-step solving explanations

\* Puzzle editing



\---



\# High-Level Architecture



```text

Angular Frontend

&#x20;       |

&#x20;       | HTTP / JSON

&#x20;       v

ASP.NET Core Web API

&#x20;       |

&#x20;       v

Sudoku Core Library

&#x20;       |

&#x20;       v

Sudoku Solver Logic

```



The frontend and backend are intentionally separated.



The backend contains no UI logic.



The frontend contains no Sudoku-solving logic.



\---



\# Repository Structure



The project uses a monorepo.



```text

sudoku-solver/

│

├─ backend/

├─ frontend/

├─ core/

│

├─ data/

├─ docs/

├─ scripts/

│

└─ README.md

```



\---



\# Backend Architecture



Location:



```text

backend/

```



Purpose:



\* Receive HTTP requests

\* Return JSON responses

\* Coordinate services

\* Call the Sudoku core library



The backend should contain NO solving logic.



Proposed structure:



```text

backend/

│

├─ Controllers/

│   └─ SudokuController.cs

│

├─ Services/

│   ├─ SudokuService.cs

│   └─ PuzzleRepositoryService.cs

│

├─ DTOs/

│   ├─ SolveRequest.cs

│   ├─ SolveResponse.cs

│   └─ PuzzleResponse.cs

│

├─ Program.cs

└─ appsettings.json

```



\---



\# Core Architecture



Location:



```text

core/

```



Purpose:



\* Sudoku data structures

\* Solving algorithms

\* Validation logic

\* Puzzle parsing



The core library should have NO knowledge of:



\* HTTP

\* Controllers

\* Angular

\* Swagger



Proposed structure:



```text

core/

│

├─ Models/

│   ├─ SudokuBoard.cs

│   ├─ Cell.cs

│   └─ Puzzle.cs

│

├─ Solvers/

│   ├─ ISudokuSolver.cs

│   └─ BacktrackingSolver.cs

│

├─ Validation/

│   └─ BoardValidator.cs

│

├─ Parsing/

│   ├─ CsvPuzzleParser.cs

│   └─ JsonPuzzleParser.cs

│

└─ Utilities/

```



\---



\# Frontend Architecture



Location:



```text

frontend/

```



Purpose:



\* Display puzzle

\* Display solution

\* Call API endpoints



The frontend should contain NO Sudoku-solving logic.



Proposed structure:



```text

frontend/

│

├─ src/

│   ├─ app/

│   │

│   ├─ components/

│   │   ├─ sudoku-board/

│   │   └─ controls/

│   │

│   ├─ pages/

│   │   └─ solver-page/

│   │

│   ├─ services/

│   │   └─ sudoku-api.service.ts

│   │

│   └─ models/

│

└─ angular.json

```



\---



\# Data Source



The project will initially use imported Sudoku datasets.



Possible formats:



\* CSV

\* JSON



Examples:



```json

{

&#x20; "id": 1,

&#x20; "board": \[

&#x20;   \[5,3,0,0,7,0,0,0,0],

&#x20;   ...

&#x20; ]

}

```



Location:



```text

data/

```



Examples:



```text

data/

│

├─ puzzles.csv

├─ puzzles.json

└─ samples/

```



No database is planned for the MVP.



\---



\# API Design



Initial API surface should remain minimal.



\## Get Puzzle



```http

GET /api/sudoku/puzzle

```



Returns a puzzle from the imported dataset.



Example:



```json

{

&#x20; "id": 42,

&#x20; "board": \[...]

}

```



\---



\## Solve Puzzle



```http

POST /api/sudoku/solve

```



Request:



```json

{

&#x20; "board": \[...]

}

```



Response:



```json

{

&#x20; "solved": true,

&#x20; "solution": \[...]

}

```



\---



\# Validation Strategy



There will NOT be a public:



```http

POST /api/sudoku/validate

```



endpoint.



Validation still exists internally.



Examples:



\* Verify imported puzzles are valid.

\* Verify solver output is valid.

\* Prevent invalid board states from propagating.



Validation belongs inside the Core library.



\---



\# Deployment Strategy



Frontend:



\* Angular

\* Intended deployment target: GitHub Pages



Backend:



\* ASP.NET Core Web API

\* Separate hosting platform



Examples:



\* Azure

\* Render

\* Railway

\* Fly.io

\* VPS



Architecture:



```text

GitHub Pages

&#x20;    |

&#x20;    v

ASP.NET API

&#x20;    |

&#x20;    v

Sudoku Solver

```



CORS will be required because frontend and backend are hosted separately.



\---



\# Technology Learning Goals



The project exists primarily to learn:



\## Angular



\* Components

\* Services

\* HTTP client

\* Routing (optional)



\## ASP.NET Core



\* Controllers

\* Dependency Injection

\* Services

\* Swagger/OpenAPI

\* Routing



\## C#



\* Class design

\* Interfaces

\* Dependency injection patterns

\* Collections



\## LINQ



Expected to be heavily used for:



\* Row analysis

\* Column analysis

\* Candidate calculation

\* Validation logic



\---



\# Important Architectural Rules



\## Controller Responsibilities



Controllers:



\* Accept HTTP requests

\* Return HTTP responses

\* Call services



Controllers should NOT:



\* Solve Sudoku

\* Contain business logic

\* Perform heavy LINQ operations



\---



\## Service Responsibilities



Services:



\* Coordinate application workflows

\* Call solver components

\* Load puzzles



Services should NOT:



\* Know about Angular

\* Contain UI concerns



\---



\## Core Responsibilities



Core contains:



\* Models

\* Solvers

\* Validation

\* Parsing



Core must remain independent of:



\* ASP.NET

\* Angular

\* Swagger



\---



\# Current Development Status



Current solution structure:



```text

SudokuSolver.sln

│

├─ Sudoku.Api

└─ Sudoku.Core

```



Visual Studio has been set up.



The default WeatherForecast template code should be removed from the API project.



Swagger should be enabled.



The next implementation step is to design the Sudoku board model and solver architecture.

