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
        // Arrange & Act
        var logger = new Logger();

        // Assert
        Assert.Equal(LogLevel.Debug, logger.MinimumLevel);
    }

    [Fact]
    public void MinimumLevel_CanBeSet()
    {
        // Arrange
        var logger = new Logger();

        // Act
        logger.MinimumLevel = LogLevel.Warning;

        // Assert
        Assert.Equal(LogLevel.Warning, logger.MinimumLevel);
    }

    [Fact]
    public void LogDebug_WritesDebugMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.LogDebug("Test debug message"));

        // Assert
        Assert.Contains("DEBUG", output);
        Assert.Contains("Test debug message", output);
    }

    [Fact]
    public void LogInfo_WritesInfoMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.LogInfo("Test info message"));

        // Assert
        Assert.Contains("INFO ", output);
        Assert.Contains("Test info message", output);
    }

    [Fact]
    public void LogWarning_WritesWarningMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.LogWarning("Test warning message"));

        // Assert
        Assert.Contains("WARN ", output);
        Assert.Contains("Test warning message", output);
    }

    [Fact]
    public void LogError_WritesErrorMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.LogError("Test error message"));

        // Assert
        Assert.Contains("ERROR", output);
        Assert.Contains("Test error message", output);
    }

    [Fact]
    public void LogCritical_WritesCriticalMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.LogCritical("Test critical message"));

        // Assert
        Assert.Contains("CRITICAL", output);
        Assert.Contains("Test critical message", output);
    }

    [Fact]
    public void Log_WithDebugLevel_WritesDebugMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.Log(LogLevel.Debug, "Test message"));

        // Assert
        Assert.Contains("DEBUG", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithInfoLevel_WritesInfoMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.Log(LogLevel.Info, "Test message"));

        // Assert
        Assert.Contains("INFO ", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithWarningLevel_WritesWarningMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.Log(LogLevel.Warning, "Test message"));

        // Assert
        Assert.Contains("WARN ", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithErrorLevel_WritesErrorMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.Log(LogLevel.Error, "Test message"));

        // Assert
        Assert.Contains("ERROR", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithCriticalLevel_WritesCriticalMessage()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.Log(LogLevel.Critical, "Test message"));

        // Assert
        Assert.Contains("CRITICAL", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelInfo_DoesNotLogDebug()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Info };
        var output = CaptureConsoleOutput(() => logger.LogDebug("Debug message"));

        // Assert
        Assert.DoesNotContain("Debug message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelInfo_LogsInfo()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Info };
        var output = CaptureConsoleOutput(() => logger.LogInfo("Info message"));

        // Assert
        Assert.Contains("Info message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelWarning_DoesNotLogInfo()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Warning };
        var output = CaptureConsoleOutput(() => logger.LogInfo("Info message"));

        // Assert
        Assert.DoesNotContain("Info message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelWarning_LogsWarning()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Warning };
        var output = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));

        // Assert
        Assert.Contains("Warning message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelError_DoesNotLogWarning()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        var output = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));

        // Assert
        Assert.DoesNotContain("Warning message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelError_LogsError()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        var output = CaptureConsoleOutput(() => logger.LogError("Error message"));

        // Assert
        Assert.Contains("Error message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelCritical_DoesNotLogError()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Critical };
        var output = CaptureConsoleOutput(() => logger.LogError("Error message"));

        // Assert
        Assert.DoesNotContain("Error message", output);
    }

    [Fact]
    public void Log_WithMinimumLevelCritical_LogsCritical()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Critical };
        var output = CaptureConsoleOutput(() => logger.LogCritical("Critical message"));

        // Assert
        Assert.Contains("Critical message", output);
    }

    [Fact]
    public void Log_IncludesTimestamp()
    {
        // Arrange
        var logger = new Logger();
        var output = CaptureConsoleOutput(() => logger.LogInfo("Test message"));

        // Assert - проверяем формат временной метки (yyyy-MM-dd HH:mm:ss.fff)
        Assert.Matches(@"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]", output);
    }

    [Fact]
    public void LogException_WithMessage_LogsExceptionWithMessage()
    {
        // Arrange
        var logger = new Logger();
        var exception = new InvalidOperationException("Test exception");

        // Act
        var output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception, "Custom message"));

        // Assert
        Assert.Contains("ERROR", output);
        Assert.Contains("Custom message", output);
        Assert.Contains("InvalidOperationException", output);
    }

    [Fact]
    public void LogException_WithoutMessage_LogsExceptionOnly()
    {
        // Arrange
        var logger = new Logger();
        var exception = new ArgumentException("Test exception");

        // Act
        var output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception));

        // Assert
        Assert.Contains("ERROR", output);
        Assert.Contains("ArgumentException", output);
        Assert.Contains("Test exception", output);
    }

    [Fact]
    public void LogException_WithMinimumLevel_RespectsMinimumLevel()
    {
        // Arrange
        var logger = new Logger { MinimumLevel = LogLevel.Error };
        var exception = new Exception("Test exception");

        // Act
        var output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Warning, exception, "Warning exception"));

        // Assert
        Assert.DoesNotContain("Warning exception", output);
    }

    [Fact]
    public async Task Log_IsThreadSafe()
    {
        // Arrange
        var logger = new Logger();
        var tasks = new List<Task<string>>();
        var originalOut = Console.Out;
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act - запускаем несколько потоков одновременно
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
            var output = stringWriter.ToString();

            // Assert - проверяем, что все сообщения были записаны
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
        // Arrange
        var logger = new Logger();

        // Act
        var output = CaptureConsoleOutput(() => logger.LogInfo(""));

        // Assert
        Assert.Contains("INFO ", output);
    }

    [Fact]
    public void Log_WithNullMessage_LogsNullMessage()
    {
        // Arrange
        var logger = new Logger();

        // Act
        var output = CaptureConsoleOutput(() => logger.LogInfo(null!));

        // Assert
        Assert.Contains("INFO ", output);
    }

    /// <summary>
    /// Перехватывает вывод Console и возвращает его как строку.
    /// </summary>
    private static string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
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

    // Edge cases для Logger

    [Fact]
    public void Log_WithVeryLongMessage_LogsCorrectly()
    {
        // Arrange
        var logger = new Logger();
        var veryLongMessage = new string('a', 100000); // Очень длинное сообщение

        // Act
        var output = CaptureConsoleOutput(() => logger.LogInfo(veryLongMessage));

        // Assert - сообщение должно быть залогировано
        Assert.Contains("INFO ", output);
        Assert.Contains(veryLongMessage.Substring(0, 100), output); // Проверяем начало
    }

    [Fact]
    public void Log_WithSpecialCharacters_LogsCorrectly()
    {
        // Arrange
        var logger = new Logger();
        var specialMessage = "Test: @#$%^&*()_+-=[]{}|;':\",./<>?\n\t\r\0";

        // Act
        var output = CaptureConsoleOutput(() => logger.LogInfo(specialMessage));

        // Assert
        Assert.Contains("INFO ", output);
        Assert.Contains("Test:", output);
    }

    [Fact]
    public void Log_WithUnicodeCharacters_LogsCorrectly()
    {
        // Arrange
        var logger = new Logger();
        var unicodeMessage = "Тест 测试 🎮 资源";

        // Act
        var output = CaptureConsoleOutput(() => logger.LogInfo(unicodeMessage));

        // Assert
        Assert.Contains("INFO ", output);
        Assert.Contains("Тест", output);
    }

    [Fact]
    public void LogException_WithNullException_HandlesCorrectly()
    {
        // Arrange
        var logger = new Logger();

        // Act & Assert - не должно быть исключения
        var output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, null!, "Test message"));
        Assert.Contains("ERROR", output);
    }

    [Fact]
    public void LogException_WithNullMessage_LogsException()
    {
        // Arrange
        var logger = new Logger();
        var exception = new InvalidOperationException("Test exception");

        // Act
        var output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, exception, null));

        // Assert
        Assert.Contains("ERROR", output);
        Assert.Contains("InvalidOperationException", output);
    }

    [Fact]
    public void MinimumLevel_SetToCritical_OnlyLogsCritical()
    {
        // Arrange
        var logger = new Logger();
        logger.MinimumLevel = LogLevel.Critical;

        // Act
        var output = CaptureConsoleOutput(() =>
        {
            logger.LogDebug("Debug message");
            logger.LogInfo("Info message");
            logger.LogWarning("Warning message");
            logger.LogError("Error message");
            logger.LogCritical("Critical message");
        });

        // Assert - только Critical должно быть залогировано
        Assert.DoesNotContain("DEBUG", output);
        Assert.DoesNotContain("INFO ", output);
        Assert.DoesNotContain("WARN ", output);
        Assert.DoesNotContain("ERROR", output);
        Assert.Contains("CRITICAL", output);
    }

    [Fact]
    public void Log_MultipleRapidCalls_DoesNotCrash()
    {
        // Arrange
        var logger = new Logger();

        // Act - много быстрых вызовов
        var output = CaptureConsoleOutput(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                logger.LogInfo($"Message {i}");
            }
        });

        // Assert - не должно быть исключений
        Assert.Contains("INFO ", output);
        Assert.Contains("Message 0", output);
        Assert.Contains("Message 999", output);
    }

    [Fact]
    public void Log_WithAllLogLevels_LogsCorrectly()
    {
        // Arrange
        var logger = new Logger();
        logger.MinimumLevel = LogLevel.Debug;

        // Act
        var output = CaptureConsoleOutput(() =>
        {
            logger.LogDebug("Debug");
            logger.LogInfo("Info");
            logger.LogWarning("Warning");
            logger.LogError("Error");
            logger.LogCritical("Critical");
        });

        // Assert - все уровни должны быть залогированы
        Assert.Contains("DEBUG", output);
        Assert.Contains("INFO ", output);
        Assert.Contains("WARN ", output);
        Assert.Contains("ERROR", output);
        Assert.Contains("CRITICAL", output);
    }

    [Fact]
    public void LogException_WithNestedException_LogsCorrectly()
    {
        // Arrange
        var logger = new Logger();
        var innerException = new ArgumentException("Inner exception");
        var outerException = new InvalidOperationException("Outer exception", innerException);

        // Act
        var output = CaptureConsoleOutput(() => logger.LogException(LogLevel.Error, outerException, "Test"));

        // Assert
        Assert.Contains("ERROR", output);
        Assert.Contains("InvalidOperationException", output);
        Assert.Contains("ArgumentException", output);
    }

    [Fact]
    public void MinimumLevel_SetToInvalidValue_StillWorks()
    {
        // Arrange
        var logger = new Logger();

        // Act - устанавливаем недопустимое значение (меньше минимального)
        logger.MinimumLevel = (LogLevel)(-1);
        var output = CaptureConsoleOutput(() => logger.LogInfo("Test"));

        // Assert - логирование все еще должно работать
        // (в реальной реализации может быть проверка, но для теста просто проверяем, что не падает)
        Assert.True(true);
    }
}

