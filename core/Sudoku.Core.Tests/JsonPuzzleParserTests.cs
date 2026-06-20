using FluentAssertions;
using Sudoku.Core.Models;
using Sudoku.Core.Parsing;
using Xunit;

namespace Sudoku.Core.Tests;

public class JsonPuzzleParserTests
{
    private const string ValidPuzzleJson = @"{
        ""id"": 1,
        ""board"": [
            [5, 3, 0, 0, 7, 0, 0, 0, 0],
            [6, 0, 0, 1, 9, 5, 0, 0, 0],
            [0, 9, 8, 0, 0, 0, 0, 6, 0],
            [8, 0, 0, 0, 6, 0, 0, 0, 3],
            [4, 0, 0, 8, 0, 3, 0, 0, 1],
            [7, 0, 0, 0, 2, 0, 0, 0, 6],
            [0, 6, 0, 0, 0, 0, 2, 8, 0],
            [0, 0, 0, 4, 1, 9, 0, 0, 5],
            [0, 0, 0, 0, 8, 0, 0, 7, 9]
        ]
    }";

    private const string PuzzleArrayJson = @"[
        {
            ""id"": 1,
            ""board"": [
                [5, 3, 0, 0, 7, 0, 0, 0, 0],
                [6, 0, 0, 1, 9, 5, 0, 0, 0],
                [0, 9, 8, 0, 0, 0, 0, 6, 0],
                [8, 0, 0, 0, 6, 0, 0, 0, 3],
                [4, 0, 0, 8, 0, 3, 0, 0, 1],
                [7, 0, 0, 0, 2, 0, 0, 0, 6],
                [0, 6, 0, 0, 0, 0, 2, 8, 0],
                [0, 0, 0, 4, 1, 9, 0, 0, 5],
                [0, 0, 0, 0, 8, 0, 0, 7, 9]
            ]
        },
        {
            ""id"": 2,
            ""board"": [
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0]
            ]
        }
    ]";

    [Fact]
    public void ParsePuzzle_WithValidJson_ShouldReturnPuzzle()
    {
        // Act
        var puzzle = JsonPuzzleParser.ParsePuzzle(ValidPuzzleJson);

        // Assert
        puzzle.Id.Should().Be(1);
        puzzle.InitialBoard.GetValue(0, 0).Should().Be(5);
        puzzle.InitialBoard.GetValue(0, 2).Should().Be(0);
    }

    [Fact]
    public void ParsePuzzle_WithInvalidJson_ShouldThrowException()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() => JsonPuzzleParser.ParsePuzzle(invalidJson));
    }

    [Fact]
    public void ParsePuzzleArray_WithValidJson_ShouldReturnPuzzleList()
    {
        // Act
        var puzzles = JsonPuzzleParser.ParsePuzzleArray(PuzzleArrayJson);

        // Assert
        puzzles.Should().HaveCount(2);
        puzzles[0].Id.Should().Be(1);
        puzzles[1].Id.Should().Be(2);
    }

    [Fact]
    public void ParsePuzzleFile_WithExistingFile_ShouldReturnPuzzleList()
    {
        // Arrange
        var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
        var testFile = Path.Combine(basePath, "data", "puzzles.json");

        // Act
        var puzzles = JsonPuzzleParser.ParsePuzzleFile(testFile);

        // Assert
        puzzles.Should().NotBeEmpty();
        puzzles[0].Id.Should().Be(1);
    }

    [Fact]
    public void ParsePuzzleFile_WithNonExistentFile_ShouldThrowException()
    {
        // Arrange
        var nonExistentFile = "data/nonexistent.json";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => JsonPuzzleParser.ParsePuzzleFile(nonExistentFile));
    }

    [Fact]
    public void SerializePuzzle_ShouldReturnValidJson()
    {
        // Arrange
        var board = new SudokuBoard(new int[,]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        });
        var puzzle = new Puzzle(1, board);

        // Act
        var json = JsonPuzzleParser.SerializePuzzle(puzzle);

        // Assert
        json.Should().Contain("\"id\":1");
        json.Should().Contain("\"board\"");
    }

    [Fact]
    public void SerializePuzzleArray_ShouldReturnValidJson()
    {
        // Arrange
        var board1 = new SudokuBoard(new int[,]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        });
        var board2 = new SudokuBoard(new int[,]
        {
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0}
        });
        var puzzles = new List<Puzzle> { new Puzzle(1, board1), new Puzzle(2, board2) };

        // Act
        var json = JsonPuzzleParser.SerializePuzzleArray(puzzles);

        // Assert
        json.Should().Contain("\"id\":1");
        json.Should().Contain("\"id\":2");
    }
}
