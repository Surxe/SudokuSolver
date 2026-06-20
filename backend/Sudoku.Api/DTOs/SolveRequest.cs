using System.ComponentModel.DataAnnotations;

namespace Sudoku.Api.DTOs;

/// <summary>
/// Request DTO for solving a Sudoku puzzle.
/// </summary>
public class SolveRequest
{
    /// <summary>
    /// The Sudoku board to solve (9x9 2D array, 0-9 where 0 represents empty).
    /// </summary>
    [Required]
    public int[][] Board { get; set; } = Array.Empty<int[]>();
}
