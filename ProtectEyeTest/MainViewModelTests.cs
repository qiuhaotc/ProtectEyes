using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ProtectEyes.Services;
using ProtectEyes.Services.Implementations;
using ProtectEyes.Services.Interfaces;
using ProtectEyes.ViewModels;

namespace ProtectEyeTest;

[System.Runtime.Versioning.SupportedOSPlatform("windows6.1")]
public class MainViewModelTests
{
    class FakeAppConfig : IAppConfig
    {
        public int RunBetween { get; set; } = 20;
        public int DisplayNotifySeconds { get; set; } = 10;
        public bool AutoStart { get; set; }
        public string LanguageMode { get; set; } = "en-US";
        public void Save() { }
    }

    class FakeAutoStart : IAutoStartService
    {
        public bool Enable(string appName, string appPath) => true;
        public bool Disable(string appName) => true;
        public bool IsEnabled(string appName) => false;
    }

    class RecordingNotifyService : INotifyService
    {
        public int StartCalls { get; private set; }
        public int StopCalls { get; private set; }
        public int TriggerNowCalls { get; private set; }
        public int LastDuration { get; private set; }
        public int LastDisplaySeconds { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start(int durationMinutes, int displayNotifySeconds)
        {
            StartCalls++;
            LastDuration = durationMinutes;
            LastDisplaySeconds = displayNotifySeconds;
            IsRunning = true;
        }

        public void Stop()
        {
            StopCalls++;
            IsRunning = false;
        }

        public void TriggerNow(int durationMinutes, int displayNotifySeconds)
        {
            TriggerNowCalls++;
            LastDuration = durationMinutes;
            LastDisplaySeconds = displayNotifySeconds;
        }
    }

    [Test, Apartment(ApartmentState.STA)]
    public void Constructing_Starts_NotifyService_When_ShouldContinue_True()
    {
        var notify = new RecordingNotifyService();
        var config = new FakeAppConfig();
        var vm = new MainViewModel(config, notify, new FakeAutoStart(), new LocalizationService(config), NullLogger<MainViewModel>.Instance);

        Assert.That(vm.ShouldContinue, Is.True);
        Assert.That(notify.StartCalls, Is.EqualTo(1));
        Assert.That(notify.LastDuration, Is.EqualTo(20));
        Assert.That(notify.LastDisplaySeconds, Is.EqualTo(10));
    }

    [Test, Apartment(ApartmentState.STA)]
    public void Setting_ShouldContinue_False_Stops_Cycle()
    {
        var notify = new RecordingNotifyService();
        var config = new FakeAppConfig();
        var vm = new MainViewModel(config, notify, new FakeAutoStart(), new LocalizationService(config), NullLogger<MainViewModel>.Instance);

        vm.ShouldContinue = false;

        Assert.That(notify.StopCalls, Is.GreaterThanOrEqualTo(1));
        Assert.That(vm.StatusDesc, Does.Contain("Paused"));
    }

    [Test, Apartment(ApartmentState.STA)]
    public void SimulateNotifyCommand_Triggers_NotifyService_Immediately()
    {
        var notify = new RecordingNotifyService();
        var config = new FakeAppConfig();
        var vm = new MainViewModel(config, notify, new FakeAutoStart(), new LocalizationService(config), NullLogger<MainViewModel>.Instance)
        {
            Duration = 5,
            DisplayNotifySeconds = 7
        };

        vm.SimulateNotifyCommand.Execute(null);

        Assert.That(notify.TriggerNowCalls, Is.EqualTo(1));
        Assert.That(notify.LastDuration, Is.EqualTo(5));
        Assert.That(notify.LastDisplaySeconds, Is.EqualTo(7));
    }
}
