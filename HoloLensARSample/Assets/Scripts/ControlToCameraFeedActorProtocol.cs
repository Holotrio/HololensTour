using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !UNITY_EDITOR && UNITY_METRO
using System.Threading.Tasks;
using Proto;
#endif

namespace TourBackend
{
    public class CameraFeedSyncObject
    {

        public Int64 timestamp;
#if !UNITY_EDITOR && UNITY_METRO
        public Windows.Graphics.Imaging.SoftwareBitmap bitmap;
#endif
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

#if !UNITY_EDITOR && UNITY_METRO
        public Windows.Graphics.Imaging.SoftwareBitmap bitmap;

        public NewFrameArrived(string _id, Windows.Graphics.Imaging.SoftwareBitmap _bitmap)
        {
            id = _id;
            bitmap = _bitmap;
        }
#endif
    }

}
