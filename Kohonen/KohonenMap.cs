using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Kohonen
{
    class KohonenMap {

        public static int SIZE_OF_NEIGHBORHOOD = 5;
        public static int NO_OF_EPOCHS = 50;
        public static int NO_OF_CLUSTERS = 16;
        public static int X_DIM = 512;
        public static int Y_DIM = 512;
        public static int RGB = 3;
        public static int[,] INPUT_FILE = new int[X_DIM * Y_DIM, RGB];
        public static double[,] K_MAP = new double[NO_OF_CLUSTERS, RGB];
        public static int lowerLimit = 100000;
        public static int upperLimit = 0;
        public static double learning_rate = 0.7;
        public static int[] CLUSTER = new int[NO_OF_CLUSTERS];
        public static bool SQUARED = true;
        public static String INPUT_FILENAME = "/home/nischal/Desktop/family_guy_mad.pgm";
        public static String KOHONENMAP_FILENAME = "/home/nischal/Desktop/kohonenMap.knm";
        public static String COMPRESSED_IMAGE_FILENAME = "/home/nischal/Desktop/family_guy_Compressed.pbm";
        public static String EXPANDED_IMAGE_FILENAME = "/home/nischal/Desktop/family_guy_Expanded.txt";

      public static Image outputImage = null;


    public static void convert(Image arg) {
      ReadFile(arg);
      InitializeCluster();
      Train();
      CompressTheImage();
    }

    public static void ReadFile(Image s) {
      //StreamReader stream = new StreamReader(s);
      Bitmap bitmap = new Bitmap(s);
      for(int i = 0; i<bitmap.Height; i++)
      {
        for(int j = 0; j<bitmap.Width; j++)
        {
          INPUT_FILE[bitmap.Width*i + j, 0] = bitmap.GetPixel(i, j).R;
          INPUT_FILE[bitmap.Width*i + j, 1] = bitmap.GetPixel(i, j).G;
          INPUT_FILE[bitmap.Width*i + j, 2] = bitmap.GetPixel(i, j).B;

        }
      }

     //File file = new File(s);
     //FileInputStream fis = null;
     //BufferedInputStream bis = null;
     //DataInputStream dis = null;

     //try {
     //fis = new FileInputStream(file);

     //// Here BufferedInputStream is added for fast reading.
     //bis = new BufferedInputStream(fis);
     //dis = new DataInputStream(bis);
     //dis.readLine();
     //dis.readLine();
     //dis.readLine();
     //dis.readLine();
     //// dis.available() returns 0 if the file does not have more lines.
     //int k = 0;
     //while (dis.available() != 0) {
     //  String line = dis.readLine();
     //  line = line.Trim();

     //  String[] splittedLine = line.Split("[ \\t]+|[ \\t]+$");

     //  for (int i = 0; i < splittedLine.length; i += 3) 
     //  {
     //    for (int j = 0; j < RGB; j++) 
     //    {
     //      if (!splittedLine[i].matches("\\s*")
     //      || !splittedLine[i].contains("")
     //      || !splittedLine[i].isEmpty())
     //        INPUT_FILE[k][j] = Integer.valueOf(splittedLine[i
     //        + j]);
     //      if (INPUT_FILE[k][j] < lowerLimit)
     //       lowerLimit = INPUT_FILE[k][j];
     //      if (INPUT_FILE[k][j] > upperLimit)
     //       upperLimit = INPUT_FILE[k][j];
     //    }
     //  k++;
     //  }
     //}

     //// dispose all the resources after using them.
     //fis.close();
     //bis.close();
     //dis.close();

     //} catch (FileNotFoundException e) {
     //e.printStackTrace();
     //} catch (IOException e) {
     //e.printStackTrace();
     //}
     //System.out.println("Exitting Read File");
     }

    public static void InitializeCluster() {
      Random rm = new Random();
      for (int i = 0; i < NO_OF_CLUSTERS; i++)
        for (int j = 0; j < RGB; j++) 
        {
            K_MAP[i,j] = lowerLimit + Math.Abs(rm.Next(upperLimit+lowerLimit)) % (upperLimit - lowerLimit);
        }
    }

    public static void Train() {
      for (int epochs = 0; epochs < NO_OF_EPOCHS; epochs++) 
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
              temp += (INPUT_FILE[i,j] - (int)K_MAP[k,j]) * (INPUT_FILE[i,j] - (int)K_MAP[k,j]);
            }
            // Find the winning index
            if (temp < min) 
            {
              min = temp;
              WinningIndex = k;
            }
          }

          // Adjust the weights of the winning pixel in the cluster
          for (int j = 0; j < RGB; j++) {
            double temp = (INPUT_FILE[i,j] - K_MAP[WinningIndex,j]);
            K_MAP[WinningIndex,j] += learning_rate * temp;

            // Adjust the weight of the neighbors
            if (SIZE_OF_NEIGHBORHOOD > 0) {
              for (int index = 1; index < SIZE_OF_NEIGHBORHOOD; index++) 
              {
                if (WinningIndex - index >= 0) 
                {
                  K_MAP[(WinningIndex - index),j] += learning_rate
                  * temp;
                  if (K_MAP[(WinningIndex - index),j] < 0)
                  K_MAP[(WinningIndex - index), j] = 0;
                }
                if (WinningIndex + index < NO_OF_CLUSTERS) 
                {
                  K_MAP[(WinningIndex + index), j] += learning_rate
                  * temp;
                  if (K_MAP[(WinningIndex + index), j] < 0)
                    K_MAP[(WinningIndex + index), j] = 0;
                }
              }
            }
          }
        }

        if(epochs % 5 == 0)
          if (SQUARED)
            learning_rate = Math.Pow(learning_rate, 2.0);
          else
            learning_rate = learning_rate / 2.0;
        if(epochs == 1)
          SIZE_OF_NEIGHBORHOOD = 2 ;
        if(epochs == 4)
          SIZE_OF_NEIGHBORHOOD = 1;
        if(epochs == 5)
          SIZE_OF_NEIGHBORHOOD = 0;
      }
    }


    public static void CompressTheImage() {

      //zapisanie wyników do bitmapy

     //FileWriter fstream_CompressedImage = new FileWriter(EXPANDED_IMAGE_FILENAME);
     //BufferedWriter out_CompressedImage = new BufferedWriter(fstream_CompressedImage);
      
      StreamWriter out_CompressedImage = new StreamWriter("xx.bmp");
      Bitmap bit = new Bitmap(X_DIM, Y_DIM);
      
      
      

      try {

      System.Console.Out.Write("P3 \n # CREATOR: XV version 3.10a-jumboFix+Enh of 20070520\n  450 600\n 255\n");
      //out.write("P3 \n # CREATOR: XV version 3.10a-jumboFix+Enh of 20070520\n  450 600\n 255\n");

      for (int i = 0; i < X_DIM * Y_DIM; i++) 
      {
        int min = 999999999;
        int WinningIndex = 0;
        for (int k = 0; k < NO_OF_CLUSTERS; k++) {
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
          //out_CompressedImage.Write(WinningIndex.ToString());
          //if(i%X_DIM == 0)
          //  out_CompressedImage.Write('\n');
          //else
          //  out_CompressedImage.Write('\t');
        }
        try {
          // Write the file of new compressed image
          //for (int j = 0; j < RGB; j++) 
          {
            bit.SetPixel((int)(i / X_DIM), i % X_DIM, Color.FromArgb((int)K_MAP[WinningIndex, 0], (int)K_MAP[WinningIndex, 1], (int)K_MAP[WinningIndex, 2]));

            //System.Console.Out.Write(K_MAP[WinningIndex, j].ToString());
            //System.Console.Out.Write("\t");
          }
          //System.Console.Out.Write("\n");
        } catch (Exception) {
          
        }
      }
      outputImage = bit;
      //bit.Save("output.bmp");
      //out.close();
      out_CompressedImage.Close();

      }
      catch(FileNotFoundException e) { }
      catch(IOException e ) {}
      

    }    
}
}
