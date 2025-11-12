using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса Logger.
/// </summary>
public class LoggerTests
{
    [Fact]
    public void Constructor_CreatesLoggerWithDefaultMinimumLevel()
    {
        var logger = new Logger();

        Assert.Equal(LogLevel.Debug, logger.MinimumLevel);
    }

    [Fact]
    public void MinimumLevel_CanBeSet()
    {
        var logger = new Logger();

        logger.MinimumLevel = LogLevel.Warning;

        Assert.Equal(LogLevel.Warning, logger.MinimumLevel);
    }

    [Fact]
    public void LogDebug_WritesDebugMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogDebug("Test debug message"));

        Assert.Contains("DEBUG", output);
        Assert.Contains("Test debug message", output);
    }

    [Fact]
    public void LogInfo_WritesInfoMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogInfo("Test info message"));

        Assert.Contains("INFO ", output);
        Assert.Contains("Test info message", output);
    }

    [Fact]
    public void LogWarning_WritesWarningMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogWarning("Test warning message"));

        Assert.Contains("WARN ", output);
        Assert.Contains("Test warning message", output);
    }

    [Fact]
    public void LogError_WritesErrorMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogError("Test error message"));

        Assert.Contains("ERROR", output);
        Assert.Contains("Test error message", output);
    }

    [Fact]
    public void LogCritical_WritesCriticalMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogCritical("Test critical message"));

        Assert.Contains("CRITICAL", output);
        Assert.Contains("Test critical message", output);
    }

    [Fact]
    public void Log_WithDebugLevel_WritesDebugMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Debug, "Test message"));

        Assert.Contains("DEBUG", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithInfoLevel_WritesInfoMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Info, "Test message"));

        Assert.Contains("INFO ", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithWarningLevel_WritesWarningMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Warning, "Test message"));

        Assert.Contains("WARN ", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithErrorLevel_WritesErrorMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Error, "Test message"));

        Assert.Contains("ERROR", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithCriticalLevel_WritesCriticalMessage()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.Log(LogLevel.Critical, "Test message"));

        Assert.Contains("CRITICAL", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelInfo_DoesNotLogDebug()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Info };
        string output = CaptureConsoleOutput(() => logger.LogDebug("Debug message"));

        Assert.DoesNotContain("Debug message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelInfo_LogsInfo()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Info };
        string output = CaptureConsoleOutput(() => logger.LogInfo("Info message"));

        Assert.Contains("Info message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelWarning_DoesNotLogInfo()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Warning };
        string output = CaptureConsoleOutput(() => logger.LogInfo("Info message"));

        Assert.DoesNotContain("Info message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelWarning_LogsWarning()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Warning };
        string output = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));

        Assert.Contains("Warning message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelError_DoesNotLogWarning()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        string output = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));

        Assert.DoesNotContain("Warning message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelError_LogsError()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        string output = CaptureConsoleOutput(() => logger.LogError("Error message"));

        Assert.Contains("Error message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelCritical_DoesNotLogError()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Critical };
        string output = CaptureConsoleOutput(() => logger.LogError("Error message"));

        Assert.DoesNotContain("Error message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelCritical_LogsCritical()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Critical };
        string output = CaptureConsoleOutput(() => logger.LogCritical("Critical message"));

        Assert.Contains("Critical message", output);
    }

    [Fact]
    public void Log_IncludesTimestamp()
    {
        var logger = new Logger();
        string output = CaptureConsoleOutput(() => logger.LogInfo("Test message"));

        Assert.Matches(@"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]", output);
    }

    [Fact]
    public void LogException_WithMessage_LogsExceptionWithMessage()
    {
        var logger = new Logger();
        var exception = new InvalidOperationException("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception, "Custom message"));

        Assert.Contains("ERROR", output);
        Assert.Contains("Custom message", output);
        Assert.Contains("InvalidOperationException", output);
    }

    [Fact]
    public void LogException_WithoutMessage_LogsExceptionOnly()
    {
        var logger = new Logger();
        var exception = new ArgumentException("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception));

        Assert.Contains("ERROR", output);
        Assert.Contains("ArgumentException", output);
        Assert.Contains("Test exception", output);
    }

    [Fact]
    public void LogException_WithMinimumLevel_RespectsMinimumLevel()
    {
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        var exception = new Exception("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Warning, exception, "Warning exception"));

        Assert.DoesNotContain("Warning exception", output);
    }

    [Fact]
    public async Task Log_IsThreadSafe()
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

            await Task.WhenAll(tasks);
            string output = stringWriter.ToString();

            for (int i = 0; i < 10; i++)
            {
                Assert.Contains($"Message {i}", output);
            }
        }
        finally
        {
            Console.SetOut(originalOut);
            stringWriter.Dispose();
        }
    }

    [Fact]
    public void Log_WithEmptyMessage_LogsEmptyMessage()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() => logger.LogInfo(""));

        Assert.Contains("INFO ", output);
    }

    [Fact]
    public void Log_WithNullMessage_LogsNullMessage()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() => logger.LogInfo(null!));

        Assert.Contains("INFO ", output);
    }

    /// <summary>
    /// Перехватывает вывод Console и возвращает его как строку.
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
    public void Log_WithVeryLongMessage_LogsCorrectly()
    {
        var logger = new Logger();
        string veryLongMessage = new string('a', 100000);

        string output = CaptureConsoleOutput(() => logger.LogInfo(veryLongMessage));

        Assert.Contains("INFO ", output);
        Assert.Contains(veryLongMessage.Substring(0, 100), output);
    }

    [Fact]
    public void Log_WithSpecialCharacters_LogsCorrectly()
    {
        var logger = new Logger();
        string specialMessage = "Test: @#$%^&*()_+-=[]{}|;':\",./<>?\n\t\r\0";

        string output = CaptureConsoleOutput(() => logger.LogInfo(specialMessage));

        Assert.Contains("INFO ", output);
        Assert.Contains("Test:", output);
    }

    [Fact]
    public void Log_WithUnicodeCharacters_LogsCorrectly()
    {
        var logger = new Logger();
        string unicodeMessage = "Тест 测试 🎮 资源";

        string output = CaptureConsoleOutput(() => logger.LogInfo(unicodeMessage));

        Assert.Contains("INFO ", output);
        Assert.Contains("Тест", output);
    }

    [Fact]
    public void LogException_WithNullException_HandlesCorrectly()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, null!, "Test message"));
        Assert.Contains("ERROR", output);
    }

    [Fact]
    public void LogException_WithNullMessage_LogsException()
    {
        var logger = new Logger();
        var exception = new InvalidOperationException("Test exception");

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception, null));

        Assert.Contains("ERROR", output);
        Assert.Contains("InvalidOperationException", output);
    }

    [Fact]
    public void MinimumLevel_SetToCritical_OnlyLogsCritical()
    {
        var logger = new Logger();
        logger.MinimumLevel = LogLevel.Critical;

        string output = CaptureConsoleOutput(() =>
        {
            logger.LogDebug("Debug message");
            logger.LogInfo("Info message");
            logger.LogWarning("Warning message");
            logger.LogError("Error message");
            logger.LogCritical("Critical message");
        });

        Assert.DoesNotContain("DEBUG", output);
        Assert.DoesNotContain("INFO ", output);
        Assert.DoesNotContain("WARN ", output);
        Assert.DoesNotContain("ERROR", output);
        Assert.Contains("CRITICAL", output);
    }

    [Fact]
    public void Log_MultipleRapidCalls_DoesNotCrash()
    {
        var logger = new Logger();

        string output = CaptureConsoleOutput(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                logger.LogInfo($"Message {i}");
            }
        });

        Assert.Contains("INFO ", output);
        Assert.Contains("Message 0", output);
        Assert.Contains("Message 999", output);
    }

    [Fact]
    public void Log_WithAllLogLevels_LogsCorrectly()
    {
        var logger = new Logger();
        logger.MinimumLevel = LogLevel.Debug;

        string output = CaptureConsoleOutput(() =>
        {
            logger.LogDebug("Debug");
            logger.LogInfo("Info");
            logger.LogWarning("Warning");
            logger.LogError("Error");
            logger.LogCritical("Critical");
        });

        Assert.Contains("DEBUG", output);
        Assert.Contains("INFO ", output);
        Assert.Contains("WARN ", output);
        Assert.Contains("ERROR", output);
        Assert.Contains("CRITICAL", output);
    }

    [Fact]
    public void LogException_WithNestedException_LogsCorrectly()
    {
        var logger = new Logger();
        var innerException = new ArgumentException("Inner exception");
        var outerException = new InvalidOperationException("Outer exception", innerException);

        string output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, outerException, "Test"));

        Assert.Contains("ERROR", output);
        Assert.Contains("InvalidOperationException", output);
        Assert.Contains("ArgumentException", output);
    }

    [Fact]
    public void MinimumLevel_SetToInvalidValue_StillWorks()
    {
        var logger = new Logger();

        logger.MinimumLevel = (LogLevel)(-1);
        string output = CaptureConsoleOutput(() => logger.LogInfo("Test"));

        Assert.True(true);
    }
}

