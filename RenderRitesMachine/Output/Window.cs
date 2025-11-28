using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Debug;
using RenderRitesMachine.Services.Diagnostics;
using RenderRitesMachine.Services.Graphics;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Output;

public class Window(
    GameWindowSettings gws,
    NativeWindowSettings nws,
    SceneManager sceneManager,
    IGuiService guiService,
    IRenderService renderService,
    ILogger? logger = null) : GameWindow(gws, nws)
{
    private static readonly Keys[] TrackedKeys = Enum.GetValues<Keys>();
    private readonly IGuiService _guiService = guiService;
    private readonly ILogger? _logger = logger;
    private readonly IRenderService _renderService = renderService;
    private readonly SceneManager _sceneManager = sceneManager;

    private string? _lastSceneName;
    private KeyboardState _previousKeyboardState = null!;

    protected override void OnLoad()
    {
        _logger?.LogInfo($"Window initialized: {ClientSize.X}x{ClientSize.Y}");
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

        _guiService.EnsureInitialized(ClientSize.X, ClientSize.Y);
        _previousKeyboardState = KeyboardState;

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
        _guiService.Resize(e.Width, e.Height);
        _sceneManager.Current?.ResizeScene(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Scene? currentScene = _sceneManager.Current;
        string? currentSceneName = currentScene?.Name;

        PumpGuiInput();

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
        FrameTimeCounter.Update();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        _guiService.BeginFrame(Color4.Transparent);
        _sceneManager.Current?.RenderScene(args);
        _guiService.EndFrame();
        if (_guiService.HasContent)
        {
            _guiService.Render();
            _renderService.ResetStateCache();
        }

        SwapBuffers();
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        _guiService.Events.Enqueue(GuiEvent.MouseMove(MouseState.Position));
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        _guiService.Events.Enqueue(GuiEvent.MouseDown(MouseState.Position, e.Button));
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        _guiService.Events.Enqueue(GuiEvent.MouseUp(MouseState.Position, e.Button));
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        Vector2d offset = e.Offset;
        Vector2 scrollDelta = new((float)offset.X, (float)offset.Y);
        _guiService.Events.Enqueue(GuiEvent.MouseScroll(MouseState.Position, scrollDelta));
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        char character = (char)e.Unicode;
        if (!char.IsControl(character))
        {
            _guiService.Events.Enqueue(GuiEvent.TextInput(character));
        }
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        _guiService.Events.Enqueue(GuiEvent.KeyDown(e.Key));
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
        base.OnKeyUp(e);

        _guiService.Events.Enqueue(GuiEvent.KeyUp(e.Key));
    }

    private void PumpGuiInput()
    {
        KeyboardState currentKeyboard = KeyboardState;

        foreach (Keys key in TrackedKeys)
        {
            if (key.Equals(Keys.Unknown))
            {
                continue;
            }

            bool isDown = currentKeyboard.IsKeyDown(key);
            bool wasDown = _previousKeyboardState.IsKeyDown(key);

            if (isDown && !wasDown)
            {
                _guiService.Events.Enqueue(GuiEvent.KeyDown(key));
            }
            else if (!isDown && wasDown)
            {
                _guiService.Events.Enqueue(GuiEvent.KeyUp(key));
            }
        }

        _previousKeyboardState = currentKeyboard;
    }
}
