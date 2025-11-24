using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui.Components;

public sealed class Label : GuiElement
{
    private string _text = string.Empty;

    public Label(GuiFont font)
    {
        Font = font ?? throw new ArgumentNullException(nameof(font));
        TextColor = Color4.White;
    }

    public GuiFont Font { get; set; }
    public Color4 TextColor { get; set; }
    public bool AutoSize { get; set; } = true;

    public string Text
    {
        get => _text;
        set => _text = value ?? string.Empty;
    }

    public override void Render(IGuiService gui)
    {
        if (!Visible)
        {
            return;
        }

        (int x, int y) = GetGlobalPosition();

        if (AutoSize)
        {
            Vector2 size = Font.MeasureText(Text);
            Width = (int)Math.Ceiling(size.X);
            Height = (int)Math.Ceiling(size.Y == 0 ? Font.LineHeight : size.Y);
        }

        gui.DrawText(Font, Text, x, y, TextColor);
        base.Render(gui);
    }
}

