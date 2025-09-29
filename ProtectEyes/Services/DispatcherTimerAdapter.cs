using System.Windows.Threading;

namespace ProtectEyes.Services;

public class DispatcherTimerAdapter : ITimer
{
    DispatcherTimer? _timer;
    public void Start(TimeSpan interval, Action callback, bool repeating = false)
    {
        _timer = new DispatcherTimer { Interval = interval };
        EventHandler handler = null!;
        handler = (_, _) =>
        {
            callback();
            if (!repeating)
            {
                _timer!.Tick -= handler;
                _timer!.Stop();
            }
        };
        _timer.Tick += handler;
        _timer.Start();
    }

    public void Stop()
    {
        _timer?.Stop();
    }
}
