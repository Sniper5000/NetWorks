using System;
using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// Class used to run actions in a separate thread at regular intervals
/// </summary>
public class PeriodicActionQueue
{
    private readonly ConcurrentQueue<Action> pendingActions = new ConcurrentQueue<Action>();

    public PeriodicActionQueue(int intervalMs)
    {
        Timer _ = new Timer(_ => Tick(), null, 0, intervalMs);
    }

    public void Enqueue(Action action)
    {
        pendingActions.Enqueue(action);
    }
    
    private void Tick()
    {
        while(pendingActions.TryDequeue(out var action))
            action();
    }
}