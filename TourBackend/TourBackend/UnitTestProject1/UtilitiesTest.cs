using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Aruco;
using Emgu.CV;

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

        /// <summary>
        /// the idea here is just to create an dictionary with codeObjects to make it easier and more
        /// readable to test. Cause in every test you have to create the recognition Manager and therefore
        /// you have to make a dictionary
        /// </summary>
        [TestMethod]
        public void Create_New_Dictionary_successfully()
        {
            // check if an empty dictionary gets created correctly
            Dictionary<int,CodeObject> empty = Utils.HelpForTesting.CreateDictionaryForInitialization(0);
            Assert.AreEqual(0, empty.Count);

            // check if a dictionary of 10 codeObjects gets created correctly
            Dictionary<int, CodeObject> tenCO = Utils.HelpForTesting.CreateDictionaryForInitialization(10);
            Assert.AreEqual(10, tenCO.Count);

            // check if a dictionary of 3 codeObjects gets created correctly and takes on the right values
            Dictionary<int, CodeObject> threeCO = Utils.HelpForTesting.CreateDictionaryForInitialization(3);
            Assert.AreEqual(3, threeCO.Count);
            for(int i = 0; i < threeCO.Count; ++i)
            {
                Assert.AreEqual(true, threeCO.ContainsKey(i + 1));
                Assert.AreEqual(false, threeCO[i+1].isActive);
                Assert.AreEqual(1f + i, threeCO[i+1].position[0]);
                Assert.AreEqual(2f + i, threeCO[i+1].position[1]);
                Assert.AreEqual(3f + i, threeCO[i+1].position[2]);
                Assert.AreEqual(0.1f + i, threeCO[i+1].rotation[0]);
                Assert.AreEqual(0.1f + i, threeCO[i+1].rotation[1]);
                Assert.AreEqual(0.1f + i, threeCO[i+1].rotation[2]);
            }
        }

        /// <summary>
        /// this test is here to try the functions from emguCV out and see what types are the arguments
        /// and so on. Just to get a better feeling of what exactly the functions take and return
        /// </summary>
        [TestMethod]
        public void TestEmguCVEstimatePoseSingleMarker()
        {
            // first get an bitmap
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Resources");
            path = Path.Combine(path, "ArucoCode_ID_1_2_3_7_10.bmp");
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

            // here we extract the translation vector out of the tvecs mat return value
            // which stored the values as doubles in the mat
            // Moritz: darauf achten, dass die Dimensionen stimmen! 
            // Generell können in rvecs/tvecs beliebig viele Einträge stehen.
            int idSize = outIDs.Size;
            int tvecsRows = tvecs.Rows;
            if (idSize == tvecsRows)
            {
                for (int p = 0; p < tvecsRows; ++p)
                {
                        double[] _tvecs = new double[3];
                        tvecs.Row(p).CopyTo<double>(_tvecs);
                        for (int i = 0; i < _tvecs.Length; ++i)
                        {
                            Console.WriteLine("tvecs " + p + "= " + _tvecs[i]);
                        }
                }
            }
                
            // here we extract the roation vector out of the rvecs mat return value
            // which stored the values as doubles in the mat
            int rvecsRows = rvecs.Rows;
            if (idSize == rvecsRows)
            {
                for (int r = 0; r < rvecsRows; ++r)
                {
                    // first give out the rotation vector
                    double[] _rvecs = new double[3];
                    rvecs.Row(r).CopyTo<double>(_rvecs);
                    for (int i = 0; i < _rvecs.Length; ++i)
                    {
                        Console.WriteLine("rvecs " + r + "= " + _rvecs[i]);
                    }

                    // then create the outputmatrix
                    // here we create a rotation matrix out of the rotation vector, it is this a 3x3 matrix with
                    // 9 elements and it is like: rotMat(x,y) = _rotmat(x+y-2)
                    // which stored the values as doubles in the mat
                    double[] _rotMat = new double[9];
                    Mat rotMat = new Mat();
                    Emgu.CV.CvInvoke.Rodrigues(rvecs.Row(r), rotMat, null);
                    rotMat.CopyTo<double>(_rotMat);
                    for (int i = 0; i < _rotMat.Length; ++i)
                    {
                        double help = i / 3;
                        int index_x = (int)Math.Floor(help);
                        int index_y = i % 3;
                        Console.WriteLine("rotMat" + r + "(" + index_x + ", " + index_y + ") = " + _rotMat[i]);
                    }
                }
            }
        }
    }
}
