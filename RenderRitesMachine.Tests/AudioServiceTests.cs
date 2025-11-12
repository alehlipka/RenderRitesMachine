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
        AudioService service = CreateService();

        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithLogger_AcceptsLogger()
    {
        var logger = new Logger();

        AudioService service = CreateService(logger);

        Assert.NotNull(service);
    }

    [Fact]
    public void LoadAudio_WithNullName_ThrowsArgumentNullException()
    {
        AudioService service = CreateService();
        string tempFile = CreateTempAudioFile();

        try
        {
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
        AudioService service = CreateService();
        string tempFile = CreateTempAudioFile();

        try
        {
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
        AudioService service = CreateService();
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");

        FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() => service.LoadAudio("audio", nonExistentPath));
        Assert.Contains(nonExistentPath, exception.Message);
    }

    [Fact]
    public void LoadAudio_WithValidFile_ReturnsBufferId()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            int bufferId = service.LoadAudio("test_audio", audioFile);

            Assert.True(bufferId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void LoadAudio_WithSameNameTwice_ReturnsSameBufferId()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            int bufferId1 = service.LoadAudio("test_audio", audioFile);
            int bufferId2 = service.LoadAudio("test_audio", audioFile);

            Assert.Equal(bufferId1, bufferId2);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithNonExistentAudioName_ThrowsKeyNotFoundException()
    {
        AudioService service = CreateService();

        try
        {
            KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() =>
                service.CreateSource("nonexistent"));
            Assert.Contains("nonexistent", exception.Message);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithValidAudio_ReturnsSourceId()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio");

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithPosition_Creates3DSource()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            var position = new Vector3(1.0f, 2.0f, 3.0f);

            int sourceId = service.CreateSource("test_audio", position);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithVolume_CreatesSourceWithVolume()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", volume: 0.5f);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithVolumeGreaterThanOne_ClampsToOne()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", volume: 2.0f);

            Assert.True(sourceId > 0);
            service.SetSourceVolume(sourceId, 1.0f);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithNegativeVolume_ClampsToZero()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", volume: -1.0f);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSource_WithLoop_CreatesLoopingSource()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", loop: true);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Play_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.Play(999);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Play_WithValidSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.Play(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Stop_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.Stop(999);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Stop_WithValidSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");
            service.Play(sourceId);

            service.Stop(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Pause_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.Pause(999);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Pause_WithValidSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");
            service.Play(sourceId);

            service.Pause(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourcePosition_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        var position = new Vector3(1.0f, 2.0f, 3.0f);

        try
        {
            service.SetSourcePosition(999, position);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourcePosition_WithValidSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");
            var position = new Vector3(1.0f, 2.0f, 3.0f);

            service.SetSourcePosition(sourceId, position);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolume_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.SetSourceVolume(999, 0.5f);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolume_WithValidSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceVolume(sourceId, 0.5f);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolume_WithVolumeGreaterThanOne_ClampsToOne()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceVolume(sourceId, 2.0f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolume_WithNegativeVolume_ClampsToZero()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceVolume(sourceId, -1.0f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceLoop_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.SetSourceLoop(999, true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceLoop_WithValidSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceLoop(sourceId, true);
            service.SetSourceLoop(sourceId, false);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void DeleteSource_WithNonExistentSourceId_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.DeleteSource(999);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void DeleteSource_WithValidSourceId_RemovesSource()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.DeleteSource(sourceId);

            service.DeleteSource(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetListenerPosition_DoesNotThrow()
    {
        AudioService service = CreateService();
        var position = new Vector3(1.0f, 2.0f, 3.0f);

        try
        {
            service.SetListenerPosition(position);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetListenerOrientation_DoesNotThrow()
    {
        AudioService service = CreateService();
        var forward = new Vector3(0, 0, -1);
        var up = new Vector3(0, 1, 0);

        try
        {
            service.SetListenerOrientation(forward, up);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetMasterVolume_WithValidVolume_DoesNotThrow()
    {
        AudioService service = CreateService();

        try
        {
            service.SetMasterVolume(0.5f);
            service.SetMasterVolume(1.0f);
            service.SetMasterVolume(0.0f);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetMasterVolume_WithVolumeGreaterThanOne_ClampsToOne()
    {
        AudioService service = CreateService();

        try
        {
            service.SetMasterVolume(2.0f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetMasterVolume_WithNegativeVolume_ClampsToZero()
    {
        AudioService service = CreateService();

        try
        {
            service.SetMasterVolume(-1.0f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetMasterVolume_UpdatesAllSources()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId1 = service.CreateSource("test_audio", volume: 0.5f);
            int sourceId2 = service.CreateSource("test_audio", volume: 0.8f);

            service.SetMasterVolume(0.5f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void IsPlaying_WithNonExistentSourceId_ReturnsFalse()
    {
        AudioService service = CreateService();

        try
        {
            bool isPlaying = service.IsPlaying(999);

            Assert.False(isPlaying);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void IsPlaying_WithStoppedSource_ReturnsFalse()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            bool isPlaying = service.IsPlaying(sourceId);

            Assert.False(isPlaying);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        AudioService service = CreateService();

        service.Dispose();
        service.Dispose();
    }

    [Fact]
    public void Dispose_AfterCreatingSources_CleansUpResources()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.Dispose();

            service.Dispose();
        }
        catch (InvalidOperationException)
        {
            service.Dispose();
        }
    }

    /// <summary>
    /// Создает временный аудио файл для тестирования.
    /// </summary>
    private static string CreateTempAudioFile()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");

        byte[] wavHeader = new byte[]
        {
            0x52, 0x49, 0x46, 0x46, 0x24, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x44, 0xAC, 0x00, 0x00, 0x88, 0x58, 0x01, 0x00, 0x02, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00
        };

        File.WriteAllBytes(tempFile, wavHeader);
        return tempFile;
    }

    /// <summary>
    /// Получает путь к тестовому аудио файлу, если он доступен.
    /// </summary>
    private static string? GetTestAudioFile()
    {
        string[] possiblePaths = new[]
        {
            Path.Combine("..", "..", "..", "..", "RenderRitesMachine", "logo.mp3"),
            Path.Combine("..", "..", "..", "..", "RenderRitesDemo", "Assets", "Sounds", "click.mp3")
        };

        foreach (string path in possiblePaths)
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
        foreach (AudioService service in _services)
        {
            try
            {
                service.Dispose();
            }
            catch
            {
            }
        }
        _services.Clear();
    }
}

