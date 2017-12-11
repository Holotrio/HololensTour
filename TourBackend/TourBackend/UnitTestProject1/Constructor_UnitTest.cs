using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proto;
using System.Threading;

namespace TourBackend
{
    [TestClass]
    public class ControlToSyncActorProtocol
    {
        [TestMethod]
        public void SyncObject_must_be_correctly_constructed()
        {

            var dict = new Dictionary<int, CodeObject>();
            var cd1 = new CodeObject();
            var cd2 = new CodeObject(); // Just build two "random" CodeObjects

            dict.Add(1, cd1);
            dict.Add(2, cd2);

            Assert.AreEqual(dict[1], cd1); // Check that the dict contains the right CodeObjects
            Assert.AreEqual(dict[2], cd2);

            var obj = new SyncObject("sync1", dict);

            Assert.AreEqual(obj.dict[1], cd1); // Check that the SyncObject contains the right CodeObjects
            Assert.AreEqual(obj.dict[2], cd2);
            Assert.AreEqual(obj.objectid, "sync1");
        }
    }

    /// <summary>
    /// This class tests all message constructors which are defined in the ControlToRecognitionManagerProtocol.
    /// </summary>
    [TestClass]
    public class ControlToRecognitionManagerProtocol
    {
        /* First test all constructor of the message types, which we need for the nano-case. */

        [TestMethod]
        public void SetActiveVirtualObject_must_be_correctly_constructed()
        {
            int _toBeActiveVirtualObjectID = 43;
            var testobject3 = new SetActiveVirtualObject("messageID", _toBeActiveVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testobject3.messageID, "messageID");
            Assert.AreEqual(testobject3.toBeActiveVirtualObjectID, _toBeActiveVirtualObjectID);
        }

        [TestMethod]
        public void SetInActiveVirtualObject_must_be_correctly_constructed()
        {
            int _toBeInActiveVirtualObjectID = 65;
            var testobject3 = new SetInActiveVirtualObject("messageID", _toBeInActiveVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testobject3.messageID, "messageID");
            Assert.AreEqual(testobject3.toBeInActiveVirtualObjectID, _toBeInActiveVirtualObjectID);
        }

        [TestMethod]
        public void RequestAllVirtualObjects_must_be_correctly_constructed()
        {
            // time is here a timespan of the length 0 hours, 0 minutes and 1 seconds
            TimeSpan _timeSpan = new TimeSpan(0, 0, 1);
            var testObject = new RequestAllVirtualObjects("messageID", _timeSpan);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.timeSpan, _timeSpan);
        }

        [TestMethod]
        public void RespondSetActiveVirtualObject_must_be_correctly_constructed()
        {
            int _nowActiveVirtualObjectID = 183749;
            var testobject2 = new RespondSetActiveVirtualObject("_id", _nowActiveVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testobject2.messageID, "_id");
            Assert.AreEqual(testobject2.nowActiveVirtualObjectID, _nowActiveVirtualObjectID);
        }

        [TestMethod]
        public void RespondSetInActiveVirtualObject_must_be_correctly_constructed()
        {
            int _nowInActiveVirtualObjectID = 13417951;
            var testobject2 = new RespondSetInActiveVirtualObject("_id", _nowInActiveVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testobject2.messageID, "_id");
            Assert.AreEqual(testobject2.nowInActiveVirtualObjectID, _nowInActiveVirtualObjectID);
        }

        [TestMethod]
        public void RespondRequestAllVirtualObjects_must_be_correctly_constructed()
        {
            Dictionary<int, CodeObject> _dict = new Dictionary<int, CodeObject>(5,null); // creates a dictionary and inserts a key value pair with a codeObjectID 5 and a codeObject which is null...
            var testobject = new RespondRequestAllVirtualObjects("_id", _dict);

            // test the constructor
            Assert.AreEqual(testobject.messageID, "_id");
            CollectionAssert.AreEqual(testobject.newCodeObjectIDToCodeObject, _dict);  // with CollectionAssert you can test whole Dictionaries...
        }

        [TestMethod]
        public void FailedToSetActiveVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "messageID";
            var testObject = new FailedToSetActiveVirtualObject(_messageID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToSetInActiveVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "messageID";
            var testObject = new FailedToSetInActiveVirtualObject(_messageID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        /* Secondly test all constructor of the message types, which we do not need for the nano-case. */

        [TestMethod]
        public void StartVirtualObject_must_be_correctly_constructed()
        {
            int _virtualObjectIDToBeStarted = 78;
            var testObject = new StartVirtualObject("messageID", _virtualObjectIDToBeStarted);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.virtualObjectIDToBeStarted, _virtualObjectIDToBeStarted);
        }

        [TestMethod]
        public void StopVirtualObject_must_be_correctly_constructed()
        {
            int _virtualObjectIDToBeStopped = 1353;
            var testObject = new StopVirtualObject("messageID", _virtualObjectIDToBeStopped);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.virtualObjectIDToBeStopped, _virtualObjectIDToBeStopped);
        }

