using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Animation;

namespace ProtectEyes
{
    /// <summary>
    /// FullWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyWindow : Window
    {
        ProtectEyesViewModel protectEyesViewModel;

        public Rectangle Area { get; set; }

        public NotifyWindow(Rectangle area, ProtectEyesViewModel protectEyesViewModel)
        {
            this.protectEyesViewModel = protectEyesViewModel;
            Area = area;
            DataContext = new NotifyViewModel(this, protectEyesViewModel);
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var corner = transform.Transform(new System.Windows.Point(Area.Right, Area.Bottom));
            Left = corner.X - ActualWidth;
            Top = corner.Y - ActualHeight;

            NotifyViewModel.StartCountTime();
        }

        public NotifyViewModel NotifyViewModel => (NotifyViewModel)DataContext;

        void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in protectEyesViewModel?.NotifyWindows)
            {
                item.NotifyViewModel.CloseWithNotify();
            }
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            anim.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, anim);
        }
    }
}
