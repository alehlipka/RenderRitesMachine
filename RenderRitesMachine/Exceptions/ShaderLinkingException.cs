namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается при ошибке линковки шейдерной программы.
/// </summary>
public class ShaderLinkingException : Exception
{
    /// <summary>
    /// Имя шейдерной программы, которая не удалось слинковать.
    /// </summary>
    public string ShaderName { get; } = string.Empty;

    /// <summary>
    /// Лог ошибки линковки от OpenGL.
    /// </summary>
    public string LinkingLog { get; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ShaderLinkingException"/>.
    /// </summary>
    /// <param name="shaderName">Имя шейдерной программы.</param>
    /// <param name="linkingLog">Лог ошибки линковки от OpenGL.</param>
    public ShaderLinkingException(string shaderName, string linkingLog)
        : base($"Shader link error for '{shaderName}': {linkingLog}")
    {
        ShaderName = shaderName;
        LinkingLog = linkingLog;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ShaderLinkingException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public ShaderLinkingException(string message) : base(message)
    {
        ShaderName = string.Empty;
        LinkingLog = string.Empty;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ShaderLinkingException"/> с указанным сообщением об ошибке и ссылкой на внутреннее исключение.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    /// <param name="innerException">Исключение, являющееся причиной текущего исключения.</param>
    public ShaderLinkingException(string message, Exception innerException) : base(message, innerException)
    {
        ShaderName = string.Empty;
        LinkingLog = string.Empty;
    }

    public ShaderLinkingException()
    {
    }
}
