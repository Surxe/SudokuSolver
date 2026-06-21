namespace Sudoku.Api.DTOs;

/// <summary>
/// Response DTO for a Sudoku solve operation.
/// </summary>
public class SolveResponse
{
    /// <summary>
    /// Whether the puzzle was successfully solved.
    /// </summary>
    public bool Solved { get; set; }

    /// <summary>
    /// The solved board (9x9 2D array), if successful.
    /// </summary>
    public int[][]? Solution { get; set; }

    /// <summary>
    /// Error message if solving failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of solve attempts made.
    /// </summary>
    public long Attempts { get; set; }

    /// <summary>
    /// Number of backtracks made during solving.
    /// </summary>
    public long Backtracks { get; set; }

    /// <summary>
    /// Time taken to solve in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Sequence of steps taken during solving (attempts and backtracks).
    /// </summary>
    public List<SolveStepDto> Steps { get; set; } = [];
}

/// <summary>
/// DTO for a single solve step.
/// </summary>
public class SolveStepDto
{
    /// <summary>
    /// Type of step (attempt or backtrack).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Row index (0-8).
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Column index (0-8).
    /// </summary>
    public int Col { get; set; }

    /// <summary>
    /// Value placed (1-9) for attempts, 0 for backtracks.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Attempt number at this step.
    /// </summary>
    public long AttemptNumber { get; set; }

    /// <summary>
    /// Backtrack number at this step.
    /// </summary>
    public long BacktrackNumber { get; set; }

    /// <summary>
    /// Board state at this step.
    /// </summary>
    public int[][] Board { get; set; } = [];
}
