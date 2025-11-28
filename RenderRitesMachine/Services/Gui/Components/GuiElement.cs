using OpenTK.Mathematics;
using RenderRitesMachine.Services.Timing;

namespace RenderRitesMachine.Services.Gui.Components;

public abstract class GuiElement
{
    private readonly List<GuiElement> _children = new();
    private readonly Color4 _debugColorBorder = Color4.YellowGreen;
    private readonly Color4 _debugColorMargin = Color4.MediumVioletRed;
    private readonly Color4 _debugColorPadding = Color4.Orange;
    private bool _hasResolvedPosition;
    private GuiHorizontalAnchor _horizontalAnchor = GuiHorizontalAnchor.Left;
    private GuiMargins _margin = GuiMargins.Zero;
    private GuiPadding _padding = GuiPadding.Zero;
    private Vector2i _position;
    private float _relativeHeight = 1f;
    private float _relativeWidth = 1f;
    private Vector2i _resolvedPosition;
    private GuiVerticalAnchor _verticalAnchor = GuiVerticalAnchor.Top;

    public Vector2i Position
    {
        get => _position;
        set
        {
            _position = value;
            _hasResolvedPosition = false;
        }
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsVisible { get; set; } = true;

    public GuiMargins Margin
    {
        get => _margin;
        set
        {
            _margin = value;
            _hasResolvedPosition = false;
        }
    }

    public GuiPadding Padding
    {
        get => _padding;
        set
        {
            _padding = value;
            _hasResolvedPosition = false;
        }
    }

    public GuiHorizontalAnchor HorizontalAnchor
    {
        get => _horizontalAnchor;
        set
        {
            _horizontalAnchor = value;
            _hasResolvedPosition = false;
        }
    }

    public GuiVerticalAnchor VerticalAnchor
    {
        get => _verticalAnchor;
        set
        {
            _verticalAnchor = value;
            _hasResolvedPosition = false;
        }
    }

    public bool UseRelativeWidth { get; set; }
    public bool UseRelativeHeight { get; set; }

    public float RelativeWidth
    {
        get => _relativeWidth;
        set => _relativeWidth = Math.Clamp(value, 0f, 1f);
    }

    public float RelativeHeight
    {
        get => _relativeHeight;
        set => _relativeHeight = Math.Clamp(value, 0f, 1f);
    }

    public bool Debug { get; set; }

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

    public virtual void Render(IGuiService gui, ITimeService time)
    {
        if (!IsVisible)
        {
            return;
        }

        if (Parent == null)
        {
            Vector2i viewportSize = new(gui.Width, gui.Height);
            UpdateAdaptiveLayout(viewportSize);
        }

        foreach (GuiElement child in _children)
        {
            child.Render(gui, time);
        }

        if (Debug)
        {
            (int x, int y) = GetGlobalPosition();
            gui.DrawRectangle(x - Margin.Left, y - Margin.Top, Width + Margin.Horizontal, Height + Margin.Vertical, 1,
                _debugColorMargin);
            gui.DrawRectangle(x, y, Width, Height, 1, _debugColorBorder);
            gui.DrawRectangle(x + Padding.Left, y + Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical,
                1, _debugColorPadding);
        }
    }

    public virtual void HandleEvent(GuiEvent evt)
    {
        if (!IsVisible)
        {
            return;
        }

        foreach (GuiElement child in _children)
        {
            child.HandleEvent(evt);
        }
    }

    public void UpdateAdaptiveLayout(Vector2i viewportSize)
    {
        Vector2i parentSize = Parent == null
            ? viewportSize
            : new Vector2i(Math.Max(0, Parent.Width - Parent.Padding.Horizontal),
                Math.Max(0, Parent.Height - Parent.Padding.Vertical));

        ApplyAdaptiveLayout(parentSize);

        Vector2i childViewport = new(
            Math.Max(0, Width - Padding.Horizontal),
            Math.Max(0, Height - Padding.Vertical));
        foreach (GuiElement child in _children)
        {
            child.UpdateAdaptiveLayout(childViewport);
        }
    }

    protected virtual void ApplyAdaptiveLayout(Vector2i parentSize)
    {
        ApplyAdaptiveSize(parentSize);
        ApplyAdaptivePosition(parentSize);
    }

    private void ApplyAdaptiveSize(Vector2i parentSize)
    {
        int availableWidth = Math.Max(0, parentSize.X - Margin.Horizontal);
        if (HorizontalAnchor == GuiHorizontalAnchor.Stretch)
        {
            Width = availableWidth;
        }
        else if (UseRelativeWidth)
        {
            Width = Math.Max(0, (int)MathF.Round(availableWidth * RelativeWidth));
        }

        int availableHeight = Math.Max(0, parentSize.Y - Margin.Vertical);
        if (VerticalAnchor == GuiVerticalAnchor.Stretch)
        {
            Height = availableHeight;
        }
        else if (UseRelativeHeight)
        {
            Height = Math.Max(0, (int)MathF.Round(availableHeight * RelativeHeight));
        }
    }

    private void ApplyAdaptivePosition(Vector2i parentSize)
    {
        int x = HorizontalAnchor switch
        {
            GuiHorizontalAnchor.Left => Margin.Left + _position.X,
            GuiHorizontalAnchor.Center => Margin.Left + GetCenteredOffset(parentSize.X, Margin.Horizontal, Width) +
                                          _position.X,
            GuiHorizontalAnchor.Right => parentSize.X - Margin.Right - Width - _position.X,
            GuiHorizontalAnchor.Stretch => Margin.Left + _position.X,
            _ => _position.X
        };

        int y = VerticalAnchor switch
        {
            GuiVerticalAnchor.Top => Margin.Top + _position.Y,
            GuiVerticalAnchor.Center => Margin.Top + GetCenteredOffset(parentSize.Y, Margin.Vertical, Height) +
                                        _position.Y,
            GuiVerticalAnchor.Bottom => parentSize.Y - Margin.Bottom - Height - _position.Y,
            GuiVerticalAnchor.Stretch => Margin.Top + _position.Y,
            _ => _position.Y
        };

        _resolvedPosition = new Vector2i(x, y);
        _hasResolvedPosition = true;
    }

    private static int GetCenteredOffset(int parentExtent, int totalMargin, int childExtent)
    {
        int available = Math.Max(0, parentExtent - totalMargin - childExtent);
        return available / 2;
    }

    private Vector2i GetResolvedLocalPosition() => _hasResolvedPosition ? _resolvedPosition : _position;

    protected Vector2i GetContentOffset() => new(Padding.Left, Padding.Top);

    protected (int X, int Y) GetGlobalPosition()
    {
        Vector2i position = GetResolvedLocalPosition();
        GuiElement? current = Parent;

        while (current != null)
        {
            position += current.GetResolvedLocalPosition();
            position += current.GetContentOffset();
            current = current.Parent;
        }

        return (position.X, position.Y);
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
