namespace Sudoku.Api.DTOs;

/// <summary>
/// Response DTO for a Sudoku puzzle.
/// </summary>
public class PuzzleResponse
{
    /// <summary>
    /// The unique identifier for the puzzle.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The puzzle board (9x9 2D array, 0-9 where 0 represents empty).
    /// </summary>
    public int[][] Board { get; set; } = Array.Empty<int[]>();
}
