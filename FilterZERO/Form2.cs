using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

using System.Drawing.Imaging;
using FFmpeg.AutoGen;
using System.Media;

namespace FilterZERO
{
    public partial class Form2 : Form
    {

        VideoCapture capture;
        bool IsPlaying = false;
        int TotalFrames;
        Mat CurrentFrame; 
        int CurrentFrameNo;
        int FPS;
        private Bitmap original;
        private Bitmap resultado;

        public Form2()
        {
            InitializeComponent();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cargarImagenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                capture = new VideoCapture(openFileDialog1.FileName);
                TotalFrames = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount));
                FPS = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
                IsPlaying = true;
                CurrentFrame = new Mat ();
                CurrentFrameNo = 0;
                trackBar1.Minimum = 0;
                trackBar1.Maximum = TotalFrames -1;
                trackBar1.Value = 0;
                PlayVideo();

            }

            if (capture != null)
            {
                filtrosToolStripMenuItem.Enabled = true;
                //pictureBox1.Image = resultado;
            }
        }

        private async void PlayVideo()
        {
            if (capture == null)
            {
                return;
            }
            try
            {
                while(IsPlaying == true && CurrentFrameNo < TotalFrames)
                {
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, CurrentFrameNo);
                    capture.Read(CurrentFrame);
                    pictureBox1.Image = CurrentFrame.Bitmap;
                    trackBar1.Value = CurrentFrameNo;
                    CurrentFrameNo += 1;
                    await Task.Delay(1000 / FPS);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnPause_Click(object sender, EventArgs e)
        {
            IsPlaying = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            IsPlaying = false;
            CurrentFrameNo = 0;
            trackBar1.Value = 0;
            pictureBox1.Image = null;
            pictureBox1.Invalidate();

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                IsPlaying = true;
                PlayVideo();
            }
            else
            {
                IsPlaying = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (capture !=null)
            {
                CurrentFrameNo = trackBar1.Value;
            }
        }

        private void ruidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            original = CurrentFrame.ConvertTo();

            Bitmap bitmap; //This is your bitmap
            Image<Bgr, byte> imageCV = new Image<Bgr, byte>(bitmap); //Image Class from Emgu.CV
            Mat mat = imageCV.Mat; //This is your Image converted to Mat

            int x = 0;
            int y = 0;
            int porcentaje = 90;
            //0 a 200
            //colores entre estos rangos uguuu
            int rangoMin = 85;
            int rangoMax = 115;
            float pBrillo = 0;

            Random rnd = new Random();
            Color rColor;
            Color oColor;

            int r = 0;
            int g = 0;
            int b = 0;

            for (x = 0; x < original.Width; x++)
            {
                for (y = 0; y < original.Height; y++)
                {
                    //Verificamos si el pixel tiene ruido o no
                    if (rnd.Next(1, 100) <= porcentaje)
                    {
                        //metodo 1 al color resultante le creamos un color al azar
                        rColor = Color.FromArgb(rnd.Next(rangoMin, rangoMax),
                            rnd.Next(rangoMin, rangoMax), rnd.Next(rangoMin, rangoMax));

                        //metodo2
                        pBrillo = rnd.Next(rangoMin, rangoMax) / 100.0f;
                        oColor = original.GetPixel(x, y);
                        r = (int)(oColor.R * pBrillo);
                        g = (int)(oColor.G * pBrillo);
                        b = (int)(oColor.B * pBrillo);

                        if (r > 255) r = 255;
                        else if (r < 0) r = 0;

                        if (g > 255) g = 255;
                        else if (g < 0) g = 0;

                        if (b > 255) b = 255;
                        else if (r < 0) b = 0;

                        rColor = Color.FromArgb(r, g, b);
                    }

                    else
                    {
                        rColor = original.GetPixel(x, y);
                    }

                    original.SetPixel(x, y, rColor);
                }
            }

            pictureBox1.Image = original;
        }


        private void contrasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //diferenciar entre areas mas iluminadas y más oscuras
            //el valor va de -100 a 100
            int contraste = 30;
            float c = (100.0f + contraste) / 100.0f;
            c *= c;
            int x = 0;
            int y = 0;

            Color rColor = new Color();
            Color oColor = new Color();

            float r = 0;
            float g = 0;
            float b = 0;

            for (x = 0; x < original.Width; x++)
            {
                for (y = 0; y < original.Height; y++)
                {
                    //Get pixel color
                    oColor = original.GetPixel(x, y);
                    //process and get the new colorxdd le salia el ingles de repente
                    r = ((((oColor.R / 255.0f) - 0.5f) * c) + 0.5f) * 255;
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;

                    g = ((((oColor.G / 255.0f) - 0.5f) * c) + 0.5f) * 255;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;

                    b = ((((oColor.B / 255.0f) - 0.5f) * c) + 0.5f) * 255;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;

                    rColor = Color.FromArgb((int)r, (int)g, (int)b);
                    original.SetPixel(x, y, rColor);
                }
            }


            pictureBox1.Image = original;
        }

        private void colorizarVidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Tomar los componentes RGB y los transforma en porcentajes para saber la cantidad de RGB de cada pixel
            //Al conocerlos recorremos pixel por pixlel la imagen transformada en grises y vamos a multiplicar los porcentajes por el gris correspondiente
            //Asi obtenemos las variaciones de color

            int x = 0;
            int y = 0;
            //queremos colorizar con (120, 200, 120)
            double rc = 120 / 255.0;
            double gc = 200 / 255.0;
            double bc = 120 / 255.0;

            Color miColor = new Color();
            int r = 0;
            int g = 0;
            int b = 0;

            //Crear la imagen en tonos de gris
            ToneGrayToolStripMenuItem_Click(sender, e);

            for (x = 0; x < original.Width; x++)
            {
                for (y = 0; y < original.Height; y++)
                {
                    miColor = original.GetPixel(x, y);

                    r = (int)(miColor.R * rc);
                    g = (int)(miColor.G * gc);
                    b = (int)(miColor.B * bc);

                    original.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            pictureBox1.Image = original;

        }

        private void ToneGrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            Color rColor = new Color(); //color que resulta
            Color oColor = new Color(); //color que obtenemos de la imagen
            float g = 0;
            for (x = 0; x < original.Width; x++)
            {
                for (y = 0; y < original.Height; y++)
                {
                    //Obtnemos el color del pixel
                    oColor = original.GetPixel(x, y);
                    g = oColor.R * 0.299f + oColor.G * 0.587f + oColor.B * 0.114f;

                    rColor = Color.FromArgb((int)g, (int)g, (int)g);

                    original.SetPixel(x, y, rColor);
                }
            }

            pictureBox1.Image = original;
        }

        private void aberracionCromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            int a = 8;

            int r = 0;
            int g = 0;
            int b = 0;

            resultado = new Bitmap(original.Width, original.Height);

            for (x = 0; x < original.Width; x++)
            {
                for (y = 0; y < original.Height; y++)
                {
                    g = original.GetPixel(x, y).G;
                    //g = original.GetPixel(x, y).G;
                    //g = original.GetPixel(x, y).G;

                    if (x + a < original.Width)
                    {
                        r = original.GetPixel(x + a, y).R;
                    }
                    else r = 0;
                    if (x - a >= 0)
                    {
                        b = original.GetPixel(x - a, y).B;
                    }
                    else b = 0;

                    resultado.SetPixel(x, y, Color.FromArgb(r,g,b));

                }
            }
        }
    }


}