using ImGuiNET;
using Leopotam.EcsLite;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.ECS;
using Window = RenderRitesMachine.Output.Window;

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

        Window? window = shared.Window;

        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        if (window.IsKeyPressed(Keys.F1))
        {
            shared.SceneManager.SwitchTo("demo");
        }

        if (window.IsKeyPressed(Keys.F2))
        {
            shared.SceneManager.SwitchTo("gui-test");
        }
    }
}

