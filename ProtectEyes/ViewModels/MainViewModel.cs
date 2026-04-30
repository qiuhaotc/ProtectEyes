using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProtectEyes.Services;
using ProtectEyes.Services.Interfaces;

namespace ProtectEyes.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAppConfig _config;
    private readonly INotifyService _notifyService;
    private readonly IAutoStartService _autoStartService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<MainViewModel> _logger;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DurationDesc))]
    private int _duration;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayNotifySecondsDesc))]
    private int _displayNotifySeconds;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusDesc))]
    private bool _shouldContinue;

    [ObservableProperty]
    private bool _autoStart;

    public IReadOnlyList<LanguageOption> LanguageOptions => _localizationService.LanguageOptions;

    public string LanguageMode
    {
        get => _localizationService.LanguageMode;
        set
        {
            if (_localizationService.LanguageMode == value)
            {
                return;
            }

            _localizationService.SetLanguageMode(value);
            OnPropertyChanged();
        }
    }

    public string ConfigTabHeader => T("ConfigTab");
    public string AboutTabHeader => T("AboutTab");
    public string DurationDesc => T("DurationDesc", Duration);
    public string DisplayNotifySecondsDesc => T("DisplayNotifySecondsDesc", DisplayNotifySeconds);
    public string MinutesHint => T("MinutesHint");
    public string SecondsHint => T("SecondsHint");
    public string AllowExecutionText => T("AllowExecution");
    public string AutoStartText => T("AutoStart");
    public string LanguageText => T("Language");
    public string SaveConfigText => T("SaveConfig");
    public string SimulateNotifyText => T("SimulateNotify");
    public string ExitText => T("Exit");
    public string StatusDesc => T("StatusRunning", ShouldContinue ? T("Running") : T("Paused"));
    public string GitHubLabel => T("GitHubLabel");
    public string LastModifiedText => T("LastModified");

    public MainViewModel(
        IAppConfig config,
        INotifyService notifyService,
        IAutoStartService autoStartService,
        ILocalizationService localizationService,
        ILogger<MainViewModel> logger)
    {
        _config = config;
        _notifyService = notifyService;
        _autoStartService = autoStartService;
        _localizationService = localizationService;
        _logger = logger;
        _localizationService.LanguageChanged += (_, _) => NotifyLocalizedPropertiesChanged();

        _duration = _config.RunBetween;
        _displayNotifySeconds = _config.DisplayNotifySeconds;
        _autoStart = _config.AutoStart;
        _shouldContinue = true;

        _logger.LogInformation(
            "MainViewModel initialized with Duration={Duration}, DisplayNotifySeconds={DisplayNotifySeconds}",
            _duration, _displayNotifySeconds);

        ApplyCycleState();
    }

    partial void OnShouldContinueChanged(bool value)
    {
        _logger.LogInformation("ShouldContinue changed to {Value}", value);
        ApplyCycleState();
    }

    [RelayCommand]
    private void SaveConfig()
    {
        _logger.LogInformation("Saving configuration...");

        _config.RunBetween = Duration;
        _config.DisplayNotifySeconds = DisplayNotifySeconds;
        _config.AutoStart = AutoStart;
        _config.Save();

        var appPath = Environment.ProcessPath ?? System.Reflection.Assembly.GetExecutingAssembly().Location;
        const string appName = "Protect_Eyes";
        if (AutoStart)
        {
            _autoStartService.Enable(appName, appPath);
        }
        else
        {
            _autoStartService.Disable(appName);
        }

        ApplyCycleState();
        _logger.LogInformation("Configuration saved successfully");
    }

    [RelayCommand]
    private void SimulateNotify()
    {
        _notifyService.TriggerNow(Duration, DisplayNotifySeconds);
    }

    private void ApplyCycleState()
    {
        if (ShouldContinue)
        {
            _notifyService.Start(Duration, DisplayNotifySeconds);
        }
        else
        {
            _notifyService.Stop();
        }
    }

    private string T(string key) => _localizationService.Translate(key);

    private string T(string key, params object[] args) => _localizationService.Translate(key, args);

    private void NotifyLocalizedPropertiesChanged()
    {
        OnPropertyChanged(nameof(LanguageMode));
        OnPropertyChanged(nameof(ConfigTabHeader));
        OnPropertyChanged(nameof(AboutTabHeader));
        OnPropertyChanged(nameof(DurationDesc));
        OnPropertyChanged(nameof(DisplayNotifySecondsDesc));
        OnPropertyChanged(nameof(MinutesHint));
        OnPropertyChanged(nameof(SecondsHint));
        OnPropertyChanged(nameof(AllowExecutionText));
        OnPropertyChanged(nameof(AutoStartText));
        OnPropertyChanged(nameof(LanguageText));
        OnPropertyChanged(nameof(SaveConfigText));
        OnPropertyChanged(nameof(SimulateNotifyText));
        OnPropertyChanged(nameof(ExitText));
        OnPropertyChanged(nameof(StatusDesc));
        OnPropertyChanged(nameof(GitHubLabel));
        OnPropertyChanged(nameof(LastModifiedText));
    }
}
