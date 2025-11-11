using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using StbImageSharp;

namespace RenderRitesMachine;

/// <summary>
/// Главный класс движка RenderRites. Предоставляет точку входа для создания и управления окном рендеринга.
/// </summary>
public sealed class RenderRites
{
    private static readonly Lazy<RenderRites> LazyMachine = new(() => new RenderRites());

    /// <summary>
    /// Единственный экземпляр движка RenderRites (Singleton).
    /// </summary>
    public static RenderRites Machine => LazyMachine.Value;

    /// <summary>
    /// Текущее окно рендеринга. Может быть null до вызова RunWindow.
    /// </summary>
    public Window? Window { get; private set; }

    /// <summary>
    /// Менеджер сцен для управления различными сценами приложения.
    /// </summary>
    public readonly SceneManager Scenes;

    /// <summary>
    /// Сервис GUI для управления интерфейсом через ImGui.
    /// </summary>
    public readonly GuiService Gui;

    private RenderRites()
    {
        Scenes = new SceneManager();
        Gui = new GuiService();
    }

    /// <summary>
    /// Создает и запускает окно рендеринга с указанными параметрами.
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="vSync">Режим вертикальной синхронизации. По умолчанию Adaptive.</param>
    /// <param name="samples">Количество сэмплов для мультисэмплинга. По умолчанию 4.</param>
    /// <param name="iconPath">Путь к иконке окна. Может быть null.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если title пустой или null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если samples меньше 1 или больше 16.</exception>
    /// <exception cref="FileNotFoundException">Выбрасывается, если iconPath указан, но файл не найден.</exception>
    public void RunWindow(string title, VSyncMode vSync = VSyncMode.Adaptive, int samples = 4, string? iconPath = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Window title cannot be null or empty.", nameof(title));
        }

        if (samples < 1 || samples > 16)
        {
            throw new ArgumentOutOfRangeException(nameof(samples), samples, "Samples must be between 1 and 16.");
        }

        GameWindowSettings gws = new()
        {
            UpdateFrequency = 0.0
        };

        NativeWindowSettings nws = new()
        {
            Title = title,
            ClientSize = new Vector2i(RenderConstants.DefaultWindowWidth, RenderConstants.DefaultWindowHeight),
            Profile = ContextProfile.Core,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6, 0),
            Vsync = vSync,
            NumberOfSamples = samples,
            IsEventDriven = false,
            MinimumClientSize = new Vector2i(RenderConstants.MinWindowWidth, RenderConstants.MinWindowHeight)
        };

        nws.Flags |= ContextFlags.Debug;

        if (iconPath != null)
        {
            if (!File.Exists(iconPath))
            {
                throw new FileNotFoundException("Icon file not found.", iconPath);
            }

            try
            {
                using FileStream iconStream = File.OpenRead(iconPath);
                ImageResult? image = ImageResult.FromStream(iconStream, ColorComponents.RedGreenBlueAlpha);

                if (image == null)
                {
                    throw new InvalidDataException($"Failed to load icon image from {iconPath}.");
                }

                WindowIcon icon = new(new Image(image.Width, image.Height, image.Data));
                nws.Icon = icon;
            }
            catch (Exception ex) when (ex is not FileNotFoundException and not InvalidDataException)
            {
                throw new IOException($"Error reading icon file: {iconPath}", ex);
            }
        }

        Window = new Window(gws, nws);
        Window.Run();
        Gui.Dispose();
        Scenes.Dispose();
    }
}
