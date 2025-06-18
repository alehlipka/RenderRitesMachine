using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Features.BoundingBox.Components;

public class BoundingBoxComponent : IComponent
{
    public int Vao;
    public PrimitiveType PrimitiveType;
    public int Count;
    public DrawElementsType DrawElementsType;
    public int IndicesStoreLocation;
    
    public void Dispose()
    {
        GL.BindVertexArray(0);
        GL.DeleteVertexArray(Vao);
    }
}