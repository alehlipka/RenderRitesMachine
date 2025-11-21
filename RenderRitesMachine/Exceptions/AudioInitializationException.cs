namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается при ошибке инициализации аудио сервиса.
/// </summary>
public class AudioInitializationException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AudioInitializationException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public AudioInitializationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AudioInitializationException"/> с указанным сообщением об ошибке и ссылкой на внутреннее исключение.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    /// <param name="innerException">Исключение, являющееся причиной текущего исключения.</param>
    public AudioInitializationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AudioInitializationException()
    {
    }
}

