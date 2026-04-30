using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ProtectEyes.Services.Implementations;
using ProtectEyes.ViewModels;
using ProtectEyeTest.TestHelpers;

namespace ProtectEyeTest;

[System.Runtime.Versioning.SupportedOSPlatform("windows6.1")]
public class NotifyViewModelTest
{
    [Test, Apartment(ApartmentState.STA)]
    public void Configure_Sets_Initial_RestDesc()
    {
        var localization = new LocalizationService(new TestAppConfig());
        var vm = new NotifyViewModel(new MockManualTimer(), new MockManualTimer(), localization, NullLogger<NotifyViewModel>.Instance);
        vm.Configure(displayNotifySeconds: 12);

        Assert.That(vm.RestDesc, Is.EqualTo("Please take a rest. Time remaining: 12s"));
    }
}
