using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace TourBackend
{
    public class GetTourStateTest
    {
        //Initialize GetTourState
        GetTourState getTourState = new GameObject().AddComponent<GetTourState>();

        [Test]
        public void GetTourState_initialize_empty_SyncObject_correctly()
        {
            // Use the Assert class to test conditions.
            //Testobject
            //GameObject test = new GameObject();
            //Create getTourState object
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();
            
            getTourState.Start();

            //Compare if the object is as expected empty
            Assert.AreEqual(getTourState.syncObject.objectid, "syncid");
            CollectionAssert.AreEqual(getTourState.syncObject.dict, new Dictionary<string, CodeObject>());
            Assert.AreEqual(getTourState.syncObject.timestamp, 10);

        }

        [Test]
        public void GetTourState_lasttimestamp_gets_updated_to_new_timestamp_of_SyncObject()
        {
            //Initiate GetTourState
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();

            //Create SyncObject
            getTourState.Start();

            //initial timestamp
            System.Int64 inititialTimestamp = 10;

            //Check old time Stamp
            Assert.AreEqual(getTourState.syncObject.timestamp, inititialTimestamp);

            //Update GetTourState
            getTourState.Update();
            //Check if copy has right timestamp
            Assert.AreEqual(getTourState.lasttimestamp, inititialTimestamp);

            //update timestamp
            System.Int64 newTimestamp = 12;

            //Change timestamp
            getTourState.syncObject.timestamp = newTimestamp;

            //Update GetTourState
            getTourState.Update();

            //Check if timestamp is changed on syncObject
            Assert.AreEqual(getTourState.lasttimestamp, newTimestamp);

        }

        //It is a integration Test not unit test. Will be needed later
        /*[Test]
        public void GetTourState_copy_new_dict_entry_of_SyncObject()
        {
            //Initiate GetTourState
            GetTourState getTourState = new GameObject().AddComponent<GetTourState>();

            //Create SyncObject
            getTourState.Start();

            //Check if CopyOfDict is a new Dictionary
            CollectionAssert.AreEqual(getTourState.CopyOfDict, new Dictionary<string, CodeObject>());

            //Adding a new Dictonary Object and change timestamp
            getTourState.syncObject.objectid = "textureid";
            getTourState.syncObject.dict.Add("textureid", new CodeObject("textureid", 1, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }));
            getTourState.syncObject.timestamp = 12;

            //Update GetTourState
            getTourState.Update();

            //Check if CopyOfDict is updated
            Assert.AreEqual(getTourState.CopyOfDict.ContainsKey("textureid"), true);
            //Check if it's the right CodeObject
            Assert.AreEqual(getTourState.CopyOfDict["objectid"].objectid, "textureid");
            Assert.AreEqual(getTourState.CopyOfDict["objectid"].mediaid, 1);
            CollectionAssert.AreEqual(getTourState.CopyOfDict["objectid"].position, new float[] { 0, 0, 0 });
            CollectionAssert.AreEqual(getTourState.CopyOfDict["objectid"].rotation, new float[] { 0, 0, 0 });

        }
        */
        [Test]
        public void GetTourState_initiate_model_Prefab_with_ID()
        {
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();
            CodeObject obj = new CodeObject("modelid", 2, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 });
            //Call Function that creates Object in Unity
            getTourState.InstantiateModel(obj);
            //Search for object in Unity
            GameObject model = null;
            model = GameObject.Find("modelid");
            Assert.AreNotEqual(model, null);
            //Destroy object in Unity
            getTourState.DestroyObject(model);

        }

        [Test]
        public void GetTourState_initialize_model_Prefab_with_ID_on_defined_position()
        {
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();
            //Position from origin
            Vector3 position = new Vector3(0.1f, 1, 1);
            //Create new Codeobject
            CodeObject obj = new CodeObject("modelid", 2, new float[] { 0.1f, 1, 1 }, new float[] { 0, 0, 0 });
            //Call Function that creates Object in Unity
            getTourState.InstantiateModel(obj);
            //Search for object in Unity
            GameObject model = null;
            model = GameObject.Find("modelid");
            //Checks if object is moved to expected position
            Assert.AreEqual(model.transform.position, position);

            //Destroy Object
            getTourState.DestroyObject(model);

        }

        [Test]
        public void GetTourState_initialize_model_Prefab_with_ID_rotated_with_defined_eulerangles()
        {
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();
            //Position from origin
            Vector3 eulerangles = new Vector3(30f, 45f, 60f);
            //Create new Codeobject
            CodeObject obj = new CodeObject("modelid", 2, new float[] { 0f, 0f, 0f }, new float[] { 30f, 45f, 60f });
            //Call Function that creates Object in Unity
            getTourState.InstantiateModel(obj);
            //Search for object in Unity
            GameObject model = null;
            model = GameObject.Find("modelid");
            //Checks if object is moved to expected position
            Assert.AreEqual((model.transform.eulerAngles.x - eulerangles.x) < 0.0001, true);
            Assert.AreEqual((model.transform.eulerAngles.y - eulerangles.y) < 0.0001, true);
            Assert.AreEqual((model.transform.eulerAngles.z - eulerangles.z) < 0.0001, true);


            //Destroy Object
            getTourState.DestroyObject(model);

        }

        [Test]
        public void GetTourState_initiate_Texture2D_Prefab_with_ID()
        {
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();
            CodeObject obj = new CodeObject("textureid", 1, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 });
            //Call Function that creates Object in Unity
            getTourState.InstantiateTexture(obj);

            //Search for object in Unity
            GameObject texture = null;
            texture = GameObject.Find("textureid");
            Assert.AreNotEqual(texture, null);
            //Destroy object in Unity
            getTourState.DestroyObject(texture);

        }

        [Test]
        public void GetTourState_initiate_Video_Prefab_with_ID()
        {
            //GetTourState getTourState = new GameObject().AddComponent<GetTourState>();
            CodeObject obj = new CodeObject("videoid", 1, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 });
            //Call Function that creates Object in Unity
            getTourState.InstantiateVideo(obj);
            //Search for object in Unity
            GameObject video = null;
            video = GameObject.Find("videoid");
            Assert.AreNotEqual(video, null);
            //Destroy object in Unity
            getTourState.DestroyObject(video);

        }
        
        //not necesarry for beginning. Videoplayer.isPrepared -> Play() not autoplay
        /*
        [Test] public void GetTourState_Video_is_playing()
        {
            CodeObject obj = new CodeObject("videoid", 1, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 });
            //Call Function that creates Object in Unity
            getTourState.InstantiateVideo(obj);
            
            //Search for object in Unity
            GameObject video = null;
            video = GameObject.Find("videoid");
            var videoplayer = video.GetComponent<UnityEngine.Video.VideoPlayer>();
            
            Assert.AreEqual(true, videoplayer.isPlaying);

        }
        */
        //Only two show how to test Vector3 objects
        [Test]
        public void VectorAssertTest()
        {
            Vector3 a = new Vector3(0.1f, 0, 1);
            Vector3 b = new Vector3(0.1f, 0, 1);
            Vector3 c = new Vector3(0.1f, 0, 2);

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(a, c);
            Assert.AreNotEqual(b, c);

        }


        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator GetTourStateTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;



        }
 
    }

}