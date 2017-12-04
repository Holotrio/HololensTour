using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Threading;

namespace TourBackend
{

    public class FrameWork {
        public CodeObject[] markers;

        public Dictionary<int, CodeObject> idToCodeObject;

        private PID pidctrl;
        private SyncObject syncobj;
        private CameraFeedSyncObject video;


        public FrameWork(SyncObject _syncobj, CameraFeedSyncObject _video, CodeObject[] _markers) {
            syncobj = _syncobj;
            video = _video;
            markers = _markers;
        }

        public void Initialize()
        {
            var propsctrl = Actor.FromProducer(() => new ControlActor("ctrl", syncobj, video, idToCodeObject));
            pidctrl = Actor.Spawn(propsctrl);

            pidctrl.RequestAsync<RespondStartFramework>(new StartFramework(pidctrl), TimeSpan.FromSeconds(1));
        }

        public PID GetPID() {
            return this.pidctrl;
        }

    }

    public class StartFramework {
        public PID ctrl;
        public StartFramework(PID _ctrl) {
            ctrl = _ctrl;
        }
    }

    public class RespondStartFramework {

        public PID syncactor;
        public PID recognitionmanager;
        public PID camerafeedactor;

        public RespondStartFramework(){ }
        public RespondStartFramework(PID _syncactor, PID _recognitionmanager, PID _camerafeedactor) {
            syncactor = _syncactor;
            recognitionmanager = _recognitionmanager;
            camerafeedactor = _camerafeedactor;
        }
    }

    // Attention: This Actor is subject to change!
    // Neither the constructors nor any other function is guaranteed to stay unchanged!

    public class ControlActor : IActor
    {
        public string id { get; }
        public SyncObject sync;
        public CameraFeedSyncObject video; // This field will *certainly* change
        public PID recognitionManager;
        public PID syncActor;
        public PID cameraFeedSyncActor;
        public PID self;
        public Dictionary<int, CodeObject> dict;
        // More managed PIDs are to follow

        public void Start() {
            var syncprops = Actor.FromProducer(() => new SyncActor("syncactor", sync));
            this.syncActor = Actor.Spawn(syncprops);

            var recogprops = Actor.FromProducer(() => new RecognitionManager("recognitionmanager", dict));
            this.recognitionManager = Actor.Spawn(recogprops);

            var cameraFeedSyncprops = Actor.FromProducer(() => new CameraFeedActor("camerafeedactor", video, self));
            this.recognitionManager = Actor.Spawn(cameraFeedSyncprops);
        }

        public ControlActor(string _id, SyncObject _sync, CameraFeedSyncObject _video, Dictionary<int, CodeObject> _dict)
        {
            id = _id;
            sync = _sync;
            video = _video;
            dict = _dict;
        }

        // This constructor is only meant for debugging 
        // It is functionally identically to the usual constructor except that it 
        // links the reference _debugPID to either of the newly created Actors, chosen according to the int argument,
        // as this allows for more convenient testing.
        // It also doesn't spawn the CameraFeedActor
        // Key for the int argument:
        // 1: RecognitionManager
        // 2: SyncActor

        public ControlActor(string _id, SyncObject _sync, CameraFeedSyncObject _video, ref PID _debugPID, int debug)
        {
            id = _id;
            sync = _sync;
            video = _video;

            var syncprops = Actor.FromProducer(() => new SyncActor("syncactor", sync));
            this.syncActor = Actor.Spawn(syncprops);

            var recogprops = Actor.FromProducer(() => new RecognitionManager("recognitionmanager", dict));
            this.recognitionManager = Actor.Spawn(recogprops);

            switch (debug)
            {
                case 1:
                    _debugPID = recognitionManager;
                    break;
                case 2:
                    _debugPID = syncActor;
                    break;
                default:
                    break;
            }
        }



        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                case StartFramework s:
                    self = s.ctrl; // Thou shallst know thyself
                    this.Start(); // Start the sub actors Recognition Manager, Sync Actor & Camera Feed Sync Manager
                    context.Sender.Tell(new RespondStartFramework(syncActor, recognitionManager, cameraFeedSyncActor));
                    break;
                case NewFrameArrived n:
                    recognitionManager.Tell(n);
                    break;
                case RespondNewFrameArrived r:
                    recognitionManager.Request(new RequestAllVirtualObjects(r.messageID,TimeSpan.FromMilliseconds(25)), self);
                break;
                case RespondRequestAllVirtualObjects r:
                    syncActor.Request(new WriteCurrentTourState(r.messageID, r.newCodeObjectIDToCodeObject), self);
                    break;
            }
            return Actor.Done;
        }

    }
}
