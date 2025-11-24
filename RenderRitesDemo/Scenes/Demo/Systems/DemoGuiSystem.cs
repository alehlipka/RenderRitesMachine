using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine.Debug;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesDemo.Scenes.Demo.Systems;

/// <summary>
/// Simple HUD overlay that demonstrates how to use the GUI surface.
/// </summary>
internal sealed class DemoGuiSystem : IEcsRunSystem
{
    private const int PanelWidth = 240;
    private const int PanelHeight = 110;
    private const int PanelMargin = 16;
    private const int PanelPadding = 10;
    private const int LegendSize = 10;
    private const int BarHeight = 8;

    private static readonly Color4 PanelBackground = new(0.2f, 0.2f, 0.2f, 0.65f);
    private static readonly Color4 PanelBorder = new(1f, 1f, 1f, 0.35f);
    private static readonly Color4 ObjectsColor = new(0.36f, 0.83f, 0.97f, 1f);
    private static readonly Color4 FpsColor = new(0.55f, 0.95f, 0.55f, 1f);
    private static readonly Color4 TimeColor = new(0.97f, 0.58f, 0.35f, 1f);
    private static readonly Color4 BarBackground = new(1f, 1f, 1f, 0.1f);

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        IGuiService gui = shared.Gui;

        if (gui.Width == 0 || gui.Height == 0)
        {
            return;
        }

        int panelX = PanelMargin;
        int panelY = Math.Max(PanelMargin, gui.Height - PanelHeight - PanelMargin);

        DrawPanel(gui, panelX, panelY);
        DrawBars(gui, shared, panelX, panelY);
        DrawCrosshair(gui);
    }

    private static void DrawPanel(IGuiService gui, int x, int y)
    {
        gui.FillRectangle(x, y, PanelWidth, PanelHeight, PanelBackground);

        gui.DrawHorizontalLine(x, y, PanelWidth, 1, PanelBorder);
        gui.DrawHorizontalLine(x, y + PanelHeight - 1, PanelWidth, 1, PanelBorder);
        gui.DrawVerticalLine(x, y, PanelHeight, 1, PanelBorder);
        gui.DrawVerticalLine(x + PanelWidth - 1, y, PanelHeight, 1, PanelBorder);
    }

    private static void DrawBars(IGuiService gui, SystemSharedObject shared, int panelX, int panelY)
    {
        int barWidth = PanelWidth - (PanelPadding * 2) - LegendSize - 6;
        int cursorX = panelX + PanelPadding;
        int cursorY = panelY + PanelPadding;

        RenderStatistics stats = shared.RenderStats;

        float objectsRatio = stats.TotalObjects == 0 ? 0f : stats.RenderedObjects / (float)stats.TotalObjects;
        DrawBar(gui, cursorX, ref cursorY, barWidth, ObjectsColor, objectsRatio);

        float fpsRatio = Math.Clamp((float)(FpsCounter.GetFps() / 144f), 0f, 1f);
        DrawBar(gui, cursorX, ref cursorY, barWidth, FpsColor, fpsRatio);

        float frameBudget = Math.Clamp(shared.Time.RenderDeltaTime * 240f, 0f, 1f);
        DrawBar(gui, cursorX, ref cursorY, barWidth, TimeColor, 1f - frameBudget);
    }

    private static void DrawBar(IGuiService gui, int cursorX, ref int cursorY, int barWidth, Color4 color, float value)
    {
        value = Math.Clamp(value, 0f, 1f);

        gui.FillRectangle(cursorX, cursorY, LegendSize, LegendSize, color);

        int barX = cursorX + LegendSize + 6;
        int barY = cursorY + (LegendSize - BarHeight) / 2;

        gui.FillRectangle(barX, barY, barWidth, BarHeight, BarBackground);
        gui.FillRectangle(barX, barY, Math.Max(1, (int)(barWidth * value)), BarHeight, color);

        cursorY += LegendSize + 12;
    }

    private static void DrawCrosshair(IGuiService gui)
    {
        int centerX = gui.Width / 2;
        int centerY = gui.Height / 2;

        gui.DrawHorizontalLine(centerX - 12, centerY, 24, 1, Color4.White);
        gui.DrawVerticalLine(centerX, centerY - 12, 24, 1, Color4.White);
    }
}

