using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Tests;

public sealed class GuiEventQueueTests
{
    [Fact]
    public void EnqueueAndDequeueWorks()
    {
        var queue = new GuiEventQueue(capacity: 4);
        queue.Enqueue(GuiEvent.MouseMove(new Vector2(10, 20)));

        Assert.True(queue.TryDequeue(out GuiEvent evt));
        Assert.Equal(GuiEventType.MouseMove, evt.Type);
        Assert.Equal(new Vector2(10, 20), evt.Position);
    }

    [Fact]
    public void CapacityOverflowDropsOldest()
    {
        var queue = new GuiEventQueue(capacity: 2);

        queue.Enqueue(GuiEvent.KeyDown(Keys.A));
        queue.Enqueue(GuiEvent.KeyDown(Keys.B));
        queue.Enqueue(GuiEvent.KeyDown(Keys.C));

        Assert.True(queue.TryDequeue(out GuiEvent evt));
        Assert.Equal(Keys.B, evt.Key);
        Assert.True(queue.TryDequeue(out evt));
        Assert.Equal(Keys.C, evt.Key);
        Assert.False(queue.TryDequeue(out _));
    }

    [Fact]
    public void DrainToListClearsQueue()
    {
        var queue = new GuiEventQueue();
        queue.Enqueue(GuiEvent.MouseDown(new Vector2(1, 2), MouseButton.Left));
        queue.Enqueue(GuiEvent.MouseUp(new Vector2(3, 4), MouseButton.Left));

        IReadOnlyList<GuiEvent> events = queue.DrainToList();

        Assert.Equal(2, events.Count);
        Assert.Equal(GuiEventType.MouseDown, events[0].Type);
        Assert.Equal(GuiEventType.MouseUp, events[1].Type);
        Assert.Equal(0, queue.Count);
    }
}

