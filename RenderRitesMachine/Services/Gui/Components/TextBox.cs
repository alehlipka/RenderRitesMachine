using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RenderRitesMachine.Services.Gui.Components;

/// <summary>
/// Text input component that supports text editing with cursor navigation.
/// </summary>
public sealed class TextBox : Panel
{
    private string _text = string.Empty;
    private int _cursorPosition;
    private bool _hasFocus;
    private float _scrollOffset;
    private double _cursorBlinkTime;
    private bool _cursorVisible = true;
    private const double CursorBlinkInterval = 0.5;

    public TextBox(GuiFont font)
    {
        Font = font ?? throw new ArgumentNullException(nameof(font));
        TextColor = Color4.White;
        CursorColor = Color4.White;
        FocusBorderColor = new Color4(0.2f, 0.5f, 1f, 1f);
        Padding = new GuiPadding(6);
        _cursorPosition = 0;
    }

    public GuiFont Font { get; set; }
    public Color4 TextColor { get; set; }
    public Color4 CursorColor { get; set; }
    public Color4 FocusBorderColor { get; set; }
    public int MaxLength { get; set; } = 0; // 0 = unlimited
    public string PlaceholderText { get; set; } = string.Empty;
    public Color4 PlaceholderColor { get; set; } = new Color4(0.5f, 0.5f, 0.5f, 1f);
    public event Action<string>? TextChanged;
    public event Action? EnterPressed;

    public string Text
    {
        get => _text;
        set
        {
            string newText = value ?? string.Empty;
            if (MaxLength > 0 && newText.Length > MaxLength)
            {
                newText = newText.Substring(0, MaxLength);
            }

            if (_text != newText)
            {
                _text = newText;
                _cursorPosition = Math.Clamp(_cursorPosition, 0, _text.Length);
                UpdateScrollOffset();
                TextChanged?.Invoke(_text);
            }
        }
    }

    public bool HasFocus => _hasFocus;

    public void SetFocus(bool focus)
    {
        if (_hasFocus == focus)
        {
            return;
        }

        _hasFocus = focus;
        if (_hasFocus)
        {
            _cursorVisible = true;
            _cursorBlinkTime = 0;
        }
    }

