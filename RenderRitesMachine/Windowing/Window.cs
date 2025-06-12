using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.Windowing;

public class Window(GameWindowSettings gws, NativeWindowSettings nws) : GameWindow(gws, nws)
{
    protected override void OnLoad()
    {
        #if DEBUG
        GlDebugWatchdog.Initialize();
        FpsCounter.Initialize();
        #endif
        
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.DepthTest);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.CullFace(TriangleFace.Back);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.ClearColor(Color4.Black);
        GL.ActiveTexture(TextureUnit.Texture0);
        
        RenderRites.Machine.SceneManager.Current?.Initialize();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        RenderRites.Machine.SceneManager.Current?.ResizeScene(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        #if DEBUG
        FpsCounter.Update(args.Time);
        RenderRites.Machine.Window!.Title = "RenderRates Machine " +
                                            $"FPS: {FpsCounter.GetFps():0.0} | " +
                                            $"GPU: {FpsCounter.GetGpuTime():0.00} ms | " +
                                            $"CPU: {FpsCounter.GetCpuTime():0.00} ms";
        #endif
        
        // RenderRites.Machine.SceneManager.Current?.UpdateScene(args);
        
        if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        #if DEBUG
        FpsCounter.BeginCpuMeasure();
        FpsCounter.BeginGpuMeasure();
        #endif
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        // RenderRites.Machine.SceneManager.Current?.RenderScene(args);
        World.Update((float)args.Time);
        #if DEBUG
        FpsCounter.EndGpuMeasure();
        #endif
        SwapBuffers();
        #if DEBUG
        FpsCounter.EndCpuMeasure();
        #endif
    }
}