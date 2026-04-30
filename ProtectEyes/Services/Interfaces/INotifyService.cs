namespace ProtectEyes.Services.Interfaces;

public interface INotifyService
{
    /// <summary>
    /// Starts the notification cycle. The first notify popup will fire after <paramref name="durationMinutes"/> minutes.
    /// Calling Start while running will reset the cycle.
    /// </summary>
    void Start(int durationMinutes, int displayNotifySeconds);

    /// <summary>
    /// Stops the cycle and closes any active notification windows without notifying completion.
    /// </summary>
    void Stop();

    /// <summary>
    /// Shows notification windows immediately and restarts the cycle from now.
    /// </summary>
    void TriggerNow(int durationMinutes, int displayNotifySeconds);

    bool IsRunning { get; }
}
