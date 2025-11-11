using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesDemo.Scenes.Preloader;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        // Добавляем обе сцены
        RenderRites.Machine.Scenes
            .Add(new PreloaderScene("preloader"))
            .Add(new GuiTestScene("guitest"))
            .SetCurrent("preloader"); // Начинаем с главной сцены

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", VSyncMode.Off, 8);
    }
}
