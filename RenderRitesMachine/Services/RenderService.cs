using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для рендеринга различных объектов (меши, bounding boxes, контуры).
/// </summary>
public class RenderService : IRenderService, IDisposable
{
    private readonly Dictionary<int, InstanceVboCache> _instanceVboCache = new();

    private class InstanceVboCache
    {
        public int Vbo { get; set; }
        public int Capacity { get; set; }
    }
    public void Render(BoundingBoxAsset mesh, ShaderAsset shader, Matrix4 meshModelMatrix)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));

        shader.Use();
        shader.SetBool("useInstanced", false);
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
        shader.SetBool("useInstanced", false);
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
        shader.SetBool("useInstanced", false);
        shader.SetMatrix4("model", meshModelMatrix);
        shader.SetVector3("cameraPosition", cameraPosition);
        shader.SetVector2("viewportSize", viewportSize);
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        GL.CullFace(TriangleFace.Back);
        GL.DepthMask(true);
    }

    public void RenderBatch(MeshAsset mesh, ShaderAsset shader, TextureAsset texture, List<Matrix4> modelMatrices)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));
        if (texture == null) throw new ArgumentNullException(nameof(texture));
        if (modelMatrices == null) throw new ArgumentNullException(nameof(modelMatrices));
        if (modelMatrices.Count == 0) return;

        texture.Bind();
        shader.Use();
        shader.SetBool("useInstanced", true);
        GL.BindVertexArray(mesh.Vao);

        int instanceCount = modelMatrices.Count;

        // Получаем или создаем кэшированный VBO
        if (!_instanceVboCache.TryGetValue(mesh.Vao, out InstanceVboCache? cache) || cache.Capacity < instanceCount)
        {
            // Удаляем старый VBO если он существует
            if (cache != null)
            {
                GL.DeleteBuffer(cache.Vbo);
            }

            // Создаем новый VBO с достаточной емкостью
            cache = new InstanceVboCache
            {
                Vbo = GL.GenBuffer(),
                Capacity = instanceCount
            };
            _instanceVboCache[mesh.Vao] = cache;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, cache.Vbo);
        float[] matrixData = new float[instanceCount * 16];
        for (int i = 0; i < instanceCount; i++)
        {
            Matrix4 m = modelMatrices[i];
            int offset = i * 16;
            matrixData[offset + 0] = m.M11; matrixData[offset + 1] = m.M21; matrixData[offset + 2] = m.M31; matrixData[offset + 3] = m.M41;
            matrixData[offset + 4] = m.M12; matrixData[offset + 5] = m.M22; matrixData[offset + 6] = m.M32; matrixData[offset + 7] = m.M42;
            matrixData[offset + 8] = m.M13; matrixData[offset + 9] = m.M23; matrixData[offset + 10] = m.M33; matrixData[offset + 11] = m.M43;
            matrixData[offset + 12] = m.M14; matrixData[offset + 13] = m.M24; matrixData[offset + 14] = m.M34; matrixData[offset + 15] = m.M44;
        }
        GL.BufferData(BufferTarget.ArrayBuffer, instanceCount * 16 * sizeof(float), matrixData, BufferUsageHint.DynamicDraw);

        for (int i = 0; i < 4; i++)
        {
            int location = 3 + i;
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), i * 4 * sizeof(float));
            GL.VertexAttribDivisor(location, 1);
        }

        GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceCount);

        for (int i = 0; i < 4; i++)
        {
            GL.VertexAttribDivisor(3 + i, 0);
            GL.DisableVertexAttribArray(3 + i);
        }

        shader.SetBool("useInstanced", false);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void RenderBatch(BoundingBoxAsset mesh, ShaderAsset shader, List<Matrix4> modelMatrices)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));
        if (shader == null) throw new ArgumentNullException(nameof(shader));
        if (modelMatrices == null) throw new ArgumentNullException(nameof(modelMatrices));
        if (modelMatrices.Count == 0) return;

        shader.Use();
        shader.SetBool("useInstanced", true);
        GL.BindVertexArray(mesh.Vao);

        int instanceCount = modelMatrices.Count;

        // Получаем или создаем кэшированный VBO
        if (!_instanceVboCache.TryGetValue(mesh.Vao, out InstanceVboCache? cache) || cache.Capacity < instanceCount)
        {
            // Удаляем старый VBO если он существует
            if (cache != null)
            {
                GL.DeleteBuffer(cache.Vbo);
            }

            // Создаем новый VBO с достаточной емкостью
            cache = new InstanceVboCache
            {
                Vbo = GL.GenBuffer(),
                Capacity = instanceCount
            };
            _instanceVboCache[mesh.Vao] = cache;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, cache.Vbo);
        float[] matrixData = new float[instanceCount * 16];
        for (int i = 0; i < instanceCount; i++)
        {
            Matrix4 m = modelMatrices[i];
            int offset = i * 16;
            matrixData[offset + 0] = m.M11; matrixData[offset + 1] = m.M21; matrixData[offset + 2] = m.M31; matrixData[offset + 3] = m.M41;
            matrixData[offset + 4] = m.M12; matrixData[offset + 5] = m.M22; matrixData[offset + 6] = m.M32; matrixData[offset + 7] = m.M42;
            matrixData[offset + 8] = m.M13; matrixData[offset + 9] = m.M23; matrixData[offset + 10] = m.M33; matrixData[offset + 11] = m.M43;
            matrixData[offset + 12] = m.M14; matrixData[offset + 13] = m.M24; matrixData[offset + 14] = m.M34; matrixData[offset + 15] = m.M44;
        }
        GL.BufferData(BufferTarget.ArrayBuffer, instanceCount * 16 * sizeof(float), matrixData, BufferUsageHint.DynamicDraw);

        for (int i = 0; i < 4; i++)
        {
            int location = 3 + i;
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), i * 4 * sizeof(float));
            GL.VertexAttribDivisor(location, 1);
        }

        GL.DrawElementsInstanced(PrimitiveType.Lines, mesh.IndicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceCount);

        for (int i = 0; i < 4; i++)
        {
            GL.VertexAttribDivisor(3 + i, 0);
            GL.DisableVertexAttribArray(3 + i);
        }

        shader.SetBool("useInstanced", false);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    /// <summary>
    /// Освобождает все кэшированные VBO для instance rendering.
    /// </summary>
    public void Dispose()
    {
        try
        {
            foreach (var cache in _instanceVboCache.Values)
            {
                if (cache != null)
                {
                    GL.DeleteBuffer(cache.Vbo);
                }
            }
        }
        catch
        {
            // Игнорируем ошибки при освобождении ресурсов OpenGL
            // Ресурсы могут быть уже освобождены или контекст OpenGL может быть недоступен
        }
        finally
        {
            _instanceVboCache.Clear();
        }
    }
}
