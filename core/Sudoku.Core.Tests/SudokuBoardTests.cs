using FluentAssertions;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Core.Tests;

public class SudokuBoardTests
{
    private readonly int[,] _validBoard = new int[,]
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

    [Fact]
    public void Constructor_From2DArray_ShouldCreateBoard()
    {
        // Act
        var board = new SudokuBoard(_validBoard);

        // Assert
        board.IsValidStructure.Should().BeTrue();
        board.GetValue(0, 0).Should().Be(5);
        board.GetValue(0, 2).Should().Be(0);
    }

    [Fact]
    public void Constructor_FromInvalidSize_ShouldThrowException()
    {
        // Arrange
        var invalidBoard = new int[8, 8];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SudokuBoard(invalidBoard));
    }

    [Fact]
    public void Constructor_FromString_ShouldCreateBoard()
    {
        // Arrange
        var boardString = "530070000600195000098000060800060003400803001700020006060000280000419005000080079";

        // Act
        var board = new SudokuBoard(boardString);

        // Assert
        board.IsValidStructure.Should().BeTrue();
        board.GetValue(0, 0).Should().Be(5);
        board.GetValue(8, 8).Should().Be(9);
    }

    [Fact]
    public void Constructor_FromInvalidStringLength_ShouldThrowException()
    {
        // Arrange
        var invalidString = "530070000";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SudokuBoard(invalidString));
    }

    [Fact]
    public void Indexer_ShouldReturnCell()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var cell = board[0, 0];

        // Assert
        cell.Row.Should().Be(0);
        cell.Column.Should().Be(0);
        cell.Value.Should().Be(5);
    }

    [Fact]
    public void SetValue_ShouldUpdateCellValue()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        board.SetValue(0, 2, 1);

        // Assert
        board.GetValue(0, 2).Should().Be(1);
    }

    [Fact]
    public void GetRow_ShouldReturnCorrectRow()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var row = board.GetRow(0);

        // Assert
        row.Should().HaveCount(9);
        row[0].Value.Should().Be(5);
        row[1].Value.Should().Be(3);
    }

    [Fact]
    public void GetColumn_ShouldReturnCorrectColumn()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var column = board.GetColumn(0);

        // Assert
        column.Should().HaveCount(9);
        column[0].Value.Should().Be(5);
        column[1].Value.Should().Be(6);
    }

    [Fact]
    public void GetBox_ShouldReturnCorrectBox()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var box = board.GetBox(0, 0);

        // Assert
        box.Should().HaveCount(9);
        box[0].Value.Should().Be(5);
        box[1].Value.Should().Be(3);
    }

    [Fact]
    public void GetBoxForCell_ShouldReturnCorrectBox()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var box = board.GetBoxForCell(4, 4);

        // Assert
        box.Should().HaveCount(9);
        // The box contains cells from rows 3-5 and cols 3-5
        // Position (4,4) is the center of the middle box
        box.Should().NotBeNull();
    }

    [Fact]
    public void Clone_ShouldCreateIndependentCopy()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var clone = board.Clone();
        clone.SetValue(0, 2, 1);

        // Assert
        board.GetValue(0, 2).Should().Be(0);
        clone.GetValue(0, 2).Should().Be(1);
    }

    [Fact]
    public void IsComplete_WithEmptyCells_ShouldReturnFalse()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act & Assert
        board.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void EmptyCellCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var count = board.EmptyCellCount;

        // Assert
        count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ToArray_ShouldReturnCorrectArray()
    {
        // Arrange
        var board = new SudokuBoard(_validBoard);

        // Act
        var array = board.ToArray();

        // Assert
        array[0, 0].Should().Be(5);
        array[0, 1].Should().Be(3);
    }
}
