using Microsoft.Win32;
using ProtectEyes.Services.Interfaces;

namespace ProtectEyes.Services.Implementations;

public class AutoStartService : IAutoStartService
{
    const string RunRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public bool Enable(string appName, string appPath)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true);
            if (key == null) return false;

            key.SetValue(appName, appPath);
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
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true);
            if (key == null) return false;

            key.DeleteValue(appName, false);
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
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, false);
            if (key == null) return false;

            var value = key.GetValue(appName);
            return value != null;
        }
        catch
        {
            return false;
        }
    }
}
