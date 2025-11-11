using ImGuiNET;
using System.Numerics;

namespace RenderRitesMachine.UI;

/// <summary>
/// Обертка для управления списком выбора (ListBox) ImGui.
/// </summary>
public class UIListBox : IDisposable
{
    private readonly string _label;
    private readonly Vector2? _size;
    private bool _isOpen;
    private bool _disposed;

    /// <summary>
    /// Создает новый список выбора.
    /// </summary>
    /// <param name="label">Название списка.</param>
    /// <param name="size">Размер списка. Если null, используется автоматический размер.</param>
    internal UIListBox(string label, Vector2? size = null)
    {
        _label = label;
        _size = size;
    }

    /// <summary>
    /// Открыт ли список.
    /// </summary>
    public bool IsOpen => _isOpen && !_disposed;

    /// <summary>
    /// Начинает отрисовку списка выбора.
    /// </summary>
    /// <returns>True, если список открыт.</returns>
    public bool Begin()
    {
        if (_disposed)
        {
            return false;
        }

        _isOpen = _size.HasValue 
            ? ImGui.BeginListBox(_label, _size.Value) 
            : ImGui.BeginListBox(_label);
        return _isOpen;
    }

    /// <summary>
    /// Заканчивает отрисовку списка выбора.
    /// </summary>
    public void End()
    {
        if (_isOpen)
        {
            ImGui.EndListBox();
        }
    }

    /// <summary>
    /// Выполняет действие внутри списка выбора. Автоматически управляет Begin/End.
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

