using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается при обнаружении ошибки OpenGL.
/// </summary>
public class OpenGLErrorException : Exception
{
    /// <summary>
    /// Код ошибки OpenGL.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <summary>
    /// Название операции, во время которой произошла ошибка.
    /// </summary>
    public string Operation { get; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OpenGLErrorException"/>.
    /// </summary>
    /// <param name="operation">Название операции, во время которой произошла ошибка.</param>
    /// <param name="errorCode">Код ошибки OpenGL.</param>
    public OpenGLErrorException(string operation, ErrorCode errorCode)
        : base($"OpenGL error during {operation}: {errorCode}")
    {
        Operation = operation;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OpenGLErrorException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public OpenGLErrorException(string message) : base(message)
    {
        Operation = string.Empty;
        ErrorCode = ErrorCode.NoError;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OpenGLErrorException"/> с указанным сообщением об ошибке и ссылкой на внутреннее исключение.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    /// <param name="innerException">Исключение, являющееся причиной текущего исключения.</param>
    public OpenGLErrorException(string message, Exception innerException) : base(message, innerException)
    {
        Operation = string.Empty;
        ErrorCode = ErrorCode.NoError;
    }

    public OpenGLErrorException()
    {
    }
}
