using NLayer;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для управления аудио через OpenAL.
/// Поддерживает 3D позиционирование звуков, управление громкостью и позицией слушателя.
/// </summary>
public sealed class AudioService : IAudioService
{
    private ALDevice? _audioDevice;
    private ALContext? _audioContext;
    private readonly Dictionary<string, int> _audioBuffers = [];
    private readonly Dictionary<int, int> _sources = [];
    private readonly Dictionary<int, float> _sourceBaseVolumes = [];
    private int _nextSourceId = 1;
    private float _masterVolume = 1.0f;
    private bool _disposed;
    private readonly ILogger? _logger;

    /// <summary>
    /// Создает новый экземпляр AudioService.
    /// </summary>
    /// <param name="logger">Логгер для записи сообщений. Может быть null.</param>
    public AudioService(ILogger? logger = null)
    {
        _logger = logger;
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            _audioDevice = ALC.OpenDevice(null);

            if (!_audioDevice.HasValue)
            {
                _logger?.LogWarning("Unable to create audio device");
                return;
            }

            _audioContext = ALC.CreateContext(_audioDevice.Value, (int[]?)null);
            if (!_audioContext.HasValue)
            {
                _logger?.LogWarning("Unable to create audio context");
                _ = ALC.CloseDevice(_audioDevice.Value);
                _audioDevice = null;
                return;
            }

            _ = ALC.MakeContextCurrent(_audioContext.Value);

            AL.Listener(ALListener3f.Position, 0, 0, 0);
            AL.Listener(ALListener3f.Velocity, 0, 0, 0);
            var forward = new Vector3(0, 0, -1);
            var up = new Vector3(0, 1, 0);
            AL.Listener(ALListenerfv.Orientation, ref forward, ref up);
            AL.Listener(ALListenerf.Gain, _masterVolume);

            _logger?.LogDebug("AudioService initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Failed to initialize AudioService: {ex.Message}");
        }
    }

    public int LoadAudio(string name, string filePath)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Audio name cannot be null or empty.");
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Audio file not found: {filePath}", filePath);
        }

        if (_audioBuffers.TryGetValue(name, out int existingBuffer))
        {
            _logger?.LogDebug($"Audio '{name}' already loaded, reusing buffer");
            return existingBuffer;
        }

        if (!_audioDevice.HasValue || !_audioContext.HasValue)
        {
            throw new InvalidOperationException("AudioService is not initialized");
        }

        try
        {
            using var mpegFile = new MpegFile(filePath);

            int sampleRate = mpegFile.SampleRate;
            int channels = mpegFile.Channels;

            var samplesList = new List<float>();
            float[] buffer = new float[4096];
            int samplesRead;

            while ((samplesRead = mpegFile.ReadSamples(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < samplesRead; i++)
                {
                    samplesList.Add(buffer[i]);
                }
            }

            if (samplesList.Count == 0)
            {
                throw new InvalidDataException("No audio data read from file");
            }

            float[] samples = [.. samplesList];

            byte[] audioData;
            ALFormat alFormat;

            if (channels == 1)
            {
                audioData = new byte[samples.Length * 2];
                for (int i = 0; i < samples.Length; i++)
                {
                    float sample = Math.Clamp(samples[i], -1.0f, 1.0f);
                    short sample16 = (short)(sample * 32767.0f);
                    audioData[i * 2] = (byte)(sample16 & 0xFF);
                    audioData[(i * 2) + 1] = (byte)((sample16 >> 8) & 0xFF);
                }
                alFormat = ALFormat.Mono16;
            }
            else
            {
                int monoSampleCount = samples.Length / channels;
                audioData = new byte[monoSampleCount * 2];

                for (int i = 0; i < monoSampleCount; i++)
                {
                    float monoSample = 0.0f;
                    for (int ch = 0; ch < channels; ch++)
                    {
                        monoSample += samples[(i * channels) + ch];
                    }
                    monoSample /= channels;

                    monoSample = Math.Clamp(monoSample, -1.0f, 1.0f);
                    short sample16 = (short)(monoSample * 32767.0f);
                    audioData[i * 2] = (byte)(sample16 & 0xFF);
                    audioData[(i * 2) + 1] = (byte)((sample16 >> 8) & 0xFF);
                }
                alFormat = ALFormat.Mono16;
            }

            int alBuffer = AL.GenBuffer();
            AL.BufferData(alBuffer, alFormat, audioData, sampleRate);

            ALError error = AL.GetError();
            if (error != ALError.NoError)
            {
                AL.DeleteBuffer(alBuffer);
                throw new InvalidOperationException($"OpenAL error after BufferData: {error}");
            }

            _audioBuffers[name] = alBuffer;
            _logger?.LogDebug($"Loaded audio '{name}' from '{filePath}' ({audioData.Length} bytes, {sampleRate}Hz, {channels}ch -> mono)");

            return alBuffer;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Failed to load audio file '{filePath}': {ex.Message}");
            throw;
        }
    }

    public int CreateSource(string audioName, Vector3? position = null, float volume = 1.0f, bool loop = false)
    {
        if (!_audioBuffers.TryGetValue(audioName, out int bufferId))
        {
            throw new KeyNotFoundException($"Audio '{audioName}' not found. Load it first using LoadAudio.");
        }

        if (!_audioDevice.HasValue || !_audioContext.HasValue)
        {
            throw new InvalidOperationException("AudioService is not initialized");
        }

        int sourceId = _nextSourceId++;
        int alSource = AL.GenSource();

        AL.Source(alSource, ALSourcei.Buffer, bufferId);

        if (position.HasValue)
        {
            AL.Source(alSource, ALSource3f.Position, position.Value.X, position.Value.Y, position.Value.Z);
        }
        else
        {
            AL.Source(alSource, ALSourceb.SourceRelative, true);
            AL.Source(alSource, ALSource3f.Position, 0, 0, 0);
        }

        float clampedVolume = Math.Clamp(volume, 0.0f, 1.0f);
        AL.Source(alSource, ALSourcef.Gain, clampedVolume * _masterVolume);
        AL.Source(alSource, ALSourceb.Looping, loop);

        _sources[sourceId] = alSource;
        _sourceBaseVolumes[sourceId] = clampedVolume;
        _logger?.LogDebug($"Created audio source {sourceId} for '{audioName}'");

        return sourceId;
    }

    public void Play(int sourceId)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            _logger?.LogWarning($"Audio source {sourceId} not found");
            return;
        }

        AL.SourcePlay(alSource);
    }

    public void Stop(int sourceId)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            return;
        }

        AL.SourceStop(alSource);
    }

    public void Pause(int sourceId)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            return;
        }

        AL.SourcePause(alSource);
    }

    public void SetSourcePosition(int sourceId, Vector3 position)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            _logger?.LogWarning($"Audio source {sourceId} not found");
            return;
        }

        AL.Source(alSource, ALSourceb.SourceRelative, false);
        AL.Source(alSource, ALSource3f.Position, position.X, position.Y, position.Z);
    }

    public void SetSourceVolume(int sourceId, float volume)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            _logger?.LogWarning($"Audio source {sourceId} not found");
            return;
        }

        float clampedVolume = Math.Clamp(volume, 0.0f, 1.0f);
        _sourceBaseVolumes[sourceId] = clampedVolume;
        AL.Source(alSource, ALSourcef.Gain, clampedVolume * _masterVolume);
    }

    public void SetSourceLoop(int sourceId, bool loop)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            _logger?.LogWarning($"Audio source {sourceId} not found");
            return;
        }

        AL.Source(alSource, ALSourceb.Looping, loop);
    }

    public void DeleteSource(int sourceId)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            return;
        }

        AL.SourceStop(alSource);
        AL.DeleteSource(alSource);
        _ = _sources.Remove(sourceId);
        _ = _sourceBaseVolumes.Remove(sourceId);
    }

    public void SetListenerPosition(Vector3 position)
    {
        if (!_audioDevice.HasValue || !_audioContext.HasValue)
        {
            return;
        }

        AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
    }

    public void SetListenerOrientation(Vector3 forward, Vector3 up)
    {
        if (!_audioDevice.HasValue || !_audioContext.HasValue)
        {
            return;
        }

        AL.Listener(ALListenerfv.Orientation, ref forward, ref up);
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Math.Clamp(volume, 0.0f, 1.0f);

        if (!_audioDevice.HasValue || !_audioContext.HasValue)
        {
            return;
        }

        AL.Listener(ALListenerf.Gain, _masterVolume);

        foreach (KeyValuePair<int, int> kvp in _sources)
        {
            int sourceId = kvp.Key;
            int alSource = kvp.Value;

            if (_sourceBaseVolumes.TryGetValue(sourceId, out float baseVolume))
            {
                AL.Source(alSource, ALSourcef.Gain, baseVolume * _masterVolume);
            }
        }
    }

    public bool IsPlaying(int sourceId)
    {
        if (!_sources.TryGetValue(sourceId, out int alSource))
        {
            return false;
        }

        AL.GetSource(alSource, ALGetSourcei.SourceState, out int state);
        return (ALSourceState)state == ALSourceState.Playing;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (int sourceId in _sources.Keys.ToList())
        {
            DeleteSource(sourceId);
        }

        foreach (int bufferId in _audioBuffers.Values)
        {
            AL.DeleteBuffer(bufferId);
        }

        _audioBuffers.Clear();
        _sources.Clear();

        if (_audioDevice.HasValue && _audioContext.HasValue)
        {
            try
            {
                ALC.DestroyContext(_audioContext.Value);
                _ = ALC.CloseDevice(_audioDevice.Value);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error disposing AudioService: {ex.Message}");
            }
        }

        _audioDevice = null;
        _audioContext = null;
        _disposed = true;
    }
}
