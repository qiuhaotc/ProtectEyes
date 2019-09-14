using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ProtectEyes
{
    public class NotifyViewModel : BaseViewModel
    {
        public NotifyViewModel(NotifyWindow notifyWindow)
        {
            this.notifyWindow = notifyWindow;
            RestPic = BitmapToBitmapImage(GetBG());
            RestDesc = $"Please Take A Rest, Time Remain {TimeoutSeconds - timeTick}s";
        }

        public void StartCountTime()
        {
            updateRestDesc = new Timer(new TimerCallback(UpdateRestDesc), null, (int)TimeSpan.FromSeconds(TimeoutSeconds).TotalMilliseconds, 500);
            new Timer(new TimerCallback(notifyWindow.CloseForm), null, (int)TimeSpan.FromSeconds(TimeoutSeconds).TotalMilliseconds, Timeout.Infinite);
        }

        NotifyWindow notifyWindow;
        int timeTick;
        Timer updateRestDesc;

        void UpdateRestDesc(object state)
        {
            timeTick++;

            if (timeTick >= TimeoutSeconds)
            {
                updateRestDesc.Change(-1, -1);
            }

            RestDesc = $"Please Take A Rest, Time Remain {TimeoutSeconds - timeTick}s";
        }

        public string RestDesc {
            get => restDesc;
            set
            {
                restDesc = value;
                NotifyPropertyChange();
            }
        }

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
        int TimeoutSeconds => int.Parse(ConfigurationManager.AppSettings["DisplayNotifySeconds"]);

        static Bitmap[] bitmapImages = new[] { Resource.BG1, Resource.BG2, Resource.BG3, Resource.BG4 };
        private string restDesc;

        static Bitmap GetBG()
        {
            return bitmapImages[new Random().Next(0, 4)];
        }
    }
}
