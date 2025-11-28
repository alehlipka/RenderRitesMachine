using System.Reflection;
using RenderRitesMachine.Services.Audio;
using RenderRitesMachine.Services.Diagnostics;
using RenderRitesMachine.Services.Graphics;
using RenderRitesMachine.Services.Gui;
using RenderRitesMachine.Services.Timing;

namespace RenderRitesMachine.Output;

/// <summary>
///     Scene factory implementation that wires all required dependencies.
/// </summary>
public class SceneFactory(
    IAssetsService assetsService,
    ITimeService timeService,
    IRenderService renderService,
    IAudioService audioService,
    IGuiService guiService,
    ILogger logger) : ISceneFactory
{
    private readonly IAssetsService _assetsService = assetsService;
    private readonly IAudioService _audioService = audioService;
    private readonly IGuiService _guiService = guiService;
    private readonly ILogger _logger = logger;
    private readonly IRenderService _renderService = renderService;
    private readonly ITimeService _timeService = timeService;
    private ISceneManager? _sceneManager;

    public T CreateScene<T>(string name) where T : Scene
    {
        ConstructorInfo? constructor = typeof(T).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            [
                typeof(string), typeof(IAssetsService), typeof(ITimeService), typeof(IRenderService),
                typeof(IAudioService), typeof(IGuiService), typeof(ISceneManager), typeof(ILogger)
            ],
            null);

        if (constructor == null)
        {
            _logger.LogError(
                $"Type {typeof(T).Name} must have a constructor with parameters: (string, IAssetsService, ITimeService, IRenderService, IAudioService, IGuiService, ISceneManager, ILogger)");
            throw new InvalidOperationException(
                $"Type {typeof(T).Name} must have a constructor with parameters: (string, IAssetsService, ITimeService, IRenderService, IAudioService, IGuiService, ISceneManager, ILogger)");
        }

        if (_sceneManager == null)
        {
            _logger.LogError("SceneManager must be set before creating scenes. Call SetSceneManager first.");
            throw new InvalidOperationException(
                "SceneManager must be set before creating scenes. Call SetSceneManager first.");
        }

        try
        {
            return (T)constructor.Invoke([
                name, _assetsService, _timeService, _renderService, _audioService, _guiService, _sceneManager, _logger
            ]);
        }
        catch (Exception ex)
        {
            _logger.LogException(LogLevel.Error, ex, $"Failed to create scene '{name}' of type {typeof(T).Name}");
            throw;
        }
    }

    /// <summary>
    ///     Assigns the scene manager. Must be called after <see cref="SceneManager" /> creation.
    /// </summary>
    public void SetSceneManager(ISceneManager sceneManager) => _sceneManager = sceneManager;
}
