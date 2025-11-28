using System.Diagnostics;
using System.Reflection;
using RenderRitesMachine.Debug;

namespace RenderRitesMachine.Tests;

public class FrameTimeCounterTests
{
    public FrameTimeCounterTests()
    {
        ResetCounter();
    }

    [Fact]
    public void GetFrameTimeBeforeUpdateReturnsZero()
    {
        double frameTime = FrameTimeCounter.GetFrameTimeMilliseconds();

        Assert.Equal(0d, frameTime);
    }

    [Fact]
    public void UpdateAfterElapsedTimeReportsPositiveFrameTime()
    {
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < TimeSpan.FromMilliseconds(600))
        {
            FrameTimeCounter.Update();
            Thread.Sleep(5);
        }

        double frameTime = FrameTimeCounter.GetFrameTimeMilliseconds();

        Assert.True(frameTime > 0d);
    }

    private static void ResetCounter()
    {
        Type type = typeof(FrameTimeCounter);

        FieldInfo? stopwatchField = type.GetField("Stopwatch", BindingFlags.NonPublic | BindingFlags.Static);
        var stopwatch = stopwatchField?.GetValue(null) as Stopwatch;
        stopwatch?.Reset();
        stopwatch?.Start();

        FieldInfo? frameCountField = type.GetField("_frameCount", BindingFlags.NonPublic | BindingFlags.Static);
        frameCountField?.SetValue(null, 0);

        FieldInfo? totalTimeField = type.GetField("_totalTime", BindingFlags.NonPublic | BindingFlags.Static);
        totalTimeField?.SetValue(null, 0d);

        FieldInfo? averageField = type.GetField("_averageFrameTimeMs", BindingFlags.NonPublic | BindingFlags.Static);
        averageField?.SetValue(null, 0d);
    }
}
