using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilterZERO
{
    public partial class Form1 : Form
    {
        //bitmap representa un bitmap o sea una img colocará la info de la imagen
        private Bitmap original;
        private Bitmap resultado;
        private Bitmap apilado;
        private string rutaAux;
        private int[] histograma = new int[256];
        private int[] histogramaR = new int[256];
        private int[] histogramaG = new int[256];
        private int[] histogramaB = new int[256];
        private int[,] conv3x3 = new int[3, 3];
        private int porcentaje;
        int contraste;

        public Form1()
        {
            InitializeComponent();
        }

        private void guardarImagenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                resultado.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void cargarImagenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                original = (Bitmap)(Bitmap.FromFile(openFileDialog1.FileName));
                rutaAux = openFileDialog1.FileName;
                resultado = original;
                apilado = resultado;
            }
            
            if (original!=null)
            {
                filtrosToolStripMenuItem.Enabled = true;
                pictureBox1.Image = resultado;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 NewWindow = new Form2();
            NewWindow.ShowDialog();
        }

        private void cámaraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 NewWindow = new Form3();
            NewWindow.ShowDialog();
        }
               
        private void colorizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            double rc = 120 / 255.0;
            double gc = 200 / 255.0;
            double bc = 120 / 255.0;
            Color miColor = new Color();
            int r, g, b;

            //Crear la imagen en tonos de gris
            tonosDeGrisToolStripMenuItem_Click(sender, e);

            for (int x = 0; x < apilado.Width; x++)
            {
                for (int y = 0; y < apilado.Height; y++)
                {
                    miColor = resultado.GetPixel(x, y);

                    r = (int)(miColor.R * rc);
                    g = (int)(miColor.G * gc);
                    b = (int)(miColor.B * bc);

                    resultado.SetPixel(x, y,  Color.FromArgb(r, g, b));
                }
            }

            pictureBox1.Image = resultado;
            apilado = resultado;
        }

        private void fContrast(object sender, EventArgs e, int Sporcentaje)
        {
            //diferenciar entre areas mas iluminadas y más oscuras
            //el valor va de -100 a 100
            float totalContraste = (100.0f + contraste) / 100.0f;
            totalContraste *= totalContraste;
            int x, y;

            Color colorResultante = new Color();
            Color colorOriginal = new Color();

            float r, g, b;

            for (x = 0; x < apilado.Width; x++)
            {
                for (y = 0; y < apilado.Height; y++)
                {
                    //Get pixel color
                    colorOriginal = apilado.GetPixel(x, y);
                    //process and get the new colorxdd le salia el ingles de repente
                    r = ((((colorOriginal.R / 255.0f) - 0.5f) * totalContraste) + 0.5f) * 255;
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;

                    g = ((((colorOriginal.G / 255.0f) - 0.5f) * totalContraste) + 0.5f) * 255;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;

                    b = ((((colorOriginal.B / 255.0f) - 0.5f) * totalContraste) + 0.5f) * 255;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;

                    colorResultante = Color.FromArgb((int)r, (int)g, (int)b);
                    resultado.SetPixel(x, y, colorResultante);
                }
            }


            pictureBox1.Image = resultado;
            apilado = resultado;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            contraste = -50;
            fContrast(sender, e, contraste);
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            contraste = 50;
            fContrast(sender, e, contraste);
        }

        private void invertirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //invertir colores de la imagen para sacar su negativo 
            int x = 0;
            int y = 0;
            Color colorResultante = new Color(); //color que resulta
            Color colorOriginal = new Color(); //color que obtenemos de la imagen

            for (x = 0; x < resultado.Width; x++)
            {
                for (y = 0; y < resultado.Height; y++)
                {
                    //obtener el color del pixel
                    colorOriginal = apilado.GetPixel(x, y);
                    //procesamos y obtenemos el nuevo color
                    colorResultante = Color.FromArgb(255 - colorOriginal.R, 255 - colorOriginal.G, 255 - colorOriginal.B);
                    //colocar el color en resultante
                    resultado.SetPixel(x, y, colorResultante);
                }
            }

            pictureBox1.Image = resultado;
            apilado = resultado;
        }

        private void mosaicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mosaico imagina que recorre un area de tal tamaño y saca el promedio de color de esa area
            int mosaico = 8;
            int xm = 0;
            int ym = 0;

            Color colorResultante;
            Color colorOriginal;

            //sumas
            int rs, bs, gs;

            int r, g, b;

            for (int x = 0; x < apilado.Width - mosaico; x +=mosaico)
            {
                for (int y = 0; y < apilado.Height - mosaico; y +=mosaico)
                {
                    rs = 0;
                    gs = 0;
                    bs = 0;
                    //promedio
                    for (xm = x; xm < (x + mosaico); xm++){
                        for (ym =y; ym < (y+ mosaico); ym++)
                        {
                            colorOriginal = apilado.GetPixel(xm, ym);
                            rs += colorOriginal.R;
                            gs += colorOriginal.G;
                            bs += colorOriginal.B;
                        }
                    }


                    r = rs / (mosaico * mosaico);
                    g = gs / (mosaico * mosaico);
                    b = bs / (mosaico * mosaico);
                    colorResultante = Color.FromArgb(r, g, b);

                    //Dibujar mosaico
                    for (xm = x; xm < (x + mosaico); xm++)
                    {
                        for (ym = y; ym < (y + mosaico); ym++)
                        {
                            resultado.SetPixel(xm, ym, colorResultante);
                        }
                    }
                }
            }

            pictureBox1.Image = resultado;
            apilado = resultado;
        }

        private void tonosDeGrisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color colorResultante;
            Color colorOriginal;

            float Grises; 

            for (int x = 0; x < apilado.Width; x++)
            {
                for (int y = 0; y < apilado.Height; y++)
                {
                    colorOriginal = apilado.GetPixel(x, y);
                    
                    Grises = colorOriginal.R * 0.267f + colorOriginal.G * 0.678f + colorOriginal.B * 0.0593f;
                    //el color de gris se pone 
                    colorResultante = Color.FromArgb((int)Grises, (int)Grises, (int)Grises);
                    resultado.SetPixel(x , y, colorResultante);
                }
            }

            pictureBox1.Image = resultado;
            apilado = resultado;
        }

        private void fRuido(object sender, EventArgs e, int Sporcentaje)
        {
            //a tal porcentaje de los pixeles se les aplicará el ruido
            //rango minimo de brillo y maximo
            //colores entre estos rangos uguuu
            int rangoMin = 50;
            int rangoMax = 195;
            float pBrillo;

            Random random = new Random();

            Color colorResultante;
            Color colorOriginal;

            int r, g, b;

            for (int x = 0; x < apilado.Width; x++)
            {
                for (int y = 0; y < apilado.Height; y++)
                {
                    //Verificamos si el pixel tendrá ruido o no al generar un numero al azar entre 1 y 100 si el num es
                    //menor o igual al porcentaje no generará ruido
                    if (random.Next(1, 100) <= Sporcentaje)
                    {
                        //metodo 1 al color resultante le creamos un color al azar
                        colorResultante = Color.FromArgb(random.Next(rangoMin, rangoMax),
                            random.Next(rangoMin, rangoMax), random.Next(rangoMin, rangoMax));

                        //metodo2
                        pBrillo = random.Next(rangoMin, rangoMax) / 100.0f;
                        colorOriginal = apilado.GetPixel(x, y);
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
                        colorResultante = apilado.GetPixel(x, y);
                    }

                    resultado.SetPixel(x, y, colorResultante);
                }
            }

            pictureBox1.Image = resultado;
            apilado = resultado;
        }

        private void ruido15_Click(object sender, EventArgs e)
        {
            porcentaje = 15;
            fRuido(sender, e, porcentaje);
        }

        private void ruido30_Click(object sender, EventArgs e)
        {
            porcentaje = 30;
            fRuido(sender, e, porcentaje);
        }

        private void ruido45_Click(object sender, EventArgs e)
        {
            porcentaje = 45;
            fRuido(sender, e, porcentaje);
        }

        private void ruido60_Click(object sender, EventArgs e)
        {
            porcentaje = 60;
            fRuido(sender, e, porcentaje);
        }

        private void ruido75_Click(object sender, EventArgs e)
        {
            porcentaje = 75;
            fRuido(sender, e, porcentaje);
        }

        private void ruido90_Click(object sender, EventArgs e)
        {
            porcentaje = 90;
            fRuido(sender, e, porcentaje);
        }

        private void Borrar_Filtros_Click_1(object sender, EventArgs e)
        {
            //cargar la misma imagen de antes desde la ruta guardada
            original = (Bitmap)(Bitmap.FromFile(rutaAux));
            resultado = original;
            apilado = resultado;
            pictureBox1.Image = original;

        }

        private void RGB_histogram_Click(object sender, EventArgs e)
        {
            Color colorOriginal = new Color();

            for (int x = 0; x < resultado.Width; x++)
            {
                for (int y = 0; y < resultado.Height; y++)
                {
                    //get pixel color
                    colorOriginal = resultado.GetPixel(x, y);
                    histogramaR[colorOriginal.R]++; histogramaG[colorOriginal.G]++; histogramaB[colorOriginal.B]++;

                    resultado.SetPixel(x, y, colorOriginal);
                }
            }
            Form4 NewWindow = new Form4(histogramaR, histogramaG, histogramaB);
            NewWindow.ShowDialog();
        }

        private void Gray_histogram_Click(object sender, EventArgs e)
        {
            //Crear la imagen en tonos de gris
            tonosDeGrisToolStripMenuItem_Click(sender, e);

            Color colorResultante = new Color();

            for (int x = 0; x < resultado.Width; x++)
            {
                for (int y = 0; y < resultado.Height; y++)
                {
                    colorResultante = resultado.GetPixel(x, y);
                    histograma[colorResultante.R]++;
                }
            }
            Form4 NewWindow = new Form4(histograma);
            NewWindow.ShowDialog();
        }
    }
}