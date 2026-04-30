using System.Windows.Threading;
using ProtectEyes.Services.Interfaces;

namespace ProtectEyes.Services.Implementations;

public class TimerService : ITimerService
{
    private DispatcherTimer? _timer;
    private Action? _callback;
    private bool _repeating;

    public bool IsRunning => _timer?.IsEnabled ?? false;

    public void Start(TimeSpan interval, Action callback, bool repeating = false)
    {
        Stop();

        _callback = callback;
        _repeating = repeating;

        _timer = new DispatcherTimer
        {
            Interval = interval
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    public void Stop()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (!_repeating)
        {
            Stop();
        }
        _callback?.Invoke();
    }
}
