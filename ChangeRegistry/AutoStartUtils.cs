using System;
using Microsoft.Win32;

namespace ChangeRegistry
{
    public class AutoStartUtils
    {
        public static bool SelfRunning(bool isStart, string exeName, string path)
        {
            var success = true;

            try
            {
                var local = Registry.LocalMachine;
                var key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
                }
                if (isStart)
                {
                    key.SetValue(exeName, path);
                    key.Close();
                }
                else
                {
                    foreach (var keyName in key.GetValueNames())
                    {
                        if (keyName.ToUpper() == exeName.ToUpper())
                        {
                            key.DeleteValue(exeName);
                            key.Close();
                        }
                    }
                }
            }
            catch
            {
                success = false;
            }

            return success;
        }

        private bool IsExistKey(string keyName)
        {
            var exist = false;

            try
            {
                var local = Registry.LocalMachine;
                var runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runs == null)
                {
                    var key2 = local.CreateSubKey("SOFTWARE");
                    var key3 = key2.CreateSubKey("Microsoft");
                    var key4 = key3.CreateSubKey("Windows");
                    var key5 = key4.CreateSubKey("CurrentVersion");
                    var key6 = key5.CreateSubKey("Run");
                    runs = key6;
                }

                foreach (string strName in runs.GetValueNames())
                {
                    if (strName.ToUpper() == keyName.ToUpper())
                    {
                        exist = true;
                        break;
                    }
                }
            }
            catch
            {
                exist = false;
            }

            return exist;
        }
    }
}
