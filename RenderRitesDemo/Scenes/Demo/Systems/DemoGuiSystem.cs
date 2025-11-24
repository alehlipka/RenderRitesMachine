using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine.Debug;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services.Gui;
using RenderRitesMachine.Services.Gui.Components;

namespace RenderRitesDemo.Scenes.Demo.Systems;

internal sealed class DemoGuiSystem : IEcsRunSystem
{
    private const int PanelMargin = 16;
    private readonly Panel _rootPanel;
    private readonly Label _fpsLabel;
    private readonly Label _objectsLabel;
    private readonly Label _frameTimeLabel;
    private readonly Button _toggleCrosshairButton;
    private readonly List<GuiEvent> _eventBuffer = new();
    private bool _showCrosshair = true;

    public DemoGuiSystem(GuiFont font)
    {
        ArgumentNullException.ThrowIfNull(font);

        _rootPanel = new Panel
        {
            Width = 260,
            Height = 110,
            BackgroundColor = new Color4(0.12f, 0.12f, 0.12f, 0.75f),
            BorderColor = new Color4(1f, 1f, 1f, 0.35f)
        };

        _fpsLabel = new Label(font)
        {
            Position = new Vector2i(10, 10),
            TextColor = Color4.White
        };

        _objectsLabel = new Label(font)
        {
            Position = new Vector2i(10, 30),
            TextColor = Color4.White
        };

        _frameTimeLabel = new Label(font)
        {
            Position = new Vector2i(10, 50),
            TextColor = Color4.White
        };

        _toggleCrosshairButton = new Button(font)
        {
            Position = new Vector2i(10, 70),
            Width = 200,
            Height = 26,
            Text = "Toggle crosshair",
            BackgroundColor = new Color4(0.25f, 0.25f, 0.25f, 0.7f),
            HoverBackgroundColor = new Color4(0.35f, 0.35f, 0.35f, 0.85f),
            PressedBackgroundColor = new Color4(0.18f, 0.18f, 0.18f, 0.95f)
        };
        _toggleCrosshairButton.Clicked += () => _showCrosshair = !_showCrosshair;

        _rootPanel.AddChild(_fpsLabel);
        _rootPanel.AddChild(_objectsLabel);
        _rootPanel.AddChild(_frameTimeLabel);
        _rootPanel.AddChild(_toggleCrosshairButton);
    }

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        IGuiService gui = shared.Gui;

        if (gui.Width == 0 || gui.Height == 0)
        {
            return;
        }

        int panelY = Math.Max(PanelMargin, gui.Height - _rootPanel.Height - PanelMargin);
        _rootPanel.Position = new Vector2i(PanelMargin, panelY);

        RenderStatistics stats = shared.RenderStats;
        _fpsLabel.Text = $"FPS: {FpsCounter.GetFps():F0}";
        _objectsLabel.Text = $"Objects: {stats.RenderedObjects}/{stats.TotalObjects}";
        _frameTimeLabel.Text = $"Frame: {shared.Time.RenderDeltaTime * 1000f:F2} ms";

        IReadOnlyList<GuiEvent> events = gui.Events.DrainToList(_eventBuffer);
        foreach (GuiEvent evt in events)
        {
            _rootPanel.HandleEvent(evt);
        }

        _rootPanel.Render(gui);

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

