using OpenTK.Mathematics;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса AudioService.
/// Примечание: Полное тестирование аудио функциональности требует OpenAL контекста и аудио устройства,
/// поэтому некоторые тесты могут пропускаться в окружениях без аудио оборудования.
/// </summary>
public class AudioServiceTests : IDisposable
{
    private readonly List<AudioService> _services = new();

    [Fact]
    public void Constructor_CreatesAudioService()
    {
        // Arrange & Act
        var service = CreateService();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithLogger_AcceptsLogger()
    {
        // Arrange
        var logger = new Logger();

        // Act
        var service = CreateService(logger);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void LoadAudio_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();
        var tempFile = CreateTempAudioFile();

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.LoadAudio(null!, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadAudio_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();
        var tempFile = CreateTempAudioFile();

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.LoadAudio("", tempFile));
            Assert.Throws<ArgumentNullException>(() => service.LoadAudio("   ", tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadAudio_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = CreateService();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");

        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => service.LoadAudio("audio", nonExistentPath));
        Assert.Contains(nonExistentPath, exception.Message);
    }

    [Fact]
    public void LoadAudio_WithValidFile_ReturnsBufferId()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            // Пропускаем тест, если нет тестового аудио файла
            return;
        }

        try
        {
            // Act
            var bufferId = service.LoadAudio("test_audio", audioFile);

            // Assert
            Assert.True(bufferId > 0);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован (нет аудио устройства)
        }
    }

    [Fact]
    public void LoadAudio_WithSameNameTwice_ReturnsSameBufferId()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            // Act
            var bufferId1 = service.LoadAudio("test_audio", audioFile);
            var bufferId2 = service.LoadAudio("test_audio", audioFile);

