using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Aruco;
using System.Reflection;
using System.Drawing;
using static System.Drawing.Bitmap;
using System.Drawing.Imaging;
using Emgu.CV.Util;

namespace TourBackend
{
    /// <summary>
    /// Used to link a message from one actor to another place [Internal Use] [Testing Only]
    /// </summary>
    public class TestActor : IActor
    {
        public object testMsg;

        public TestActor(ref object _msg)
        {
            testMsg = new Object();
            _msg = testMsg;
        }

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            testMsg = msg;
            return Actor.Done;
        }
    }

    /// <summary>
    /// This classes encompasses various utility functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Encompasses functions which are needed to convert a Bitmap to an Emgu Image, since System.Drawing.Bitmap is not fully available on .NET Core 2.0 [Internal Use] [Testing Only]
        /// </summary>
        public static class BitmapToImage
        {
            /// <summary>
            /// Gets the color of one specific pixel in the BGR format. [Internal Use] [Testing Only]
            /// </summary>
            /// <param name="_bitmap">User provided Bitmap</param>
            /// <param name="x">x-coordinate of the pixel whose color is to be determined.</param>
            /// <param name="y">y-coordinate of the pixel whose color is to be determined.</param>
            /// <returns>BGR value (flipped RGB value) of the pixel</returns>
            public static byte[] ColorOfPixelXY(System.Drawing.Bitmap _bitmap, int x, int y)
            {
                var color = _bitmap.GetPixel(x, y);
                byte[] returnarray = new byte[3];
                returnarray[0] = color.B;
                returnarray[1] = color.G;
                returnarray[2] = color.R;
                return returnarray;
            }

            /// <summary>
            /// Gets the pixel information of all pixels in a Bitmap and returns them as byte array in BGR format. [Internal Use] [Testing Only]
            /// </summary>
            /// <param name="_bitmap">Bitmap to be analyzed</param>
            /// <returns>3D byte array with pixel data: WidthxHeightxBGR</returns>
            public static byte[][][] GetPixelBytes(System.Drawing.Bitmap _bitmap)
            {
                byte[][][] returnvalue = new Byte[_bitmap.Width][][];
                for (int i = 0; i < _bitmap.Width; i++)
                {
                    returnvalue[i] = new Byte[_bitmap.Height][];
                    for (int b = 0; b < _bitmap.Height; b++)
                    {
                        returnvalue[i][b] = new Byte[3];
                        byte[] q = ColorOfPixelXY(_bitmap, i, b);
                        for (int s = 0; s < 3; s++)
                        {
                            returnvalue[i][b][s] = q[s];
                        }
                    }
                }
                return returnvalue;
            }

            /// <summary>
            /// Creates an Emgu Image from the given Bitmap without using System.Drawing.Bitmap functions. [Internal Use] [Testing Only]
            /// </summary>
            /// <param name="_bitmap">Bitmap to be analyzed.</param>
            /// <returns>Emgu Image which represents the given Bitmap</returns>
            public static Image<Bgr, Byte> CreateImagefromBitmap(System.Drawing.Bitmap _bitmap)
            {

                Image<Bgr, Byte> returnvalue = new Image<Bgr, byte>(_bitmap.Width, _bitmap.Height);
                byte[][][] pixels = GetPixelBytes(_bitmap);

                for (int i = 0; i < _bitmap.Width; i++)
                {
                    for (int a = 0; a < _bitmap.Height; a++)
                    {

                        byte b = pixels[i][a][0];
                        byte g = pixels[i][a][1];
                        byte r = pixels[i][a][2];
                        returnvalue.Data[a, i, 0] = b;
                        returnvalue.Data[a, i, 1] = g;
                        returnvalue.Data[a, i, 2] = r;

                    }
                }
                return returnvalue;
                // Beware: Emgu uses the HxW format instead of the intuitive WxH format. 
                // It's possible that the returned Image has to be flipped to acheive intended behaviour.
            }
        }

        public static class HelpForTesting
        {
            /// <summary>
            /// creates ten markers with all the components needed to initialise the Recognition Manager with all
            /// the markers that have the isActive to false
            /// </summary>
            /// <returns></returns>
            public static Dictionary<int, CodeObject> CreateDictionaryForInitialization(int _numberOfCodeObjects)
            {
                int[] ids = new int[_numberOfCodeObjects];
                double[,] positions = new double[_numberOfCodeObjects, 3];
                double[,] rotations = new double[_numberOfCodeObjects, 9];
                const bool isActive = false;
                Dictionary<int, CodeObject> returnDict = new Dictionary<int, CodeObject>();

                for (int i = 0; i < _numberOfCodeObjects; ++i)
                {
                    ids[i] = i + 1;
                    positions[i, 0] = 1f + i;
                    positions[i, 1] = 2f + i;
                    positions[i, 2] = 3f + i;
                    for (int r = 0; r < 9; ++r)
                    {
                        rotations[i, r] = 0.1f + i / (_numberOfCodeObjects + 1);
                    }
                }

                for (int i = 0; i < _numberOfCodeObjects; ++i)
                {
                    double[] position = new double[3];
                    position[0] = positions[i, 0];
                    position[1] = positions[i, 1];
                    position[2] = positions[i, 2];

                    double[] rotation = new double[9];
                    for (int r = 0; r < 9; ++r)
                    {
                        rotation[r] = rotations[i, r];
                    }
                    CodeObject codeObject = new CodeObject(ids[i], position, rotation, isActive);
                    returnDict.Add(codeObject.id, codeObject);
                }
                return returnDict;
            }

            /// <summary>
            /// here we create an array which returns us the translation elements of a given file
            /// </summary>
            /// <param name="_filename">
            /// the file needs to be a .bmp file and the filename should be of the form "testBitmap.bmp"
            /// and it should lie in the resourses folder...
            /// </param>
            /// <returns> 
            /// the returned Array has in each row the translation data for the given Marker in the outID array
            /// of the functioncall detectMarkers. meaning the first marker in outID has its translation data also
            /// in the first row. the second marker in the second row and so on...
            /// </returns>
            public static double[,] GetTranslationsOfBitmapFile(string _filename)
            {
                // first get the bitmap file
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(path, "Resources");
                path = Path.Combine(path, _filename);
                Stream testfile = File.OpenRead(path);
                var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

                // then define the arguments for the emguCV functions

                // first set all arguments for the DetectMarkers function call
                Emgu.CV.Image<Bgr, Byte> _image = Utils.BitmapToImage.CreateImagefromBitmap(_testbitmap);
                var MarkerTypeToFind = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);
                var outCorners = new VectorOfVectorOfPointF();
                var outIDs = new VectorOfInt();
                DetectorParameters _detectorParameters = DetectorParameters.GetDefault();

                // now detect the markers in the image bitmap
                Emgu.CV.Aruco.ArucoInvoke.DetectMarkers(_image, MarkerTypeToFind, outCorners, outIDs, _detectorParameters, null);

                // then define all arguments for the estimatePoseSingleMarkers function call
                float markerLength = 0.1f; // set the default markerLength which is usually given in the unit meter
                Mat cameraMatrix = new Mat();
                cameraMatrix.Create(3, 3, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
                cameraMatrix.SetTo(new[] { 1039.7024546115156f, 0.0f, 401.9889542361556f, 0.0f, 1038.5598693279526f, 179.02511993572065f, 0.0f, 0.0f, 11.0f });
                Mat distcoeffs = new Mat();
                distcoeffs.Create(1, 5, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
                distcoeffs.SetTo(new[] { 0.1611302127599187f, 0.11645978908419138f, -0.020783847362699993f, -0.006686827095385685f, 0.0f });
                Mat rvecs = new Mat();
                Mat tvecs = new Mat();

                // now get all the pose in tvecs and all rotations in rvecs
                Emgu.CV.Aruco.ArucoInvoke.EstimatePoseSingleMarkers(outCorners, markerLength, cameraMatrix, distcoeffs, rvecs, tvecs);

                // define the return value
                double[,] returnArray = new double[outIDs.Size,3];

                // here we extract the translation vector out of the tvecs mat return value
                // which stored the values as doubles in the mat
                int idSize = outIDs.Size;
                int tvecsRows = tvecs.Rows;
                if (idSize == tvecsRows)
                {
                    for (int i = 0; i < tvecsRows; ++i)
                    {
                        double[] _tvecs = new double[3];
                        tvecs.Row(i).CopyTo<double>(_tvecs);
                        for (int p = 0; p < _tvecs.Length; ++p)
                        {
                            returnArray[i,p] = _tvecs[p];
                        }
                    }
                }

                // now return the array where first all translation vectors are stored and then all rotation matrices
                return returnArray;
            }


            public static double[,] GetRotationMatricesOfBitmapFile(string _filename)
            {
                // first get the bitmap file
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(path, "Resources");
                path = Path.Combine(path, _filename);
                Stream testfile = File.OpenRead(path);
                var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

                // then define the arguments for the emguCV functions

                // first set all arguments for the DetectMarkers function call
                Emgu.CV.Image<Bgr, Byte> _image = Utils.BitmapToImage.CreateImagefromBitmap(_testbitmap);
                var MarkerTypeToFind = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);
                var outCorners = new VectorOfVectorOfPointF();
                var outIDs = new VectorOfInt();
                DetectorParameters _detectorParameters = DetectorParameters.GetDefault();

                // now detect the markers in the image bitmap
                Emgu.CV.Aruco.ArucoInvoke.DetectMarkers(_image, MarkerTypeToFind, outCorners, outIDs, _detectorParameters, null);

                // then define all arguments for the estimatePoseSingleMarkers function call
                float markerLength = 0.1f; // set the default markerLength which is usually given in the unit meter
                Mat cameraMatrix = new Mat();
                cameraMatrix.Create(3, 3, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
                cameraMatrix.SetTo(new[] { 1039.7024546115156f, 0.0f, 401.9889542361556f, 0.0f, 1038.5598693279526f, 179.02511993572065f, 0.0f, 0.0f, 11.0f });
                Mat distcoeffs = new Mat();
                distcoeffs.Create(1, 5, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
                distcoeffs.SetTo(new[] { 0.1611302127599187f, 0.11645978908419138f, -0.020783847362699993f, -0.006686827095385685f, 0.0f });
                Mat rvecs = new Mat();
                Mat tvecs = new Mat();

                // now get all the pose in tvecs and all rotations in rvecs
                Emgu.CV.Aruco.ArucoInvoke.EstimatePoseSingleMarkers(outCorners, markerLength, cameraMatrix, distcoeffs, rvecs, tvecs);

                // define the return value
                double[,] returnArray = new double[outIDs.Size, 9];

                // here we extract the roation vector out of the rvecs mat return value
                // which stored the values as doubles in the mat
                int idSize = outIDs.Size;
                int rvecsRows = rvecs.Rows;
                if (idSize == rvecsRows)
                {
                    for (int i = 0; i < rvecsRows; ++i)
                    {
                        // then create the outputmatrix
                        // here we create a rotation matrix out of the rotation vector, it is this a 3x3 matrix with
                        // 9 elements and it is like: rotMat(x,y) = _rotmat(x+y-2)
                        // which stored the values as doubles in the mat
                        double[] rotMat = new double[9];
                        Mat _rotMat = new Mat();
                        Emgu.CV.CvInvoke.Rodrigues(rvecs.Row(i), _rotMat, null);
                        _rotMat.CopyTo<double>(rotMat);
                        for (int r = 0; r < rotMat.Length; ++r)
                        {
                            returnArray[i, r] = rotMat[r];
                        }
                    }                
                }
                // now return the array where first all translation vectors are stored and then all rotation matrices
                return returnArray;
            }

        }
    }
}
