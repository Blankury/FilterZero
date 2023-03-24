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
    public partial class Form4 : Form
    {
        private int[] histograma;
        private int mayor;

        public Form4(int[] pHistograma )
        {
            InitializeComponent();

            histograma = pHistograma;
            int n = 0;
            //Encontramos cual es el valor que tiene la mayor cantidad de pixeles
            mayor = 0;
            for (n=0; n < 256; n++)
            {
                if (histograma[n] > mayor)
                    mayor = histograma[n];
            }


            //llevar a cabo un escalamiento para que el histograma no ocupe tanto espacio
            for (n = 0; n < 256; n++)            
                histograma[n] = (int)((float)histograma[n] / (float)mayor * 256.0f);                
        }

        private void Form4_Paint(object sender, PaintEventArgs e)
        {
            int n = 0;
            int altura = 0;
            Graphics g = e.Graphics;
            Pen plumaH = new Pen(Color.Black);
            Pen plumaEjes = new Pen(Color.Coral);

            g.DrawLine(plumaEjes, 19, 271, 277, 271);
            g.DrawLine(plumaEjes, 19, 270, 19, 14);

            for (n=0; n < 256; n++)
            {
                
                g.DrawLine(plumaH, n+20, 270, n+20, 270-histograma[n]);
            }
        }
    }
}
