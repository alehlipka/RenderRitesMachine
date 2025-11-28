namespace RenderRitesMachine.Services.Gui;

/// <summary>
///     Simple bounded queue for GUI input events.
/// </summary>
public sealed class GuiEventQueue
{
    private readonly Queue<GuiEvent> _events = new();
    private readonly object _syncRoot = new();

    public GuiEventQueue(int capacity = 1024)
    {
        Capacity = Math.Clamp(capacity, 1, 16384);
    }

    public int Capacity { get; }

    public int Count
    {
        get
        {
            lock (_syncRoot)
            {
                return _events.Count;
            }
        }
    }

    public void Enqueue(in GuiEvent guiEvent)
    {
        lock (_syncRoot)
        {
            if (_events.Count >= Capacity)
            {
                _ = _events.Dequeue();
            }

            _events.Enqueue(guiEvent);
        }
    }

    public bool TryDequeue(out GuiEvent guiEvent)
    {
        lock (_syncRoot)
        {
            if (_events.Count == 0)
            {
                guiEvent = default;
                return false;
            }

            guiEvent = _events.Dequeue();
            return true;
        }
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            _events.Clear();
        }
    }

    public IReadOnlyList<GuiEvent> DrainToList(List<GuiEvent>? buffer = null)
    {
        buffer ??= new List<GuiEvent>(_events.Count);
        buffer.Clear();

        lock (_syncRoot)
        {
            while (_events.Count > 0)
            {
                buffer.Add(_events.Dequeue());
            }
        }

        return buffer;
    }
}
