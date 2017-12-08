using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;

namespace TourBackend
{
    /* 
     * The idea here is to define messageTypes for the communication between the actors.
     * To identify the specific message instance we also need a variable _messageID = _id (this is useful for debugging)
     * and to have the control of the outgoing and incoming messages...
    */

    /* Now we first define all the commands from the ControlActor to the RecognitionManager */

    /// <summary>
    /// with this message we want to be able to set an VirtualObject active meaning that it his internal state
    /// isActive is changed to true whatever it was before
    /// </summary>
    public class SetActiveVirtualObject
    {
        public string messageID;
        public int toBeActiveVirtualObjectID;

        public SetActiveVirtualObject(string _messageID, int _toBeActiveVirtualObjectID)
        {
            messageID = _messageID;
            toBeActiveVirtualObjectID = _toBeActiveVirtualObjectID;
        }
    }

    /// <summary>
    /// with this message we want to be able to set an VirtualObject inactive meaning that it his internal state
    /// isActive is changed to false whatever it was before
    /// </summary>
    public class SetInActiveVirtualObject
    {
        public string messageID;
        public int toBeInActiveVirtualObjectID;

        public SetInActiveVirtualObject(string _messageID, int _toBeInActiveVirtualObjectID)
        {
            messageID = _messageID;
            toBeInActiveVirtualObjectID = _toBeInActiveVirtualObjectID;
        }
    }

    /// <summary>
    /// with this message we want that the controler can request all virtual objects to know which virtualObjects are 
    /// currently in the isActive == true state
    /// </summary>
    public class RequestAllVirtualObjects
    {
        // here we need another variable time, meaning if it would take 
        // too long to get all the informations from all GameObjectActors
        // we can then easily define a behavior like throwing an error or sth else
        public string messageID;
        public TimeSpan timeSpan;

        public RequestAllVirtualObjects(string _messageID, TimeSpan _time)
        {
            messageID = _messageID;
            timeSpan = _time;
        }
    }

    /* the following three message commands are features that are not used for the nano-case ! */

    /// <summary>
    /// this was intented to start a video on a virtual object
    /// </summary>
    public class StartVirtualObject
    {
        public string messageID;
        public int virtualObjectIDToBeStarted;

        public StartVirtualObject(string _messageID, int _virtualObjectIDToBeStarted)
        {
            messageID = _messageID;
            virtualObjectIDToBeStarted = _virtualObjectIDToBeStarted;
        }
    }

    /// <summary>
    /// this was intented to stop a video on a virtual object
    /// </summary>
    public class StopVirtualObject
    {
        public string messageID;
        public int virtualObjectIDToBeStopped;

        public StopVirtualObject(string _messageID, int _virtualObjectIDToBeStopped)
        {
            messageID = _messageID;
            virtualObjectIDToBeStopped = _virtualObjectIDToBeStopped;
        }
    }

    /// <summary>
    /// this was intented to be able to kill a specific virtualObject
    /// </summary>
    public class KillVirtualObject
    {
        public string messageID;
        public int toBeKilledVirtualObjectID;

        public KillVirtualObject(string _messageID, int _toBeKilledVirtualObjectID)
        {
            messageID = _messageID;
            toBeKilledVirtualObjectID = _toBeKilledVirtualObjectID;
        }
    }

    /* Now we define all the responds to the upper commands */

    /// <summary>
    /// the idea here is that if the virtualObject is set active that the controlActor gets the respond with the 
    /// command messageID he sent to the recognitionManager AND the respond should also have the ID from the virtualObject
    /// which is now in the active mode.
    /// </summary>
    public class RespondSetActiveVirtualObject
    {
        public string messageID;
        public int nowActiveVirtualObjectID;
        

        public RespondSetActiveVirtualObject(string _messageID, int _nowActiveVirtualObjectID)
        {
            messageID = _messageID;
            nowActiveVirtualObjectID = _nowActiveVirtualObjectID;
        }
    }

    /// <summary>
    /// the idea here is that if the virtualObject is set inactive that the controlActor gets the respond with the 
    /// command messageID he sent to the recognitionManager AND the respond should also have the ID from the virtualObject
    /// which is now in the inactive mode.
    /// </summary>
    public class RespondSetInActiveVirtualObject
    {
        public string messageID;
        public int nowInActiveVirtualObjectID;

