using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;

namespace TourBackend
{
    /* 
     * This is kind of a pseude Protocoll, cause we have all the same messages from the control to the 
     * recognition manager as from the recognition manager to the CodeObjects. Therefore to know which
     * messages are allowed between the recognition manager and the code objects, you should go to the
     * ControlToRecognitionManagerProtocol.cs file and read more there...
    */

    public class UpdateCodeObjectActor {
        public string messageid;
        public int objectid;
        public double[] position;
        public double[] rotation;

        public UpdateCodeObjectActor(string _messageid, int _objectid, double[] _position, double[] _rotation) {
            messageid = _messageid;
            position = _position;
            rotation = _rotation;
            objectid = _objectid;
        }
    }

    public class RequestCodeObject {
        public string messageid;

        public RequestCodeObject(string _messageid) {
            messageid = _messageid;
        }
    }

    public class RespondCodeObject
    {
        public string messageid;
        public CodeObject codeobject;

        public RespondCodeObject(string _messageid, CodeObject _codeobject)
        {
            messageid = _messageid;
            codeobject = _codeobject;
        }
    }

}
