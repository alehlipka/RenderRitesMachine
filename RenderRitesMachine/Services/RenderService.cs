using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для рендеринга различных объектов (меши, bounding boxes, контуры).
/// </summary>
public class RenderService : IRenderService
{
    private int _currentShaderId = -1;
    private int _currentTextureId = -1;
    private int _currentVao = -1;

    public void Render(int vao, int indicesCount, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset? texture, PrimitiveType primitiveType)
    {
        if (shader == null) throw new ArgumentNullException(nameof(shader));

        if (texture != null)
        {
            if (_currentTextureId != texture.Id)
            {
                texture.Bind();
                _currentTextureId = texture.Id;
            }
        }
        else
        {
            _currentTextureId = -1;
        }

        if (_currentShaderId != shader.Id)
        {
            shader.Use();
            _currentShaderId = shader.Id;
        }

        shader.SetMatrix4("model", meshModelMatrix);

        if (_currentVao != vao)
        {
            GL.BindVertexArray(vao);
            _currentVao = vao;
        }

        GL.DrawElements(primitiveType, indicesCount, DrawElementsType.UnsignedInt, 0);
    }

    public void RenderOutline(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, Vector3 cameraPosition, Vector2 viewportSize)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));

        GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(false);
        GL.DepthFunc(DepthFunction.Less);
        GL.CullFace(TriangleFace.Front);

        if (_currentShaderId != shader.Id)
        {
            shader.Use();
            _currentShaderId = shader.Id;
        }

        shader.SetMatrix4("model", meshModelMatrix);
        shader.SetVector3("cameraPosition", cameraPosition);
        shader.SetVector2("viewportSize", viewportSize);

        if (_currentVao != mesh.Vao)
        {
            GL.BindVertexArray(mesh.Vao);
            _currentVao = mesh.Vao;
        }

        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        GL.CullFace(TriangleFace.Back);
        GL.DepthMask(true);
    }
}
