using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

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
            /* use the Function from OpenCVSharp.Aruco.CvAruco for the detection: 
             * public static void DetectMarkers(InputArray image, Dictionary dictionary, out Point2f[][] corners, out int[] ids, DetectorParameters parameters, out Point2f[][] rejectedImgPoints)
             * 
             */

            // Mat _mat = BitmapConverter.ToMat(_bitmap);
            // indicates the type of markers that will be searched
            OpenCvSharp.Aruco.Dictionary _dict = OpenCvSharp.Aruco.CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.DictArucoOriginal);
            // _dict=0; vector of detected marker corners.
            // For each marker, its four corners are provided. For N detected markers,
            // the dimensions of this array is Nx4.The order of the corners is clockwise.

            // vector of identifiers of the detected markers. The identifier is of type int.
            // For N detected markers, the size of ids is also N. The identifiers have the same order than the markers 
            // in the imgPoints array.

            // marker detection parameters
            OpenCvSharp.Aruco.DetectorParameters _parameters = DetectorParameters.Create();

            // contains the imgPoints of those squares whose inner code has not a 
            // correct codification. Useful for debugging purposes.

            //OpenCvSharp.Aruco.CvAruco.DetectMarkers(_mat, _dict, out Point2f[][] _corners, out int[] _ids, _parameters , out Point2f[][] _rejectedImgPoints);

        }

    }
}
