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

        //VideoCaptureDevice camara;
        FilterInfoCollection filterInfoCollection;
        //static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

        // Create a list to store the faces for each frame
        private List<Rectangle> faceRectangles = new List<Rectangle>();
        private bool OpenCamera = true;
        public Form3()
        {
            InitializeComponent();

            
        }

        private void btnCapturar_Click(object sender, EventArgs e)
        {

            var camara = new VideoCaptureDevice(filterInfoCollection[cmbCameras.SelectedIndex].MonikerString);
           

            var capture = new VideoCapture(cmbCameras.SelectedIndex); // Abrir la cámara por defecto
            if (!capture.IsOpened)
            {
                MessageBox.Show("No se encontraron dispositivos de video.");
                return;
            }

            var faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

            while (OpenCamera)
            {
                using (var frame = capture.QueryFrame().ToImage<Bgr, byte>())
                {
                    if (frame == null)
                        break;

                    var grayFrame = frame.Convert<Gray, byte>();
                    var faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 3, Size.Empty);

                    foreach (var face in faces)
                    {
                        frame.Draw(face, new Bgr(Color.Orange), 2);
                    }

                    CameraBox.Image = frame.ToBitmap();

                    if (CvInvoke.WaitKey(1) >= 0) // Salir si se presiona una tecla
                        break;

                    int faceCount = faces.Length;
                    numFaces.Text = faceCount+"";
                }
            }
            capture.Dispose();

        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = new Bitmap((Bitmap)eventArgs.Frame.Clone());
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);

            Rectangle[] faces = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);

            // Add the faces to the list
            faceRectangles.AddRange(faces);

            // Keep only the faces from the last 10 frames
            if (faceRectangles.Count > 1)
            {
                faceRectangles.RemoveRange(0, faceRectangles.Count - 1);
            }

            foreach (Rectangle face in faceRectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, face);
                    }
                }
            }

            CameraBox.Image = bitmap;

            int faceCount = faces.Length;
        }





        private void Form3_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbCameras.Items.Add(filterInfo.Name);
            cmbCameras.SelectedIndex = 0;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            OpenCamera = false;
            CameraBox.Image = null;
        }


        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCamera = false;
            CameraBox.Image = null;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string name = namePerson.Text;
            MessageBox.Show(name);


        }
    }
}
