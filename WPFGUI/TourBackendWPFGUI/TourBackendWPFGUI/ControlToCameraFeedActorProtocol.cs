﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TourBackend
{
    public class CameraFeedSyncObject
    {

        public Int64 timestamp;

        public Bitmap bitmap;
        public string id;
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

        public void UpdateFrame()
        {
            OnFrameUpdated(EventArgs.Empty);
        }

    }

    public class NewFrameArrived
    {
        public string id;
        public Bitmap bitmap;

        public NewFrameArrived(string _id, Bitmap _bitmap)
        {
            id = _id;
            bitmap = _bitmap;
        }
    }
}
