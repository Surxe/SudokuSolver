# Local Testing Guide

This guide explains how to run the Sudoku Solver application locally for testing.

## Prerequisites

- **.NET 8.0 SDK** - For running the backend API
- **Node.js 18+** - For running the Angular frontend
- **npm** - Package manager for Node.js
- **Docker & Docker Compose** (optional) - For containerized deployment

## Running Services Separately (Recommended for Development)

### Backend

The backend is an ASP.NET Core Web API that serves Sudoku puzzles and solves them.

```bash
cd backend/Sudoku.Api
dotnet run
```

The API will start on `http://localhost:5129` (or another available port).

**Verify it's running:**
```bash
curl http://localhost:5129/api/sudoku/puzzle
```

You should receive a JSON response with a puzzle.

### Frontend

The frontend is an Angular 18 application that displays puzzles and communicates with the backend.

```bash
cd frontend
npm start
```

The Angular dev server will start on `http://localhost:4200`.

**Access the application:**
Open http://localhost:4200 in your browser.

### Configuration

The frontend API URL is configured in `frontend/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5129/api/sudoku'
};
```

If your backend runs on a different port, update this file accordingly.

The backend puzzle data path is configured in `backend/Sudoku.Api/appsettings.json`:

```json
{
  "PuzzleDataPath": "D:\\Repositories\\SudokuSolver\\data\\puzzles.json"
}
```

Update this path if your repository is located elsewhere.

## Running with Docker Compose

Docker Compose orchestrates both services in containers for easier testing.

```bash
docker-compose up --build
```

This will:
1. Build the backend Docker image
2. Build the frontend Docker image (with Nginx)
3. Start both services in a network
4. Expose frontend on port 80
5. Expose backend on port 5000

**Access the application:**
Open http://localhost in your browser.

**Stop the services:**
```bash
docker-compose down
```

## Testing the Application

### Manual Testing Workflow

1. **Load the application** - Open http://localhost:4200 (or http://localhost for Docker)
2. **View random puzzle** - A puzzle is automatically loaded on startup
3. **Solve the puzzle** - Click the "Solve" button
4. **View solution** - The solved puzzle displays with statistics (attempts, duration)
5. **Load new puzzle** - Click "Load New Puzzle" to start over

### Expected Behavior

- **Initial load**: App fetches a random puzzle from the backend
- **Puzzle display**: 9x9 grid with given cells highlighted in blue
- **Solve action**: Frontend sends board to backend, backend solves with backtracking
- **Solution display**: Grid updates with solved values, statistics shown below
- **Error handling**: If API fails, error message displays in red

### API Endpoints

**Get Random Puzzle:**
```
GET http://localhost:5129/api/sudoku/puzzle
```

Response:
```json
{
  "id": 1923,
  "board": [[8,0,0,0,3,9,0,0,0], ...]
}
```

**Solve Puzzle:**
```
POST http://localhost:5129/api/sudoku/solve
Content-Type: application/json

{
  "board": [[8,0,0,0,3,9,0,0,0], ...]
}
```

Response:
```json
{
  "solved": true,
  "solution": [[7,3,5,9,8,1,2,4,6], ...],
  "attempts": 51,
  "durationMs": 2
}
```

## Troubleshooting

### Backend Issues

**"No puzzles available in the dataset"**
- Check that `data/puzzles.json` exists
- Verify the `PuzzleDataPath` in `appsettings.json` points to the correct location
- Use an absolute path if relative paths don't resolve correctly

**Port already in use**
- The backend will automatically use an available port if 5129 is taken
- Check the console output for the actual port being used
- Update the frontend `environment.ts` to match the actual port

### Frontend Issues

**"Failed to load puzzle" error**
- Verify the backend is running and accessible
- Check the API URL in `frontend/src/environments/environment.ts`
- Open browser DevTools (F12) to check console for detailed errors
- Verify CORS is configured in the backend (already configured in `Program.cs`)

**npm run dev fails**
- Use `npm start` instead (the Angular CLI doesn't have a "dev" script by default)
- Ensure you're in the `frontend` directory (not `frontend/src`)

### Docker Issues

**Container build fails**
- Ensure Docker is running
- Check that all files are committed/saved
- Verify Dockerfile paths are correct

**Containers can't communicate**
- Check that both services are in the same Docker network
- Verify the frontend is using the correct backend URL (use service name: `http://backend:8080`)

## Development Tips

### Hot Reload

- **Backend**: `dotnet run` supports hot reload for C# code changes
- **Frontend**: `npm start` supports hot reload for Angular component changes

### Debugging

- **Backend**: Use Visual Studio's debugger or `dotnet run` with breakpoints
- **Frontend**: Use browser DevTools (F12) for Angular debugging
- **Network**: Check the Network tab in DevTools to see API requests/responses

### Logs

- **Backend logs**: Console output from `dotnet run` shows API requests and errors
- **Frontend logs**: Browser console (F12) shows Angular errors and API failures

## Project Structure

```
SudokuSolver/
├── backend/
│   └── Sudoku.Api/          # ASP.NET Core Web API
│       ├── Controllers/     # API endpoints
│       ├── DTOs/           # Data transfer objects
│       ├── Services/       # Business logic
│       └── Program.cs      # Application entry point
├── frontend/
│   ├── src/
│   │   ├── app/            # Angular components
│   │   │   ├── components/ # UI components
│   │   │   ├── models/     # TypeScript interfaces
│   │   │   └── services/   # HTTP services
│   │   └── environments/   # Environment configuration
│   ├── Dockerfile          # Frontend container image
│   └── nginx.conf          # Nginx configuration
├── core/
│   └── Sudoku.Core/        # Core solving logic (C#)
├── data/
│   └── puzzles.json        # Puzzle dataset
├── docker-compose.yml      # Docker orchestration
└── docs/
    └── local_testing_guide.md
```