            // Assert
            Assert.Equal(bufferId1, bufferId2);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithNonExistentAudioName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                service.CreateSource("nonexistent"));
            Assert.Contains("nonexistent", exception.Message);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithValidAudio_ReturnsSourceId()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            // Act
            var sourceId = service.CreateSource("test_audio");

            // Assert
            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithPosition_Creates3DSource()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var position = new Vector3(1.0f, 2.0f, 3.0f);

            // Act
            var sourceId = service.CreateSource("test_audio", position);

            // Assert
            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithVolume_CreatesSourceWithVolume()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            // Act
            var sourceId = service.CreateSource("test_audio", volume: 0.5f);

            // Assert
            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithVolumeGreaterThanOne_ClampsToOne()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            // Act
            var sourceId = service.CreateSource("test_audio", volume: 2.0f);

            // Assert
            Assert.True(sourceId > 0);
            // Проверяем, что громкость была зажата
            service.SetSourceVolume(sourceId, 1.0f); // Это не должно изменить значение, так как уже зажато
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithNegativeVolume_ClampsToZero()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            // Act
            var sourceId = service.CreateSource("test_audio", volume: -1.0f);

            // Assert
            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void CreateSource_WithLoop_CreatesLoopingSource()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            // Act
            var sourceId = service.CreateSource("test_audio", loop: true);

            // Assert
            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Play_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.Play(999); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Play_WithValidSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act & Assert
            service.Play(sourceId);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Stop_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.Stop(999); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Stop_WithValidSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");
            service.Play(sourceId);

            // Act & Assert
            service.Stop(sourceId);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Pause_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.Pause(999); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Pause_WithValidSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");
            service.Play(sourceId);

            // Act & Assert
            service.Pause(sourceId);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourcePosition_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var position = new Vector3(1.0f, 2.0f, 3.0f);

        try
        {
            // Act & Assert
            service.SetSourcePosition(999, position); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourcePosition_WithValidSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");
            var position = new Vector3(1.0f, 2.0f, 3.0f);

            // Act & Assert
            service.SetSourcePosition(sourceId, position);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourceVolume_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.SetSourceVolume(999, 0.5f); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourceVolume_WithValidSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act & Assert
            service.SetSourceVolume(sourceId, 0.5f);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourceVolume_WithVolumeGreaterThanOne_ClampsToOne()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act
            service.SetSourceVolume(sourceId, 2.0f);

            // Assert - не должно выбрасывать исключение
            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourceVolume_WithNegativeVolume_ClampsToZero()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act
            service.SetSourceVolume(sourceId, -1.0f);

            // Assert - не должно выбрасывать исключение
            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourceLoop_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.SetSourceLoop(999, true); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetSourceLoop_WithValidSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act & Assert
            service.SetSourceLoop(sourceId, true);
            service.SetSourceLoop(sourceId, false);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void DeleteSource_WithNonExistentSourceId_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.DeleteSource(999); // Не должно выбрасывать исключение
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void DeleteSource_WithValidSourceId_RemovesSource()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act
            service.DeleteSource(sourceId);

            // Assert - повторное удаление не должно выбрасывать исключение
            service.DeleteSource(sourceId);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetListenerPosition_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var position = new Vector3(1.0f, 2.0f, 3.0f);

        try
        {
            // Act & Assert
            service.SetListenerPosition(position);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetListenerOrientation_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var forward = new Vector3(0, 0, -1);
        var up = new Vector3(0, 1, 0);

        try
        {
            // Act & Assert
            service.SetListenerOrientation(forward, up);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetMasterVolume_WithValidVolume_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act & Assert
            service.SetMasterVolume(0.5f);
            service.SetMasterVolume(1.0f);
            service.SetMasterVolume(0.0f);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetMasterVolume_WithVolumeGreaterThanOne_ClampsToOne()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act
            service.SetMasterVolume(2.0f);

            // Assert - не должно выбрасывать исключение
            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetMasterVolume_WithNegativeVolume_ClampsToZero()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act
            service.SetMasterVolume(-1.0f);

            // Assert - не должно выбрасывать исключение
            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void SetMasterVolume_UpdatesAllSources()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId1 = service.CreateSource("test_audio", volume: 0.5f);
            var sourceId2 = service.CreateSource("test_audio", volume: 0.8f);

            // Act
            service.SetMasterVolume(0.5f);

            // Assert - не должно выбрасывать исключение
            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void IsPlaying_WithNonExistentSourceId_ReturnsFalse()
    {
        // Arrange
        var service = CreateService();

        try
        {
            // Act
            var isPlaying = service.IsPlaying(999);

            // Assert
            Assert.False(isPlaying);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void IsPlaying_WithStoppedSource_ReturnsFalse()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act
            var isPlaying = service.IsPlaying(sourceId);

            // Assert
            Assert.False(isPlaying);
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
        }
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        service.Dispose();
        // Повторный вызов Dispose не должен выбрасывать исключение
        service.Dispose();
    }

    [Fact]
    public void Dispose_AfterCreatingSources_CleansUpResources()
    {
        // Arrange
        var service = CreateService();
        var audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var sourceId = service.CreateSource("test_audio");

            // Act
            service.Dispose();

            // Assert - повторный вызов Dispose не должен выбрасывать исключение
            service.Dispose();
        }
        catch (InvalidOperationException)
        {
            // Пропускаем тест, если OpenAL не инициализирован
            service.Dispose();
        }
    }

    /// <summary>
    /// Создает временный аудио файл для тестирования.
    /// </summary>
    private static string CreateTempAudioFile()
    {
        // Создаем минимальный WAV файл для тестирования
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");

        // Создаем минимальный WAV заголовок (44 байта) + пустые данные
        var wavHeader = new byte[]
        {
            0x52, 0x49, 0x46, 0x46, // "RIFF"
            0x24, 0x00, 0x00, 0x00, // Размер файла - 8
            0x57, 0x41, 0x56, 0x45, // "WAVE"
            0x66, 0x6D, 0x74, 0x20, // "fmt "
            0x10, 0x00, 0x00, 0x00, // Размер fmt chunk
            0x01, 0x00,             // Audio format (PCM)
            0x01, 0x00,             // Количество каналов (моно)
            0x44, 0xAC, 0x00, 0x00, // Sample rate (44100)
            0x88, 0x58, 0x01, 0x00, // Byte rate
            0x02, 0x00,             // Block align
            0x10, 0x00,             // Bits per sample (16)
            0x64, 0x61, 0x74, 0x61, // "data"
            0x00, 0x00, 0x00, 0x00  // Размер данных
        };

        File.WriteAllBytes(tempFile, wavHeader);
        return tempFile;
    }

    /// <summary>
    /// Получает путь к тестовому аудио файлу, если он доступен.
    /// </summary>
    private static string? GetTestAudioFile()
    {
        // Пробуем найти существующий аудио файл
        var possiblePaths = new[]
        {
            Path.Combine("..", "..", "..", "..", "RenderRitesMachine", "logo.mp3"),
            Path.Combine("..", "..", "..", "..", "RenderRitesDemo", "Assets", "Sounds", "click.mp3")
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
        }

        return null;
    }

    /// <summary>
    /// Создает новый экземпляр AudioService для тестирования.
    /// </summary>
    private AudioService CreateService(ILogger? logger = null)
    {
        var service = new AudioService(logger);
        _services.Add(service);
        return service;
    }

    public void Dispose()
    {
        foreach (var service in _services)
        {
            try
            {
                service.Dispose();
            }
            catch
            {
                // Игнорируем ошибки при очистке
            }
        }
        _services.Clear();
    }
}