    public override void HandleEvent(GuiEvent evt)
    {
        if (!IsVisible)
        {
            return;
        }

        switch (evt.Type)
        {
            case GuiEventType.MouseDown:
                if (HitTest(evt.Position))
                {
                    SetFocus(true);
                    UpdateCursorPositionFromMouse(evt.Position);
                }
                else
                {
                    SetFocus(false);
                }

                break;

            case GuiEventType.KeyDown:
                if (_hasFocus && evt.Key.HasValue)
                {
                    HandleKeyDown(evt.Key.Value);
                }

                break;

            case GuiEventType.TextInput:
                if (_hasFocus && evt.Character.HasValue)
                {
                    HandleTextInput(evt.Character.Value);
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

        if (_hasFocus)
        {
            _cursorBlinkTime += time.RenderDeltaTime;
            if (_cursorBlinkTime >= CursorBlinkInterval)
            {
                _cursorVisible = !_cursorVisible;
                _cursorBlinkTime = 0;
            }
        }
        else
        {
            _cursorVisible = false;
        }

        Color4 originalBorder = BorderColor;
        if (_hasFocus)
        {
            BorderColor = FocusBorderColor;
        }

        base.Render(gui, time);

        BorderColor = originalBorder;

        (int x, int y) = GetGlobalPosition();
        int textX = x + Padding.Left;
        int textY = y + Padding.Top;

        string displayText = _text;
        Color4 displayColor = TextColor;

        if (string.IsNullOrEmpty(_text) && !string.IsNullOrEmpty(PlaceholderText))
        {
            displayText = PlaceholderText;
            displayColor = PlaceholderColor;
        }

        if (!string.IsNullOrEmpty(displayText))
        {
            RenderVisibleText(gui, displayText, displayColor, textX, textY);
        }

        if (_hasFocus && _cursorVisible)
        {
            DrawCursor(gui, textX, textY);
        }
    }

    private void HandleKeyDown(Keys key)
    {
        switch (key)
        {
            case Keys.Backspace:
                if (_cursorPosition > 0)
                {
                    _cursorPosition--;
                    Text = _text.Remove(_cursorPosition, 1);
                    UpdateScrollOffset();
                }

                break;

            case Keys.Delete:
                if (_cursorPosition < _text.Length)
                {
                    Text = _text.Remove(_cursorPosition, 1);
                    UpdateScrollOffset();
                }

                break;

            case Keys.Left:
                if (_cursorPosition > 0)
                {
                    _cursorPosition--;
                    UpdateScrollOffset();
                }

                _cursorVisible = true;
                _cursorBlinkTime = 0;
                break;

            case Keys.Right:
                if (_cursorPosition < _text.Length)
                {
                    _cursorPosition++;
                    UpdateScrollOffset();
                }

                _cursorVisible = true;
                _cursorBlinkTime = 0;
                break;

            case Keys.Home:
                _cursorPosition = 0;
                UpdateScrollOffset();
                _cursorVisible = true;
                _cursorBlinkTime = 0;
                break;

            case Keys.End:
                _cursorPosition = _text.Length;
                UpdateScrollOffset();
                _cursorVisible = true;
                _cursorBlinkTime = 0;
                break;

            case Keys.Enter:
                EnterPressed?.Invoke();
                break;
        }
    }

    private void HandleTextInput(char character)
    {
        if (char.IsControl(character) && character != '\n' && character != '\t')
        {
            return;
        }

        if (MaxLength > 0 && _text.Length >= MaxLength)
        {
            return;
        }

        Text = _text.Insert(_cursorPosition, character.ToString());
        _cursorPosition++;
        UpdateScrollOffset();
        _cursorVisible = true;
        _cursorBlinkTime = 0;
    }

    private void UpdateCursorPositionFromMouse(Vector2 mousePosition)
    {
        (int x, int y) = GetGlobalPosition();
        int textX = x + Padding.Left;
        int relativeX = (int)mousePosition.X - textX + (int)_scrollOffset;

        int bestPosition = 0;
        float bestDistance = float.MaxValue;

        for (int i = 0; i <= _text.Length; i++)
        {
            string substring = i > 0 ? _text.Substring(0, i) : string.Empty;
            float currentX = Font.MeasureText(substring).X;
            float distance = Math.Abs(currentX - relativeX);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPosition = i;
            }
        }

        _cursorPosition = bestPosition;
        UpdateScrollOffset();
        _cursorVisible = true;
        _cursorBlinkTime = 0;
    }

    private void UpdateScrollOffset()
    {
        if (string.IsNullOrEmpty(_text))
        {
            _scrollOffset = 0;
            return;
        }

        string textBeforeCursor = _cursorPosition > 0 ? _text.Substring(0, _cursorPosition) : string.Empty;
        float cursorX = Font.MeasureText(textBeforeCursor).X;

        int visibleWidth = Math.Max(0, Width - Padding.Horizontal);
        float textWidth = Font.MeasureText(_text).X;

        if (textWidth <= visibleWidth)
        {
            _scrollOffset = 0;
            return;
        }

        if (cursorX < _scrollOffset)
        {
            _scrollOffset = Math.Max(0, cursorX - 10);
        }
        else if (cursorX > _scrollOffset + visibleWidth)
        {
            _scrollOffset = cursorX - visibleWidth + 10;
        }

        if (_scrollOffset < 0)
        {
            _scrollOffset = 0;
        }

        float maxScroll = textWidth - visibleWidth;
        if (_scrollOffset > maxScroll)
        {
            _scrollOffset = Math.Max(0, maxScroll);
        }
    }

    private void DrawCursor(IGuiService gui, int textX, int textY)
    {
        string textBeforeCursor = _cursorPosition > 0 ? _text.Substring(0, _cursorPosition) : string.Empty;
        float cursorX = Font.MeasureText(textBeforeCursor).X;

        int cursorPixelX = textX + (int)cursorX - (int)_scrollOffset;
        int cursorHeight = (int)Font.LineHeight;
        int cursorY = textY + (int)Font.Baseline;

        gui.DrawVerticalLine(cursorPixelX, cursorY - cursorHeight + Padding.Top, cursorHeight, 1, CursorColor);
    }

    private void RenderVisibleText(IGuiService gui, string text, Color4 color, int textX, int textY)
    {
        int visibleWidth = Math.Max(0, Width - Padding.Horizontal);
        if (visibleWidth <= 0)
        {
            return;
        }

        float fullWidth = Font.MeasureText(text).X;
        if (fullWidth <= visibleWidth && _scrollOffset <= 0f)
        {
            gui.DrawText(Font, text, textX, textY, color);
            return;
        }

        (int startIndex, int endIndex, float startOffset) = GetVisibleSubstringBounds(text, visibleWidth);
        if (startIndex >= endIndex)
        {
            return;
        }

        string visibleText = text.Substring(startIndex, endIndex - startIndex);
        int drawX = textX - (int)MathF.Round(_scrollOffset - startOffset);
        gui.DrawText(Font, visibleText, drawX, textY, color);
    }

    private (int Start, int End, float StartOffset) GetVisibleSubstringBounds(string text, int visibleWidth)
    {
        float scrollStart = MathF.Max(0f, _scrollOffset);
        float scrollEnd = scrollStart + visibleWidth;

        float currentOffset = 0f;
        int startIndex = 0;
        int endIndex = text.Length;
        float startOffset = 0f;
        bool startFound = false;

        for (int i = 0; i < text.Length; i++)
        {
            float advance = GetGlyphAdvance(text[i]);
            float charStart = currentOffset;
            float charEnd = charStart + advance;

            if (!startFound && charEnd >= scrollStart)
            {
                startIndex = i;
                startOffset = charStart;
                startFound = true;
            }

            if (startFound && charStart >= scrollEnd)
            {
                endIndex = i;
                break;
            }

            currentOffset = charEnd;
        }

        if (!startFound)
        {
            startIndex = text.Length;
            startOffset = currentOffset;
        }

        startIndex = Math.Clamp(startIndex, 0, text.Length);
        endIndex = Math.Clamp(endIndex, startIndex, text.Length);

        return (startIndex, endIndex, startOffset);
    }

    private float GetGlyphAdvance(char ch)
    {
        return Font.TryGetGlyph(ch, out GuiFont.Glyph glyph) ? glyph.XAdvance : 0f;
    }
}

