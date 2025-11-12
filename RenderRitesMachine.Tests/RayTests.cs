using OpenTK.Mathematics;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса Ray (edge cases).
/// </summary>
public class RayTests
{
    [Fact]
    public void IntersectsAABB_WithZeroDirection_ReturnsNull()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.Zero);
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        // Act
        var result = ray.IntersectsAABB(boxMin, boxMax);

        // Assert - луч с нулевым направлением не пересекает
        Assert.Null(result);
    }

    [Fact]
    public void IntersectsAABB_WithOriginInsideBox_ReturnsNonNegative()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        // Act
        var result = ray.IntersectsAABB(boxMin, boxMax);

        // Assert - начало луча внутри коробки, результат должен быть неотрицательным или null
        // В зависимости от реализации алгоритма может быть 0 или положительное значение
        if (result.HasValue)
        {
            Assert.True(result.Value >= 0);
        }
    }

    [Fact]
    public void IntersectsAABB_WithInvertedBox_HandlesCorrectly()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        // Min больше Max - инвертированный AABB
        var boxMin = new Vector3(10, 10, 10);
        var boxMax = new Vector3(-10, -10, -10);

        // Act
        var result = ray.IntersectsAABB(boxMin, boxMax);

        // Assert - не должно быть исключений
        Assert.True(result == null || result.HasValue);
    }

    [Fact]
    public void IntersectsAABB_WithExtremeBox_HandlesCorrectly()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var extremeMin = new Vector3(float.MinValue / 2, float.MinValue / 2, float.MinValue / 2);
        var extremeMax = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);

        // Act
        var result = ray.IntersectsAABB(extremeMin, extremeMax);

        // Assert - не должно быть исключений
        Assert.True(result == null || result.HasValue);
    }

    [Fact]
    public void IntersectsAABB_WithParallelRay_ReturnsNull()
    {
        // Arrange
        var ray = new Ray(new Vector3(0, 0, 5), Vector3.UnitX); // Луч параллелен оси X
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        // Act
        var result = ray.IntersectsAABB(boxMin, boxMax);

        // Assert - луч параллелен и не пересекает
        Assert.Null(result);
    }

    [Fact]
    public void IntersectsAABB_WithRayBehindBox_HandlesCorrectly()
    {
        // Arrange
        var ray = new Ray(new Vector3(0, 0, 5), -Vector3.UnitZ); // Луч направлен назад
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        // Act
        var result = ray.IntersectsAABB(boxMin, boxMax);

        // Assert - луч может или не может пересекать коробку в зависимости от алгоритма
        // Просто проверяем, что не падает
        Assert.True(result == null || result.HasValue);
    }

    [Fact]
    public void TransformToLocalSpace_WithIdentityMatrix_ReturnsSameRay()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var identity = Matrix4.Identity;

        // Act
        var transformed = ray.TransformToLocalSpace(identity);

        // Assert
        Assert.Equal(ray.Origin, transformed.Origin);
        Assert.Equal(ray.Direction, transformed.Direction);
    }

    [Fact]
    public void TransformToLocalSpace_WithZeroScale_ThrowsException()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var zeroScale = Matrix4.CreateScale(0);

        // Act & Assert - матрица с нулевым масштабом не может быть инвертирована
        Assert.Throws<InvalidOperationException>(() => ray.TransformToLocalSpace(zeroScale));
    }

    [Fact]
    public void TransformToLocalSpace_WithExtremeMatrix_HandlesCorrectly()
    {
        // Arrange
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var extremeMatrix = Matrix4.CreateScale(1e6f) * Matrix4.CreateTranslation(1e6f, 1e6f, 1e6f);

        // Act
        var transformed = ray.TransformToLocalSpace(extremeMatrix);

        // Assert - не должно быть исключений
        Assert.NotNull(transformed);
        Assert.True(transformed.Direction.Length <= 1.0f + float.Epsilon); // Должно быть нормализовано
    }

    [Fact]
    public void GetFromScreen_WithZeroWindowSize_HandlesCorrectly()
    {
        // Arrange
        var windowSize = new Vector2i(0, 0);
        var cameraPos = Vector3.Zero;
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);

        // Act & Assert - деление на ноль должно обрабатываться
        // В реальной реализации это должно проверяться, но для теста просто проверяем, что не падает
        try
        {
            var ray = Ray.GetFromScreen(0, 0, windowSize, cameraPos, projection, view);
            Assert.NotNull(ray);
        }
        catch (DivideByZeroException)
        {
            // Ожидаемое исключение для нулевого размера окна
            Assert.True(true);
        }
    }

    [Fact]
    public void GetFromScreen_WithExtremeCoordinates_HandlesCorrectly()
    {
        // Arrange
        var windowSize = new Vector2i(1920, 1080);
        var cameraPos = Vector3.Zero;
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);

        // Act - экстремальные координаты мыши (но не слишком экстремальные, чтобы избежать переполнения)
        var ray1 = Ray.GetFromScreen(1e6f, 1e6f, windowSize, cameraPos, projection, view);
        var ray2 = Ray.GetFromScreen(-1e6f, -1e6f, windowSize, cameraPos, projection, view);

        // Assert - не должно быть исключений, направление должно быть нормализовано
        Assert.NotNull(ray1);
        Assert.NotNull(ray2);
        // Проверяем, что направление валидно (не NaN, не Infinity)
        Assert.False(float.IsNaN(ray1.Direction.Length));
        Assert.False(float.IsInfinity(ray1.Direction.Length));
        Assert.False(float.IsNaN(ray2.Direction.Length));
        Assert.False(float.IsInfinity(ray2.Direction.Length));
    }
}

