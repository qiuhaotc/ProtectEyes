using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ProtectEyes
{
    /// <summary>
    /// FullWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyWindow : Window
    {
        public Rectangle Area { get; set; }

        public NotifyWindow(Rectangle area)
        {
            Area = area;
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new System.Windows.Point(Area.Right, Area.Bottom));

                Left = corner.X - ActualWidth;
                Top = corner.Y - ActualHeight;
            }));
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
