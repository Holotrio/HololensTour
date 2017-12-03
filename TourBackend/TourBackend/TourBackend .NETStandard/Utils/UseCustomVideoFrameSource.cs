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

namespace TourBackend
{

    // TestActor which links every received message to a referenced object
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

    public static class Utils
    {
        public static class BitmapToImage
        {
            public static byte[] ColorOfPixelXY(System.Drawing.Bitmap _bitmap, int x, int y)
            {
                var color = _bitmap.GetPixel(x, y);
                byte[] returnarray = new byte[3];
                returnarray[0] = color.B;
                returnarray[1] = color.G;
                returnarray[2] = color.R;
                return returnarray;
            }

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
                float[ , ] positions = new float[_numberOfCodeObjects, 3];
                float[ , ] rotations = new float[_numberOfCodeObjects, 3];
                const bool isActive = false;
                Dictionary<int, CodeObject> returnDict = new Dictionary<int, CodeObject>();

                for (int i = 0; i < _numberOfCodeObjects; ++i)
                {
                    ids[i] = i + 1;
                    positions[i , 0]= 1f + i;
                    positions[i , 1] = 2f + i;
                    positions[i , 2] = 3f + i;
                    rotations[i, 0] = 0.1f + i;
                    rotations[i, 1] = 0.1f + i;
                    rotations[i, 2] = 0.1f + i;
                }

                for (int i = 0; i < _numberOfCodeObjects; ++i)
                {
                    float[] position = new float[3];
                    position[0] = positions[i, 0];
                    position[1] = positions[i, 1];
                    position[2] = positions[i, 2];

                    float[] rotation = new float[3];
                    rotation[0] = rotations[i, 0];
                    rotation[1] = rotations[i, 1];
                    rotation[2] = rotations[i, 2];

                    CodeObject codeObject = new CodeObject(ids[i], position, rotation, isActive);
                    returnDict.Add(codeObject.id, codeObject);
                }
                return returnDict;
            }

            public static void CreateMarkerWithID(int _ID)
            {
                // define the path where the marker bitmap should be stored
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(path, "Resources");

                // define the type of the Aruco Code with the dictionary
                var typeOfArucoCode = new  Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);

                // create the mat where the marker is firstly stored
                Emgu.CV.Mat arucoCodeAsMat = new Emgu.CV.Mat();

                // draw the marker into the mat
                Emgu.CV.Aruco.ArucoInvoke.DrawMarker(typeOfArucoCode, 1, 200, arucoCodeAsMat);


                // specifiy the filename of the created marker and store it
                string filename = "ArucoCode_with_ID_" + _ID + ".bmp";
                var pathTarget = Path.Combine(path,filename);
                arucoCodeAsMat.Save(pathTarget);
            }
        }
    }
}
