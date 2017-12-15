using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proto;
using System.Threading;

namespace TourBackend
{
    [TestClass]
    public class ControlActor_UnitTest
    {
        [TestMethod]
        public void ControlActor_must_correctly_initialize_SyncActor()
        {
            var syncobj = new SyncObject("sync1", new Dictionary<int, CodeObject>());
            var debugPID = new PID();
            // There is a constructor for debugging which allows to view the PID of the chosen Actor
            // the int argument of the constructor is interpreted as the actor which is to be linked to debugPID
            // Key:
            // 1: RecognitionManager
            // 2: SyncActor

            var pidID1 = debugPID.Id;

            var propsctrl = Actor.FromProducer(() => new ControlActor("ctrl", syncobj, null, ref debugPID, 2));
            var pidctrl = Actor.Spawn(propsctrl);
            pidctrl.Tell(new StartFramework(pidctrl));

            var pidID2 = debugPID.Id;

            debugPID.Stop();
            pidctrl.Stop();

            Assert.AreNotEqual(pidID1, pidID2);
        }

        [TestMethod]
        public void ControlActor_must_correctly_initialize_RecognitionManager()
        {
            var syncobj = new SyncObject("sync1", new Dictionary<int, CodeObject>());
            var debugPID = new PID();
            // There is a constructor for debugging which allows to view the PID of the chosen Actor
            // the int argument of the constructor is interpreted as the actor which is to be linked to debugPID
            // Key:
            // 1: RecognitionManager
            // 2: SyncActor

            var pidID1 = debugPID.Id;

            var propsctrl = Actor.FromProducer(() => new ControlActor("ctrl", syncobj, null, ref debugPID, 1));
            var pidctrl = Actor.Spawn(propsctrl);

            var pidID2 = debugPID.Id;

            debugPID.Stop();
            pidctrl.Stop();

            Assert.AreNotEqual(pidID1, pidID2);
            Assert.AreNotEqual(pidID2, null);
        }


        [TestMethod]
        public void ControlActor_must_be_able_to_get_initialized()
        {
            var syncobj = new SyncObject("sync1", new Dictionary<int, CodeObject>());
            var camobj = new CameraFeedSyncObject("test");
            var debugPID = new PID();

            CodeObject[] codeobjs = new CodeObject[1];
            codeobjs.SetValue(new CodeObject(),0);

            // There is a constructor for debugging which allows to view the PID of the chosen Actor
            // the int argument of the constructor is interpreted as the actor which is to be linked to debugPID
            // Key:
            // 1: RecognitionManager
            // 2: SyncActor

            FrameWork fw = new FrameWork(syncobj, camobj, codeobjs);
            fw.Initialize();
        }

    }
}
