using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

public sealed class TimeServiceTests
{
    [Fact]
    public void ConstructorInitializesZeroDeltaTimes()
    {
        var service = new TimeService();

        Assert.Equal(0f, service.UpdateDeltaTime);
        Assert.Equal(0f, service.RenderDeltaTime);
    }

    [Fact]
    public void DeltaTimePropertiesCanBeAssigned()
    {
        var service = new TimeService
        {
            UpdateDeltaTime = 1.5f,
            RenderDeltaTime = 0.75f
        };

        Assert.Equal(1.5f, service.UpdateDeltaTime);
        Assert.Equal(0.75f, service.RenderDeltaTime);
    }
}

