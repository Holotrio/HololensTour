using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Emgu.CV;

namespace TourBackend
{
    public class CameraFeedSyncObject
    {

        public Int64 timestamp { get; private set; }

        public Mat frame { get; private set; }
        public readonly string id;
        public object thisLock = new Object();

        public event EventHandler FrameUpdated;

        public CameraFeedSyncObject(string _id)
        {
            id = _id;
        }

        protected void OnFrameUpdated(EventArgs e)
        {
            EventHandler handler = FrameUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// <para>WPF GUI to Framework</para>
        /// <para>Updates Camerfeedsyncobjects with the bitmap of the selected picture</para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bitmap"></param>
        public void UpdateCameraFeedSyncObject(Int64 _timestamp, Mat _frame)
        {
            if (_frame != null) {
                lock (thisLock) {
                    timestamp = _timestamp;
                    frame = _frame.Clone();
                    OnFrameUpdated(EventArgs.Empty);
                }
            }
        }

    }
}