        [TestMethod]
        public void CreateNewVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "Create1";
            int _codeObjectID = 1;
            double[] _position = { 1d, 2d, 4d };
            double[] _rotation = { 1d, 2.3d, 34d, 0.5d, 2d, 3d, 8.9d, 0.9d, 2.1d };
            bool _isActive = false;
            CodeObject _codeObject = new CodeObject(_codeObjectID, _position, _rotation, _isActive);
            var msg = new CreateNewVirtualObject(_messageID, _codeObject);

            // test the constructor
            Assert.AreEqual(msg.messageID, _messageID);
            Assert.AreEqual(msg.codeObjectToBeCreated, _codeObject);
            Assert.AreEqual(msg.codeObjectToBeCreated.id, _codeObjectID);
        }

        [TestMethod]
        public void KillVirtualObject_must_be_correctly_constructed()
        {
            int _toBeKilledVirtualObjectID = 917432;
            var testObject = new KillVirtualObject("messageID", _toBeKilledVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.toBeKilledVirtualObjectID, _toBeKilledVirtualObjectID);
        }

        [TestMethod]
        public void RespondStartVirtualObject_must_be_correctly_constructed()
        {
            int _nowStartedVirtualObjectID = 252452;
            var testObject = new RespondStartVirtualObject("messageID", _nowStartedVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.nowStartedVirtualObjectID, _nowStartedVirtualObjectID);
        }

        [TestMethod]
        public void RespondStopVirtualObject_must_be_correctly_constructed()
        {
            int _nowStoppedVirtualObjectID = 462456354;
            var testObject = new RespondStopVirtualObject("messageID", _nowStoppedVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.nowStoppedVirtualObjectID, _nowStoppedVirtualObjectID);
        }

        [TestMethod]
        public void RespondCreateNewVirtualObject_must_be_correctly_constructed()
        {
            int _createdCodeObjectID = 99;
            var testObject = new RespondCreateNewVirtualObject("messageID", _createdCodeObjectID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.nowCreatedVirtualObjectID, _createdCodeObjectID);
        }

        [TestMethod]
        public void RespondKillVirtualObject_must_be_correctly_constructed()
        {
            int _nowKilledVirtualObjectID = 666;
            var testobject = new RespondKillVirtualObject("messageID", _nowKilledVirtualObjectID);

            // test the constructor
            Assert.AreEqual(testobject.messageID, "messageID");
            Assert.AreEqual(testobject.nowKilledVirtualObjectID, _nowKilledVirtualObjectID);
        }

        [TestMethod]
        public void FailedToStartVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "messageID";
            var testObject = new FailedToStartVirtualObject(_messageID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToStopVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "messageID";
            var testObject = new FailedToStopVirtualObject(_messageID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToCreateNewVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "messageID";
            var testObject = new FailedToCreateNewVirtualObject(_messageID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToKillVirtualObject_must_be_correctly_constructed()
        {
            string _messageID = "messageID";
            var testObject = new FailedToKillVirtualObject(_messageID);

            // test the constructor
            Assert.AreEqual(testObject.messageID, _messageID); ;
        }
    }

    [TestClass]
    public class OtherConstructors
    {
        /* test the constructor for the codeObjects. */

        [TestMethod]
        public void CodeObject_must_create_object()
        {
            bool _isActive = false;
            int _objectid = 35735;
            double[] _position = { 1d, 2d, 3d };
            double[] _rotation = { 2d, 5d, 7d };

            // create two testobjects with the two different constructors
            CodeObject testCodeObject1 = new CodeObject(_objectid, _position, _rotation);
            CodeObject testCodeObject2 = new CodeObject(_objectid, _position, _rotation, _isActive);

            // test the constructor with 4 arguments with default isActive = true;
            Assert.AreEqual(testCodeObject1.id, _objectid);
            Assert.AreEqual(testCodeObject1.position, _position);
            Assert.AreEqual(testCodeObject1.rotation, _rotation);
            Assert.AreEqual(testCodeObject1.isActive, true);

            // test the constructor with 5 arguments
            Assert.AreEqual(testCodeObject2.id, _objectid);
            Assert.AreEqual(testCodeObject2.position, _position);
            Assert.AreEqual(testCodeObject2.rotation, _rotation);
            Assert.AreEqual(testCodeObject2.isActive, _isActive);
        }
    }
}
