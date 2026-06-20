using Sudoku.Core.Models;

namespace Sudoku.Core.Validation;

/// <summary>
/// Provides validation logic for Sudoku boards.
/// </summary>
public static class BoardValidator
{
    /// <summary>
    /// Validates the structure of a board (9x9 with values 0-9).
    /// </summary>
    /// <param name="board">The board to validate.</param>
    /// <returns>A ValidationResult indicating whether the structure is valid.</returns>
    public static ValidationResult ValidateStructure(SudokuBoard board)
    {
        var result = new ValidationResult();

        if (!board.IsValidStructure)
        {
            result.AddError("Board structure is invalid: must be 9x9 with values 0-9");
        }

        return result;
    }

    /// <summary>
    /// Validates that there are no duplicate non-zero values in any row.
    /// </summary>
    /// <param name="board">The board to validate.</param>
    /// <returns>A ValidationResult indicating whether rows are valid.</returns>
    public static ValidationResult ValidateRows(SudokuBoard board)
    {
        var result = new ValidationResult();

        for (int row = 0; row < 9; row++)
        {
            var seen = new HashSet<int>();
            var cells = board.GetRow(row);

            foreach (var cell in cells)
            {
                if (cell.Value != 0)
                {
                    if (seen.Contains(cell.Value))
                    {
                        result.AddError($"Row {row} contains duplicate value {cell.Value}");
                    }
                    seen.Add(cell.Value);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Validates that there are no duplicate non-zero values in any column.
    /// </summary>
    /// <param name="board">The board to validate.</param>
    /// <returns>A ValidationResult indicating whether columns are valid.</returns>
    public static ValidationResult ValidateColumns(SudokuBoard board)
    {
        var result = new ValidationResult();

        for (int col = 0; col < 9; col++)
        {
            var seen = new HashSet<int>();
            var cells = board.GetColumn(col);

            foreach (var cell in cells)
            {
                if (cell.Value != 0)
                {
                    if (seen.Contains(cell.Value))
                    {
                        result.AddError($"Column {col} contains duplicate value {cell.Value}");
                    }
                    seen.Add(cell.Value);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Validates that there are no duplicate non-zero values in any 3x3 box.
    /// </summary>
    /// <param name="board">The board to validate.</param>
    /// <returns>A ValidationResult indicating whether boxes are valid.</returns>
    public static ValidationResult ValidateBoxes(SudokuBoard board)
    {
        var result = new ValidationResult();

        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                var seen = new HashSet<int>();
                var cells = board.GetBox(boxRow, boxCol);

                foreach (var cell in cells)
                {
                    if (cell.Value != 0)
                    {
                        if (seen.Contains(cell.Value))
                        {
                            result.AddError($"Box ({boxRow}, {boxCol}) contains duplicate value {cell.Value}");
                        }
                        seen.Add(cell.Value);
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Validates that the board follows all Sudoku rules (no duplicates in rows, columns, or boxes).
    /// </summary>
    /// <param name="board">The board to validate.</param>
    /// <returns>A ValidationResult indicating whether the board is valid.</returns>
    public static ValidationResult ValidateBoard(SudokuBoard board)
    {
        var result = ValidateStructure(board);
        if (!result.IsValid)
            return result;

        var rowResult = ValidateRows(board);
        var colResult = ValidateColumns(board);
        var boxResult = ValidateBoxes(board);

        if (!rowResult.IsValid)
        {
            foreach (var error in rowResult.ErrorMessages)
                result.AddError(error);
        }

        if (!colResult.IsValid)
        {
            foreach (var error in colResult.ErrorMessages)
                result.AddError(error);
        }

        if (!boxResult.IsValid)
        {
            foreach (var error in boxResult.ErrorMessages)
                result.AddError(error);
        }

        return result;
    }

    /// <summary>
    /// Validates that a complete solution is correct (no zeros and all rules satisfied).
    /// </summary>
    /// <param name="board">The board to validate as a solution.</param>
    /// <returns>A ValidationResult indicating whether the solution is valid.</returns>
    public static ValidationResult ValidateSolution(SudokuBoard board)
    {
        var result = ValidateBoard(board);
        if (!result.IsValid)
            return result;

        if (!board.IsComplete)
        {
            result.AddError("Solution is not complete: contains empty cells");
        }

        return result;
    }

    /// <summary>
    /// Checks if placing a value at a specific position would violate Sudoku rules.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <param name="value">The value to place (1-9).</param>
    /// <returns>True if the placement is valid, false otherwise.</returns>
    public static bool IsValidPlacement(SudokuBoard board, int row, int column, int value)
    {
        if (value < 1 || value > 9)
            return false;

        // Check row
        var rowCells = board.GetRow(row);
        if (rowCells.Any(c => c.Value == value))
            return false;

        // Check column
        var colCells = board.GetColumn(column);
        if (colCells.Any(c => c.Value == value))
            return false;

        // Check box
        var boxCells = board.GetBoxForCell(row, column);
        if (boxCells.Any(c => c.Value == value))
            return false;

        return true;
    }

    /// <summary>
    /// Gets the valid values that can be placed at a specific position.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <param name="row">Row index (0-8).</param>
    /// <param name="column">Column index (0-8).</param>
    /// <returns>A list of valid values (1-9) for the position.</returns>
    public static List<int> GetValidValues(SudokuBoard board, int row, int column)
    {
        var validValues = new List<int>();

        for (int value = 1; value <= 9; value++)
        {
            if (IsValidPlacement(board, row, column, value))
            {
                validValues.Add(value);
            }
        }

        return validValues;
    }
}
