using System.Drawing;
using NUnit.Framework;
using ProtectEyes;

namespace ProtectEyeTest
{
    public class NotifyViewModelTest
    {
        [Test, RequiresSTA]
        public void TestSetRestDesc()
        {
            var notifyModel = new NotifyViewModel(null, null);
            notifyModel.SetRestDesc();
            Assert.AreEqual("Please Take A Rest, Times Remain: 10s", notifyModel.RestDesc);
        }
    }
}
