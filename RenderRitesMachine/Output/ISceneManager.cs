namespace RenderRitesMachine.Output;

/// <summary>
/// Scene manager interface that controls application scenes.
/// </summary>
public interface ISceneManager
{
    /// <summary>
    /// Currently active scene. May be null when none is set.
    /// </summary>
    Scene? Current { get; }

    /// <summary>
    /// Switches the active scene by name.
    /// </summary>
    /// <param name="name">Name of the scene to activate.</param>
    void SwitchTo(string name);

    /// <summary>
    /// Projects each scene into a custom result using the provided selector.
    /// </summary>
    /// <typeparam name="TResult">Projection result type.</typeparam>
    /// <param name="selector">Function that converts a scene into a result.</param>
    /// <returns>Sequence of projection results.</returns>
    IEnumerable<TResult> Select<TResult>(Func<Scene, TResult> selector);
}
