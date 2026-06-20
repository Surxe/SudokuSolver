using FluentAssertions;
using Sudoku.Core.Models;
using Sudoku.Core.Validation;
using Sudoku.Core.Parsing;
using Sudoku.Core.Solvers;
using Xunit;

namespace Sudoku.Core.Tests;

public class CellTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateCell()
    {
        // Arrange & Act
        var cell = new Cell(0, 0, 5, true);

        // Assert
        cell.Row.Should().Be(0);
        cell.Column.Should().Be(0);
        cell.Value.Should().Be(5);
        cell.IsGiven.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithInvalidRow_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(-1, 0, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(9, 0, 5));
    }

    [Fact]
    public void Constructor_WithInvalidColumn_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(0, -1, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(0, 9, 5));
    }

    [Fact]
    public void SetValue_WithInvalidValue_ShouldThrowException()
    {
        // Arrange
        var cell = new Cell(0, 0, 5);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => cell.Value = -1);
        Assert.Throws<ArgumentOutOfRangeException>(() => cell.Value = 10);
    }

    [Fact]
    public void ToString_ShouldReturnDotForEmptyCell()
    {
        // Arrange
        var cell = new Cell(0, 0, 0);

        // Act
        var result = cell.ToString();

        // Assert
        result.Should().Be(".");
    }

    [Fact]
    public void ToString_ShouldReturnValueForNonEmptyCell()
    {
        // Arrange
        var cell = new Cell(0, 0, 5);

        // Act
        var result = cell.ToString();

        // Assert
        result.Should().Be("5");
    }
}
