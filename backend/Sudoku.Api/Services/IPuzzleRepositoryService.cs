using Sudoku.Api.DTOs;

namespace Sudoku.Api.Services;

/// <summary>
/// Interface for puzzle data access operations.
/// </summary>
public interface IPuzzleRepositoryService
{
    /// <summary>
    /// Gets a random puzzle from the dataset.
    /// </summary>
    /// <returns>A puzzle response, or null if no puzzles are available.</returns>
    Task<PuzzleResponse?> GetRandomPuzzleAsync();

    /// <summary>
    /// Gets a specific puzzle by ID.
    /// </summary>
    /// <param name="id">The puzzle ID.</param>
    /// <returns>A puzzle response, or null if not found.</returns>
    Task<PuzzleResponse?> GetPuzzleByIdAsync(int id);

    /// <summary>
    /// Gets all puzzles from the dataset.
    /// </summary>
    /// <returns>A list of puzzle responses.</returns>
    Task<List<PuzzleResponse>> GetAllPuzzlesAsync();
}
