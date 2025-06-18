using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Features.BoundingBox.Components;

namespace RenderRitesMachine.Utilities;

public static class BoundingBoxCreator
{
    public static BoundingBoxComponent Create(MeshComponent meshComponent)
    {
        float[] vertices =
        [
            meshComponent.Minimum.X, meshComponent.Minimum.Y, meshComponent.Minimum.Z,
            meshComponent.Maximum.X, meshComponent.Minimum.Y, meshComponent.Minimum.Z,
            meshComponent.Maximum.X, meshComponent.Maximum.Y, meshComponent.Minimum.Z,
            meshComponent.Minimum.X, meshComponent.Maximum.Y, meshComponent.Minimum.Z,
            meshComponent.Minimum.X, meshComponent.Minimum.Y, meshComponent.Maximum.Z,
            meshComponent.Maximum.X, meshComponent.Minimum.Y, meshComponent.Maximum.Z,
            meshComponent.Maximum.X, meshComponent.Maximum.Y, meshComponent.Maximum.Z,
            meshComponent.Minimum.X, meshComponent.Maximum.Y, meshComponent.Maximum.Z
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
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
        
        return new BoundingBoxComponent
        {
            Vao = vao,
            PrimitiveType = PrimitiveType.Lines,
            Count = indices.Length,
            DrawElementsType = DrawElementsType.UnsignedInt,
            IndicesStoreLocation = 0
        };
    }
}