using NUnit.Framework;
using ProtectEyes;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace ProtectEyeTest
{
    public class NotifyWindowTest
    {
        [Test, RequiresSTA]
        public void TestCloseButton()
        {
            var rectangle = new Rectangle(0,0, 500, 500);
            var window = new NotifyWindow(rectangle, null);
            window.Show();
            Assert.IsTrue(window.IsVisible);

            window.CloseButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.IsFalse(window.IsVisible);
        }
    }
}
