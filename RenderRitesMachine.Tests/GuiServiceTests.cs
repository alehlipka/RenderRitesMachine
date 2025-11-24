using Moq;
using OpenTK.Mathematics;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Tests;

public sealed class GuiServiceTests
{
    private static GuiService CreateService(FakeGuiRenderer renderer)
    {
        ILogger logger = Mock.Of<ILogger>();
        return new GuiService(logger, renderer);
    }

    [Fact]
    public void EnsureInitializedConfiguresRendererAndSurface()
    {
        var renderer = new FakeGuiRenderer();
        GuiService service = CreateService(renderer);

        service.EnsureInitialized(128, 72);

        Assert.Equal(1, renderer.InitializeCalls);
        Assert.Equal((128, 72), renderer.LastSize);
        Assert.Equal(128, service.Width);
        Assert.Equal(72, service.Height);
    }

    [Fact]
    public void BeginFrameTwiceThrowsInvalidOperationException()
    {
        var renderer = new FakeGuiRenderer();
        GuiService service = CreateService(renderer);
        service.EnsureInitialized(64, 64);

        service.BeginFrame(Color4.Black);

        Assert.Throws<InvalidOperationException>(() => service.BeginFrame(Color4.White));
    }

    [Fact]
    public void DrawingWithoutBeginFrameThrows()
    {
        var renderer = new FakeGuiRenderer();
        GuiService service = CreateService(renderer);
        service.EnsureInitialized(32, 32);

        Assert.Throws<InvalidOperationException>(() => service.DrawPixel(1, 1, Color4.White));
    }

    [Fact]
    public void EndFrameWithoutDrawKeepsHasContentFalse()
    {
        var renderer = new FakeGuiRenderer();
        GuiService service = CreateService(renderer);
        service.EnsureInitialized(32, 32);

        service.BeginFrame(Color4.Black);
        service.EndFrame();

        Assert.False(service.HasContent);
        Assert.Equal(0, renderer.UploadCalls);
    }

    [Fact]
    public void DrawingUpdatesHasContentAndUploadsSurface()
    {
        var renderer = new FakeGuiRenderer();
        GuiService service = CreateService(renderer);
        service.EnsureInitialized(64, 64);

        service.BeginFrame(Color4.Black);
        service.FillRectangle(0, 0, 10, 10, Color4.Red);
        service.EndFrame();

        Assert.True(service.HasContent);
        Assert.Equal(1, renderer.UploadCalls);

        service.Render();
        Assert.Equal(1, renderer.RenderCalls);
    }

    [Fact]
    public void RenderWithoutContentDoesNothing()
    {
        var renderer = new FakeGuiRenderer();
        GuiService service = CreateService(renderer);
        service.EnsureInitialized(16, 16);

        service.Render();

        Assert.Equal(0, renderer.RenderCalls);
    }

    private sealed class FakeGuiRenderer : IGuiRenderer
    {
        public int InitializeCalls { get; private set; }
        public int UploadCalls { get; private set; }
        public int RenderCalls { get; private set; }
        public (int Width, int Height) LastSize { get; private set; }

        public void Initialize()
        {
            InitializeCalls++;
        }

        public void EnsureTextureSize(int width, int height)
        {
            LastSize = (width, height);
        }

        public void UploadSurface(GuiSurface surface)
        {
            UploadCalls++;
        }

        public void Render()
        {
            RenderCalls++;
        }

        public void Dispose()
        {
        }
    }
}

