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
        bool lbpDetect = false;

        int X = 0, Y = 0;

        Image<Bgr, Byte> m_cam;

        //VideoCaptureDevice camara;
        FilterInfoCollection filterInfoCollection;
        //Mat m1 = new Mat();
        //public string FaceName;

        ////private List<Rectangle> faceRectangles = new List<Rectangle>();
        //int faceCount = 0;
        //private Image<Rgb, byte> bgrFrame = null;
        //private Image<Gray, Byte> detectedFace = null;

        //Mat m1 = new Mat();
        //System.Timers.Timer aTimer = new System.Timers.Timer();
        //System.Timers.Timer hTimer = new System.Timers.Timer();

        //VideoCapture capture;
        //bool pause = false;
        //bool haarDetect = false;
        //bool lbpDetect = false;

        //int X = 0, Y = 0;

        //Image<Bgr, Byte> m_cam;

        public Form3()
        {
            InitializeComponent();
        }


        private void btnCapturar_Click(object sender, EventArgs e)
        {
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

            //camara = new VideoCaptureDevice(filterInfoCollection[cmbCameras.SelectedIndex].MonikerString);
            //camara.Start(); 
            //camara.NewFrame += VideoCaptureDevice_NewFrame;



            //numFaces.Text = faceCount.ToString();


            //try
            //{
            //    pause = false;
            //    capture = new VideoCapture();
            //    Mat m = new Mat();
            //    capture.Read(m);
            //    CameraBox.Image = m;
            //    //aTimer.Elapsed += new ElapsedEventHandler(imageCapture);
            //    aTimer.Interval = 500;
            //    aTimer.Enabled = true;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);

            //}
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
                string facePath = Path.GetFullPath("haarcascade_frontalface_default.xml");

                CascadeClassifier classifierFace = new CascadeClassifier(facePath);

                var imgGray = m1.ToImage<Bgr, Byte>().Convert<Gray, byte>().Clone();
                Rectangle[] faces = classifierFace.DetectMultiScale(imgGray, 1.1, 5);
                m_cam = m1.ToImage<Bgr, Byte>();

                foreach (var face in faces)
                {
                    m_cam.Draw(face, new Bgr(0, 0, 255), 2);

                    imgGray.ROI = face;

                }
                CameraBox2.Image = m_cam;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }


        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            //Image<Rgb, byte> grayImage = new Image<Rgb, byte>(bitmap);

            //bgrFrame = new Image<Rgb, byte>(bitmap);
            //Image<Gray, byte> grayframe = bgrFrame.Convert<Gray, byte>();
            ////Rectangle[] faces = cascadeClassifier.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));
            ////Rectangle[] faces = cascadeClassifier.DetectMultiScale(grayframe, 1.4, 0);
            //Rectangle[] faces = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, Size.Empty);

            ////detect face
            //FaceName = "No face detected";
            //foreach (var face in faces)
            //{
            //    bgrFrame.Draw(face, new Rgb(255, 255, 0), 2);
            //    detectedFace = bgrFrame.Copy(face).Convert<Gray, byte>();
                
            //}
            //faceCount = faces.Length;
            //if (faceCount > 0)
            //{
            //    MessageBox.Show(faceCount.ToString());

            //}
            //CameraBox.Image = bgrFrame.ToBitmap();

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
