namespace ProtectEyes.Services.Interfaces;

public interface ITimerService
{
    void Start(TimeSpan interval, Action callback, bool repeating = false);
    void Stop();
    bool IsRunning { get; }
}
