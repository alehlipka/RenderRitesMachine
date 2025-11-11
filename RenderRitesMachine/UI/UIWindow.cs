using System.Numerics;
using ImGuiNET;

namespace RenderRitesMachine.UI;

/// <summary>
/// Обертка для управления окном ImGui.
/// Автоматически управляет жизненным циклом окна.
/// </summary>
public class UIWindow : IDisposable
{
    private readonly string _title;
    private readonly ImGuiWindowFlags _flags;
    private bool _isOpen;
    private bool _disposed;

    /// <summary>
    /// Создает новое окно.
    /// </summary>
    /// <param name="title">Название окна.</param>
    /// <param name="flags">Флаги окна.</param>
    internal UIWindow(string title, ImGuiWindowFlags flags)
    {
        _title = title;
        _flags = flags;
        _isOpen = true;
    }

    /// <summary>
    /// Открыто ли окно.
    /// </summary>
    public bool IsOpen => _isOpen && !_disposed;

    /// <summary>
    /// Начинает отрисовку окна. Возвращает true, если окно открыто и готово к отрисовке.
    /// </summary>
    /// <returns>True, если окно открыто.</returns>
    public bool Begin()
    {
        if (_disposed)
        {
            return false;
        }

        _isOpen = ImGui.Begin(_title, ref _isOpen, _flags);
        return _isOpen;
    }

    /// <summary>
    /// Начинает отрисовку окна с возможностью закрытия.
    /// </summary>
    /// <param name="open">Ссылка на переменную, указывающую, открыто ли окно.</param>
    /// <returns>True, если окно открыто.</returns>
    public bool Begin(ref bool open)
    {
        if (_disposed)
        {
            open = false;
            return false;
        }

        _isOpen = ImGui.Begin(_title, ref open, _flags);
        if (!_isOpen)
        {
            open = false;
        }
        return _isOpen;
    }

    /// <summary>
    /// Заканчивает отрисовку окна.
    /// </summary>
    public void End()
    {
        if (_isOpen)
        {
            ImGui.End();
        }
    }

    /// <summary>
    /// Выполняет действие внутри окна. Автоматически управляет Begin/End.
    /// </summary>
    /// <param name="action">Действие для выполнения внутри окна.</param>
    public void With(Action action)
    {
        if (Begin())
        {
            try
            {
                action();
            }
            finally
            {
                End();
            }
        }
    }

    /// <summary>
    /// Выполняет действие внутри окна с возможностью закрытия.
    /// </summary>
    /// <param name="open">Ссылка на переменную, указывающую, открыто ли окно.</param>
    /// <param name="action">Действие для выполнения внутри окна.</param>
    public void With(ref bool open, Action action)
    {
        if (Begin(ref open))
        {
            try
            {
                action();
            }
            finally
            {
                End();
            }
        }
    }

    /// <summary>
    /// Закрывает окно.
    /// </summary>
    public void Close()
    {
        _isOpen = false;
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

