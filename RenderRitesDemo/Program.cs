using OpenTK.Windowing.Common;
using RenderRitesDemo.Scenes.Preloader;
using RenderRitesMachine;

namespace RenderRitesDemo;

internal static class Program
{
    private static void Main()
    {
        RenderRites.Machine.Scenes.Add(new PreloaderScene("preloader")).SetCurrent("preloader");
        RenderRites.Machine.RunWindow("RenderRites Machine Demo", 0, VSyncMode.On, 8);
    }
}