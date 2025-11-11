using ImGuiNET;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.UI;

/// <summary>
/// Обертка для управления меню ImGui.
/// Автоматически управляет жизненным циклом меню.
/// </summary>
public class UIMenu : IDisposable
{
    private readonly bool _isMainMenu;
    private readonly string? _label;
    private bool _isOpen;
    private bool _disposed;

    /// <summary>
    /// Создает новое меню.
    /// </summary>
    /// <param name="isMainMenu">Является ли это главным меню.</param>
    /// <param name="label">Название меню (не используется для главного меню).</param>
    internal UIMenu(bool isMainMenu, string? label = null)
    {
        _isMainMenu = isMainMenu;
        _label = label;
    }

    /// <summary>
    /// Открыто ли меню.
    /// </summary>
    public bool IsOpen => _isOpen && !_disposed;

    /// <summary>
    /// Начинает отрисовку меню. Возвращает true, если меню открыто.
    /// </summary>
    /// <returns>True, если меню открыто.</returns>
    public bool Begin()
    {
        if (_disposed)
        {
            return false;
        }

        EnsureContext();

        if (_isMainMenu)
        {
            _isOpen = ImGui.BeginMainMenuBar();
        }
        else if (_label != null)
        {
            _isOpen = ImGui.BeginMenu(_label);
        }
        else
        {
            _isOpen = false;
        }

        return _isOpen;
    }

    /// <summary>
    /// Убеждается, что контекст ImGui установлен.
    /// </summary>
    private static void EnsureContext()
    {
        // Получаем GuiService через UI класс
        var guiService = UI.GetGuiService();
        if (guiService == null)
        {
            return;
        }

        IntPtr context = guiService.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }
    }

    /// <summary>
    /// Заканчивает отрисовку меню.
    /// </summary>
    public void End()
    {
        if (_isOpen)
        {
            if (_isMainMenu)
            {
                ImGui.EndMainMenuBar();
            }
            else
            {
                ImGui.EndMenu();
            }
        }
    }

    /// <summary>
    /// Выполняет действие внутри меню. Автоматически управляет Begin/End.
    /// </summary>
    /// <param name="action">Действие для выполнения внутри меню.</param>
    public void With(Action action)
    {
        EnsureContext();

        if (Begin())
        {
            try
            {
                // Контекст уже установлен в Begin(), но убеждаемся что он сохраняется
                action();
            }
            finally
            {
                End();
            }
        }
    }

    /// <summary>
    /// Добавляет подменю.
    /// Внутри action используйте прямой вызов ImGui.MenuItem() и ImGui.Separator().
    /// </summary>
    /// <param name="label">Название подменю.</param>
    /// <param name="action">Действие для выполнения внутри подменю. Используйте ImGui.MenuItem() и ImGui.Separator() внутри.</param>
    public void SubMenu(string label, Action action)
    {
        if (!_isOpen)
        {
            return;
        }

        // Контекст уже установлен в Begin(), не нужно переустанавливать
        // Используем прямой вызов ImGui, как в рабочем варианте
        if (ImGui.BeginMenu(label))
        {
            try
            {
                action();
            }
            finally
            {
                ImGui.EndMenu();
            }
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_isOpen)
            {
                End();
            }
            _disposed = true;
        }
    }
}

