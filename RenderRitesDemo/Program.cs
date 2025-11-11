using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.Demo;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        // Добавляем пользовательские сцены
        // Сцена логотипа автоматически добавляется движком и запускается первой
        RenderRites.Machine.Scenes
            .Add(new DemoScene("demo"))
            .Add(new GuiTestScene("guitest"));

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", VSyncMode.On, 8);
    }
}
