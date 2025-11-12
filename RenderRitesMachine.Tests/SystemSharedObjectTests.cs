using Moq;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса SystemSharedObject.
/// </summary>
public class SystemSharedObjectTests
{
    private SystemSharedObject CreateSystemSharedObject(IOpenGLWrapper? openGLWrapper = null)
    {
        var camera = new PerspectiveCamera();
        var timeService = Mock.Of<ITimeService>();
        var assetsService = Mock.Of<IAssetsService>();
        var renderService = Mock.Of<IRenderService>();
        var guiService = Mock.Of<IGuiService>();
        var audioService = Mock.Of<IAudioService>();
        var sceneManager = Mock.Of<ISceneManager>();
        var logger = Mock.Of<ILogger>();

        return new SystemSharedObject(
            camera,
            timeService,
            assetsService,
            renderService,
            guiService,
            audioService,
            sceneManager,
            logger,
            openGLWrapper
        );
    }

    [Fact]
    public void Constructor_InitializesWithProvidedServices()
    {
        // Arrange
        var camera = new PerspectiveCamera();
        var timeService = Mock.Of<ITimeService>();
        var assetsService = Mock.Of<IAssetsService>();
        var renderService = Mock.Of<IRenderService>();
        var guiService = Mock.Of<IGuiService>();
        var audioService = Mock.Of<IAudioService>();
        var sceneManager = Mock.Of<ISceneManager>();
        var logger = Mock.Of<ILogger>();

        // Act
        var shared = new SystemSharedObject(
            camera,
            timeService,
            assetsService,
            renderService,
            guiService,
            audioService,
            sceneManager,
            logger
        );

        // Assert
        Assert.Same(camera, shared.Camera);
        Assert.Same(timeService, shared.Time);
        Assert.Same(assetsService, shared.Assets);
        Assert.Same(renderService, shared.Render);
        Assert.Same(guiService, shared.Gui);
        Assert.Same(sceneManager, shared.SceneManager);
        Assert.Same(logger, shared.Logger);
        Assert.Null(shared.Window);
    }

    [Fact]
    public void MarkShaderActive_AddsShaderToActiveSet()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shaderId = 1;

        // Act
        shared.MarkShaderActive(shaderId);
        shared.MarkShaderActive(shaderId); // Дубликат не должен добавиться

