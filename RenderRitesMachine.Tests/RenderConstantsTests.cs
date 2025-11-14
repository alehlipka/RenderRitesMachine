using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса RenderConstants.
/// </summary>
public class RenderConstantsTests
{
    [Fact]
    public void MaxStencilValue_IsValid()
    {
        Assert.True(RenderConstants.MaxStencilValue > 0);
        Assert.True(RenderConstants.MaxStencilValue <= 255);
    }

    [Fact]
    public void AnisotropicFilteringLevel_IsPositive()
    {
        Assert.True(RenderConstants.AnisotropicFilteringLevel > 0);
    }

    [Fact]
    public void CameraNearPlane_IsPositive()
    {
        Assert.True(RenderConstants.CameraNearPlane > 0);
    }

    [Fact]
    public void CameraFarPlane_IsGreaterThanNearPlane()
    {
        Assert.True(RenderConstants.CameraFarPlane > RenderConstants.CameraNearPlane);
    }

    [Fact]
    public void CameraFov_RangeIsValid()
    {
        Assert.True(RenderConstants.CameraMinFov > 0);
        Assert.True(RenderConstants.CameraMaxFov > RenderConstants.CameraMinFov);
        Assert.True(RenderConstants.CameraMaxFov <= 90);
    }

    [Fact]
    public void CameraPitch_RangeIsValid()
    {
        Assert.True(RenderConstants.CameraMinPitch >= -90);
        Assert.True(RenderConstants.CameraMaxPitch <= 90);
        Assert.True(RenderConstants.CameraMaxPitch > RenderConstants.CameraMinPitch);
    }

    [Fact]
    public void VertexAttributeSize_IsPositive()
    {
        Assert.True(RenderConstants.VertexAttributeSize > 0);
    }

    [Fact]
    public void PositionAttributeSize_IsPositive()
    {
        Assert.True(RenderConstants.PositionAttributeSize > 0);
    }

    [Fact]
    public void FloatEpsilon_IsSmallPositive()
    {
        Assert.True(RenderConstants.FloatEpsilon > 0);
        Assert.True(RenderConstants.FloatEpsilon < 0.001f);
    }

    [Fact]
    public void WindowSize_IsValid()
    {
        Assert.True(RenderConstants.MinWindowWidth > 0);
        Assert.True(RenderConstants.MinWindowHeight > 0);
        Assert.True(RenderConstants.DefaultWindowWidth >= RenderConstants.MinWindowWidth);
        Assert.True(RenderConstants.DefaultWindowHeight >= RenderConstants.MinWindowHeight);
    }

    [Fact]
    public void VertexAttributeSize_IsGreaterThanPositionAttributeSize()
    {
        Assert.True(RenderConstants.VertexAttributeSize >= RenderConstants.PositionAttributeSize);
    }
}

