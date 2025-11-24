using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui.Components;

public abstract class GuiElement
{
    private readonly List<GuiElement> _children = new();

    public Vector2i Position { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Visible { get; set; } = true;

    protected IReadOnlyList<GuiElement> Children => _children;
    protected GuiElement? Parent { get; private set; }

    public void AddChild(GuiElement child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (child.Parent == this)
        {
            return;
        }

        child.Parent?._children.Remove(child);
        child.Parent = this;
        _children.Add(child);
    }

    public void RemoveChild(GuiElement child)
    {
        if (child == null)
        {
            return;
        }

        if (_children.Remove(child))
        {
            child.Parent = null;
        }
    }

    public virtual void Render(IGuiService gui)
    {
        if (!Visible)
        {
            return;
        }

        foreach (GuiElement child in _children)
        {
            child.Render(gui);
        }
    }

    public virtual void HandleEvent(GuiEvent evt)
    {
        if (!Visible)
        {
            return;
        }

        foreach (GuiElement child in _children)
        {
            child.HandleEvent(evt);
        }
    }

    protected (int X, int Y) GetGlobalPosition()
    {
        int x = Position.X;
        int y = Position.Y;
        GuiElement? current = Parent;

        while (current != null)
        {
            x += current.Position.X;
            y += current.Position.Y;
            current = current.Parent;
        }

        return (x, y);
    }

    protected bool HitTest(Vector2 position)
    {
        (int globalX, int globalY) = GetGlobalPosition();
        return position.X >= globalX &&
               position.X <= globalX + Width &&
               position.Y >= globalY &&
               position.Y <= globalY + Height;
    }
}

