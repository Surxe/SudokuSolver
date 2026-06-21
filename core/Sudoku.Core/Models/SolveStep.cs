namespace Sudoku.Core.Models;

/// <summary>
/// Represents a single step in the solving process (attempt or backtrack).
/// </summary>
public class SolveStep
{
    /// <summary>
    /// Gets the type of step (attempt or backtrack).
    /// </summary>
    public SolveStepType Type { get; set; }

    /// <summary>
    /// Gets the row index (0-8).
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Gets the column index (0-8).
    /// </summary>
    public int Col { get; set; }

    /// <summary>
    /// Gets the value placed (1-9) for attempts, 0 for backtracks.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets the attempt number at this step.
    /// </summary>
    public long AttemptNumber { get; set; }

    /// <summary>
    /// Gets the backtrack number at this step.
    /// </summary>
    public long BacktrackNumber { get; set; }

    /// <summary>
    /// Gets the board state at this step.
    /// </summary>
    public int[][] Board { get; set; } = [];
}

/// <summary>
/// Represents the type of solve step.
/// </summary>
public enum SolveStepType
{
    /// <summary>
    /// A value placement attempt.
    /// </summary>
    Attempt,

    /// <summary>
    /// A backtrack (value removal).
    /// </summary>
    Backtrack
}
