using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.Demo;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        // Добавляем пользовательские сцены через фабрику
        // Сцена логотипа автоматически добавляется движком и запускается первой
        RenderRites.Machine.Scenes
            .AddScene<DemoScene>("demo")
            .AddScene<GuiTestScene>("guitest");

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", VSyncMode.Off, 8);
    }
}
