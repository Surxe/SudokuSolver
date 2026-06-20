namespace Sudoku.Core.Models;

/// <summary>
/// Represents a single cell in a Sudoku board.
/// </summary>
public class Cell
{
    /// <summary>
    /// Gets the row index (0-8).
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the column index (0-8).
    /// </summary>
    public int Column { get; }

    private int _value;
    /// <summary>
    /// Gets or sets the cell value (0-9, where 0 represents empty).
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            if (value < 0 || value > 9)
                throw new ArgumentOutOfRangeException(nameof(value), "Cell value must be between 0 and 9");
            _value = value;
        }
    }

    /// <summary>
    /// Gets whether this cell was part of the initial puzzle (a given).
    /// </summary>
    public bool IsGiven { get; }

    /// <summary>
    /// Initializes a new instance of the Cell class.
    /// </summary>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <param name="value">Cell value (0-9).</param>
    /// <param name="isGiven">Whether this cell is a given (initial value).</param>
    public Cell(int row, int column, int value, bool isGiven = false)
    {
        if (row < 0 || row > 8)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8");
        if (column < 0 || column > 8)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8");

        Row = row;
        Column = column;
        Value = value;
        IsGiven = isGiven;
    }

    /// <summary>
    /// Returns a string representation of the cell.
    /// </summary>
    public override string ToString()
    {
        return Value == 0 ? "." : Value.ToString();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current cell.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Cell other)
            return false;

        return Row == other.Row && Column == other.Column && Value == other.Value;
    }

    /// <summary>
    /// Returns a hash code for the current cell.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column, Value);
    }
}
