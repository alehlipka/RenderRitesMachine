using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Features.Mesh.Components;

public struct MeshComponent : IComponent
{
    public int Vao;
    public PrimitiveType PrimitiveType;
    public int Count;
    public DrawElementsType DrawElementsType;
    public int IndicesStoreLocation;
    public Vector3 Minimum;
    public Vector3 Maximum;

    public void Dispose()
    {
        GL.BindVertexArray(0);
        GL.DeleteVertexArray(Vao);
    }
}