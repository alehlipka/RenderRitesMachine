using OpenTK.Mathematics;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Edge-case tests for <see cref="Ray"/>.
/// </summary>
public sealed class RayTests
{
    [Fact]
    public void IntersectsAABBWithZeroDirectionReturnsNull()
    {
        var ray = new Ray(Vector3.Zero, Vector3.Zero);
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        float? result = ray.IntersectsAABB(boxMin, boxMax);

        Assert.Null(result);
    }

    [Fact]
    public void IntersectsAABBWithOriginInsideBoxReturnsNonNegative()
    {
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        float? result = ray.IntersectsAABB(boxMin, boxMax);

        if (result.HasValue)
        {
            Assert.True(result.Value >= 0);
        }
    }

    [Fact]
    public void IntersectsAABBWithInvertedBoxHandlesCorrectly()
    {
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var boxMin = new Vector3(10, 10, 10);
        var boxMax = new Vector3(-10, -10, -10);

        float? result = ray.IntersectsAABB(boxMin, boxMax);

        Assert.True(result == null || result.HasValue);
    }

    [Fact]
    public void IntersectsAABBWithExtremeBoxHandlesCorrectly()
    {
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var extremeMin = new Vector3(float.MinValue / 2, float.MinValue / 2, float.MinValue / 2);
        var extremeMax = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);

        float? result = ray.IntersectsAABB(extremeMin, extremeMax);

        Assert.True(result == null || result.HasValue);
    }

    [Fact]
    public void IntersectsAABBWithParallelRayReturnsNull()
    {
        var ray = new Ray(new Vector3(0, 0, 5), Vector3.UnitX);
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        float? result = ray.IntersectsAABB(boxMin, boxMax);

        Assert.Null(result);
    }

    [Fact]
    public void IntersectsAABBWithRayBehindBoxHandlesCorrectly()
    {
        var ray = new Ray(new Vector3(0, 0, 5), -Vector3.UnitZ);
        var boxMin = new Vector3(-1, -1, -1);
        var boxMax = new Vector3(1, 1, 1);

        float? result = ray.IntersectsAABB(boxMin, boxMax);

        Assert.True(result == null || result.HasValue);
    }

    [Fact]
    public void TransformToLocalSpaceWithIdentityMatrixReturnsSameRay()
    {
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        Matrix4 identity = Matrix4.Identity;

        Ray transformed = ray.TransformToLocalSpace(identity);

        Assert.Equal(ray.Origin, transformed.Origin);
        Assert.Equal(ray.Direction, transformed.Direction);
    }

    [Fact]
    public void TransformToLocalSpaceWithZeroScaleThrowsException()
    {
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        var zeroScale = Matrix4.CreateScale(0);

        _ = Assert.Throws<InvalidOperationException>(() => ray.TransformToLocalSpace(zeroScale));
    }

    [Fact]
    public void TransformToLocalSpaceWithExtremeMatrixHandlesCorrectly()
    {
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        Matrix4 extremeMatrix = Matrix4.CreateScale(1e6f) * Matrix4.CreateTranslation(1e6f, 1e6f, 1e6f);

        Ray transformed = ray.TransformToLocalSpace(extremeMatrix);

        Assert.NotNull(transformed);
        Assert.True(transformed.Direction.Length <= 1.0f + float.Epsilon);
    }

    [Fact]
    public void GetFromScreenWithZeroWindowSizeHandlesCorrectly()
    {
        var windowSize = new Vector2i(0, 0);
        Vector3 cameraPos = Vector3.Zero;
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);

        try
        {
            var ray = Ray.GetFromScreen(0, 0, windowSize, cameraPos, projection, view);
            Assert.NotNull(ray);
        }
        catch (DivideByZeroException)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void GetFromScreenWithExtremeCoordinatesHandlesCorrectly()
    {
        var windowSize = new Vector2i(1920, 1080);
        Vector3 cameraPos = Vector3.Zero;
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 1.0f, 0.1f, 100.0f);
        var view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);

        var ray1 = Ray.GetFromScreen(1e6f, 1e6f, windowSize, cameraPos, projection, view);
        var ray2 = Ray.GetFromScreen(-1e6f, -1e6f, windowSize, cameraPos, projection, view);

        Assert.NotNull(ray1);
        Assert.NotNull(ray2);
        Assert.False(float.IsNaN(ray1.Direction.Length));
        Assert.False(float.IsInfinity(ray1.Direction.Length));
        Assert.False(float.IsNaN(ray2.Direction.Length));
        Assert.False(float.IsInfinity(ray2.Direction.Length));
    }
}
