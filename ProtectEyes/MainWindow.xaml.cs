using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows;

namespace ProtectEyes
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [SupportedOSPlatform("windows6.1")]
    public partial class MainWindow : Window
    {
        public ToolStripMenuItem[]? AreaMenuItems { get; private set; }
        public NotifyIcon NotifyIcon { get; private set; } = new();

        public ProtectEyesViewModel ProtectEyesModel => (ProtectEyesViewModel)DataContext;

        public MainWindow(ProtectEyesViewModel protectEyesViewModel)
        {
            InitializeComponent();
            InitWindow();
            Hide();
            DataContext = protectEyesViewModel;
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
                new ToolStripMenuItem("Close", null, (sender, e)=>{
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

        void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ExitApp()
        {
            NotifyIcon.Dispose();
            Environment.Exit(0);
        }

        void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
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
                        UseShellExecute = true, // 必须为 true 才能用默认浏览器打开 URL (.NET Core 默认是 false)
                        Verb = "open"
                    };
                    Process.Start(psi);
                }
                else
                {
                    System.Windows.MessageBox.Show("链接格式不正确: " + e.Uri, "打开链接", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"无法打开链接: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                e.Handled = true;
            }
        }
    }
}
