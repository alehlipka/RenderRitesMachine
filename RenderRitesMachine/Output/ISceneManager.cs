namespace RenderRitesMachine.Output;

/// <summary>
/// Интерфейс для менеджера сцен, предоставляющего управление сценами приложения.
/// </summary>
public interface ISceneManager
{
    /// <summary>
    /// Текущая активная сцена. Может быть null, если сцена не установлена.
    /// </summary>
    Scene? Current { get; }

    /// <summary>
    /// Переключает текущую активную сцену на указанную по имени.
    /// </summary>
    /// <param name="name">Имя сцены для переключения.</param>
    void SwitchTo(string name);

    /// <summary>
    /// Проецирует каждую сцену в результат с помощью указанной функции.
    /// </summary>
    /// <typeparam name="TResult">Тип результата проекции.</typeparam>
    /// <param name="selector">Функция для преобразования сцены в результат.</param>
    /// <returns>Последовательность результатов проекции.</returns>
    IEnumerable<TResult> Select<TResult>(Func<Scene, TResult> selector);
}
