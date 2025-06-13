using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.Utilities;

public static class BoundingBoxCreator
{
    public static BoundingBoxComponent Create(Entity parent, Vector3 min, Vector3 max)
    {
        float[] vertices =
        [
            min.X, min.Y, min.Z,
            max.X, min.Y, min.Z,
            max.X, max.Y, min.Z,
            min.X, max.Y, min.Z,
            min.X, min.Y, max.Z,
            max.X, min.Y, max.Z,
            max.X, max.Y, max.Z,
            min.X, max.Y, max.Z
        ];
        
        uint[] indices =
        [
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 4, 1, 5, 2, 6, 3, 7
        ];
        
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();
        int vao = GL.GenVertexArray();
        
        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(vao);
        GL.DeleteBuffer(ebo);
        
        return new BoundingBoxComponent()
        {
            Parent = parent,
            Vao = vao,
            PrimitiveType = PrimitiveType.Lines,
            Count = indices.Length,
            DrawElementsType = DrawElementsType.UnsignedInt,
            IndicesStoreLocation = 0,
            OriginalMinimum = min,
            OriginalMaximum = max
        };
    }
    
    public static BoundingBoxComponent CreateForEntity(World world, Entity entity)
    {
        MeshComponent meshComponent = world.GetComponent<MeshComponent>(entity);
        
        return Create(entity, meshComponent.Minimum, meshComponent.Maximum);
    }
}