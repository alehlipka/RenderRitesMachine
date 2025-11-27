using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Rendering service interface that exposes drawing primitives.
/// </summary>
public interface IRenderService
{
    /// <summary>
    /// Renders a mesh or bounding box.
    /// </summary>
    /// <param name="vao">Vertex Array Object identifier.</param>
    /// <param name="indicesCount">Number of indices to draw.</param>
    /// <param name="shader">Shader used for rendering.</param>
    /// <param name="meshModelMatrix">Model matrix applied to the mesh.</param>
    /// <param name="texture">Texture to bind, or null for untextured primitives.</param>
    /// <param name="primitiveType">Primitive type (e.g., triangles or lines).</param>
    void Render(int vao, int indicesCount, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset? texture, PrimitiveType primitiveType);

    /// <summary>
    /// Invalidates cached state so the next draw rebinds VAO, shader, and texture.
    /// </summary>
    void ResetStateCache();
}
