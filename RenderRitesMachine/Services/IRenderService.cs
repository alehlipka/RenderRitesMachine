using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Интерфейс для сервиса рендеринга, предоставляющего методы для отрисовки различных объектов.
/// </summary>
public interface IRenderService
{
    /// <summary>
    /// Рендерит bounding box.
    /// </summary>
    void Render(BoundingBoxAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix);

    /// <summary>
    /// Рендерит меш с текстурой.
    /// </summary>
    void Render(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset texture);

    /// <summary>
    /// Рендерит контур меша.
    /// </summary>
    void RenderOutline(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, Vector3 cameraPosition);
}

