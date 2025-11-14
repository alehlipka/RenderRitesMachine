using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для рендеринга различных объектов (меши, bounding boxes, контуры).
/// </summary>
public class RenderService : IRenderService
{
    public void Render(BoundingBoxAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));

        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Lines, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }

    public void Render(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset texture)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));
        if (texture == null) throw new ArgumentNullException(nameof(texture));

        texture.Bind();
        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }

    public void RenderOutline(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, Vector3 cameraPosition, Vector2 viewportSize)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));

        GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(false);
        GL.DepthFunc(DepthFunction.Less);
        GL.CullFace(TriangleFace.Front);
        shader.Use();
        shader.SetMatrix4("model", meshModelMatrix);
        shader.SetVector3("cameraPosition", cameraPosition);
        shader.SetVector2("viewportSize", viewportSize);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        GL.CullFace(TriangleFace.Back);
        GL.DepthMask(true);
    }
}
