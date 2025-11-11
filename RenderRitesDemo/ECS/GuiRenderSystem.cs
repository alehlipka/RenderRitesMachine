using ImGuiNET;
using Leopotam.EcsLite;
using RenderRitesMachine;
using RenderRitesMachine.Debug;

namespace RenderRitesDemo.ECS;

/// <summary>
/// Система для рендеринга GUI через ImGui.
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
        // Главное меню
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Сцены"))
            {
                string currentScene = RenderRites.Machine.Scenes.Current?.Name ?? "";
                bool isPreloader = currentScene == "preloader";
                bool isGuiTest = currentScene == "guitest";
                
                if (ImGui.MenuItem("Главная сцена", "F1", isPreloader))
                {
                    RenderRites.Machine.Scenes.SetCurrent("preloader");
                }
                if (ImGui.MenuItem("GUI Тест", "F2", isGuiTest))
                {
                    RenderRites.Machine.Scenes.SetCurrent("guitest");
                }
                ImGui.EndMenu();
            }
            
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
            ImGui.ShowDemoWindow(ref _showDemoWindow);
        }
        
        // Окно метрик
        if (_showMetricsWindow)
        {
            if (ImGui.Begin("Метрики", ref _showMetricsWindow))
            {
                ImGui.Text($"FPS: {FpsCounter.GetFps():F2}");
                ImGui.Text($"Время кадра: {1000.0 / FpsCounter.GetFps():F2} мс");
                
                ImGui.Separator();
                ImGui.Text("Статистика ImGui:");
                ImGui.Text($"Активных окон: {ImGui.GetIO().MetricsRenderWindows}");
                ImGui.Text($"Активных виджетов: {ImGui.GetIO().MetricsActiveWindows}");
            }
            ImGui.End();
        }
        
        // Окно "О программе"
        if (_showAboutWindow)
        {
            if (ImGui.Begin("О программе", ref _showAboutWindow))
            {
                ImGui.Text("RenderRites Machine Demo");
                ImGui.Separator();
                ImGui.Text("Движок рендеринга на базе OpenTK и ECS");
                ImGui.Text("Версия: 1.0.0");
                ImGui.Separator();
                if (ImGui.Button("Закрыть"))
                {
                    _showAboutWindow = false;
                }
            }
            ImGui.End();
        }
        
        // Пример информационного окна
        if (ImGui.Begin("Информация о сцене"))
        {
            ImGui.Text("Добро пожаловать в RenderRites!");
            ImGui.Separator();
            ImGui.Text("Переключение сцен:");
            ImGui.BulletText("F1 - Главная сцена");
            ImGui.BulletText("F2 - GUI Тест");
            ImGui.Separator();
            ImGui.Text("Управление:");
            ImGui.BulletText("WASD - перемещение камеры");
            ImGui.BulletText("Мышь - поворот камеры");
            ImGui.BulletText("ESC - выход");
            ImGui.Separator();
            ImGui.Text("Нажмите 'Окна' в меню для открытия дополнительных окон.");
        }
        ImGui.End();
    }
}

