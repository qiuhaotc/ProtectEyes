using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace ProtectEyes
{
    public class ProtectEyesViewModel : BaseViewModel
    {
        public ProtectEyesViewModel()
        {
            shouldContinue = true;
            Duration = int.Parse(ConfigurationManager.AppSettings["RunBetween"]);
            StartDurationIfNeeded();
        }

        int duration;
        public string DurationDesc => $"Duration: {Duration} minutes";

        public int Duration
        {
            get => duration;
            set
            {
                duration = value;
                NotifyPropertyChange(nameof(DurationDesc));
            }
        }

        public void ShowNotifyWindows()
        {
            if (ShouldContinue)
            {
                foreach (var notifyWindow in NotifyWindows)
                {
                    InvokeDispatcher(() => { notifyWindow.Show(); }, notifyWindow.Dispatcher);
                }
            }
        }

        bool shouldContinue;
        public bool ShouldContinue
        {
            get => shouldContinue;
            set
            {
                shouldContinue = value;
                StartDurationIfNeeded();
            }
        }
        public List<NotifyWindow> NotifyWindows { get; private set; } = new List<NotifyWindow>();

        internal void SaveConfig()
        {
            ConfigurationManager.AppSettings["RunBetween"] = Duration.ToString();
            SetConfigValue("RunBetween", Duration.ToString());
            StartDurationIfNeeded();
        }

        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                {
                    config.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    config.AppSettings.Settings.Add(key, value);
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        System.Threading.Timer threadTimer;

        void StartDurationIfNeeded()
        {
            if (ShouldContinue)
            {
                if(threadTimer != null)
                {
                    threadTimer.Change(-1, -1);
                }

                NotifyWindows.Clear();
                InitNotifyForm();
                threadTimer = new System.Threading.Timer(new TimerCallback(ShowNotifyWindow), null, (int)TimeSpan.FromSeconds(Duration).TotalMilliseconds, Timeout.Infinite);
            }
        }

        void InitNotifyForm()
        {
            foreach (var scr in Screen.AllScreens)
            {
                var notifyWindow = new NotifyWindow(scr.WorkingArea, this);
                NotifyWindows.Add(notifyWindow);
            }
        }

        public void ShowNotifyWindow(object state)
        {
            ShowNotifyWindows();
        }

        public void NotifyClosed(NotifyWindow notifyWindow)
        {
            NotifyWindows.Remove(notifyWindow);

            if (NotifyWindows.Count == 0)
            {
                StartDurationIfNeeded();
            }
        }
    }
}