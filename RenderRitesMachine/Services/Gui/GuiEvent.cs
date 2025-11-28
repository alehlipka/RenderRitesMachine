using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
///     Rich input event used by GUI systems.
/// </summary>
public readonly record struct GuiEvent
{
    private GuiEvent(
        GuiEventType type,
        Vector2 position,
        Keys? key,
        MouseButton? button,
        Vector2 scrollDelta,
        char? character)
    {
        Type = type;
        Position = position;
        Key = key;
        Button = button;
        ScrollDelta = scrollDelta;
        Character = character;
    }

    public GuiEventType Type { get; }
    public Vector2 Position { get; }
    public Keys? Key { get; }
    public MouseButton? Button { get; }
    public Vector2 ScrollDelta { get; }
    public char? Character { get; }

    public static GuiEvent MouseMove(Vector2 position) =>
        new(GuiEventType.MouseMove, position, null, null, Vector2.Zero, null);

    public static GuiEvent MouseDown(Vector2 position, MouseButton button) =>
        new(GuiEventType.MouseDown, position, null, button, Vector2.Zero, null);

    public static GuiEvent MouseUp(Vector2 position, MouseButton button) =>
        new(GuiEventType.MouseUp, position, null, button, Vector2.Zero, null);

    public static GuiEvent MouseScroll(Vector2 position, Vector2 scrollDelta) =>
        new(GuiEventType.MouseScroll, position, null, null, scrollDelta, null);

    public static GuiEvent KeyDown(Keys key) =>
        new(GuiEventType.KeyDown, Vector2.Zero, key, null, Vector2.Zero, null);

    public static GuiEvent KeyUp(Keys key) =>
        new(GuiEventType.KeyUp, Vector2.Zero, key, null, Vector2.Zero, null);

    public static GuiEvent TextInput(char character) =>
        new(GuiEventType.TextInput, Vector2.Zero, null, null, Vector2.Zero, character);
}
