using Sudoku.Core.Models;

namespace Sudoku.Core.Solvers;

/// <summary>
/// Defines the contract for Sudoku solvers.
/// </summary>
public interface ISudokuSolver
{
    /// <summary>
    /// Attempts to solve a Sudoku puzzle.
    /// </summary>
    /// <param name="board">The board to solve.</param>
    /// <returns>A SolveResult containing the solution and solving statistics.</returns>
    SolveResult Solve(SudokuBoard board);

    /// <summary>
    /// Attempts to solve a Sudoku puzzle with a cancellation token.
    /// </summary>
    /// <param name="board">The board to solve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A SolveResult containing the solution and solving statistics.</returns>
    SolveResult Solve(SudokuBoard board, System.Threading.CancellationToken cancellationToken);
}

/// <summary>
/// Represents the result of a solve operation.
/// </summary>
public class SolveResult
{
    /// <summary>
    /// Gets whether the puzzle was successfully solved.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets the solved board, if successful.
    /// </summary>
    public SudokuBoard? Solution { get; set; }

    /// <summary>
    /// Gets the number of solve attempts made.
    /// </summary>
    public long Attempts { get; set; }

    /// <summary>
    /// Gets the number of backtracks performed.
    /// </summary>
    public long Backtracks { get; set; }

    /// <summary>
    /// Gets the time taken to solve the puzzle.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets an error message if solving failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
