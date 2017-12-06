using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Proto;
using System.Threading;
using System.IO;
using System.Reflection;

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
        public void CameraFeedSyncObject_must_raise_event_when_UpdateFrame_is_called()
        {
            CameraFeedSyncObject test = new CameraFeedSyncObject("new");

            bool eventreceived = false;

            test.FrameUpdated += delegate (object sender, EventArgs e)
                {
                    eventreceived = true;
                };

            test.UpdateFrame();

            Assert.AreEqual(eventreceived, true);
        }

        [TestMethod]
        public void NewFrameArrived_must_be_correctly_constructed() {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "TestVideo_007.bmp");

            Stream testfile = File.OpenRead(path);

            var testframe = System.Drawing.Bitmap.FromStream(testfile);

            var newframe = new NewFrameArrived("id1", (System.Drawing.Bitmap)testframe);

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

            // instead of the PID of the control actor the PID of the test actor is given to receive the sent message
            var propsSyncActor1 = Actor.FromProducer(() => new CameraFeedActor("CameraFeedActor", syncobj2, pidtest));
            var pidSyncActor1 = Actor.Spawn(propsSyncActor1);

            // Creates a testframe from local bitmaps
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "TestVideo_007.bmp");
            Stream testfile = File.OpenRead(path);
            var testframe = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            lock (test.thisLock)
            {
                test.bitmap = testframe;
                test.timestamp = 110100010;
            }
            // The timestamp is also the message id

            test.UpdateFrame();

            if (msg.GetType() == typeof(NewFrameArrived))
            {
                Assert.AreEqual(((NewFrameArrived)msg).id, "110100010");
            }
        }
    }
}
