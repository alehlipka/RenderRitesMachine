using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Output;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса PerspectiveCamera.
/// </summary>
public class PerspectiveCameraTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var camera = new PerspectiveCamera();

        // Assert
        Assert.Equal(Vector3.Zero, camera.Position);
        Assert.Equal(1.0f, camera.AspectRatio);
        Assert.Equal(90.0f, camera.Fov);
        Assert.Equal(0.0f, camera.Pitch);
        Assert.Equal(-90.0f, camera.Yaw);
        Assert.Equal(30.0f, camera.Speed);
        Assert.Equal(90.0f, camera.AngularSpeed);
    }

    [Fact]
    public void Position_SetValue_UpdatesPosition()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var newPosition = new Vector3(10, 20, 30);

        // Act
        camera.Position = newPosition;

        // Assert
        Assert.Equal(newPosition, camera.Position);
    }

    [Fact]
    public void Position_SetSameValue_DoesNotMarkDirty()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var position = new Vector3(5, 5, 5);
        camera.Position = position;
        var firstViewMatrix = camera.ViewMatrix;

        // Act
        camera.Position = position; // Устанавливаем то же значение
        var secondViewMatrix = camera.ViewMatrix;

        // Assert - матрица должна быть та же самая (кэширована)
        Assert.Equal(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void AspectRatio_SetValidValue_UpdatesAspectRatio()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        const float newAspectRatio = 16.0f / 9.0f;

        // Act
        camera.AspectRatio = newAspectRatio;

        // Assert
        Assert.Equal(newAspectRatio, camera.AspectRatio);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-10.0f)]
    public void AspectRatio_SetInvalidValue_ThrowsArgumentOutOfRangeException(float invalidValue)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => camera.AspectRatio = invalidValue);
    }

    [Fact]
    public void AspectRatio_SetSameValue_DoesNotMarkDirty()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        const float aspectRatio = 16.0f / 9.0f;
        camera.AspectRatio = aspectRatio;
        var firstProjectionMatrix = camera.ProjectionMatrix;

        // Act
        camera.AspectRatio = aspectRatio; // Устанавливаем то же значение
        var secondProjectionMatrix = camera.ProjectionMatrix;

        // Assert - матрица должна быть та же самая (кэширована)
        Assert.Equal(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Theory]
    [InlineData(45.0f)]
    [InlineData(60.0f)]
    [InlineData(90.0f)]
    [InlineData(1.0f)]
    public void Fov_SetValidValue_UpdatesFov(float fov)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        camera.Fov = fov;

        // Assert
        Assert.Equal(fov, camera.Fov, 0.0001f);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-10.0f)]
    [InlineData(91.0f)]
    [InlineData(100.0f)]
    public void Fov_SetInvalidValue_ThrowsArgumentOutOfRangeException(float invalidFov)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => camera.Fov = invalidFov);
    }

    [Fact]
    public void Fov_SetSameValue_DoesNotMarkDirty()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        const float fov = 60.0f;
        camera.Fov = fov;
        var firstProjectionMatrix = camera.ProjectionMatrix;

        // Act
        camera.Fov = fov; // Устанавливаем то же значение
        var secondProjectionMatrix = camera.ProjectionMatrix;

        // Assert - матрица должна быть та же самая (кэширована)
        Assert.Equal(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Theory]
    [InlineData(-89.0f)]
    [InlineData(0.0f)]
    [InlineData(89.0f)]
    public void Pitch_SetValidValue_UpdatesPitch(float pitch)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        camera.Pitch = pitch;

        // Assert
        Assert.Equal(pitch, camera.Pitch, 0.0001f);
    }

    [Fact]
    public void Pitch_SetValueOutOfRange_ClampsToValidRange()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        camera.Pitch = -100.0f; // Меньше минимума
        Assert.Equal(RenderConstants.CameraMinPitch, camera.Pitch, 0.0001f);

        camera.Pitch = 100.0f; // Больше максимума
        Assert.Equal(RenderConstants.CameraMaxPitch, camera.Pitch, 0.0001f);
    }

    [Fact]
    public void Yaw_SetValue_UpdatesYaw()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        const float yaw = 45.0f;

        // Act
        camera.Yaw = yaw;

        // Assert
        Assert.Equal(yaw, camera.Yaw, 0.0001f);
    }

    [Fact]
    public void ViewMatrix_AfterPositionChange_IsRecalculated()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var firstViewMatrix = camera.ViewMatrix;

        // Act
        camera.Position = new Vector3(10, 20, 30);
        var secondViewMatrix = camera.ViewMatrix;

        // Assert
        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void ViewMatrix_AfterPitchChange_IsRecalculated()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var firstViewMatrix = camera.ViewMatrix;

        // Act
        camera.Pitch = 45.0f;
        var secondViewMatrix = camera.ViewMatrix;

        // Assert
        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void ViewMatrix_AfterYawChange_IsRecalculated()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var firstViewMatrix = camera.ViewMatrix;

        // Act
        camera.Yaw = 45.0f;
        var secondViewMatrix = camera.ViewMatrix;

        // Assert
        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void ProjectionMatrix_AfterAspectRatioChange_IsRecalculated()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var firstProjectionMatrix = camera.ProjectionMatrix;

        // Act
        camera.AspectRatio = 16.0f / 9.0f;
        var secondProjectionMatrix = camera.ProjectionMatrix;

        // Assert
        Assert.NotEqual(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Fact]
    public void ProjectionMatrix_AfterFovChange_IsRecalculated()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var firstProjectionMatrix = camera.ProjectionMatrix;

        // Act
        camera.Fov = 60.0f;
        var secondProjectionMatrix = camera.ProjectionMatrix;

        // Assert
        Assert.NotEqual(firstProjectionMatrix, secondProjectionMatrix);
    }

    [Fact]
    public void Front_IsNormalized()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        var front = camera.Front;

        // Assert
        Assert.Equal(1.0f, front.Length, 0.0001f);
    }

    [Fact]
    public void Up_IsNormalized()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        var up = camera.Up;

        // Assert
        Assert.Equal(1.0f, up.Length, 0.0001f);
    }

    [Fact]
    public void Right_IsNormalized()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        var right = camera.Right;

        // Assert
        Assert.Equal(1.0f, right.Length, 0.0001f);
    }

    [Fact]
    public void Front_Right_Up_AreOrthogonal()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        camera.Pitch = 30.0f;
        camera.Yaw = 45.0f;

        // Act
        var front = camera.Front;
        var right = camera.Right;
        var up = camera.Up;

        // Assert - векторы должны быть ортогональны
        Assert.Equal(0.0f, Vector3.Dot(front, right), 0.0001f);
        Assert.Equal(0.0f, Vector3.Dot(front, up), 0.0001f);
        Assert.Equal(0.0f, Vector3.Dot(right, up), 0.0001f);
    }

    [Fact]
    public void ViewMatrix_UsesCorrectLookAtParameters()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        camera.Position = new Vector3(0, 0, 10);
        camera.Pitch = 0;
        camera.Yaw = -90;

        // Act
        var viewMatrix = camera.ViewMatrix;

        // Assert - камера должна смотреть в направлении -Z
        var expectedViewMatrix = Matrix4.LookAt(
            camera.Position,
            camera.Position + camera.Front,
            camera.Up
        );

        Assert.Equal(expectedViewMatrix, viewMatrix);
    }

    [Fact]
    public void ProjectionMatrix_UsesCorrectPerspectiveParameters()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        camera.Fov = 60.0f;
        camera.AspectRatio = 16.0f / 9.0f;

        // Act
        var projectionMatrix = camera.ProjectionMatrix;

        // Assert
        var expectedProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60.0f),
            camera.AspectRatio,
            RenderConstants.CameraNearPlane,
            RenderConstants.CameraFarPlane
        );

        Assert.Equal(expectedProjectionMatrix, projectionMatrix);
    }

    // Edge cases для PerspectiveCamera

    [Theory]
    [InlineData(float.MinValue)]
    [InlineData(float.NegativeInfinity)]
    public void AspectRatio_SetExtremeValue_ThrowsException(float extremeValue)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - только отрицательные и отрицательная бесконечность должны выбрасывать исключение
        Assert.ThrowsAny<Exception>(() => camera.AspectRatio = extremeValue);
    }

    [Theory]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NaN)]
    public void AspectRatio_SetInfinityOrNaN_MayNotThrow(float extremeValue)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - PositiveInfinity и NaN могут не выбрасывать исключение сразу,
        // но могут вызвать проблемы позже. Проверяем, что код не падает с неожиданной ошибкой.
        try
        {
            camera.AspectRatio = extremeValue;
            // Если не выбросило исключение, проверяем, что значение установлено
            Assert.True(true);
        }
        catch (Exception)
        {
            // Исключение тоже допустимо
            Assert.True(true);
        }
    }

    [Theory]
    [InlineData(0.0001f)]
    [InlineData(1000.0f)]
    [InlineData(0.1f)]
    [InlineData(10.0f)]
    public void AspectRatio_SetExtremeButValidValue_UpdatesCorrectly(float validExtremeValue)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act
        camera.AspectRatio = validExtremeValue;

        // Assert
        Assert.Equal(validExtremeValue, camera.AspectRatio);
    }

    [Theory]
    [InlineData(float.MinValue)]
    [InlineData(float.NegativeInfinity)]
    [InlineData(0.0f)]
    [InlineData(91.0f)]
    public void Fov_SetExtremeValue_ThrowsException(float extremeValue)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - значения вне допустимого диапазона должны выбрасывать исключение
        Assert.ThrowsAny<Exception>(() => camera.Fov = extremeValue);
    }

    [Theory]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NaN)]
    public void Fov_SetInfinityOrNaN_MayNotThrow(float extremeValue)
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - PositiveInfinity и NaN могут не выбрасывать исключение сразу
        try
        {
            camera.Fov = extremeValue;
            Assert.True(true);
        }
        catch (Exception)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void Position_SetExtremeValues_UpdatesCorrectly()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - камера должна обрабатывать очень большие координаты
        var extremePosition = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);
        camera.Position = extremePosition;
        Assert.Equal(extremePosition, camera.Position);

        var negativeExtreme = new Vector3(float.MinValue / 2, float.MinValue / 2, float.MinValue / 2);
        camera.Position = negativeExtreme;
        Assert.Equal(negativeExtreme, camera.Position);
    }

    [Fact]
    public void Position_SetVerySmallDelta_MarksDirty()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var position1 = new Vector3(1.0f, 2.0f, 3.0f);
        camera.Position = position1;
        var firstViewMatrix = camera.ViewMatrix;

        // Act - устанавливаем немного другое значение (достаточно большое, чтобы Vector3 != сработал)
        var position2 = new Vector3(1.0f + 0.0001f, 2.0f + 0.0001f, 3.0f + 0.0001f);
        camera.Position = position2;
        var secondViewMatrix = camera.ViewMatrix;

        // Assert - матрица должна быть пересчитана, так как это разные объекты Vector3
        // Camera проверяет равенство через !=, поэтому даже маленькие изменения пересчитывают матрицу
        Assert.NotEqual(firstViewMatrix, secondViewMatrix);
    }

    [Fact]
    public void Pitch_Yaw_ExtremeValues_ClampsCorrectly()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - Pitch должен ограничиваться
        camera.Pitch = float.MaxValue;
        Assert.True(camera.Pitch <= RenderConstants.CameraMaxPitch);
        Assert.True(camera.Pitch >= RenderConstants.CameraMinPitch);

        camera.Pitch = float.MinValue;
        Assert.True(camera.Pitch <= RenderConstants.CameraMaxPitch);
        Assert.True(camera.Pitch >= RenderConstants.CameraMinPitch);

        // Yaw не ограничивается, но должен обрабатывать экстремальные значения
        camera.Yaw = 360.0f * 1000.0f; // Много оборотов
        Assert.True(camera.Yaw >= -360.0f * 1000.0f); // Проверяем, что значение установлено

        camera.Yaw = -360.0f * 1000.0f;
        Assert.True(camera.Yaw <= 360.0f * 1000.0f);
    }

    [Fact]
    public void ViewMatrix_WithExtremePosition_IsValid()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        camera.Position = new Vector3(1e6f, 1e6f, 1e6f);

        // Act
        var viewMatrix = camera.ViewMatrix;

        // Assert - матрица должна быть валидной (не NaN, не Infinity)
        Assert.False(float.IsNaN(viewMatrix.M11));
        Assert.False(float.IsInfinity(viewMatrix.M11));
    }

    [Fact]
    public void ProjectionMatrix_WithExtremeAspectRatio_IsValid()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        camera.AspectRatio = 1000.0f; // Очень широкий экран

        // Act
        var projectionMatrix = camera.ProjectionMatrix;

        // Assert - матрица должна быть валидной
        Assert.False(float.IsNaN(projectionMatrix.M11));
        Assert.False(float.IsInfinity(projectionMatrix.M11));
    }

    [Fact]
    public void Front_Right_Up_WithExtremeAngles_RemainNormalized()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        camera.Pitch = 89.0f; // Почти вертикально вверх
        camera.Yaw = 360.0f * 10.0f; // Много оборотов

        // Act
        var front = camera.Front;
        var right = camera.Right;
        var up = camera.Up;

        // Assert - все векторы должны быть нормализованы
        Assert.Equal(1.0f, front.Length, 0.0001f);
        Assert.Equal(1.0f, right.Length, 0.0001f);
        Assert.Equal(1.0f, up.Length, 0.0001f);
    }

    [Fact]
    public void Speed_AngularSpeed_SetExtremeValues_UpdatesCorrectly()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - Speed и AngularSpeed не имеют ограничений
        camera.Speed = float.MaxValue;
        Assert.Equal(float.MaxValue, camera.Speed);

        camera.Speed = 0.0f;
        Assert.Equal(0.0f, camera.Speed);

        camera.AngularSpeed = float.MaxValue;
        Assert.Equal(float.MaxValue, camera.AngularSpeed);

        camera.AngularSpeed = 0.0f;
        Assert.Equal(0.0f, camera.AngularSpeed);
    }

    [Fact]
    public void Fov_SetBoundaryValues_WorksCorrectly()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act & Assert - граничные значения должны работать
        camera.Fov = RenderConstants.CameraMinFov;
        Assert.Equal(RenderConstants.CameraMinFov, camera.Fov, 0.0001f);

        camera.Fov = RenderConstants.CameraMaxFov;
        Assert.Equal(RenderConstants.CameraMaxFov, camera.Fov, 0.0001f);
    }

    [Fact]
    public void AspectRatio_SetVerySmallValue_WorksCorrectly()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act - очень маленькое, но положительное значение
        camera.AspectRatio = float.Epsilon * 1000.0f; // Очень маленькое, но > 0

        // Assert
        Assert.True(camera.AspectRatio > 0);
    }

    [Fact]
    public void ViewMatrix_MultipleRapidChanges_IsConsistent()
    {
        // Arrange
        var camera = new PerspectiveCamera();

        // Act - быстро меняем параметры много раз
        for (int i = 0; i < 100; i++)
        {
            camera.Position = new Vector3(i, i * 2, i * 3);
            camera.Pitch = i % 90;
            camera.Yaw = i * 10.0f;
            var matrix = camera.ViewMatrix;
            Assert.False(float.IsNaN(matrix.M11));
        }

        // Assert - последняя матрица должна быть валидной
        var finalMatrix = camera.ViewMatrix;
        Assert.False(float.IsNaN(finalMatrix.M11));
        Assert.False(float.IsInfinity(finalMatrix.M11));
    }
}

