using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.ECS.Components;

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