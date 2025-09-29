using Microsoft.Win32;

namespace ProtectEyes.Services;

public class AutoStartManager : IAutoStartManager
{
    const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

    public bool Enable(string appName, string exePath)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true) ??
                             Registry.CurrentUser.CreateSubKey(RunKey, true);
            key?.SetValue(appName, '"' + exePath + '"');
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Disable(string appName)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
            if (key == null) return true;
            if (Array.Exists(key.GetValueNames(), n => string.Equals(n, appName, StringComparison.OrdinalIgnoreCase)))
            {
                key.DeleteValue(appName, false);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsEnabled(string appName)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false);
            if (key == null) return false;
            return Array.Exists(key.GetValueNames(), n => string.Equals(n, appName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }
}
