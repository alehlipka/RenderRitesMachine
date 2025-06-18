using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.Wireframe.System;

public class WireFrameUpdateSystem : IUpdateSystem
{
    private bool _isWireFrame;
    
    public void Update(float deltaTime, World world)
    {
        if (!RenderRites.Machine.Window!.IsKeyPressed(Keys.W)) return;
        
        _isWireFrame = !_isWireFrame;
        GL.PolygonMode(TriangleFace.FrontAndBack, _isWireFrame ? PolygonMode.Line : PolygonMode.Fill);
    }
}