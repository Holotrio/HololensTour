using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proto;
using System.Threading;
using System.Threading.Tasks;

namespace TourBackend
{
    [TestClass]
    public class CodeObjectActor_UnitTest
    {
        [TestMethod]
        public void UpdateCodeObjectActor_must_be_constructed_correctly()
        {
            var update = new UpdateCodeObjectActor("m1", 1, new[] { 1f, 2f, 3f }, new[] { 7f, 15f, 28f });
            Assert.AreEqual(update.messageid, "m1");
            Assert.AreEqual(update.objectid, 1);
            CollectionAssert.AreEqual(update.position, new[] { 1f, 2f, 3f });
            CollectionAssert.AreEqual(update.rotation, new[] { 7f, 15f, 28f });
        }

        [TestMethod]
        public void RequestCodeObject_must_be_constructed_correctly()
        {

            var pid = new PID();

            var request = new RequestCodeObject("message1");
            Assert.AreEqual(request.messageid, "message1");
        }

        [TestMethod]
        public async Task CodeObjectActor_must_respond_to_RequestCodeObject()
        {
            var propsCOActor = Actor.FromProducer(() => new CodeObjectActor(2));
            var pidCOActor = Actor.Spawn(propsCOActor);

            pidCOActor.Tell(new UpdateCodeObjectActor("q", 2, new[] {1f,23f }, new[] { 4f,15f}));
            var reply = await pidCOActor.RequestAsync<RespondCodeObject>(new RequestCodeObject("Hello"), TimeSpan.FromSeconds(1));

            Assert.AreEqual(reply.messageid, "Hello");
            Assert.AreEqual(reply.codeobject.isActive, true);
            CollectionAssert.AreEqual(reply.codeobject.position, new[] { 1f, 23f });
            CollectionAssert.AreEqual(reply.codeobject.rotation, new[] { 4f, 15f });
        }

        [TestMethod]
        public async Task CodeObject_must_be_activatable() {
            var propsCOActor = Actor.FromProducer(() => new CodeObjectActor(3, false));
            var pidCOActor = Actor.Spawn(propsCOActor);

            var reply0 = await pidCOActor.RequestAsync<RespondCodeObject>(new RequestCodeObject("Hello"), TimeSpan.FromSeconds(5));
            Assert.AreEqual(reply0.codeobject.isActive, false);

            var reply1 = await pidCOActor.RequestAsync<RespondSetActiveVirtualObject>(new SetActiveVirtualObject("Excelsior!", 3), TimeSpan.FromSeconds(1));
            Assert.AreEqual(reply1.messageID, "Excelsior!");
            Assert.AreEqual(reply1.nowActiveVirtualObjectID, 3);

            var reply2 = await pidCOActor.RequestAsync<RespondCodeObject>(new RequestCodeObject("Hello"), TimeSpan.FromSeconds(1));
            Assert.AreEqual(reply2.codeobject.isActive, true);
        }

        public async Task CodeObject_must_be_deactivatable()
        {
            var propsCOActor = Actor.FromProducer(() => new CodeObjectActor(4));
            var pidCOActor = Actor.Spawn(propsCOActor);

            var reply0 = await pidCOActor.RequestAsync<RespondCodeObject>(new RequestCodeObject("Hello"), TimeSpan.FromSeconds(5));
            Assert.AreEqual(reply0.codeobject.isActive, true);

            var reply1 = await pidCOActor.RequestAsync<RespondSetInActiveVirtualObject>(new SetInActiveVirtualObject("Excelsior!", 4), TimeSpan.FromSeconds(1));
            Assert.AreEqual(reply1.messageID, "Excelsior!");
            Assert.AreEqual(reply1.nowInActiveVirtualObjectID, 4);

            var reply2 = await pidCOActor.RequestAsync<RespondCodeObject>(new RequestCodeObject("Hello"), TimeSpan.FromSeconds(1));
            Assert.AreEqual(reply2.codeobject.isActive, false);
        }

    }
}
