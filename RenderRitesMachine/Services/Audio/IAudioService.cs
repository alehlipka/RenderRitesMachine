using OpenTK.Mathematics;

namespace RenderRitesMachine.Services;

/// <summary>
/// Интерфейс для сервиса управления аудио через OpenAL.
/// Поддерживает 3D позиционирование звуков, управление громкостью и позицией слушателя.
/// </summary>
public interface IAudioService : IDisposable
{
    /// <summary>
    /// Загружает аудио файл и возвращает его идентификатор для последующего использования.
    /// </summary>
    /// <param name="name">Имя аудио ресурса.</param>
    /// <param name="filePath">Путь к аудио файлу (MP3, WAV и т.д.).</param>
    /// <returns>Идентификатор загруженного аудио ресурса.</returns>
    int LoadAudio(string name, string filePath);

    /// <summary>
    /// Создает новый источник звука и возвращает его идентификатор.
    /// </summary>
    /// <param name="audioName">Имя загруженного аудио ресурса.</param>
    /// <param name="position">Позиция источника звука в 3D пространстве. Если null, звук будет 2D (не позиционированный).</param>
    /// <param name="volume">Громкость звука от 0.0 до 1.0. По умолчанию 1.0.</param>
    /// <param name="loop">Зацикливать ли звук. По умолчанию false.</param>
    /// <returns>Идентификатор созданного источника звука.</returns>
    int CreateSource(string audioName, Vector3? position = null, float volume = 1.0f, bool loop = false);

    /// <summary>
    /// Воспроизводит звук по идентификатору источника.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    void Play(int sourceId);

    /// <summary>
    /// Останавливает воспроизведение звука.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    void Stop(int sourceId);

    /// <summary>
    /// Приостанавливает воспроизведение звука.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    void Pause(int sourceId);

    /// <summary>
    /// Устанавливает позицию источника звука в 3D пространстве.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    /// <param name="position">Новая позиция источника звука.</param>
    void SetSourcePosition(int sourceId, Vector3 position);

    /// <summary>
    /// Устанавливает громкость источника звука.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    /// <param name="volume">Громкость от 0.0 (тишина) до 1.0 (максимум).</param>
    void SetSourceVolume(int sourceId, float volume);

    /// <summary>
    /// Устанавливает, должен ли звук зацикливаться.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    /// <param name="loop">Зацикливать ли звук.</param>
    void SetSourceLoop(int sourceId, bool loop);

    /// <summary>
    /// Удаляет источник звука и освобождает связанные ресурсы.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    void DeleteSource(int sourceId);

    /// <summary>
    /// Устанавливает позицию слушателя (игрока/камеры) в 3D пространстве.
    /// </summary>
    /// <param name="position">Позиция слушателя.</param>
    void SetListenerPosition(Vector3 position);

    /// <summary>
    /// Устанавливает ориентацию слушателя (направление взгляда и вверх).
    /// </summary>
    /// <param name="forward">Вектор направления взгляда (нормализованный).</param>
    /// <param name="up">Вектор "вверх" (нормализованный).</param>
    void SetListenerOrientation(Vector3 forward, Vector3 up);

    /// <summary>
    /// Устанавливает общую громкость (мастер-громкость) для всех звуков.
    /// </summary>
    /// <param name="volume">Громкость от 0.0 (тишина) до 1.0 (максимум).</param>
    void SetMasterVolume(float volume);

    /// <summary>
    /// Проверяет, воспроизводится ли звук в данный момент.
    /// </summary>
    /// <param name="sourceId">Идентификатор источника звука.</param>
    /// <returns>True, если звук воспроизводится, иначе false.</returns>
    bool IsPlaying(int sourceId);
}
