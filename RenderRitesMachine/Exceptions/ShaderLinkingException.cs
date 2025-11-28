namespace RenderRitesMachine.Exceptions;

/// <summary>
///     Thrown when a shader program fails to link.
/// </summary>
public class ShaderLinkingException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ShaderLinkingException" /> class.
    /// </summary>
    /// <param name="shaderName">Shader program name.</param>
    /// <param name="linkingLog">OpenGL linking log.</param>
    public ShaderLinkingException(string shaderName, string linkingLog)
        : base($"Shader link error for '{shaderName}': {linkingLog}")
    {
        ShaderName = shaderName;
        LinkingLog = linkingLog;
    }

    /// <summary>
    ///     Initializes a new instance with a custom error message.
    /// </summary>
    /// <param name="message">Error description.</param>
    public ShaderLinkingException(string message) : base(message)
    {
        ShaderName = string.Empty;
        LinkingLog = string.Empty;
    }

    /// <summary>
    ///     Initializes a new instance with a custom message and inner exception.
    /// </summary>
    /// <param name="message">Error description.</param>
    /// <param name="innerException">Underlying exception.</param>
    public ShaderLinkingException(string message, Exception innerException) : base(message, innerException)
    {
        ShaderName = string.Empty;
        LinkingLog = string.Empty;
    }

    public ShaderLinkingException()
    {
    }

    /// <summary>
    ///     Name of the shader program that failed to link.
    /// </summary>
    public string ShaderName { get; } = string.Empty;

    /// <summary>
    ///     OpenGL linking log.
    /// </summary>
    public string LinkingLog { get; } = string.Empty;
}
