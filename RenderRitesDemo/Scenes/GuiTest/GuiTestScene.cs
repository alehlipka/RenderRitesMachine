using RenderRitesDemo.ECS;
using RenderRitesDemo.ECS.Features.SceneSwitch;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.Scenes.GuiTest;

/// <summary>
/// Сцена для тестирования различных элементов GUI через ImGui.
/// </summary>
public class GuiTestScene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IGuiService guiService, IAudioService audioService, ISceneManager sceneManager, ILogger logger)
    : Scene(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger)
{
    protected override void OnLoad()
    {
        Camera.Position = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 10.0f);

        ResizeSystems.Add(new MainResizeSystem());
        UpdateSystems.Add(new SceneSwitchSystem());
        RenderSystems.Add(new GuiRenderSystem());
    }
}

