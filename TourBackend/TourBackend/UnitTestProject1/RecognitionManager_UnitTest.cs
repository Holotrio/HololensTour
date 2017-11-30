using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proto;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace TourBackend
{
    [TestClass]
    public class RecognititonManager_UnitTest
    {
        /// <summary>
        /// The idea here is to test that if the conrtrolActor asks the recognitionManager
        /// to get all CodeObjects, that are in the current tourState (meaning the setActive bool is true,
        /// the controlActor gets a dictionary back. The dictionary consists of an CodeObjectID as a key and 
        /// the CodeObject itself as a value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Control_Asks_RecognitionManager_RequestAllVirtualObjects()
        // for the RequestAsync method call we need firstly an async keyword in the declaration of the Task
        {
            // first create some objects which should be in the dictionary of the recognition Manager in the 
            // initialize step => all codeObjects should have the isActive false
            // CodeObject 1
            int _codeObjectID1 = 1;
            float[] _position1 = { 1f, 2f, 4f };
            float[] _rotation1 = { 1f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive1 = true;
            CodeObject _codeObject1 = new CodeObject(_codeObjectID1, _position1, _rotation1, _isActive1);
            // CodeObject 2
            int _codeObjectID2 = 2;
            float[] _position2 = { 2f, 2f, 4f };
            float[] _rotation2 = { 2f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive2 = false;
            CodeObject _codeObject2 = new CodeObject(_codeObjectID2, _position2, _rotation2, _isActive2);
            // CodeObject 3, this time we want to have the codeObject to be initialised with the default value
            // true for the isActive 
            int _codeObjectID3 = 3;
            float[] _position3 = { 3f, 2f, 4f };
            float[] _rotation3 = { 3f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            CodeObject _codeObject3 = new CodeObject(_codeObjectID3, _position3, _rotation3);
            // then create the dictionary for initialisation of the recognition manager
            Dictionary<int, CodeObject> _initialDict = new Dictionary<int, CodeObject>();
            _initialDict.Add(_codeObjectID1, _codeObject1);
            _initialDict.Add(_codeObjectID2, _codeObject2);
            _initialDict.Add(_codeObjectID3, _codeObject3);
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", _initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);
            // here we really do now the request from the testControlActor to the recognitionManager and we store
            // the respond to the request in response where this must be a object of the class RespondRequestAllVirtualObjects
            // which contains of a dictionary and a messageID to know to which Request the Respond was
            var _msgRequestAll = new RequestAllVirtualObjects("RequestAll1", TimeSpan.FromSeconds(1));
            var response = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll, TimeSpan.FromSeconds(1));
            // here we actually test if the Call "RequestAllVirtualObjects" can what we intended
            // first we check if the response have the same messageID as the request had
            Assert.AreEqual(response.messageID, "RequestAll1");
            // then we check if the dictionaries are the same. First define the expected Dictionary
            Dictionary<int, CodeObject> expectedDictionary = new Dictionary<int, CodeObject>();
            expectedDictionary.Add(_codeObject1.id, _codeObject1); // since his boolean isActive is true
            expectedDictionary.Add(_codeObject3.id, _codeObject3); // since his boolean isActive is true
            // _codeObject2 is not expected since his isActive is false
            CollectionAssert.AreEqual(response.newCodeObjectIDToCodeObject, expectedDictionary);
        }
        /// <summary>
        /// The idea here is that we send a message to the Recognition Manager to SetActive a specific 
        /// VirtualObject. The Recognition Manager should response with the messageID of the SetActive Command
        /// and the VirtualObjectID of the now active VirtualObject
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Control_Asks_RecognitionManager_To_SetActiveVirtualObject()
        {
            // first create some objects which should be in the dictionary of the recognition Manager in the 
            // initialize step => all codeObjects should have the isActive false
            // CodeObject 1
            int _codeObjectID1 = 1;
            float[] _position1 = { 1f, 2f, 4f };
            float[] _rotation1 = { 1f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive1 = false;
            CodeObject _codeObject1 = new CodeObject(_codeObjectID1, _position1, _rotation1, _isActive1);
            // CodeObject 2
            int _codeObjectID2 = 2;
            float[] _position2 = { 2f, 2f, 4f };
            float[] _rotation2 = { 2f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive2 = false;
            CodeObject _codeObject2 = new CodeObject(_codeObjectID2, _position2, _rotation2, _isActive2);
            // CodeObject 3, this time we want to have the codeObject to be initialised with the default value
            // true for the isActive 
            int _codeObjectID3 = 3;
            float[] _position3 = { 3f, 2f, 4f };
            float[] _rotation3 = { 3f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            CodeObject _codeObject3 = new CodeObject(_codeObjectID3, _position3, _rotation3);
            // then create the dictionary for initialisation of the recognition manager
            Dictionary<int, CodeObject> _initialDict = new Dictionary<int, CodeObject>();
            _initialDict.Add(_codeObjectID1, _codeObject1);
            _initialDict.Add(_codeObjectID2, _codeObject2);
            _initialDict.Add(_codeObjectID3, _codeObject3);
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", _initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);
            // now send the message that codeObject 1 should be set active
            var _msgSetActiveCO1 = new SetActiveVirtualObject("SetActiveCO1", 1);
            var responseToSetActive = await _pidTestRecognitionManager.RequestAsync<RespondSetActiveVirtualObject>(_msgSetActiveCO1, TimeSpan.FromSeconds(1));
            // now we should get the right messageID and the right CodeObject ID back
            Assert.AreEqual(responseToSetActive.messageID, "SetActiveCO1");
            Assert.AreEqual(responseToSetActive.nowActiveVirtualObjectID, 1);
            // and then test if the data is at the current state in the dictionary
            // first make an request all call to the recognitionManager to get the whole Dictionary back
            var _msgRequestAll = new RequestAllVirtualObjects("RequestAll1", TimeSpan.FromSeconds(1));
            var responseToRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll);
            // and then test if the right thing is in the dictionary
            Assert.AreEqual(responseToRequestAll.messageID, "RequestAll1");
            Assert.AreEqual(responseToRequestAll.newCodeObjectIDToCodeObject.ContainsKey(1), true);
            Assert.AreEqual(responseToRequestAll.newCodeObjectIDToCodeObject[1].isActive, true);
            // test if we wanna set Active a CodeObject which does not exist, then a failed to message should come back
            var _msgSetActiveCO7 = new SetActiveVirtualObject("SetActiveCO7", 7);
            var responseToSetActive7 = await _pidTestRecognitionManager.RequestAsync<FailedToSetActiveVirtualObject>(_msgSetActiveCO7, TimeSpan.FromSeconds(1));
            Assert.AreEqual(responseToSetActive7.messageID, "SetActiveCO7");
        }

        /// <summary>
        /// The idea here is that we send a message to the Recognition Manager to SetInActive a specific 
        /// VirtualObject. The Recognition Manager should response with the messageID of the SetInActive Command
        /// and the VirtualObjectID of the now inactive VirtualObject
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Control_Asks_RecognitionManager_To_SetInActiveVirtualObject()
        {
            // first create some objects which should be in the dictionary of the recognition Manager in the 
            // initialize step => all codeObjects should have the isActive false
            // CodeObject 1
            int _codeObjectID1 = 1;
            float[] _position1 = { 1f, 2f, 4f };
            float[] _rotation1 = { 1f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive1 = true;
            CodeObject _codeObject1 = new CodeObject(_codeObjectID1, _position1, _rotation1, _isActive1);
            // CodeObject 2
            int _codeObjectID2 = 2;
            float[] _position2 = { 2f, 2f, 4f };
            float[] _rotation2 = { 2f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive2 = false;
            CodeObject _codeObject2 = new CodeObject(_codeObjectID2, _position2, _rotation2, _isActive2);
            // CodeObject 3, this time we want to have the codeObject to be initialised with the default value
            // true for the isActive 
            int _codeObjectID3 = 3;
            float[] _position3 = { 3f, 2f, 4f };
            float[] _rotation3 = { 3f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            CodeObject _codeObject3 = new CodeObject(_codeObjectID3, _position3, _rotation3);
            // then create the dictionary for initialisation of the recognition manager
            Dictionary<int, CodeObject> _initialDict = new Dictionary<int, CodeObject>();
            _initialDict.Add(_codeObjectID1, _codeObject1);
            _initialDict.Add(_codeObjectID2, _codeObject2);
            _initialDict.Add(_codeObjectID3, _codeObject3);
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", _initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);
            // now send the message that codeObject 1 should be set inActive
            var _msgSetInActiveCO1 = new SetInActiveVirtualObject("SetInActiveCO1", 1);
            var responseToSetInActive = await _pidTestRecognitionManager.RequestAsync<RespondSetInActiveVirtualObject>(_msgSetInActiveCO1, TimeSpan.FromSeconds(1));
            // now we should get the right messageID and the right CodeObject ID back
            Assert.AreEqual(responseToSetInActive.messageID, "SetInActiveCO1");
            Assert.AreEqual(responseToSetInActive.nowInActiveVirtualObjectID, 1);
            // and then test if the data is at the current state in the dictionary
            // first make an request all call to the recognitionManager to get the whole Dictionary back
            var _msgRequestAll = new RequestAllVirtualObjects("RequestAll1", TimeSpan.FromSeconds(1));
            var responseToRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll);
            // and then test if the right thing is in the dictionary, meaning that the isActive of the 
            // codeObject1 has changed from true to false and that is if the CodeObject is no longer in that respondDictionary
            Assert.AreEqual(responseToRequestAll.messageID, "RequestAll1");
            Assert.AreEqual(responseToRequestAll.newCodeObjectIDToCodeObject.ContainsKey(1), false);
            // test if we wanna set InActive a CodeObject which does not exist, then a failed to message should come back
            var _msgSetInActiveCO7 = new SetInActiveVirtualObject("SetInActiveCO7", 7);
            var responseToSetInActive7 = await _pidTestRecognitionManager.RequestAsync<FailedToSetInActiveVirtualObject>(_msgSetInActiveCO7, TimeSpan.FromSeconds(1));
            Assert.AreEqual(responseToSetInActive7.messageID, "SetInActiveCO7");
        }
        /// <summary>
        /// here the idea is that if the controlActor gets a message from the cameraFeedActor that a new 
        /// frame arrived, he should forward this message to the recognitionManager. The communication between
        /// the controlActor and the recognitionManager is here ONLY tested and not the whole flow from the camerafeedActor.
        /// Therefore if the message NewFrameArrived comes to the Recognition Manager than he should start to work
        /// with this Frame and if he is finished he should answer with the message RespondNewframeArrived.
        /// THIS TEST ONLY TESTS THE RESPOND MESSAGE IS CORRECTLY BUT NOT THAT THE EVALUATION OF THE FRAME WAS RIGHT!!!
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Control_forwards_message_NewFrameArrived_to_the_recognitionManager()
        {
            // first create some objects which should be in the dictionary of the recognition Manager in the 
            // initialize step => all codeObjects should have the isActive false
            // CodeObject 1
            int _codeObjectID1 = 1;
            float[] _position1 = { 1f, 2f, 4f };
            float[] _rotation1 = { 1f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive1 = true;
            CodeObject _codeObject1 = new CodeObject(_codeObjectID1, _position1, _rotation1, _isActive1);
            // CodeObject 2
            int _codeObjectID2 = 2;
            float[] _position2 = { 2f, 2f, 4f };
            float[] _rotation2 = { 2f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive2 = false;
            CodeObject _codeObject2 = new CodeObject(_codeObjectID2, _position2, _rotation2, _isActive2);
            // CodeObject 3, this time we want to have the codeObject to be initialised with the default value
            // true for the isActive 
            int _codeObjectID3 = 3;
            float[] _position3 = { 3f, 2f, 4f };
            float[] _rotation3 = { 3f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            CodeObject _codeObject3 = new CodeObject(_codeObjectID3, _position3, _rotation3);
            // then create the dictionary for initialisation of the recognition manager
            Dictionary<int, CodeObject> _initialDict = new Dictionary<int, CodeObject>();
            _initialDict.Add(_codeObjectID1, _codeObject1);
            _initialDict.Add(_codeObjectID2, _codeObject2);
            _initialDict.Add(_codeObjectID3, _codeObject3);
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", _initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);
            // create a new object of the message type NewFrameArrived. for this we need firstly a new messageID
            string _messageID = "NewFrameArrived1";
            // and secondly a SoftwareBitmap and to get a testbitmap we need to follow these steps...they are
            // copied from the CameraFeedActor_UnitTest.cs and where there defined
            // Creates a testframe with the right Type
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "TestVideo_000.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);
            // now we are able to create the message
            var msg = new NewFrameArrived(_messageID, _testbitmap);
            // now send this message to the recognitionManager and get the answer in the response variable
            var response = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(msg, TimeSpan.FromSeconds(1));
            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(response.messageID, _messageID);
        }

        /// <summary>
        /// the idea here is that if the controlActor sends the message NewFrameArrived to the RecognitionManager
        /// that he does the evaluation of the frame and . After the evaluation of the frame the Recognition Manager should update his values in the
        /// dictionary CodeObjectIDToCodeObject. After that he should respond to the controlActor with the corresponding 
        /// defined message: RespondNewFrameArrived
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RecognitionManager_evaluates_the_frames_correctly()
        {
            // first create some objects which should be in the dictionary of the recognition Manager in the 
            // initialize step => all codeObjects should have the isActive false
            // CodeObject 1
            int _codeObjectID1 = 1;
            float[] _position1 = { 1f, 2f, 4f };
            float[] _rotation1 = { 1f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive1 = true;
            CodeObject _codeObject1 = new CodeObject(_codeObjectID1, _position1, _rotation1, _isActive1);
            // CodeObject 2
            int _codeObjectID2 = 2;
            float[] _position2 = { 2f, 2f, 4f };
            float[] _rotation2 = { 2f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive2 = false;
            CodeObject _codeObject2 = new CodeObject(_codeObjectID2, _position2, _rotation2, _isActive2);
            // CodeObject 3, this time we want to have the codeObject to be initialised with the default value
            // true for the isActive 
            int _codeObjectID3 = 3;
            float[] _position3 = { 3f, 2f, 4f };
            float[] _rotation3 = { 3f, 2.3f, 34f, 0.5f, 2f, 3f, 8.9f, 0.9f, 2.1f };
            bool _isActive3 = false;
            CodeObject _codeObject3 = new CodeObject(_codeObjectID3, _position3, _rotation3, _isActive3);
            // then create the dictionary for initialisation of the recognition manager
            Dictionary<int, CodeObject> _initialDict = new Dictionary<int, CodeObject>();
            _initialDict.Add(_codeObjectID1, _codeObject1);
            _initialDict.Add(_codeObjectID2, _codeObject2);
            _initialDict.Add(_codeObjectID3, _codeObject3);
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", _initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);


            // now we test one frame with no marker in it!!!


            // first create a message new frame arrived
            // create a new object of the message type NewFrameArrived. for this we need firstly a new messageID
            string _messageID1 = "FrameWithNoMarker";
            // and secondly a SoftwareBitmap and to get a testbitmap we need to follow these steps...they are
            // copied from the CameraFeedActor_UnitTest.cs and where there defined
            // Creates a testframe with the right Type
            var path1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path1 = Path.Combine(path1, "Resources");
            // here choose a file which has no marker in it
            path1 = Path.Combine(path1, "test_aruco_none.bmp");
            Stream testfile1 = File.OpenRead(path1);
            var _testbitmapNoMarker = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile1);
            // now we are able to create the message
            var _noMarkerMsg = new NewFrameArrived(_messageID1, _testbitmapNoMarker);
            // now send this message to the recognitionManager and get the answer in the response variable
            var _responseNoMarkerMsg = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(_noMarkerMsg, TimeSpan.FromSeconds(1));
            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(_responseNoMarkerMsg.messageID, _messageID1);
            // check that nothing changed in the dictionary, cause the frame had no marker in it
            var _msgRequestAll1 = new RequestAllVirtualObjects("RequestAll1", TimeSpan.FromSeconds(1));
            var responseRequestAll1 = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll1, TimeSpan.FromSeconds(1));
            // test that the messageIDs are the same
            Assert.AreEqual(responseRequestAll1.messageID, "RequestAll1");
            // the dictionary from the respond should be empty since no markers where in the frame, meaning
            // that all of them should have the isActive == false and therefore should be the dictionary empty
            // a generic dictionary is empty if its Count variable is == 0
            Assert.AreEqual(responseRequestAll1.newCodeObjectIDToCodeObject.Count, 0);


            //now test a frame that has one marker in it!!


            // first create a message new frame arrived
            // create a new object of the message type NewFrameArrived. for this we need firstly a new messageID
            string _messageID2 = "FrameWithOneMarker";
            // and secondly a SoftwareBitmap and to get a testbitmap we need to follow these steps...they are
            // copied from the CameraFeedActor_UnitTest.cs and where there defined
            // Creates a testframe with the right Type
            var path2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path2 = Path.Combine(path2, "Resources");
            // here choose a file which has one marker in it and the marker has the same id from codeObject2
            path2 = Path.Combine(path2, "test_aruco_id_1.bmp");
            Stream testfile2 = File.OpenRead(path2);
            var _testbitmapOneMarker = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile2);
            // now we are able to create the message
            var _oneMarkerMsg = new NewFrameArrived(_messageID2, _testbitmapOneMarker);
            // now send this message to the recognitionManager and get the answer in the response variable
            var _responseOneMarkerMsg = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(_oneMarkerMsg, TimeSpan.FromSeconds(1));
            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(_responseOneMarkerMsg.messageID, _messageID2);
            // check that nothing changed in the dictionary, cause the frame had no marker in it
            var _msgRequestAll2 = new RequestAllVirtualObjects("RequestAll2", TimeSpan.FromSeconds(1));
            var responseRequestAll2 = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll2, TimeSpan.FromSeconds(1));
            // test that the messageIDs are the same
            Assert.AreEqual(responseRequestAll2.messageID, "RequestAll2");
            // the dictionary from the respond should have one element in it
            Assert.AreEqual(responseRequestAll1.newCodeObjectIDToCodeObject.Count, 1);
            // and the specific element should be codeObject2 since this was in this frame
            Assert.AreEqual(responseRequestAll2.newCodeObjectIDToCodeObject.ContainsKey(_codeObject2.id), true);


            //now test a frame that has three marker in it!!


            // first create a message new frame arrived
            // create a new object of the message type NewFrameArrived. for this we need firstly a new messageID
            string _messageID3 = "FrameWithThreeMarkers";
            // and secondly a SoftwareBitmap and to get a testbitmap we need to follow these steps...they are
            // copied from the CameraFeedActor_UnitTest.cs and where there defined
            // Creates a testframe with the right Type
            var path3 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path3 = Path.Combine(path3, "Resources");
            // here choose a file which has one marker in it and the marker has the same id from codeObject2
            path3 = Path.Combine(path3, "test_aruco_id_1_2_3.bmp");
            Stream testfile3 = File.OpenRead(path3);
            var _testbitmapThreeMarkers = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile3);
            // now we are able to create the message
            var _threeMarkersMsg = new NewFrameArrived(_messageID3, _testbitmapThreeMarkers);
            // now send this message to the recognitionManager and get the answer in the response variable
            var _responseThreeMarkesrMsg = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(_threeMarkersMsg, TimeSpan.FromSeconds(1));
            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(_responseThreeMarkesrMsg.messageID, _messageID3);
            // check that nothing changed in the dictionary, cause the frame had no marker in it
            var _msgRequestAll3 = new RequestAllVirtualObjects("RequestAll3", TimeSpan.FromSeconds(1));
            var responseRequestAll3 = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll3, TimeSpan.FromSeconds(1));
            // test that the messageIDs are the same
            Assert.AreEqual(responseRequestAll3.messageID, "RequestAll3");
            // the dictionary from the respond should have trhee elements in it
            Assert.AreEqual(responseRequestAll1.newCodeObjectIDToCodeObject.Count, 3);
            // and the specific elements should be codeObjects 1,2 and 3 since they are in this frame
            Assert.AreEqual(responseRequestAll2.newCodeObjectIDToCodeObject.ContainsKey(_codeObject1.id), true);
            Assert.AreEqual(responseRequestAll2.newCodeObjectIDToCodeObject.ContainsKey(_codeObject2.id), true);
            Assert.AreEqual(responseRequestAll2.newCodeObjectIDToCodeObject.ContainsKey(_codeObject3.id), true);

            // Remark: Since we get new positions and rotations out of the recognition, we can not test the 
            // respond Dictionary regarding the codeobjects. But if they are in it, then the isActive is certainly
            // true, but if we got the right position we must trust to the library we use, that they make 
            // the evaluation of the frame correctly
        }
    }
}
