namespace Sudoku.Core.Validation;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets whether the validation passed.
    /// </summary>
    public bool IsValid { get; private set; }

    /// <summary>
    /// Gets a list of error messages from the validation.
    /// </summary>
    public List<string> ErrorMessages { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationResult class.
    /// </summary>
    public ValidationResult()
    {
        IsValid = true;
        ErrorMessages = new List<string>();
    }

    /// <summary>
    /// Adds an error message to the validation result.
    /// </summary>
    /// <param name="message">The error message to add.</param>
    public void AddError(string message)
    {
        IsValid = false;
        ErrorMessages.Add(message);
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Success() => new ValidationResult();

    /// <summary>
    /// Creates a failed validation result with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public static ValidationResult Failure(string message)
    {
        var result = new ValidationResult();
        result.AddError(message);
        return result;
    }
}
