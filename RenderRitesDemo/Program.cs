using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesDemo.Scenes.Demo;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        // Добавляем сцены
        RenderRites.Machine.Scenes
            .Add(new DemoScene("demo"))
            .Add(new GuiTestScene("guitest"))
            .SetCurrent("demo");

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", VSyncMode.Off, 8);
    }
}
