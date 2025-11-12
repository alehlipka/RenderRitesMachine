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
        ITimeService timeService = Mock.Of<ITimeService>();
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        ISceneManager sceneManager = Mock.Of<ISceneManager>();
        ILogger logger = Mock.Of<ILogger>();

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
        var camera = new PerspectiveCamera();
        ITimeService timeService = Mock.Of<ITimeService>();
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        ISceneManager sceneManager = Mock.Of<ISceneManager>();
        ILogger logger = Mock.Of<ILogger>();

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
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shaderId = 1;

        shared.MarkShaderActive(shaderId);
        shared.MarkShaderActive(shaderId);

        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Once);

        shared.ClearActiveShaders();
        shared.MarkShaderActive(shaderId);

        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Exactly(2));
    }

    [Fact]
    public void ClearActiveShaders_ClearsActiveShadersSet()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.MarkShaderActive(1);
        shared.MarkShaderActive(2);
        shared.MarkShaderActive(3);

        shared.ClearActiveShaders();

        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UseProgram(1), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraMatricesUnchanged_DoesNotUpdate()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Position = new Vector3(0, 0, 10);
        shared.Camera.AspectRatio = 16.0f / 9.0f;
        shared.Camera.Fov = 60.0f;

        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), It.IsAny<bool>(), ref It.Ref<Matrix4>.IsAny), Times.Never);
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraPositionChanges_UpdatesMatrices()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Position = new Vector3(0, 0, 10);
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        shared.Camera.Position = new Vector3(10, 20, 30);
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraAspectRatioChanges_UpdatesMatrices()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.AspectRatio = 16.0f / 9.0f;
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        shared.Camera.AspectRatio = 4.0f / 3.0f;
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WhenCameraFovChanges_UpdatesMatrices()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Fov = 60.0f;
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        shared.Camera.Fov = 75.0f;
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2));
    }

    [Fact]
    public void Window_CanBeSet()
    {
        SystemSharedObject shared = CreateSystemSharedObject();
        Window? window = null;

        shared.Window = window;

        Assert.Null(shared.Window);
    }

    [Fact]
    public void MarkShaderActive_MultipleShaders_AllAreTracked()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shader1 = 1;
        const int shader2 = 2;
        const int shader3 = 3;

        shared.MarkShaderActive(shader1);
        shared.MarkShaderActive(shader2);
        shared.MarkShaderActive(shader3);
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UseProgram(shader1), Times.AtLeastOnce);
        mockOpenGL.Verify(g => g.UseProgram(shader2), Times.AtLeastOnce);
        mockOpenGL.Verify(g => g.UseProgram(shader3), Times.AtLeastOnce);

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), It.IsAny<bool>(), ref It.Ref<Matrix4>.IsAny), Times.AtLeast(6));
    }

    [Fact]
    public void MarkShaderActive_WithNegativeId_Works()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int negativeShaderId = -1;

        shared.MarkShaderActive(negativeShaderId);

        mockOpenGL.Verify(g => g.UseProgram(negativeShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActive_WithZeroId_Works()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int zeroShaderId = 0;

        shared.MarkShaderActive(zeroShaderId);

        mockOpenGL.Verify(g => g.UseProgram(zeroShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActive_WithVeryLargeId_Works()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int largeShaderId = int.MaxValue;

        shared.MarkShaderActive(largeShaderId);

        mockOpenGL.Verify(g => g.UseProgram(largeShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActive_ManyShaders_AllAreTracked()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);

        for (int i = 0; i < 1000; i++)
        {
            shared.MarkShaderActive(i);
        }

        shared.UpdateActiveShaders();
        mockOpenGL.Verify(g => g.UseProgram(It.IsAny<int>()), Times.AtLeast(1000));
    }

    [Fact]
    public void ClearActiveShaders_WithManyShaders_ClearsAll()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);

        for (int i = 0; i < 100; i++)
        {
            shared.MarkShaderActive(i);
        }

        shared.ClearActiveShaders();

        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UseProgram(1), Times.AtLeast(2));
    }

    [Fact]
    public void UpdateActiveShaders_WithNoActiveShaders_DoesNotCrash()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);

        shared.UpdateActiveShaders();

        Assert.True(true);
    }

    [Fact]
    public void UpdateActiveShaders_MultipleTimes_IsConsistent()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.MarkShaderActive(1);

        for (int i = 0; i < 100; i++)
        {
            shared.UpdateActiveShaders();
        }

        Assert.True(true);
    }

    [Fact]
    public void MarkShaderActive_SameShaderMultipleTimes_OnlyCallsUseProgramOnce()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shaderId = 5;

        for (int i = 0; i < 100; i++)
        {
            shared.MarkShaderActive(shaderId);
        }

        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Once);
    }
}

