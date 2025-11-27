using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Thrown when an OpenGL error is detected.
/// </summary>
public class OpenGLErrorException : Exception
{
    /// <summary>
    /// OpenGL error code.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <summary>
    /// Name of the operation during which the error occurred.
    /// </summary>
    public string Operation { get; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGLErrorException"/> class.
    /// </summary>
    /// <param name="operation">Operation name where the error occurred.</param>
    /// <param name="errorCode">OpenGL error code.</param>
    public OpenGLErrorException(string operation, ErrorCode errorCode)
        : base($"OpenGL error during {operation}: {errorCode}")
    {
        Operation = operation;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance with a custom error message.
    /// </summary>
    /// <param name="message">Error description.</param>
    public OpenGLErrorException(string message) : base(message)
    {
        Operation = string.Empty;
        ErrorCode = ErrorCode.NoError;
    }

    /// <summary>
    /// Initializes a new instance with a custom error message and inner exception.
    /// </summary>
    /// <param name="message">Error description.</param>
    /// <param name="innerException">Underlying exception.</param>
    public OpenGLErrorException(string message, Exception innerException) : base(message, innerException)
    {
        Operation = string.Empty;
        ErrorCode = ErrorCode.NoError;
    }

    public OpenGLErrorException()
    {
    }
}
