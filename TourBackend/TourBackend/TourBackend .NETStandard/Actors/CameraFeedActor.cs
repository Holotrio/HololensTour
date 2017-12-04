using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;

namespace TourBackend
{
    public class CameraFeedActor : IActor
    {

        public string id { get; }
        public CameraFeedSyncObject sync;
        public PID ctrlActor;
        public Bitmap latestBitmap;
        public Int64 latestTimestamp;

        public CameraFeedActor(string _id, CameraFeedSyncObject _sync, PID _ctrlActor)
        {
            ctrlActor = _ctrlActor;
            id = _id;
            sync = _sync;
            sync.FrameUpdated += OnFrameUpdated;
        }

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
            }
            return Actor.Done;
        }

        /*This function is enacted upon receiving an update from the SyncObject. 
         It copies the data from the object and notifies the ControlActor that 
         new data is available.*/

        protected void OnFrameUpdated(object Sender, EventArgs e)
        {
            if (true) // Condition here is to be defined... might make sense to only process every second frame or so
            {
                lock (sync.thisLock)
                {
                    latestBitmap = sync.bitmap;
                    latestTimestamp = sync.timestamp;
                }
                ctrlActor.Tell(new NewFrameArrived(latestTimestamp.ToString(), latestBitmap));
                Console.WriteLine("CameraFeedSyncObject says: New Frame Arrived");
            }
        }

    }
}
