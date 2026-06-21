using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.DTOs;
using Sudoku.Api.Services;
using Sudoku.Core.Models;
using Sudoku.Core.Solvers;
using System.Text.Json;

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

    /// <summary>
    /// Solves a Sudoku puzzle with streaming progress via Server-Sent Events.
    /// </summary>
    /// <param name="board">The board as a URL-encoded JSON array.</param>
    /// <returns>A stream of solve progress events.</returns>
    [HttpGet("solve/stream")]
    [Produces("text/event-stream")]
    public async Task SolveStream([FromQuery] string board)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no"); // Disable nginx buffering

        try
        {
            var boardData = JsonSerializer.Deserialize<int[][]>(board);
            if (boardData == null || boardData.Length != 9 || boardData.Any(row => row.Length != 9))
            {
                await SendErrorEvent("Invalid board format");
                return;
            }

            // Convert jagged array to 2D array
            var board2D = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board2D[i, j] = boardData[i][j];
                }
            }

            var sudokuBoard = new SudokuBoard(board2D);
            var callback = new SolverProgressCallback(Response);
            var solver = new BacktrackingSolver();

            // Solve synchronously to maintain response context
            solver.Solve(sudokuBoard, callback);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing board JSON");
            await SendErrorEvent("Invalid board JSON format");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during streaming solve");
            await SendErrorEvent(ex.Message);
        }
    }

    /// <summary>
    /// Sends an error event to the SSE stream.
    /// </summary>
    private async Task SendErrorEvent(string errorMessage)
    {
        var errorEvent = new SolveStreamEvent
        {
            EventType = "error",
            ErrorMessage = errorMessage
        };
        var json = JsonSerializer.Serialize(errorEvent);
        await Response.WriteAsync($"data: {json}\n\n");
    }
}
