using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProtectEyes;

public class NotifyViewModel : BaseViewModel
{
    // timers
    readonly Services.ITimer _closeTimer;
    readonly Services.ITimer _tickTimer;
    readonly Action? _onClosedOverride;

    // context
    readonly ProtectEyesViewModel _protectEyesViewModel;
    readonly NotifyWindow? _notifyWindow;

    // state
    int _timeTick;
    string _restDesc = string.Empty;

    public NotifyViewModel(
        NotifyWindow? notifyWindow,
        ProtectEyesViewModel protectEyesViewModel,
        Services.ITimer? closeTimer = null,
        Services.ITimer? tickTimer = null,
        Action? onClosedOverride = null)
    {
        _protectEyesViewModel = protectEyesViewModel;
        _notifyWindow = notifyWindow;
        RestPic = BitmapToBitmapImage(GetBG());
        SetRestDesc();
        CloseAllNotifyWindowsCommand = new CommandHandler(CloseAllNotifyWindows, () => true);
        _closeTimer = closeTimer ?? new Services.DispatcherTimerAdapter();
        _tickTimer = tickTimer ?? new Services.DispatcherTimerAdapter();
        _onClosedOverride = onClosedOverride;
    }

    int DisplayNotifySeconds => _protectEyesViewModel.DisplayNotifySeconds;

    public BitmapImage RestPic { get; set; } = null!; // initialized in ctor

    public string RestDesc
    {
        get => _restDesc;
        set
        {
            _restDesc = value;
            NotifyPropertyChange();
        }
    }

    public ICommand CloseAllNotifyWindowsCommand { get; set; } = null!;

    public void StartCountTime()
    {
        // 1 second ticking update
        _tickTimer.Start(TimeSpan.FromSeconds(1), () =>
        {
            _timeTick++;
            if (_timeTick >= DisplayNotifySeconds)
            {
                _tickTimer.Stop();
            }
            SetRestDesc();
        }, repeating: true);

        // schedule close
        _closeTimer.Start(TimeSpan.FromSeconds(DisplayNotifySeconds), () => CloseForm(null, EventArgs.Empty));
    }

    public void SetRestDesc() => RestDesc = $"Please Take A Rest, Times Remain: {DisplayNotifySeconds - _timeTick}s";

    internal void CloseWithOutNotify()
    {
        if (_onClosedOverride != null)
            _onClosedOverride();
        else
            _notifyWindow?.Close();
    }

    internal void CloseWithNotify()
    {
        _onClosedOverride?.Invoke();
        _notifyWindow?.Close();
        if (_notifyWindow != null)
            _protectEyesViewModel.NotifyClosed(_notifyWindow);
    }

    void CloseForm(object? sender, EventArgs e) => CloseWithNotify();

    void CloseAllNotifyWindows()
    {
        foreach (var item in _protectEyesViewModel.NotifyWindows.ToArray())
            item.NotifyViewModel.CloseWithNotify();
    }

    static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        var result = new BitmapImage();
        result.BeginInit();
        result.CacheOption = BitmapCacheOption.OnLoad;
        result.StreamSource = stream;
        result.EndInit();
        result.Freeze();
        return result;
    }

    static readonly Bitmap[] _bitmapImages = { Resource.BG1, Resource.BG2, Resource.BG3, Resource.BG4 };
    static Bitmap GetBG() => _bitmapImages[new Random().Next(0, _bitmapImages.Length)];
}
