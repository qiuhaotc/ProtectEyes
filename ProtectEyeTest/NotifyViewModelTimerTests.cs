using NUnit.Framework;
using ProtectEyes;

namespace ProtectEyeTest;

[TestFixture, System.Runtime.Versioning.SupportedOSPlatform("windows6.1"), Apartment(ApartmentState.STA)]
public class NotifyViewModelTimerTests
{
    class DummyProtectEyesVM : ProtectEyesViewModel
    {
        public DummyProtectEyesVM() : base() { }
    }

    [Test]
    public void Countdown_Text_Updates_And_AutoClose_Triggered()
    {
        var mockTick = new TestHelpers.MockManualTimer();
        var mockClose = new TestHelpers.MockManualTimer();
        bool closed = false;
        var parent = new DummyProtectEyesVM();
        parent.DisplayNotifySeconds = 3;
        var vm = new NotifyViewModel(null, parent, mockClose, mockTick, () => closed = true);

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
