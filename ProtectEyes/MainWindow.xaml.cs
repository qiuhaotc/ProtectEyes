using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows;
using ProtectEyes.Services.Interfaces;
using ProtectEyes.ViewModels;

namespace ProtectEyes;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
[SupportedOSPlatform("windows6.1")]
public partial class MainWindow : Window
{
    private readonly INotifyService _notifyService;
    private readonly ILocalizationService _localizationService;
    private bool _isExiting;

    public ToolStripMenuItem[]? AreaMenuItems { get; private set; }
    public NotifyIcon NotifyIcon { get; private set; } = new();

    public MainViewModel MainViewModel => (MainViewModel)DataContext;

    public MainWindow(
        MainViewModel mainViewModel,
        INotifyService notifyService,
        ILocalizationService localizationService)
    {
        _notifyService = notifyService;
        _localizationService = localizationService;
        _localizationService.LanguageChanged += (_, _) => UpdateTrayText();

        InitializeComponent();
        InitWindow();
        Hide();
        DataContext = mainViewModel;
    }

    void InitWindow()
    {
        // already initialized via property initializer
        NotifyIcon.Text = "Protect Eyes";
        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            NotifyIcon.Icon = Resource.SunGlasses;
        }

        NotifyIcon.Visible = true;

        NotifyIcon.DoubleClick += (_, _) =>
        {
            if (IsVisible)
            {
                Hide();
            }
            else
            {
                Show();
                Activate();
            }
        };

        AreaMenuItems = new[]
        {
            new ToolStripMenuItem(T("TrayClose"), null, (sender, e) =>
            {
                ExitApp();
            })
        };

        if (AreaMenuItems is { Length: > 0 })
        {
            var cms = new ContextMenuStrip();
            cms.Items.AddRange(AreaMenuItems);
            NotifyIcon.ContextMenuStrip = cms;
        }

        Closing += MainWindow_Closing;
    }

    void ExitApp()
    {
        if (_isExiting)
        {
            return;
        }

        var dispatcher = Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
        {
            dispatcher.BeginInvoke(new Action(ExitApp));
            return;
        }

        _isExiting = true;
        Closing -= MainWindow_Closing;
        _notifyService?.Stop();
        if (NotifyIcon is not null)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
        }
        System.Windows.Application.Current?.Shutdown();
    }

    void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        if (_isExiting)
        {
            return;
        }

        Hide();
        e.Cancel = true;
    }

    void Button_Click(object sender, RoutedEventArgs e)
    {
        ExitApp();
    }

    void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        try
        {
            if (e.Uri is not null && (e.Uri.Scheme == Uri.UriSchemeHttp || e.Uri.Scheme == Uri.UriSchemeHttps))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(psi);
            }
            else
            {
                System.Windows.MessageBox.Show(
                    T("OpenLinkInvalidMessage", e.Uri?.ToString() ?? string.Empty),
                    T("OpenLinkInvalidTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                T("OpenLinkErrorMessage", ex.Message),
                T("OpenLinkErrorTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            e.Handled = true;
        }
    }

    private void UpdateTrayText()
    {
        if (AreaMenuItems is { Length: > 0 })
        {
            AreaMenuItems[0].Text = T("TrayClose");
        }
    }

    private string T(string key) => _localizationService.Translate(key);

    private string T(string key, params object[] args) => _localizationService.Translate(key, args);
}
