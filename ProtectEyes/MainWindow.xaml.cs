using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace ProtectEyes
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MenuItem[] AreaMenuItems { get; private set; }
        public bool CanClose { get; private set; }
        public NotifyIcon NotifyIcon { get; private set; }

        public List<NotifyWindow> NotifyWindows { get; set; } = new List<NotifyWindow>();

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            Hide();
        }

        void InitWindow()
        {
            NotifyIcon = new NotifyIcon();
            NotifyIcon.Text = "Protect Eye";
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                NotifyIcon.Icon = Resource.SunGlasses;
            }

            NotifyIcon.Visible = true;

            NotifyIcon.DoubleClick += (sender, e) =>
            {
                if (IsVisible)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            };


            AreaMenuItems = new[]
            {
                new MenuItem("Close", (sender, e)=>{
                    ExitApp();
                })
            };

            if (AreaMenuItems != null && AreaMenuItems.Length > 0)
            {
                NotifyIcon.ContextMenu = new ContextMenu(AreaMenuItems.ToArray());
            }

            Closing += MainWindow_Closing;

            foreach (var scr in Screen.AllScreens)
            {
                var notifyWindow = new NotifyWindow(scr.WorkingArea);
                notifyWindow.Show();
                NotifyWindows.Add(notifyWindow);
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ExitApp()
        {
            foreach (var window in NotifyWindows)
            {
                if (window.IsVisible)
                {
                    window.Close();
                }
            }

            NotifyIcon.Dispose();

            Environment.Exit(0);
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!CanClose)
            {
                Hide();
                e.Cancel = true;
            }
            else
            {
                ExitApp();
            }
        }
    }
}
