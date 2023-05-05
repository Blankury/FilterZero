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
        MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 0.6d, 0.6d);
        HaarCascade faceDetected;
        //Image<Bgr, Byte> Frame;
        //Capture camera;
        //Image<Gray, byte> result;
        //Image<Gray, byte> TrainedFace = null;
        //Image<Gray, byte> grayFace = null;
        //List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        //List<string> labels = new List<string>();
        //List<string> usuarios = new List<string>();
        int contador, numLabels, t;
        string nombre, nombres = null;


        public Form3()
        {
            InitializeComponent();
            //haarcascade es para la detecion de rostros
            faceDetected = new HaarCascade("haarcascade_frontalface_default.xml");
            //try
            //{

            //    string Labelsinf = File.ReadAllText(Application.StartupPath + "/Faces/Faces.txt");
            //    string[] Labels = Labelsinf.Split(',');
            //    //el primer label antes, sera el numero de caras guardadas.
            //    numLabels = Convert.ToInt16(Labels[0]);
            //    contador = numLabels;
            //    string FacesLoad;
            //    for (int i = 1; i < numLabels + 1; i++)
            //    {
            //        FacesLoad = "caras" + i + ".bmp";
            //        trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/Faces/Faces.txt"));
            //        labels.Add(Labels[i]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //MessageBox.Show("Nada en la db.");
            //}

        }


        private void btnCapturar_Click(object sender, EventArgs e)
        {
            //camera = new Capture();
            //camera.QueryFrame();
            //Application.Idle += new EventHandler(FrameProcedure);
        }

        //private void FrameProcedure(object sender, EventArgs e)
        //{
        //    //Users.Add("");
        //    try
        //    {

        //        Frame = camera.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
        //        grayFace = Frame.Convert<Gray, Byte>();
        //        MCvAvgComp[][] facesDetectedNow = grayFace.DetectHaarCascade(faceDetected, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
        //        foreach (MCvAvgComp f in facesDetectedNow[0])
        //        {
        //            result = Frame.Copy(f.rect).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
        //            Frame.Draw(f.rect, new Bgr(Color.Green), 3);

        //            if (trainingImages.ToArray().Length != 0)
        //            {
        //                MCvTermCriteria termCriterias = new MCvTermCriteria(contador, 0.001);
        //                EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), 1500, ref termCriterias);
        //                nombre = recognizer.Recognize(result);
        //                Frame.Draw(Name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.Red));


        //            }

        //        }
        //        cameraBox.Image = Frame;
        //        nombres = "";
        //    }
        //    catch (Exception)
        //    {
        //        //
        //    }
            

        //}
    }
}

public partial class Videos : Form
{

    string VideoActual = "";
    string NombreVActual = "";
    VideoCapture videoCapture;
    bool Reproduciendo = false;
    int framesTotales;
    float framesActualNum;
    Mat frameActual;
    Bitmap bmpRes;
    int FPS;
    int filtro = 9;

    private void btnCargarVideo_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            VideoActual = openFileDialog.FileName;
            NombreVActual = openFileDialog.SafeFileName;
        }

    }

    private async void PlayVideo()
    {
        if (videoCapture == null)
        {
            return;
        }

        try
        {
            while (Reproduciendo == true && framesActualNum < framesTotales)
            {
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.PosFrames, framesActualNum);
                videoCapture.Read(frameActual);

                switch (filtro)
                {
                    case 0:
                        bmpRes = InvertirColores(frameActual.ToBitmap());
                        break;
                    default:
                        bmpRes = frameActual.ToBitmap();
                        break;
                }

                VideoF2.Image = bmpRes;
                VideoF2.SizeMode = PictureBoxSizeMode.StretchImage;
                VideoF1.Image = frameActual.ToBitmap();
                VideoF1.SizeMode = PictureBoxSizeMode.StretchImage;
                LineaT.Value = (int)framesActualNum;
                if (filtro >= 0 && filtro <= 4)
                {
                    framesActualNum += 2.5f;
                }
                else
                {
                    framesActualNum += 1.95f;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }

    unsafe public Bitmap InvertirColores(Bitmap bmap)
    {
        Bitmap imgOriginal;
        imgOriginal = new Bitmap(bmap);
        BitmapData bmpData = imgOriginal.LockBits(new Rectangle(0, 0, imgOriginal.Width, imgOriginal.Height), ImageLockMode.ReadWrite, imgOriginal.PixelFormat);
        int bytesPerPixel = Bitmap.GetPixelFormatSize(imgOriginal.PixelFormat) / 8;
        int heightInPixels = bmpData.Height;
        int widthInBytes = bmpData.Width * bytesPerPixel;
        byte* PtrFirstPixel = (byte*)bmpData.Scan0;

        VarGau.Visible = false;
        VarGau.Maximum = heightInPixels;
        VarGau.Value = 0;

        for (int y = 0; y < heightInPixels; y++)
        {
            byte* currentLine = PtrFirstPixel + (y * bmpData.Stride);
            for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
            {
                currentLine[x] = (byte)(255 - currentLine[x]); // blue
                currentLine[x + 1] = (byte)(255 - currentLine[x + 1]); // green
                currentLine[x + 2] = (byte)(255 - currentLine[x + 2]); // red
            }
            VarGau.Value++;
        }

        imgOriginal.UnlockBits(bmpData);
        VarGau.Visible = false;
        return imgOriginal;
    }

    private void btnAplicarFiltroV_Click(object sender, EventArgs e)
    {
        int selectedIndex = cmbFiltrosV.SelectedIndex;
        switch (selectedIndex)
        {
            case 0: //Invertir colores
                filtro = 0;
                break;
            case 1: //Tono Sepia
                filtro = 1;
                break;
            case 2: //Escala de Grises
                filtro = 2;
                break;
            case 3: //Pixelizado
                filtro = 3;
                break;
            case 4: //
                filtro = 4;
                break;
            default:
                MessageBox.Show("Por favor seleccione un filtro.");
                break;
        }
    }
}
