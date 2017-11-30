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
            var cd1 = new CodeObject();
            var cd2 = new CodeObject(); // Just build two "random" CodeObjects
            CodeObject[] codeobjs = new CodeObject[2];
            codeobjs.SetValue(cd1,0);
            codeobjs.SetValue(cd2, 1);

            dict.Add(1, cd1);
            dict.Add(2, cd2);

            FrameWork fw = new FrameWork(syncobject, camerafeedsyncobject, codeobjs);
            fw.Initialize();

            // Creates a testframe from local bitmaps
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "TestVideo_000.bmp");
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
            while (stop.ElapsedMilliseconds < 1000 && syncobject.dict != dict) {
                Thread.Sleep(5); // Arbitrary sleep length
            }
            stop.Stop();

            // Fail test if syncobj hasn't been updated
            CollectionAssert.AreEqual(syncobject.dict, dict);
        }

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

    }
}
