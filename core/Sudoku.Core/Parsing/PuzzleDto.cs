namespace Sudoku.Core.Parsing;

/// <summary>
/// Data Transfer Object for puzzle JSON serialization/deserialization.
/// </summary>
public class PuzzleDto
{
    /// <summary>
    /// Gets or sets the puzzle identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the board as a 2D array.
    /// </summary>
    public int[][] Board { get; set; } = Array.Empty<int[]>();

    /// <summary>
    /// Gets or sets the solved board as a 2D array (optional).
    /// </summary>
    public int[][]? SolvedBoard { get; set; }
}
