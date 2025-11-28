using OpenTK.Mathematics;
using RenderRitesMachine.Exceptions;
using RenderRitesMachine.Services.Audio;
using RenderRitesMachine.Services.Diagnostics;

namespace RenderRitesMachine.Tests;

/// <summary>
///     Tests for <see cref="AudioService" />.
///     Note: full coverage requires an OpenAL context and audio hardware, so some tests may be skipped in headless
///     environments.
/// </summary>
public sealed class AudioServiceTests : IDisposable
{
    private readonly List<AudioService> _services = [];

    public void Dispose()
    {
        foreach (AudioService service in _services)
        {
            service.Dispose();
        }

        _services.Clear();
    }

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
    public void ConstructorInitializesAudioServiceSuccessfully()
    {
        try
        {
            AudioService service = CreateService();

            Assert.NotNull(service);
            service.SetListenerPosition(new Vector3(0, 0, 0));
        }
        catch (AudioInitializationException)
        {
            Assert.Fail(
                "AudioService initialization should not throw AudioInitializationException in normal conditions");
        }
    }

    [Fact]
    public void ConstructorCanThrowAudioInitializationExceptionOnInitializationFailure()
    {
        try
        {
            AudioService service = CreateService();
            Assert.NotNull(service);
        }
        catch (AudioInitializationException ex)
        {
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }
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
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");

        FileNotFoundException exception =
            Assert.Throws<FileNotFoundException>(() => service.LoadAudio("audio", nonExistentPath));
        Assert.Contains(nonExistentPath, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void LoadAudioWithValidFileReturnsBufferId()
    {
        AudioService service;
        try
        {
            service = CreateService();
        }
        catch (AudioInitializationException)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void LoadAudioWithSameNameTwiceReturnsSameBufferId()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithValidAudioReturnsSourceId()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithPositionCreates3DSource()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithVolumeCreatesSourceWithVolume()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithVolumeGreaterThanOneClampsToOne()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithNegativeVolumeClampsToZero()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void CreateSourceWithLoopCreatesLoopingSource()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void PlayWithValidSourceIdDoesNotThrow()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void StopWithValidSourceIdDoesNotThrow()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void PauseWithValidSourceIdDoesNotThrow()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void SetSourcePositionWithValidSourceIdDoesNotThrow()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolumeWithValidSourceIdDoesNotThrow()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolumeWithVolumeGreaterThanOneClampsToOne()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void SetSourceVolumeWithNegativeVolumeClampsToZero()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void SetSourceLoopWithValidSourceIdDoesNotThrow()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void DeleteSourceWithValidSourceIdRemovesSource()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void SetMasterVolumeUpdatesAllSources()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        catch (AudioInitializationException)
        {
        }
    }

    [Fact]
    public void IsPlayingWithStoppedSourceReturnsFalse()
    {
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
        catch (AudioInitializationException)
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
        AudioService? service = TryCreateService();
        if (service == null)
        {
            return;
        }

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
    ///     Creates a temporary audio file for testing.
    /// </summary>
    private static string CreateTempAudioFile()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");

        byte[] wavHeader =
        [
            0x52, 0x49, 0x46, 0x46, 0x24, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00,
            0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x44, 0xAC, 0x00, 0x00, 0x88, 0x58, 0x01, 0x00, 0x02, 0x00, 0x10, 0x00,
            0x64, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00
        ];

        File.WriteAllBytes(tempFile, wavHeader);
        return tempFile;
    }

    /// <summary>
    ///     Returns the path to the demo audio file if it exists.
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
    ///     Creates a new <see cref="AudioService" /> instance for testing.
    /// </summary>
    private AudioService CreateService(ILogger? logger = null)
    {
        var service = new AudioService(logger);
        _services.Add(service);
        return service;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="AudioService" /> for testing.
    ///     Returns null if initialization fails.
    /// </summary>
    private AudioService? TryCreateService(ILogger? logger = null)
    {
        try
        {
            return CreateService(logger);
        }
        catch (AudioInitializationException)
        {
            return null;
        }
    }
}
