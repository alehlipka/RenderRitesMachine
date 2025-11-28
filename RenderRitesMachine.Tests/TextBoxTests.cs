using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Services.Gui;
using RenderRitesMachine.Services.Gui.Components;
using RenderRitesMachine.Services.Timing;

namespace RenderRitesMachine.Tests;

public sealed class TextBoxTests
{
    private static GuiFont CreateMockFont()
    {
        try
        {
            string fontPath = Path.Combine("..", "..", "..", "..", "RenderRitesDemo", "Assets", "Fonts", "arial.ttf");
            if (File.Exists(fontPath))
            {
                return GuiFont.LoadFromFile(fontPath);
            }
        }
        catch
        {
        }

        return CreateSimpleMockFont();
    }

    private static GuiFont CreateSimpleMockFont()
    {
        byte[] fontData = new byte[1024];
        return GuiFont.LoadFromMemory(fontData, 20, 64, 64);
    }

    private static TextBox CreateTextBox(GuiFont? font = null)
    {
        font ??= CreateMockFont();
        return new TextBox(font);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        GuiFont font = CreateMockFont();
        var textBox = new TextBox(font);

        Assert.Equal(string.Empty, textBox.Text);
        Assert.False(textBox.HasFocus);
        Assert.Equal(0, textBox.MaxLength);
        Assert.Equal(string.Empty, textBox.PlaceholderText);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenFontIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new TextBox(null!));

