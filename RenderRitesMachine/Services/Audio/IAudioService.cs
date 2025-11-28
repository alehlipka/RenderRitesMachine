using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Audio;

/// <summary>
///     OpenAL audio service interface with 3D positioning, volume control, and listener management.
/// </summary>
public interface IAudioService : IDisposable
{
    /// <summary>
    ///     Loads an audio file and returns its identifier.
    /// </summary>
    /// <param name="name">Logical audio name.</param>
    /// <param name="filePath">Path to the audio file (MP3, WAV, etc.).</param>
    /// <returns>Identifier of the loaded audio resource.</returns>
    int LoadAudio(string name, string filePath);

    /// <summary>
    ///     Creates a new sound source and returns its identifier.
    /// </summary>
    /// <param name="audioName">Name of the loaded audio resource.</param>
    /// <param name="position">3D position of the source; null for non-positional audio.</param>
    /// <param name="volume">Volume from 0.0 to 1.0 (default 1.0).</param>
    /// <param name="loop">Whether the source loops (default false).</param>
    /// <returns>Identifier of the created source.</returns>
    int CreateSource(string audioName, Vector3? position = null, float volume = 1.0f, bool loop = false);

    /// <summary>
    ///     Starts playback for the specified source.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    void Play(int sourceId);

    /// <summary>
    ///     Stops playback for the specified source.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    void Stop(int sourceId);

    /// <summary>
    ///     Pauses playback for the specified source.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    void Pause(int sourceId);

    /// <summary>
    ///     Sets the 3D position for a sound source.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    /// <param name="position">New source position.</param>
    void SetSourcePosition(int sourceId, Vector3 position);

    /// <summary>
    ///     Adjusts the volume for a sound source.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    /// <param name="volume">Volume in the range [0,1].</param>
    void SetSourceVolume(int sourceId, float volume);

    /// <summary>
    ///     Enables or disables looping for a sound source.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    /// <param name="loop">Whether the source loops.</param>
    void SetSourceLoop(int sourceId, bool loop);

    /// <summary>
    ///     Deletes a sound source and releases its resources.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    void DeleteSource(int sourceId);

    /// <summary>
    ///     Sets the listener (player/camera) position.
    /// </summary>
    /// <param name="position">Listener position.</param>
    void SetListenerPosition(Vector3 position);

    /// <summary>
    ///     Sets the listener orientation (forward and up vectors).
    /// </summary>
    /// <param name="forward">Normalized forward vector.</param>
    /// <param name="up">Normalized up vector.</param>
    void SetListenerOrientation(Vector3 forward, Vector3 up);

    /// <summary>
    ///     Sets the master volume applied to all sounds.
    /// </summary>
    /// <param name="volume">Volume in the range [0,1].</param>
    void SetMasterVolume(float volume);

    /// <summary>
    ///     Checks whether the specified source is playing.
    /// </summary>
    /// <param name="sourceId">Sound source identifier.</param>
    /// <returns>True if the source is playing; otherwise false.</returns>
    bool IsPlaying(int sourceId);
}
