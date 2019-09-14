using System;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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
            DataContext = new NotifyViewModel(this);
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            BaseViewModel.InvokeDispatcher(new Action(() =>
            {
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new System.Windows.Point(Area.Right, Area.Bottom));
                Left = corner.X - ActualWidth;
                Top = corner.Y - ActualHeight;
            }), Dispatcher);
            NotifyViewModel.StartCountTime();
        }

        NotifyViewModel NotifyViewModel => (NotifyViewModel)DataContext;

        bool isClosed;

        public void CloseForm(object state)
        {
            BaseViewModel.InvokeDispatcher(() => 
            {
                if (!isClosed)
                {
                    isClosed = true;
                    Close();
                }
            }, Dispatcher);
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isClosed)
            {
                isClosed = true;
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            protectEyesViewModel?.NotifyClosed(this);
            base.OnClosed(e);
        }
    }
}
