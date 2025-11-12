using ImGuiNET;

namespace RenderRitesMachine.UI;

/// <summary>
/// Обертка для управления выпадающим списком (Combo) ImGui.
/// </summary>
public class UICombo : IDisposable
{
    private readonly string _label;
    private readonly string _preview;
    private bool _isOpen;
    private bool _disposed;

    /// <summary>
    /// Создает новый выпадающий список.
    /// </summary>
    /// <param name="label">Название списка.</param>
    /// <param name="preview">Текущее выбранное значение для отображения.</param>
    internal UICombo(string label, string preview)
    {
        _label = label;
        _preview = preview;
    }

    /// <summary>
    /// Открыт ли список.
    /// </summary>
    public bool IsOpen => _isOpen && !_disposed;

    /// <summary>
    /// Начинает отрисовку выпадающего списка.
    /// </summary>
    /// <returns>True, если список открыт.</returns>
    public bool Begin()
    {
        if (_disposed)
        {
            return false;
        }

        _isOpen = ImGui.BeginCombo(_label, _preview);
        return _isOpen;
    }

    /// <summary>
    /// Заканчивает отрисовку выпадающего списка.
    /// </summary>
    public void End()
    {
        if (_isOpen)
        {
            ImGui.EndCombo();
        }
    }

    /// <summary>
    /// Выполняет действие внутри выпадающего списка. Автоматически управляет Begin/End.
    /// </summary>
    /// <param name="action">Действие для выполнения внутри списка.</param>
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
    /// Добавляет элемент для выбора в список.
    /// </summary>
    /// <param name="label">Название элемента.</param>
    /// <param name="isSelected">Выбран ли элемент.</param>
    /// <returns>True, если элемент был выбран.</returns>
    public bool Selectable(string label, bool isSelected = false)
    {
        if (!_isOpen)
        {
            return false;
        }

        bool clicked = ImGui.Selectable(label, isSelected);
        if (isSelected)
        {
            ImGui.SetItemDefaultFocus();
        }
        return clicked;
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

