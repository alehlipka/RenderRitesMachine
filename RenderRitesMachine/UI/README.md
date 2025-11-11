# UI Обертка над ImGui

Обертка над ImGui для удобного построения интерфейсов в RenderRites Machine.

## Основные возможности

- Автоматическое управление контекстом ImGui
- Упрощенный API для создания окон, меню и виджетов
- Автоматическое управление жизненным циклом UI элементов
- Поддержка using-паттерна для автоматической очистки ресурсов

## Инициализация

Обертка автоматически инициализируется в `Window.OnLoad()`. Дополнительная инициализация не требуется.

## Примеры использования

### Окна

```csharp
// Простое окно
UI.Window("Название окна").With(() =>
{
    UI.Text("Содержимое окна");
    if (UI.Button("Кнопка"))
    {
        // Действие
    }
});

// Окно с возможностью закрытия
bool isOpen = true;
UI.Window("Окно").With(ref isOpen, () =>
{
    UI.Text("Это окно можно закрыть");
});
```

### Меню

**Примечание:** Для меню рекомендуется использовать прямой вызов `ImGui`, так как обертка может вызывать проблемы с контекстом. Обертка больше подходит для окон.

```csharp
// Главное меню - прямой вызов ImGui (рекомендуется)
IntPtr context = RenderRites.Machine.Gui.GetContext();
if (context != IntPtr.Zero)
{
    ImGui.SetCurrentContext(context);
}

if (ImGui.BeginMainMenuBar())
{
    if (ImGui.BeginMenu("Файл"))
    {
        if (ImGui.MenuItem("Открыть", "Ctrl+O"))
        {
            // Действие
        }
        if (ImGui.MenuItem("Сохранить", "Ctrl+S"))
        {
            // Действие
        }
        ImGui.Separator();
        if (ImGui.MenuItem("Выход"))
        {
            // Действие
        }
        ImGui.EndMenu();
    }
    ImGui.EndMainMenuBar();
}
```

### Виджеты

```csharp
// Кнопки
if (UI.Button("Нажми меня"))
{
    // Действие
}

// Текст
UI.Text("Обычный текст");
UI.TextColored("Цветной текст", new Vector4(1, 0, 0, 1));
UI.TextWrapped("Длинный текст с переносом");

// Чекбоксы
bool value = false;
UI.Checkbox("Включить опцию", ref value);

// Слайдеры
float sliderValue = 0.5f;
UI.SliderFloat("Значение", ref sliderValue, 0.0f, 1.0f);

// Текстовые поля
string text = "";
UI.InputText("Имя", ref text);
```

### Вкладки

```csharp
if (UI.BeginTabBar("Вкладки"))
{
    if (UI.BeginTabItem("Вкладка 1"))
    {
        UI.Text("Содержимое вкладки 1");
        UI.EndTabItem();
    }
    
    if (UI.BeginTabItem("Вкладка 2"))
    {
        UI.Text("Содержимое вкладки 2");
        UI.EndTabItem();
    }
    
    UI.EndTabBar();
}
```

### Выпадающие списки

```csharp
using var combo = UI.Combo("Выберите", selectedItem);
combo.With(() =>
{
    for (int i = 0; i < items.Length; i++)
    {
        if (combo.Selectable(items[i], i == selectedIndex))
        {
            selectedIndex = i;
        }
    }
});
```

## Преимущества перед прямым использованием ImGui

1. **Автоматическое управление контекстом** - не нужно вручную устанавливать контекст перед каждым вызовом (для окон)
2. **Упрощенный синтаксис** - меньше boilerplate кода для окон
3. **Безопасность ресурсов** - автоматическое управление Begin/End через using и методы With() для окон
4. **Единообразный API** - все методы доступны через статический класс UI
5. **Улучшенная читаемость** - код становится более понятным и структурированным

**Примечание:** Для меню рекомендуется использовать прямой вызов `ImGui`, так как обертка может вызывать проблемы с контекстом. Обертка лучше всего работает для окон и виджетов.

## Миграция с прямого использования ImGui

### Было:
```csharp
IntPtr context = RenderRites.Machine.Gui.GetContext();
if (context != IntPtr.Zero)
{
    ImGui.SetCurrentContext(context);
}

if (ImGui.Begin("Окно"))
{
    ImGui.Text("Текст");
    if (ImGui.Button("Кнопка"))
    {
        // Действие
    }
    ImGui.End();
}
```

### Стало:
```csharp
UI.Window("Окно").With(() =>
{
    UI.Text("Текст");
    if (UI.Button("Кнопка"))
    {
        // Действие
    }
});
```

