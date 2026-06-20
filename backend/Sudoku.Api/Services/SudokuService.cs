using Sudoku.Api.DTOs;
using Sudoku.Core.Models;
using Sudoku.Core.Solvers;
using Sudoku.Core.Validation;

namespace Sudoku.Api.Services;

/// <summary>
/// Service for Sudoku solving operations.
/// </summary>
public class SudokuService : ISudokuService
{
    private readonly ISudokuSolver _solver;

    public SudokuService()
    {
        _solver = new BacktrackingSolver();
    }

    /// <summary>
    /// Solves a Sudoku puzzle asynchronously.
    /// </summary>
    public async Task<SolveResponse> SolveAsync(SolveRequest request, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                // Validate request structure
                if (request.Board == null || request.Board.Length != 9)
                {
                    return new SolveResponse
                    {
                        Solved = false,
                        ErrorMessage = "Board must be 9x9"
                    };
                }

                // Convert DTO board to Core library board
                var boardArray = ConvertJaggedTo2D(request.Board);
                var board = new SudokuBoard(boardArray);

                // Validate board structure
                var validationResult = BoardValidator.ValidateBoard(board);
                if (!validationResult.IsValid)
                {
                    return new SolveResponse
                    {
                        Solved = false,
                        ErrorMessage = $"Invalid board: {string.Join(", ", validationResult.ErrorMessages)}"
                    };
                }

                // Solve the puzzle
                var result = _solver.Solve(board, cancellationToken);

                if (result.Success && result.Solution != null)
                {
                    return new SolveResponse
                    {
                        Solved = true,
                        Solution = Convert2DToJagged(result.Solution.ToArray()),
                        Attempts = result.Attempts,
                        DurationMs = (long)result.Duration.TotalMilliseconds
                    };
                }
                else
                {
                    return new SolveResponse
                    {
                        Solved = false,
                        ErrorMessage = result.ErrorMessage ?? "Failed to solve puzzle",
                        Attempts = result.Attempts,
                        DurationMs = (long)result.Duration.TotalMilliseconds
                    };
                }
            }
            catch (Exception ex)
            {
                return new SolveResponse
                {
                    Solved = false,
                    ErrorMessage = $"Error solving puzzle: {ex.Message}"
                };
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Converts a jagged array to a 2D array.
    /// </summary>
    private int[,] ConvertJaggedTo2D(int[][] jaggedArray)
    {
        var rows = jaggedArray.Length;
        var cols = jaggedArray[0].Length;
        var result = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = jaggedArray[i][j];
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a 2D array to a jagged array.
    /// </summary>
    private int[][] Convert2DToJagged(int[,] array2D)
    {
        var rows = array2D.GetLength(0);
        var cols = array2D.GetLength(1);
        var result = new int[rows][];

        for (int i = 0; i < rows; i++)
        {
            result[i] = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                result[i][j] = array2D[i, j];
            }
        }

        return result;
    }
}
