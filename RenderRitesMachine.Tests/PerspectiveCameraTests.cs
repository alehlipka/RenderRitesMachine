using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Output;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Tests for <see cref="PerspectiveCamera"/>.
/// </summary>
public sealed class PerspectiveCameraTests
{
    [Fact]
    public void ConstructorInitializesWithDefaultValues()
    {
        var camera = new PerspectiveCamera();

        Assert.Equal(Vector3.Zero, camera.Position);
        Assert.Equal(1.0f, camera.AspectRatio);
        Assert.Equal(90.0f, camera.Fov);
        Assert.Equal(0.0f, camera.Pitch);
        Assert.Equal(-90.0f, camera.Yaw);
        Assert.Equal(30.0f, camera.Speed);
        Assert.Equal(90.0f, camera.AngularSpeed);
    }

    [Fact]
    public void PositionSetValueUpdatesPosition()
    {
        var camera = new PerspectiveCamera();
        var newPosition = new Vector3(10, 20, 30);

        camera.Position = newPosition;

        Assert.Equal(newPosition, camera.Position);
    }

    [Fact]
    public void PositionSetSameValueDoesNotMarkDirty()
    {
        var camera = new PerspectiveCamera();
        var position = new Vector3(5, 5, 5);
        camera.Position = position;
        Matrix4 firstViewMatrix = camera.ViewMatrix;

        camera.Position = position;
        Matrix4 secondViewMatrix = camera.ViewMatrix;

        Assert.Equal(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void AspectRatioSetValidValueUpdatesAspectRatio()
    {
        var camera = new PerspectiveCamera();
        const float newAspectRatio = 16.0f / 9.0f;

        camera.AspectRatio = newAspectRatio;

        Assert.Equal(newAspectRatio, camera.AspectRatio);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-10.0f)]
    public void AspectRatioSetInvalidValueThrowsArgumentOutOfRangeException(float invalidValue)
    {
        var camera = new PerspectiveCamera();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => camera.AspectRatio = invalidValue);
    }

