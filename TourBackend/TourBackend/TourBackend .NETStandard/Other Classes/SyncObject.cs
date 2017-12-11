using System;
using System.Collections.Generic;
using System.Text;

namespace TourBackend
{
    public class SyncObject
    {
        public Int64 timestamp;

        public object thisLock = new Object();

        public string objectid;
        public Dictionary<int, CodeObject> dict;

        //Basic Konstruktor 
        public SyncObject(string _objectid, Dictionary<int, CodeObject> _dict)
        {
            objectid = _objectid;
            dict = _dict;
        }


        // Enables event based communication with user
        public event EventHandler SyncObjectUpdated;

        protected void OnSyncObjectUpdated(EventArgs e)
        {
            EventHandler handler = SyncObjectUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void UpdateSyncObject()
        {
            OnSyncObjectUpdated(EventArgs.Empty);
        }

        public void SetTimeStamp(Int64 _timestamp)
        {
            this.timestamp = _timestamp;
        }

    }
}
