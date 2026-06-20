using FluentAssertions;
using Sudoku.Core.Models;
using Sudoku.Core.Parsing;
using Sudoku.Core.Solvers;
using Sudoku.Core.Validation;
using Xunit;

namespace Sudoku.Core.Tests;

public class IntegrationTests
{
    [Fact]
    public void EndToEnd_LoadParseSolveValidate_ShouldWork()
    {
        // Arrange
        var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
        var testFile = Path.Combine(basePath, "data", "puzzles.json");
        var solver = new BacktrackingSolver();

        // Act - Load puzzles from JSON
        var puzzles = JsonPuzzleParser.ParsePuzzleFile(testFile);
        puzzles.Should().NotBeEmpty();

        // Take first puzzle for testing
        var puzzle = puzzles.First();
        
        // Solve the puzzle
        var result = solver.Solve(puzzle.InitialBoard);

        // Assert
        result.Success.Should().BeTrue();
        result.Solution.Should().NotBeNull();
        result.Solution.IsComplete.Should().BeTrue();

        // Validate the solution
        var validationResult = BoardValidator.ValidateSolution(result.Solution);
        validationResult.IsValid.Should().BeTrue();
        validationResult.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public void EndToEnd_SolveMultiplePuzzles_ShouldAllSucceed()
    {
        // Arrange
        var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
        var testFile = Path.Combine(basePath, "data", "puzzles.json");
        var solver = new BacktrackingSolver();

        // Act - Load puzzles from JSON
        var puzzles = JsonPuzzleParser.ParsePuzzleFile(testFile);
        
        // Solve first 10 puzzles (for performance)
        var testPuzzles = puzzles.Take(10).ToList();
        var results = new List<SolveResult>();

        foreach (var puzzle in testPuzzles)
        {
            var result = solver.Solve(puzzle.InitialBoard);
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(10);
        results.All(r => r.Success).Should().BeTrue();
        results.All(r => r.Solution != null).Should().BeTrue();
        
        // Validate all solutions
        foreach (var result in results)
        {
            var validationResult = BoardValidator.ValidateSolution(result.Solution);
            validationResult.IsValid.Should().BeTrue();
        }
    }

    [Fact]
    public void EndToEnd_SolverPerformance_ShouldBeReasonable()
    {
        // Arrange
        var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
        var testFile = Path.Combine(basePath, "data", "puzzles.json");
        var solver = new BacktrackingSolver();

        // Act - Load puzzles from JSON
        var puzzles = JsonPuzzleParser.ParsePuzzleFile(testFile);
        
        // Solve first 5 puzzles and measure performance
        var testPuzzles = puzzles.Take(5).ToList();
        var totalTime = TimeSpan.Zero;

        foreach (var puzzle in testPuzzles)
        {
            var result = solver.Solve(puzzle.InitialBoard);
            result.Success.Should().BeTrue();
            totalTime += result.Duration;
        }

        // Assert - Each puzzle should solve in less than 1 second
        var averageTime = totalTime.TotalMilliseconds / testPuzzles.Count;
        averageTime.Should().BeLessThan(1000);
    }
}
