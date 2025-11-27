using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui.Components;

public sealed class Button : Panel
{
    private string _text = string.Empty;
    private bool _isHovered;
    private bool _isPressed;

    public Button(GuiFont font)
    {
        Font = font ?? throw new ArgumentNullException(nameof(font));
        TextColor = Color4.White;
        HoverBackgroundColor = new Color4(0.35f, 0.35f, 0.35f, 0.8f);
        PressedBackgroundColor = new Color4(0.2f, 0.2f, 0.2f, 0.9f);
        Padding = new GuiPadding(6);
    }

    public GuiFont Font { get; set; }
    public Color4 TextColor { get; set; }
    public Color4 HoverBackgroundColor { get; set; }
    public Color4 PressedBackgroundColor { get; set; }
    public event Action? Clicked;

    public string Text
    {
        get => _text;
        set => _text = value ?? string.Empty;
    }

    protected override Color4 ResolveBackgroundColor()
    {
        if (_isPressed)
        {
            return PressedBackgroundColor;
        }

        if (_isHovered)
        {
            return HoverBackgroundColor;
        }

        return base.ResolveBackgroundColor();
    }

    public override void HandleEvent(GuiEvent evt)
    {
        if (!IsVisible)
        {
            return;
        }

        switch (evt.Type)
        {
            case GuiEventType.MouseMove:
                _isHovered = HitTest(evt.Position);
                if (!_isHovered)
                {
                    _isPressed = false;
                }

                break;

            case GuiEventType.MouseDown:
                if (HitTest(evt.Position))
                {
                    _isHovered = true;
                    _isPressed = true;
                }

                break;

            case GuiEventType.MouseUp:
                bool wasPressed = _isPressed;
                _isPressed = false;
                if (wasPressed && HitTest(evt.Position))
                {
                    Clicked?.Invoke();
                }

                break;
        }

        base.HandleEvent(evt);
    }

    public override void Render(IGuiService gui, ITimeService time)
    {
        if (!IsVisible)
        {
            return;
        }

        base.Render(gui, time);

        (int x, int y) = GetGlobalPosition();
        Vector2 textSize = Font.MeasureText(Text);

        int contentWidth = Math.Max(0, Width - Padding.Horizontal);
        int contentHeight = Math.Max(0, Height - Padding.Vertical);
        int textX = x + Padding.Left + Math.Max(0, (contentWidth - (int)Math.Ceiling(textSize.X)) / 2);
        int textY = y + Padding.Top + Math.Max(0, (contentHeight - (int)Math.Ceiling(textSize.Y)) / 2);

        gui.DrawText(Font, Text, textX, textY, TextColor);
    }
}

