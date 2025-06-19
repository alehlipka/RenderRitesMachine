using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

public static class RenderService
{
    public static void Render(MeshAsset mesh, TextureAsset texture, ShaderAsset shader, Matrix4 meshModelMatrix)
    {
        texture.Bind();
        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }
}
