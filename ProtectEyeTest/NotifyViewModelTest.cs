using NUnit.Framework;
using ProtectEyes;

namespace ProtectEyeTest
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows6.1")]
    public class NotifyViewModelTest
    {
        [Test, Apartment(ApartmentState.STA)]
        public void TestSetRestDesc()
        {
            var vm = new ProtectEyesViewModel();
            var rect = System.Windows.Forms.Screen.PrimaryScreen!.WorkingArea;
            var notifyWindow = new NotifyWindow(rect, vm);
            var notifyModel = notifyWindow.NotifyViewModel;
            notifyModel.SetRestDesc();
            var expected = $"Please Take A Rest, Times Remain: {vm.DisplayNotifySeconds}s";
            Assert.That(notifyModel.RestDesc, Is.EqualTo(expected));
        }
    }
}
