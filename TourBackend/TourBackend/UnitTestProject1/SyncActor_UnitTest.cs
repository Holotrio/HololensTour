using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proto;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace TourBackend
{
    [TestClass]
    public class SyncActor_UnitTest
    {
        [TestMethod]
        public async Task SyncActor_needs_to_be_able_to_write_on_SyncObject()
        {

            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject(90909, new[] { 1d, 2d, 3d }, new[] { 4d, 5d, 6d });
            var cd2 = new CodeObject(890, new[] { 4d, 7d, 8d }, new[] { 1d, 19d, 3d }); // Just build to "random" CodeObjects

            dict.Add(90909, cd1);
            dict.Add(890, cd2);

            Assert.AreEqual(dict[90909], cd1); // Check that the dict contains the right CodeObjects
            Assert.AreEqual(dict[890], cd2);

            var obj = new SyncObject("sync1", new Dictionary<int, CodeObject>());

            CollectionAssert.AreEqual((System.Collections.ICollection)obj.dict, (System.Collections.ICollection)new Dictionary<string, CodeObject>()); // Check that SyncObject has no dict

            var propsSyncActor1 = Actor.FromProducer(() => new SyncActor("SyncActor1", obj));
            var pidSyncActor1 = Actor.Spawn(propsSyncActor1);

            var reply = await pidSyncActor1.RequestAsync<RespondWriteCurrentTourState>(new WriteCurrentTourState("dab", dict), TimeSpan.FromSeconds(1));

            pidSyncActor1.Stop();

            Assert.IsTrue(obj.dict.ContainsKey(90909));
            Assert.IsTrue(obj.dict.ContainsKey(890));

        }

        [TestMethod]
        public async Task SyncActor_needs_to_write_different_timestamps_for_different_writes()
        {
            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject(5, new[] { 1d, 2d, 3d }, new[] { 4d, 5d, 6d });
            var cd2 = new CodeObject(46, new[] { 4d, 7d, 8d }, new[] { 1d, 19d, 3d }); // Just build two "random" CodeObjects
            var cd3 = new CodeObject(67, new[] { 57d, 34d, 124d }, new[] { 1235d,35d, 757d }); // Just build to "random" CodeObjects

            dict.Add(5, cd1);
            dict.Add(46, cd2);

            Assert.AreEqual(dict[5], cd1); // Check that the dict contains the right CodeObjects
            Assert.AreEqual(dict[46], cd2);

            var obj = new SyncObject("sync1", new Dictionary<int, CodeObject>());

            CollectionAssert.AreEqual((System.Collections.ICollection)obj.dict, (System.Collections.ICollection)new Dictionary<string, CodeObject>()); // Check that SyncObject has no dict

            var propsSyncActor1 = Actor.FromProducer(() => new SyncActor("SyncActor1", obj));
            var pidSyncActor1 = Actor.Spawn(propsSyncActor1);

            var reply = await pidSyncActor1.RequestAsync<RespondWriteCurrentTourState>(new WriteCurrentTourState("dab", dict), TimeSpan.FromSeconds(1));

            var timestamp1 = obj.timestamp;

            Thread.Sleep(1); // Actors might be faster than 1 ms, but 1 ms is maximal precision of stopwatch

            dict.Add(67, cd3);
            var reply2 = await pidSyncActor1.RequestAsync<RespondWriteCurrentTourState>(new WriteCurrentTourState("dab2", dict), TimeSpan.FromSeconds(1));

            var timestamp2 = obj.timestamp;

            pidSyncActor1.Stop();

            Assert.AreNotEqual(timestamp1,timestamp2);

        }
        [TestMethod]
        public async Task SyncActor_needs_to_respond_after_writing_on_SyncObject()
        {

            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject(45, new[] { 1d, 2d, 3d }, new[] { 4d, 5d, 6d });
            var cd2 = new CodeObject(245, new[] { 4d, 7d, 8d }, new[] { 1d, 19d, 3d }); // Just build to "random" CodeObjects

            dict.Add(45, cd1);
            dict.Add(245, cd2);

            Assert.AreEqual(dict[45], cd1); // Check that the dict contains the right CodeObjects
            Assert.AreEqual(dict[245], cd2);

            var obj = new SyncObject("sync1", null);

            CollectionAssert.AreEqual(obj.dict, null); // Check that SyncObject has no dict

            var propsSyncActor1 = Actor.FromProducer(() => new SyncActor("SyncActor1", obj));
            var pidSyncActor1 = Actor.Spawn(propsSyncActor1);

            var reply = await pidSyncActor1.RequestAsync<RespondWriteCurrentTourState>(new WriteCurrentTourState("req1", dict), TimeSpan.FromSeconds(1)); // Expect Answer

            pidSyncActor1.Stop();

            Assert.AreEqual(reply.id, "req1");

        }

    }
}
