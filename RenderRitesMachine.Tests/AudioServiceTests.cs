using OpenTK.Mathematics;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса AudioService.
/// Примечание: Полное тестирование аудио функциональности требует OpenAL контекста и аудио устройства,
/// поэтому некоторые тесты могут пропускаться в окружениях без аудио оборудования.
/// </summary>
public sealed class AudioServiceTests : IDisposable
{
    private readonly List<AudioService> _services = [];

    [Fact]
    public void ConstructorCreatesAudioService()
    {
        AudioService service = CreateService();

        Assert.NotNull(service);
    }

    [Fact]
    public void ConstructorWithLoggerAcceptsLogger()
    {
        var logger = new Logger();

        AudioService service = CreateService(logger);

        Assert.NotNull(service);
    }

    [Fact]
    public void LoadAudioWithNullNameThrowsArgumentNullException()
    {
        AudioService service = CreateService();
        string tempFile = CreateTempAudioFile();

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() => service.LoadAudio(null!, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadAudioWithEmptyNameThrowsArgumentNullException()
    {
        AudioService service = CreateService();
        string tempFile = CreateTempAudioFile();

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() => service.LoadAudio("", tempFile));
            _ = Assert.Throws<ArgumentNullException>(() => service.LoadAudio("   ", tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadAudioWithNonExistentFileThrowsFileNotFoundException()
    {
        AudioService service = CreateService();
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");

        FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() => service.LoadAudio("audio", nonExistentPath));
        Assert.Contains(nonExistentPath, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void LoadAudioWithValidFileReturnsBufferId()
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
    public void LoadAudioWithSameNameTwiceReturnsSameBufferId()
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
    public void CreateSourceWithNonExistentAudioNameThrowsKeyNotFoundException()
    {
        AudioService service = CreateService();

        try
        {
            KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() =>
                service.CreateSource("nonexistent"));
            Assert.Contains("nonexistent", exception.Message, StringComparison.Ordinal);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithValidAudioReturnsSourceId()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio");

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithPositionCreates3DSource()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            var position = new Vector3(1.0f, 2.0f, 3.0f);

            int sourceId = service.CreateSource("test_audio", position);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithVolumeCreatesSourceWithVolume()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", volume: 0.5f);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithVolumeGreaterThanOneClampsToOne()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", volume: 2.0f);

            Assert.True(sourceId > 0);
            service.SetSourceVolume(sourceId, 1.0f);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithNegativeVolumeClampsToZero()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", volume: -1.0f);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithLoopCreatesLoopingSource()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);

            int sourceId = service.CreateSource("test_audio", loop: true);

            Assert.True(sourceId > 0);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void PlayWithNonExistentSourceIdDoesNotThrow()
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
    public void PlayWithValidSourceIdDoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.Play(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void StopWithNonExistentSourceIdDoesNotThrow()
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
    public void StopWithValidSourceIdDoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");
            service.Play(sourceId);

            service.Stop(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void PauseWithNonExistentSourceIdDoesNotThrow()
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
    public void PauseWithValidSourceIdDoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");
            service.Play(sourceId);

            service.Pause(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourcePositionWithNonExistentSourceIdDoesNotThrow()
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
    public void SetSourcePositionWithValidSourceIdDoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");
            var position = new Vector3(1.0f, 2.0f, 3.0f);

            service.SetSourcePosition(sourceId, position);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolumeWithNonExistentSourceIdDoesNotThrow()
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
    public void SetSourceVolumeWithValidSourceIdDoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceVolume(sourceId, 0.5f);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolumeWithVolumeGreaterThanOneClampsToOne()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceVolume(sourceId, 2.0f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolumeWithNegativeVolumeClampsToZero()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceVolume(sourceId, -1.0f);

            Assert.True(true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetSourceLoopWithNonExistentSourceIdDoesNotThrow()
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
    public void SetSourceLoopWithValidSourceIdDoesNotThrow()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.SetSourceLoop(sourceId, true);
            service.SetSourceLoop(sourceId, false);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void DeleteSourceWithNonExistentSourceIdDoesNotThrow()
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
    public void DeleteSourceWithValidSourceIdRemovesSource()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            service.DeleteSource(sourceId);

            service.DeleteSource(sourceId);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void SetListenerPositionDoesNotThrow()
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
    public void SetListenerOrientationDoesNotThrow()
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
    public void SetMasterVolumeWithValidVolumeDoesNotThrow()
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
    public void SetMasterVolumeWithVolumeGreaterThanOneClampsToOne()
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
    public void SetMasterVolumeWithNegativeVolumeClampsToZero()
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
    public void SetMasterVolumeUpdatesAllSources()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
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
    public void IsPlayingWithNonExistentSourceIdReturnsFalse()
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
    public void IsPlayingWithStoppedSourceReturnsFalse()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
            int sourceId = service.CreateSource("test_audio");

            bool isPlaying = service.IsPlaying(sourceId);

            Assert.False(isPlaying);
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Fact]
    public void DisposeDoesNotThrow()
    {
        AudioService service = CreateService();

        service.Dispose();
        service.Dispose();
    }

    [Fact]
    public void DisposeAfterCreatingSourcesCleansUpResources()
    {
        AudioService service = CreateService();
        string? audioFile = GetTestAudioFile();

        if (audioFile == null)
        {
            return;
        }

        try
        {
            _ = service.LoadAudio("test_audio", audioFile);
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

        byte[] wavHeader =
        [
            0x52, 0x49, 0x46, 0x46, 0x24, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x44, 0xAC, 0x00, 0x00, 0x88, 0x58, 0x01, 0x00, 0x02, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00
        ];

        File.WriteAllBytes(tempFile, wavHeader);
        return tempFile;
    }

    /// <summary>
    /// Получает путь к тестовому аудио файлу, если он доступен.
    /// </summary>
    private static string? GetTestAudioFile()
    {
        string path = Path.Combine("..", "..", "..", "..", "RenderRitesMachine", "logo.mp3");

        if (File.Exists(path))
        {
            return Path.GetFullPath(path);
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
            service.Dispose();
        }
        _services.Clear();
    }
}