    [Fact]
    public void TextInput_AddsCharacterAtCursor()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);

        textBox.HandleEvent(GuiEvent.TextInput('H'));
        textBox.HandleEvent(GuiEvent.TextInput('e'));
        textBox.HandleEvent(GuiEvent.TextInput('l'));
        textBox.HandleEvent(GuiEvent.TextInput('l'));
        textBox.HandleEvent(GuiEvent.TextInput('o'));

        Assert.Equal("Hello", textBox.Text);
    }

    [Fact]
    public void TextInput_DoesNotAddCharacter_WhenNoFocus()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(false);

        textBox.HandleEvent(GuiEvent.TextInput('H'));

        Assert.Equal(string.Empty, textBox.Text);
    }

    [Fact]
    public void Backspace_RemovesCharacterBeforeCursor()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.End));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Backspace));

        Assert.Equal("Hell", textBox.Text);
    }

    [Fact]
    public void Backspace_RemovesCharacterAtMiddlePosition()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);

        textBox.HandleEvent(GuiEvent.TextInput('H'));
        textBox.HandleEvent(GuiEvent.TextInput('e'));
        textBox.HandleEvent(GuiEvent.TextInput('l'));
        textBox.HandleEvent(GuiEvent.TextInput('l'));
        textBox.HandleEvent(GuiEvent.TextInput('o'));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Left));
        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Left));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Backspace));

        Assert.Equal("Helo", textBox.Text);
    }

    [Fact]
    public void Backspace_DoesNothing_WhenCursorAtStart()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Home));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Backspace));

        Assert.Equal("Hello", textBox.Text);
    }

    [Fact]
    public void Backspace_DoesNothing_WhenTextIsEmpty()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Backspace));

        Assert.Equal(string.Empty, textBox.Text);
    }

    [Fact]
    public void Delete_RemovesCharacterAfterCursor()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);

        textBox.HandleEvent(GuiEvent.TextInput('H'));
        textBox.HandleEvent(GuiEvent.TextInput('e'));
        textBox.HandleEvent(GuiEvent.TextInput('l'));
        textBox.HandleEvent(GuiEvent.TextInput('l'));
        textBox.HandleEvent(GuiEvent.TextInput('o'));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Home));
        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Right));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Delete));

        Assert.Equal("Hllo", textBox.Text);
    }

    [Fact]
    public void Delete_DoesNothing_WhenCursorAtEnd()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.End));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Delete));

        Assert.Equal("Hello", textBox.Text);
    }

    [Fact]
    public void LeftArrow_MovesCursorLeft()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.End));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Left));

        textBox.HandleEvent(GuiEvent.TextInput('X'));

        Assert.Equal("HellXo", textBox.Text);
    }

    [Fact]
    public void RightArrow_MovesCursorRight()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Home));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Right));

        textBox.HandleEvent(GuiEvent.TextInput('X'));

        Assert.Equal("HXello", textBox.Text);
    }

    [Fact]
    public void Home_MovesCursorToStart()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.End));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Home));

        textBox.HandleEvent(GuiEvent.TextInput('X'));

        Assert.Equal("XHello", textBox.Text);
    }

    [Fact]
    public void End_MovesCursorToEnd()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.Text = "Hello";

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Home));

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.End));

        textBox.HandleEvent(GuiEvent.TextInput('X'));

        Assert.Equal("HelloX", textBox.Text);
    }

    [Fact]
    public void MaxLength_TruncatesText()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.MaxLength = 5;

        textBox.Text = "Hello World";

        Assert.Equal("Hello", textBox.Text);
    }

    [Fact]
    public void MaxLength_PreventsInput_WhenLimitReached()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        textBox.MaxLength = 5;

        textBox.Text = "Hello";
        textBox.HandleEvent(GuiEvent.TextInput('X'));

        Assert.Equal("Hello", textBox.Text);
    }

    [Fact]
    public void TextChanged_EventFires_WhenTextChanges()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        string? changedText = null;

        textBox.TextChanged += text => changedText = text;

        textBox.Text = "Hello";

        Assert.Equal("Hello", changedText);
    }

    [Fact]
    public void EnterPressed_EventFires_WhenEnterPressed()
    {
        TextBox textBox = CreateTextBox();
        textBox.SetFocus(true);
        bool enterPressed = false;

        textBox.EnterPressed += () => enterPressed = true;

        textBox.HandleEvent(GuiEvent.KeyDown(Keys.Enter));

        Assert.True(enterPressed);
    }

    [Fact]
    public void SetFocus_UpdatesHasFocus()
    {
        TextBox textBox = CreateTextBox();

        Assert.False(textBox.HasFocus);

        textBox.SetFocus(true);
        Assert.True(textBox.HasFocus);

        textBox.SetFocus(false);
        Assert.False(textBox.HasFocus);
    }

    [Fact]
    public void PlaceholderText_ShowsWhenTextIsEmpty()
    {
        TextBox textBox = CreateTextBox();
        textBox.PlaceholderText = "Enter text...";
        textBox.Text = string.Empty;

        Assert.Equal(string.Empty, textBox.Text);
        Assert.Equal("Enter text...", textBox.PlaceholderText);
    }

    [Fact]
    public void Render_LongText_IsClippedWithinBounds()
    {
        GuiFont font = CreateMockFont();
        TextBox textBox = new(font)
        {
            Width = 80,
            Height = 30,
            Padding = new GuiPadding(4)
        };

        textBox.SetFocus(true);
        string longText = new('A', 64);
        foreach (char ch in longText)
        {
            textBox.HandleEvent(GuiEvent.TextInput(ch));
        }

        var gui = new RecordingGuiService();
        textBox.Render(gui, new FakeTimeService());

        Assert.NotNull(gui.LastDrawText);
        Assert.NotEqual(longText, gui.LastDrawText!.Value.Text);
        Assert.True(gui.LastDrawText!.Value.Text.Length < longText.Length);
    }

    [Fact]
    public void Render_LongPlaceholder_IsClippedEvenWithoutScroll()
    {
        GuiFont font = CreateMockFont();
        TextBox textBox = new(font)
        {
            Width = 70,
            Height = 28,
            Padding = new GuiPadding(4),
            PlaceholderText = new string('B', 40)
        };

        var gui = new RecordingGuiService();
        textBox.Render(gui, new FakeTimeService());

        Assert.NotNull(gui.LastDrawText);
        Assert.NotEqual(textBox.PlaceholderText, gui.LastDrawText!.Value.Text);
        Assert.True(gui.LastDrawText!.Value.Text.Length < textBox.PlaceholderText.Length);
    }

    private sealed class RecordingGuiService : IGuiService
    {
        public (GuiFont Font, string Text, int X, int Y, Color4 Color)? LastDrawText { get; private set; }

        public GuiEventQueue Events { get; } = new();
        public bool HasContent => false;
        public int Width => 0;
        public int Height => 0;

        public void BeginFrame(Color4 clearColor)
        {
        }

        public void Dispose()
        {
        }

        public void DrawHorizontalLine(int x, int y, int length, int thickness, Color4 color)
        {
        }

        public void DrawPixel(int x, int y, Color4 color)
        {
        }

        public void DrawText(GuiFont font, string text, int x, int y, Color4 color) =>
            LastDrawText = (font, text, x, y, color);

        public void DrawVerticalLine(int x, int y, int length, int thickness, Color4 color)
        {
        }

        public void DrawRectangle(int x, int y, int width, int height, int thickness, Color4 color)
        {
        }

        public void EnsureInitialized(int width, int height)
        {
        }

        public void EndFrame()
        {
        }

        public void FillRectangle(int x, int y, int width, int height, Color4 color)
        {
        }

        public void Render()
        {
        }

        public void Resize(int width, int height)
        {
        }
    }

    private sealed class FakeTimeService : ITimeService
    {
        public float UpdateDeltaTime { get; set; }
        public float RenderDeltaTime { get; set; }
    }
}
