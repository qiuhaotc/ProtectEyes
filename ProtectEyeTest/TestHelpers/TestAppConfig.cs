using ProtectEyes.Services;

namespace ProtectEyeTest.TestHelpers;

public class TestAppConfig : IAppConfig
{
    public int RunBetween { get; set; } = 20;
    public int DisplayNotifySeconds { get; set; } = 10;
    public bool AutoStart { get; set; }
    public string LanguageMode { get; set; } = "en-US";
    public bool Saved { get; private set; }

    public void Save() => Saved = true;
}
