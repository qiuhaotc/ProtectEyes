using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace ProtectEyes
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChange([CallerMemberName]string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        public void NotifyPropertyChange(Func<string> propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName.Invoke()));
        }

        public static void InvokeDispatcher(Action action, Dispatcher dispatcher, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
        {
            dispatcher?.BeginInvoke(dispatcherPriority, action);
        }

        protected DispatcherTimer GetDispatcherTimer(TimeSpan interval, EventHandler tick, bool runOnceThenStop = true, bool autoStart = true)
        {
            var timer = new DispatcherTimer();
            timer.Interval = interval;
            timer.Tick += tick;

            if (autoStart)
            {
                timer.Start();
            }

            if (runOnceThenStop)
            {
                timer.Tick += StopTimer;
            }

            return timer;
        }

        void StopTimer(object sender, EventArgs e)
        {
            (sender as DispatcherTimer)?.Stop();
        }
    }
}
