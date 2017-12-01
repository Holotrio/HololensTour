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
    }
}
