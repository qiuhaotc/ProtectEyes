namespace ProtectEyes.Services;

public interface IAppConfig
{
    int RunBetween { get; set; }
    int DisplayNotifySeconds { get; set; }
    bool AutoStart { get; set; }
    void Save();
}
