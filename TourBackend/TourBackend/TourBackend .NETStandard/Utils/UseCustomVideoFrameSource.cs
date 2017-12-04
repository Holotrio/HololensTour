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

namespace TourBackend
{

    // TestActor which links every received message to a referenced object
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
                double[ , ] positions = new double[_numberOfCodeObjects, 3];
                double[ , ] rotations = new double[_numberOfCodeObjects, 3];
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
                    double[] position = new double[3];
                    position[0] = positions[i, 0];
                    position[1] = positions[i, 1];
                    position[2] = positions[i, 2];

                    double[] rotation = new double[3];
                    rotation[0] = rotations[i, 0];
                    rotation[1] = rotations[i, 1];
                    rotation[2] = rotations[i, 2];

                    CodeObject codeObject = new CodeObject(ids[i], position, rotation, isActive);
                    returnDict.Add(codeObject.id, codeObject);
                }
                return returnDict;
            }           
        }
    }
}
