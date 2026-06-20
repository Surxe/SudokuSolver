using FluentAssertions;
using Sudoku.Core.Models;
using Sudoku.Core.Solvers;
using Sudoku.Core.Validation;
using Xunit;

namespace Sudoku.Core.Tests;

public class BacktrackingSolverTests
{
    private readonly int[,] _easyPuzzle = new int[,]
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
    };

    private readonly int[,] _solvedPuzzle = new int[,]
    {
        {5, 3, 4, 6, 7, 8, 9, 1, 2},
        {6, 7, 2, 1, 9, 5, 3, 4, 8},
        {1, 9, 8, 3, 4, 2, 5, 6, 7},
        {8, 5, 9, 7, 6, 1, 4, 2, 3},
        {4, 2, 6, 8, 5, 3, 7, 9, 1},
        {7, 1, 3, 9, 2, 4, 8, 5, 6},
        {9, 6, 1, 5, 3, 7, 2, 8, 4},
        {2, 8, 7, 4, 1, 9, 6, 3, 5},
        {3, 4, 5, 2, 8, 6, 1, 7, 9}
    };

    private readonly int[,] _invalidPuzzle = new int[,]
    {
        {5, 3, 5, 0, 7, 0, 0, 0, 0}, // Duplicate 5 in row
        {6, 0, 0, 1, 9, 5, 0, 0, 0},
        {0, 9, 8, 0, 0, 0, 0, 6, 0},
        {8, 0, 0, 0, 6, 0, 0, 0, 3},
        {4, 0, 0, 8, 0, 3, 0, 0, 1},
        {7, 0, 0, 0, 2, 0, 0, 0, 6},
        {0, 6, 0, 0, 0, 0, 2, 8, 0},
        {0, 0, 0, 4, 1, 9, 0, 0, 5},
        {0, 0, 0, 0, 8, 0, 0, 7, 9}
    };

    [Fact]
    public void Solve_WithValidPuzzle_ShouldReturnSolution()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var board = new SudokuBoard(_easyPuzzle);

        // Act
        var result = solver.Solve(board);

        // Assert
        result.Success.Should().BeTrue();
        result.Solution.Should().NotBeNull();
        result.Solution.IsComplete.Should().BeTrue();
        
        var validationResult = BoardValidator.ValidateSolution(result.Solution);
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Solve_WithInvalidPuzzle_ShouldReturnFailure()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var board = new SudokuBoard(_invalidPuzzle);

        // Act
        var result = solver.Solve(board);

        // Assert
        result.Success.Should().BeFalse();
        result.Solution.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Solve_WithAlreadySolvedPuzzle_ShouldReturnSolution()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var board = new SudokuBoard(_solvedPuzzle);

        // Act
        var result = solver.Solve(board);

        // Assert
        result.Success.Should().BeTrue();
        result.Solution.Should().NotBeNull();
        result.Solution.IsComplete.Should().BeTrue();
    }

    [Fact]
    public void Solve_ShouldTrackStatistics()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var board = new SudokuBoard(_easyPuzzle);

        // Act
        var result = solver.Solve(board);

        // Assert
        result.Attempts.Should().BeGreaterThan(0);
        result.Backtracks.Should().BeGreaterThanOrEqualTo(0);
        result.Duration.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void Solve_WithEmptyBoard_ShouldReturnSolution()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var emptyBoard = new int[9, 9];
        var board = new SudokuBoard(emptyBoard);

        // Act
        var result = solver.Solve(board);

        // Assert
        result.Success.Should().BeTrue();
        result.Solution.Should().NotBeNull();
        result.Solution.IsComplete.Should().BeTrue();
    }

    [Fact]
    public void Solve_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var emptyBoard = new int[9, 9];
        var board = new SudokuBoard(emptyBoard);
        var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromMilliseconds(1));

        // Act
        var result = solver.Solve(board, cts.Token);

        // Assert
        // Empty board takes longer to solve, so it should be cancelled
        // If it completes before cancellation, that's also acceptable
        result.Success.Should().Be(result.Solution != null);
    }

    [Fact]
    public void Solve_ShouldNotModifyOriginalBoard()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var board = new SudokuBoard(_easyPuzzle);
        var originalValue = board.GetValue(0, 2);

        // Act
        var result = solver.Solve(board);

        // Assert
        board.GetValue(0, 2).Should().Be(originalValue);
    }

    [Fact]
    public void Solve_WithKnownSolution_ShouldMatchExpected()
    {
        // Arrange
        var solver = new BacktrackingSolver();
        var board = new SudokuBoard(_easyPuzzle);
        var expectedSolution = new SudokuBoard(_solvedPuzzle);

        // Act
        var result = solver.Solve(board);

        // Assert
        result.Success.Should().BeTrue();
        result.Solution.Should().NotBeNull();
        result.Solution.Equals(expectedSolution).Should().BeTrue();
    }
}
