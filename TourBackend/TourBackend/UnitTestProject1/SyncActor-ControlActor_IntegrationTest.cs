using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using Proto;

namespace TourBackend
{
    class SyncActor_ControlActor_IntegrationTest
    {
        [TestMethod]
        public async Task ControlActor_must_correctly_initialize_SyncActor_which_works()
        {
            var syncobj = new SyncObject("sync1", new Dictionary<int, CodeObject>());
            var debugPID = new PID();
            // There is a constructor for debugging which allows to view the PID of the chosen Actor
            // the int argument of the constructor is interpreted as the actor which is to be linked to debugPID
            // Key:
            // 1: RecognitionManager
            // 2: SyncActor

            var propsctrl = Actor.FromProducer(() => new ControlActor("ctrl", syncobj, null, ref debugPID, 2));
            var pidctrl = Actor.Spawn(propsctrl);

            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject(7, new[] { 1f, 2f, 3f }, new[] { 4f, 5f, 6f });
            var cd2 = new CodeObject(9, new[] { 4f, 7f, 8f }, new[] { 1f, 19f, 3f }); // Just build two "random" CodeObjects

            dict.Add(7, cd1);
            dict.Add(9, cd2);

            Assert.AreEqual(dict[7], cd1); // Check that the dict contains the right CodeObjects
            Assert.AreEqual(dict[9], cd2);

            var reply = await debugPID.RequestAsync<RespondWriteCurrentTourState>(new WriteCurrentTourState("Hello", dict), TimeSpan.FromSeconds(1));
            // Note: in any context it does not matter for the behaviour
            // of the actor which receives a message where said message was sent from.
            // The sender only determines where the response is sent to.


            debugPID.Stop();
            pidctrl.Stop(); // Cleanup, just to be safe.

            CollectionAssert.AreEqual((System.Collections.ICollection)syncobj.dict, (System.Collections.ICollection)dict);
        }
    }
}
