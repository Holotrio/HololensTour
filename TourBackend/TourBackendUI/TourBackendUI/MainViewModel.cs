﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Emgu.CV;
using TourBackend;
using Panel = Windows.Devices.Enumeration.Panel;
using Windows.ApplicationModel.Core;

namespace TourBackendUI
{
    public class MainViewModel : ViewModel
    {
        public CameraFeedSyncObject CameraFeedSyncObject = new CameraFeedSyncObject(Guid.Empty.ToString());
        Int64 _lasttimestamp = DateTime.Now.Ticks;

        //Initialisierung SyncObject
        public SyncObject SyncObject = new SyncObject("syncid", new Dictionary<int, CodeObject>());
        public Dictionary<int, CodeObject> CopyOfDict = new Dictionary<int, CodeObject>();

        private readonly OnDemandCamera _frameSource;
        CommandTestFrames _frames;

        readonly FrameWork _frameWork;
        private MediaCapture _mediaCapture;
        private CaptureElement _stream;

        public Int64 inputtime;
        public Int64 outputtime;
        public Int64 rtt;

        //int numbersOfMarker = 1;

        public MediaCapture MediaCapture
        {
            get { return _mediaCapture; }
            set
            {
                if (Equals(value, _mediaCapture)) return;
                _mediaCapture = value;
                OnPropertyChanged();
            }
        }

        public CaptureElement Stream
        {
            get { return _stream; }
            set
            {
                if (Equals(value, _stream)) return;
                _stream = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand InitCam { get; set; }

        public MainViewModel()
        {
            //DesignModeOnly
            Markers = new ObservableCollection<int>
            {
                1,2,3,4,5,6,7
            };
        }

        public MainViewModel(ref CaptureElement streamPreview)
        {
            _stream = streamPreview;

            Markers = new ObservableCollection<int>
            {
                1,2,3
            };
            InitCam = new RelayCommand(InitCamera);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += GetFrame;


            SyncObject.SetTimeStamp(_lasttimestamp);

            SyncObject.SyncObjectUpdated += OnSyncObjectUpdated;
            _frameSource = new OnDemandCamera(30);

            CodeObject[] codeobjs = new CodeObject[1024];
            var dict = Utils.HelpForTesting.CreateDictionaryForInitialization(1024);
            foreach (KeyValuePair<int, CodeObject> pair in dict) {
                codeobjs.SetValue(pair.Value, pair.Key-1);
            }

            _frameWork = new FrameWork(SyncObject, CameraFeedSyncObject, codeobjs);
            _frameWork.Initialize();

            Timer.Start();

        }

        public async void InitCamera()
        {
            var app = Application.Current as App;
            if (app.WebcamReciever != null && app.WebcamReciever.Ready) return;

            try
            {
                app.WebcamReciever = new WebcamReciever(Panel.Front, ref _stream);

                MediaCapture = app.WebcamReciever.MediaCapture;
                Stream = app.WebcamReciever.StreamPreview;
                if (!app.WebcamReciever.Ready)
                {
                    Debug.WriteLine("WebcamReceiver not ready!");
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine("Initializing camera failed:");
               // Debug.WriteLine(e);
            }

        }

        private void GetFrame(object sender, object e)
        {
            if (_frameSource.Ready)
            {
                var testmat = _frameSource.Mat.Clone();

                UpdateCamerFeedSyncObject(Guid.NewGuid().ToString(), testmat);
            }
        }

        public ObservableCollection<int> Markers { get; set; }


        public DispatcherTimer Timer { get; } = new DispatcherTimer();

        /// <summary>
        /// <para>WPF GUI to Framework</para>
        /// <para>Updates Camerfeedsyncobjects with the bitmap of the selected picture</para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bitmap"></param>
        public void UpdateCamerFeedSyncObject(string id, Mat bitmap)
        {
            //Debug.WriteLine(Convert.ToBase64String(bitmap.GetData()));
            inputtime = DateTime.Now.Ticks;
            CameraFeedSyncObject.timestamp = DateTime.Now.Ticks;
            CameraFeedSyncObject.bitmap = bitmap.Clone();
            CameraFeedSyncObject.UpdateFrame();
            
        }

        /// <summary>
        /// <para>Framework to WPF GUI</para>
        /// <para>Event which will call GetTourState when SyncObject is updated</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSyncObjectUpdated(object sender, EventArgs e)
        {
            GetTourState();
            outputtime = DateTime.Now.Ticks;
            rtt = outputtime - inputtime;
            // Make it shine, boy
        }


        /// <summary>
        /// Gets the new tourstate and adds the detected markers to the combobox
        /// </summary>
        public async void GetTourState()
        {
            if (SyncObject.timestamp != _lasttimestamp)
            {
                _lasttimestamp = SyncObject.timestamp;
                CopyOfDict = DeepCopy.CopyInt(SyncObject.dict);
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => Markers.Clear());
                //ReadDictionaryData
                foreach (int objectid in SyncObject.dict.Keys)
                {
                    //CodeObject with current key
                    CodeObject obj = CopyOfDict[objectid];

                    //Add items to dropdownlist 
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => Markers.Add(obj.id));

                }
            }
        }


    }
}