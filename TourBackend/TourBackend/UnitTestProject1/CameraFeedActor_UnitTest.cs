using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Proto;
using System.Threading;
using System.IO;
using System.Reflection;
using Emgu.CV;

namespace TourBackend
{
    [TestClass]
    public class CameraFeedActor_UnitTest
    {

        [TestMethod]
        public void Constructor_CameraFeedSyncObject_Test()
        {
            var test = new CameraFeedSyncObject("new");
            Assert.AreEqual(test.id, "new");

        }
        [TestMethod]
        public void CameraFeedSyncObject_must_raise_event_when_UpdateCameraFeedSyncObject_is_called()
        {
            CameraFeedSyncObject test = new CameraFeedSyncObject("new");

            bool eventreceived = false;

            test.FrameUpdated += delegate (object sender, EventArgs e)
                {
                    eventreceived = true;
                };

            test.UpdateCameraFeedSyncObject(1, new Mat());

            Assert.AreEqual(eventreceived, true);
        }

        [TestMethod]
        public void NewFrameArrived_must_be_correctly_constructed()
        {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1.bmp");

            var testframe = new Mat(path);

            var newframe = new NewFrameArrived("id1", testframe);

            Assert.AreEqual("id1", newframe.id);

        }

        /// <summary>Test if CameraFeedSyncObject fires an event FramUpdated and CameraFeedActor listens to it and sends NewFrameArrived to ctrlpid</summary>
        [TestMethod]
        public void CameraFeedActor_needs_to_get_update_from_CameraFeedSyncObject_when_using_local_frames()
        {
            CameraFeedSyncObject test = new CameraFeedSyncObject("new");
            object msg = new Object();

            var propstest = Actor.FromProducer(() => new TestActor(ref msg));
            var pidtest = Actor.Spawn(propstest);

            var syncobj = new SyncObject("sync1", new Dictionary<int, CodeObject>());
            var syncobj2 = new CameraFeedSyncObject("sync2");

            var propsctrl = Actor.FromProducer(() => new ControlActor("ctrl", syncobj, null, new Dictionary<int, CodeObject>()));
            var pidctrl = Actor.Spawn(propsctrl);

            // Statt der PID des ControlActor wird die des TestActors gegeben um die gesendete Nachricht abzufangen
            var propsSyncActor1 = Actor.FromProducer(() => new CameraFeedActor("CameraFeedActor", syncobj2, pidtest));
            var pidSyncActor1 = Actor.Spawn(propsSyncActor1);

            // Creates a testframe from local frames
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1.bmp");
            var testframe = new Mat(path);

            test.UpdateCameraFeedSyncObject(110100010, testframe);

            if (msg.GetType() == typeof(NewFrameArrived))
            {
                Assert.AreEqual(((NewFrameArrived)msg).id, "110100010");
            }
        }

    }
}
