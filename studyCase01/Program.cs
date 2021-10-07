using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Problem01
{
    class Summation
    {
        private object myLocker = new object();
        static long Sum_Global = 0;
        static int G_index = 0;

        public long SumGlobal // Encapsulation
        {
            get { return Sum_Global; }
            set { Sum_Global = value; }
        }

        public void Sum(byte[] Data_Global)
        {
            while(G_index < 1000000000)
            {
                lock(myLocker)
                {
                    if (G_index < 1000000000)
                    {
                        if (Data_Global[G_index] % 2 == 0)
                        {
                            if (G_index < 1000000000)
                            {
                              Sum_Global -= Data_Global[G_index];
                            }

                        }
                        else if (Data_Global[G_index] % 3 == 0)
                        {
                            if (G_index < 1000000000)
                            {
                              Sum_Global += (Data_Global[G_index] * 2);
                            }

                        }
                        else if (Data_Global[G_index] % 5 == 0)
                        {
                            if (G_index < 1000000000)
                            {
                              Sum_Global += (Data_Global[G_index] / 2);
                            }

                        }
                        else if (Data_Global[G_index] % 7 == 0)
                        {
                            if (G_index < 1000000000)
                            {
                              Sum_Global += (Data_Global[G_index] / 3);
                            }

                        }
                        Data_Global[G_index] = 0;
                        G_index++;
                    }

                }
            }
        }
    }

    class Program
    {
        static byte[] Data_Global = new byte[1000000000];

        // static int G_index = 0;


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

        static void Main(string[] args)
        {
            Stopwatch sw = new();
            Summation sumc = new();

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

            Thread th1 = new Thread(() => sumc.Sum(Data_Global));
            Thread th2 = new Thread(() => sumc.Sum(Data_Global));
            Thread th3 = new Thread(() => sumc.Sum(Data_Global));
            Thread th4 = new Thread(() => sumc.Sum(Data_Global));

            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();

            th1.Start();
            th2.Start();
            th3.Start();
            th4.Start();

            th1.Join();
            th2.Join();
            th3.Join();
            th4.Join();

            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            Console.WriteLine("Summation result: {0}", sumc.SumGlobal);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
