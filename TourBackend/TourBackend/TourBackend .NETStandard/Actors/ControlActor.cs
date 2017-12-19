﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Threading;
using System.Diagnostics;

namespace TourBackend
{
    /// <summary>
    /// Main Actor in the system. Controls and supervises essential actors [Internal Use] [Modifyable]
    /// </summary>
    public class ControlActor : IActor
    {
        /*Please see the ActorDiagram for information on how the Actors work together*/

        public string id { get; }
        public SyncObject sync;
        public CameraFeedSyncObject video;

        public PID self;
        #region ManagedPIDs
        /* These PIDs are started, managed and maintained by the ControlActor*/
        public PID recognitionManager;
        public PID syncActor;
        public PID cameraFeedSyncActor;
        #endregion

        /*IDToCodeObject is the dictionary containing the information which CodeObject has which ID*/
        public Dictionary<int, CodeObject> IDToCodeObject;

        /// <summary>
        /// Starts the essential Actors [Internal Use]
        /// </summary>
        public void Start()
        {
            var syncprops = Actor.FromProducer(() => new SyncActor("syncactor", sync));
            this.syncActor = Actor.Spawn(syncprops);

            var recogprops = Actor.FromProducer(() => new RecognitionManager("recognitionmanager", IDToCodeObject));
            this.recognitionManager = Actor.Spawn(recogprops);

            var cameraFeedSyncprops = Actor.FromProducer(() => new CameraFeedActor("camerafeedactor", video, self));
            this.cameraFeedSyncActor = Actor.Spawn(cameraFeedSyncprops);
        }

        /// <summary>
        /// Constructs the ControlActor. Use for testing/operation. [Internal Use]
        /// </summary>
        /// <param name="_id">Name the ControlActor should have, e.g. CtrlActor</param>
        /// <param name="_sync">User provided SyncObject</param>
        /// <param name="_video">User provided CameraFeedSyncObject</param>
        /// <param name="_dict">User defined Markers</param>
        public ControlActor(string _id, SyncObject _sync, CameraFeedSyncObject _video, Dictionary<int, CodeObject> _dict)
        {
            id = _id;
            sync = _sync;
            video = _video;
            IDToCodeObject = _dict;
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

            var recogprops = Actor.FromProducer(() => new RecognitionManager("recognitionmanager", IDToCodeObject));
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

        /// <summary>
        /// Behaviour of ControlActor. [Internal Use] [Modifyable]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                /*StartFramework means, that the Framework has been constructed & is being initialized*/
                case StartFramework s:
                    self = s.ctrl;
                    this.Start(); // Start the essential actors RecognitionManager, SyncActor & CameraFeedSyncManager
                    context.Sender.Tell(new RespondStartFramework(syncActor, recognitionManager, cameraFeedSyncActor));
                    Debug.WriteLine("ControlActor has been started");
                    break;

                /*This message should arrive everytime a new frame has been deposited on the CameraFeedSyncObject*/
                case NewFrameArrived n:
                    recognitionManager.Request(new NewFrameArrived(n.id, n.frame), self);
                    Debug.WriteLine("ControlActor says: Frame arrived");
                    break;

                /*This message is sent by the RecognitionManager, telling the ControlActor that it is done evaluatng the current frame
                 The Control Actor then retrieves all recognized objects via RequestAllVirtualObjects*/

                case RespondNewFrameArrived r:
                    recognitionManager.Request(new RequestAllVirtualObjects(r.messageID, TimeSpan.FromMilliseconds(25)), self);
                    Debug.WriteLine("ControlActor says: RecognitionManager has evaulated Frame");
                    break;

                /*This message is sent by the RecognitionManager, telling the ControlActor the currently active Markers
                 Active is defined as having been recognized in the latest frame.
                 The ControlActor then tells the SyncActor to make the data available to the user.*/
                case RespondRequestAllVirtualObjects r:
                    syncActor.Request(new WriteCurrentTourState(r.messageID, r.newCodeObjectIDToCodeObject), self);
                    Debug.WriteLine("ControlActor says: Requesting write of Current Tour State");
                    break;
            }
            return Actor.Done;
        }

    }
}
