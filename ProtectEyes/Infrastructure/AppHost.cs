using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtectEyes.Services;
using ProtectEyes.Services.Implementations;
using ProtectEyes.Services.Interfaces;
using ProtectEyes.ViewModels;

namespace ProtectEyes.Infrastructure;

[SupportedOSPlatform("windows6.1")]
public static class AppHost
{
    public static IServiceProvider Services { get; private set; } = null!;

    public static void ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Singleton services
        services.AddSingleton<IAppConfig, JsonAppConfig>();
        services.AddSingleton<IAutoStartService, AutoStartService>();
        services.AddSingleton<INotifyService, NotifyService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();

        // Each timer must be a fresh instance per consumer
        services.AddTransient<ITimerService, TimerService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<NotifyViewModel>();

        // Windows
        services.AddSingleton<MainWindow>();

        Services = services.BuildServiceProvider();
    }

    public static T GetService<T>() where T : class
    {
        return Services.GetRequiredService<T>();
    }
}
