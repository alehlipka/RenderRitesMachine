using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using RenderRitesMachine.Managers;
using RenderRitesMachine.Windowing;

namespace RenderRitesMachine;

public sealed class RenderRites
{
    private static readonly Lazy<RenderRites> LazyMachine = new(() => new RenderRites());
    public static RenderRites Machine => LazyMachine.Value;

    public Window? Window;
    public readonly SceneManager SceneManager;
    public readonly ObjectManager ObjectManager;
    public readonly ShaderManager ShaderManager;
    public readonly TextureManager TextureManager;
    public readonly Random Random;

    private RenderRites()
    {
        SceneManager = new SceneManager();
        ObjectManager = new ObjectManager();
        ShaderManager = new ShaderManager();
        TextureManager = new TextureManager();
        Random = new Random();
    }

    public void RunWindow(string title, double updateFrequency = 60, VSyncMode vSync = VSyncMode.Adaptive, int samples = 4)
    {
        GameWindowSettings gws = new()
        {
            UpdateFrequency = updateFrequency
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

        SceneManager.Dispose();
        ObjectManager.Dispose();
        ShaderManager.Dispose();
        TextureManager.Dispose();
    }
}