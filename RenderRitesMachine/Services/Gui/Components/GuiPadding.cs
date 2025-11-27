namespace RenderRitesMachine.Services.Gui.Components;

/// <summary>
/// Represents inner spacing (padding) for a GUI element.
/// </summary>
public struct GuiPadding
{
    public static GuiPadding Zero => new(0);

    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }

    public GuiPadding(int uniform)
        : this(uniform, uniform, uniform, uniform)
    {
    }

    public GuiPadding(int horizontal, int vertical)
        : this(horizontal, vertical, horizontal, vertical)
    {
    }

    public GuiPadding(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public int Horizontal => Left + Right;
    public int Vertical => Top + Bottom;
}


