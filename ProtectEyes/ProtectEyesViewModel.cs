using System.Reflection;
using System.Windows.Input;
using ProtectEyes.Services;

namespace ProtectEyes
{
    public class ProtectEyesViewModel : BaseViewModel
    {
        readonly IAppConfig _config;
        readonly Services.ITimer _durationTimer;
        readonly IAutoStartManager _autoStartManager;
        static readonly object _syncLock = new object();

        // backing fields (mutable state)
        int _duration;
        int _displayNotifySeconds;
        bool _shouldContinue;
        bool _autoStart;

        public ProtectEyesViewModel(IAppConfig? config = null, Services.ITimer? timer = null, IAutoStartManager? autoStartManager = null)
        {
            _config = config ?? new Services.JsonAppConfig();
            _durationTimer = timer ?? new Services.DispatcherTimerAdapter();
            _autoStartManager = autoStartManager ?? new Services.AutoStartManager();
            Duration = _config.RunBetween;
            DisplayNotifySeconds = _config.DisplayNotifySeconds;
            _autoStart = _config.AutoStart;
            SaveConfigCommand = new CommandHandler(SaveConfig, () => true);
            ShouldContinue = true;
        }

        public string DurationDesc => $"Duration: {Duration} minutes";
        public string StatusDesc => $"Status: Is Running {ShouldContinue}";

        public int Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                NotifyPropertyChange(nameof(DurationDesc));
            }
        }

        public void ShowNotifyWindows()
        {
            if (ShouldContinue)
            {
                foreach (var notifyWindow in NotifyWindows)
                {
                    notifyWindow.Show();
                }
            }
        }

        public int DisplayNotifySeconds
        {
            get => _displayNotifySeconds;
            set
            {
                _displayNotifySeconds = value;
                NotifyPropertyChange(nameof(DisplayNotifySecondsDesc));
            }
        }

        public string DisplayNotifySecondsDesc => $"Notifycation Display Seconds: {DisplayNotifySeconds}s";
        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                _autoStart = value;

                var appPath = Assembly.GetExecutingAssembly().Location;
                const string appName = "Protect_Eyes"; // Registry value name
                bool ok = _autoStart
                    ? _autoStartManager.Enable(appName, appPath)
                    : _autoStartManager.Disable(appName);
                if (ok)
                {
                    _config.AutoStart = _autoStart;
                    _config.Save();
                }
            }
        }

        public bool ShouldContinue
        {
            get => _shouldContinue;
            set
            {
                _shouldContinue = value;
                StartDurationIfNeeded();
                NotifyPropertyChange(nameof(StatusDesc));
            }
        }
        public List<NotifyWindow> NotifyWindows { get; private set; } = new List<NotifyWindow>();

        public ICommand SaveConfigCommand { get; set; }

        void SaveConfig()
        {
            _config.RunBetween = Duration;
            _config.DisplayNotifySeconds = DisplayNotifySeconds;
            _config.AutoStart = _autoStart;
            _config.Save();
            StartDurationIfNeeded();
        }

        // Legacy SetConfigValue removed; persisted via IAppConfig implementation.

        Services.ITimer? _notifyCycleTimer;

        void StartDurationIfNeeded()
        {
            if (ShouldContinue)
            {
                _notifyCycleTimer?.Stop();

                NotifyWindows.ForEach(u => { u.NotifyViewModel.CloseWithOutNotify(); });
                NotifyWindows.Clear();
                InitNotifyForm();
                _notifyCycleTimer = _durationTimer; // reuse injected timer for cycle
                _notifyCycleTimer.Start(TimeSpan.FromMinutes(Duration), () => ShowNotifyWindow(this, EventArgs.Empty));
            }
            else
            {
                _notifyCycleTimer?.Stop();

                NotifyWindows.ForEach(u => { u.NotifyViewModel.CloseWithOutNotify(); });
            }
        }

        void InitNotifyForm()
        {
            foreach (var scr in Screen.AllScreens)
            {
                var notifyWindow = new NotifyWindow(scr.WorkingArea, this);
                notifyWindow.Left = -100000; // Out side the window
                NotifyWindows.Add(notifyWindow);
            }
        }

        public void ShowNotifyWindow(object? sender, EventArgs e)
        {
            ShowNotifyWindows();
        }


        public void NotifyClosed(NotifyWindow notifyWindow)
        {
            lock (_syncLock)
            {
                NotifyWindows.Remove(notifyWindow);

                if (NotifyWindows.Count == 0)
                {
                    StartDurationIfNeeded();
                }
            }
        }
    }
}
