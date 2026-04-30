using System.Windows;
using System.Windows.Media.Animation;
using ProtectEyes.ViewModels;

namespace ProtectEyes;

/// <summary>
/// NotifyWindow.xaml 的交互逻辑
/// </summary>
public partial class NotifyWindow : Window
{
    public Rectangle Area { get; set; }
    public NotifyViewModel? NotifyViewModel { get; }

    public NotifyWindow(Rectangle area, NotifyViewModel viewModel)
    {
        Area = area;
        InitializeComponent();
        NotifyViewModel = viewModel;
        DataContext = viewModel;
        viewModel.RequestClose += OnRequestClose;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        var source = PresentationSource.FromVisual(this);
        if (source?.CompositionTarget is { } target)
        {
            var transform = target.TransformFromDevice;
            var corner = transform.Transform(new System.Windows.Point(Area.Right, Area.Bottom));
            Left = corner.X - ActualWidth;
            Top = corner.Y - ActualHeight;
        }

        NotifyViewModel?.StartCountTime();
    }

    private void OnRequestClose()
    {
        if (Dispatcher.CheckAccess())
        {
            Close();
        }
        else
        {
            Dispatcher.BeginInvoke(new Action(Close));
        }
    }

    void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Closing -= Window_Closing;
        e.Cancel = true;
        var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
        anim.Completed += (_, _) =>
        {
            if (NotifyViewModel is { } vm)
            {
                vm.RequestClose -= OnRequestClose;
            }
            Close();
        };
        BeginAnimation(OpacityProperty, anim);
    }
}
