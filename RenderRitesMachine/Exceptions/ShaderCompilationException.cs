namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Thrown when a shader fails to compile.
/// </summary>
public class ShaderCompilationException : Exception
{
    /// <summary>
    /// Type of shader that failed to compile.
    /// </summary>
    public string ShaderType { get; } = string.Empty;

    /// <summary>
    /// Path to the shader file.
    /// </summary>
    public string ShaderPath { get; } = string.Empty;

    /// <summary>
    /// OpenGL compilation log.
    /// </summary>
    public string CompilationLog { get; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderCompilationException"/> class.
    /// </summary>
    /// <param name="shaderType">Shader type (e.g., vertex or fragment).</param>
    /// <param name="shaderPath">Path to the shader file.</param>
    /// <param name="compilationLog">OpenGL compilation log.</param>
    public ShaderCompilationException(string shaderType, string shaderPath, string compilationLog)
        : base($"Shader compilation error for {shaderType} at '{shaderPath}': {compilationLog}")
    {
        ShaderType = shaderType;
        ShaderPath = shaderPath;
        CompilationLog = compilationLog;
    }

    /// <summary>
    /// Initializes a new instance with a custom error message.
    /// </summary>
    /// <param name="message">Error description.</param>
    public ShaderCompilationException(string message) : base(message)
    {
        ShaderType = string.Empty;
        ShaderPath = string.Empty;
        CompilationLog = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance with a custom message and inner exception.
    /// </summary>
    /// <param name="message">Error description.</param>
    /// <param name="innerException">Underlying exception.</param>
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
