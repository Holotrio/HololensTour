using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace TourBackend
{
    [TestClass]
    public class BitmapToImageTest
    {
        [TestMethod]
        public void ColorOfPixelXY_must_return_the_right_values()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "braun.bmp");
            Stream testfile = File.OpenRead(path);
            var _testbitmap = (Bitmap)Bitmap.FromStream(testfile);

            var ret = Utils.BitmapToImage.ColorOfPixelXY(_testbitmap,395,244);

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
            var _testbitmap = (Bitmap)Bitmap.FromStream(testfile);

            var ret = Utils.BitmapToImage.GetPixelBytes(_testbitmap);
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
            var _testbitmap = (Bitmap)Bitmap.FromStream(testfile);

            var ret = Utils.BitmapToImage.CreateImagefromBitmap(_testbitmap);
            
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
    [TestClass]
    public class CommandTestFramesTest {

        [TestMethod]
        public void Constructor_must_work_as_expected() {

            Bitmap[] bitmaps = new Bitmap[3];
            String path;
            String path1;
            String path2;
            String path3;
            Stream testfile1;
            Stream testfile2;
            Stream testfile3;
            CommandTestFrames cmnd;

            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path1 = Path.Combine(path, "Resources");
            path1 = Path.Combine(path1, "TestFrames");
            path1 = Path.Combine(path1, "TestVideo_000.bmp");
            testfile1 = File.OpenRead(path1);

            bitmaps[0] = (Bitmap)Image.FromStream(testfile1);

            path2 = Path.Combine(path, "Resources");
            path2 = Path.Combine(path2, "TestFrames");
            path2 = Path.Combine(path2, "TestVideo_001.bmp");
            testfile2 = File.OpenRead(path2);

            bitmaps[1] = (Bitmap)Image.FromStream(testfile2);

            path3 = Path.Combine(path, "Resources");
            path3 = Path.Combine(path3, "TestFrames");
            path3 = Path.Combine(path3, "TestVideo_002.bmp");
            testfile3 = File.OpenRead(path3);

            bitmaps[2] = (Bitmap)Image.FromStream(testfile3);

            cmnd = new CommandTestFrames(bitmaps);
            CollectionAssert.AreEqual(cmnd.frames, bitmaps);
            Assert.AreEqual(cmnd.currentIdx, 0);
        }

        [TestMethod]
        public void ReturnAndSetNextFrame_must_work_as_expected() {
            Bitmap[] bitmaps = new Bitmap[3];
            String path;
            String path1;
            String path2;
            String path3;
            Stream testfile1;
            Stream testfile2;
            Stream testfile3;
            CommandTestFrames cmnd;

            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path1 = Path.Combine(path, "Resources");
            path1 = Path.Combine(path1, "TestFrames");
            path1 = Path.Combine(path1, "TestVideo_000.bmp");
            testfile1 = File.OpenRead(path1);

            bitmaps[0] = (Bitmap)Image.FromStream(testfile1);

            path2 = Path.Combine(path, "Resources");
            path2 = Path.Combine(path2, "TestFrames");
            path2 = Path.Combine(path2, "TestVideo_001.bmp");
            testfile2 = File.OpenRead(path2);

            bitmaps[1] = (Bitmap)Image.FromStream(testfile2);

            path3 = Path.Combine(path, "Resources");
            path3 = Path.Combine(path3, "TestFrames");
            path3 = Path.Combine(path3, "TestVideo_002.bmp");
            testfile3 = File.OpenRead(path3);

            bitmaps[2] = (Bitmap)Image.FromStream(testfile3);

            cmnd = new CommandTestFrames(bitmaps);

            Bitmap test = cmnd.ReturnAndSetNextFrame();
            Assert.AreEqual(bitmaps[0], test);
            Assert.AreEqual(cmnd.currentIdx, 1);
            cmnd.ReturnAndSetNextFrame();
            cmnd.ReturnAndSetNextFrame();
            Assert.AreEqual(cmnd.currentIdx, 0);
            Bitmap test2 = cmnd.ReturnAndSetNextFrame();
            Assert.AreEqual(bitmaps[0], test);
            Assert.AreEqual(cmnd.currentIdx, 1);
        }
        [TestMethod]
        public void ReturnAndSetPreviousFrame_must_work_as_expected()
        {
            Bitmap[] bitmaps = new Bitmap[3];
            String path;
            String path1;
            String path2;
            String path3;
            Stream testfile1;
            Stream testfile2;
            Stream testfile3;
            CommandTestFrames cmnd;

            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path1 = Path.Combine(path, "Resources");
            path1 = Path.Combine(path1, "TestFrames");
            path1 = Path.Combine(path1, "TestVideo_000.bmp");
            testfile1 = File.OpenRead(path1);

            bitmaps[0] = (Bitmap)Image.FromStream(testfile1);

            path2 = Path.Combine(path, "Resources");
            path2 = Path.Combine(path2, "TestFrames");
            path2 = Path.Combine(path2, "TestVideo_001.bmp");
            testfile2 = File.OpenRead(path2);

            bitmaps[1] = (Bitmap)Image.FromStream(testfile2);

            path3 = Path.Combine(path, "Resources");
            path3 = Path.Combine(path3, "TestFrames");
            path3 = Path.Combine(path3, "TestVideo_002.bmp");
            testfile3 = File.OpenRead(path3);

            bitmaps[2] = (Bitmap)Image.FromStream(testfile3);

            cmnd = new CommandTestFrames(bitmaps);

            Bitmap test1 = cmnd.ReturnAndSetPreviousFrame();
            Assert.AreEqual(bitmaps[0], test1);
            Assert.AreEqual(cmnd.currentIdx, 2);

            Bitmap test2 = cmnd.ReturnAndSetPreviousFrame();
            Assert.AreEqual(bitmaps[2], test2);
            Assert.AreEqual(cmnd.currentIdx, 1);
        }
        [TestMethod]
        public void GetCurrentFrame_must_work_as_expected()
        {
            Bitmap[] bitmaps = new Bitmap[3];
            String path;
            String path1;
            String path2;
            String path3;
            Stream testfile1;
            Stream testfile2;
            Stream testfile3;
            CommandTestFrames cmnd;

            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path1 = Path.Combine(path, "Resources");
            path1 = Path.Combine(path1, "TestFrames");
            path1 = Path.Combine(path1, "TestVideo_000.bmp");
            testfile1 = File.OpenRead(path1);

            bitmaps[0] = (Bitmap)Image.FromStream(testfile1);

            path2 = Path.Combine(path, "Resources");
            path2 = Path.Combine(path2, "TestFrames");
            path2 = Path.Combine(path2, "TestVideo_001.bmp");
            testfile2 = File.OpenRead(path2);

            bitmaps[1] = (Bitmap)Image.FromStream(testfile2);

            path3 = Path.Combine(path, "Resources");
            path3 = Path.Combine(path3, "TestFrames");
            path3 = Path.Combine(path3, "TestVideo_002.bmp");
            testfile3 = File.OpenRead(path3);

            bitmaps[2] = (Bitmap)Image.FromStream(testfile3);

            cmnd = new CommandTestFrames(bitmaps);

            Bitmap test = cmnd.GetCurrentFrame();
            Assert.AreEqual(bitmaps[0], test);
            Assert.AreEqual(cmnd.currentIdx, 0);
        }
        [TestMethod]
        public void Reset_must_work_as_expected()
        {
            Bitmap[] bitmaps = new Bitmap[3];
            String path;
            String path1;
            String path2;
            String path3;
            Stream testfile1;
            Stream testfile2;
            Stream testfile3;
            CommandTestFrames cmnd;

            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path1 = Path.Combine(path, "Resources");
            path1 = Path.Combine(path1, "TestFrames");
            path1 = Path.Combine(path1, "TestVideo_000.bmp");
            testfile1 = File.OpenRead(path1);

            bitmaps[0] = (Bitmap)Image.FromStream(testfile1);

            path2 = Path.Combine(path, "Resources");
            path2 = Path.Combine(path2, "TestFrames");
            path2 = Path.Combine(path2, "TestVideo_001.bmp");
            testfile2 = File.OpenRead(path2);

            bitmaps[1] = (Bitmap)Image.FromStream(testfile2);

            path3 = Path.Combine(path, "Resources");
            path3 = Path.Combine(path3, "TestFrames");
            path3 = Path.Combine(path3, "TestVideo_002.bmp");
            testfile3 = File.OpenRead(path3);

            bitmaps[2] = (Bitmap)Image.FromStream(testfile3);

            cmnd = new CommandTestFrames(bitmaps);

            Bitmap test = cmnd.ReturnAndSetNextFrame();
            Bitmap test2 = cmnd.ReturnAndSetNextFrame();
            cmnd.Reset();
            Assert.AreEqual(cmnd.currentIdx, 0);
        }
    }
    [TestClass]
    public class HelpForTesting
    {

    }
}
