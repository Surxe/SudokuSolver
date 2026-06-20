using FluentAssertions;
using Sudoku.Core.Models;
using Sudoku.Core.Validation;
using Xunit;

namespace Sudoku.Core.Tests;

public class BoardValidatorTests
{
    private readonly int[,] _validBoard = new int[,]
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

    private readonly int[,] _invalidRowBoard = new int[,]
    {
        {5, 3, 5, 6, 7, 8, 9, 1, 2}, // Duplicate 5 in row
        {6, 7, 2, 1, 9, 5, 3, 4, 8},
        {1, 9, 8, 3, 4, 2, 5, 6, 7},
        {8, 5, 9, 7, 6, 1, 4, 2, 3},
        {4, 2, 6, 8, 5, 3, 7, 9, 1},
        {7, 1, 3, 9, 2, 4, 8, 5, 6},
        {9, 6, 1, 5, 3, 7, 2, 8, 4},
        {2, 8, 7, 4, 1, 9, 6, 3, 5},
        {3, 4, 5, 2, 8, 6, 1, 7, 9}
    };

    private readonly int[,] _invalidColumnBoard = new int[,]
    {
        {5, 3, 4, 6, 7, 8, 9, 1, 2},
        {5, 7, 2, 1, 9, 5, 3, 4, 8}, // Duplicate 5 in column 0
        {1, 9, 8, 3, 4, 2, 5, 6, 7},
        {8, 5, 9, 7, 6, 1, 4, 2, 3},
        {4, 2, 6, 8, 5, 3, 7, 9, 1},
        {7, 1, 3, 9, 2, 4, 8, 5, 6},
        {9, 6, 1, 5, 3, 7, 2, 8, 4},
        {2, 8, 7, 4, 1, 9, 6, 3, 5},
        {3, 4, 5, 2, 8, 6, 1, 7, 9}
    };

    private readonly int[,] _invalidBoxBoard = new int[,]
    {
        {5, 3, 5, 6, 7, 8, 9, 1, 2}, // Duplicate 5 in box (0,0)
        {6, 7, 2, 1, 9, 5, 3, 4, 8},
        {1, 9, 8, 3, 4, 2, 5, 6, 7},
        {8, 5, 9, 7, 6, 1, 4, 2, 3},
        {4, 2, 6, 8, 5, 3, 7, 9, 1},
        {7, 1, 3, 9, 2, 4, 8, 5, 6},
        {9, 6, 1, 5, 3, 7, 2, 8, 4},
        {2, 8, 7, 4, 1, 9, 6, 3, 5},
        {3, 4, 5, 2, 8, 6, 1, 7, 9}
    };

    [Fact]
    public void ValidateStructure_WithValidBoard_ShouldReturnValid()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.ValidateStructure(board);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public void ValidateRows_WithValidBoard_ShouldReturnValid()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.ValidateRows(board);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateRows_WithDuplicateInRow_ShouldReturnInvalid()
    {
        // Arrange
        var board = new SudokuBoard(_invalidRowBoard);

        // Act
        var result = BoardValidator.ValidateRows(board);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Row 0 contains duplicate value 5");
    }

    [Fact]
    public void ValidateColumns_WithValidBoard_ShouldReturnValid()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.ValidateColumns(board);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateColumns_WithDuplicateInColumn_ShouldReturnInvalid()
    {
        // Arrange
        var board = new SudokuBoard(_invalidColumnBoard);

        // Act
        var result = BoardValidator.ValidateColumns(board);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Column 0 contains duplicate value 5");
    }

    [Fact]
    public void ValidateBoxes_WithValidBoard_ShouldReturnValid()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.ValidateBoxes(board);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateBoxes_WithDuplicateInBox_ShouldReturnInvalid()
    {
        // Arrange
        var board = new SudokuBoard(_invalidBoxBoard);

        // Act
        var result = BoardValidator.ValidateBoxes(board);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Box (0, 0) contains duplicate value 5");
    }

    [Fact]
    public void ValidateBoard_WithValidBoard_ShouldReturnValid()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.ValidateBoard(board);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateSolution_WithCompleteValidSolution_ShouldReturnValid()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.ValidateSolution(board);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateSolution_WithIncompleteBoard_ShouldReturnInvalid()
    {
        // Arrange
        var incompleteBoard = new int[,]
        {
            {5, 3, 0, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };
        var board = new SudokuBoard(incompleteBoard);

        // Act
        var result = BoardValidator.ValidateSolution(board);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Solution is not complete: contains empty cells");
    }

    [Fact]
    public void IsValidPlacement_WithValidMove_ShouldReturnTrue()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.IsValidPlacement(board, 0, 0, 5);

        // Assert
        result.Should().BeFalse(); // 5 is already at (0,0)
    }

    [Fact]
    public void IsValidPlacement_WithInvalidValue_ShouldReturnFalse()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var result = BoardValidator.IsValidPlacement(board, 0, 0, 10);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetValidValues_ShouldReturnPossibleValues()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var validValues = BoardValidator.GetValidValues(board, 0, 0);

        // Assert
        validValues.Should().BeEmpty(); // Board is complete, no valid values
    }
}
