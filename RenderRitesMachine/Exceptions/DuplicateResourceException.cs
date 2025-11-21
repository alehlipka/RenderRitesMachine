namespace RenderRitesMachine.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается при попытке добавить ресурс с уже существующим именем.
/// </summary>
public class DuplicateResourceException : Exception
{
    /// <summary>
    /// Имя ресурса, которое уже существует.
    /// </summary>
    public string ResourceName { get; } = string.Empty;

    /// <summary>
    /// Тип ресурса (например, "mesh", "shader", "texture", "bounding box").
    /// </summary>
    public string ResourceType { get; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DuplicateResourceException"/>.
    /// </summary>
    /// <param name="resourceType">Тип ресурса (например, "mesh", "shader", "texture", "bounding box").</param>
    /// <param name="resourceName">Имя ресурса, которое уже существует.</param>
    public DuplicateResourceException(string resourceType, string resourceName)
        : base($"A {resourceType} with the name '{resourceName}' already exists.")
    {
        ResourceType = resourceType;
        ResourceName = resourceName;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DuplicateResourceException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public DuplicateResourceException(string message) : base(message)
    {
        ResourceType = string.Empty;
        ResourceName = string.Empty;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DuplicateResourceException"/> с указанным сообщением об ошибке и ссылкой на внутреннее исключение.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    /// <param name="innerException">Исключение, являющееся причиной текущего исключения.</param>
    public DuplicateResourceException(string message, Exception innerException) : base(message, innerException)
    {
        ResourceType = string.Empty;
        ResourceName = string.Empty;
    }

    public DuplicateResourceException()
    {
    }
}
