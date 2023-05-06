using Emgu.CV;
using Emgu.CV.Structure;
using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FilterZERO
{
    public partial class Form3 : Form
    {

        VideoCaptureDevice camara;
        FilterInfoCollection filterInfoCollection;
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        bool camIsOn = false;
        public Form3()
        {
            InitializeComponent();
        }


        private void btnCapturar_Click(object sender, EventArgs e)
        {
            camara = new VideoCaptureDevice(filterInfoCollection[cmbCameras.SelectedIndex].MonikerString);
            camIsOn = true;
            camara.NewFrame += VideoCaptureDevice_NewFrame;
            camara.Start();
            //while (camIsOn)
            //{
                
            //}
            
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();


            CameraBox.Image = (Bitmap)eventArgs.Frame.Clone();


            Image<Rgb, byte> grayImage = new Image<Rgb, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.4, 0);

            foreach(Rectangle rectangulo in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen lapiz = new Pen(Color.YellowGreen, 1))
                        graphics.DrawRectangle(lapiz, rectangulo);
                    {
                    }

                }
            }

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
                camIsOn = false;

            }



        }

        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (camara.IsRunning == true)
            {
                camara.Stop();
                camIsOn = false;

            }
            this.Close();
        }
    }
}
