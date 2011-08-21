using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Kohonen
{
    public partial class Form1 : Form
    {
        String nazwapliku;
        float AspectRatio;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "Pliki BMP (*.bmp)|*.bmp";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            nazwapliku = openFileDialog1.FileName;
                            Bitmap bit = new Bitmap(nazwapliku);

                            pictureBox1.Image = bit;
                            label5.Text = "Pixel format: " + bit.PixelFormat.ToString();
                            label9.Text = "Rozmiar obrazka: " + bit.Width + " x " + bit.Height;
                            AspectRatio = ((float)bit.Height / (float)bit.Width);

                            label8.Text = numericUpDown3.Value + " x " + Math.Truncate((float)numericUpDown3.Value * AspectRatio);
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd odczytu pliku! " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
			//nowy watek
			Thread t = new Thread(new ThreadStart(SOMStart));
			t.Start();
            //pictureBox2.Image = mapa.bit;
        }
		private void SOMStart()
		{
            DateTime startTime = DateTime.Now;
            label10.Text = "Kompresja w toku";
            KohonenMap.convert(pictureBox1.Image, Convert.ToInt32(numericUpDown3.Value), Convert.ToInt32(numericUpDown1.Value), 16, Convert.ToDouble(numericUpDown2.Value));
            pictureBox2.Image = KohonenMap.outputImage;
            DateTime stopTime = DateTime.Now;
            TimeSpan roznica = stopTime - startTime;
            label10.Text = ("Kompresja zajęła:" + roznica.TotalSeconds +" sekund.");
		}
		

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            label8.Text = numericUpDown3.Value + " x " + Math.Truncate((float)numericUpDown3.Value * AspectRatio);
        }
    }
}
