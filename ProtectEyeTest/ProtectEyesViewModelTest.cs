using NUnit.Framework;
using ProtectEyes;
using System.Windows.Forms;

namespace ProtectEyeTest
{
    public class ProtectEyesViewModelTest
    {
        [Test, RequiresSTA]
        public void TestShowNotifyWindows()
        {
            var window = new MainWindow();
            window.Show();
            window.ProtectEyesModel.ShouldContinue = true;
            Assert.AreEqual(Screen.AllScreens.Length, window.ProtectEyesModel.NotifyWindows.Count);
            Assert.IsFalse(window.ProtectEyesModel.NotifyWindows[0].IsVisible);

            window.ProtectEyesModel.ShowNotifyWindows();
            Assert.IsTrue(window.ProtectEyesModel.NotifyWindows[0].IsVisible);
        }
    }
}
