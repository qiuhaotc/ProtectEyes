using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtectEyes.Services.Interfaces;
using ProtectEyes.ViewModels;

namespace ProtectEyes.Services.Implementations;

public class NotifyService : INotifyService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotifyService> _logger;
    private readonly object _syncLock = new();
    private readonly List<NotifyWindow> _activeWindows = new();

    private ITimerService? _cycleTimer;
    private int _durationMinutes;
    private int _displayNotifySeconds;

    public NotifyService(IServiceProvider services, ILogger<NotifyService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public bool IsRunning => _cycleTimer?.IsRunning ?? false;

    public void Start(int durationMinutes, int displayNotifySeconds)
    {
        Stop();

        _durationMinutes = Math.Max(1, durationMinutes);
        _displayNotifySeconds = Math.Max(1, displayNotifySeconds);

        _cycleTimer = _services.GetRequiredService<ITimerService>();
        _cycleTimer.Start(TimeSpan.FromMinutes(_durationMinutes), ShowNotifyWindows, repeating: false);

        _logger.LogInformation("Notify cycle started: every {Duration}m for {Seconds}s", _durationMinutes, _displayNotifySeconds);
    }

    public void Stop()
    {
        _cycleTimer?.Stop();
        _cycleTimer = null;
        CloseAllSilently();
    }

    public void TriggerNow(int durationMinutes, int displayNotifySeconds)
    {
        _cycleTimer?.Stop();
        _cycleTimer = null;

        _durationMinutes = Math.Max(1, durationMinutes);
        _displayNotifySeconds = Math.Max(1, displayNotifySeconds);

        _logger.LogInformation("Notify popup triggered manually for {Seconds}s", _displayNotifySeconds);
        ShowNotifyWindows();
    }

    private void ShowNotifyWindows()
    {
        lock (_syncLock)
        {
            CloseAllSilently();

            foreach (var screen in Screen.AllScreens)
            {
                var vm = _services.GetRequiredService<NotifyViewModel>();
                vm.Configure(_displayNotifySeconds);
                var window = new NotifyWindow(screen.WorkingArea, vm);
                vm.Closed += OnNotifyClosed;
                _activeWindows.Add(window);
                window.Show();
            }
        }
    }

    private void OnNotifyClosed()
    {
        // Marshal to UI thread; one screen finishing triggers cleanup + reschedule.
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
        {
            dispatcher.BeginInvoke(new Action(HandleNotifyClosedInternal));
        }
        else
        {
            HandleNotifyClosedInternal();
        }
    }

    private void HandleNotifyClosedInternal()
    {
        lock (_syncLock)
        {
            // Close any remaining sibling windows so they all dismiss together.
            foreach (var window in _activeWindows.ToArray())
            {
                if (window.NotifyViewModel is { } vm)
                {
                    vm.Closed -= OnNotifyClosed;
                }
            }
            _activeWindows.Clear();
        }

        // Reschedule next cycle.
        if (_durationMinutes > 0)
        {
            _cycleTimer = _services.GetRequiredService<ITimerService>();
            _cycleTimer.Start(TimeSpan.FromMinutes(_durationMinutes), ShowNotifyWindows, repeating: false);
        }
    }

    private void CloseAllSilently()
    {
        lock (_syncLock)
        {
            foreach (var window in _activeWindows.ToArray())
            {
                if (window.NotifyViewModel is { } vm)
                {
                    vm.Closed -= OnNotifyClosed;
                    vm.CloseWithOutNotify();
                }
            }
            _activeWindows.Clear();
        }
    }
}
