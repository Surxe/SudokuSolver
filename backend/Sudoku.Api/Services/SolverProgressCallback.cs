using Sudoku.Core.Solvers;
using System.Text.Json;

namespace Sudoku.Api.Services;

/// <summary>
/// Implementation of ISolverProgressCallback that writes events to an SSE response.
/// </summary>
public class SolverProgressCallback : ISolverProgressCallback
{
    private readonly HttpResponse _response;
    private readonly object _lock = new();

    public SolverProgressCallback(HttpResponse response)
    {
        _response = response;
    }

    /// <summary>
    /// Called when a cell placement attempt is made.
    /// </summary>
    public void OnAttempt(int row, int col, int value, long attemptNumber, long backtrackNumber, int[][] board)
    {
        SendEvent(new DTOs.SolveStreamEvent
        {
            EventType = "attempt",
            Row = row,
            Col = col,
            Value = value,
            AttemptNumber = (int)attemptNumber,
            BacktrackNumber = (int)backtrackNumber,
            Board = board
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Called when a backtrack occurs (cell is cleared).
    /// </summary>
    public void OnBacktrack(int row, int col, long attemptNumber, long backtrackNumber, int[][] board)
    {
        SendEvent(new DTOs.SolveStreamEvent
        {
            EventType = "backtrack",
            Row = row,
            Col = col,
            AttemptNumber = (int)attemptNumber,
            BacktrackNumber = (int)backtrackNumber,
            Board = board
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Called when solving completes (success or failure).
    /// </summary>
    public void OnComplete(int[][] board, long totalAttempts, long totalBacktracks, TimeSpan duration)
    {
        SendEvent(new DTOs.SolveStreamEvent
        {
            EventType = "complete",
            Board = board,
            AttemptNumber = (int)totalAttempts,
            BacktrackNumber = (int)totalBacktracks,
            DurationMs = (long)duration.TotalMilliseconds
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Sends an event to the SSE response.
    /// </summary>
    private async Task SendEvent(DTOs.SolveStreamEvent evt)
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(evt);
            _response.WriteAsync($"data: {json}\n\n").Wait();
        }
    }
}
