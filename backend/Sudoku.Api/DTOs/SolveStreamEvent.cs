namespace Sudoku.Api.DTOs;

/// <summary>
/// Represents a streaming event for Sudoku solving progress.
/// </summary>
public class SolveStreamEvent
{
    /// <summary>
    /// Gets or sets the type of event (attempt, backtrack, complete, error).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the row index for attempt/backtrack events.
    /// </summary>
    public int? Row { get; set; }

    /// <summary>
    /// Gets or sets the column index for attempt/backtrack events.
    /// </summary>
    public int? Col { get; set; }

    /// <summary>
    /// Gets or sets the value placed for attempt events.
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// Gets or sets the current attempt number.
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Gets or sets the current backtrack number.
    /// </summary>
    public int BacktrackNumber { get; set; }

    /// <summary>
    /// Gets or sets the current board state as a jagged array.
    /// </summary>
    public int[][]? Board { get; set; }

    /// <summary>
    /// Gets or sets the error message for error events.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the duration in milliseconds for complete events.
    /// </summary>
    public long? DurationMs { get; set; }
}
