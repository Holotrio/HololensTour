using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

#if !UNITY_EDITOR && UNITY_METRO // UWP specific DLLs
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Graphics.Imaging;

using System.Threading.Tasks;

#endif

/*
 * Usage:
 * 1. Call Initialize with the CameraFeedSyncObject as argument
 
     */


#if !UNITY_EDITOR && UNITY_METRO
public class VideoController : MonoBehaviour
{

    private MediaCapture mediaCapture = null;
    public TourBackend.CameraFeedSyncObject sync;
    private MediaFrameReader frameReader = null;
    private string TAG = "debug...";
    public System.Diagnostics.Stopwatch stopwatch = null;

    public async Task Initialize(TourBackend.CameraFeedSyncObject _sync)
    {
        sync = _sync;
        await InitializeMediaCaptureAsyncTask();
        await StartFrameReaderAsyncTask();
    }

    public void Start() {
    }
    public void Update() {
    }

    public async Task<bool> InitializeMediaCaptureAsyncTask()
    {

        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        if (mediaCapture != null)
        {
            Debug.Log(TAG + ": InitializeMediaCaptureAsyncTask() fails because mediaCapture is not null");
            return false;
        }

        var allGroups = await MediaFrameSourceGroup.FindAllAsync();
        foreach (var group in allGroups)
        {
            Debug.Log(group.DisplayName + ", " + group.Id);
        }

        if (allGroups.Count <= 0)
        {
            Debug.Log(TAG + ": InitializeMediaCaptureAsyncTask() fails because there is no MediaFrameSourceGroup");
            return false;
        }

        // Initialize mediacapture with the source group.
        mediaCapture = new MediaCapture();
        var settings = new MediaCaptureInitializationSettings
        {
            SourceGroup = allGroups[0],
            // This media capture can share streaming with other apps.
            SharingMode = MediaCaptureSharingMode.SharedReadOnly,
            // Only stream video and don't initialize audio capture devices.
            StreamingCaptureMode = StreamingCaptureMode.Video,
            // Set to CPU to ensure frames always contain CPU SoftwareBitmap images
            // instead of preferring GPU D3DSurface images.
            MemoryPreference = MediaCaptureMemoryPreference.Cpu
        };

        await mediaCapture.InitializeAsync(settings);
        Debug.Log(TAG + ": MediaCapture is successfully initialized in shared mode.");

        try
        {
            var mediaFrameSourceVideoPreview = mediaCapture.FrameSources.Values.Single(x => x.Info.MediaStreamType == MediaStreamType.VideoPreview);
            var minResFormat = mediaFrameSourceVideoPreview.SupportedFormats.OrderBy(x => x.VideoFormat.Width * x.VideoFormat.Height).FirstOrDefault();
            await mediaFrameSourceVideoPreview.SetFormatAsync(minResFormat);
            Debug.Log(TAG + ": minResFormat.Subtype is " + minResFormat.Subtype);
            frameReader = await mediaCapture.CreateFrameReaderAsync(mediaFrameSourceVideoPreview, minResFormat.Subtype);
            frameReader.FrameArrived += OnFrameArrived;
            // TODO: framewidth
            /*
            controller.frameWidth = Convert.ToInt32(minResFormat.VideoFormat.Width);
            controller.frameHeight = Convert.ToInt32(minResFormat.VideoFormat.Height);
            videoBufferSize = controller.frameWidth * controller.frameHeight * 4;
            */
            Debug.Log(TAG + ": FrameReader is successfully initialized");
        }
        catch (Exception e)
        {
            Debug.Log(TAG + ": FrameReader is not initialized");
            Debug.Log(TAG + ": Exception: " + e);
            return false;
        }

        Debug.Log(TAG + ": InitializeMediaCaptureAsyncTask() is successful");
        return true;
    }

    public async Task<bool> StartFrameReaderAsyncTask()
    {
        MediaFrameReaderStartStatus mediaFrameReaderStartStatus = await frameReader.StartAsync();
        if (mediaFrameReaderStartStatus == MediaFrameReaderStartStatus.Success)
        {
            Debug.Log(TAG + ": StartFrameReaderAsyncTask() is successful");
            return true;
        }
        else
        {
            Debug.Log(TAG + ": StartFrameReaderAsyncTask() is not successful, status = " + mediaFrameReaderStartStatus);
            return false;
        }
    }

    public async Task<bool> StopFrameReaderAsyncTask()
    {
        await frameReader.StopAsync();
        Debug.Log(TAG + ": StopFrameReaderAsyncTask() is successful");
        return true;
    }

    private void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
    {
        ARUWPUtils.VideoTick();
        using (var frame = sender.TryAcquireLatestFrame())
        {
            if (frame != null)
            {
                var softwareBitmap = SoftwareBitmap.Convert(frame.VideoMediaFrame.SoftwareBitmap, BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore);
                UpdateCameraSync(softwareBitmap);
            }
        }
    }

    private void UpdateCameraSync(SoftwareBitmap _softwarebitmap)
    {
        lock (sync.thisLock)
        {
            sync.bitmap = _softwarebitmap;
            sync.timestamp = stopwatch.ElapsedMilliseconds;
        }
        sync.UpdateFrame();
    }

}

#endif
