using Sudoku.Api.DTOs;
using Sudoku.Core.Models;
using Sudoku.Core.Parsing;

namespace Sudoku.Api.Services;

/// <summary>
/// Service for puzzle data access operations.
/// </summary>
public class PuzzleRepositoryService : IPuzzleRepositoryService
{
    private List<Puzzle>? _cachedPuzzles;
    private readonly object _lock = new();
    private readonly string _puzzleDataPath;

    public PuzzleRepositoryService(IConfiguration configuration)
    {
        _puzzleDataPath = configuration.GetValue<string>("PuzzleDataPath") 
            ?? Path.Combine("..", "..", "..", "..", "..", "data", "puzzles.json");
    }

    /// <summary>
    /// Gets a random puzzle from the dataset.
    /// </summary>
    public async Task<PuzzleResponse?> GetRandomPuzzleAsync()
    {
        var puzzles = await GetCachedPuzzlesAsync();
        if (puzzles == null || puzzles.Count == 0)
            return null;

        var random = new Random();
        var puzzle = puzzles[random.Next(puzzles.Count)];
        return ConvertToResponse(puzzle);
    }

    /// <summary>
    /// Gets a specific puzzle by ID.
    /// </summary>
    public async Task<PuzzleResponse?> GetPuzzleByIdAsync(int id)
    {
        var puzzles = await GetCachedPuzzlesAsync();
        if (puzzles == null)
            return null;

        var puzzle = puzzles.FirstOrDefault(p => p.Id == id);
        return puzzle != null ? ConvertToResponse(puzzle) : null;
    }

    /// <summary>
    /// Gets all puzzles from the dataset.
    /// </summary>
    public async Task<List<PuzzleResponse>> GetAllPuzzlesAsync()
    {
        var puzzles = await GetCachedPuzzlesAsync();
        if (puzzles == null)
            return new List<PuzzleResponse>();

        return puzzles.Select(ConvertToResponse).ToList();
    }

    /// <summary>
    /// Gets cached puzzles or loads them from file.
    /// </summary>
    private async Task<List<Puzzle>?> GetCachedPuzzlesAsync()
    {
        if (_cachedPuzzles != null)
            return _cachedPuzzles;

        lock (_lock)
        {
            if (_cachedPuzzles != null)
                return _cachedPuzzles;

            try
            {
                var fullPath = Path.GetFullPath(_puzzleDataPath);
                if (!File.Exists(fullPath))
                {
                    return null;
                }

                var puzzles = JsonPuzzleParser.ParsePuzzleFile(fullPath);
                _cachedPuzzles = puzzles;
                return _cachedPuzzles;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Converts a Core Puzzle to a PuzzleResponse DTO.
    /// </summary>
    private PuzzleResponse ConvertToResponse(Puzzle puzzle)
    {
        return new PuzzleResponse
        {
            Id = puzzle.Id,
            Board = Convert2DToJagged(puzzle.InitialBoard.ToArray())
        };
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
