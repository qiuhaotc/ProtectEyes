namespace ProtectEyes.Services.Interfaces;

public interface IAutoStartService
{
    bool Enable(string appName, string appPath);
    bool Disable(string appName);
    bool IsEnabled(string appName);
}
