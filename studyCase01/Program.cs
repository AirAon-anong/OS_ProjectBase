using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Problem01
{
    class Program
    {
        static byte[] Data_Global = new byte[1000000000];
        static long Sum_Global = 0;
        // static int G_index = 0;

        static int checkCount = 0;
        static int processedCounter = 0;
        static int workingCounter = 0;
        const int workingLimit = 10;
        const int THREADINDEX_LIMIT = 10000000;
        static int currentIndex = 0;

        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new();

            try
            {
                Data_Global = (byte[])bf.Deserialize(fs);
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Read Failed:" + se.Message);
                returnData = 1;
            }
            finally
            {
                fs.Close();
            }

            return returnData;
        }
        static void Sum(int startIndex, int stopIndex)
        {
            checkCount++;

            while(startIndex < stopIndex)
            {
              if (Data_Global[startIndex] % 2 == 0)
              {
                  Sum_Global -= Data_Global[startIndex];
              }
              else if (Data_Global[startIndex] % 3 == 0)
              {
                  Sum_Global += (Data_Global[startIndex] * 2);
              }
              else if (Data_Global[startIndex] % 5 == 0)
              {
                  Sum_Global += (Data_Global[startIndex] / 2);
              }
              else if (Data_Global[startIndex] % 7 == 0)
              {
                  Sum_Global += (Data_Global[startIndex] / 3);
              }
              Data_Global[startIndex] = 0;
              startIndex++;
            }
            processedCounter++;
            workingCounter--;
        }
        static void Main(string[] args)
        {
            Stopwatch sw = new();
            int y;

            /* Read data from file */
            Console.Write("Data read...");
            y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
            }
            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();

            while(currentIndex < 1000000000)
            {
                //wait for free limit...
                while (workingCounter >= workingLimit)
                {
                    Thread.Sleep(1);
                }
                workingCounter += 1;
                Thread th = new Thread(() => Sum( currentIndex , currentIndex + THREADINDEX_LIMIT ));
                th.Start();
                Thread.Sleep(1);
                currentIndex += THREADINDEX_LIMIT;
            }
            while (processedCounter < checkCount)
            {
                Thread.Sleep(1);
            }

            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            Console.WriteLine("Summation result: {0}", Sum_Global);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
