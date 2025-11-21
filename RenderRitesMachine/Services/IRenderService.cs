using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Интерфейс для сервиса рендеринга, предоставляющего методы для отрисовки различных объектов.
/// </summary>
public interface IRenderService
{
    /// <summary>
    /// Рендерит меш или bounding box.
    /// </summary>
    /// <param name="vao">Vertex Array Object</param>
    /// <param name="indicesCount">Количество индексов</param>
    /// <param name="shader">Шейдер</param>
    /// <param name="meshModelMatrix">Матрица модели</param>
    /// <param name="texture">Текстура (опционально, null для bounding boxes)</param>
    /// <param name="primitiveType">Тип примитива (Triangles для мешей, Lines для bounding boxes)</param>
    void Render(int vao, int indicesCount, ShaderAsset shader, Matrix4 meshModelMatrix, TextureAsset? texture, PrimitiveType primitiveType);
}
