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
        int filter = 5;
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

                    switch (filter)
                    {
                        case 0:
                            {
                                original = AberracionCromaticaFiltro(CurrentFrame.Bitmap);
                                break;   
                            }
                        case 1:
                            {
                                original = colorizarFiltro(CurrentFrame.Bitmap);
                                break; }
                        case 2:
                            {
                                original = contrasteFiltro(CurrentFrame.Bitmap);
                                break;
                            }
                        case 3:
                            {
                                original = ruidoFiltro(CurrentFrame.Bitmap);
                                break;
                            }
                        case 4:
                            {
                                original = TonosdeGrisFiltro(CurrentFrame.Bitmap);
                                break;
                            }
                        default:
                            {
                                original = CurrentFrame.Bitmap;

                                break;
                            }
                    }
                    
                    pictureBox1.Image = original;
                    trackBar1.Value = CurrentFrameNo;

                    if (filter >= 5)
                    {
                        CurrentFrameNo += 1;
                    }
                    else
                    {
                        CurrentFrameNo += 3;
                    }

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

        private void aberracionCromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = 0;
        }

        private Bitmap AberracionCromaticaFiltro(Bitmap Current)
        {
            int x = 0;
            int y = 0;
            int a = 8;

            int r = 0;
            int g = 0;
            int b = 0;

            for (x = 0; x < Current.Width; x++)
            {
                for (y = 0; y < Current.Height; y++)
                {
                    g = Current.GetPixel(x, y).G;

                    if (x + a < Current.Width)
                    {
                        r = Current.GetPixel(x + a, y).R;
                    }
                    else r = 0;
                    if (x - a >= 0)
                    {
                        b = Current.GetPixel(x - a, y).B;
                    }
                    else b = 0;

                    Current.SetPixel(x, y, Color.FromArgb(r, g, b));

                }
            }
            return Current;
        }

        private void colorizarVidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = 1;
        }
        
        private Bitmap colorizarFiltro(Bitmap current)
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
            TonosdeGrisFiltro(current);

            for (x = 0; x < current.Width; x++)
            {
                for (y = 0; y < current.Height; y++)
                {
                    miColor = current.GetPixel(x, y);

                    r = (int)(miColor.R * rc);
                    g = (int)(miColor.G * gc);
                    b = (int)(miColor.B * bc);

                    original.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return current;
        }

        private void contrasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = 2;
        }
        
        private Bitmap contrasteFiltro(Bitmap current)
        {
            //diferenciar entre areas mas iluminadas y más oscuras
            //el valor va de -100 a 100
            int contraste = 30;
            float c = (100.0f + contraste) / 100.0f;
            c *= c;
            int x = 0;
            int y = 0;

            Color colorResultante = new Color();
            Color colorOriginal = new Color();

            float r = 0;
            float g = 0;
            float b = 0;

            for (x = 0; x < current.Width; x++)
            {
                for (y = 0; y < current.Height; y++)
                {
                    //Get pixel color
                    colorOriginal = current.GetPixel(x, y);
                    //process and get the new colorxdd le salia el ingles de repente
                    r = ((((colorOriginal.R / 255.0f) - 0.5f) * c) + 0.5f) * 255;
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;

                    g = ((((colorOriginal.G / 255.0f) - 0.5f) * c) + 0.5f) * 255;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;

                    b = ((((colorOriginal.B / 255.0f) - 0.5f) * c) + 0.5f) * 255;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;

                    colorResultante = Color.FromArgb((int)r, (int)g, (int)b);
                    current.SetPixel(x, y, colorResultante);
                }
            }


            return current;
        }

        private void ruidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = 3;
        }

        private Bitmap ruidoFiltro(Bitmap current)
        {
            int x = 0;
            int y = 0;
            int porcentaje = 90;
            //0 a 200
            //colores entre estos rangos uguuu
            int rangoMin = 85;
            int rangoMax = 115;
            float pBrillo = 0;

            Random rnd = new Random();
            Color colorResultante;
            Color colorOriginal;

            int r = 0;
            int g = 0;
            int b = 0;

            for (x = 0; x < current.Width; x++)
            {
                for (y = 0; y < current.Height; y++)
                {
                    //Verificamos si el pixel tiene ruido o no
                    if (rnd.Next(1, 100) <= porcentaje)
                    {
                        //metodo 1 al color resultante le creamos un color al azar
                        colorResultante = Color.FromArgb(rnd.Next(rangoMin, rangoMax),
                            rnd.Next(rangoMin, rangoMax), rnd.Next(rangoMin, rangoMax));

                        //metodo2
                        pBrillo = rnd.Next(rangoMin, rangoMax) / 100.0f;
                        colorOriginal = current.GetPixel(x, y);
                        r = (int)(colorOriginal.R * pBrillo);
                        g = (int)(colorOriginal.G * pBrillo);
                        b = (int)(colorOriginal.B * pBrillo);

                        if (r > 255) r = 255;
                        else if (r < 0) r = 0;

                        if (g > 255) g = 255;
                        else if (g < 0) g = 0;

                        if (b > 255) b = 255;
                        else if (r < 0) b = 0;

                        colorResultante = Color.FromArgb(r, g, b);
                    }

                    else
                    {
                        colorResultante = current.GetPixel(x, y);
                    }

                    current.SetPixel(x, y, colorResultante);
                }
            }

            return current;
        }

        private void ToneGrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = 4;
        }

        private Bitmap TonosdeGrisFiltro(Bitmap current)
        {
       
            int x = 0;
            int y = 0;
            Color colorResultante = new Color(); //color que resulta
            Color colorOriginal = new Color(); //color que obtenemos de la imagen
            float g = 0;
            for (x = 0; x < current.Width; x++)
            {
                for (y = 0; y < current.Height; y++)
                {
                    //Obtnemos el color del pixel
                    colorOriginal = current.GetPixel(x, y);
                    g = colorOriginal.R * 0.299f + colorOriginal.G * 0.587f + colorOriginal.B * 0.114f;

                    colorResultante = Color.FromArgb((int)g, (int)g, (int)g);

                    current.SetPixel(x, y, colorResultante);
                }
            }

            return current;
        }
       
    }


}