using System.Diagnostics;

namespace RenderRitesMachine.Debug;

public static class FrameTimeCounter
{
    private const double SampleIntervalSeconds = .5;

    private static readonly Stopwatch Stopwatch;
    private static int _frameCount;
    private static double _totalTime;
    private static double _averageFrameTimeMs;

    static FrameTimeCounter()
    {
        Stopwatch = Stopwatch.StartNew();
    }

    public static void Update()
    {
        _frameCount++;
        _totalTime = Stopwatch.Elapsed.TotalSeconds;

        if (!(_totalTime >= SampleIntervalSeconds))
        {
            return;
        }

        _averageFrameTimeMs = _frameCount > 0 ? (_totalTime / _frameCount * 1000) : 0;
        _frameCount = 0;
        Stopwatch.Restart();
    }

    public static double GetFrameTimeMilliseconds()
    {
        return _averageFrameTimeMs;
    }

    public static double GetFrameTimeSeconds()
    {
        return _averageFrameTimeMs / 1000;
    }
}

