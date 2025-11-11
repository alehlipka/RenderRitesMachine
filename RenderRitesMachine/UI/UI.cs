using ImGuiNET;
using RenderRitesMachine.Services;
using System.Numerics;

namespace RenderRitesMachine.UI;

/// <summary>
/// Главный класс-обертка над ImGui для удобного построения интерфейсов.
/// Автоматически управляет контекстом ImGui и предоставляет упрощенный API.
/// </summary>
public static class UI
{
    private static GuiService? _guiService;

    /// <summary>
    /// Инициализирует UI обертку с указанным сервисом GUI.
    /// </summary>
    /// <param name="guiService">Сервис GUI для работы с ImGui.</param>
    public static void Initialize(GuiService guiService)
    {
        _guiService = guiService;
        EnsureContext();
    }

    /// <summary>
    /// Получает сервис GUI. Используется внутренними классами обертки.
    /// </summary>
    /// <returns>Сервис GUI или null, если не инициализирован.</returns>
    internal static GuiService? GetGuiService()
    {
        return _guiService;
    }

    /// <summary>
    /// Убеждается, что контекст ImGui установлен.
    /// </summary>
    private static void EnsureContext()
    {
        if (_guiService == null)
        {
            return;
        }

        IntPtr context = _guiService.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }
    }

    /// <summary>
    /// Выполняет действие с гарантией установленного контекста ImGui.
    /// </summary>
    /// <param name="action">Действие для выполнения.</param>
    public static void WithContext(Action action)
    {
        EnsureContext();
        action();
    }

    /// <summary>
    /// Создает новое окно с указанным названием.
    /// </summary>
    /// <param name="title">Название окна.</param>
    /// <param name="flags">Флаги окна.</param>
    /// <returns>Объект для управления окном.</returns>
    public static UIWindow Window(string title, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        EnsureContext();
        return new UIWindow(title, flags);
    }

    /// <summary>
    /// Создает главное меню.
    /// </summary>
    /// <returns>Объект для управления главным меню.</returns>
    public static UIMenu MainMenu()
    {
        EnsureContext();
        return new UIMenu(true);
    }

    /// <summary>
    /// Создает обычное меню.
    /// </summary>
    /// <param name="label">Название меню.</param>
    /// <returns>Объект для управления меню.</returns>
    public static UIMenu Menu(string label)
    {
        EnsureContext();
        return new UIMenu(false, label);
    }

    /// <summary>
    /// Создает кнопку.
    /// </summary>
    /// <param name="label">Текст кнопки.</param>
    /// <param name="size">Размер кнопки. Если null, используется автоматический размер.</param>
    /// <returns>True, если кнопка была нажата.</returns>
    public static bool Button(string label, Vector2? size = null)
    {
        EnsureContext();
        return size.HasValue ? ImGui.Button(label, size.Value) : ImGui.Button(label);
    }

    /// <summary>
    /// Создает маленькую кнопку.
    /// </summary>
    /// <param name="label">Текст кнопки.</param>
    /// <returns>True, если кнопка была нажата.</returns>
    public static bool SmallButton(string label)
    {
        EnsureContext();
        return ImGui.SmallButton(label);
    }

    /// <summary>
    /// Выводит текст.
    /// </summary>
    /// <param name="text">Текст для вывода.</param>
    public static void Text(string text)
    {
        EnsureContext();
        ImGui.Text(text);
    }

    /// <summary>
    /// Выводит цветной текст.
    /// </summary>
    /// <param name="text">Текст для вывода.</param>
    /// <param name="color">Цвет текста (RGBA).</param>
    public static void TextColored(string text, Vector4 color)
    {
        EnsureContext();
        ImGui.TextColored(color, text);
    }

    /// <summary>
    /// Выводит текст с автоматическим переносом.
    /// </summary>
    /// <param name="text">Текст для вывода.</param>
    public static void TextWrapped(string text)
    {
        EnsureContext();
        ImGui.TextWrapped(text);
    }

    /// <summary>
    /// Выводит отключенный (серый) текст.
    /// </summary>
    /// <param name="text">Текст для вывода.</param>
    public static void TextDisabled(string text)
    {
        EnsureContext();
        ImGui.TextDisabled(text);
    }

    /// <summary>
    /// Создает чекбокс.
    /// </summary>
    /// <param name="label">Название чекбокса.</param>
    /// <param name="value">Текущее значение чекбокса.</param>
    /// <returns>True, если значение изменилось.</returns>
    public static bool Checkbox(string label, ref bool value)
    {
        EnsureContext();
        return ImGui.Checkbox(label, ref value);
    }

    /// <summary>
    /// Создает радиокнопку.
    /// </summary>
    /// <param name="label">Название радиокнопки.</param>
    /// <param name="active">Активна ли радиокнопка.</param>
    /// <returns>True, если радиокнопка была выбрана.</returns>
    public static bool RadioButton(string label, bool active)
    {
        EnsureContext();
        return ImGui.RadioButton(label, active);
    }

    /// <summary>
    /// Создает текстовое поле ввода.
    /// </summary>
    /// <param name="label">Название поля.</param>
    /// <param name="value">Текущее значение.</param>
    /// <param name="maxLength">Максимальная длина строки.</param>
    /// <returns>True, если значение изменилось.</returns>
    public static bool InputText(string label, ref string value, uint maxLength = 256)
    {
        EnsureContext();
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
        Array.Resize(ref buffer, (int)maxLength);
        
        bool changed = ImGui.InputText(label, buffer, maxLength);
        if (changed)
        {
            value = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }
        
        return changed;
    }

    /// <summary>
    /// Создает слайдер для float значения.
    /// </summary>
    /// <param name="label">Название слайдера.</param>
    /// <param name="value">Текущее значение.</param>
    /// <param name="min">Минимальное значение.</param>
    /// <param name="max">Максимальное значение.</param>
    /// <returns>True, если значение изменилось.</returns>
    public static bool SliderFloat(string label, ref float value, float min, float max)
    {
        EnsureContext();
        return ImGui.SliderFloat(label, ref value, min, max);
    }

    /// <summary>
    /// Создает слайдер для int значения.
    /// </summary>
    /// <param name="label">Название слайдера.</param>
    /// <param name="value">Текущее значение.</param>
    /// <param name="min">Минимальное значение.</param>
    /// <param name="max">Максимальное значение.</param>
    /// <returns>True, если значение изменилось.</returns>
    public static bool SliderInt(string label, ref int value, int min, int max)
    {
        EnsureContext();
        return ImGui.SliderInt(label, ref value, min, max);
    }

    /// <summary>
    /// Создает поле для перетаскивания float значения.
    /// </summary>
    /// <param name="label">Название поля.</param>
    /// <param name="value">Текущее значение.</param>
    /// <param name="speed">Скорость изменения.</param>
    /// <param name="min">Минимальное значение.</param>
    /// <param name="max">Максимальное значение.</param>
    /// <returns>True, если значение изменилось.</returns>
    public static bool DragFloat(string label, ref float value, float speed = 1.0f, float min = 0.0f, float max = 0.0f)
    {
        EnsureContext();
        return ImGui.DragFloat(label, ref value, speed, min, max);
    }

    /// <summary>
    /// Создает поле для ввода int значения.
    /// </summary>
    /// <param name="label">Название поля.</param>
    /// <param name="value">Текущее значение.</param>
    /// <returns>True, если значение изменилось.</returns>
    public static bool InputInt(string label, ref int value)
    {
        EnsureContext();
        return ImGui.InputInt(label, ref value);
    }

    /// <summary>
    /// Создает разделитель.
    /// </summary>
    public static void Separator()
    {
        EnsureContext();
        ImGui.Separator();
    }

    /// <summary>
    /// Добавляет отступ.
    /// </summary>
    public static void Spacing()
    {
        EnsureContext();
        ImGui.Spacing();
    }

    /// <summary>
    /// Размещает следующий элемент на той же строке.
    /// </summary>
    /// <param name="offsetFromStartX">Смещение от начала строки.</param>
    /// <param name="spacing">Расстояние между элементами.</param>
    public static void SameLine(float offsetFromStartX = 0.0f, float spacing = -1.0f)
    {
        EnsureContext();
        ImGui.SameLine(offsetFromStartX, spacing);
    }

    /// <summary>
    /// Создает элемент списка с маркером.
    /// </summary>
    /// <param name="text">Текст элемента.</param>
    public static void BulletText(string text)
    {
        EnsureContext();
        ImGui.BulletText(text);
    }

    /// <summary>
    /// Начинает группу вкладок.
    /// </summary>
    /// <param name="label">Название группы вкладок.</param>
    /// <returns>True, если группа вкладок открыта.</returns>
    public static bool BeginTabBar(string label)
    {
        EnsureContext();
        return ImGui.BeginTabBar(label);
    }

    /// <summary>
    /// Заканчивает группу вкладок.
    /// </summary>
    public static void EndTabBar()
    {
        EnsureContext();
        ImGui.EndTabBar();
    }

    /// <summary>
    /// Начинает вкладку.
    /// </summary>
    /// <param name="label">Название вкладки.</param>
    /// <returns>True, если вкладка открыта.</returns>
    public static bool BeginTabItem(string label)
    {
        EnsureContext();
        return ImGui.BeginTabItem(label);
    }

    /// <summary>
    /// Заканчивает вкладку.
    /// </summary>
    public static void EndTabItem()
    {
        EnsureContext();
        ImGui.EndTabItem();
    }

    /// <summary>
    /// Создает выпадающий список (Combo).
    /// </summary>
    /// <param name="label">Название списка.</param>
    /// <param name="preview">Текущее выбранное значение для отображения.</param>
    /// <returns>Объект для управления выпадающим списком.</returns>
    public static UICombo Combo(string label, string preview)
    {
        EnsureContext();
        return new UICombo(label, preview);
    }

    /// <summary>
    /// Создает список выбора (ListBox).
    /// </summary>
    /// <param name="label">Название списка.</param>
    /// <param name="size">Размер списка.</param>
    /// <returns>Объект для управления списком.</returns>
    public static UIListBox ListBox(string label, Vector2? size = null)
    {
        EnsureContext();
        return new UIListBox(label, size);
    }

    /// <summary>
    /// Создает узел дерева.
    /// </summary>
    /// <param name="label">Название узла.</param>
    /// <returns>True, если узел открыт.</returns>
    public static bool TreeNode(string label)
    {
        EnsureContext();
        return ImGui.TreeNode(label);
    }

    /// <summary>
    /// Закрывает узел дерева.
    /// </summary>
    public static void TreePop()
    {
        EnsureContext();
        ImGui.TreePop();
    }

    /// <summary>
    /// Создает редактор цвета (RGB).
    /// </summary>
    /// <param name="label">Название редактора.</param>
    /// <param name="color">Текущий цвет (RGB).</param>
    /// <returns>True, если цвет изменился.</returns>
    public static bool ColorEdit3(string label, ref Vector3 color)
    {
        EnsureContext();
        return ImGui.ColorEdit3(label, ref color);
    }

    /// <summary>
    /// Создает редактор цвета (RGBA).
    /// </summary>
    /// <param name="label">Название редактора.</param>
    /// <param name="color">Текущий цвет (RGBA).</param>
    /// <returns>True, если цвет изменился.</returns>
    public static bool ColorEdit4(string label, ref Vector4 color)
    {
        EnsureContext();
        return ImGui.ColorEdit4(label, ref color);
    }

    /// <summary>
    /// Создает кнопку цвета.
    /// </summary>
    /// <param name="label">Название кнопки.</param>
    /// <param name="color">Цвет кнопки.</param>
    /// <param name="flags">Флаги редактора цвета.</param>
    /// <param name="size">Размер кнопки.</param>
    /// <returns>True, если кнопка была нажата.</returns>
    public static bool ColorButton(string label, Vector4 color, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None, Vector2? size = null)
    {
        EnsureContext();
        return size.HasValue 
            ? ImGui.ColorButton(label, color, flags, size.Value) 
            : ImGui.ColorButton(label, color, flags);
    }

    /// <summary>
    /// Показывает демо-окно ImGui.
    /// </summary>
    /// <param name="open">Открыто ли окно.</param>
    public static void ShowDemoWindow(ref bool open)
    {
        EnsureContext();
        ImGui.ShowDemoWindow(ref open);
    }

    /// <summary>
    /// Показывает редактор стилей ImGui.
    /// </summary>
    public static void ShowStyleEditor()
    {
        EnsureContext();
        ImGui.ShowStyleEditor();
    }

    /// <summary>
    /// Добавляет пункт меню в текущее открытое меню.
    /// </summary>
    /// <param name="label">Название пункта.</param>
    /// <param name="shortcut">Горячая клавиша (опционально).</param>
    /// <param name="selected">Выбран ли пункт (для чекбоксов).</param>
    /// <param name="enabled">Включен ли пункт.</param>
    /// <returns>True, если пункт был выбран.</returns>
    public static bool MenuItem(string label, string? shortcut = null, bool selected = false, bool enabled = true)
    {
        EnsureContext();
        return ImGui.MenuItem(label, shortcut, selected, enabled);
    }
}

