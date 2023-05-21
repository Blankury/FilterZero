using Emgu.CV;
using Emgu.CV.Structure;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Emgu.CV.Face;
using System.Timers;
using System.IO;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace FilterZERO  
{
    public partial class Form3 : Form
    {
        private int faceCount = 0;

        //VideoCaptureDevice camara;
        FilterInfoCollection filterInfoCollection;

        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

        //Crear una lista para almacenar las caras de cada cuadro
        private bool OpenCamera = true;
        private Image<Gray, Byte> detectedFace = null;
        private List<FaceData> faceList = new List<FaceData>();

        private VectorOfMat imageList = new VectorOfMat();
        private List<string> namesList = new List<string>();
        private VectorOfInt labelList = new VectorOfInt();
        //private EigenFaceRecognizer recognizer;

        public Form3()
        {
            InitializeComponent();
        }

        private void BtnCapturar_Click(object sender, EventArgs e)
        {
            var camera = new VideoCapture(cmbCameras.SelectedIndex);

            if (!camera.IsOpened)
            {
                MessageBox.Show("No hay cámaras disponibles.");
                return;
            }

            var haarCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

            while (OpenCamera)
            {
                using (var frame = camera.QueryFrame().ToImage<Bgr, Byte>())
                {
                    if (frame == null)
                        break;

                    var grayFrame = frame.Convert<Gray, Byte>();
                    var faces = haarCascade.DetectMultiScale(grayFrame, 1.1, 3, Size.Empty);

                    foreach (var face in faces)
                    {
                        frame.Draw(face, new Bgr(Color.Orange), 2);
                        detectedFace = grayFrame.Copy(face).Convert<Gray, Byte>();
                    }

                    CameraBox.Image = frame.ToBitmap();

                    if (CvInvoke.WaitKey(1) >= 0) // Salir si se presiona una tecla
                        break;

                    int faceCount = faces.Length;
                    numFaces.Text = faceCount+"";
                }
            }
            camera.Dispose();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbCameras.Items.Add(filterInfo.Name);
            cmbCameras.SelectedIndex = 0;
            GetFacesList();

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            OpenCamera = false;
            CameraBox.Image = null;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            string personName = namePerson.Text;

            if (detectedFace == null)
            {
                MessageBox.Show("No hay rostros detectados.");
                return;
            }
            if (personName == "")
            {
                MessageBox.Show("Agregue un nombre.");
                return;
            }

            //Guardar rostro detectado
            detectedFace = detectedFace.Resize(100, 100, Inter.Cubic);
            detectedFace.Save("Faces\\" + "face" + (faceList.Count + 1) + ".bmp"); ;
            
            StreamWriter writer = new StreamWriter("Faces\\FaceList.txt", true);
           
            writer.WriteLine(String.Format("face{0}:{1}", (faceList.Count + 1), personName));
            writer.Close();

            GetFacesList();

            MessageBox.Show("Agregado.");
        }

        public void GetFacesList()
        {
            faceList.Clear();
            NameList.Items.Clear();

            string line;

            // Cree un directorio / archivo vacío para datos faciales si no existe
            if (!Directory.Exists("Faces\\"))
            {
                Directory.CreateDirectory("Faces\\");
            }

            if (!File.Exists("Faces\\FaceList.txt"))
            {
                string text = "No se puede encontrar el archivo de datos faciales:\n\n";
                text += "Faces\\FaceList.txt" + "\n\n";
                text += "Si es la primera vez que ejecuta la aplicación, se creará un archivo vacio para usted.";

                MessageBox.Show(text, "Aviso", MessageBoxButtons.OK);
               
                String dirName = Path.GetDirectoryName("Faces\\FaceList.txt");
                Directory.CreateDirectory(dirName);
                File.Create("Faces\\FaceList.txt").Close();

            }

            StreamReader reader = new StreamReader("Faces\\FaceList.txt");
            FaceData faceInstance = null;

            int i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] lineParts = line.Split(':');
                faceInstance = new FaceData();
                faceInstance.FaceImage = new Image<Gray, Byte>("Faces\\" + lineParts[0] + ".bmp");
                faceInstance.PersonName = lineParts[1];
                faceList.Add(faceInstance);
            }
            foreach (var face in faceList)
            {
                imageList.Push(face.FaceImage.Mat);
                namesList.Add(face.PersonName);
                labelList.Push(new[] { i++ });
                NameList.Items.Add(face.PersonName);
                namePerson.Text = String.Empty;
            }
            reader.Close();

        }


        private void NameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentSelection = NameList.SelectedItem.ToString();

            foreach (var face in faceList)                
            {
                if (face.PersonName == currentSelection)
                {
                    SelectedFace.Image = face.FaceImage.ToBitmap();
                }
            }
            
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCamera = false;
            CameraBox.Image = null;
            this.Close();
        }
    }
}
