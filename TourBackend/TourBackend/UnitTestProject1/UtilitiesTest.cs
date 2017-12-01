using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace TourBackend
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void ColorOfPixelXY_must_return_the_right_values()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "braun.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            var ret = Utils.ColorOfPixelXY(_testbitmap,395,244);

            Assert.AreEqual(ret[0], 11); // B-Wert
            Assert.AreEqual(ret[1], 37); // G-Wert
            Assert.AreEqual(ret[2], 107); // R-Wert
        }

        [TestMethod]
        public void GetPixelBytes_must_return_the_right_values() {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "braun.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            var ret = Utils.GetPixelBytes(_testbitmap);
            for (int i = 0; i < _testbitmap.Width; i++)
            {
                for (int b = 0; b < _testbitmap.Height; b++)
                {
                    Assert.AreEqual(ret[i][b][0], 11); // B-Wert
                    Assert.AreEqual(ret[i][b][1], 37); // G-Wert
                    Assert.AreEqual(ret[i][b][2], 107); // R-Wert
                }
            }
        }

        [TestMethod]
        public void CreateImagefromBitmap_must_return_the_right_values()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "braun.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(testfile);

            var ret = Utils.CreateImagefromBitmap(_testbitmap);

            for (int i = 0; i < _testbitmap.Width; i++)
            {
                for (int a = 0; a < _testbitmap.Height; a++)
                {
                    Assert.AreEqual(ret.Data[a, i, 0], 11);
                    Assert.AreEqual(ret.Data[a, i, 1], 37);
                    Assert.AreEqual(ret.Data[a, i, 2], 107);
                }
            }

        }

    }
}
