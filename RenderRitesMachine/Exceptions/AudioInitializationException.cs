namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Thrown when the audio service fails to initialize.
/// </summary>
public class AudioInitializationException : Exception
{
    /// <summary>
    /// Initializes a new instance with a custom error message.
    /// </summary>
    /// <param name="message">Error description.</param>
    public AudioInitializationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom message and inner exception.
    /// </summary>
    /// <param name="message">Error description.</param>
    /// <param name="innerException">Underlying exception.</param>
    public AudioInitializationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AudioInitializationException()
    {
    }
}

