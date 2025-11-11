using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Debug;

namespace RenderRitesMachine.Output;

public class Window(GameWindowSettings gws, NativeWindowSettings nws) : GameWindow(gws, nws)
{
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
        
        // Инициализация GUI
        RenderRites.Machine.Gui.Initialize(this);
        
        
        var currentScene = RenderRites.Machine.Scenes.Current;
        if (currentScene != null)
        {
            currentScene.SetWindow(this);
            currentScene.Initialize();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        RenderRites.Machine.Scenes.Current?.ResizeScene(e);
    }

    private string? _lastSceneName;
    
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        RenderRites.Machine.Window!.Title = $"RenderRites Machine FPS: {FpsCounter.GetFps():F0}";
        
        // Проверяем, изменилась ли сцена
        var currentScene = RenderRites.Machine.Scenes.Current;
        string? currentSceneName = currentScene?.Name;
        
        if (currentSceneName != _lastSceneName && currentScene != null)
        {
            // Новая сцена была установлена - нужно её инициализировать
            currentScene.SetWindow(this);
            currentScene.Initialize();
            _lastSceneName = currentSceneName;
        }
        
        // Обновление GUI
        RenderRites.Machine.Gui.Update((float)args.Time);
        
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
        RenderRites.Machine.Scenes.Current?.RenderScene(args);
        
        // Рендеринг GUI поверх всего
        RenderRites.Machine.Gui.Render();
        
        SwapBuffers();
    }
    
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        
        // Передача текстового ввода в ImGui
        if (RenderRites.Machine.Gui != null)
        {
            IntPtr context = RenderRites.Machine.Gui.GetContext();
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
}