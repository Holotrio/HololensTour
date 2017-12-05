using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Proto;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace TourBackend
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void A_single_frame_needs_to_travel_from_CameraFeedSyncObject_To_SyncActor_with_mock_Frames()
        {
            CameraFeedSyncObject camerafeedsyncobject = new CameraFeedSyncObject("new");
            SyncObject syncobject = new SyncObject("sync1", new Dictionary<int, CodeObject>());
            
            //This dict will have to be updated to be true to the frame
            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject(1, new[] { -0.0716567589573924d, 0.000621125026751875d, 0.421481938204676d }, new[] { 0.997540310976068d, -0.00352611230630855d, -0.0700063890639387d, -0.00696372073051795d, -0.99877837158065d, -0.0489210696560579d, -0.0697483660837701d, 0.0492882439807792d, -0.996346242244098d }, true);
            CodeObject[] codeobjs = new CodeObject[1];
            codeobjs.SetValue(cd1,0);
            dict.Add(1, cd1);

            FrameWork fw = new FrameWork(syncobject, camerafeedsyncobject, codeobjs);
            fw.Initialize();

            // Creates a testframe from local bitmaps
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1.bmp");
            Stream testfile = File.OpenRead(path);
            var testframe = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            lock (camerafeedsyncobject.thisLock)
            {
                camerafeedsyncobject.bitmap = testframe;
                camerafeedsyncobject.timestamp = 110100010;
            }
            // The timestamp is also the message id

            camerafeedsyncobject.UpdateFrame();

            // See if the output has been updated within 1 second
            Stopwatch stop = new Stopwatch();
            stop.Start();
            while (stop.ElapsedMilliseconds < 20000 && syncobject.dict.ContainsKey(1) != true) {
                Thread.Sleep(5); // Arbitrary sleep length
            }
            stop.Stop();

            //Console.WriteLine(syncobject.dict[1].id);

            // Fail test if syncobj hasn't been updated
            Assert.IsTrue(syncobject.dict.ContainsKey(1));

            Assert.IsTrue(Math.Abs(syncobject.dict[1].position[0] - dict[1].position[0])<0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].position[1] - dict[1].position[1]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].position[2] - dict[1].position[2]) < 0.01d);

            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[0] - dict[1].rotation[0]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[1] - dict[1].rotation[1]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[2] - dict[1].rotation[2]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[3] - dict[1].rotation[3]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[4] - dict[1].rotation[4]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[5] - dict[1].rotation[5]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[6] - dict[1].rotation[6]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[7] - dict[1].rotation[7]) < 0.01d);
            Assert.IsTrue(Math.Abs(syncobject.dict[1].rotation[8] - dict[1].rotation[8]) < 0.01d);
        }
        /*
        [TestMethod]
        public void Multiple_frames_need_to_travel_from_CameraFeedSyncObject_To_SyncActor_with_mock_Frames()
        {
            CameraFeedSyncObject camerafeedsyncobject = new CameraFeedSyncObject("new");
            SyncObject syncobject = new SyncObject("sync1", new Dictionary<int, CodeObject>());

            //This dict will have to be updated to be true to the frame
            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject();
            var cd2 = new CodeObject(); // Just build two "random" CodeObjects
            CodeObject[] codeobjs = new CodeObject[2];
            codeobjs.SetValue(cd1, 0);
            codeobjs.SetValue(cd2, 1);

            dict.Add(1, cd1);
            dict.Add(2, cd2);

            FrameWork fw = new FrameWork(syncobject, camerafeedsyncobject, codeobjs);
            fw.Initialize();

            for (int i=0;i<5;i++) {

                // Creates a testframe from local bitmaps
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(path, "Resources");
                path = Path.Combine(path, "TestVideo_00"+i+".bmp");
                Stream testfile = File.OpenRead(path);
                var testframe = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

                lock (camerafeedsyncobject.thisLock)
                {
                    camerafeedsyncobject.bitmap = testframe;
                    camerafeedsyncobject.timestamp = 110100010;
                }
                // The timestamp is also the message id

                camerafeedsyncobject.UpdateFrame();

                // See if the output has been updated within 1 second
                Stopwatch stop = new Stopwatch();
                stop.Start();
                while (stop.ElapsedMilliseconds < 1000 && syncobject.dict != dict)
                {
                    Thread.Sleep(5); // Arbitrary sleep length
                }
                stop.Stop();

                // Fail test if syncobj hasn't been updated
                CollectionAssert.AreEqual(syncobject.dict, dict);
            }
        }
        */
    }
}
