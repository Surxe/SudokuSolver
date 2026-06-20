using Sudoku.Core.Models;
using Sudoku.Core.Validation;
using System.Threading;

namespace Sudoku.Core.Solvers;

/// <summary>
/// Implements a backtracking algorithm with MRV heuristic for solving Sudoku puzzles.
/// </summary>
public class BacktrackingSolver : ISudokuSolver
{
    private long _attempts;
    private long _backtracks;

    /// <summary>
    /// Attempts to solve a Sudoku puzzle.
    /// </summary>
    /// <param name="board">The board to solve.</param>
    /// <returns>A SolveResult containing the solution and solving statistics.</returns>
    public SolveResult Solve(SudokuBoard board)
    {
        return Solve(board, CancellationToken.None);
    }

    /// <summary>
    /// Attempts to solve a Sudoku puzzle with a cancellation token.
    /// </summary>
    /// <param name="board">The board to solve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A SolveResult containing the solution and solving statistics.</returns>
    public SolveResult Solve(SudokuBoard board, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _attempts = 0;
        _backtracks = 0;

        try
        {
            // Validate the initial board structure
            var validationResult = BoardValidator.ValidateBoard(board);
            if (!validationResult.IsValid)
            {
                return new SolveResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid board: {string.Join(", ", validationResult.ErrorMessages)}",
                    Attempts = _attempts,
                    Backtracks = _backtracks,
                    Duration = stopwatch.Elapsed
                };
            }

            // Create a copy to work with
            var workingBoard = board.Clone();

            // Attempt to solve
            bool solved = SolveRecursive(workingBoard, cancellationToken);

            stopwatch.Stop();

            if (solved)
            {
                return new SolveResult
                {
                    Success = true,
                    Solution = workingBoard,
                    Attempts = _attempts,
                    Backtracks = _backtracks,
                    Duration = stopwatch.Elapsed
                };
            }
            else
            {
                return new SolveResult
                {
                    Success = false,
                    ErrorMessage = cancellationToken.IsCancellationRequested 
                        ? "Solving was cancelled" 
                        : "No solution exists for this puzzle",
                    Attempts = _attempts,
                    Backtracks = _backtracks,
                    Duration = stopwatch.Elapsed
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new SolveResult
            {
                Success = false,
                ErrorMessage = $"Error during solving: {ex.Message}",
                Attempts = _attempts,
                Backtracks = _backtracks,
                Duration = stopwatch.Elapsed
            };
        }
    }

    /// <summary>
    /// Recursively solves the puzzle using backtracking with MRV heuristic.
    /// </summary>
    /// <param name="board">The board to solve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if solved, false otherwise.</returns>
    private bool SolveRecursive(SudokuBoard board, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Find the empty cell with the minimum remaining values (MRV heuristic)
        var (row, col) = FindBestEmptyCell(board);
        
        if (row == -1)
        {
            // No empty cells found - puzzle is solved
            return true;
        }

        // Get valid values for this cell
        var validValues = BoardValidator.GetValidValues(board, row, col);

        // Try each valid value
        foreach (var value in validValues)
        {
            _attempts++;
            board.SetValue(row, col, value);

            if (SolveRecursive(board, cancellationToken))
            {
                return true;
            }

            // Backtrack
            _backtracks++;
            board.SetValue(row, col, 0);
        }

        return false;
    }

    /// <summary>
    /// Finds the empty cell with the minimum remaining values (MRV heuristic).
    /// </summary>
    /// <param name="board">The board to search.</param>
    /// <returns>A tuple of (row, column) for the best cell, or (-1, -1) if no empty cells.</returns>
    private (int row, int col) FindBestEmptyCell(SudokuBoard board)
    {
        int bestRow = -1;
        int bestCol = -1;
        int minValidValues = 10; // More than maximum possible (9)

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetValue(row, col) == 0)
                {
                    var validValues = BoardValidator.GetValidValues(board, row, col);
                    int count = validValues.Count;

                    if (count < minValidValues)
                    {
                        minValidValues = count;
                        bestRow = row;
                        bestCol = col;

                        // If we found a cell with only 1 valid value, it's optimal
                        if (count == 1)
                        {
                            return (bestRow, bestCol);
                        }
                    }
                }
            }
        }

        return (bestRow, bestCol);
    }
}
