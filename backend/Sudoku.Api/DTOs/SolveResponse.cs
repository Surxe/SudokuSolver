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
    /// Time taken to solve in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }
}
