using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.Demo;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesMachine;
using RenderRitesMachine.Output;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        // Добавляем сцены (логотип должен быть первым)
        RenderRites.Machine.Scenes
            .Add(new LogoScene("logo"))
            .Add(new DemoScene("demo"))
            .Add(new GuiTestScene("guitest"))
            .SetCurrent("logo"); // Начинаем со сцены с логотипом

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", VSyncMode.Off, 8);
    }
}
