using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Output;

namespace RenderRitesMachine.Tests;

public sealed class OrthographicCameraTests
{
    [Fact]
    public void ConstructorInitializesDefaults()
    {
        var camera = new OrthographicCamera();

        Assert.Equal(Vector3.Zero, camera.Position);
        Assert.Equal(1.0f, camera.AspectRatio);
        Assert.Equal(10.0f, camera.Height, 0.0001f);
        Assert.Equal(10.0f, camera.Width, 0.0001f);
        Assert.Equal(0.0f, camera.Pitch);
        Assert.Equal(-90.0f, camera.Yaw);
    }

    [Fact]
    public void HeightSetValidValueUpdatesProjection()
    {
        var camera = new OrthographicCamera();
        Matrix4 original = camera.ProjectionMatrix;

        camera.Height = 14.0f;
        Matrix4 updated = camera.ProjectionMatrix;

        Assert.NotEqual(original, updated);
        Assert.Equal(14.0f, camera.Height, 0.0001f);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-10.0f)]
    public void HeightSetInvalidValueThrows(float invalidValue)
    {
        var camera = new OrthographicCamera();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => camera.Height = invalidValue);
    }

    [Fact]
    public void WidthReflectsAspectRatio()
    {
        var camera = new OrthographicCamera
        {
            AspectRatio = 16.0f / 9.0f,
            Height = 9.0f
        };

        Assert.Equal(16.0f, camera.Width, 0.0001f);
    }

    [Fact]
    public void ProjectionMatrixMatchesOpenTkImplementation()
    {
        var camera = new OrthographicCamera
        {
            AspectRatio = 4.0f / 3.0f,
            Height = 6.0f
        };

        Matrix4 expected = Matrix4.CreateOrthographic(camera.Width, camera.Height, RenderConstants.CameraNearPlane, RenderConstants.CameraFarPlane);

        Assert.Equal(expected, camera.ProjectionMatrix);
    }
}

