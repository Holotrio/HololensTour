using Proto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TourBackend
{
    /// <summary>
    /// Root initilization class of the whole system.
    /// It's constructor is called with all information that is needed for the system to work.
    /// </summary>
    public class FrameWork
    {
        /* markers is the array which is given to the framework by the constructor.
            It contains the IDs of the markers which are to be recognized */
        public CodeObject[] markers;

        public Dictionary<int, CodeObject> idToCodeObject;

        /* pidctrl is the PID of the control actor itself*/
        private PID pidctrl;
        /*syncobj and video are the synchronization objects as defined in SyncActor and
         CameraFeedActor, respectively. They are passed over to the framework by the constructor*/
        private SyncObject syncobj;
        private CameraFeedSyncObject video;

        /// <summary>
        /// Constructs the framework [External Use]
        /// </summary>
        /// <param name="_syncobj">SyncObject as defined in Other_Classes/SyncObject, used for synchronizing output data with user</param>
        /// <param name="_video">CameraFeedSyncObject as defined in Other_Classes/CameraFeedSyncObject used for synchronizing input data with user</param>
        /// <param name="_markers">CodeObjects as defined in Other_Classes/CodeObject used to encapsulate detected markers</param>
        public FrameWork(SyncObject _syncobj, CameraFeedSyncObject _video, CodeObject[] _markers)
        {
            syncobj = _syncobj;
            video = _video;
            markers = _markers;
            this.idToCodeObject = new Dictionary<int, CodeObject>();
            foreach (var entry in markers)
            {
                idToCodeObject.Add(entry.id, entry);
            }
        }


        /*The initialization process makes sure that all essential Actors, which are 
             RecognitionManager, SyncActor & CameraFeedActor are started and that the 
             ControlActor knows its own PID*/
        /// <summary>
        /// Initializes the framework [External Use]
        /// </summary>
        public void Initialize()
        {
            var propsctrl = Actor.FromProducer(() => new ControlActor("ctrl", syncobj, video, idToCodeObject));
            pidctrl = Actor.Spawn(propsctrl);

            pidctrl.RequestAsync<RespondStartFramework>(new StartFramework(pidctrl), TimeSpan.FromSeconds(1));
        }
        /// <summary>
        /// Returns own PID. [Internal Use]
        /// </summary>
        /// <returns></returns>
        private PID GetPID()
        {
            return this.pidctrl;
        }

    }

    /// <summary>
    /// This message is there to let the ControlActor know its own PID. [Internal Use]
    /// </summary>
    public class StartFramework
    {
        public PID ctrl;
        public StartFramework(PID _ctrl)
        {
            ctrl = _ctrl;
        }
    }

    /* The class RespondStartFramework is the answer to StartFramework.
     It verifies that RecognitionManager, SyncActor & CameraFeedActor have been started
     by passing their PIDs. */
    /// <summary>
    /// Answer to StartFramework [Internal Use]
    /// </summary>
    public class RespondStartFramework
    {

        public PID syncactor;
        public PID recognitionmanager;
        public PID camerafeedactor;

        public RespondStartFramework() { }
        public RespondStartFramework(PID _syncactor, PID _recognitionmanager, PID _camerafeedactor)
        {
            syncactor = _syncactor;
            recognitionmanager = _recognitionmanager;
            camerafeedactor = _camerafeedactor;
        }
    }
}
