using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.Demo;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        RenderRites.Machine.Scenes
            .AddScene<DemoScene>("demo")
            .AddScene<GuiTestScene>("gui-test");

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", VSyncMode.Off, 8);
    }
}
