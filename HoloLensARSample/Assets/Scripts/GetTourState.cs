using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TourBackend
{

    public class GetTourState : MonoBehaviour
    {
        public SyncObject syncObject;
        public Dictionary<string, CodeObject> CopyOfDict;// = new Dictionary<string, CodeObject>();
        public System.Int64 lasttimestamp;
        // Use this for initialization
        public void Start()
        {
            //Initialisierung SyncObject
            lasttimestamp = 10;
            syncObject = new SyncObject("syncid", new Dictionary<string, CodeObject>());
            syncObject.SetTimeStamp(lasttimestamp);
            CopyOfDict = new Dictionary<string, CodeObject>();

        }

        // Update is called once per frame
        public void Update()
        {
            //GetCopyOfDictionary
            //ToDo: Test if lock is free


            //Only copy if timestamp changed
            if (syncObject.timestamp != lasttimestamp)
            {
                lasttimestamp = syncObject.timestamp;
                CopyOfDict = syncObject.dict;
                //ReadDictionaryData
                foreach (string objectid in CopyOfDict.Keys)
                {
                    //CodeObject with current key
                    CodeObject obj = CopyOfDict[objectid];
                    //TestTypeOfData
                    switch (obj.mediaid)
                    {
                        case 1:
                            //instantiate Texture, set position and rotation
                            InstantiateTexture(obj);
                            break;
                        case 2:
                            //instantiate 3D model, set position and rotation
                            InstantiateModel(obj);
                            break;
                        case 3:
                            //instantiate video, set position and rotation
                            InstantiateVideo(obj);
                            break;
                        default:
                            //media without mediaid -> error
                            break;


                    }

                }
            }


        }

        public void InstantiateTexture(CodeObject obj)
        {
            //Let the option open to load the texture itself instead of loading a prefab. Can be done later
            GameObject textureobj = (GameObject)Resources.Load("Texture/" + obj.objectid);
            textureobj = Instantiate(textureobj);
            textureobj.transform.name = obj.objectid;

            //Set position
            textureobj.transform.position = SetPosition(obj);

            //Set eulerangles
            textureobj.transform.eulerAngles = SetEulerangles(obj);

        }

        public void InstantiateModel(CodeObject obj)
        {
            GameObject modelobj = (GameObject)Resources.Load("Model/" + obj.objectid);
            modelobj = Instantiate(modelobj);
            modelobj.transform.name = obj.objectid;

            //Set position
            modelobj.transform.position = SetPosition(obj);

            //Set eulerangles
            modelobj.transform.eulerAngles = SetEulerangles(obj);

        }

        public void InstantiateVideo(CodeObject obj)
        {
            //Let the option open to load the texture itself instead of loading a prefab. Can be done later
            GameObject videoobj = (GameObject)Resources.Load("Video/" + obj.objectid);
            videoobj = Instantiate(videoobj);
            videoobj.transform.name = obj.objectid;

            //Set position
            videoobj.transform.position = SetPosition(obj);

            //Set eulerangles
            videoobj.transform.eulerAngles = SetEulerangles(obj);

            //Get video component of prefab
            var videoplayer = videoobj.GetComponent<UnityEngine.Video.VideoPlayer>();

            Debug.Log(videoplayer.isPrepared + " : Status of the videoplayer");
            
            videoplayer.playOnAwake = true;
            videoplayer.waitForFirstFrame = true;

            //Start playing the video
            //videoplayer.Play();
            //videoplay.isPlaying is not necesarry true now. It has to be loaded first.
            //To do: possibility to stop and restart the video

        }


        public void DestroyObject(GameObject obj)
        {
            DestroyImmediate(obj);
        }

        public Vector3 SetPosition(CodeObject obj)
        {
            float x = obj.position[0];
            float y = obj.position[1];
            float z = obj.position[2];
            Vector3 position = new Vector3(x, y, z);
            return position;

        }

        public Vector3 SetEulerangles(CodeObject obj)
        {
            float rotX = obj.rotation[0];
            float rotY = obj.rotation[1];
            float rotZ = obj.rotation[2];
            Vector3 eulerangles = new Vector3(rotX, rotY, rotZ);
            return eulerangles;
        }
    }
}