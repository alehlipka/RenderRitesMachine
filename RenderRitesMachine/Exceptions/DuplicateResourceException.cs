namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Thrown when attempting to add a resource with an existing name.
/// </summary>
public class DuplicateResourceException : Exception
{
    /// <summary>
    /// Name of the resource that already exists.
    /// </summary>
    public string ResourceName { get; } = string.Empty;

    /// <summary>
    /// Resource type (e.g., mesh, shader, texture, bounding box).
    /// </summary>
    public string ResourceType { get; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateResourceException"/> class.
    /// </summary>
    /// <param name="resourceType">Resource type (e.g., mesh, shader, texture, bounding box).</param>
    /// <param name="resourceName">Name of the resource that already exists.</param>
    public DuplicateResourceException(string resourceType, string resourceName)
        : base($"A {resourceType} with the name '{resourceName}' already exists.")
    {
        ResourceType = resourceType;
        ResourceName = resourceName;
    }

    /// <summary>
    /// Initializes a new instance with a custom error message.
    /// </summary>
    /// <param name="message">Error description.</param>
    public DuplicateResourceException(string message) : base(message)
    {
        ResourceType = string.Empty;
        ResourceName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance with a custom error message and inner exception.
    /// </summary>
    /// <param name="message">Error description.</param>
    /// <param name="innerException">Underlying exception.</param>
    public DuplicateResourceException(string message, Exception innerException) : base(message, innerException)
    {
        ResourceType = string.Empty;
        ResourceName = string.Empty;
    }

    public DuplicateResourceException()
    {
    }
}
