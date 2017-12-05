using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;
using Emgu;
using Emgu.CV.Aruco;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace TourBackend
{
    /// <summary>
    /// The Recognition Manager Actor evaluates the frames and does the bookkeeping of all Aruco Code Markers with their
    /// properties which are stored as CodeObjects. The CameraMatrix, the Distortion Coefficients and the Aruco Code Marker
    /// length is defined in the method "FrameEvaluation" which is also in defined in the Recognition Manager. These parameters
    /// which are needed by EmguCV for the recognition can vary and if you have to adjust them, do it there.
    /// </summary>
    public class RecognitionManager : IActor
    {
        protected string Id { get; }

        // this dictionary is created when the recognition manager gets initialized by the control actor and
        // it stays static meaning that the key to value relations will not change. The only thing that changes
        // are the codeObject properties like rotation, position, isActive...
        public Dictionary<int, CodeObject> codeObjectIDToCodeObject = new Dictionary<int, CodeObject>();

        // constructor
        public RecognitionManager(string id, Dictionary<int, CodeObject> _dict)
        {
            Id = id;
            codeObjectIDToCodeObject = _dict;
        }

        // here we write all reactions if we receive a certain message from any other actor
        public Task ReceiveAsync(IContext context)
        {
            var message = context.Message;

            // here we check of which type the message is and then we react specifically. the message types are all defined
            // in the folder Protocols
            switch (message)
            {
                // the idea here is that if we get the requestall virtual objects we check what we have in 
                // the dictionary codeObjectIDToCodeObject and then we create a new Dictionary with only the 
                // CodeObjects in it which have the isActive == true
                case RequestAllVirtualObjects r:
                                            
                        // first create a new empty dictionary for the response
                        Dictionary<int, CodeObject> returnDict = new Dictionary<int, CodeObject>();
                       
                        foreach (var entry in codeObjectIDToCodeObject)
                        {
                            if (entry.Value.isActive == true)
                            {
                                returnDict.Add(entry.Key, entry.Value);
                            }
                        }

                        // if we now have the return Dictionary, send the Respond message back to the sender
                        context.Sender.Tell(new RespondRequestAllVirtualObjects(r.messageID, returnDict));
                        Console.WriteLine("Recognition Manager says: The work of the RequestAll with ID = '" + r.messageID + "' is done.");
                    
                    break;

                // the idea here is if we get the setActive message we should look up in the dictionary if the 
                // requested CodeObject does exitst. If it exists we should set the isActive to true. And then 
                // send back the response to the sender. If it does not exist in the internal dictionary we should 
                // respond with a failedTo message.
                case SetActiveVirtualObject s:
                                            
                        if (codeObjectIDToCodeObject.ContainsKey(s.toBeActiveVirtualObjectID))
                        {
                            codeObjectIDToCodeObject[s.toBeActiveVirtualObjectID].isActive = true;
                            // respond to the sender
                            context.Sender.Tell(new RespondSetActiveVirtualObject(s.messageID, s.toBeActiveVirtualObjectID));
                            Console.WriteLine("Recognition Manager says: The CodeObject with ID = '" + s.toBeActiveVirtualObjectID + "' is now active");
                        }
                        else
                        {
                            // respond to the sender with a failedToMessage
                            context.Sender.Tell(new FailedToSetActiveVirtualObject(s.messageID));
                            Console.WriteLine("Recognition Manager says: The CodeObject with ID = '" + s.toBeActiveVirtualObjectID + "' which should have been activated, does not exist in the internal dictionary");
                        }
                    
                    break;

                // the idea here is if we get the setInActive message we should look up in the dictionary if the 
                // requested CodeObject does exitst. If it exists we should set the isActive to false. And then 
                // send back the response back to the sender. If it does not exist in the internal dictionary we should 
                // respond with a failedTo message.
                case SetInActiveVirtualObject s:
                                          
                        if (codeObjectIDToCodeObject.ContainsKey(s.toBeInActiveVirtualObjectID))
                        {
                            codeObjectIDToCodeObject[s.toBeInActiveVirtualObjectID].isActive = false;
                            // respond to the sender
                            context.Sender.Tell(new RespondSetInActiveVirtualObject(s.messageID, s.toBeInActiveVirtualObjectID));
                            Console.WriteLine("Recognition Manager says: The CodeObject with ID = '" + s.toBeInActiveVirtualObjectID + "' is now inactive");
                    }
                    else
                        {
                            // respond to the sender with a failedToMessage
                            context.Sender.Tell(new FailedToSetInActiveVirtualObject(s.messageID));
                            Console.WriteLine("Recognition Manager says: The CodeObject with ID = '" + s.toBeInActiveVirtualObjectID + "' which should have been inactivated, does not exist in the internal dictionary");

                    }

                    break;

                // the idea here is if the recognition manager gets this message, he takes the frame, evaluate it and
                // then update the internal dictionary. having done this he should reply to the controlActor that all went well
                case NewFrameArrived n:

                        Console.WriteLine("Recognition Manager says: The work with the frame with ID = '" + n.id + "' started");
                        // do the work here with the recognition of the frame
                        FrameEvaluation(n.bitmap);
                        // after the successfull evaluation respond to the control Actor
                        context.Sender.Tell(new RespondNewFrameArrived(n.id));
                        Console.WriteLine("Recognition Manager says: The work with the frame with ID = '" + n.id + "' is done");
                    
                    break;
            }
            return Actor.Done;
        }

        /// <summary>
        /// use this method to recognize the aruco code markers in the bitmap (frame) and update then all
        /// markers in the dictionary with their corresponding new properties according to the information which is
        /// in the frame.
        /// </summary>
        /// <param name="_bitmap">
        /// this is the frame, the photo which gets evaluated. it has to be of the type System.Drawing.Bitmap
        /// </param>
        public void FrameEvaluation(Bitmap _bitmap)
        {
            // first set all arguments for the DetectMarkers method call
            Emgu.CV.Image<Bgr, Byte> _image = Utils.BitmapToImage.CreateImagefromBitmap(_bitmap);
            var MarkerTypeToFind = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);
            var outCorners = new VectorOfVectorOfPointF();
            var outIDs = new VectorOfInt();
            DetectorParameters _detectorParameters = DetectorParameters.GetDefault();

            // now detect the markers in the image bitmap and for further information look up the EmguCv documentation
            Emgu.CV.Aruco.ArucoInvoke.DetectMarkers(_image, MarkerTypeToFind, outCorners, outIDs, _detectorParameters, null);
            
            // then define all arguments for the estimatePoseSingleMarkers method call
            float markerLength = 0.1f; // set the default markerLength which is usually given in the unit meters
            // the cameraMatrix and distortion coefficients are the one for the hololens. the data is from: https://github.com/qian256/HoloLensCamCalib/blob/master/near/hololens896x504.yaml
            Mat cameraMatrix = new Mat();
            cameraMatrix.Create(3, 3, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
            cameraMatrix.SetTo(new[] { 1039.7024546115156f, 0.0f, 401.9889542361556f, 0.0f, 1038.5598693279526f, 179.02511993572065f, 0.0f, 0.0f, 11.0f });
            Mat distcoeffs = new Mat();
            distcoeffs.Create(1, 5, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
            distcoeffs.SetTo(new[] { 0.1611302127599187f, 0.11645978908419138f, -0.020783847362699993f, -0.006686827095385685f, 0.0f });
            Mat rvecs = new Mat();
            Mat tvecs = new Mat();

            // now get all the pose in form of a translation vector in tvecs and all rotations in form of a rotation vector in rvecs
            // and for further information look up the EmguCv documentation or the description of the method "UpdateInternalDictionary"
            Emgu.CV.Aruco.ArucoInvoke.EstimatePoseSingleMarkers(outCorners, markerLength, cameraMatrix, distcoeffs, rvecs, tvecs);

            Console.WriteLine("Recognition Manager says: The Positions and Rotations are extracted out of the frame.");

            // now update the internal dictionary with the new data
            UpdateInternalDictionary(outIDs, tvecs, rvecs);

            Console.WriteLine("Recognition Manager says: The internal dictionary is updated according to the frame.");
        }

        /// <summary>
        /// the idea here is to update the dictionary meaning that the codeObjects which are in the frame
        /// get the value true for their components isActive. Further the codeObjects should be updated with their
        /// current position and rotation as it was recognised in the FrameEvaluation Method
        /// </summary>
        /// <param name="_ids">
        /// has all the ID's of the recognised ArucoCodeMarkers in the frame unordered in it. the type of the ID's are only integers
        /// </param>
        /// <param name="_translationVectors">
        /// has all translationVectors of the recognised ArucoCodeMarkers in the frame in it. For each marker there are 3 double values
        /// which describe the translation along the axis x,y and z stored like [i,0] = x , [i,1] = y and [i,2] = z. The order which marker is 
        /// in which row is exactly according to the order of the _ids vectorOfInt.
        /// </param>
        /// <param name="_rotationVectors">
        /// has all rotationVectors of the recognised ArucoCodeMarkers in the frame in it. For each marker there are 3 double values
        /// which describe the rotation. they form a vector. its direction describes the rotation axis and its length defines the rotation 
        /// in radian degree. The order which marker is in which row is exactly according to the order of the _ids vectorOfInt.
        /// </param>
        public void UpdateInternalDictionary(VectorOfInt _ids, Mat _translationVectors, Mat _rotationVectors)
        {
            // first define the parameters to be able to test them in a boolean expression
            int idSize = _ids.Size;
            int tvecsRows = _translationVectors.Rows;
            int rvecsRows = _rotationVectors.Rows;

            // iterate through the whole internal dictionary, to set all CodeObject.isActive default to false
            foreach (KeyValuePair<int, CodeObject> entry in codeObjectIDToCodeObject)
            {
                entry.Value.isActive = false;
            }

            // idea: go through the whole array of detected markers to update the data in the internal dictionary 
            for (int i = 0; i < _ids.Size; ++i)
            {
                if (codeObjectIDToCodeObject.ContainsKey(_ids[i]))
                {                    
                    // set first the isActive of the recognised CodeObject to true
                    codeObjectIDToCodeObject[_ids[i]].isActive = true;

                    // then update the position                       
                    if (tvecsRows == idSize)
                    {
                        // first get the data out of the mat
                        double[] translationVector = new double[3];
                        _translationVectors.Row(i).CopyTo<double>(translationVector);
                        // then iterate through the vector and update its position
                        for (int p = 0; p < translationVector.Length; ++p)
                        {
                            codeObjectIDToCodeObject[_ids[i]].position[p] = translationVector[p];
                        }
                    }

                    // and finally update the roation matrix of the codeObject
                    if (rvecsRows == idSize)
                    {
                        // first create the rotation matrix still as a Mat
                        Mat _rotMat = new Mat();
                        Emgu.CV.CvInvoke.Rodrigues(_rotationVectors.Row(i), _rotMat, null);
                        // then get the data out of the Mat
                        double[] rotMat = new double[9];
                        _rotMat.CopyTo<double>(rotMat);
                        // then update the rotationMatrix
                        for (int r = 0; r < rotMat.Length; ++r)
                        {
                            codeObjectIDToCodeObject[_ids[i]].rotation[r] = rotMat[r];
                        }
                    }
                }
                else
                {
                    // there is a marker which is recognised but is not initialised in the dictionary, meaning it does not exist for our purpose and therefore we did not implement a behavior for this case                    
                }
            }
        }
    }
}
