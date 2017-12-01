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
    public class RecognitionManager : IActor
    {
        // this is the id of this actor
        protected string Id { get; }
        // this dictionary is created when the recognition manager gets initialized by the controlactor and
        // it stays static meaning that the keys to value relations will not change. The only thing that changes
        // are the codeObject itself id est their properties.
        public Dictionary<int, CodeObject> codeObjectIDToCodeObject = new Dictionary<int, CodeObject>();
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
                            if (entry.Value.isActive == true)
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

            /*
             To Kiser with love


              var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            var path1 = Path.Combine(path, "BadSample.bmp");
            Stream testfile = File.OpenRead(path1);

            MemoryStream mStream = new MemoryStream();
            testfile.CopyToAsync(mStream);
            Thread.Sleep(1000);
            Bitmap bmpImage = new Bitmap(mStream);





            Emgu.CV.Image<Bgr, Byte> img = new Emgu.CV.Image<Bgr, Byte>(bmpImage);

            var outArray = new VectorOfVectorOfPointF();
            var idArray = new VectorOfInt();

            DetectorParameters dectp = DetectorParameters.GetDefault();

            var dict = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);

            Emgu.CV.Mat mat = new Emgu.CV.Mat();

            Emgu.CV.Aruco.ArucoInvoke.DrawMarker(dict, 1, 200, mat);

            var path2 = Path.Combine(path, "sample.bmp");
            mat.Save(path2);

            ArucoInvoke.DetectMarkers(img, dict, outArray, idArray, dectp);

            var cornerarray = outArray.ToArrayOfArray();
            PointF[] corners;

            var ids = idArray.ToArray();
            for (int i = 0; i<ids.Length;i++)
            {
                corners = cornerarray[i];
                Console.WriteLine(i);
                Console.WriteLine($"{corners[0].X}, {corners[0].Y} | {corners[1].X}, {corners[1].Y} | {corners[2].X}, {corners[2].Y} | {corners[3].X}, {corners[3].Y}");
            }
            Console.ReadLine();
             */
            Mat camera = new Mat();
            camera.Create(3, 3, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
            camera.SetTo(new[] { 1039.7024546115156f, 0.0f, 401.9889542361556f, 0.0f, 1038.5598693279526f, 179.02511993572065f, 0.0f, 0.0f, 11.0f });

            Mat distcoeffs = new Mat();
            distcoeffs.Create(1, 5, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
            distcoeffs.SetTo(new[] { 0.1611302127599187f, 0.11645978908419138f, -0.020783847362699993f, -0.006686827095385685f, 0.0f });

            Mat rvecs = new Mat();
            Mat tvecs = new Mat();

            Emgu.CV.Image<Bgr, Byte> _image = Utils.BitmapToImage.CreateImagefromBitmap(_testbitmap);

            var _outCorners = new VectorOfVectorOfPointF();

            var _outIDs = new VectorOfInt();

            DetectorParameters _detectorParameters = DetectorParameters.GetDefault();

            var _dict = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);

            // now do the evaluation
            Emgu.CV.Aruco.ArucoInvoke.DetectMarkers(_image, _dict, _outCorners, _outIDs, _detectorParameters, null);
            Emgu.CV.Aruco.ArucoInvoke.EstimatePoseSingleMarkers(_outCorners, 0.75f, camera, distcoeffs, rvecs, tvecs);

        }

        /// <summary>
        /// the idea here is to update the dictionary meaning that the codeObjects which are in the frame
        /// get the value true for their components isActive. Further the codeObjects should be updated with their
        /// current position and rotation as it was recognised in the FrameEvaluation Method
        /// </summary>
        public void UpdateInternalDictionary(VectorOfInt _ids)
        {
            // iterate through the whole internal dictionary, to set all CodeObject.isActive default to false
            foreach (var entry in codeObjectIDToCodeObject)
            {
                entry.Value.isActive = false;
            }

            // idea: go through the whole array of detected markers to update the data in the internal dictionary 
            for (int i = 0; i < _ids.Size; ++i)
            {
                if (codeObjectIDToCodeObject.ContainsKey(_ids[i]))
                {
                    codeObjectIDToCodeObject[_ids[i]].isActive = true;
                }
            }
        }

    }
}
