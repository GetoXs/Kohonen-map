using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Kohonen
{
    class KohonenMap
    {

        public static int SIZE_OF_NEIGHBORHOOD;// = 5;
        public static int NO_OF_EPOCHS;// = 50;
        public static int NO_OF_CLUSTERS;// = 16;
        public static int X_DIM;
        public static int Y_DIM;
        public static int RGB = 3;
        public static int[,] INPUT_FILE;// = new int[X_DIM * Y_DIM,RGB];
        public static double[,] K_MAP;// = new double[NO_OF_CLUSTERS,RGB];
        public static int lowerLimit = 255;
        public static int upperLimit = 0;
        public static double learning_rate;// = 0.7;
        public static int[] CLUSTER;// = new int[NO_OF_CLUSTERS];
        public static bool SQUARED = true;
        public static Bitmap bitmapa;
        public static Image outputImage = null;
        private static Form1 form;


        public static void convert(Form1 form, Image arg, int Neighbourhood, int epochs, int clusters, double learning_rate)
        {
          KohonenMap.form = form;
            InitializeCompression(arg, Neighbourhood, epochs, clusters, learning_rate);
            InitializeCluster();
            Train();
            FinishedBitmap();
        }

        public static void InitializeCompression(Image arg, int Neighbourhood, int epochs, int clusters, double rate)
        {

            bitmapa = new Bitmap(arg);
            SIZE_OF_NEIGHBORHOOD = Neighbourhood;
            NO_OF_EPOCHS = epochs;
            NO_OF_CLUSTERS = clusters;
            learning_rate = rate;
            Y_DIM = bitmapa.Height;
            X_DIM = bitmapa.Width;
            INPUT_FILE = new int[Y_DIM * X_DIM, RGB];
            //odczyt bitmapy
            int k = 0;
            for (int i = 0; i < Y_DIM; i++)
            {
                for (int j = 0; j < X_DIM; j++)
                {
                    INPUT_FILE[k, 0] = bitmapa.GetPixel(j, i).R;
                    INPUT_FILE[k, 1] = bitmapa.GetPixel(j, i).G;
                    INPUT_FILE[k, 2] = bitmapa.GetPixel(j, i).B;
                    k++;
                }
            }

            K_MAP = new double[NO_OF_CLUSTERS, RGB];
            CLUSTER = new int[NO_OF_CLUSTERS];

        }

        public static void InitializeCluster()
        {
            Random rm = new Random();
            for (int i = 0; i < NO_OF_CLUSTERS; i++)
                for (int j = 0; j < RGB; j++)
                {
                    //K_MAP[i, j] = lowerLimit + Math.Abs(rm.Next(upperLimit + lowerLimit)) % (upperLimit - lowerLimit);
                  K_MAP[i, j] = rm.Next(upperLimit, lowerLimit);
                }
        }

        public static void Train()
        {
            for (int epochs = 0; epochs < NO_OF_EPOCHS; epochs++)
            {
              form.changeProgressBar(epochs+1);
                for (int i = 0; i < X_DIM * Y_DIM; i++)
                {
                    int min = int.MaxValue;
                    int WinningIndex = 0;
                    for (int k = 0; k < NO_OF_CLUSTERS; k++)
                    {
                        int temp = 0;
                        // obliczenie odległości euklidesowej
                        for (int j = 0; j < RGB; j++)
                        {
                            temp += (INPUT_FILE[i, j] - (int)K_MAP[k, j]) * (INPUT_FILE[i, j] - (int)K_MAP[k, j]); //na razie niech będzie
                        }
                        // znalezienie zwycięzcy
                        if (temp < min)
                        {
                            min = temp;
                            WinningIndex = k;
                        }
                    }

                    // obliczanie wag w klastrze
                    for (int j = 0; j < RGB; j++)
                    {
                        double temp = (INPUT_FILE[i, j] - K_MAP[WinningIndex, j]);
                        K_MAP[WinningIndex, j] += learning_rate * temp;

                        // obliczanie wag wsrod sasiadow
                        if (SIZE_OF_NEIGHBORHOOD > 0)
                        {
                            for (int index = 1; index < SIZE_OF_NEIGHBORHOOD; index++)
                            {
                                if (WinningIndex - index >= 0)
                                {
                                    K_MAP[(WinningIndex - index), j] += learning_rate * temp;
                                    if (K_MAP[(WinningIndex - index), j] < 0)
                                        K_MAP[(WinningIndex - index), j] = 0;
                                }
                                if (WinningIndex + index < NO_OF_CLUSTERS)
                                {
                                    K_MAP[(WinningIndex + index), j] += learning_rate * temp;
                                    if (K_MAP[(WinningIndex + index), j] < 0)
                                        K_MAP[(WinningIndex + index), j] = 0;
                                }
                            }

                        }
                    }
                }

                if (epochs % 5 == 0)
                    if (SQUARED)
                        learning_rate = Math.Pow(learning_rate, 2.0);
                    else
                        learning_rate = learning_rate / 2.0;
                if (epochs == 1)
                    SIZE_OF_NEIGHBORHOOD = 2;
                if (epochs == 4)
                    SIZE_OF_NEIGHBORHOOD = 1;
                if (epochs == 5)
                    SIZE_OF_NEIGHBORHOOD = 0;
            }
        }

        public static void FinishedBitmap()
        {
            //StreamWriter out_CompressedImage = new StreamWriter("xx.bmp");
          Bitmap bit = new Bitmap(X_DIM, Y_DIM);

            try
            {
                for (int i = 0; i < X_DIM * Y_DIM; i++)
                {
                    int min = 999999999;
                    int WinningIndex = 0;
                    for (int k = 0; k < NO_OF_CLUSTERS; k++)
                    {
                        int temp = 0;
                        // Calcuate the euclidian distance
                        for (int j = 0; j < RGB; j++)
                        {
                            temp += (INPUT_FILE[i, j] - (int)K_MAP[k, j]) * (INPUT_FILE[i, j] - (int)K_MAP[k, j]);
                        }
                        // Find the winning index
                        if (temp < min)
                        {
                            min = temp;
                            WinningIndex = k;
                        }
                    }
                    try
                    {
                        bit.SetPixel(i % X_DIM, (int)(i / X_DIM), Color.FromArgb((int)K_MAP[WinningIndex, 0], (int)K_MAP[WinningIndex, 1], (int)K_MAP[WinningIndex, 2]));

                    }
                    catch (Exception)
                    {

                    }
                }
                outputImage = bit;
                //out_CompressedImage.Close();

            }
            catch (FileNotFoundException) { }
            catch (IOException) { }


        }
    }
}