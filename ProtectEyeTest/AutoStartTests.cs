using NUnit.Framework;
using ProtectEyes;
using ProtectEyes.Services;

namespace ProtectEyeTest
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class AutoStartTests
    {
        class FakeAppConfig : IAppConfig
        {
            public int RunBetween { get; set; } = 20;
            public int DisplayNotifySeconds { get; set; } = 10;
            public bool AutoStart { get; set; } = false;
            public bool Saved { get; private set; }
            public void Save() => Saved = true;
        }

        class FakeAutoStartManager : IAutoStartManager
        {
            public int EnableCalls { get; private set; }
            public int DisableCalls { get; private set; }
            public bool ResultToReturn { get; set; } = true;
            public string? LastAppName { get; private set; }
            public string? LastExePath { get; private set; }

            public bool Enable(string appName, string exePath)
            {
                EnableCalls++;
                LastAppName = appName;
                LastExePath = exePath;
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

        [Test]
        public void AutoStart_Enable_Disable_UpdatesConfigOnSuccess()
        {
            var cfg = new FakeAppConfig { AutoStart = false };
            var auto = new FakeAutoStartManager { ResultToReturn = true };
            var vm = new ProtectEyesViewModel(cfg, null, auto);

            Assert.That(cfg.AutoStart, Is.False);
            vm.AutoStart = true;
            Assert.That(auto.EnableCalls, Is.EqualTo(1));
            Assert.That(cfg.AutoStart, Is.True, "Config should update to true when enable succeeds");
            Assert.That(auto.LastAppName, Is.EqualTo("Protect_Eyes"));
            Assert.That(string.IsNullOrWhiteSpace(auto.LastExePath), Is.False);
            Assert.That(auto.LastExePath, Does.Contain("ProtectEyes"));

            vm.AutoStart = false;
            Assert.That(auto.DisableCalls, Is.EqualTo(1));
            Assert.That(cfg.AutoStart, Is.False, "Config should update to false when disable succeeds");
        }

        [Test]
        public void AutoStart_NotUpdated_WhenManagerFails()
        {
            var cfg = new FakeAppConfig { AutoStart = false };
            var auto = new FakeAutoStartManager { ResultToReturn = false };
            var vm = new ProtectEyesViewModel(cfg, null, auto);

            vm.AutoStart = true; // fails
            Assert.That(auto.EnableCalls, Is.EqualTo(1));
            Assert.That(cfg.AutoStart, Is.False, "Config should remain unchanged when enable fails");

            cfg.AutoStart = true; // simulate external change
            auto.ResultToReturn = false;
            vm.AutoStart = false; // disable attempt fails
            Assert.That(auto.DisableCalls, Is.EqualTo(1));
            Assert.That(cfg.AutoStart, Is.True, "Config should remain true because disable failed");
        }
    }
}
