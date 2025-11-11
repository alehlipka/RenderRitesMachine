using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

public static class RenderService
{
    public static void Render(BoundingBoxAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix)
    {
        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Lines, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }

    public static void Render(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset texture)
    {
        texture.Bind();
        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }

    public static void RenderOutline(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, Vector3 cameraPosition)
    {
        GL.Disable(EnableCap.DepthTest);
        GL.CullFace(TriangleFace.Front);
        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        shader.SetVector3("cameraPosition", cameraPosition);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        GL.CullFace(TriangleFace.Back);
        GL.Enable(EnableCap.DepthTest);
    }
}
