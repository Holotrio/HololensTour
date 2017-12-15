using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Panel = Windows.Devices.Enumeration.Panel;

namespace TourBackendUI
{
    public class WebcamReciever
    {
        public MediaCapture MediaCapture { get; set; }
        public bool Ready { get; set; }

        public DeviceInformation Device { get; set; }
        public EnclosureLocation EnclosureLocation => Device.EnclosureLocation;
        public bool External => EnclosureLocation == null || EnclosureLocation.Panel == Panel.Unknown;
        public Panel Panel => EnclosureLocation.Panel;

        public CaptureElement StreamPreview { get; set; }

        public WebcamReciever(Panel desiredPanel, ref CaptureElement streamPreview)
        {
            InitCam(desiredPanel);
            StreamPreview = streamPreview;
        }

        public async Task<VideoFrame> Grab(MediaStreamType streamType = MediaStreamType.VideoPreview)
        {
            var frameProperties = MediaCapture.VideoDeviceController.GetMediaStreamProperties(streamType) as VideoEncodingProperties;
            var frame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)frameProperties.Width, (int)frameProperties.Height);
            return (await MediaCapture.GetPreviewFrameAsync(frame));
        }

        /// <summary>
        /// Basically converts a SoftwareBitmap to an Emgu Image
        /// </summary>
        /// <returns></returns>
        public async Task<Image<Bgr, byte>> GrabImage()
        {
            if (!Ready) return null;
            var bitmap = (await Grab()).SoftwareBitmap;
            var img = new Image<Bgr, byte>(bitmap.PixelWidth, bitmap.PixelHeight);
            // Effect is hard-coded to operate on BGRA8 format only
            if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8)
            {
                // In BGRA8 format, each pixel is defined by 4 bytes
                const int BYTES_PER_PIXEL = 4;

                using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
                using (var reference = buffer.CreateReference())
                {
                    unsafe
                    {
                        byte* data;
                        uint capacity;
                        ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

                        // Get information about the BitmapBuffer
                        var desc = buffer.GetPlaneDescription(0);

                        // Iterate over all pixels
                        for (int row = 0; row < desc.Height; row++)
                        {
                            for (int col = 0; col < desc.Width; col++)
                            {
                                // Index of the current pixel in the buffer (defined by the next 4 bytes, BGRA8)
                                var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;

                                // Read the current pixel information into b,g,r channels (leave out alpha channel)

                                // Boost the green channel, leave the other two untouched

                                img[row, col] = new Bgr(data[currPixel + 0], data[currPixel + 1], data[currPixel + 2]);
                            }
                        }
                    }
                }
            }
            return img;
        }
        /// <summary>
        /// Returns the Mat stored in the GrabImage Image
        /// </summary>
        /// <returns></returns>
        public async Task<Mat> GrabMat()
        {
            if (!Ready) return null;
            Emgu.CV.Image<Bgr, byte> ret = await GrabImage();

            return ret.Mat;
        }
        public async Task<SoftwareBitmap> GrabAndPreprocess(Func<int, int, int, byte, byte> preprocessing)
        {
            if (!Ready) return null;
            var bmp = (await Grab()).SoftwareBitmap;
            EditPixels(bmp, preprocessing);
            return bmp;
        }

        private async void InitCam(Panel desiredPanel)
        {
            if (Ready) return;

            MediaCapture = new MediaCapture();
            Device = await FindCameraDeviceByPanelAsync(desiredPanel);

            await MediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                VideoDeviceId = Device.Id,
                StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video,
                PhotoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.VideoPreview
            });

            StreamPreview.FlowDirection = FlowDirection.LeftToRight;
            StreamPreview.Source = MediaCapture;

            await MediaCapture.StartPreviewAsync();
            Ready = true;
        }


        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            var desiredDevice = allVideoDevices
                .FirstOrDefault(x => x.EnclosureLocation != null
                                     && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }


        private unsafe void EditPixels(SoftwareBitmap bitmap, Func<int, int, int, byte, byte> func)
        {
            // Effect is hard-coded to operate on BGRA8 format only
            if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8)
            {
                // In BGRA8 format, each pixel is defined by 4 bytes
                const int BYTES_PER_PIXEL = 4;

                using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
                using (var reference = buffer.CreateReference())
                {
                    // Get a pointer to the pixel buffer
                    byte* data;
                    uint capacity;
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

                    // Get information about the BitmapBuffer
                    var desc = buffer.GetPlaneDescription(0);

                    // Iterate over all pixels
                    for (int row = 0; row < desc.Height; row++)
                    {
                        for (int col = 0; col < desc.Width; col++)
                        {
                            // Index of the current pixel in the buffer (defined by the next 4 bytes, BGRA8)
                            var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;

                            // Read the current pixel information into b,g,r channels (leave out alpha channel)

                            // Boost the green channel, leave the other two untouched
                            data[currPixel + 0] = func(col, row, 0, data[currPixel + 0]);
                            data[currPixel + 1] = func(col, row, 1, data[currPixel + 1]);
                            data[currPixel + 2] = func(col, row, 2, data[currPixel + 2]);
                        }
                    }
                }
            }
        }
    }

    [ComImport]
    [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }
}