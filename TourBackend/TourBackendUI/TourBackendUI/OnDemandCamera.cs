using System;
using System.Runtime.CompilerServices;
using Windows.Media.Devices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;

namespace TourBackendUI
{
    public class OnDemandCamera
    {
        private Mat _mat = new Mat();
        private DispatcherTimer _timer = new DispatcherTimer();
        public OnDemandCamera(int fps)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(1000d / fps);
            _timer.Tick += Grab;
            _timer.Start();
        }

        private async void Grab(object sender, object ea)
        {
            var app = Application.Current as App;
            if (app?.WebcamReciever?.MediaCapture == null) return;
            try
            {
                _mat = await app.WebcamReciever.GrabMat();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private App App => Application.Current as App;
        private WebcamReciever WebcamReciever => App?.WebcamReciever;
        public bool Ready => WebcamReciever != null && WebcamReciever.Ready && _mat != null;
        public Mat Mat => Ready ? _mat : throw new InvalidOperationException("camera not ready");
        public WriteableBitmap WriteableBitmap => Mat.ToWritableBitmap();
        public Image<Bgr, byte> ImageBgrByte => Mat.ToImage<Bgr, byte>();
    }
}