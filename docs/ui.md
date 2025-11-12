# Интерфейс пользователя

## ImGui интеграция

Движок включает встроенную поддержку ImGui для создания пользовательских интерфейсов:

```csharp
// Получение контекста ImGui
IntPtr context = RenderRites.Machine.Gui.GetContext();
if (context != IntPtr.Zero)
{
    ImGui.SetCurrentContext(context);
    // Использование ImGui API
}
```

## UI обертка

Для упрощения работы с ImGui предоставлена обертка `UI`:

```csharp
using RenderRitesMachine.UI;

// Создание окна
UI.Window("Мое окно").With(() =>
{
    UI.Text("Привет, мир!");
    if (UI.Button("Нажми меня"))
    {
        // Действие
    }
});

// Чекбоксы, слайдеры и другие виджеты
bool value = false;
UI.Checkbox("Опция", ref value);

float slider = 0.5f;
UI.SliderFloat("Значение", ref slider, 0.0f, 1.0f);
```

Подробнее см. [RenderRitesMachine/UI/README.md](../RenderRitesMachine/UI/README.md)

## Работа с UI в системах

```csharp
public class GameUISystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        // Использование UI обертки
        UI.Window("Game HUD").With(() =>
        {
            UI.Text($"Health: {GetPlayerHealth()}");
            UI.Text($"Score: {GetScore()}");
            
            if (UI.Button("Pause"))
            {
                shared.SceneManager.SwitchTo("pause");
            }
        });
        
        // Или прямой доступ к ImGui
        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Game"))
                {
                    if (ImGui.MenuItem("Restart"))
                    {
                        RestartGame();
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }
    }
}
```

