using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace ProtectEyes
{
    public class ProtectEyesViewModel : BaseViewModel
    {
        public ProtectEyesViewModel()
        {
            Duration = int.Parse(ConfigurationManager.AppSettings["RunBetween"]);
            DisplayNotifySeconds = int.Parse(ConfigurationManager.AppSettings["DisplayNotifySeconds"]);
            SaveConfigCommand = new CommandHandler(SaveConfig, () => true);
            ShouldContinue = true;
            autoStart = bool.Parse(ConfigurationManager.AppSettings["AutoStart"]);
        }

        int duration;
        public string DurationDesc => $"Duration: {Duration} minutes";
        public string StatusDesc => $"Status: Is Running {ShouldContinue}";

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
                    notifyWindow.Show();
                }
            }
        }

        int displayNotifySeconds;
        public int DisplayNotifySeconds
        {
            get => displayNotifySeconds;
            set
            {
                displayNotifySeconds = value;
                NotifyPropertyChange(nameof(DisplayNotifySecondsDesc));
            }
        }

        public string DisplayNotifySecondsDesc => $"Notifycation Display Seconds: {DisplayNotifySeconds}s";
        public bool AutoStart
        {
            get => autoStart;
            set
            {
                autoStart = value;

                var appPath = Assembly.GetExecutingAssembly().Location;
                var path = Path.Combine(new FileInfo(appPath).DirectoryName, "ChangeRegistry.exe");

                try
                {
                    using (var process = Process.Start(new ProcessStartInfo(path, $"{autoStart} Protect_Eyes {appPath}")
                    {
                        Verb = "runas"
                    }))
                    {
                        process?.WaitForExit();
                        SetConfigValue("AutoStart", autoStart.ToString());
                    }
                }
                catch
                {
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
                NotifyPropertyChange(nameof(StatusDesc));
            }
        }
        public List<NotifyWindow> NotifyWindows { get; private set; } = new List<NotifyWindow>();

        public ICommand SaveConfigCommand { get; set; }

        void SaveConfig()
        {
            ConfigurationManager.AppSettings["RunBetween"] = Duration.ToString();
            ConfigurationManager.AppSettings["DisplayNotifySeconds"] = DisplayNotifySeconds.ToString();
            SetConfigValue("RunBetween", Duration.ToString());
            SetConfigValue("DisplayNotifySeconds", DisplayNotifySeconds.ToString());
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

        DispatcherTimer timer;

        void StartDurationIfNeeded()
        {
            if (ShouldContinue)
            {
                if (timer != null)
                {
                    timer.Stop();
                }

                NotifyWindows.ForEach(u => { u.NotifyViewModel.CloseWithOutNotify(); });
                NotifyWindows.Clear();
                InitNotifyForm();
                timer = GetDispatcherTimer(TimeSpan.FromMinutes(Duration), ShowNotifyWindow);
            }
            else
            {
                if (timer != null)
                {
                    timer.Stop();
                }

                NotifyWindows.ForEach(u => { u.NotifyViewModel.CloseWithOutNotify(); });
            }
        }

        void InitNotifyForm()
        {
            foreach (var scr in Screen.AllScreens)
            {
                var notifyWindow = new NotifyWindow(scr.WorkingArea, this);
                notifyWindow.Left = -100000; // Out side the window
                NotifyWindows.Add(notifyWindow);
            }
        }

        public void ShowNotifyWindow(object sender, EventArgs e)
        {
            ShowNotifyWindows();
        }

        static object syncLock = new object();
        private bool autoStart;

        public void NotifyClosed(NotifyWindow notifyWindow)
        {
            lock (syncLock)
            {
                NotifyWindows.Remove(notifyWindow);

                if (NotifyWindows.Count == 0)
                {
                    StartDurationIfNeeded();
                }
            }
        }
    }
}