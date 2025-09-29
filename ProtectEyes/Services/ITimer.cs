namespace ProtectEyes.Services;

public interface ITimer
{
    // Start a timer; if repeating = false runs once. Returns an id or handle conceptually via out parameter not needed now.
    void Start(TimeSpan interval, Action callback, bool repeating = false);
    void Stop();
}

// Extended cancellable handle based timer for future use (optional for UI timer adapter)
public interface ITimerHandle
{
    void Stop();
    bool IsRunning { get; }
}

public interface IMultiTimer
{
    ITimerHandle Schedule(TimeSpan interval, Action callback, bool repeating = false);
    void StopAll();
}
