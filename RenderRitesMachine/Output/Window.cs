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
        
        RenderRites.Machine.Scenes.Current?.Initialize();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        RenderRites.Machine.Scenes.Current?.ResizeScene(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        RenderRites.Machine.Window!.Title = $"RenderRates Machine FPS: {FpsCounter.GetFps():F0}";
        
        RenderRites.Machine.Scenes.Current?.UpdateScene(args);
        
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
        SwapBuffers();
    }
}