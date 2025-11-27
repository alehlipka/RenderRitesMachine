namespace RenderRitesMachine.Output;

/// <summary>
/// Factory responsible for creating fully wired scenes.
/// </summary>
public interface ISceneFactory
{
    /// <summary>
    /// Creates a scene of the specified type.
    /// </summary>
    T CreateScene<T>(string name) where T : Scene;
}
