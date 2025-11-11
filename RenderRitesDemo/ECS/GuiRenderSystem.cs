using ImGuiNET;
using Leopotam.EcsLite;
using RenderRitesMachine;
using RenderRitesMachine.Debug;
using RenderRitesMachine.UI;

namespace RenderRitesDemo.ECS;

/// <summary>
/// Система для рендеринга GUI через ImGui с использованием обертки UI.
/// </summary>
public class GuiRenderSystem : IEcsRunSystem
{
    private bool _showDemoWindow = false;
    private bool _showMetricsWindow = false;
    private bool _showAboutWindow = false;

    public void Run(IEcsSystems systems)
    {
        // Убеждаемся, что контекст ImGui установлен
        IntPtr context = RenderRites.Machine.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        // Главное меню - используем прямой вызов ImGui для надежности
        // Обертка для меню вызывает проблемы с контекстом, поэтому используем прямой вызов
        if (ImGui.BeginMainMenuBar())
        {
            // Подменю "Сцены"
            if (ImGui.BeginMenu("Сцены"))
            {
                string currentScene = RenderRites.Machine.Scenes.Current?.Name ?? "";
                bool isDemo = currentScene == "demo";
                bool isGuiTest = currentScene == "guitest";

                if (ImGui.MenuItem("Главная сцена", "F1", isDemo))
                {
                    RenderRites.Machine.Scenes.SwitchTo("demo");
                }
                if (ImGui.MenuItem("GUI Тест", "F2", isGuiTest))
                {
                    RenderRites.Machine.Scenes.SwitchTo("guitest");
                }
                ImGui.EndMenu();
            }

            // Подменю "Окна"
            if (ImGui.BeginMenu("Окна"))
            {
                if (ImGui.MenuItem("Демо окно", "Ctrl+D", _showDemoWindow))
                {
                    _showDemoWindow = !_showDemoWindow;
                }
                if (ImGui.MenuItem("Метрики", "Ctrl+M", _showMetricsWindow))
                {
                    _showMetricsWindow = !_showMetricsWindow;
                }
                ImGui.Separator();
                if (ImGui.MenuItem("О программе"))
                {
                    _showAboutWindow = true;
                }
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        // Демо окно ImGui
        if (_showDemoWindow)
        {
            UI.ShowDemoWindow(ref _showDemoWindow);
        }

        // Окно метрик
        UI.Window("Метрики").With(ref _showMetricsWindow, () =>
        {
            UI.Text($"FPS: {FpsCounter.GetFps():F2}");
            UI.Text($"Время кадра: {1000.0 / FpsCounter.GetFps():F2} мс");

            UI.Separator();
            UI.Text("Статистика ImGui:");
            UI.Text($"Активных окон: {ImGuiNET.ImGui.GetIO().MetricsRenderWindows}");
            UI.Text($"Активных виджетов: {ImGuiNET.ImGui.GetIO().MetricsActiveWindows}");
        });

        // Окно "О программе"
        UI.Window("О программе").With(ref _showAboutWindow, () =>
        {
            UI.Text("RenderRites Machine Demo");
            UI.Separator();
            UI.Text("Движок рендеринга на базе OpenTK и ECS");
            UI.Text("Версия: 1.0.0");
            UI.Separator();
            if (UI.Button("Закрыть"))
            {
                _showAboutWindow = false;
            }
        });

        // Информационное окно
        UI.Window("Информация о сцене").With(() =>
        {
            UI.Text("Добро пожаловать в RenderRites!");
            UI.Separator();
            UI.Text("Переключение сцен:");
            UI.BulletText("F1 - Главная сцена");
            UI.BulletText("F2 - GUI Тест");
            UI.Separator();
            UI.Text("Управление:");
            UI.BulletText("WASD - перемещение камеры");
            UI.BulletText("Мышь - поворот камеры");
            UI.BulletText("ESC - выход");
            UI.Separator();
            UI.Text("Нажмите 'Окна' в меню для открытия дополнительных окон.");
        });
    }
}

