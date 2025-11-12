namespace RenderRitesMachine.Services;

/// <summary>
/// Интерфейс для сервиса GUI, предоставляющего доступ к ImGui контексту.
/// </summary>
public interface IGuiService
{
    /// <summary>
    /// Получает контекст ImGui для использования в других частях приложения.
    /// </summary>
    /// <returns>Контекст ImGui или IntPtr.Zero, если не инициализирован.</returns>
    IntPtr GetContext();
}

