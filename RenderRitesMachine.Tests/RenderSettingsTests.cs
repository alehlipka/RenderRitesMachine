using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Tests;

public sealed class RenderSettingsTests
{
    [Fact]
    public void ValidateWithDefaultSettingsDoesNotThrow()
    {
        var settings = new RenderSettings();

        settings.Validate();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(256)]
    public void ValidateThrowsWhenMaxStencilValueOutOfRange(int invalidValue)
    {
        var settings = new RenderSettings
        {
            MaxStencilValue = invalidValue
        };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => settings.Validate());
    }

    [Fact]
    public void ValidateThrowsWhenFarPlaneLessOrEqualNearPlane()
    {
        var settings = new RenderSettings
        {
            CameraNearPlane = 1f,
            CameraFarPlane = 1f
        };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => settings.Validate());
    }

    [Fact]
    public void ValidateThrowsWhenDefaultWidthLessThanMinimum()
    {
        var settings = new RenderSettings
        {
            MinWindowWidth = 800,
            DefaultWindowWidth = 799
        };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => settings.Validate());
    }

    [Fact]
    public void ValidateThrowsWhenSamplesOutsideRange()
    {
        var settings = new RenderSettings
        {
            DefaultSamples = 0
        };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => settings.Validate());
    }
}

