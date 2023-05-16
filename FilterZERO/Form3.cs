using Emgu.CV;
using Emgu.CV.Structure;
using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Emgu.CV.Face;
using System.Timers;
using System.IO;

namespace FilterZERO
{
    public partial class Form3 : Form
    {
        int faceCount = 0;

        VideoCaptureDevice camara;
        FilterInfoCollection filterInfoCollection;
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

        public Form3()
        {
            InitializeComponent();
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);

            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.4, 0);
            Rectangle[] faces = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 4);

            foreach (Rectangle rectangulo in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen lapiz = new Pen(Color.Yellow, 2))
                    {
                        graphics.DrawRectangle(lapiz, rectangulo);
                        graphics.Dispose();
                    }
                }
            }

            faceCount = rectangles.Length;

            CameraBox.Image = bitmap;

        }
        private void btnCapturar_Click(object sender, EventArgs e)
        {
            camara = new VideoCaptureDevice(filterInfoCollection[cmbCameras.SelectedIndex].MonikerString);
            camara.Start();
            camara.NewFrame += VideoCaptureDevice_NewFrame;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbCameras.Items.Add(filterInfo.Name);
            cmbCameras.SelectedIndex = 0;
            camara = new VideoCaptureDevice();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (camara.IsRunning == true)
            {
                camara.Stop();
            }
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            numFaces.Text = faceCount.ToString();
        }

        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (camara.IsRunning == true)
            {
                camara.Stop();
            }
            this.Close();
        }
    }
}
