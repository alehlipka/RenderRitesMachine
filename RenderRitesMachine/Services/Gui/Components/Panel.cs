using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui.Components;

public class Panel : GuiElement
{
    public Color4 BackgroundColor { get; set; } = new(0f, 0f, 0f, 0.6f);
    public Color4 BorderColor { get; set; } = Color4.Transparent;
    public int BorderThickness { get; set; } = 1;

    public override void Render(IGuiService gui, ITimeService time)
    {
        if (!IsVisible)
        {
            return;
        }

        (int x, int y) = GetGlobalPosition();
        Color4 background = ResolveBackgroundColor();
        if (background.A > 0f)
        {
            gui.FillRectangle(x, y, Width, Height, background);
        }

        Color4 border = ResolveBorderColor();
        if (BorderThickness > 0 && border.A > 0f)
        {
            int thickness = BorderThickness;
            gui.DrawHorizontalLine(x, y, Width, thickness, border);
            gui.DrawHorizontalLine(x, y + Height - thickness, Width, thickness, border);
            gui.DrawVerticalLine(x, y, Height, thickness, border);
            gui.DrawVerticalLine(x + Width - thickness, y, Height, thickness, border);
        }

        base.Render(gui, time);
    }

    protected virtual Color4 ResolveBackgroundColor() => BackgroundColor;
    protected virtual Color4 ResolveBorderColor() => BorderColor;
}

