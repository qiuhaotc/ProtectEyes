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

        public ProtectEyesViewModel ProtectEyesModel => (ProtectEyesViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            Hide();
            DataContext = new ProtectEyesViewModel();
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
                    Hide();
                }
                else
                {
                    Show();
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
        }

        void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ExitApp()
        {
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

        void Button_Click(object sender, RoutedEventArgs e)
        {
            ProtectEyesModel.SaveConfig();
        }
    }
}
