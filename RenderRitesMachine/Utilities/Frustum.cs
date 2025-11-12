using OpenTK.Mathematics;

namespace RenderRitesMachine.Utilities;

/// <summary>
/// Представляет пирамиду видимости (frustum) камеры для frustum culling.
/// Учитывает, что OpenTK использует транспонированные матрицы (row-major).
/// </summary>
public class Frustum
{
    private readonly Plane[] _planes = new Plane[6];

    /// <summary>
    /// Плоскости пирамиды видимости: Left, Right, Bottom, Top, Near, Far.
    /// </summary>
    public Plane[] Planes => _planes;

    /// <summary>
    /// Создает frustum из матрицы ViewProjection.
    /// </summary>
    /// <param name="viewProjection">Матрица ViewProjection (View * Projection).</param>
    /// <remarks>
    /// OpenTK использует row-major матрицы, поэтому при извлечении плоскостей
    /// нужно учитывать транспонирование. Плоскости извлекаются из транспонированной матрицы.
    /// </remarks>
    public Frustum(Matrix4 viewProjection)
    {
        ExtractPlanes(viewProjection);
    }

    /// <summary>
    /// Создает frustum из матриц View и Projection.
    /// </summary>
    /// <param name="view">Матрица вида.</param>
    /// <param name="projection">Матрица проекции.</param>
    public Frustum(Matrix4 view, Matrix4 projection)
        : this(view * projection)
    {
    }

    /// <summary>
    /// Извлекает плоскости пирамиды видимости из матрицы ViewProjection.
    /// Учитывает, что OpenTK использует row-major матрицы.
    /// </summary>
    /// <remarks>
    /// OpenTK хранит матрицы в row-major формате. Для извлечения плоскостей используем
    /// стандартный алгоритм, работающий со столбцами матрицы. В row-major формате столбцы
    /// извлекаются как (Row0.X, Row1.X, Row2.X, Row3.X) и т.д.
    /// </remarks>
    private void ExtractPlanes(Matrix4 m)
    {
        _planes[0] = new Plane(
            m.Row0.W + m.Row0.X,
            m.Row1.W + m.Row1.X,
            m.Row2.W + m.Row2.X,
            m.Row3.W + m.Row3.X
        );

        _planes[1] = new Plane(
            m.Row0.W - m.Row0.X,
            m.Row1.W - m.Row1.X,
            m.Row2.W - m.Row2.X,
            m.Row3.W - m.Row3.X
        );

        _planes[2] = new Plane(
            m.Row0.W + m.Row0.Y,
            m.Row1.W + m.Row1.Y,
            m.Row2.W + m.Row2.Y,
            m.Row3.W + m.Row3.Y
        );

        _planes[3] = new Plane(
            m.Row0.W - m.Row0.Y,
            m.Row1.W - m.Row1.Y,
            m.Row2.W - m.Row2.Y,
            m.Row3.W - m.Row3.Y
        );

        _planes[4] = new Plane(
            m.Row0.W + m.Row0.Z,
            m.Row1.W + m.Row1.Z,
            m.Row2.W + m.Row2.Z,
            m.Row3.W + m.Row3.Z
        );

        _planes[5] = new Plane(
            m.Row0.W - m.Row0.Z,
            m.Row1.W - m.Row1.Z,
            m.Row2.W - m.Row2.Z,
            m.Row3.W - m.Row3.Z
        );

        for (int i = 0; i < 6; i++)
        {
            _planes[i].Normalize();
        }
    }

    /// <summary>
    /// Проверяет, пересекается ли AABB (Axis-Aligned Bounding Box) с пирамидой видимости.
    /// </summary>
    /// <param name="min">Минимальная точка AABB в мировом пространстве.</param>
    /// <param name="max">Максимальная точка AABB в мировом пространстве.</param>
    /// <returns>True, если AABB пересекается или находится внутри frustum, иначе false.</returns>
    public bool IntersectsAABB(Vector3 min, Vector3 max)
    {
        foreach (Plane plane in _planes)
        {
            Vector3 positiveVertex = new(
                plane.Normal.X > 0 ? max.X : min.X,
                plane.Normal.Y > 0 ? max.Y : min.Y,
                plane.Normal.Z > 0 ? max.Z : min.Z
            );

            float distance = plane.DistanceToPoint(positiveVertex);
            if (distance < 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Проверяет, пересекается ли AABB (в локальном пространстве) с пирамидой видимости
    /// после применения матрицы трансформации. Оптимизированная версия, которая работает
    /// с трансформированным AABB напрямую без преобразования всех вершин.
    /// </summary>
    /// <param name="localMin">Минимальная точка AABB в локальном пространстве.</param>
    /// <param name="localMax">Максимальная точка AABB в локальном пространстве.</param>
    /// <param name="modelMatrix">Матрица трансформации из локального в мировое пространство.</param>
    /// <returns>True, если AABB пересекается или находится внутри frustum, иначе false.</returns>
    public bool IntersectsAABB(Vector3 localMin, Vector3 localMax, Matrix4 modelMatrix)
    {
        Vector3 center = (localMin + localMax) * 0.5f;
        Vector3 extent = (localMax - localMin) * 0.5f;

        Vector3 worldCenter = Vector3.TransformPosition(center, modelMatrix);

        Vector3 axisX = new Vector3(modelMatrix.Row0.X, modelMatrix.Row1.X, modelMatrix.Row2.X);
        Vector3 axisY = new Vector3(modelMatrix.Row0.Y, modelMatrix.Row1.Y, modelMatrix.Row2.Y);
        Vector3 axisZ = new Vector3(modelMatrix.Row0.Z, modelMatrix.Row1.Z, modelMatrix.Row2.Z);

        axisX *= extent.X;
        axisY *= extent.Y;
        axisZ *= extent.Z;

        foreach (Plane plane in _planes)
        {
            float r = Math.Abs(Vector3.Dot(axisX, plane.Normal)) +
                      Math.Abs(Vector3.Dot(axisY, plane.Normal)) +
                      Math.Abs(Vector3.Dot(axisZ, plane.Normal));

            float distance = plane.DistanceToPoint(worldCenter);

            if (distance < -r)
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// Представляет плоскость в 3D пространстве в форме ax + by + cz + d = 0.
/// </summary>
public struct Plane
{
    public Vector3 Normal;
    public float D;

    public Plane(float a, float b, float c, float d)
    {
        Normal = new Vector3(a, b, c);
        D = d;
    }

    public Plane(Vector3 normal, float d)
    {
        Normal = normal;
        D = d;
    }

    /// <summary>
    /// Нормализует плоскость (нормализует нормаль и корректирует D).
    /// </summary>
    public void Normalize()
    {
        float length = Normal.Length;
        if (length > 0.0001f)
        {
            float invLength = 1.0f / length;
            Normal *= invLength;
            D *= invLength;
        }
    }

    /// <summary>
    /// Вычисляет расстояние от точки до плоскости.
    /// Положительное значение означает, что точка находится с положительной стороны плоскости.
    /// </summary>
    public float DistanceToPoint(Vector3 point)
    {
        return Vector3.Dot(Normal, point) + D;
    }
}

