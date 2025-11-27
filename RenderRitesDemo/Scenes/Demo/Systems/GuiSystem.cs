using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine.Debug;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;
using RenderRitesMachine.Services.Gui.Components;

namespace RenderRitesDemo.Scenes.Demo.Systems;

internal sealed class GuiSystem : IEcsRunSystem
{
    private readonly Panel _rootPanel;
    private readonly Label _fpsLabel;
    private readonly Label _objectsLabel;
    private readonly Label _frameTimeLabel;
    private readonly Button _toggleCrosshairButton;
    private readonly TextBox _textBox;
    private readonly List<GuiEvent> _eventBuffer = [];
    private bool _showCrosshair = true;

    public GuiSystem(GuiFont font)
    {
        ArgumentNullException.ThrowIfNull(font);

        _rootPanel = new Panel
        {
            Width = 260,
            Height = 165,
            BackgroundColor = new Color4(0.12f, 0.12f, 0.12f, 0.75f),
            BorderColor = new Color4(1f, 1f, 1f, 0.35f),
            Margin = new GuiMargins(50),
            Padding = new GuiPadding(10),
            Debug = true
        };

        _fpsLabel = new Label(font)
        {
            Position = new Vector2i(0, 0),
            TextColor = Color4.White
        };

        _objectsLabel = new Label(font)
        {
            Position = new Vector2i(0, 20),
            TextColor = Color4.White
        };

        _frameTimeLabel = new Label(font)
        {
            Position = new Vector2i(0, 40),
            TextColor = Color4.White
        };

        _toggleCrosshairButton = new Button(font)
        {
            Position = new Vector2i(0, 65),
            Height = 30,
            Width = _rootPanel.Width - _rootPanel.Padding.Horizontal,
            Text = "Toggle crosshair",
            BackgroundColor = new Color4(0.25f, 0.25f, 0.25f, 0.7f),
            HoverBackgroundColor = new Color4(0.35f, 0.35f, 0.35f, 0.85f),
            PressedBackgroundColor = new Color4(0.18f, 0.18f, 0.18f, 0.95f)
        };
        _toggleCrosshairButton.Clicked += () => _showCrosshair = !_showCrosshair;

        _textBox = new TextBox(font)
        {
            Position = new Vector2i(0, 105),
            Height = 38,
            PlaceholderText = "Enter text here...",
            BackgroundColor = new Color4(0.15f, 0.15f, 0.15f, 0.9f),
            BorderColor = new Color4(0.5f, 0.5f, 0.5f, 0.8f),
            TextColor = Color4.White,
            FocusBorderColor = new Color4(0.2f, 0.6f, 1f, 1f),
            HorizontalAnchor = GuiHorizontalAnchor.Stretch
        };

        _rootPanel.AddChild(_fpsLabel);
        _rootPanel.AddChild(_objectsLabel);
        _rootPanel.AddChild(_frameTimeLabel);
        _rootPanel.AddChild(_toggleCrosshairButton);
        _rootPanel.AddChild(_textBox);
    }

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        IGuiService gui = shared.Gui;
        ITimeService time = shared.Time;

        if (gui.Width == 0 || gui.Height == 0)
        {
            return;
        }

        _rootPanel.UpdateAdaptiveLayout(new Vector2i(gui.Width, gui.Height));

        RenderStatistics stats = shared.RenderStats;
        _fpsLabel.Text = $"FPS: {FpsCounter.GetFps():F0}";
        _objectsLabel.Text = $"Objects: {stats.RenderedObjects}/{stats.TotalObjects}";
        _frameTimeLabel.Text = $"Frame time: {FrameTimeCounter.GetFrameTimeMilliseconds():F4} ms";

        IReadOnlyList<GuiEvent> events = gui.Events.DrainToList(_eventBuffer);
        foreach (GuiEvent evt in events)
        {
            _rootPanel.HandleEvent(evt);
        }

        _rootPanel.Render(gui, time);

        if (_showCrosshair)
        {
            DrawCrosshair(gui);
        }
    }

    private static void DrawCrosshair(IGuiService gui)
    {
        int centerX = gui.Width / 2;
        int centerY = gui.Height / 2;

        gui.DrawHorizontalLine(centerX - 12, centerY, 24, 1, Color4.White);
        gui.DrawVerticalLine(centerX, centerY - 12, 24, 1, Color4.White);
    }
}

