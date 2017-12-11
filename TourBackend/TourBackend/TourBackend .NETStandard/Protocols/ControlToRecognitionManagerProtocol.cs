using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;

namespace TourBackend
{
    /* 
     * The idea here is to define messageTypes for the communication between the two actors: 
     * (i) Control Actor (ii) Recognition Manager 
     * To identify the specific message instance we also need a variable _messageID = _id (this is useful for debugging)
     * and to have the control of the outgoing and incoming messages in the Control Actor.
    */

    /* Now we first define all the commands/requests from the ControlActor to the RecognitionManager */

    /// <summary>
    /// with this message type we are able to set a specific VirtualObject active meaning that it his internal state
    /// isActive is changed to true whatever value that boolean had before. If the VirtualObject in question is not
    /// listed in the internal dictionary of the Recognition Manager we get a failed to do this task respond.
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
    /// with this message type we are able to set a specific VirtualObject inactive meaning that it his internal state
    /// isActive is changed to false whatever value that boolean had before. If the VirtualObject in question is not
    /// listed in the internal dictionary of the Recognition Manager we get a failed to do this task respond.
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
    /// with this message type we are able to request all virtual objects with the current internal state  
    /// isActive == true in form of a dictionary which maps the CodeObjectID's to the CodeObjects 
    /// </summary>
    public class RequestAllVirtualObjects
    {
        // here we need another variable time, meaning if it would take 
        // too long to get all the informations from all GameObjectActors
        // we can then easily define a behavior like throwing an error
        public string messageID;
        public TimeSpan timeSpan;

        public RequestAllVirtualObjects(string _messageID, TimeSpan _time)
        {
            messageID = _messageID;
            timeSpan = _time;
        }
    }

    /* the following three message commands/requests are features that are not used for the nano-case ! */

    /// <summary>
    /// with this message type we are able to start an action (like playing a video) of a virtual object. If the 
    /// VirtualObject in question is not isted in the internal dictionary of the Recognition Manager we get a 
    /// failed to do this task respond.
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
    /// with this message type we are able to stop an action (like playing a video) of a virtual object.
    /// If the VirtualObject in question is not listed in the internal dictionary of the Recognition 
    /// Manager we get a failed to do this task respond.
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
    /// with this message type we are able to create a virtualObject in the sense 
    /// that we can add a specific CodeObject to the internal dictionary of the Recognition Manager
    /// which CodeObjectID is not yet in there. If its ID already exists there, we get a failed to to this
    /// task respond.
    /// </summary>
    public class CreateNewVirtualObject
    {
        public CodeObject codeObjectToBeCreated;
        public string messageID;

        public CreateNewVirtualObject(string _messageID, CodeObject _codeObjectToBeCreated)
        {
            messageID = _messageID;
            codeObjectToBeCreated = _codeObjectToBeCreated;
        }
    }

    /// <summary>
    /// with this message type we are able to kill a specific virtualObject meaning to delete it in the 
    /// internal dictionary "codeObjectIDToCodeObject" of the recognition manager. If the VirtualObject in question is not
    /// listed in the internal dictionary of the Recognition Manager we get a failed to do this task respond.
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

    /* Now we define all the responds to the upper commands/requests from the control actor to the recognition manager */

    /// <summary>
    /// with this message type we are able to respond to the command SetActiveVirtualObject to say the work
    /// was successfully done
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
    /// with this message type we are able to respond to the command SetInActiveVirtualObject to say the work
    /// was successfully done
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
    /// with this message type we are able to respond to the command RequestAllVirtualObjects in the sense that
    /// the work was successfully done
    /// </summary>
    public class RespondRequestAllVirtualObjects
    {
        public string messageID;
        // we also need a dictionary to be able to give all requested CodeObjects back to the sender
        // in form of a dictionary with a key "CodeObjectID" and a value "CodeObject" itself. BUT a codeObject should
        // only be in this dictionary if its internal variable isActive == true, otherwise it should not be in the dictionary
        public Dictionary<int, CodeObject> newCodeObjectIDToCodeObject;

        public RespondRequestAllVirtualObjects(string _messageID, Dictionary<int,CodeObject> _dict)
        {
            messageID = _messageID;
            newCodeObjectIDToCodeObject = _dict;
        }
    }

    /* the following three message responds to the commands are features that are not used for the nano-case ! */

    /// <summary>
    /// with this message type we are able to respond to the command StartVirtualObject to say the work
    /// was successfully done
    /// </summary>
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

    /// <summary>
    /// with this message type we are able to respond to the command StopVirtualObject to say the work
    /// was successfully done
    /// </summary>
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

    /// <summary>
    /// with this message type we are able to respond to the command CreateNewVirtualObject to say the work
    /// was successfully done
    /// </summary>
    public class RespondCreateNewVirtualObject
    { 
        public string messageID;
        public int nowCreatedVirtualObjectID;

        public RespondCreateNewVirtualObject(string _messageID, int _nowCreatedVirtualObjectID)
        {
            messageID = _messageID;
            nowCreatedVirtualObjectID = _nowCreatedVirtualObjectID;
        }
    }

    /// <summary>
    /// with this message type we are able to respond to the command KillVirtualObject to say the work
    /// was successfully done
    /// </summary>
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

    /* Now we define all the fails to the upper tasks. The idea here is that we only have to respond with a fail message and we know then enough about the error.
     * Secondly the messageID from the command has also to be sent, cause we want to know which command could not be done. */

    /* First the failures for the nano case. */

    /// <summary>
    /// with this message we are able to respond to the command SetActiveVirtualObject with a failed to do this task message.
    /// </summary>
    public class FailedToSetActiveVirtualObject
    {
        public string messageID;

        public FailedToSetActiveVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /// <summary>
    /// with this message we are able to respond to the command SetInActiveVirtualObject with a failed to do this task message.
    /// </summary>
    public class FailedToSetInActiveVirtualObject
    {
        public string messageID;

        public FailedToSetInActiveVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /* Secondly the failures for the not nano case. */

    /// <summary>
    /// with this message we are able to respond to the command StartVirtualObject with a failed to do this task message.
    /// </summary>
    public class FailedToStartVirtualObject
    {
        public string messageID;

        public FailedToStartVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /// <summary>
    /// with this message we are able to respond to the command StopVirtualObject with a failed to do this task message.
    /// </summary>
    public class FailedToStopVirtualObject
    {
        public string messageID;

        public FailedToStopVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /// <summary>
    /// with this message we are able to respond to the command CreateNewVirtualObject with a failed to do this task message.
    /// </summary>
    public class FailedToCreateNewVirtualObject
    {
        public string messageID;

        public FailedToCreateNewVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }

    /// <summary>
    /// with this message we are able to respond to the command KillVirtualObject with a failed to do this task message.
    /// </summary>
    public class FailedToKillVirtualObject
    {
        public string messageID;

        public FailedToKillVirtualObject(string _messageID)
        {
            messageID = _messageID;
        }
    }
}
