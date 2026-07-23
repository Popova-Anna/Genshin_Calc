namespace GenshinAccountAnalyzer.Application.Exceptions;

/// <summary>
/// Thrown when a raw account export cannot be parsed into the internal model
/// (malformed content, unsupported schema version, missing required fields, ...).
/// </summary>
public sealed class AccountImportException : Exception
{
    /// <summary>Initializes a new instance with a descriptive message.</summary>
    /// <param name="message">A human-readable description of the failure.</param>
    public AccountImportException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance with a descriptive message and the underlying cause.</summary>
    /// <param name="message">A human-readable description of the failure.</param>
    /// <param name="innerException">The exception that caused this failure.</param>
    public AccountImportException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
