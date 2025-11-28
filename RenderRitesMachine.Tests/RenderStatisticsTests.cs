using RenderRitesMachine.Debug;

namespace RenderRitesMachine.Tests;

public sealed class RenderStatisticsTests
{
    [Fact]
    public void ResetClearsCounters()
    {
        var stats = new RenderStatistics
        {
            TotalObjects = 10,
            RenderedObjects = 5
        };

        stats.Reset();

        Assert.Equal(0, stats.TotalObjects);
        Assert.Equal(0, stats.RenderedObjects);
    }

    [Fact]
    public void PropertiesCanBeUpdated()
    {
        var stats = new RenderStatistics
        {
            TotalObjects = 2,
            RenderedObjects = 1
        };

        Assert.Equal(2, stats.TotalObjects);
        Assert.Equal(1, stats.RenderedObjects);

        stats.TotalObjects = 4;
        stats.RenderedObjects = 3;

        Assert.Equal(4, stats.TotalObjects);
        Assert.Equal(3, stats.RenderedObjects);
    }
}
