using NUnit.Framework;
using ProtectEyes.Services;

namespace ProtectEyeTest
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ConfigTests
    {
        [Test]
        public void TestSaveConfig()
        {
            var cfg = new JsonAppConfig();
            cfg.RunBetween = 25;
            cfg.DisplayNotifySeconds = 11;
            cfg.AutoStart = true;
            cfg.Save();

            var reload = new JsonAppConfig();
            Assert.That(reload.RunBetween, Is.EqualTo(25));
            Assert.That(reload.DisplayNotifySeconds, Is.EqualTo(11));
            Assert.That(reload.AutoStart, Is.True);
        }
    }
}
