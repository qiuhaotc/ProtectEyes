namespace ProtectEyes.Services;

public interface IAutoStartManager
{
    bool Enable(string appName, string exePath);
    bool Disable(string appName);
    bool IsEnabled(string appName);
}
