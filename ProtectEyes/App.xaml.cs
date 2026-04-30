using System.Runtime.Versioning;
using System.Windows;
using ProtectEyes.Infrastructure;

namespace ProtectEyes;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : System.Windows.Application
{
    static Mutex? _singleInstanceMutex; // 防止多实例产生两个托盘图标

    [SupportedOSPlatform("windows6.1")]
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 单实例检测
        bool createdNew = false;
        try
        {
            _singleInstanceMutex = new Mutex(initiallyOwned: true, name: "Global/ProtectEyes.SingleInstance", createdNew: out createdNew);
        }
        catch
        {
            createdNew = false;
        }

        if (!createdNew)
        {
            // 已有实例，直接退出避免第二个托盘图标
            Shutdown();
            return;
        }

        // 进程结束兜底释放托盘图标，防止残留影子图标
        AppDomain.CurrentDomain.ProcessExit += (_, _) => SafeDisposeNotifyIcon();
        AppDomain.CurrentDomain.UnhandledException += (_, _) => SafeDisposeNotifyIcon();

        // 配置依赖注入
        AppHost.ConfigureServices();

        // 获取主窗口并显示
        var mainWindow = AppHost.GetService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SafeDisposeNotifyIcon();
        _singleInstanceMutex?.ReleaseMutex();
        base.OnExit(e);
    }

    static void SafeDisposeNotifyIcon()
    {
        try
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                var mainWindow = Current?.MainWindow as MainWindow;
                mainWindow?.NotifyIcon?.Dispose();
            }
        }
        catch { /* 忽略释放异常 */ }
    }
}
