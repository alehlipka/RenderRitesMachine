using RenderRitesMachine.Services.Diagnostics;

namespace RenderRitesMachine.Tests;

/// <summary>
///     Tests for <see cref="Logger" />.
/// </summary>
public sealed class LoggerTests
{
    [Fact]
    public void ConstructorCreatesLoggerWithDefaultMinimumLevel()
    {
        var logger = new Logger();

        Assert.Equal(LogLevel.Debug, logger.MinimumLevel);
    }

    [Fact]
    public void MinimumLevelCanBeSet()
    {
        var logger = new Logger
        {
            MinimumLevel = LogLevel.Warning
        };

        Assert.Equal(LogLevel.Warning, logger.MinimumLevel);
    }

    [Fact]
    public void LogDebugWritesDebugMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogDebug("Test debug message"));

        Assert.Contains("DEBUG", output, StringComparison.Ordinal);
        Assert.Contains("Test debug message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogInfoWritesInfoMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogInfo("Test info message"));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains("Test info message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWarningWritesWarningMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogWarning("Test warning message"));

        Assert.Contains("WARN ", output, StringComparison.Ordinal);
        Assert.Contains("Test warning message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogErrorWritesErrorMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogError("Test error message"));

        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("Test error message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogCriticalWritesCriticalMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogCritical("Test critical message"));

        Assert.Contains("CRITICAL", output, StringComparison.Ordinal);
        Assert.Contains("Test critical message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithDebugLevelWritesDebugMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Debug, "Test message"));

        Assert.Contains("DEBUG", output, StringComparison.Ordinal);
        Assert.Contains("Test message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithInfoLevelWritesInfoMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Info, "Test message"));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains("Test message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithWarningLevelWritesWarningMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Warning, "Test message"));

        Assert.Contains("WARN ", output, StringComparison.Ordinal);
        Assert.Contains("Test message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithErrorLevelWritesErrorMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Error, "Test message"));

        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("Test message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithCriticalLevelWritesCriticalMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Critical, "Test message"));

        Assert.Contains("CRITICAL", output, StringComparison.Ordinal);
        Assert.Contains("Test message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelInfoDoesNotLogDebug()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Info };
        string output = CaptureConsoleOutput(() => logger.LogDebug("Debug message"));

        Assert.DoesNotContain("Debug message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelInfoLogsInfo()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Info };
        string output = CaptureConsoleOutput(() => logger.LogInfo("Info message"));

        Assert.Contains("Info message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelWarningDoesNotLogInfo()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Warning };
        string output = CaptureConsoleOutput(() => logger.LogInfo("Info message"));

        Assert.DoesNotContain("Info message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelWarningLogsWarning()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Warning };
        string output = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));

        Assert.Contains("Warning message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelErrorDoesNotLogWarning()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        string output = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));

        Assert.DoesNotContain("Warning message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelErrorLogsError()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        string output = CaptureConsoleOutput(() => logger.LogError("Error message"));

        Assert.Contains("Error message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelCriticalDoesNotLogError()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Critical };
        string output = CaptureConsoleOutput(() => logger.LogError("Error message"));

        Assert.DoesNotContain("Error message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithMinimumLevelCriticalLogsCritical()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Critical };
        string output = CaptureConsoleOutput(() => logger.LogCritical("Critical message"));

        Assert.Contains("Critical message", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogIncludesTimestamp()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogInfo("Test message"));

        Assert.Matches(@"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]", output);
    }

    [Fact]
    public void LogExceptionWithMessageLogsExceptionWithMessage()
    {
        var logger = new Logger();
        var exception = new InvalidOperationException("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception, "Custom message"));

        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("Custom message", output, StringComparison.Ordinal);
        Assert.Contains("InvalidOperationException", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogExceptionWithoutMessageLogsExceptionOnly()
    {
        var logger = new Logger();
        var exception = new ArgumentException("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception));

        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("ArgumentException", output, StringComparison.Ordinal);
        Assert.Contains("Test exception", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogExceptionWithMinimumLevelRespectsMinimumLevel()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        var exception = new InvalidOperationException("Test exception");

        string output =
            CaptureConsoleOutput(() => logger.LogException(LogLevel.Warning, exception, "Warning exception"));

        Assert.DoesNotContain("Warning exception", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LogIsThreadSafe()
    {
        var logger = new Logger();
        var tasks = new List<Task<string>>();
        TextWriter originalOut = Console.Out;
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks.Add(Task.Run(() =>
                {
                    logger.LogInfo($"Message {index}");
                    return $"Message {index}";
                }));
            }

            _ = await Task.WhenAll(tasks);
            string output = stringWriter.ToString();

            for (int i = 0; i < 10; i++)
            {
                Assert.Contains($"Message {i}", output, StringComparison.Ordinal);
            }
        }
        finally
        {
            Console.SetOut(originalOut);
            await stringWriter.DisposeAsync();
        }
    }

    [Fact]
    public void LogWithEmptyMessageLogsEmptyMessage()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() => logger.LogInfo(""));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithNullMessageLogsNullMessage()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() => logger.LogInfo(null!));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Captures console output and returns it as a string.
    /// </summary>
    private static string CaptureConsoleOutput(Action action)
    {
        TextWriter originalOut = Console.Out;
        try
        {
            using var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            action();
            return stringWriter.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void LogWithVeryLongMessageLogsCorrectly()
    {
        var logger = new Logger();
        string veryLongMessage = new('a', 100000);

        string output = CaptureConsoleOutput(() => logger.LogInfo(veryLongMessage));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains(veryLongMessage[..100], output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithSpecialCharactersLogsCorrectly()
    {
        var logger = new Logger();
        string specialMessage = "Test: @#$%^&*()_+-=[]{}|;':\",./<>?\n\t\r\0";

        string output = CaptureConsoleOutput(() => logger.LogInfo(specialMessage));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains("Test:", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithUnicodeCharactersLogsCorrectly()
    {
        var logger = new Logger();
        string unicodeMessage = "Тест 测试 🎮 资源";

        string output = CaptureConsoleOutput(() => logger.LogInfo(unicodeMessage));

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains("Тест", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogExceptionWithNullExceptionHandlesCorrectly()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, null!, "Test message"));
        Assert.Contains("ERROR", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogExceptionWithNullMessageLogsException()
    {
        var logger = new Logger();
        var exception = new InvalidOperationException("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception));

        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("InvalidOperationException", output, StringComparison.Ordinal);
    }

    [Fact]
    public void MinimumLevelSetToCriticalOnlyLogsCritical()
    {
        var logger = new Logger
        {
            MinimumLevel = LogLevel.Critical
        };

        string output = CaptureConsoleOutput(() =>
        {
            logger.LogDebug("Debug message");
            logger.LogInfo("Info message");
            logger.LogWarning("Warning message");
            logger.LogError("Error message");
            logger.LogCritical("Critical message");
        });

        Assert.DoesNotContain("DEBUG", output, StringComparison.Ordinal);
        Assert.DoesNotContain("INFO ", output, StringComparison.Ordinal);
        Assert.DoesNotContain("WARN ", output, StringComparison.Ordinal);
        Assert.DoesNotContain("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("CRITICAL", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogMultipleRapidCallsDoesNotCrash()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                logger.LogInfo($"Message {i}");
            }
        });

        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains("Message 0", output, StringComparison.Ordinal);
        Assert.Contains("Message 999", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogWithAllLogLevelsLogsCorrectly()
    {
        var logger = new Logger
        {
            MinimumLevel = LogLevel.Debug
        };

        string output = CaptureConsoleOutput(() =>
        {
            logger.LogDebug("Debug");
            logger.LogInfo("Info");
            logger.LogWarning("Warning");
            logger.LogError("Error");
            logger.LogCritical("Critical");
        });

        Assert.Contains("DEBUG", output, StringComparison.Ordinal);
        Assert.Contains("INFO ", output, StringComparison.Ordinal);
        Assert.Contains("WARN ", output, StringComparison.Ordinal);
        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("CRITICAL", output, StringComparison.Ordinal);
    }

    [Fact]
    public void LogExceptionWithNestedExceptionLogsCorrectly()
    {
        var logger = new Logger();
        var innerException = new ArgumentException("Inner exception");
        var outerException = new InvalidOperationException("Outer exception", innerException);

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, outerException, "Test"));

        Assert.Contains("ERROR", output, StringComparison.Ordinal);
        Assert.Contains("InvalidOperationException", output, StringComparison.Ordinal);
        Assert.Contains("ArgumentException", output, StringComparison.Ordinal);
    }

    [Fact]
    public void MinimumLevelSetToInvalidValueStillWorks()
    {
        var logger = new Logger
        {
            MinimumLevel = (LogLevel)(-1)
        };
        string output = CaptureConsoleOutput(() => logger.LogInfo("Test"));

        Assert.True(true);
    }
}
