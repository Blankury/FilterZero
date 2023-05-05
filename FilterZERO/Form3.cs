using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilterZERO
{
    public partial class Form3 : Form
    {
        //MCvFont font = new MCvFont(Emgu.CV.CvEnum.FontFace.HersheyTriplex, 0.6d, 0.6d);
        CascadeClassifier faceDetected;
        Image<Bgr, Byte> Frame;
        VideoCapture camera;
        Image<Gray, byte> result;
        Image<Gray, byte> TrainedFace = null;
        Image<Gray, byte> grayFace = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> usuarios = new List<string>();
        int contador, numLabels, t;
        string nombre, nombres = null;


        public Form3()
        {
            InitializeComponent();
            //CascadeClassifier es para la detecion de rostros
            faceDetected = new CascadeClassifier("haarcascade_frontalface_default.xml");
            try
            {
                string Labelsinf = File.ReadAllText(Application.StartupPath + "/Faces/Faces.txt");
                string[] Labels = Labelsinf.Split(',');
                //el primer label antes, sera el numero de caras guardadas.
                numLabels = Convert.ToInt16(Labels[0]);
                contador = numLabels;
                string FacesLoad;
                for (int i = 1; i < numLabels + 1; i++)
                {
                    FacesLoad = "caras" + i + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/Faces/Faces.txt"));
                    labels.Add(Labels[i]);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Nada en la db.");
            }

        }


        private void btnCapturar_Click(object sender, EventArgs e)
        {
            camera = new VideoCapture();
            camera.QueryFrame();
            Application.Idle += new EventHandler(FrameProcedure);

        }

        private void FrameProcedure(object sender, EventArgs e)
        {
                //Users.Add("");
                try
            {

                //Frame = camera.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.Inter Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //Frame = camera.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.Inter.Cubic);

                //grayFace = Frame.Convert<Gray, Byte>();
                //MCvAvgComp[][] facesDetectedNow = grayFace.DetectHaarCascade(faceDetected, 1.2, 10, Emgu.CV.CvEnum.HaarDetectionType.DoCannyPruning, new Size(20, 20));
                //foreach (MCvAvgComp f in facesDetectedNow[0])
                //{
                //    result = Frame.Copy(f.Rect).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
                //    Frame.Draw(f.Rect, new Bgr(Color.Green), 3);

                //    if (trainingImages.ToArray().Length != 0)
                //    {
                //        MCvTermCriteria termCriterias = new MCvTermCriteria(contador, 0.001);
                //        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), 1500, ref termCriterias);
                //        nombre = recognizer.Recognize(result);
                //        Frame.Draw(Name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.Red));


                //    }

                //}
                cameraBox.Image = Frame;
                nombres = "";
            }
            catch (Exception)
            {
                //
            }


        }
    }
}
