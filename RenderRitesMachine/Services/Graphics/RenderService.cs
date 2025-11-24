using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Rendering service that draws meshes, bounding boxes, and other primitives.
/// </summary>
public class RenderService : IRenderService
{
    private int _currentShaderId = -1;
    private int _currentTextureId = -1;
    private int _currentVao = -1;

    public void Render(int vao, int indicesCount, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset? texture, PrimitiveType primitiveType)
    {
        ArgumentNullException.ThrowIfNull(shader);

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

    public void ResetStateCache()
    {
        _currentShaderId = -1;
        _currentTextureId = -1;
        _currentVao = -1;
    }
}
