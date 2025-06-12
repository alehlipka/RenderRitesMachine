using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        RenderRites.Machine.SceneManager.AddMany([
            new PreloaderScene("preloader"),
            new MainScene("main")
        ]).SetCurrent("preloader");

        RenderRites.Machine.RunWindow("RenderRites Machine Demo", 0, VSyncMode.On, 8);
    }
}