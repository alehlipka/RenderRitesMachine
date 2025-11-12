using ImGuiNET;
using Leopotam.EcsLite;
using RenderRitesMachine.Debug;
using RenderRitesMachine.ECS;
using RenderRitesMachine.UI;

namespace RenderRitesDemo.ECS;

/// <summary>
/// Система для рендеринга GUI через ImGui с использованием обертки UI.
/// </summary>
public class GuiRenderSystem : IEcsRunSystem
{
    private bool _showMetricsWindow;

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Сцены"))
            {
                string currentScene = shared.SceneManager.Current?.Name ?? "";
                bool isDemo = currentScene == "demo";
                bool isGuiTest = currentScene == "gui-test";

                if (ImGui.MenuItem("Главная сцена", "F1", isDemo))
                {
                    shared.SceneManager.SwitchTo("demo");
                }
                if (ImGui.MenuItem("GUI Тест", "F2", isGuiTest))
                {
                    shared.SceneManager.SwitchTo("gui-test");
                }
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        UI.Window("Метрики").With(ref _showMetricsWindow, () =>
        {
            UI.Text($"FPS: {FpsCounter.GetFps():F2}");
            UI.Text($"Время кадра: {1000.0 / FpsCounter.GetFps():F2} мс");

            UI.Separator();
            UI.Text("Статистика рендеринга:");
            RenderStatistics stats = shared.RenderStats;
            UI.Text($"Всего объектов: {stats.TotalObjects}");
            UI.Text($"Отрендерено: {stats.RenderedObjects}");
            UI.Text($"Отсечено (Frustum Culling): {stats.CulledObjects}");
            UI.Text($"Процент отсечения: {stats.CullingPercentage:F1}%");

            UI.Separator();
            bool enableFrustumCulling = shared.EnableFrustumCulling;
            if (UI.Checkbox("Включить Frustum Culling", ref enableFrustumCulling))
            {
                shared.EnableFrustumCulling = enableFrustumCulling;
            }

            UI.Separator();
            UI.Text("Статистика ImGui:");
            UI.Text($"Активных окон: {ImGui.GetIO().MetricsRenderWindows}");
            UI.Text($"Активных виджетов: {ImGui.GetIO().MetricsActiveWindows}");
        });
    }
}

