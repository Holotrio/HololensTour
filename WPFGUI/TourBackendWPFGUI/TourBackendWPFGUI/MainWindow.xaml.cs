using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        public double x_marker = 0;
        public double y_marker = 0;

        public double windowHeight = 350;
        public double windowWidth = 525;

        public SyncObject syncObject;
        public Dictionary<int, CodeObject> CopyOfDict;// = new Dictionary<string, CodeObject>();
        public System.Int64 lasttimestamp;

        public CommandTestFrames frames;
        public bool framesactivated = false;

        public MainWindow()
        {/*
            Rectangle rect = new Rectangle(10, 10, 100, 100);
            Color red = Color.FromRgb(255, 0, 0);
            Brush brush = new SolidColorBrush(red);
            Pen pen = new Pen(brush, 1);
            Point point = new Point(0, 0);
            DrawingContext.DrawEllipse(brush, pen, point, 10, 10);
            */
            InitializeComponent();
            Canvas.SetBottom(Markerpointer2, 0);
            windowHeight = Height;
            windowWidth = Width;

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


            //Initialisierung SyncObject
            lasttimestamp = 10;
            syncObject = new SyncObject("syncid", new Dictionary<int, CodeObject>());
            syncObject.SetTimeStamp(lasttimestamp);
            CopyOfDict = new Dictionary<int, CodeObject>();

        }

        public void GetTourState()
        {
            if (syncObject.timestamp != lasttimestamp)
            {
                lasttimestamp = syncObject.timestamp;
                CopyOfDict = syncObject.dict;
                //ReadDictionaryData
                foreach (int objectid in CopyOfDict.Keys)
                {
                    //CodeObject with current key
                    CodeObject obj = CopyOfDict[objectid];

                    Marker4.Text = obj.id.ToString();

                    Markers.Items.Add(Marker4);
                }
            }
        }

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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("App is closing");
            this.Close();
        }

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
                Framebox.Source = BitmapToImageSource(new Bitmap(openFileDialog.FileName));
                frames = new CommandTestFrames(System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(openFileDialog.FileName)).Where(m => m.ToUpper().EndsWith("JPG") || m.ToUpper().EndsWith("BMP") || m.ToUpper().EndsWith("GIF") || m.ToUpper().EndsWith("PNG")));
                framesactivated = true;
            }
        }

        private void BtnMoveMarkerRight_Click(object sender, RoutedEventArgs e)
        {
            x_marker += 10;
            Canvas.SetLeft(Markerpointer2, x_marker);
            pos_x.Text = ((int)x_marker).ToString();
        }

        private void BtnMoveMarkerUp_Click(object sender, RoutedEventArgs e)
        {
            y_marker += 10;
            Canvas.SetBottom(Markerpointer2, y_marker);
            pos_y.Text = ((int)y_marker).ToString();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWindowHeight = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualHeight;
            double newWindowWidth = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth;

            x_marker = (x_marker * newWindowWidth) / windowWidth;
            y_marker = (y_marker * newWindowHeight) / windowHeight;

            Canvas.SetLeft(Markerpointer2, x_marker);
            pos_x.Text = ((int)x_marker).ToString();

            Canvas.SetBottom(Markerpointer2, y_marker);
            pos_y.Text = ((int)y_marker).ToString();

            windowHeight = newWindowHeight;
            windowWidth = newWindowWidth;
        }

        private void DisplayContentCodeObject()
        {

        }

        private void Markers_DropDownClosed(object sender, EventArgs e)
        {
            //here you change all the values of the codeobject
            MessageBox.Show(Markers.Text);
            //compare dict and get values

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (framesactivated && frames.length != 0)
            {
                Framebox.Source = BitmapToImageSource(frames.ReturnAndSetNextFrame());
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (framesactivated && frames.length != 0)
            {
                Framebox.Source = BitmapToImageSource(frames.ReturnAndSetPreviousFrame());
            }
        }
    }
}
