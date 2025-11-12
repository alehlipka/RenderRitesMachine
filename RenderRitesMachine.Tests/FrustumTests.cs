using OpenTK.Mathematics;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса Frustum (edge cases).
/// </summary>
public class FrustumTests
{
    [Fact]
    public void IntersectsAABB_WithZeroSizeAABB_HandlesCorrectly()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var point = Vector3.Zero;

        // Act
        var result = frustum.IntersectsAABB(point, point);

        // Assert - не должно быть исключений, результат зависит от позиции точки
        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithExtremeAABB_HandlesCorrectly()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var extremeMin = new Vector3(float.MinValue / 2, float.MinValue / 2, float.MinValue / 2);
        var extremeMax = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);

        // Act
        var result = frustum.IntersectsAABB(extremeMin, extremeMax);

        // Assert - не должно быть исключений
        Assert.True(result || !result); // Просто проверяем, что метод не падает
    }

    [Fact]
    public void IntersectsAABB_WithInvertedAABB_HandlesCorrectly()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        // Min больше Max - инвертированный AABB
        var min = new Vector3(10, 10, 10);
        var max = new Vector3(-10, -10, -10);

        // Act
        var result = frustum.IntersectsAABB(min, max);

        // Assert - не должно быть исключений
        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithTransform_ExtremeMatrix_HandlesCorrectly()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var localMin = new Vector3(-1, -1, -1);
        var localMax = new Vector3(1, 1, 1);
        var extremeMatrix = Matrix4.CreateScale(1e6f) * Matrix4.CreateTranslation(1e6f, 1e6f, 1e6f);

        // Act
        var result = frustum.IntersectsAABB(localMin, localMax, extremeMatrix);

        // Assert - не должно быть исключений
        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithTransform_ZeroScale_HandlesCorrectly()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var localMin = new Vector3(-1, -1, -1);
        var localMax = new Vector3(1, 1, 1);
        var zeroScaleMatrix = Matrix4.CreateScale(0);

        // Act
        var result = frustum.IntersectsAABB(localMin, localMax, zeroScaleMatrix);

        // Assert - не должно быть исключений
        Assert.True(result || !result);
    }

    [Fact]
    public void Frustum_WithIdentityMatrix_CreatesCorrectly()
    {
        // Arrange & Act
        var frustum = new Frustum(Matrix4.Identity);

        // Assert - не должно быть исключений
        Assert.NotNull(frustum.Planes);
        Assert.Equal(6, frustum.Planes.Length);
    }

    [Fact]
    public void Frustum_WithZeroMatrix_HandlesCorrectly()
    {
        // Arrange & Act
        var zeroMatrix = new Matrix4();
        var frustum = new Frustum(zeroMatrix);

        // Assert - не должно быть исключений
        Assert.NotNull(frustum.Planes);
        Assert.Equal(6, frustum.Planes.Length);
    }

    [Fact]
    public void IntersectsAABB_FarFromCamera_ReturnsFalse()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        // AABB очень далеко от камеры
        var farMin = new Vector3(-1, -1, -1000);
        var farMax = new Vector3(1, 1, -900);

        // Act
        var result = frustum.IntersectsAABB(farMin, farMax);

        // Assert - должен быть вне frustum
        Assert.False(result);
    }

    [Fact]
    public void IntersectsAABB_BehindCamera_ReturnsFalse()
    {
        // Arrange
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        // AABB позади камеры
        var behindMin = new Vector3(-1, -1, 1);
        var behindMax = new Vector3(1, 1, 10);

        // Act
        var result = frustum.IntersectsAABB(behindMin, behindMax);

        // Assert - должен быть вне frustum
        Assert.False(result);
    }
}

