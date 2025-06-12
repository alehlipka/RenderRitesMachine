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
        
        RenderRites.Machine.Scenes.Current?.Initialize();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        RenderRites.Machine.Scenes.Current?.ResizeScene(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        #if DEBUG
        RenderRites.Machine.Window!.Title = $"RenderRates Machine FPS: {FpsCounter.GetFps():F0}";
        #endif
        
        RenderRites.Machine.Scenes.Current?.UpdateScene(args);
        
        if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        #if DEBUG
        FpsCounter.Update();
        #endif
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        RenderRites.Machine.Scenes.Current?.RenderScene(args);
        SwapBuffers();
    }
}