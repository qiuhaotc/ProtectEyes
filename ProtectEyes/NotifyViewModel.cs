using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ProtectEyes
{
    public class NotifyViewModel : BaseViewModel
    {
        public NotifyViewModel(NotifyWindow notifyWindow, ProtectEyesViewModel protectEyesViewModel)
        {
            this.protectEyesViewModel = protectEyesViewModel;
            this.notifyWindow = notifyWindow;
            RestPic = BitmapToBitmapImage(GetBG());
            SetRestDesc();
            CloseAllNotifyWindowsCommand = new CommandHandler(CloseAllNotifyWindows, () => true);
        }

        public void StartCountTime()
        {
            updateRestDesc = new DispatcherTimer();
            updateRestDesc.Interval = TimeSpan.FromSeconds(1);
            updateRestDesc.Tick += UpdateRestDesc;
            updateRestDesc.Start();

            GetDispatcherTimer(TimeSpan.FromSeconds(DisplayNotifySeconds), CloseForm);
        }

        ProtectEyesViewModel protectEyesViewModel;
        NotifyWindow notifyWindow;
        int timeTick;
        DispatcherTimer updateRestDesc;

        void UpdateRestDesc(object sender,EventArgs e)
        {
            timeTick++;

            if (timeTick >= DisplayNotifySeconds)
            {
                updateRestDesc.Stop();
            }

            SetRestDesc();
        }

        public void SetRestDesc()
        {
            RestDesc = $"Please Take A Rest, Times Remain: {DisplayNotifySeconds - timeTick}s";
        }

        string restDesc;

        public string RestDesc {
            get => restDesc;
            set
            {
                restDesc = value;
                NotifyPropertyChange();
            }
        }

        public ICommand CloseAllNotifyWindowsCommand { get; set; }

        internal void CloseWithOutNotify()
        {
            notifyWindow.Close();
        }

        internal void CloseWithNotify()
        {
            notifyWindow.Close();
            protectEyesViewModel.NotifyClosed(notifyWindow);
        }

        void CloseForm(object sender, EventArgs e)
        {
            CloseWithNotify();
        }

        void CloseAllNotifyWindows()
        {
            foreach (var item in protectEyesViewModel.NotifyWindows.ToArray())
            {
                item.NotifyViewModel.CloseWithNotify();
            }
        }


        int DisplayNotifySeconds { get; } = int.Parse(ConfigurationManager.AppSettings["DisplayNotifySeconds"]);

        public BitmapImage RestPic { get; set; }
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                stream.Position = 0;
                var result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        static Bitmap[] bitmapImages = new[] { Resource.BG1, Resource.BG2, Resource.BG3, Resource.BG4 };

        static Bitmap GetBG()
        {
            return bitmapImages[new Random().Next(0, 4)];
        }
    }
}
