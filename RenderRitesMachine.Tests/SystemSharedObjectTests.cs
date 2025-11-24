using Moq;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса SystemSharedObject.
/// </summary>
public sealed class SystemSharedObjectTests
{
    private static SystemSharedObject CreateSystemSharedObject(IOpenGLWrapper? openGLWrapper = null)
    {
        var camera = new PerspectiveCamera();
        ITimeService timeService = Mock.Of<ITimeService>();
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        ISceneManager sceneManager = Mock.Of<ISceneManager>();
        ILogger logger = Mock.Of<ILogger>();

        return new SystemSharedObject(
            camera,
            timeService,
            assetsService,
            renderService,
            audioService,
            guiService,
            sceneManager,
            logger,
            openGLWrapper
        );
    }

    [Fact]
    public void ConstructorInitializesWithProvidedServices()
    {
        var camera = new PerspectiveCamera();
        ITimeService timeService = Mock.Of<ITimeService>();
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        ISceneManager sceneManager = Mock.Of<ISceneManager>();
        ILogger logger = Mock.Of<ILogger>();

        var shared = new SystemSharedObject(
            camera,
            timeService,
            assetsService,
            renderService,
            audioService,
            guiService,
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
    public void MarkShaderActiveAddsShaderToActiveSet()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

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
    public void ClearActiveShadersClearsActiveShadersSet()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

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
    public void UpdateActiveShadersWhenCameraMatricesUnchangedDoesNotUpdate()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.Camera.Position = new Vector3(0, 0, 10);
        shared.Camera.AspectRatio = 16.0f / 9.0f;
        var perspective = Assert.IsType<PerspectiveCamera>(shared.Camera);
        perspective.Fov = 60.0f;

        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), It.IsAny<bool>(), ref It.Ref<Matrix4>.IsAny), Times.Never);
    }

    [Fact]
    public void UpdateActiveShadersWhenCameraPositionChangesUpdatesMatrices()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

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
    public void UpdateActiveShadersWhenCameraAspectRatioChangesUpdatesMatrices()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

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
    public void UpdateActiveShadersWhenCameraFovChangesUpdatesMatrices()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        PerspectiveCamera perspective = Assert.IsType<PerspectiveCamera>(shared.Camera);
        perspective.Fov = 60.0f;
        shared.MarkShaderActive(1);
        shared.UpdateActiveShaders();

        mockOpenGL.Reset();

        perspective.Fov = 75.0f;
        shared.UpdateActiveShaders();

        mockOpenGL.Verify(g => g.UniformMatrix4(It.IsAny<int>(), true, ref It.Ref<Matrix4>.IsAny), Times.AtLeast(2));
    }

    [Fact]
    public void WindowCanBeSet()
    {
        SystemSharedObject shared = CreateSystemSharedObject();
        Window? window = null;

        shared.Window = window;

        Assert.Null(shared.Window);
    }

    [Fact]
    public void MarkShaderActiveMultipleShadersAllAreTracked()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

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
    public void MarkShaderActiveWithNegativeIdWorks()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int negativeShaderId = -1;

        shared.MarkShaderActive(negativeShaderId);

        mockOpenGL.Verify(g => g.UseProgram(negativeShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActiveWithZeroIdWorks()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int zeroShaderId = 0;

        shared.MarkShaderActive(zeroShaderId);

        mockOpenGL.Verify(g => g.UseProgram(zeroShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActiveWithVeryLargeIdWorks()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int largeShaderId = int.MaxValue;

        shared.MarkShaderActive(largeShaderId);

        mockOpenGL.Verify(g => g.UseProgram(largeShaderId), Times.Once);
    }

    [Fact]
    public void MarkShaderActiveManyShadersAllAreTracked()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);

        for (int i = 0; i < 1000; i++)
        {
            shared.MarkShaderActive(i);
        }

        shared.UpdateActiveShaders();
        mockOpenGL.Verify(g => g.UseProgram(It.IsAny<int>()), Times.AtLeast(1000));
    }

    [Fact]
    public void ClearActiveShadersWithManyShadersClearsAll()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

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
    public void UpdateActiveShadersWithNoActiveShadersDoesNotCrash()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);

        shared.UpdateActiveShaders();

        Assert.True(true);
    }

    [Fact]
    public void UpdateActiveShadersMultipleTimesIsConsistent()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        shared.MarkShaderActive(1);

        for (int i = 0; i < 100; i++)
        {
            shared.UpdateActiveShaders();
        }

        Assert.True(true);
    }

    [Fact]
    public void MarkShaderActiveSameShaderMultipleTimesOnlyCallsUseProgramOnce()
    {
        var mockOpenGL = new Mock<IOpenGLWrapper>();
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "view")).Returns(1);
        _ = mockOpenGL.Setup(g => g.GetUniformLocation(It.IsAny<int>(), "projection")).Returns(2);

        SystemSharedObject shared = CreateSystemSharedObject(mockOpenGL.Object);
        const int shaderId = 5;

        for (int i = 0; i < 100; i++)
        {
            shared.MarkShaderActive(shaderId);
        }

        mockOpenGL.Verify(g => g.UseProgram(shaderId), Times.Once);
    }
}
