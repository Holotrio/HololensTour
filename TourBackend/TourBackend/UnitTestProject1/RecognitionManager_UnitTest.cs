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
            // create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            initialDict[1].isActive = true;
            initialDict[5].isActive = true;
            initialDict[7].isActive = true;
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // here we really do now the request from the testControlActor to the recognitionManager and we store
            // the respond to the request in response where this must be a object of the class RespondRequestAllVirtualObjects
            // which contains of a dictionary and a messageID to know to which Request the Respond was
            var _msgRequestAll = new RequestAllVirtualObjects("RequestAll", TimeSpan.FromSeconds(1));
            var responseRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll, TimeSpan.FromSeconds(1));

            // here we actually test if the Call "RequestAllVirtualObjects" can what we intended
            // first we check if the response have the same messageID as the request had
            Assert.AreEqual("RequestAll", responseRequestAll.messageID);

            // the respondDict should have three CodeObjects with ID 1, 5 and 7 in it
            Dictionary<int, CodeObject> expectedRespDict = new Dictionary<int, CodeObject>();
            expectedRespDict.Add(1, initialDict[1]);
            expectedRespDict.Add(5, initialDict[5]);
            expectedRespDict.Add(7, initialDict[7]);
            CollectionAssert.AreEqual(expectedRespDict, responseRequestAll.newCodeObjectIDToCodeObject);
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
            // create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // now send the message that codeObject 1 should be set active
            var _msgSetActiveCO1 = new SetActiveVirtualObject("SetActiveCO1", 1);
            var responseToSetActive = await _pidTestRecognitionManager.RequestAsync<RespondSetActiveVirtualObject>(_msgSetActiveCO1, TimeSpan.FromSeconds(1));

            // now we should get the right messageID and the right CodeObject ID back
            Assert.AreEqual("SetActiveCO1", responseToSetActive.messageID);
            Assert.AreEqual(1, responseToSetActive.nowActiveVirtualObjectID);

            // and then test if the data is at the current state in the dictionary
            // first make an request all call to the recognitionManager to get the whole Dictionary back
            var _msgRequestAll = new RequestAllVirtualObjects("RequestAll1", TimeSpan.FromSeconds(1));
            var responseToRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll);

            // and then test if the right thing is in the dictionary
            Dictionary<int, CodeObject> expectedRespDict = new Dictionary<int, CodeObject>();
            expectedRespDict.Add(1, initialDict[1]);
            CollectionAssert.AreEqual(expectedRespDict, responseToRequestAll.newCodeObjectIDToCodeObject);

            // test if we wanna set Active a CodeObject which does not exist, then a failed to message should come back
            var _msgSetActiveCO100 = new SetActiveVirtualObject("SetActiveCO100", 100);
            var responseToSetActive100 = await _pidTestRecognitionManager.RequestAsync<FailedToSetActiveVirtualObject>(_msgSetActiveCO100, TimeSpan.FromSeconds(1));
            Assert.AreEqual("SetActiveCO100", responseToSetActive100.messageID);
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
            // create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            initialDict[1].isActive = true;
            initialDict[5].isActive = true;
            initialDict[7].isActive = true;
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // now send the message that codeObject 1 should be set inActive
            var _msgSetInActiveCO1 = new SetInActiveVirtualObject("SetInActiveCO1", 1);
            var responseToSetInActive = await _pidTestRecognitionManager.RequestAsync<RespondSetInActiveVirtualObject>(_msgSetInActiveCO1, TimeSpan.FromSeconds(1));

            // now we should get the right messageID and the right CodeObject ID back
            Assert.AreEqual("SetInActiveCO1", responseToSetInActive.messageID);
            Assert.AreEqual(1, responseToSetInActive.nowInActiveVirtualObjectID);

            // and then test if the data is at the current state in the dictionary
            // first make an request all call to the recognitionManager to get the whole Dictionary back
            var _msgRequestAll = new RequestAllVirtualObjects("RequestAll", TimeSpan.FromSeconds(1));
            var responseToRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(_msgRequestAll);

            // and then test if the right thing is in the dictionary, meaning that the isActive of the 
            // codeObject1 has changed from true to false and that is if the CodeObject is no longer in that respondDictionary
            Assert.AreEqual("RequestAll", responseToRequestAll.messageID);
            // the respondDict should have two CodeObjects with ID 5 and 7 in it
            Dictionary<int, CodeObject> expectedRespDict = new Dictionary<int, CodeObject>();
            expectedRespDict.Add(5, initialDict[5]);
            expectedRespDict.Add(7, initialDict[7]);
            CollectionAssert.AreEqual(expectedRespDict, responseToRequestAll.newCodeObjectIDToCodeObject);

            // test if we wanna set InActive a CodeObject which does not exist, then a failed to message should come back
            var _msgSetInActiveCO100 = new SetInActiveVirtualObject("SetInActiveCO100", 100);
            var responseToSetInActive100 = await _pidTestRecognitionManager.RequestAsync<FailedToSetInActiveVirtualObject>(_msgSetInActiveCO100, TimeSpan.FromSeconds(1));
            Assert.AreEqual("SetInActiveCO100", responseToSetInActive100.messageID);
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
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // create a new object of the message type NewFrameArrived. for this we need firstly a new messageID
            string _messageID = "NewFrameArrived1";
            // and secondly a SoftwareBitmap and to get a testbitmap we need to follow these steps...they are
            // copied from the CameraFeedActor_UnitTest.cs and where there defined
            // Creates a testframe with the right Type
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            var msg = new NewFrameArrived(_messageID, _testbitmap);

            // now send this message to the recognitionManager and get the answer in the response variable
            var response = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(msg, TimeSpan.FromSeconds(10));

            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(response.messageID, _messageID);
        }

        /// <summary>
        /// the idea here is that if the controlActor sends the message NewFrameArrived to the RecognitionManager (=RM), then 
        /// the RM makes the evaluation of the frames and After the that he should update his values in the
        /// dictionary CodeObjectIDToCodeObject. Finally the RM should respond to the controlActor with the corresponding 
        /// defined message: RespondNewFrameArrived to signalise that all works perfectly fine.
        /// </summary>
        [TestMethod]
        public async Task RecognitionManager_evaluates_one_frame_with_no_marker_correctly()
        {
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            initialDict[1].isActive = true;
            initialDict[5].isActive = true;
            initialDict[7].isActive = true;
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // create a new object of the message type NewFrameArrived
            string _messageID = "NewFrameArrived";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_No_Marker.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            // now we are able to create the message
            var msg = new NewFrameArrived(_messageID, _testbitmap);

            // now send this message to the recognitionManager and get the answer in the response variable
            var respondNewFrameArrived = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(msg, TimeSpan.FromSeconds(10));

            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(respondNewFrameArrived.messageID, _messageID);

            // now check with the request all if the right changes were made in the dictionary
            var requestAll = new RequestAllVirtualObjects("Request", TimeSpan.FromSeconds(1));
            var respondRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(requestAll, TimeSpan.FromSeconds(1));
            Assert.AreEqual("Request", respondRequestAll.messageID);
            Assert.AreEqual(0, respondRequestAll.newCodeObjectIDToCodeObject.Count);
        }

        /// <summary>
        /// the idea here is that if the controlActor sends the message NewFrameArrived to the RecognitionManager (=RM), then 
        /// the RM makes the evaluation of the frames and After the that he should update his values in the
        /// dictionary CodeObjectIDToCodeObject. Finally the RM should respond to the controlActor with the corresponding 
        /// defined message: RespondNewFrameArrived to signalise that all works perfectly fine.
        /// </summary>
        [TestMethod]
        public async Task RecognitionManager_evaluates_one_frame_with_one_marker_correctly()
        {
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // create a new object of the message type NewFrameArrived
            string _messageID = "NewFrameArrived";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            // now we are able to create the message
            var msg = new NewFrameArrived(_messageID, _testbitmap);

            // now send this message to the recognitionManager and get the answer in the response variable
            var respondNewFrameArrived = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(msg, TimeSpan.FromSeconds(10));

            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(respondNewFrameArrived.messageID, _messageID);

            // check with the request all if the right objects are in the dictionary
            var requestAll = new RequestAllVirtualObjects("Request", TimeSpan.FromSeconds(1));
            var respondRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(requestAll, TimeSpan.FromSeconds(1));
            Assert.AreEqual("Request", respondRequestAll.messageID);
            Assert.AreEqual(1, respondRequestAll.newCodeObjectIDToCodeObject.Count);
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(1));

            // check that the translation vector of the codeobject is the same
            double e = 0.0000000001d; // is the error we accept for the comparison of two double numbers
            double[,] correctTranslations = Utils.HelpForTesting.GetTranslationsOfBitmapFile("ArucoCode_ID_1.bmp");
            Assert.AreEqual(true, System.Math.Abs(correctTranslations[0, 0] - respondRequestAll.newCodeObjectIDToCodeObject[1].position[0]) < e);
            Assert.AreEqual(true, System.Math.Abs(correctTranslations[0, 1] - respondRequestAll.newCodeObjectIDToCodeObject[1].position[1]) < e);
            Assert.AreEqual(true, System.Math.Abs(correctTranslations[0, 2] - respondRequestAll.newCodeObjectIDToCodeObject[1].position[2]) < e);

            // check that the rotation matrix is the same
            double[,] correctRotMat = Utils.HelpForTesting.GetRotationMatricesOfBitmapFile("ArucoCode_ID_1.bmp");
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[0, r] - respondRequestAll.newCodeObjectIDToCodeObject[1].rotation[r]) < e);
            }
        }

        /// <summary>
        /// the idea here is that if the controlActor sends the message NewFrameArrived to the RecognitionManager (=RM), then 
        /// the RM makes the evaluation of the frames and After the that he should update his values in the
        /// dictionary CodeObjectIDToCodeObject. Finally the RM should respond to the controlActor with the corresponding 
        /// defined message: RespondNewFrameArrived to signalise that all works perfectly fine.
        /// </summary>
        [TestMethod]
        public async Task RecognitionManager_evaluates_one_frame_with_two_markers_correctly()
        {
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // create a new object of the message type NewFrameArrived
            string _messageID = "NewFrameArrived";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1_8.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            // now we are able to create the message
            var msg = new NewFrameArrived(_messageID, _testbitmap);

            // now send this message to the recognitionManager and get the answer in the response variable
            var respondNewFrameArrived = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(msg, TimeSpan.FromSeconds(10));

            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(respondNewFrameArrived.messageID, _messageID);

            // now check with the request all if the right changes were made in the dictionary
            var requestAll = new RequestAllVirtualObjects("Request", TimeSpan.FromSeconds(1));
            var respondRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(requestAll, TimeSpan.FromSeconds(1));
            Assert.AreEqual("Request", respondRequestAll.messageID);
            Assert.AreEqual(2, respondRequestAll.newCodeObjectIDToCodeObject.Count);
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(1));
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(8));

            // check if the translation vectors are the ones we expect it to be
            double e = 0.0000000001d; // is the error we accept for the comparison of two double numbers
            double[,] correctTranslations = Utils.HelpForTesting.GetTranslationsOfBitmapFile("ArucoCode_ID_1_8.bmp");
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[0, p] - respondRequestAll.newCodeObjectIDToCodeObject[1].position[p]) < e);
            }
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[1, p] - respondRequestAll.newCodeObjectIDToCodeObject[8].position[p]) < e);
            }

            // check if the rotation matrices are as expected
            double[,] correctRotMat = Utils.HelpForTesting.GetRotationMatricesOfBitmapFile("ArucoCode_ID_1_8.bmp");
            for(int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[0, r] - respondRequestAll.newCodeObjectIDToCodeObject[1].rotation[r]) < e);
            }
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[1, r] - respondRequestAll.newCodeObjectIDToCodeObject[8].rotation[r]) < e);
            }
        }

        /// <summary>
        /// the idea here is that if the controlActor sends the message NewFrameArrived to the RecognitionManager (=RM), then 
        /// the RM makes the evaluation of the frames and After the that he should update his values in the
        /// dictionary CodeObjectIDToCodeObject. Finally the RM should respond to the controlActor with the corresponding 
        /// defined message: RespondNewFrameArrived to signalise that all works perfectly fine.
        /// </summary>
        [TestMethod]
        public async Task RecognitionManager_evaluates_one_frame_with_multiple_markers_correctly()
        {
            // then create the testrecognition manager and the dictionary with all the initialized markers in it. 
            // But all have isActive = false...
            Dictionary<int, CodeObject> initialDict = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            var _propsTestRecognitionManager = Actor.FromProducer(() => new RecognitionManager("RecognitionManager", initialDict));
            var _pidTestRecognitionManager = Actor.Spawn(_propsTestRecognitionManager);

            // create a new object of the message type NewFrameArrived
            string _messageID = "NewFrameArrived";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1_2_3_7_10.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            // now we are able to create the message
            var msg = new NewFrameArrived(_messageID, _testbitmap);

            // now send this message to the recognitionManager and get the answer in the response variable
            var respondNewFrameArrived = await _pidTestRecognitionManager.RequestAsync<RespondNewFrameArrived>(msg, TimeSpan.FromSeconds(10));

            // now check if we get the right answer meaning the right message id
            Assert.AreEqual(respondNewFrameArrived.messageID, _messageID);

            // now check with the request all if the right changes were made in the dictionary
            var requestAll = new RequestAllVirtualObjects("Request", TimeSpan.FromSeconds(1));
            var respondRequestAll = await _pidTestRecognitionManager.RequestAsync<RespondRequestAllVirtualObjects>(requestAll, TimeSpan.FromSeconds(1));
            Assert.AreEqual("Request", respondRequestAll.messageID);
            Assert.AreEqual(5, respondRequestAll.newCodeObjectIDToCodeObject.Count);
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(1));
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(2));
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(3));
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(7));
            Assert.AreEqual(true, respondRequestAll.newCodeObjectIDToCodeObject.ContainsKey(10));

            // check if the translation vectors are the ones we expect it to be
            double e = 0.0000000001d; // is the error we accept for the comparison of two double numbers
            double[,] correctTranslations = Utils.HelpForTesting.GetTranslationsOfBitmapFile("ArucoCode_ID_1_2_3_7_10.bmp");
            // see TestEmguCVEstimatePoseSingleMarker output with the input file "ArucoCode_ID_1_2_3_7_10.bmp " why we have 
            // the following ID order in the correctTranslations Array
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[0, p] - respondRequestAll.newCodeObjectIDToCodeObject[7].position[p]) < e);
            }
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[1, p] - respondRequestAll.newCodeObjectIDToCodeObject[2].position[p]) < e);
            }
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[2, p] - respondRequestAll.newCodeObjectIDToCodeObject[3].position[p]) < e);
            }
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[3, p] - respondRequestAll.newCodeObjectIDToCodeObject[1].position[p]) < e);
            }
            for (int p = 0; p < 3; ++p)
            {
                Assert.AreEqual(true, System.Math.Abs(correctTranslations[4, p] - respondRequestAll.newCodeObjectIDToCodeObject[10].position[p]) < e);
            }

            // check if the rotation matrices are as expected
            double[,] correctRotMat = Utils.HelpForTesting.GetRotationMatricesOfBitmapFile("ArucoCode_ID_1_2_3_7_10.bmp");
            // see TestEmguCVEstimatePoseSingleMarker output with the input file "ArucoCode_ID_1_2_3_7_10.bmp " why we have 
            // the following ID order in the correctRotMat Array
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[0, r] - respondRequestAll.newCodeObjectIDToCodeObject[7].rotation[r]) < e);
            }
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[1, r] - respondRequestAll.newCodeObjectIDToCodeObject[2].rotation[r]) < e);
            }
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[2, r] - respondRequestAll.newCodeObjectIDToCodeObject[3].rotation[r]) < e);
            }
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[3, r] - respondRequestAll.newCodeObjectIDToCodeObject[1].rotation[r]) < e);
            }
            for (int r = 0; r < 9; ++r)
            {
                Assert.AreEqual(true, System.Math.Abs(correctRotMat[4, r] - respondRequestAll.newCodeObjectIDToCodeObject[10].rotation[r]) < e);
            }
        }
    }
}