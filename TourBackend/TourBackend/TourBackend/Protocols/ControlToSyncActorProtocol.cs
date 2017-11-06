﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBackend
{

    public class SyncObject
    {

        private Object thisLock = new Object();

        public string objectid;
        public Dictionary<string, CodeObject> dict;

        //Basic Konstruktor 
        public SyncObject(string _objectid, Dictionary<string, CodeObject> _dict)
        {
            objectid = _objectid;
            dict = _dict;
        }

    }

    public class CodeObject {

        public string objectid;
        public int mediaid;
        public int[] position;
        public int[] rotation;

        public CodeObject(string _objectid, int _mediaid, int[] _position, int[] _rotation) {
            objectid = _objectid;
            mediaid = _mediaid;
            position = _position;
            rotation = _rotation;
        }
    }

    public class WriteCurrentTourState
    {

        public string id;
        public Dictionary<string, CodeObject> dict;

        public WriteCurrentTourState(string _id, Dictionary<string, CodeObject> _dict)
        {
            id = _id;
            dict = _dict;
        }
    }

    public class RespondWriteCurrentTourState {

        public string id;

        public RespondWriteCurrentTourState(string _id) {
            id = _id;
        }

    }

}
