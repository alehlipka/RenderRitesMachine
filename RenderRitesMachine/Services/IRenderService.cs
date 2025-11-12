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
    void RenderOutline(MeshAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix, Vector3 cameraPosition, Vector2 viewportSize);

    /// <summary>
    /// Рендерит множество одинаковых объектов за один вызов (batch rendering).
    /// </summary>
    void RenderBatch(MeshAsset mesh, ShaderAsset shader, TextureAsset texture, List<Matrix4> modelMatrices);

    /// <summary>
    /// Рендерит множество одинаковых bounding boxes за один вызов (batch rendering).
    /// </summary>
    void RenderBatch(BoundingBoxAsset mesh, ShaderAsset shader, List<Matrix4> modelMatrices);
}

