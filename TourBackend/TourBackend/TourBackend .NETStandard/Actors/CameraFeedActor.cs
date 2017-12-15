using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;
using Emgu.CV;
using System.Diagnostics;

namespace TourBackend
{
    /// <summary>
    /// Essential actor in the system. Manages the interaction with input data (i.e.: frames) and notifies the ControlActor.
    /// </summary>
    public class CameraFeedActor : IActor
    {

        public string id { get; private set; }
        private CameraFeedSyncObject sync;
        private PID ctrlActor;
        private Mat latestframe;
        private Int64 latestTimestamp;

        /// <summary>
        /// Started by ControlActor. [Internal Use]
        /// </summary>
        /// <param name="_id">Name to be given, e.g.: CamFeedActor</param>
        /// <param name="_sync">User provided SyncObject</param>
        /// <param name="_ctrlActor">PID of the ControlActor which started this actor</param>
        public CameraFeedActor(string _id, CameraFeedSyncObject _sync, PID _ctrlActor)
        {
            ctrlActor = _ctrlActor;
            id = _id;
            sync = _sync;
            sync.FrameUpdated += OnFrameUpdated;
        }

        /// <summary>
        /// Behaviour of the actor. Might want to expand upon this. [Internal Use] [Modifyable]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ReceiveAsync(IContext context)
        {
            return Actor.Done;
        }

        /// <summary>
        /// This function is enacted upon receiving an update from the SyncObject. It copies the data from the object and notifies the ControlActor that new data is available. [Internal Use]
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        protected void OnFrameUpdated(object Sender, EventArgs e)
        {
            if (true) // Condition here is to be defined... might make sense to only process every second frame or so
            {
                latestframe = sync.frame;
                latestTimestamp = sync.timestamp;
                Debug.WriteLine("CamSync: " + latestframe.Width);
                ctrlActor.Tell(new NewFrameArrived(latestTimestamp.ToString(), latestframe.Clone()));
                Debug.WriteLine("CameraFeedSyncObject says: New Frame Arrived");
            }
        }

    }
}
