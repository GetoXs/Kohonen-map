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
        int _neighbourhood;
        int _epochs;
        int _clusters;
        double _learnign_rate;
        Image _imgSrc;
        long lenStart;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "C:\\";
            //openFileDialog1.Filter = "Pliki BMP (*.bmp)|*.bmp";
            //openFileDialog1.FilterIndex = 2;
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
                            label9.Text = "Wymiary obrazka: " + bit.Width + " x " + bit.Height;
                            MemoryStream ms = new MemoryStream();
                            bit.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            lenStart = ms.Length;
                            label11.Text = "Rozmiar PNG: " + (lenStart / 1024).ToString() + "kB";
                            ms.Close();
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
          if (pictureBox1.Image == null)
          {
            MessageBox.Show("Wpierw wybierz obrazek");
            return;
          }
          
          _neighbourhood = Convert.ToInt32(numericUpDown3.Value);
          _epochs = Convert.ToInt32(numericUpDown1.Value);
          _clusters = 16;
          _learnign_rate = Convert.ToDouble(numericUpDown2.Value);
          _imgSrc = pictureBox1.Image;

          //MemoryStream ms = new MemoryStream();
          _imgSrc.Save("test.png", System.Drawing.Imaging.ImageFormat.Png);
//          long lenS = ms.Length;


          label10.Text = "Kompresja w toku";
          progressBar1.Visible = true;
          progressBar1.Maximum = _epochs;
          this.Enabled = false;
			    //nowy watek
			    Thread t = new Thread(new ThreadStart(SOMStart));
			    t.Start();
        }
      public void changeProgressBar(int status)
      {
        this.Invoke((MethodInvoker)delegate
        {
          progressBar1.Value = status;

        });
      }
		private void SOMStart()
		{
          DateTime startTime = DateTime.Now;
          KohonenMap.convert(this, _imgSrc, _neighbourhood, _epochs, _clusters, _learnign_rate);
          pictureBox2.Image = KohonenMap.outputImage;
          DateTime stopTime = DateTime.Now;
          TimeSpan roznica = stopTime - startTime;
          this.Invoke((MethodInvoker)delegate
          {
            label10.Text = "Kompresja zajęła: " + roznica.TotalSeconds + " sekund.";
            this.Enabled = true;
            progressBar1.Visible = false;
            MemoryStream ms = new MemoryStream();
            pictureBox2.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            label12.Text = "Rozmiar PNG: " + (ms.Length / 1024).ToString() + "kB";
            label13.Text = "Stopień kompresji: " + (((float)lenStart/ms.Length)).ToString(".00");
            ms.Close();
          });

          
		}
		

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            label8.Text = numericUpDown3.Value + " x " + Math.Truncate((float)numericUpDown3.Value * AspectRatio);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
          saveFileDialog1.Filter = "PNG|.png";
          if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
            pictureBox2.Image.Save(saveFileDialog1.FileName);
          }
        }
    }
}
