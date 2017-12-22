using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using Windows.ApplicationModel.Core;

namespace TourBackendUI
{
    public static class Helpers
    {
        /// <summary>
        /// Converts an Emgu Mat to a WritableBitmap
        /// </summary>
        /// <param name="mat">Mat to be converted</param>
        /// <returns></returns>
        public static WriteableBitmap ToWritableBitmap(this Mat mat)
        {
            var size = mat.Size;
            var bmp = new WriteableBitmap(size.Width, size.Height);
            var buffer = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            using (var resultImage = new Image<Bgra, byte>(bmp.PixelWidth, bmp.PixelHeight, bmp.PixelWidth * 4, handle.AddrOfPinnedObject()))
            {
                resultImage.ConvertFrom(mat);
            }
            handle.Free();
            using (var resultStream = bmp.PixelBuffer.AsStream())
            {
                resultStream.Write(buffer, 0, buffer.Length);
            }
            return bmp;
        }

        /// <summary>
        /// Converts a StorageFile to an Emgu Mat
        /// </summary>
        /// <param name="file">StorageFile to be converted</param>
        /// <returns></returns>
        public static async Task<Mat> ToMatAsync(this StorageFile file)
        {
            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                var s = new Size((int)decoder.PixelWidth, (int)decoder.PixelHeight);

                var transform = new BitmapTransform();
                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                var sourcePixels = pixelData.DetachPixelData();
                var handle = GCHandle.Alloc(sourcePixels, GCHandleType.Pinned);
                using (var img = new Image<Bgra, byte>(s.Width, s.Height, s.Width * 4, handle.AddrOfPinnedObject()))
                {
                    var m = new Mat();
                    CvInvoke.CvtColor(img, m, ColorConversion.Bgra2Bgr);
                    handle.Free();
                    return m;
                }
            }
        }
        // Attention: Do NOT use in conjunction with Preview - Doesn't work!
        /// <summary>
        /// Is supposed to convert a MediaCapture to an Emgu Mat
        /// </summary>
        /// <param name="mediaCapture"></param>
        /// <returns></returns>
        public static async Task<Mat> ToMatAsync(this MediaCapture mediaCapture)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await mediaCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng(), stream));
                await mediaCapture.StopPreviewAsync();
                await mediaCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng(), stream);
                await mediaCapture.StartPreviewAsync();
                stream.Seek(0);
                var data = new byte[stream.Size];
                await stream.AsStreamForRead().ReadAsync(data, 0, data.Length);
                stream.Dispose();
                var result = new Mat();
                if (data.Length != 0)
                {
                    CvInvoke.Imdecode(data, ImreadModes.Grayscale, result);
                }
                else
                {
                    Debug.WriteLine("Buffer length = 0");
                }
                //var img = result.ToImage<Bgr, byte>();
                //var s = img.ToJpegData();
                //Debug.WriteLine(Convert.ToBase64String(s));
                return result;

            }
        }
    }
}