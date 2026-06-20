using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.DTOs;
using Sudoku.Api.Services;

namespace Sudoku.Api.Controllers;

/// <summary>
/// Controller for Sudoku puzzle operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SudokuController : ControllerBase
{
    private readonly ISudokuService _sudokuService;
    private readonly IPuzzleRepositoryService _puzzleRepositoryService;
    private readonly ILogger<SudokuController> _logger;

    public SudokuController(
        ISudokuService sudokuService,
        IPuzzleRepositoryService puzzleRepositoryService,
        ILogger<SudokuController> logger)
    {
        _sudokuService = sudokuService;
        _puzzleRepositoryService = puzzleRepositoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a random puzzle from the dataset.
    /// </summary>
    /// <returns>A puzzle response.</returns>
    [HttpGet("puzzle")]
    [ProducesResponseType(typeof(PuzzleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PuzzleResponse>> GetRandomPuzzle()
    {
        try
        {
            var puzzle = await _puzzleRepositoryService.GetRandomPuzzleAsync();
            
            if (puzzle == null)
            {
                _logger.LogWarning("No puzzles available in the dataset");
                return NotFound("No puzzles available");
            }

            return Ok(puzzle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving random puzzle");
            return StatusCode(500, "An error occurred while retrieving the puzzle");
        }
    }

    /// <summary>
    /// Gets a specific puzzle by ID.
    /// </summary>
    /// <param name="id">The puzzle ID.</param>
    /// <returns>A puzzle response.</returns>
    [HttpGet("puzzle/{id}")]
    [ProducesResponseType(typeof(PuzzleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PuzzleResponse>> GetPuzzleById(int id)
    {
        try
        {
            var puzzle = await _puzzleRepositoryService.GetPuzzleByIdAsync(id);
            
            if (puzzle == null)
            {
                _logger.LogWarning("Puzzle with ID {PuzzleId} not found", id);
                return NotFound($"Puzzle with ID {id} not found");
            }

            return Ok(puzzle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving puzzle with ID {PuzzleId}", id);
            return StatusCode(500, "An error occurred while retrieving the puzzle");
        }
    }

    /// <summary>
    /// Solves a Sudoku puzzle.
    /// </summary>
    /// <param name="request">The solve request containing the board.</param>
    /// <returns>A solve response with the solution or error.</returns>
    [HttpPost("solve")]
    [ProducesResponseType(typeof(SolveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SolveResponse>> SolvePuzzle([FromBody] SolveRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _sudokuService.SolveAsync(request);

            if (response.Solved)
            {
                _logger.LogInformation("Puzzle solved successfully in {DurationMs}ms with {Attempts} attempts", 
                    response.DurationMs, response.Attempts);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Failed to solve puzzle: {ErrorMessage}", response.ErrorMessage);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error solving puzzle");
            return StatusCode(500, "An error occurred while solving the puzzle");
        }
    }
}
