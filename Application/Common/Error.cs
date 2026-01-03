namespace Application.Common;

/// <summary>
/// Represents an error with a code and description.
/// Immutable value object for consistent error handling across the application.
/// </summary>
public sealed record Error(string Code, string Description)
{
    /// <summary>
    /// Represents no error (success state).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Represents a null value error.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    public static Error Validation(string description) => new("Validation.Error", description);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    public static Error NotFound(string entity, long id) => 
        new($"{entity}.NotFound", $"{entity} with ID {id} was not found.");

    /// <summary>
    /// Creates a conflict error (duplicate, already exists, etc.).
    /// </summary>
    public static Error Conflict(string description) => new("Error.Conflict", description);

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    public static Error Forbidden(string description) => new("Error.Forbidden", description);

    /// <summary>
    /// Creates a business rule violation error.
    /// </summary>
    public static Error BusinessRule(string description) => new("Error.BusinessRule", description);
}
