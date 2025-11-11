using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Debug;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

public class Window(GameWindowSettings gws, NativeWindowSettings nws, GuiService guiService, SceneManager sceneManager) : GameWindow(gws, nws)
{
    private readonly GuiService _guiService = guiService;
    private readonly SceneManager _sceneManager = sceneManager;

    protected override void OnLoad()
    {
        FpsCounter.Initialize();
        GlDebugWatchdog.Initialize();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Blend);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.CullFace(TriangleFace.Back);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        GL.ClearColor(Color4.Black);

        _guiService.Initialize(this);
        RenderRitesMachine.UI.UI.Initialize(_guiService);

        var currentScene = _sceneManager.Current;
        if (currentScene != null)
        {
            currentScene.SetWindow(this);
            currentScene.Initialize();
            currentScene.ResizeScene(new ResizeEventArgs(ClientSize));
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        _sceneManager.Current?.ResizeScene(e);
    }

    private string? _lastSceneName;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Title = $"RenderRites Machine FPS: {FpsCounter.GetFps():F0}";

        var currentScene = _sceneManager.Current;
        string? currentSceneName = currentScene?.Name;

        if (currentSceneName != _lastSceneName && currentScene != null)
        {
            currentScene.SetWindow(this);
            currentScene.Initialize();
            currentScene.ResizeScene(new ResizeEventArgs(ClientSize));
            _lastSceneName = currentSceneName;
        }

        _guiService.Update((float)args.Time);

        currentScene?.UpdateScene(args);

        if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        FpsCounter.Update();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        _sceneManager.Current?.RenderScene(args);

        _guiService.Render();

        SwapBuffers();
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        IntPtr context = _guiService.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
            ImGuiIOPtr io = ImGui.GetIO();
            if (e.Unicode > 0 && e.Unicode < 0x10000)
            {
                io.AddInputCharacter((uint)e.Unicode);
            }
        }
    }
}
