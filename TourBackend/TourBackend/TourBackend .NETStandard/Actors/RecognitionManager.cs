using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;
using Emgu;
using Emgu.CV.Aruco;
namespace TourBackend
{
    public class RecognitionManager : IActor
    {
        // this is the id of this actor
        protected string Id { get; }
        // this dictionary is created when the recognition manager gets initialized by the controlactor and
        // it stays static meaning that the keys to value relations will not change. The only thing that changes
        // are the codeObject itself id est their properties.
        public Dictionary<int,CodeObject> codeObjectIDToCodeObject = new Dictionary<int, CodeObject>();
        // here is the constructor
        public RecognitionManager(string id, Dictionary<int, CodeObject> _dict)
        {
            Id = id;
            codeObjectIDToCodeObject = _dict;
        }
        // here we write all actions if we receive a certain message
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                // the idea here is that if we get the requestall virtual objects we check what we have in 
                // the dictionary codeObjectIDToCodeObject and then we create a new Dictionary with only the 
                // CodeObjects in it which have the isActive == true
                case RequestAllVirtualObjects r:
                    {
                        // first create a new empty dictionary
                        Dictionary<int, CodeObject> returnDict = new Dictionary<int, CodeObject>();
                        // for each codeObject in the dictionary, add the key value pair into the returnDict
                        // if and only if his isActive == true
                        foreach (var entry in codeObjectIDToCodeObject)
                        {
                            if(entry.Value.isActive == true)
                            {
                                returnDict.Add(entry.Key, entry.Value);
                            }
                        }
                        // if we now have the return Dictionary, send the Respond message back to the sender
                        var _respondToRequestMessage = new RespondRequestAllVirtualObjects(r.messageID, returnDict);
                        context.Sender.Tell(_respondToRequestMessage);
                    }
                    break;

                // the idea here is if we get the setActive message we should look up in the dictionary if the 
                // requested CodeObject does exitst. If it exists we should set the isActive to true. And then 
                // send back the response to the sender
                case SetActiveVirtualObject s:
                    {
                        if (codeObjectIDToCodeObject.ContainsKey(s.toBeActiveVirtualObjectID))
                        {
                            codeObjectIDToCodeObject[s.toBeActiveVirtualObjectID].isActive = true;
                            // respond to the sender
                            var _respondMsg = new RespondSetActiveVirtualObject(s.messageID, s.toBeActiveVirtualObjectID);
                            context.Sender.Tell(_respondMsg);
                        }
                        else
                        {
                            // respond to the sender with a failedToMessage
                            var _failMsg = new FailedToSetActiveVirtualObject(s.messageID);
                            context.Sender.Tell(_failMsg);
                        }
                    }
                    break;

                // the idea here is if we get the setInActive message we should look up in the dictionary if the 
                // requested CodeObject does exitst. If it exists we should set the isActive to false. And then 
                // send back the response back to the sender.
                case SetInActiveVirtualObject s:
                    {
                        if (codeObjectIDToCodeObject.ContainsKey(s.toBeInActiveVirtualObjectID))
                        {
                            codeObjectIDToCodeObject[s.toBeInActiveVirtualObjectID].isActive = false;
                            // respond to the sender
                            var _respondMsg = new RespondSetInActiveVirtualObject(s.messageID, s.toBeInActiveVirtualObjectID);
                            context.Sender.Tell(_respondMsg);
                        }
                        else
                        {
                            // respond to the sender with a failedToMessage
                            var _failMsg = new FailedToSetInActiveVirtualObject(s.messageID);
                            context.Sender.Tell(_failMsg);
                        }
                    }
                    break;

                case NewFrameArrived n:
                    {
                        // do the work here with the recognition of the frame
                        FrameEvaluation(n.bitmap);
                        // after the successfull evaluation respond to the control Actor
                        var _respondMsg = new RespondNewFrameArrived(n.id);
                        context.Sender.Tell(_respondMsg);
                    }
                    break;
            }
            return Actor.Done;
        }


        /// <summary>
        /// the idea here is that we use this function to recognize the markers in the bitmap and update then all
        /// markers in the dictionary with their position and rotation etc. 
        /// </summary>
        public void FrameEvaluation(Bitmap _bitmap)
        {
            // use the Function from Emgu.Cv.Aruco.ArucoInvoke for the detection: 

            /*  *** here we should also have the conversion of the information from the corner information to the actual
             * rotation and position of the marker *** */

            // now we have done the evaluation of the frame and now we want to update the dictionary with the current data

        }

        /// <summary>
        /// the idea here is to update the dictionary meaning that the codeObjects which are in the frame
        /// get the value true for their components isActive. Further the codeObjects should be updated with their
        /// current position and rotation as it was recognised in the FrameEvaluation Method
        /// </summary>
        public void UpdateInternalDictionary(int[] _ids)
        {
            // iterate through the whole internal dictionary, to set all CodeObject.isActive default to false
            foreach (var entry in codeObjectIDToCodeObject)
            {
                entry.Value.isActive = false;
            }

            // idea: go through the whole array of detected markers to update the data in the internal dictionary 
            for (int i = 0; i < _ids.Length; ++i)
            {
                if (codeObjectIDToCodeObject.ContainsKey(_ids[i]))
                {
                    codeObjectIDToCodeObject[_ids[i]].isActive = true;
                }
            }
        }

    }
}
