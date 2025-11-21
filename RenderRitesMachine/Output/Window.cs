using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Debug;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

public class Window(GameWindowSettings gws, NativeWindowSettings nws, SceneManager sceneManager, ILogger? logger = null) : GameWindow(gws, nws)
{
    private readonly SceneManager _sceneManager = sceneManager;
    private readonly ILogger? _logger = logger;

    protected override void OnLoad()
    {
        _logger?.LogInfo($"Window initialized: {ClientSize.X}x{ClientSize.Y}");
        FpsCounter.Initialize();
        GlDebugWatchdog.Initialize(_logger);
        _logger?.LogDebug("FPS counter and OpenGL debug watchdog initialized");

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Blend);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.CullFace(TriangleFace.Back);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        GL.ClearColor(Color4.Black);
        _logger?.LogDebug("OpenGL state configured");

        Scene? currentScene = _sceneManager.Current;
        if (currentScene != null)
        {
            currentScene.SetWindow(this);
            currentScene.Initialize();
            currentScene.ResizeScene(new ResizeEventArgs(ClientSize));
            _logger?.LogInfo($"Initial scene '{currentScene.Name}' loaded");
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        _logger?.LogDebug($"Window resized to {e.Width}x{e.Height}");
        _sceneManager.Current?.ResizeScene(e);
    }

    private string? _lastSceneName;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Title = $"RenderRites Machine FPS: {FpsCounter.GetFps():F0}";

        Scene? currentScene = _sceneManager.Current;
        string? currentSceneName = currentScene?.Name;

        if (currentSceneName != _lastSceneName && currentScene != null)
        {
            _logger?.LogInfo($"Scene changed detected: '{_lastSceneName ?? "none"}' -> '{currentSceneName}'");
            currentScene.SetWindow(this);
            currentScene.Initialize();
            currentScene.ResizeScene(new ResizeEventArgs(ClientSize));
            _lastSceneName = currentSceneName;
        }

        currentScene?.UpdateScene(args);

        if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            _logger?.LogInfo("Escape key pressed, closing window");
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        FpsCounter.Update();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        _sceneManager.Current?.RenderScene(args);

        SwapBuffers();
    }
}
