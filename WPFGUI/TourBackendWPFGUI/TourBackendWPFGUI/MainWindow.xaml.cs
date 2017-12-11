using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourBackend;

namespace TourBackendWPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        //***********************************************************
        //Parameters
        public double frameHeight = 190;
        public double frameWidth = 262.5;

        public double hololensWindowHeight = 504;
        public double hololensWindowWidth = 896;

        //Variables
        public double x_marker = 0;
        public double y_marker = 0;
        
        public double windowHeight = 350;
        public double windowWidth = 525;

        public SyncObject syncObject;
        public Dictionary<int, CodeObject> CopyOfDict;// = new Dictionary<string, CodeObject>();
        public System.Int64 lasttimestamp;

        public CameraFeedSyncObject cameraFeedSyncObject;
        public Bitmap bitmap;

        //will be added as soon as dll can be imported
        //public FrameWork frameWork;

        public CommandTestFrames frames;
        public bool framesactivated = false;

        public Stopwatch stopwatch;

        //***********************************************************

        //1) Startfunction
        public MainWindow()
        {/*
            Idea how to replace the Markerpointer from the xaml by the code behind

            Rectangle rect = new Rectangle(10, 10, 100, 100);
            Color red = Color.FromRgb(255, 0, 0);
            Brush brush = new SolidColorBrush(red);
            Pen pen = new Pen(brush, 1);
            Point point = new Point(0, 0);
            DrawingContext.DrawEllipse(brush, pen, point, 10, 10);
            */


            InitializeComponent();

            //Set Markerpointer to (0,0) will be replaced so that the markerpointer only shows when a detected marker was choosen
            Canvas.SetBottom(Markerpointer, 0);

            //Get the actual window size 
            windowHeight = Height;
            windowWidth = Width;

            
            /*
             
            Different ways to create a new item in the combobox
             
            ComboBoxItem comboBoxItem = new ComboBoxItem();
            comboBoxItem.Content = "id_1";
            Markers.Items.Add(comboBoxItem);
            ComboBoxItem comboBoxItem2 = new ComboBoxItem();
            comboBoxItem2.Content = "id_2";
            Markers.Items.Add(comboBoxItem2);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "id_3";
            Markers.Items.Add(textBlock);

            TextBlock textBlock2 = new TextBlock();
            textBlock2.Text = "id_4";
            Markers.Items.Insert(1, textBlock2);

            Marker4.Text = "id_265";
            */


            
            //creating stopwatch which will be used in framework
            stopwatch = new Stopwatch();
            stopwatch.Start();
            lasttimestamp = stopwatch.ElapsedMilliseconds;

            //Initialisierung SyncObject
            syncObject = new SyncObject("syncid", new Dictionary<int, CodeObject>());
            syncObject.SetTimeStamp(lasttimestamp);
            CopyOfDict = new Dictionary<int, CodeObject>();
            
            syncObject.SyncObjectUpdated += OnSyncObjectUpdated;

            cameraFeedSyncObject = new CameraFeedSyncObject("initial_id");

        }
        //***********************************************************

        //2) Framework related functions

        /// <summary>
        /// <para>WPF GUI to Framework</para>
        /// <para>Updates Camerfeedsyncobjects with the bitmap of the selected picture</para>
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_bitmap"></param>
        public void UpdateCamerFeedSyncObject(string _id, Bitmap _bitmap)
        {
            cameraFeedSyncObject.id = _id;
            cameraFeedSyncObject.timestamp = stopwatch.ElapsedMilliseconds;
            cameraFeedSyncObject.bitmap = bitmap;
            cameraFeedSyncObject.UpdateFrame();
        }

        /// <summary>
        /// <para>Framework to WPF GUI</para>
        /// <para>Event which will call GetTourState when SyncObject is updated</para>
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        protected void OnSyncObjectUpdated(object Sender, EventArgs e)
        {
            GetTourState();
        }

        //ToDo change to Event of changed Syncobject

        /// <summary>
        /// Gets the new tourstate and adds the detected markers to the combobox
        /// </summary>
        public void GetTourState()
        {
            if (syncObject.timestamp != lasttimestamp)
            {
                lasttimestamp = syncObject.timestamp;
                CopyOfDict = CopySyncDict.CopyInt(syncObject.dict);
                //ReadDictionaryData
                foreach (int objectid in CopyOfDict.Keys)
                {
                    //CodeObject with current key
                    CodeObject obj = CopyOfDict[objectid];

                    //Add items to dropdownlist 
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = obj.id.ToString();
                    Markers.Items.Add(textBlock);

                }
            }
        }
        
        //***********************************************************

        //3) Buttons

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                    "Portable Network Graphic (*.png)|*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                //here choose your file
                //Framebox.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                bitmap = new Bitmap(openFileDialog.FileName);
                Framebox.Source = BitmapToImageSource(bitmap);
                frames = new CommandTestFrames(System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(openFileDialog.FileName)).Where(m => m.ToUpper().EndsWith("JPG") || m.ToUpper().EndsWith("BMP") || m.ToUpper().EndsWith("GIF") || m.ToUpper().EndsWith("PNG")));
                framesactivated = true;
                UpdateCamerFeedSyncObject(openFileDialog.FileName, bitmap);

                //frameWork = new FrameWork(syncObject, cameraFeedSyncObject, new CodeObject[3]);
                //frameWork.Initialize();

            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("App is closing");
            this.Close();
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (framesactivated && frames.length != 0)
            {
                bitmap = frames.ReturnAndSetNextFrame();
                Framebox.Source = BitmapToImageSource(bitmap);
                UpdateCamerFeedSyncObject("nextframe_id", bitmap);
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (framesactivated && frames.length != 0)
            {
                Bitmap bitmap = frames.ReturnAndSetPreviousFrame();
                Framebox.Source = BitmapToImageSource(bitmap);
                UpdateCamerFeedSyncObject("previousframe_id", bitmap);
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (framesactivated && frames.length != 0)
            {
                frames.Reset();
                Bitmap bitmap = frames.GetCurrentFrame();
                Framebox.Source = BitmapToImageSource(bitmap);
                UpdateCamerFeedSyncObject("firstframe_id", bitmap);
            }
        }
        //***********************************************************

        //4) Displayfunction
        
        /// <summary>
        /// Sets the markerpointer on the frame depending on the size of the window
        /// </summary>
        /// <param name="position"></param>
        public void SetPointOnMarker(float[] position)
        {
            double xpos = position[0];
            double zpos = position[2];

            xpos = xpos * (windowWidth * 6f / 11f) / hololensWindowWidth;
            zpos = zpos * (windowHeight * 4f / 8f) / hololensWindowHeight;

            Canvas.SetLeft(Markerpointer, xpos);
            Canvas.SetBottom(Markerpointer, zpos);

        }

        //Event when a marker is selected in the combobox
        public void Markers_DropDownClosed(object sender, EventArgs e)
        {
            //here you change all the values of the codeobject
            //MessageBox.Show(Markers.Text);
            //compare dict and get values
            DisplayContentCodeObject();

        }

        /// <summary>
        /// Identify the selected marker and display the relevant data of the framework
        /// </summary>
        private void DisplayContentCodeObject()
        {
            //Compare Aruco ID with the chosen Marker -> basically parse string to int
            int id = -1;

            //Dear Moritz, I'd love to hear your way of solving this problem. I get from the Event Markers_DropDownClosed the text atm.
            //I didn't have time to add a int value to the marker. Sorry I was quite stressed and had a lot of things to do and didn't thing of that
            //but I know I can do it this way. <sys:Int32>5</sys:Int32>
            //I will change it tomorrow
            string stringid = Markers.Text;
            stringid = stringid.Substring(stringid.LastIndexOf('_') + 1);
            bool possibleToParse = Int32.TryParse(stringid, out id);

            if (possibleToParse)
            {
                CodeObject testobject = new CodeObject(id, new float[] { 350, 2, 290 }, new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

                if (!CopyOfDict.ContainsKey(id)) CopyOfDict.Add(id, testobject);

                if (CopyOfDict.ContainsKey(id))
                {
                    CodeObject obj = CopyOfDict[id];
                    pos_x.Text = obj.position[0].ToString();
                    pos_y.Text = obj.position[1].ToString();
                    pos_z.Text = obj.position[2].ToString();
                    SetPointOnMarker(obj.position);
                }
            }

        }

        //Event which updates the window size and changes the position of the markerpointer
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWindowHeight = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualHeight;
            double newWindowWidth = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth;

            x_marker = (x_marker * newWindowWidth) / windowWidth;
            y_marker = (y_marker * newWindowHeight) / windowHeight;

            Canvas.SetLeft(Markerpointer, x_marker);
            pos_x.Text = ((int)x_marker).ToString();

            Canvas.SetBottom(Markerpointer, y_marker);
            pos_y.Text = ((int)y_marker).ToString();

            windowHeight = newWindowHeight;
            windowWidth = newWindowWidth;

            object eventobject = new object();
            Markers_DropDownClosed(eventobject, EventArgs.Empty);
        }
        //***********************************************************

        //5) Additional needed functions

        /// <summary>
        /// Converts bitmap to a bitmapimage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


    }
}
