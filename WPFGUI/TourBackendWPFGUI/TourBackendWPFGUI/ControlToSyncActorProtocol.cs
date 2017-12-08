using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TourBackend
{
    public class SyncObject
    {
        //has to be added to framework
        public event EventHandler SyncObjectUpdated;

        protected void OnSyncObjectUpdated(EventArgs e)
        {
            EventHandler handler = SyncObjectUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void UpdateSyncObject()
        {
            OnSyncObjectUpdated(EventArgs.Empty);
        }

        public Int64 timestamp;

        public object thisLock = new Object();

        public string objectid;
        public Dictionary<int, CodeObject> dict;

        //Basic Konstruktor 
        public SyncObject(string _objectid, Dictionary<int, CodeObject> _dict)
        {
            objectid = _objectid;
            dict = _dict;
        }

        public void SetTimeStamp(Int64 _timestamp)
        {
            this.timestamp = _timestamp;
        }

    }

    // Encapsulates the current state of an Object 
    // This definition is subject to fluent change

    public class CodeObject
    {

        public int id = -1;

        public bool isActive = true;
        public float[] position;
        public float[] rotation;
        public float[] scaling;

        public CodeObject(CodeObject codeobj)
        {

            position = new float[3];
            rotation = new float[9];
            scaling = new float[3];

            id = codeobj.id;
            codeobj.position.CopyTo(position, 0);
            codeobj.rotation.CopyTo(rotation, 0);
            codeobj.scaling.CopyTo(scaling, 0);
            isActive = codeobj.isActive;
        }

        public CodeObject()
        {
        }

        public CodeObject(int _objectid, float[] _position, float[] _rotation, bool _isActive)
        {
            id = _objectid;
            position = _position;
            rotation = _rotation;
            isActive = _isActive;
        }

        public CodeObject(int _objectid, float[] _position, float[] _rotation)
        {
            id = _objectid; ;
            position = _position;
            rotation = _rotation;
        }
    }

    // Request to update the SyncObject with the current TourState

    public class WriteCurrentTourState
    {

        public string id;
        public Dictionary<int, CodeObject> dict;

        public WriteCurrentTourState(string _id, Dictionary<int, CodeObject> _dict)
        {
            id = _id;
            dict = _dict;
        }

    }

    // Respond that the updating of the SyncObject has been successful

    public class RespondWriteCurrentTourState
    {

        public string id;

        public RespondWriteCurrentTourState(string _id)
        {
            id = _id;
        }

    }

}
