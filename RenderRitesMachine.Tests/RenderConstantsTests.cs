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
        // Assert
        Assert.True(RenderConstants.MaxStencilValue > 0);
        Assert.True(RenderConstants.MaxStencilValue <= 255);
    }

    [Fact]
    public void AnisotropicFilteringLevel_IsPositive()
    {
        // Assert
        Assert.True(RenderConstants.AnisotropicFilteringLevel > 0);
    }

    [Fact]
    public void CameraNearPlane_IsPositive()
    {
        // Assert
        Assert.True(RenderConstants.CameraNearPlane > 0);
    }

    [Fact]
    public void CameraFarPlane_IsGreaterThanNearPlane()
    {
        // Assert
        Assert.True(RenderConstants.CameraFarPlane > RenderConstants.CameraNearPlane);
    }

    [Fact]
    public void CameraFov_RangeIsValid()
    {
        // Assert
        Assert.True(RenderConstants.CameraMinFov > 0);
        Assert.True(RenderConstants.CameraMaxFov > RenderConstants.CameraMinFov);
        Assert.True(RenderConstants.CameraMaxFov <= 90);
    }

    [Fact]
    public void CameraPitch_RangeIsValid()
    {
        // Assert
        Assert.True(RenderConstants.CameraMinPitch >= -90);
        Assert.True(RenderConstants.CameraMaxPitch <= 90);
        Assert.True(RenderConstants.CameraMaxPitch > RenderConstants.CameraMinPitch);
    }

    [Fact]
    public void VertexAttributeSize_IsPositive()
    {
        // Assert
        Assert.True(RenderConstants.VertexAttributeSize > 0);
    }

    [Fact]
    public void PositionAttributeSize_IsPositive()
    {
        // Assert
        Assert.True(RenderConstants.PositionAttributeSize > 0);
    }

    [Fact]
    public void FloatEpsilon_IsSmallPositive()
    {
        // Assert
        Assert.True(RenderConstants.FloatEpsilon > 0);
        Assert.True(RenderConstants.FloatEpsilon < 0.001f);
    }

    [Fact]
    public void WindowSize_IsValid()
    {
        // Assert
        Assert.True(RenderConstants.MinWindowWidth > 0);
        Assert.True(RenderConstants.MinWindowHeight > 0);
        Assert.True(RenderConstants.DefaultWindowWidth >= RenderConstants.MinWindowWidth);
        Assert.True(RenderConstants.DefaultWindowHeight >= RenderConstants.MinWindowHeight);
    }

    [Fact]
    public void VertexAttributeSize_IsGreaterThanPositionAttributeSize()
    {
        // Assert
        Assert.True(RenderConstants.VertexAttributeSize >= RenderConstants.PositionAttributeSize);
    }

    [Fact]
    public void MaxBatchSize_IsPositive()
    {
        // Assert
        Assert.True(RenderConstants.MaxBatchSize > 0);
    }

    [Fact]
    public void MaxBatchSize_IsReasonable()
    {
        // Assert - MaxBatchSize должен быть достаточно большим для эффективного batch rendering,
        // но не слишком большим, чтобы избежать проблем с памятью и производительностью
        Assert.True(RenderConstants.MaxBatchSize >= 100);
        Assert.True(RenderConstants.MaxBatchSize <= 10000);
    }
}

