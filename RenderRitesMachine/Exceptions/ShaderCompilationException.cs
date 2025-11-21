namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается при ошибке компиляции шейдера.
/// </summary>
public class ShaderCompilationException : Exception
{
    /// <summary>
    /// Тип шейдера, который не удалось скомпилировать.
    /// </summary>
    public string ShaderType { get; } = string.Empty;

    /// <summary>
    /// Путь к файлу шейдера.
    /// </summary>
    public string ShaderPath { get; } = string.Empty;

    /// <summary>
    /// Лог ошибки компиляции от OpenGL.
    /// </summary>
    public string CompilationLog { get; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ShaderCompilationException"/>.
    /// </summary>
    /// <param name="shaderType">Тип шейдера (например, "VertexShader" или "FragmentShader").</param>
    /// <param name="shaderPath">Путь к файлу шейдера.</param>
    /// <param name="compilationLog">Лог ошибки компиляции от OpenGL.</param>
    public ShaderCompilationException(string shaderType, string shaderPath, string compilationLog)
        : base($"Shader compilation error for {shaderType} at '{shaderPath}': {compilationLog}")
    {
        ShaderType = shaderType;
        ShaderPath = shaderPath;
        CompilationLog = compilationLog;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ShaderCompilationException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public ShaderCompilationException(string message) : base(message)
    {
        ShaderType = string.Empty;
        ShaderPath = string.Empty;
        CompilationLog = string.Empty;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ShaderCompilationException"/> с указанным сообщением об ошибке и ссылкой на внутреннее исключение.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    /// <param name="innerException">Исключение, являющееся причиной текущего исключения.</param>
    public ShaderCompilationException(string message, Exception innerException) : base(message, innerException)
    {
        ShaderType = string.Empty;
        ShaderPath = string.Empty;
        CompilationLog = string.Empty;
    }

    public ShaderCompilationException()
    {
    }
}
