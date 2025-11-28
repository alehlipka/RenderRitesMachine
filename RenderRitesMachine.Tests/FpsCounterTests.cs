using System.Diagnostics;
using System.Reflection;
using RenderRitesMachine.Debug;

namespace RenderRitesMachine.Tests;

public sealed class FpsCounterTests
{
    public FpsCounterTests()
    {
        ResetCounter();
    }

    [Fact]
    public void GetFpsBeforeUpdatesReturnsZero()
    {
        double fps = FpsCounter.GetFps();

        Assert.Equal(0d, fps);
    }

    [Fact]
    public void UpdateAfterElapsedTimeProducesPositiveFps()
    {
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < TimeSpan.FromMilliseconds(600))
        {
            FpsCounter.Update();
            Thread.Sleep(5);
        }

        double fps = FpsCounter.GetFps();

        Assert.True(fps > 0d);
    }

    private static void ResetCounter()
    {
        Type type = typeof(FpsCounter);

        FieldInfo? stopwatchField = type.GetField("Stopwatch", BindingFlags.NonPublic | BindingFlags.Static);
        var stopwatch = stopwatchField?.GetValue(null) as Stopwatch;
        stopwatch?.Reset();
        stopwatch?.Start();

        FieldInfo? frameCountField = type.GetField("_frameCount", BindingFlags.NonPublic | BindingFlags.Static);
        frameCountField?.SetValue(null, 0);

        FieldInfo? totalTimeField = type.GetField("_totalTime", BindingFlags.NonPublic | BindingFlags.Static);
        totalTimeField?.SetValue(null, 0d);

        FieldInfo? averageFpsField = type.GetField("_averageFps", BindingFlags.NonPublic | BindingFlags.Static);
        averageFpsField?.SetValue(null, 0d);
    }
}
