using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса RenderConstants.
/// </summary>
public sealed class RenderConstantsTests : IDisposable
{
    public RenderConstantsTests()
    {
        RenderConstants.Reset();
    }

    public void Dispose()
    {
        RenderConstants.Reset();
    }

    [Fact]
    public void MaxStencilValueIsValid()
    {
        Assert.True(RenderConstants.MaxStencilValue > 0);
        Assert.True(RenderConstants.MaxStencilValue <= 255);
    }

    [Fact]
    public void AnisotropicFilteringLevelIsPositive()
    {
        Assert.True(RenderConstants.AnisotropicFilteringLevel > 0);
    }

    [Fact]
    public void CameraNearPlaneIsPositive()
    {
        Assert.True(RenderConstants.CameraNearPlane > 0);
    }

    [Fact]
    public void CameraFarPlaneIsGreaterThanNearPlane()
    {
        Assert.True(RenderConstants.CameraFarPlane > RenderConstants.CameraNearPlane);
    }

    [Fact]
    public void CameraFovRangeIsValid()
    {
        Assert.True(RenderConstants.CameraMinFov > 0);
        Assert.True(RenderConstants.CameraMaxFov > RenderConstants.CameraMinFov);
        Assert.True(RenderConstants.CameraMaxFov <= 90);
    }

    [Fact]
    public void CameraPitchRangeIsValid()
    {
        Assert.True(RenderConstants.CameraMinPitch >= -90);
        Assert.True(RenderConstants.CameraMaxPitch <= 90);
        Assert.True(RenderConstants.CameraMaxPitch > RenderConstants.CameraMinPitch);
    }

    [Fact]
    public void VertexAttributeSizeIsPositive()
    {
        Assert.True(RenderConstants.VertexAttributeSize > 0);
    }

    [Fact]
    public void PositionAttributeSizeIsPositive()
    {
        Assert.True(RenderConstants.PositionAttributeSize > 0);
    }

    [Fact]
    public void FloatEpsilonIsSmallPositive()
    {
        Assert.True(RenderConstants.FloatEpsilon > 0);
        Assert.True(RenderConstants.FloatEpsilon < 0.001f);
    }

    [Fact]
    public void WindowSizeIsValid()
    {
        Assert.True(RenderConstants.MinWindowWidth > 0);
        Assert.True(RenderConstants.MinWindowHeight > 0);
        Assert.True(RenderConstants.DefaultWindowWidth >= RenderConstants.MinWindowWidth);
        Assert.True(RenderConstants.DefaultWindowHeight >= RenderConstants.MinWindowHeight);
    }

    [Fact]
    public void VertexAttributeSizeIsGreaterThanPositionAttributeSize()
    {
        Assert.True(RenderConstants.VertexAttributeSize >= RenderConstants.PositionAttributeSize);
    }

    [Fact]
    public void ConfigureAppliesCustomSettings()
    {
        var custom = new RenderSettings
        {
            MaxStencilValue = 128,
            AnisotropicFilteringLevel = 4.0f,
            CameraNearPlane = 0.1f,
            CameraFarPlane = 500f,
            CameraMinFov = 5f,
            CameraMaxFov = 60f,
            CameraMinPitch = -45f,
            CameraMaxPitch = 45f,
            VertexAttributeSize = 9,
            PositionAttributeSize = 3,
            FloatEpsilon = 0.00001f,
            MinWindowWidth = 640,
            MinWindowHeight = 480,
            DefaultWindowWidth = 1920,
            DefaultWindowHeight = 1080,
            DefaultVSyncMode = OpenTK.Windowing.Common.VSyncMode.Adaptive,
            DefaultSamples = 8,
            DefaultWindowState = OpenTK.Windowing.Common.WindowState.Fullscreen
        };

        using (new TemporaryRenderSettings(custom))
        {
            Assert.Equal(128, RenderConstants.MaxStencilValue);
            Assert.Equal(500f, RenderConstants.CameraFarPlane);
            Assert.Equal(1920, RenderConstants.DefaultWindowWidth);
            Assert.Equal(8, RenderConstants.DefaultSamples);
            Assert.Equal(OpenTK.Windowing.Common.WindowState.Fullscreen, RenderConstants.DefaultWindowState);
        }
    }

    [Fact]
    public void ConfigureInvalidSettingsThrows()
    {
        var invalid = new RenderSettings
        {
            CameraFarPlane = 0.001f
        };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => RenderConstants.Configure(invalid));
    }

    private sealed class TemporaryRenderSettings : IDisposable
    {
        private readonly RenderSettings _previous;

        public TemporaryRenderSettings(RenderSettings settings)
        {
            _previous = RenderConstants.Settings with { };
            RenderConstants.Configure(settings);
        }

        public void Dispose()
        {
            RenderConstants.Configure(_previous);
        }
    }
}
