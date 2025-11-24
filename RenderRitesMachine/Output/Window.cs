using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Debug;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Output;

public class Window(GameWindowSettings gws, NativeWindowSettings nws, SceneManager sceneManager, IGuiService guiService, ILogger? logger = null) : GameWindow(gws, nws)
{
    private readonly SceneManager _sceneManager = sceneManager;
    private readonly IGuiService _guiService = guiService;
    private readonly ILogger? _logger = logger;
    private KeyboardState _previousKeyboardState = null!;
    private MouseState _previousMouseState = null!;
    private static readonly MouseButton[] TrackedMouseButtons = [MouseButton.Left, MouseButton.Right, MouseButton.Middle];
    private static readonly Keys[] TrackedKeys = Enum.GetValues<Keys>();

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

        _guiService.EnsureInitialized(ClientSize.X, ClientSize.Y);
        _previousKeyboardState = KeyboardState;
        _previousMouseState = MouseState;

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

    private string? _lastSceneName;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Title = $"RenderRites Machine FPS: {FpsCounter.GetFps():F0}";

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

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        _guiService.BeginFrame(Color4.Transparent);
        _sceneManager.Current?.RenderScene(args);
        _guiService.EndFrame();
        _guiService.Render();

        SwapBuffers();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        Vector2 position = MouseState.Position;
        Vector2d offset = e.Offset;
        Vector2 scrollDelta = new((float)offset.X, (float)offset.Y);
        _guiService.Events.Enqueue(GuiEvent.MouseScroll(position, scrollDelta));
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

    private void PumpGuiInput()
    {
        KeyboardState currentKeyboard = KeyboardState;

        foreach (Keys key in TrackedKeys)
        {
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

        MouseState currentMouse = MouseState;
        Vector2 currentPosition = currentMouse.Position;
        if (currentPosition != _previousMouseState.Position)
        {
            _guiService.Events.Enqueue(GuiEvent.MouseMove(currentPosition));
        }

        foreach (MouseButton button in TrackedMouseButtons)
        {
            bool wasDown = _previousMouseState.IsButtonDown(button);
            bool isDown = currentMouse.IsButtonDown(button);

            if (isDown && !wasDown)
            {
                _guiService.Events.Enqueue(GuiEvent.MouseDown(currentPosition, button));
            }
            else if (!isDown && wasDown)
            {
                _guiService.Events.Enqueue(GuiEvent.MouseUp(currentPosition, button));
            }
        }

        _previousKeyboardState = currentKeyboard;
        _previousMouseState = currentMouse;
    }
}
