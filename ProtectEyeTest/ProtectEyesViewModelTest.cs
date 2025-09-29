using NUnit.Framework;
using ProtectEyes;

namespace ProtectEyeTest
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows6.1")]
    public class ProtectEyesViewModelTest
    {
        [Test, Apartment(ApartmentState.STA)]
        public void TestShowNotifyWindows()
        {
            var protectEyesViewModel = new ProtectEyesViewModel();
            var window = new MainWindow(protectEyesViewModel);
            window.Show();
            window.ProtectEyesModel.ShouldContinue = true;
            Assert.That(window.ProtectEyesModel.NotifyWindows.Count, Is.EqualTo(Screen.AllScreens.Length));
            Assert.That(window.ProtectEyesModel.NotifyWindows[0].IsVisible, Is.False);

            window.ProtectEyesModel.ShowNotifyWindows();
            Assert.That(window.ProtectEyesModel.NotifyWindows[0].IsVisible, Is.True);
        }
    }
}
