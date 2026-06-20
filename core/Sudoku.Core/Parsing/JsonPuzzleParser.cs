using System.Text.Json;
using Sudoku.Core.Models;

namespace Sudoku.Core.Parsing;

/// <summary>
/// Provides functionality to parse Sudoku puzzles from JSON format.
/// </summary>
public static class JsonPuzzleParser
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Parses a single puzzle from JSON.
    /// </summary>
    /// <param name="json">The JSON string representing a puzzle.</param>
    /// <returns>A Puzzle object.</returns>
    /// <exception cref="JsonException">If the JSON is invalid.</exception>
    /// <exception cref="ArgumentException">If the puzzle data is invalid.</exception>
    public static Puzzle ParsePuzzle(string json)
    {
        var dto = JsonSerializer.Deserialize<PuzzleDto>(json, _options)
            ?? throw new JsonException("Failed to deserialize puzzle JSON");

        return ConvertDtoToPuzzle(dto);
    }

    /// <summary>
    /// Parses multiple puzzles from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>A list of Puzzle objects.</returns>
    /// <exception cref="FileNotFoundException">If the file does not exist.</exception>
    /// <exception cref="JsonException">If the JSON is invalid.</exception>
    /// <exception cref="ArgumentException">If the puzzle data is invalid.</exception>
    public static List<Puzzle> ParsePuzzleFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Puzzle file not found: {filePath}");

        var json = File.ReadAllText(filePath);
        return ParsePuzzleArray(json);
    }

    /// <summary>
    /// Parses multiple puzzles from a JSON array string.
    /// </summary>
    /// <param name="json">The JSON string representing an array of puzzles.</param>
    /// <returns>A list of Puzzle objects.</returns>
    /// <exception cref="JsonException">If the JSON is invalid.</exception>
    /// <exception cref="ArgumentException">If the puzzle data is invalid.</exception>
    public static List<Puzzle> ParsePuzzleArray(string json)
    {
        var dtos = JsonSerializer.Deserialize<PuzzleDto[]>(json, _options)
            ?? throw new JsonException("Failed to deserialize puzzle array JSON");

        var puzzles = new List<Puzzle>();
        foreach (var dto in dtos)
        {
            puzzles.Add(ConvertDtoToPuzzle(dto));
        }

        return puzzles;
    }

    /// <summary>
    /// Converts a PuzzleDto to a Puzzle object.
    /// </summary>
    /// <param name="dto">The PuzzleDto to convert.</param>
    /// <returns>A Puzzle object.</returns>
    /// <exception cref="ArgumentException">If the DTO data is invalid.</exception>
    private static Puzzle ConvertDtoToPuzzle(PuzzleDto dto)
    {
        if (dto.Board == null || dto.Board.Length != 9)
            throw new ArgumentException($"Puzzle {dto.Id} board must be 9x9");

        var boardArray = ConvertJaggedTo2D(dto.Board);
        var initialBoard = new SudokuBoard(boardArray);

        SudokuBoard? solvedBoard = null;
        if (dto.SolvedBoard != null)
        {
            if (dto.SolvedBoard.Length != 9)
                throw new ArgumentException($"Puzzle {dto.Id} solved board must be 9x9");

            var solvedArray = ConvertJaggedTo2D(dto.SolvedBoard);
            solvedBoard = new SudokuBoard(solvedArray);
        }

        return new Puzzle(dto.Id, initialBoard, solvedBoard);
    }

    /// <summary>
    /// Converts a jagged array to a 2D array.
    /// </summary>
    /// <param name="jaggedArray">The jagged array to convert.</param>
    /// <returns>A 2D array.</returns>
    private static int[,] ConvertJaggedTo2D(int[][] jaggedArray)
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
    /// Serializes a puzzle to JSON.
    /// </summary>
    /// <param name="puzzle">The puzzle to serialize.</param>
    /// <returns>A JSON string representing the puzzle.</returns>
    public static string SerializePuzzle(Puzzle puzzle)
    {
        var dto = ConvertPuzzleToDto(puzzle);
        return JsonSerializer.Serialize(dto, _options);
    }

    /// <summary>
    /// Serializes a list of puzzles to a JSON array.
    /// </summary>
    /// <param name="puzzles">The puzzles to serialize.</param>
    /// <returns>A JSON string representing the puzzles.</returns>
    public static string SerializePuzzleArray(List<Puzzle> puzzles)
    {
        var dtos = puzzles.Select(ConvertPuzzleToDto).ToArray();
        return JsonSerializer.Serialize(dtos, _options);
    }

    /// <summary>
    /// Converts a Puzzle to a PuzzleDto.
    /// </summary>
    /// <param name="puzzle">The puzzle to convert.</param>
    /// <returns>A PuzzleDto object.</returns>
    private static PuzzleDto ConvertPuzzleToDto(Puzzle puzzle)
    {
        var dto = new PuzzleDto
        {
            Id = puzzle.Id,
            Board = Convert2DToJagged(puzzle.InitialBoard.ToArray())
        };

        if (puzzle.SolvedBoard != null)
        {
            dto.SolvedBoard = Convert2DToJagged(puzzle.SolvedBoard.ToArray());
        }

        return dto;
    }

    /// <summary>
    /// Converts a 2D array to a jagged array.
    /// </summary>
    /// <param name="array2D">The 2D array to convert.</param>
    /// <returns>A jagged array.</returns>
    private static int[][] Convert2DToJagged(int[,] array2D)
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
