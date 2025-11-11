using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

/// <summary>
/// Фабрика для создания сцен с правильными зависимостями.
/// </summary>
public interface ISceneFactory
{
    /// <summary>
    /// Создает сцену указанного типа с именем.
    /// </summary>
    T CreateScene<T>(string name) where T : Scene;
}

