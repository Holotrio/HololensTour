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
    /// <summary>
    /// This message tells the SyncActor to write a new Dictionary on the SyncObject [Internal Use]
    /// </summary>
    public class WriteCurrentTourState
    {

        public string id;
        public Dictionary<int, CodeObject> IDToCodeObject;

        public WriteCurrentTourState(string _id, Dictionary<int, CodeObject> _dict)
        {
            id = _id;
            IDToCodeObject = _dict;
        }

    }

    // Respond that the updating of the SyncObject has been successful
    /// <summary>
    /// This message signifies that the new dictionary has been succesfully written. [Internal Use]
    /// </summary>
    public class RespondWriteCurrentTourState {

        public string id;

        public RespondWriteCurrentTourState(string _id) {
            id = _id;
        }

    }

}
