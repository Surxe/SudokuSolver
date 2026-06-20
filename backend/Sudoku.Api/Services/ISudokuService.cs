using Sudoku.Api.DTOs;

namespace Sudoku.Api.Services;

/// <summary>
/// Interface for Sudoku solving operations.
/// </summary>
public interface ISudokuService
{
    /// <summary>
    /// Solves a Sudoku puzzle asynchronously.
    /// </summary>
    /// <param name="request">The solve request containing the board.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A solve response with the solution or error.</returns>
    Task<SolveResponse> SolveAsync(SolveRequest request, CancellationToken cancellationToken = default);
}
