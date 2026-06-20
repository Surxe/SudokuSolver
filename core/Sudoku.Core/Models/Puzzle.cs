namespace Sudoku.Core.Models;

/// <summary>
/// Represents a Sudoku puzzle with an initial board and optionally a solved board.
/// </summary>
public class Puzzle
{
    /// <summary>
    /// Gets the unique identifier for the puzzle.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the initial board state.
    /// </summary>
    public SudokuBoard InitialBoard { get; }

    /// <summary>
    /// Gets the solved board state, if available.
    /// </summary>
    public SudokuBoard? SolvedBoard { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Puzzle class.
    /// </summary>
    /// <param name="id">Unique identifier for the puzzle.</param>
    /// <param name="initialBoard">The initial board state.</param>
    /// <param name="solvedBoard">The solved board state (optional).</param>
    public Puzzle(int id, SudokuBoard initialBoard, SudokuBoard? solvedBoard = null)
    {
        Id = id;
        InitialBoard = initialBoard ?? throw new ArgumentNullException(nameof(initialBoard));
        SolvedBoard = solvedBoard;
    }

    /// <summary>
    /// Sets the solved board for this puzzle.
    /// </summary>
    /// <param name="solvedBoard">The solved board state.</param>
    public void SetSolvedBoard(SudokuBoard solvedBoard)
    {
        SolvedBoard = solvedBoard ?? throw new ArgumentNullException(nameof(solvedBoard));
    }

    /// <summary>
    /// Gets whether this puzzle has a solved board.
    /// </summary>
    public bool HasSolution => SolvedBoard != null;

    /// <summary>
    /// Gets whether the initial board is already solved.
    /// </summary>
    public bool IsSolved => InitialBoard.IsComplete;

    /// <summary>
    /// Determines whether the specified puzzle is equal to the current puzzle.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Puzzle other)
            return false;

        return Id == other.Id && InitialBoard.Equals(other.InitialBoard);
    }

    /// <summary>
    /// Returns a hash code for the current puzzle.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, InitialBoard);
    }

    /// <summary>
    /// Returns a string representation of the puzzle.
    /// </summary>
    public override string ToString()
    {
        return $"Puzzle {Id}\n{InitialBoard}";
    }
}
