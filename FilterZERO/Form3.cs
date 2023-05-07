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
        Mat m1 = new Mat();
        System.Timers.Timer aTimer = new System.Timers.Timer();
        System.Timers.Timer hTimer = new System.Timers.Timer();
        VideoCapture capture;
        bool pause = false;
        bool haarDetect = false;
        int faceCount = 0;
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

        Image<Bgr, Byte> m_cam;

        FilterInfoCollection filterInfoCollection;

        public Form3()
        {
            InitializeComponent();
        }


        private void btnCapturar_Click(object sender, EventArgs e)
        {
            //camara = new VideoCaptureDevice(filterInfoCollection[cmbCameras.SelectedIndex].MonikerString);
            //camara.Start(); 
            //camara.NewFrame += VideoCaptureDevice_NewFrame;
            try
            {
                pause = false;
                capture = new VideoCapture();
                Mat m = new Mat();
                capture.Read(m);
                CameraBox1.Image = m;
                aTimer.Elapsed += new ElapsedEventHandler(imageCapture);
                aTimer.Interval = 500;
                aTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }


        private void imageCapture(object source, ElapsedEventArgs e)
        {

            try
            {
                while (!pause)
                {
                    capture.Read(m1);

                    if (!m1.IsEmpty)
                    {
                        CameraBox1.Image = m1;
                        double fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);

                    }
                    else
                    {
                        break;
                    }
                    if (haarDetect == true)
                    {
                        DetectFaceHaar();

                    }
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);

            }
            return;
        }

        public void DetectFaceHaar()
        {

            try
            {
                string facePath = "haarcascade_frontalface_default.xml";

                CascadeClassifier classifierFace = new CascadeClassifier(facePath);

                var imgGray = m1.ToImage<Bgr, Byte>().Convert<Gray, byte>().Clone();
                //Image<Gray, byte> imgGray = m1.ToImage<Bgr, Byte>().Convert<Gray, byte>().Clone();

                Rectangle[] faces = classifierFace.DetectMultiScale(imgGray, 1.1, 5);
                //Rectangle[] faces = cascadeClassifier.DetectMultiScale(imgGray, 1.1, 3, Size.Empty);

                m_cam = m1.ToImage<Bgr, Byte>();

                foreach (var face in faces)
                {
                    m_cam.Draw(face, new Bgr(0, 0, 255), 2);

                    imgGray.ROI = face;

                }
                faceCount = faces.Length;

                CameraBox2.Image = m_cam;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbCameras.Items.Add(filterInfo.Name);
            cmbCameras.SelectedIndex = 0;
            //camara = new VideoCaptureDevice();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (camara.IsRunning == true)
            //{
            //    camara.Stop();
            //}
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            haarDetect = true;
            numFaces.Text = faceCount.ToString();

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            pause = true;
        }

        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (camara.IsRunning == true)
            //{
            //    camara.Stop();
            //}
            //this.Close();
        }
    }
}