        public RespondSetInActiveVirtualObject(string _messageID, int _nowInActiveVirtualObjectID)
        {
            messageID = _messageID;
            nowInActiveVirtualObjectID = _nowInActiveVirtualObjectID;
        }
    }

    /// <summary>
    /// the idea here is that we answer to the requestAllVirtualObjects with a dictionary with all active virtual objects and 
    /// their virtualobjectID in it. Furthermore we also send the messageID of the original command, to have the full information
    /// needed for the controlActor
    /// </summary>
    public class RespondRequestAllVirtualObjects
    {
        public string messageID;
        // we also need a dictionary to be able to give all requested CodeObjects back to the ControlActor
        // in form of a dictionary with a key CodeObjectID and a variable CodeObject itself. BUT a codeObject should
        // only be in this dictionary if its internal variable isActive == true, otherwise it should not be in the dictionary
        public Dictionary<int, CodeObject> newCodeObjectIDToCodeObject;

        public RespondRequestAllVirtualObjects(string _messageID, Dictionary<int,CodeObject> _dict)
        {
            messageID = _messageID;
            newCodeObjectIDToCodeObject = _dict;
        }
    }

    /* the following three message responds to the commands are features that are not used for the nano-case ! */

    public class RespondStartVirtualObject
    {
        public string messageID;
        public int nowStartedVirtualObjectID;

        public RespondStartVirtualObject(string _messageID, int _nowStartedVirtualObjectID)
        {
            messageID = _messageID;
            nowStartedVirtualObjectID = _nowStartedVirtualObjectID;
        }
    }

    public class RespondStopVirtualObject
    {
        public string messageID;
        public int nowStoppedVirtualObjectID;

        public RespondStopVirtualObject(string _messageID, int _nowStoppedVirtualObjectID)
        {
            messageID = _messageID;
            nowStoppedVirtualObjectID = _nowStoppedVirtualObjectID;
        }
    }

    public class RespondKillVirtualObject
    {
        public string messageID;
        public int nowKilledVirtualObjectID;

        public RespondKillVirtualObject(string _messageID, int _nowKilledVirtualObjectID)
        {
            messageID = _messageID;
            nowKilledVirtualObjectID = _nowKilledVirtualObjectID;
        }
    }

    /* Now we define all the fails to the upper tasks. The idea here is that we only have to respond with a fail message
     * Secondly the messageID from the command has also to be sent, cause we want to know which command could not be done. */

    /* First the failures for the nano case. */

    public class FailedToSetActiveVirtualObject
    {
        public string messageID;
        public FailedToSetActiveVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    public class FailedToSetInActiveVirtualObject
    {
        public string messageID;
        public FailedToSetInActiveVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }
 
    public class FailedToRequestAllVirtualObjects
    {
        public string messageID;
        public FailedToRequestAllVirtualObjects(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /* Secondly the failures for the not nano case. */

    public class FailedToStartVirtualObject
    {
        public string messageID;
        public FailedToStartVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    public class FailedToStopVirtualObject
    {
        public string messageID;
        public FailedToStopVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    public class FailedToKillVirtualObject
    {
        public string messageID;
        public FailedToKillVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }
    
    /// <summary>
    /// this message Type here is only for testing... actually in a usecase the controlActor never asks the recognition manager
    /// to create a new object, cause that he does it on his own. if the reco Manager sees a new marker then he immediately
    /// creates a new VirtualObject from his own without calling the ControllActor 
    /// </summary>
    public class CreateNewVirtualObject
    {
        public CodeObject codeObject;
        public int codeObjectID;
        public string messageID;
        public CreateNewVirtualObject(string _messageID, int _codeObjectID, CodeObject _codeObject)
        {
            messageID = _messageID;
            codeObject = _codeObject;
            codeObjectID = _codeObjectID;
        }
    }
    /// <summary>
    /// this is the respond to the created object
    /// </summary>
    public class RespondCreateNewVirtualObject
    {
        public string messageID;
        public RespondCreateNewVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /// <summary>
    /// here the sense of this message is that if we got the message NewFrameArrived with a specific messageID,
    /// then we should work with that frame and if the work is succesfully done we should respond with that 
    /// message type and with the same messageID as the request came and then the control Actor knows that
    /// the frame was successfully done
    /// </summary>
    public class RespondNewFrameArrived
    {
        public string messageID;
        public RespondNewFrameArrived(string _messageID)
        {
            messageID = _messageID;
        }
    }
}
