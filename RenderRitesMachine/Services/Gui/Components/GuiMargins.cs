namespace RenderRitesMachine.Services.Gui.Components;

/// <summary>
/// Simple helper that represents outer spacing for a GUI element.
/// </summary>
public struct GuiMargins
{
    public static GuiMargins Zero => new(0);

    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }

    public GuiMargins(int uniform)
        : this(uniform, uniform, uniform, uniform)
    {
    }

    public GuiMargins(int horizontal, int vertical)
        : this(horizontal, vertical, horizontal, vertical)
    {
    }

    public GuiMargins(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public int Horizontal => Left + Right;
    public int Vertical => Top + Bottom;
}


