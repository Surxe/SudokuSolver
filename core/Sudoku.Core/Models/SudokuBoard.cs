namespace Sudoku.Core.Models;

/// <summary>
/// Represents a 9x9 Sudoku board.
/// </summary>
public class SudokuBoard
{
    private readonly Cell[,] _cells;

    /// <summary>
    /// Gets the cells of the board as a 2D array.
    /// </summary>
    public Cell[,] Cells => _cells;

    /// <summary>
    /// Initializes a new instance of the SudokuBoard class from a 2D array.
    /// </summary>
    /// <param name="board">9x9 2D array of integers (0-9, where 0 represents empty).</param>
    public SudokuBoard(int[,] board)
    {
        if (board.GetLength(0) != 9 || board.GetLength(1) != 9)
            throw new ArgumentException("Board must be 9x9", nameof(board));

        _cells = new Cell[9, 9];
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int value = board[row, col];
                bool isGiven = value != 0;
                _cells[row, col] = new Cell(row, col, value, isGiven);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the SudokuBoard class from an 81-character string.
    /// </summary>
    /// <param name="boardString">81-character string representing the board (0-9, where 0 represents empty).</param>
    public SudokuBoard(string boardString)
    {
        if (boardString.Length != 81)
            throw new ArgumentException("Board string must be 81 characters", nameof(boardString));

        _cells = new Cell[9, 9];
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            int value = int.Parse(boardString[i].ToString());
            bool isGiven = value != 0;
            _cells[row, col] = new Cell(row, col, value, isGiven);
        }
    }

    /// <summary>
    /// Gets or sets the cell at the specified row and column.
    /// </summary>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <returns>The cell at the specified position.</returns>
    public Cell this[int row, int column]
    {
        get
        {
            if (row < 0 || row > 8 || column < 0 || column > 8)
                throw new ArgumentOutOfRangeException("Row and column must be between 0 and 8");
            return _cells[row, column];
        }
        set
        {
            if (row < 0 || row > 8 || column < 0 || column > 8)
                throw new ArgumentOutOfRangeException("Row and column must be between 0 and 8");
            _cells[row, column] = value;
        }
    }

    /// <summary>
    /// Gets the value at the specified row and column.
    /// </summary>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <returns>The cell value (0-9).</returns>
    public int GetValue(int row, int column)
    {
        return _cells[row, column].Value;
    }

    /// <summary>
    /// Sets the value at the specified row and column.
    /// </summary>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <param name="value">The value to set (0-9).</param>
    public void SetValue(int row, int column, int value)
    {
        _cells[row, column].Value = value;
    }

    /// <summary>
    /// Gets all cells in the specified row.
    /// </summary>
    /// <param name="row">Row index (0-8).</param>
    /// <returns>An array of 9 cells in the specified row.</returns>
    public Cell[] GetRow(int row)
    {
        var cells = new Cell[9];
        for (int col = 0; col < 9; col++)
        {
            cells[col] = _cells[row, col];
        }
        return cells;
    }

    /// <summary>
    /// Gets all cells in the specified column.
    /// </summary>
    /// <param name="column">Column index (0-8).</param>
    /// <returns>An array of 9 cells in the specified column.</returns>
    public Cell[] GetColumn(int column)
    {
        var cells = new Cell[9];
        for (int row = 0; row < 9; row++)
        {
            cells[row] = _cells[row, column];
        }
        return cells;
    }

    /// <summary>
    /// Gets all cells in the specified 3x3 box.
    /// </summary>
    /// <param name="boxRow">Box row index (0-2).</param>
    /// <param name="boxColumn">Box column index (0-2).</param>
    /// <returns>An array of 9 cells in the specified box.</returns>
    public Cell[] GetBox(int boxRow, int boxColumn)
    {
        var cells = new Cell[9];
        int index = 0;
        for (int row = boxRow * 3; row < boxRow * 3 + 3; row++)
        {
            for (int col = boxColumn * 3; col < boxColumn * 3 + 3; col++)
            {
                cells[index++] = _cells[row, col];
            }
        }
        return cells;
    }

    /// <summary>
    /// Gets the 3x3 box that contains the specified cell.
    /// </summary>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <returns>An array of 9 cells in the box containing the specified cell.</returns>
    public Cell[] GetBoxForCell(int row, int column)
    {
        int boxRow = row / 3;
        int boxColumn = column / 3;
        return GetBox(boxRow, boxColumn);
    }

    /// <summary>
    /// Creates a deep copy of the board.
    /// </summary>
    /// <returns>A new SudokuBoard that is a copy of this board.</returns>
    public SudokuBoard Clone()
    {
        var boardArray = new int[9, 9];
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                boardArray[row, col] = _cells[row, col].Value;
            }
        }
        return new SudokuBoard(boardArray);
    }

    /// <summary>
    /// Gets whether the board has a valid structure (9x9 with values 0-9).
    /// </summary>
    public bool IsValidStructure
    {
        get
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = _cells[row, col].Value;
                    if (value < 0 || value > 9)
                        return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Gets whether the board is completely solved (no empty cells).
    /// </summary>
    public bool IsComplete
    {
        get
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (_cells[row, col].Value == 0)
                        return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Gets the number of empty cells (cells with value 0).
    /// </summary>
    public int EmptyCellCount
    {
        get
        {
            int count = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (_cells[row, col].Value == 0)
                        count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Returns a string representation of the board for visualization.
    /// </summary>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (int row = 0; row < 9; row++)
        {
            if (row > 0 && row % 3 == 0)
                sb.AppendLine("------+-------+------");
            
            for (int col = 0; col < 9; col++)
            {
                if (col > 0 && col % 3 == 0)
                    sb.Append("| ");
                
                sb.Append(_cells[row, col].ToString());
                sb.Append(" ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    /// Converts the board to a 2D integer array.
    /// </summary>
    /// <returns>A 9x9 2D array of integers.</returns>
    public int[,] ToArray()
    {
        var array = new int[9, 9];
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                array[row, col] = _cells[row, col].Value;
            }
        }
        return array;
    }

    /// <summary>
    /// Determines whether the specified board is equal to the current board.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not SudokuBoard other)
            return false;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (_cells[row, col].Value != other._cells[row, col].Value)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns a hash code for the current board.
    /// </summary>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                hash.Add(_cells[row, col].Value);
            }
        }
        return hash.ToHashCode();
    }
}