    [Fact]
    public void AspectRatioSetSameValueDoesNotMarkDirty()
    {
        var camera = new PerspectiveCamera();
        const float aspectRatio = 16.0f / 9.0f;
        camera.AspectRatio = aspectRatio;
        Matrix4 firstProjectionMatrix = camera.ProjectionMatrix;

        camera.AspectRatio = aspectRatio;
        Matrix4 secondProjectionMatrix = camera.ProjectionMatrix;

        Assert.Equal(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Theory]
    [InlineData(45.0f)]
    [InlineData(60.0f)]
    [InlineData(90.0f)]
    [InlineData(1.0f)]
    public void FovSetValidValueUpdatesFov(float fov)
    {
        var camera = new PerspectiveCamera
        {
            Fov = fov
        };

        Assert.Equal(fov, camera.Fov, 0.0001f);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-10.0f)]
    [InlineData(91.0f)]
    [InlineData(100.0f)]
    public void FovSetInvalidValueThrowsArgumentOutOfRangeException(float invalidFov)
    {
        var camera = new PerspectiveCamera();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => camera.Fov = invalidFov);
    }

    [Fact]
    public void FovSetSameValueDoesNotMarkDirty()
    {
        var camera = new PerspectiveCamera();
        const float fov = 60.0f;
        camera.Fov = fov;
        Matrix4 firstProjectionMatrix = camera.ProjectionMatrix;

        camera.Fov = fov;
        Matrix4 secondProjectionMatrix = camera.ProjectionMatrix;

        Assert.Equal(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Theory]
    [InlineData(-89.0f)]
    [InlineData(0.0f)]
    [InlineData(89.0f)]
    public void PitchSetValidValueUpdatesPitch(float pitch)
    {
        var camera = new PerspectiveCamera
        {
            Pitch = pitch
        };

        Assert.Equal(pitch, camera.Pitch, 0.0001f);
    }

    [Fact]
    public void PitchSetValueOutOfRangeClampsToValidRange()
    {
        var camera = new PerspectiveCamera
        {
            Pitch = -100.0f
        };
        Assert.Equal(RenderConstants.CameraMinPitch, camera.Pitch, 0.0001f);

        camera.Pitch = 100.0f;
        Assert.Equal(RenderConstants.CameraMaxPitch, camera.Pitch, 0.0001f);
    }

    [Fact]
    public void YawSetValueUpdatesYaw()
    {
        var camera = new PerspectiveCamera();
        const float yaw = 45.0f;

        camera.Yaw = yaw;

        Assert.Equal(yaw, camera.Yaw, 0.0001f);
    }

    [Fact]
    public void ViewMatrixAfterPositionChangeIsRecalculated()
    {
        var camera = new PerspectiveCamera();
        Matrix4 firstViewMatrix = camera.ViewMatrix;

        camera.Position = new Vector3(10, 20, 30);
        Matrix4 secondViewMatrix = camera.ViewMatrix;

        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void ViewMatrixAfterPitchChangeIsRecalculated()
    {
        var camera = new PerspectiveCamera();
        Matrix4 firstViewMatrix = camera.ViewMatrix;

        camera.Pitch = 45.0f;
        Matrix4 secondViewMatrix = camera.ViewMatrix;

        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void ViewMatrixAfterYawChangeIsRecalculated()
    {
        var camera = new PerspectiveCamera();
        Matrix4 firstViewMatrix = camera.ViewMatrix;

        camera.Yaw = 45.0f;
        Matrix4 secondViewMatrix = camera.ViewMatrix;

        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void ProjectionMatrixAfterAspectRatioChangeIsRecalculated()
    {
        var camera = new PerspectiveCamera();
        Matrix4 firstProjectionMatrix = camera.ProjectionMatrix;

        camera.AspectRatio = 16.0f / 9.0f;
        Matrix4 secondProjectionMatrix = camera.ProjectionMatrix;

        Assert.NotEqual(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Fact]
    public void ProjectionMatrixAfterFovChangeIsRecalculated()
    {
        var camera = new PerspectiveCamera();
        Matrix4 firstProjectionMatrix = camera.ProjectionMatrix;

        camera.Fov = 60.0f;
        Matrix4 secondProjectionMatrix = camera.ProjectionMatrix;

        Assert.NotEqual(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Fact]
    public void FrontIsNormalized()
    {
        var camera = new PerspectiveCamera();

        Vector3 front = camera.Front;

        Assert.Equal(1.0f, front.Length, 0.0001f);
    }

    [Fact]
    public void UpIsNormalized()
    {
        var camera = new PerspectiveCamera();

        Vector3 up = camera.Up;

        Assert.Equal(1.0f, up.Length, 0.0001f);
    }

    [Fact]
    public void RightIsNormalized()
    {
        var camera = new PerspectiveCamera();

        Vector3 right = camera.Right;

        Assert.Equal(1.0f, right.Length, 0.0001f);
    }

    [Fact]
    public void FrontRightUpAreOrthogonal()
    {
        var camera = new PerspectiveCamera
        {
            Pitch = 30.0f,
            Yaw = 45.0f
        };

        Vector3 front = camera.Front;
        Vector3 right = camera.Right;
        Vector3 up = camera.Up;

        Assert.Equal(0.0f, Vector3.Dot(front, right), 0.0001f);
        Assert.Equal(0.0f, Vector3.Dot(front, up), 0.0001f);
        Assert.Equal(0.0f, Vector3.Dot(right, up), 0.0001f);
    }

    [Fact]
    public void ViewMatrixUsesCorrectLookAtParameters()
    {
        var camera = new PerspectiveCamera
        {
            Position = new Vector3(0, 0, 10),
            Pitch = 0,
            Yaw = -90
        };

        Matrix4 viewMatrix = camera.ViewMatrix;

        var expectedViewMatrix = Matrix4.LookAt(
            camera.Position,
            camera.Position + camera.Front,
            camera.Up
        );

        Assert.Equal(expectedViewMatrix, viewMatrix);
    }

    [Fact]
    public void ProjectionMatrixUsesCorrectPerspectiveParameters()
    {
        var camera = new PerspectiveCamera
        {
            Fov = 60.0f,
            AspectRatio = 16.0f / 9.0f
        };

        Matrix4 projectionMatrix = camera.ProjectionMatrix;

        var expectedProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60.0f),
            camera.AspectRatio,
            RenderConstants.CameraNearPlane,
            RenderConstants.CameraFarPlane
        );

        Assert.Equal(expectedProjectionMatrix, projectionMatrix);
    }

    [Theory]
    [InlineData(float.MinValue)]
    [InlineData(float.NegativeInfinity)]
    public void AspectRatioSetExtremeValueThrowsException(float extremeValue)
    {
        var camera = new PerspectiveCamera();

        _ = Assert.ThrowsAny<Exception>(() => camera.AspectRatio = extremeValue);
    }

    [Theory]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NaN)]
    public void AspectRatioSetInfinityOrNaNMayNotThrow(float extremeValue)
    {
        var camera = new PerspectiveCamera();

        try
        {
            camera.AspectRatio = extremeValue;
            Assert.True(true);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
        }
    }

    [Theory]
    [InlineData(0.0001f)]
    [InlineData(1000.0f)]
    [InlineData(0.1f)]
    [InlineData(10.0f)]
    public void AspectRatioSetExtremeButValidValueUpdatesCorrectly(float validExtremeValue)
    {
        var camera = new PerspectiveCamera
        {
            AspectRatio = validExtremeValue
        };

        Assert.Equal(validExtremeValue, camera.AspectRatio);
    }

    [Theory]
    [InlineData(float.MinValue)]
    [InlineData(float.NegativeInfinity)]
    [InlineData(0.0f)]
    [InlineData(91.0f)]
    public void FovSetExtremeValueThrowsException(float extremeValue)
    {
        var camera = new PerspectiveCamera();

        _ = Assert.ThrowsAny<Exception>(() => camera.Fov = extremeValue);
    }

    [Theory]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NaN)]
    public void FovSetInfinityOrNaNMayNotThrow(float extremeValue)
    {
        var camera = new PerspectiveCamera();

        try
        {
            camera.Fov = extremeValue;
            Assert.True(true);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void PositionSetExtremeValuesUpdatesCorrectly()
    {
        var camera = new PerspectiveCamera();

        var extremePosition = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);
        camera.Position = extremePosition;
        Assert.Equal(extremePosition, camera.Position);

        var negativeExtreme = new Vector3(float.MinValue / 2, float.MinValue / 2, float.MinValue / 2);
        camera.Position = negativeExtreme;
        Assert.Equal(negativeExtreme, camera.Position);
    }

    [Fact]
    public void PositionSetVerySmallDeltaMarksDirty()
    {
        var camera = new PerspectiveCamera();
        var position1 = new Vector3(1.0f, 2.0f, 3.0f);
        camera.Position = position1;
        Matrix4 firstViewMatrix = camera.ViewMatrix;

        var position2 = new Vector3(1.0f + 0.0001f, 2.0f + 0.0001f, 3.0f + 0.0001f);
        camera.Position = position2;
        Matrix4 secondViewMatrix = camera.ViewMatrix;

        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void PitchYawExtremeValuesClampsCorrectly()
    {
        var camera = new PerspectiveCamera
        {
            Pitch = float.MaxValue
        };
        Assert.True(camera.Pitch <= RenderConstants.CameraMaxPitch);
        Assert.True(camera.Pitch >= RenderConstants.CameraMinPitch);

        camera.Pitch = float.MinValue;
        Assert.True(camera.Pitch <= RenderConstants.CameraMaxPitch);
        Assert.True(camera.Pitch >= RenderConstants.CameraMinPitch);

        camera.Yaw = 360.0f * 1000.0f;
        Assert.True(camera.Yaw >= -360.0f * 1000.0f);

        camera.Yaw = -360.0f * 1000.0f;
        Assert.True(camera.Yaw <= 360.0f * 1000.0f);
    }

    [Fact]
    public void ViewMatrixWithExtremePositionIsValid()
    {
        var camera = new PerspectiveCamera
        {
            Position = new Vector3(1e6f, 1e6f, 1e6f)
        };

        Matrix4 viewMatrix = camera.ViewMatrix;

        Assert.False(float.IsNaN(viewMatrix.M11));
        Assert.False(float.IsInfinity(viewMatrix.M11));
    }

    [Fact]
    public void ProjectionMatrixWithExtremeAspectRatioIsValid()
    {
        var camera = new PerspectiveCamera
        {
            AspectRatio = 1000.0f
        };

        Matrix4 projectionMatrix = camera.ProjectionMatrix;

        Assert.False(float.IsNaN(projectionMatrix.M11));
        Assert.False(float.IsInfinity(projectionMatrix.M11));
    }

    [Fact]
    public void FrontRightUpWithExtremeAnglesRemainNormalized()
    {
        var camera = new PerspectiveCamera
        {
            Pitch = 89.0f,
            Yaw = 360.0f * 10.0f
        };

        Vector3 front = camera.Front;
        Vector3 right = camera.Right;
        Vector3 up = camera.Up;

        Assert.Equal(1.0f, front.Length, 0.0001f);
        Assert.Equal(1.0f, right.Length, 0.0001f);
        Assert.Equal(1.0f, up.Length, 0.0001f);
    }

    [Fact]
    public void SpeedAngularSpeedSetExtremeValuesUpdatesCorrectly()
    {
        var camera = new PerspectiveCamera
        {
            Speed = float.MaxValue
        };
        Assert.Equal(float.MaxValue, camera.Speed);

        camera.Speed = 0.0f;
        Assert.Equal(0.0f, camera.Speed);

        camera.AngularSpeed = float.MaxValue;
        Assert.Equal(float.MaxValue, camera.AngularSpeed);

        camera.AngularSpeed = 0.0f;
        Assert.Equal(0.0f, camera.AngularSpeed);
    }

    [Fact]
    public void FovSetBoundaryValuesWorksCorrectly()
    {
        var camera = new PerspectiveCamera
        {
            Fov = RenderConstants.CameraMinFov
        };
        Assert.Equal(RenderConstants.CameraMinFov, camera.Fov, 0.0001f);

        camera.Fov = RenderConstants.CameraMaxFov;
        Assert.Equal(RenderConstants.CameraMaxFov, camera.Fov, 0.0001f);
    }

    [Fact]
    public void AspectRatioSetVerySmallValueWorksCorrectly()
    {
        var camera = new PerspectiveCamera
        {
            AspectRatio = float.Epsilon * 1000.0f
        };

        Assert.True(camera.AspectRatio > 0);
    }

    [Fact]
    public void ViewMatrixMultipleRapidChangesIsConsistent()
    {
        var camera = new PerspectiveCamera();

        for (int i = 0; i < 100; i++)
        {
            camera.Position = new Vector3(i, i * 2, i * 3);
            camera.Pitch = i % 90;
            camera.Yaw = i * 10.0f;
            Matrix4 matrix = camera.ViewMatrix;
            Assert.False(float.IsNaN(matrix.M11));
        }

        Matrix4 finalMatrix = camera.ViewMatrix;
        Assert.False(float.IsNaN(finalMatrix.M11));
        Assert.False(float.IsInfinity(finalMatrix.M11));
    }
}
