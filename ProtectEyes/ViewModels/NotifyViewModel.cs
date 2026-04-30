using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProtectEyes.Services.Interfaces;

namespace ProtectEyes.ViewModels;

public partial class NotifyViewModel : ObservableObject
{
    private readonly ITimerService _closeTimer;
    private readonly ITimerService _tickTimer;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<NotifyViewModel> _logger;

    private int _displayNotifySeconds;
    private int _timeTick;
    private Action? _onClosedOverride;

    [ObservableProperty]
    private BitmapImage _restPic = null!;

    [ObservableProperty]
    private string _restDesc = string.Empty;

    /// <summary>
    /// Raised when the view should close itself.
    /// </summary>
    public event Action? RequestClose;

    /// <summary>
    /// Raised after the notification cycle finishes naturally so the host can reschedule.
    /// </summary>
    public event Action? Closed;

    public NotifyViewModel(
        ITimerService closeTimer,
        ITimerService tickTimer,
        ILocalizationService localizationService,
        ILogger<NotifyViewModel> logger)
    {
        _closeTimer = closeTimer;
        _tickTimer = tickTimer;
        _localizationService = localizationService;
        _logger = logger;
        _localizationService.LanguageChanged += OnLanguageChanged;

        RestPic = BitmapToBitmapImage(GetBG());
    }

    /// <summary>
    /// Sets countdown configuration. Must be invoked before showing the view.
    /// </summary>
    public void Configure(int displayNotifySeconds, Action? onClosedOverride = null)
    {
        _displayNotifySeconds = displayNotifySeconds;
        _onClosedOverride = onClosedOverride;
        _timeTick = 0;
        UpdateRestDesc();
    }

    public void StartCountTime()
    {
        _logger.LogInformation("Notification countdown started for {Seconds}s", _displayNotifySeconds);

        _tickTimer.Start(TimeSpan.FromSeconds(1), () =>
        {
            _timeTick++;
            if (_timeTick >= _displayNotifySeconds)
            {
                _tickTimer.Stop();
            }
            UpdateRestDesc();
        }, repeating: true);

        _closeTimer.Start(TimeSpan.FromSeconds(_displayNotifySeconds), CloseWithNotify);
    }

    [RelayCommand]
    public void CloseWithOutNotify()
    {
        _tickTimer.Stop();
        _closeTimer.Stop();
        _localizationService.LanguageChanged -= OnLanguageChanged;
        if (_onClosedOverride != null)
        {
            _onClosedOverride();
        }
        else
        {
            RequestClose?.Invoke();
        }
    }

    public void CloseWithNotify()
    {
        _tickTimer.Stop();
        _closeTimer.Stop();
        _localizationService.LanguageChanged -= OnLanguageChanged;
        _onClosedOverride?.Invoke();
        RequestClose?.Invoke();
        Closed?.Invoke();
    }

    private void UpdateRestDesc()
        => RestDesc = _localizationService.Translate("NotifyCountdown", Math.Max(0, _displayNotifySeconds - _timeTick));

    private void OnLanguageChanged(object? sender, EventArgs e) => UpdateRestDesc();

    private static Bitmap GetBG()
    {
        var bitmaps = new[] { Resource.BG1, Resource.BG2, Resource.BG3, Resource.BG4 };
        return bitmaps[new Random().Next(bitmaps.Length)];
    }

    private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
    {
        using var memory = new MemoryStream();
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }
}