        // Assert - UseProgram должен быть вызван только один раз (при первом добавлении)
        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Once);

        // Очищаем и добавляем снова
        shared.ClearActiveShaders();
        shared.MarkShaderActive(shaderId);

        // Теперь UseProgram должен быть вызван еще раз
        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Exactly(2));
    }

    [Fact]
    public void ClearActiveShaders_ClearsActiveShadersSet()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.MarkShaderActive(1);
        shared.MarkShaderActive(2);
        shared.MarkShaderActive(3);

        // Act
        shared.ClearActiveShaders();

        // Assert - после очистки новые шейдеры должны добавляться заново
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        // UseProgram должен быть вызван минимум 2 раза для шейдера 1
        // (первый раз при первом добавлении, второй раз после очистки)
        mockOpenGL.Verify(g => g.UseProgram(1), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraMatricesUnchanged_DoesNotUpdate()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Position = new Vector3(0, 0, 10);
        shared.Camera.AspectRatio = 16.0f / 9.0f;
        shared.Camera.Fov = 60.0f;

        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        // Сбрасываем счетчик вызовов
        mockOpenGL.Reset();

        // Act - не меняем камеру
        shared.UpdateActiveShaders();

        // Assert - UniformMatrix4 не должен быть вызван, так как матрицы не изменились
        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), It.IsAny<bool>(), ref It.Ref<Matrix4>.IsAny), Times.Never);
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraPositionChanges_UpdatesMatrices()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Position = new Vector3(0, 0, 10);
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        // Act
        shared.Camera.Position = new Vector3(10, 20, 30);
        shared.UpdateActiveShaders();

        // Assert - UniformMatrix4 должен быть вызван для обновления матриц
        // Проверяем, что метод был вызван с любыми параметрами (так как location может быть любым)
        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2)); // минимум 2 раза (view и projection)
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraAspectRatioChanges_UpdatesMatrices()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.AspectRatio = 16.0f / 9.0f;
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        // Act
        shared.Camera.AspectRatio = 4.0f / 3.0f;
        shared.UpdateActiveShaders();

        // Assert - UniformMatrix4 должен быть вызван для обновления матриц
        // При изменении AspectRatio обновляются обе матрицы (view и projection)
        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraFovChanges_UpdatesMatrices()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Fov = 60.0f;
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        // Act
        shared.Camera.Fov = 75.0f;
        shared.UpdateActiveShaders();

        // Assert - UniformMatrix4 должен быть вызван для обновления матриц
        // При изменении FOV обновляются обе матрицы (view и projection)
        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2));
    }

    [Fact]
    public void Window_CanBeSet()
    {
        // Arrange
        var shared = CreateSystemSharedObject();
        // Window - это конкретный класс, который нельзя мокировать напрямую
        // Но мы можем проверить, что свойство можно установить в null
        Window? window = null;

        // Act
        shared.Window = window;

        // Assert
        Assert.Null(shared.Window);

        // Для реального тестирования с Window нужен OpenGL контекст,
        // но базовая функциональность установки свойства работает
    }

    [Fact]
    public void MarkShaderActive_MultipleShaders_AllAreTracked()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shader1 = 1;
        const int shader2 = 2;
        const int shader3 = 3;

        // Act
        shared.MarkShaderActive(shader1);
        shared.MarkShaderActive(shader2);
        shared.MarkShaderActive(shader3);
        shared.UpdateActiveShaders();

        // Assert - все шейдеры должны быть обработаны
        mockOpenGL.Verify(g => g.UseProgram(shader1), Times.AtLeastOnce);
        mockOpenGL.Verify(g => g.UseProgram(shader2), Times.AtLeastOnce);
        mockOpenGL.Verify(g => g.UseProgram(shader3), Times.AtLeastOnce);

        // При UpdateActiveShaders все шейдеры должны получить обновление матриц
        // Минимум 6 вызовов (3 шейдера * 2 матрицы), но может быть больше из-за MarkShaderActive
        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), It.IsAny<bool>(), ref It.Ref<Matrix4>.IsAny), Times.AtLeast(6));
    }

    // Edge cases для SystemSharedObject

    [Fact]
    public void MarkShaderActive_WithNegativeId_Works()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int negativeShaderId = -1;

        // Act - OpenGL может использовать отрицательные ID для ошибок, но мы должны обработать это
        shared.MarkShaderActive(negativeShaderId);

        // Assert - UseProgram должен быть вызван (даже с отрицательным ID)
        mockOpenGL.Verify(g => g.UseProgram(negativeShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActive_WithZeroId_Works()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int zeroShaderId = 0;

        // Act
        shared.MarkShaderActive(zeroShaderId);

        // Assert
        mockOpenGL.Verify(g => g.UseProgram(zeroShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActive_WithVeryLargeId_Works()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int largeShaderId = int.MaxValue;

        // Act
        shared.MarkShaderActive(largeShaderId);

        // Assert
        mockOpenGL.Verify(g => g.UseProgram(largeShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActive_ManyShaders_AllAreTracked()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);

        // Act - добавляем много шейдеров
        for (int i = 0; i < 1000; i++)
        {
            shared.MarkShaderActive(i);
        }

        // Assert - все шейдеры должны быть добавлены
        shared.UpdateActiveShaders();
        mockOpenGL.Verify(g => g.UseProgram(It.IsAny<int>()), Times.AtLeast(1000));
    }

    [Fact]
    public void ClearActiveShaders_WithManyShaders_ClearsAll()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);

        // Добавляем много шейдеров
        for (int i = 0; i < 100; i++)
        {
            shared.MarkShaderActive(i);
        }

        // Act
        shared.ClearActiveShaders();

        // Assert - после очистки новые шейдеры должны добавляться заново
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        // UseProgram должен быть вызван минимум 2 раза для шейдера 1
        mockOpenGL.Verify(g => g.UseProgram(1), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WithNoActiveShaders_DoesNotCrash()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        var shared = CreateSystemSharedObject(mockOpenGL.Object);

        // Act - вызываем UpdateActiveShaders без активных шейдеров
        shared.UpdateActiveShaders();

        // Assert - не должно быть исключений
        Assert.True(true);
    }

    [Fact]
    public void UpdateActiveShaders_MultipleTimes_IsConsistent()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.MarkShaderActive(1);

        // Act - вызываем UpdateActiveShaders много раз
        for (int i = 0; i < 100; i++)
        {
            shared.UpdateActiveShaders();
        }

        // Assert - не должно быть исключений
        Assert.True(true);
    }

    [Fact]
    public void MarkShaderActive_SameShaderMultipleTimes_OnlyCallsUseProgramOnce()
    {
        // Arrange
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        var shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shaderId = 5;

        // Act - добавляем один и тот же шейдер много раз
        for (int i = 0; i < 100; i++)
        {
            shared.MarkShaderActive(shaderId);
        }

        // Assert - UseProgram должен быть вызван только один раз (при первом добавлении)
        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Once);
    }
}

