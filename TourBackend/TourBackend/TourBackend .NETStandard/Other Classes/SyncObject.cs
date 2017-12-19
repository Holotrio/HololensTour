using System;
using System.Collections.Generic;
using System.Text;

namespace TourBackend
{
    public class SyncObject
    {
        public Int64 timestamp { get; private set; }

        public object thisLock = new Object();

        public string objectid { get; private set; }
        public Dictionary<int, CodeObject> dict { get; private set; }

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

        public void UpdateSyncObject(Int64 _timestamp, Dictionary<int, CodeObject> _dict)
        {
            lock (thisLock)
            {
                timestamp = _timestamp;
                dict = _dict;
                OnSyncObjectUpdated(EventArgs.Empty);
            }
        }

        public void SetTimeStamp(Int64 _timestamp)
        {
            this.timestamp = _timestamp;
        }

    }
}
