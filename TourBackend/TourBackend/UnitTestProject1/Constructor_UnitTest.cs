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
        /* Test all constructors of the commands. First the ones for the nano-case. */
       
        [TestMethod]
        public void SetActiveVirtualObject_Constructor_must_create_object()
        {
            int _toBeActiveVirtualObjectID = 43;
            var testobject3 = new SetActiveVirtualObject("messageID", _toBeActiveVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testobject3.messageID, "messageID");
            Assert.AreEqual(testobject3.toBeActiveVirtualObjectID, _toBeActiveVirtualObjectID);
        }

        [TestMethod]
        public void SetInActiveVirtualObject_Constructor_must_create_object()
        {
            int _toBeInActiveVirtualObjectID = 65;
            var testobject3 = new SetInActiveVirtualObject("messageID", _toBeInActiveVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testobject3.messageID, "messageID");
            Assert.AreEqual(testobject3.toBeInActiveVirtualObjectID, _toBeInActiveVirtualObjectID);
        }

        [TestMethod]
        public void RequestAllVirtualObjects_Constructor_must_create_object()
        {
            // time is here a timespan of the length 0 hours, 0 minutes and 1 seconds
            TimeSpan _timeSpan = new TimeSpan(0, 0, 1);
            var testObject = new RequestAllVirtualObjects("messageID", _timeSpan);
            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.timeSpan, _timeSpan);
        }

        /* Test all constructors of the commands. Second the ones for the not nano-case. */

        [TestMethod]
        public void StartVirtualObject_Constructor_must_create_object()
        {
            int _virtualObjectIDToBeStarted = 78;
            var testObject = new StartVirtualObject("messageID", _virtualObjectIDToBeStarted);
            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.virtualObjectIDToBeStarted, _virtualObjectIDToBeStarted);
        }

        [TestMethod]
        public void StopVirtualObject_Constructor_must_create_object()
        {
            int _virtualObjectIDToBeStopped = 1353;
            var testObject = new StopVirtualObject("messageID", _virtualObjectIDToBeStopped);
            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.virtualObjectIDToBeStopped, _virtualObjectIDToBeStopped);
        }

        [TestMethod]
        public void KillVirtualObject_Constructor_must_create_object()
        {
           int _toBeKilledVirtualObjectID = 917432;
            var testObject = new KillVirtualObject("messageID", _toBeKilledVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.toBeKilledVirtualObjectID, _toBeKilledVirtualObjectID);
        }

        /* Test all constructors of the responds to the commands. First the ones for the nano-case. */

        [TestMethod]
        public void RespondSetActiveVirtualObject_Constructor_must_create_object()
        {
            // test the constructor
            int _nowActiveVirtualObjectID = 183749;
            var testobject2 = new RespondSetActiveVirtualObject("_id", _nowActiveVirtualObjectID);
            Assert.AreEqual(testobject2.messageID, "_id");
            Assert.AreEqual(testobject2.nowActiveVirtualObjectID, _nowActiveVirtualObjectID);
        }

        [TestMethod]
        public void RespondSetInActiveVirtualObject_Constructor_must_create_object()
        {
            int _nowInActiveVirtualObjectID = 13417951;
            var testobject2 = new RespondSetInActiveVirtualObject("_id", _nowInActiveVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testobject2.messageID, "_id");
            Assert.AreEqual(testobject2.nowInActiveVirtualObjectID, _nowInActiveVirtualObjectID);
        }

        [TestMethod]
        public void RespondRequestAllVirtualObjects_Constructor_must_create_object()
        {
            // first create a new pseude Dictionary...
            Dictionary<int, CodeObject> _dict = new Dictionary<int, CodeObject>();
            // insert a key value pair with a codeObjectID and a codeObject which is null...
            _dict.Add(5, null);
            var testobject = new RespondRequestAllVirtualObjects("_id", _dict);
            // test the constructor with the two arguments
            Assert.AreEqual(testobject.messageID, "_id");
            // with CollectionAssert you can test whole Dictionaries...
            CollectionAssert.AreEqual(testobject.newCodeObjectIDToCodeObject, _dict);
        }

        /* Test all constructors of the responds to the commands. Second the ones for the not nano-case. */

        [TestMethod]
        public void RespondStartVirtualObject_Constructor_must_create_object()
        {
            int _nowStartedVirtualObjectID = 252452;
            var testObject = new RespondStartVirtualObject("messageID", _nowStartedVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.nowStartedVirtualObjectID, _nowStartedVirtualObjectID);
        }

        [TestMethod]
        public void RespondStopVirtualObject_Constructor_must_create_object()
        {
            int _nowStoppedVirtualObjectID = 462456354;
            var testObject = new RespondStopVirtualObject("messageID", _nowStoppedVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testObject.messageID, "messageID");
            Assert.AreEqual(testObject.nowStoppedVirtualObjectID, _nowStoppedVirtualObjectID);
        }

        [TestMethod]
        public void RespondKillVirtualObject_Constructor_must_create_object()
        {
            int _nowKilledVirtualObjectID = 666;
            var testobject = new RespondKillVirtualObject("messageID", _nowKilledVirtualObjectID);
            // test the constructor
            Assert.AreEqual(testobject.messageID, "messageID");
            Assert.AreEqual(testobject.nowKilledVirtualObjectID, _nowKilledVirtualObjectID);
        }

        /* Test all constructors of the failures to the commands. First the ones for the nano-case. */

        [TestMethod]
        public void FailedToSetActiveVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "messageID";
            var testObject = new FailedToSetActiveVirtualObject(_messageID);
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToSetInActiveVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "messageID";
            var testObject = new FailedToSetInActiveVirtualObject(_messageID);
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToRequestAllVirtualObjects_Constructor_must_create_object()
        {
            string _messageID = "messageID";
            var testObject = new FailedToRequestAllVirtualObjects(_messageID);
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        /* Test all constructors of the failures to the commands. Second the ones for the not nano-case. */

        [TestMethod]
        public void FailedToStartVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "messageID";
            var testObject = new FailedToStartVirtualObject(_messageID);
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToStopVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "messageID";
            var testObject = new FailedToStopVirtualObject(_messageID);
            Assert.AreEqual(testObject.messageID, _messageID);
        }

        [TestMethod]
        public void FailedToKillVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "messageID";
            var testObject = new FailedToKillVirtualObject(_messageID);
            Assert.AreEqual(testObject.messageID, _messageID); ;
        }

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
            CodeObject testCodeObject2 = new CodeObject(_objectid, _position, _rotation,_isActive);
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

        /* test the constructor for the createNewVirtualObject. */

        [TestMethod]
        public void CreateNewVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "Create1";
            int _codeObjectID = 1;
            double[] _position = { 1d, 2d, 4d };
            double[] _rotation = { 1d, 2.3d, 34d, 0.5d, 2d, 3d, 8.9d, 0.9d, 2.1d };
            bool _isActive = false;
            CodeObject _codeObject = new CodeObject(_codeObjectID, _position, _rotation, _isActive);
            var msg = new CreateNewVirtualObject(_messageID, _codeObjectID, _codeObject);
            Assert.AreEqual(msg.messageID, _messageID);
            Assert.AreEqual(msg.codeObjectID, _codeObjectID);
            Assert.AreEqual(msg.codeObject, _codeObject);
            Assert.AreEqual(msg.codeObject.id, _codeObjectID);
        }

        /* test the constructor for the Respond to createNewVirtualObject. */

        [TestMethod]
        public void RespondCreateNewVirtualObject_Constructor_must_create_object()
        {
            string _messageID = "Create1";
            var msg = new RespondCreateNewVirtualObject(_messageID);
            Assert.AreEqual(msg.messageID, _messageID);
        }

    }
}
