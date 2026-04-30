using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ProtectEyes.Services;
using ProtectEyes.Services.Implementations;
using ProtectEyes.Services.Interfaces;
using ProtectEyes.ViewModels;

namespace ProtectEyeTest;

[TestFixture, Apartment(ApartmentState.STA)]
public class AutoStartTests
{
    class FakeAppConfig : IAppConfig
    {
        public int RunBetween { get; set; } = 20;
        public int DisplayNotifySeconds { get; set; } = 10;
        public bool AutoStart { get; set; } = false;
        public string LanguageMode { get; set; } = "en-US";
        public bool Saved { get; private set; }
        public void Save() => Saved = true;
    }

    class FakeAutoStartService : IAutoStartService
    {
        public int EnableCalls { get; private set; }
        public int DisableCalls { get; private set; }
        public bool ResultToReturn { get; set; } = true;
        public string? LastAppName { get; private set; }
        public string? LastExePath { get; private set; }

        public bool Enable(string appName, string appPath)
        {
            EnableCalls++;
            LastAppName = appName;
            LastExePath = appPath;
            return ResultToReturn;
        }

        public bool Disable(string appName)
        {
            DisableCalls++;
            LastAppName = appName;
            return ResultToReturn;
        }

        public bool IsEnabled(string appName) => false;
    }

    class NoopNotifyService : INotifyService
    {
        public bool IsRunning => false;
        public void Start(int durationMinutes, int displayNotifySeconds) { }
        public void Stop() { }
        public void TriggerNow(int durationMinutes, int displayNotifySeconds) { }
    }

    [Test]
    public void SaveConfig_Enable_PersistsAndCallsAutoStart()
    {
        var cfg = new FakeAppConfig();
        var auto = new FakeAutoStartService();
        var vm = new MainViewModel(cfg, new NoopNotifyService(), auto, new LocalizationService(cfg), NullLogger<MainViewModel>.Instance)
        {
            AutoStart = true
        };

        vm.SaveConfigCommand.Execute(null);

        Assert.That(auto.EnableCalls, Is.EqualTo(1));
        Assert.That(cfg.AutoStart, Is.True);
        Assert.That(cfg.Saved, Is.True);
        Assert.That(auto.LastAppName, Is.EqualTo("Protect_Eyes"));
        Assert.That(string.IsNullOrWhiteSpace(auto.LastExePath), Is.False);
    }

    [Test]
    public void SaveConfig_Disable_CallsAutoStartDisable()
    {
        var cfg = new FakeAppConfig { AutoStart = true };
        var auto = new FakeAutoStartService();
        var vm = new MainViewModel(cfg, new NoopNotifyService(), auto, new LocalizationService(cfg), NullLogger<MainViewModel>.Instance)
        {
            AutoStart = false
        };

        vm.SaveConfigCommand.Execute(null);

        Assert.That(auto.DisableCalls, Is.EqualTo(1));
        Assert.That(cfg.AutoStart, Is.False);
    }
}
