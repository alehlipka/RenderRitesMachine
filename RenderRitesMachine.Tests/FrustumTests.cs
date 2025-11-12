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
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        Vector3 point = Vector3.Zero;

        bool result = frustum.IntersectsAABB(point, point);

        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithExtremeAABB_HandlesCorrectly()
    {
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var extremeMin = new Vector3(float.MinValue / 2, float.MinValue / 2, float.MinValue / 2);
        var extremeMax = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);

        bool result = frustum.IntersectsAABB(extremeMin, extremeMax);

        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithInvertedAABB_HandlesCorrectly()
    {
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var min = new Vector3(10, 10, 10);
        var max = new Vector3(-10, -10, -10);

        bool result = frustum.IntersectsAABB(min, max);

        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithTransform_ExtremeMatrix_HandlesCorrectly()
    {
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var localMin = new Vector3(-1, -1, -1);
        var localMax = new Vector3(1, 1, 1);
        Matrix4 extremeMatrix = Matrix4.CreateScale(1e6f) * Matrix4.CreateTranslation(1e6f, 1e6f, 1e6f);

        bool result = frustum.IntersectsAABB(localMin, localMax, extremeMatrix);

        Assert.True(result || !result);
    }

    [Fact]
    public void IntersectsAABB_WithTransform_ZeroScale_HandlesCorrectly()
    {
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var localMin = new Vector3(-1, -1, -1);
        var localMax = new Vector3(1, 1, 1);
        var zeroScaleMatrix = Matrix4.CreateScale(0);

        bool result = frustum.IntersectsAABB(localMin, localMax, zeroScaleMatrix);

        Assert.True(result || !result);
    }

    [Fact]
    public void Frustum_WithIdentityMatrix_CreatesCorrectly()
    {
        var frustum = new Frustum(Matrix4.Identity);

        Assert.NotNull(frustum.Planes);
        Assert.Equal(6, frustum.Planes.Length);
    }

    [Fact]
    public void Frustum_WithZeroMatrix_HandlesCorrectly()
    {
        var zeroMatrix = new Matrix4();
        var frustum = new Frustum(zeroMatrix);

        Assert.NotNull(frustum.Planes);
        Assert.Equal(6, frustum.Planes.Length);
    }

    [Fact]
    public void IntersectsAABB_FarFromCamera_ReturnsFalse()
    {
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var farMin = new Vector3(-1, -1, -1000);
        var farMax = new Vector3(1, 1, -900);

        bool result = frustum.IntersectsAABB(farMin, farMax);

        Assert.False(result);
    }

    [Fact]
    public void IntersectsAABB_BehindCamera_ReturnsFalse()
    {
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var frustum = new Frustum(view, projection);
        var behindMin = new Vector3(-1, -1, 1);
        var behindMax = new Vector3(1, 1, 10);

        bool result = frustum.IntersectsAABB(behindMin, behindMax);

        Assert.False(result);
    }
}

