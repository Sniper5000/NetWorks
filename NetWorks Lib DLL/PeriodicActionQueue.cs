using System.Collections.Concurrent;

/// <summary>
/// Class used to run actions in a separate thread at regular intervals
/// </summary>
public class PeriodicActionQueue
{
    private readonly ConcurrentQueue<Action> pendingActions = new();

    public PeriodicActionQueue(int intervalMs)
    {
        Timer _ = new(_ => Tick(), null, 0, intervalMs);
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