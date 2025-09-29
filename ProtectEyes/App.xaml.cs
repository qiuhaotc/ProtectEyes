using System.Runtime.Versioning;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtectEyes.Services;

namespace ProtectEyes
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IHost? HostInstance { get; private set; }
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
            HostInstance = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IAppConfig, JsonAppConfig>();
                    services.AddTransient<ProtectEyes.Services.ITimer, DispatcherTimerAdapter>();
                    services.AddSingleton<IAutoStartManager, AutoStartManager>();
                    services.AddSingleton<ProtectEyesViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            _main = HostInstance.Services.GetRequiredService<MainWindow>();
            // _main.Hide(); // 仅显示托盘
        }

        MainWindow? _main;

        protected override async void OnExit(ExitEventArgs e)
        {
            SafeDisposeNotifyIcon();
            _singleInstanceMutex?.ReleaseMutex();
            if (HostInstance is IAsyncDisposable asyncDisp)
            {
                await asyncDisp.DisposeAsync();
            }
            else
            {
                HostInstance?.Dispose();
            }
            base.OnExit(e);
        }

        static void SafeDisposeNotifyIcon()
        {
            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    var main = HostInstance?.Services.GetService<MainWindow>();
                    main?.NotifyIcon?.Dispose();
                }
            }
            catch { /* 忽略释放异常 */ }
        }
    }
}
