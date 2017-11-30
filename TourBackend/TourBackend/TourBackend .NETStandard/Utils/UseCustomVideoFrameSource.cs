using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using Emgu.CV;
using Emgu.CV.Structure;

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
        public static byte[] ColorOfPixelXY(System.Drawing.Bitmap _bitmap, int x, int y) {
            var color = _bitmap.GetPixel(x, y);
            byte[] returnarray = new byte[3];
            returnarray[0] = color.B;
            returnarray[1] = color.G;
            returnarray[2] = color.R;
            return returnarray;
        }

        public static byte[][][] GetPixelBytes(System.Drawing.Bitmap _bitmap, int width, int height) {
            byte[][][] returnvalue = new Byte[width][][];
            for (int i = 0; i<width; i++) {
                returnvalue[i] = new Byte[height][];
                for (int b = 0; b<height; b++) {
                    returnvalue[i][b] = new Byte[3];
                    byte[] q = ColorOfPixelXY(_bitmap, i, b);
                    for (int s = 0; s<3; s++) {
                        returnvalue[i][b][s] = q[s];
                    }
                }
            }
            return returnvalue;
        }

        public static Image<Bgr, Byte> CreateMatfromArray(System.Drawing.Bitmap _bitmap, int width, int height) {

            Image<Bgr, Byte> returnvalue = new Image<Bgr, byte>(width, height);
            byte[][][] pixels = GetPixelBytes(_bitmap, width, height);

            for (int i = 0; i < width; i++)
            {
                for (int a = 0; a < height; a++)
                {

                    byte b = pixels[i][a][0];
                    byte g = pixels[i][a][1];
                    byte r = pixels[i][a][2];
                    returnvalue.Data[i, a, 0] = b;
                    returnvalue.Data[i, a, 1] = g;
                    returnvalue.Data[i, a, 2] = r;

                }
            }
            return returnvalue;
        }  
    }
}
