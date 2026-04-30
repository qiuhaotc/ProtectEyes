using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ProtectEyes.Services.Implementations;
using ProtectEyes.ViewModels;
using ProtectEyeTest.TestHelpers;

namespace ProtectEyeTest;

[TestFixture, System.Runtime.Versioning.SupportedOSPlatform("windows6.1"), Apartment(ApartmentState.STA)]
public class NotifyViewModelTimerTests
{
    [Test]
    public void Countdown_Text_Updates_And_AutoClose_Triggered()
    {
        var mockTick = new MockManualTimer();
        var mockClose = new MockManualTimer();
        bool closed = false;

        var localization = new LocalizationService(new TestAppConfig());
        var vm = new NotifyViewModel(mockClose, mockTick, localization, NullLogger<NotifyViewModel>.Instance);
        vm.Configure(displayNotifySeconds: 3, onClosedOverride: () => closed = true);

        vm.StartCountTime();
        Assert.That(vm.RestDesc, Does.Contain("3"));

        mockTick.Advance(TimeSpan.FromSeconds(1));
        Assert.That(vm.RestDesc, Does.Contain("2"));

        mockTick.Advance(TimeSpan.FromSeconds(1));
        Assert.That(vm.RestDesc, Does.Contain("1"));

        // 第 3 秒触发关闭
        mockTick.Advance(TimeSpan.FromSeconds(1));
        mockClose.Advance(TimeSpan.FromSeconds(3));
        Assert.That(closed, Is.True, "应当触发关闭回调");
    }
}
