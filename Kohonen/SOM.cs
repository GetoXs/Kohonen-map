using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Kohonen
{
    public class SOM
    {
        private Neuron[,] Wagi;     // Wagi neuronów
        private int Iteracja;       // Obecna iteracja.
        private int SzerWyjscia;    // Rozmiar siatki wyjściowej.
        private int Iteracji;       // Liczba iteracji.
        private double Lambda;      // Współczynnik uczenia się.
        private Random rnd = new Random(); // Zmienna losowa.
        public Bitmap bit;          // Mapa bitowa z plikiem wynikowym.

        private List<double[]> Schematy = new List<double[]>();


        public SOM(int SzerWyjscia, int Iteracji, double Lambda, string File)
        {
            this.SzerWyjscia = SzerWyjscia;
            this.Iteracji = Iteracji;
            this.Lambda = Lambda;
            Inicjalizacja();
            LadujDane(File);
            Normalizacja();
            Nauka(0.0000001);
            Finalizacja();
            this.bit = Image();
        }

        private void Inicjalizacja()
        {
            Wagi = new Neuron[SzerWyjscia, SzerWyjscia];
            for (int i = 0; i < SzerWyjscia; i++)
            {
                for (int j = 0; j < SzerWyjscia; j++)
                {
                    Wagi[i, j] = new Neuron(i, j, SzerWyjscia);
                    Wagi[i, j].Wagi = new double[3];
                    for (int k = 0; k < 3; k++)
                    {
                        Wagi[i, j].Wagi[k] = rnd.NextDouble();
                    }
                }
            }
        }

        private void LadujDane(string file) //przerobić na BMP - na razie średnio działa
        {
            Bitmap MapaWejsciowa = new Bitmap(file);
            double[] Wejscia = new double[3];

            int x, y;

            for (x = 0; x < MapaWejsciowa.Width; x++)
            {
                for (y = 0; y < MapaWejsciowa.Height; y++)
                {
                    Color pixelColor = MapaWejsciowa.GetPixel(x, y);

                    Wejscia[0] = pixelColor.R/256;
                    Wejscia[1] = pixelColor.G/256;
                    Wejscia[2] = pixelColor.B/256;
                    Schematy.Add(Wejscia);
                }
            }
        }

        private void Normalizacja()
        {
            for (int j = 0; j < 3; j++)
            {
                double Suma = 0;
                for (int i = 0; i < Schematy.Count; i++)
                {
                    Suma += Schematy[i][j];
                }
                double Srednia = Suma / Schematy.Count;
                for (int i = 0; i < Schematy.Count; i++)
                {
                    Schematy[i][j] = Schematy[i][j] / Srednia;
                }
            }
        }

        private void Nauka(double maxError)
        {
            double ObecnyBlad = double.MaxValue;
            while (ObecnyBlad > maxError)
            {
                ObecnyBlad = 0;
                List<double[]> TrainingSet = new List<double[]>();
                foreach (double[] Wzor in Schematy)
                {
                    TrainingSet.Add(Wzor);
                }
                for (int i = 0; i < Schematy.Count; i++)
                {
                    double[] Wzor = TrainingSet[rnd.Next(Schematy.Count - i)];
                    ObecnyBlad += SchematUczenia(Wzor);
                    TrainingSet.Remove(Wzor);
                }
                Console.WriteLine(ObecnyBlad.ToString("0.0000000"));
            }
        }

        private double SchematUczenia(double[] Wzor)
        {
            double Error = 0;
            Neuron Zwyciezca = Winner(Wzor);
            for (int i = 0; i < SzerWyjscia; i++)
            {
                for (int j = 0; j < SzerWyjscia; j++)
                {
                    Error += Wagi[i, j].AktualizujWagi(Wzor, Zwyciezca, Iteracja);
                }
            }
            Iteracja++;
            return Math.Abs(Error / (SzerWyjscia * SzerWyjscia));
        }

        private void Finalizacja()
        {
            for (int i = 0; i < Schematy.Count; i++)
            {
                Neuron n = Winner(Schematy[i]);
            }
        }

        private Neuron Winner(double[] Wzor)
        {
            Neuron Zwyciezca = null;
            double Minimum = double.MaxValue;
            for (int i = 0; i < SzerWyjscia; i++)
                for (int j = 0; j < SzerWyjscia; j++)
                {
                    double d = Odleglosc(Wzor, Wagi[i, j].Wagi);
                    if (d < Minimum)
                    {
                        Minimum = d;
                        Zwyciezca = Wagi[i, j];
                    }
                }
            return Zwyciezca;
        }

        private double Odleglosc(double[] Wektor1, double[] Wektor2)
        {
            double Wartosc = 0;
            for (int i = 0; i < Wektor1.Length; i++)
            {
                Wartosc += Math.Pow((Wektor1[i] - Wektor2[i]), 2);
            }
            return Math.Sqrt(Wartosc);
        }
        public Bitmap Image()
        {
            Bitmap Obraz = new Bitmap(SzerWyjscia,SzerWyjscia);

            for (int i = 0; i < SzerWyjscia; i++)
            {
                for (int j = 0; j < SzerWyjscia; j++)
                {
                    double r, g, b;

                    r = Wagi[i, j].Wagi[0];
                    g = Wagi[i, j].Wagi[1];
                    b = Wagi[i, j].Wagi[2];

                    Color kolor = Color.FromArgb(Convert.ToInt32(r*256), Convert.ToInt32(g*256), Convert.ToInt32(b*256));
                    Obraz.SetPixel(i, j, kolor);
                }
            }

            return Obraz;
        }
    }

    public class Neuron
    {
        public double[] Wagi;
        public int X;
        public int Y;
        private int Dlugosc;
        private double nf;

        public Neuron(int x, int y, int Dlugosc)
        {
            X = x;
            Y = y;
            this.Dlugosc = Dlugosc;
            nf = 1000 / Math.Log(Dlugosc);
        }

        private double Gauss(Neuron Zwyciezca, int Iteracja)
        {
            double Odleglosc = Math.Sqrt(Math.Pow(Zwyciezca.X - X, 2) + Math.Pow(Zwyciezca.Y - Y, 2));
            return Math.Exp(-Math.Pow(Odleglosc, 2) / (Math.Pow(Sila(Iteracja), 2)));
        }

        private double SzybkoscNauki(int Iteracja)
        {
            return Math.Exp(-Iteracja / 1000) * 0.1;
        }

        private double Sila(int Iteracja)
        {
            return Math.Exp(-Iteracja / nf) * Dlugosc;
        }

        public double AktualizujWagi(double[] Wzor, Neuron Zwyciezca, int Iteracja)
        {
            double Suma = 0;
            for (int i = 0; i < Wagi.Length; i++)
            {
                double Delta = SzybkoscNauki(Iteracja) * Gauss(Zwyciezca, Iteracja) * (Wzor[i] - Wagi[i]); //tutaj null
                Wagi[i] += Delta;
                Suma += Delta;
            }
            return Suma / Wagi.Length;
        }
    }
}
