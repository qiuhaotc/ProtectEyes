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

        public static void InvokeDispatcher(Action action, Dispatcher dispatcher, DispatcherPriority dispatcherPriority = DispatcherPriority.ApplicationIdle)
        {
            dispatcher?.BeginInvoke(dispatcherPriority, action);
        }
    }
}
