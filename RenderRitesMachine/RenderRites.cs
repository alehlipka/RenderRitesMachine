using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using RenderRitesMachine.Output;
using StbImageSharp;

namespace RenderRitesMachine;

public sealed class RenderRites
{
    private static readonly Lazy<RenderRites> LazyMachine = new(() => new RenderRites());
    public static RenderRites Machine => LazyMachine.Value;

    public Window? Window;
    public readonly SceneManager Scenes;

    private RenderRites()
    {
        Scenes = new SceneManager();
    }

    public void RunWindow(string title, VSyncMode vSync = VSyncMode.Adaptive, int samples = 4, string? iconPath = null)
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

        nws.Flags |= ContextFlags.Debug;
        
        if (iconPath != null)
        {
            FileStream iconStream = File.OpenRead(iconPath);
            ImageResult? image = ImageResult.FromStream(iconStream, ColorComponents.RedGreenBlueAlpha);

            WindowIcon icon = new(new Image(image.Width, image.Height, image.Data));
            nws.Icon = icon;
        }
        
        Window = new Window(gws, nws);
        Window.Run();
        Scenes.Dispose();
    }
}