using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine;

public sealed class RenderRites
{
    private static readonly Lazy<RenderRites> LazyMachine = new(() => new RenderRites());
    public static RenderRites Machine => LazyMachine.Value;

    public Window? Window;
    public readonly SceneManager Scenes;
    public readonly AssetsService Assets;
    public readonly RenderService Renderer;

    private RenderRites()
    {
        Scenes = new SceneManager();
        Assets = new AssetsService();
        Renderer = new RenderService();
    }

    public void RunWindow(string title, VSyncMode vSync = VSyncMode.Adaptive, int samples = 4)
    {
        GameWindowSettings gws = new()
        {
            UpdateFrequency = 0.0
        };

        NativeWindowSettings nws = new()
        {
            Title = title,
            ClientSize = new Vector2i(1024, 768),
            Profile = ContextProfile.Core,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6, 0),
            Vsync = vSync,
            NumberOfSamples = samples,
            IsEventDriven = false,
            MinimumClientSize = new Vector2i(800, 600)
        };

        #if DEBUG
        nws.Flags |= ContextFlags.Debug;
        #endif
        
        Window = new Window(gws, nws);
        Window.Run();

        Renderer.Dispose();
        Assets.Dispose();
        Scenes.Dispose();
    }
}