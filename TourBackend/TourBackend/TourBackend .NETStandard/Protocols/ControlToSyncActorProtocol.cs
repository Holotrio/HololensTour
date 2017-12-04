using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Diagnostics;

namespace TourBackend
{

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

    public class RespondWriteCurrentTourState {

        public string id;

        public RespondWriteCurrentTourState(string _id) {
            id = _id;
        }

    }

}
