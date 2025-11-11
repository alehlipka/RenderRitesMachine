using ImGuiNET;
using Leopotam.EcsLite;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;
using RenderRitesMachine.ECS;

namespace RenderRitesDemo.ECS.Features.SceneSwitch;

/// <summary>
/// Система для переключения между сценами через клавиатуру.
/// </summary>
public class SceneSwitchSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        if (shared.Window == null) return;

        var window = shared.Window;

        // Устанавливаем контекст ImGui для проверки
        IntPtr context = RenderRites.Machine.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        // Переключение сцен через функциональные клавиши
        // Работает независимо от того, захвачен ли ввод ImGui (для переключения сцен это важно)
        if (window.IsKeyPressed(Keys.F1))
        {
            RenderRites.Machine.Scenes.SetCurrent("preloader");
        }

        if (window.IsKeyPressed(Keys.F2))
        {
            RenderRites.Machine.Scenes.SetCurrent("guitest");
        }
    }
}

