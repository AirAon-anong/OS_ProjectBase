# OS_ProjectBase

## Answer && Time 🐱

### Version 0 (Base Code)

#### Mo's Runtime

- Summation result: 888701676
- Time used: 22236ms

### Version 1
4 thread and Reduce Function Calls

สมมติฐาน : แบ่ง Thread แล้วให้ Thread แยกกันคำนวนในแต่ละช่วงข้อมูลที่มอบหมายให้มีผลให้ลดเวลาในการคำนวน

ผลที่ได้ : ลดเวลาได้แต่ว่าทำงานไม่ถูกต้อง

```
static void Sum(int startIndex, int stopIndex)
{
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
}
```

```
Thread th1 = new Thread(() => Sum(0, 250000000));
Thread th2 = new Thread(() => Sum(250000000, 500000000));
Thread th3 = new Thread(() => Sum(500000000, 750000000));
Thread th4 = new Thread(() => Sum(750000000, 1000000000));

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
```

#### Mo's Runtime
- Summation result: 222799230
- Time used: 10511ms

### Version 2 (Wrong Answer, Less time)
Dynamic Threading

สมมติฐาน : การสร้าง Thread ใหม่ขึ้นมาคำนวนงานเรื่อยๆตามงานที่เหลืออยู่จะช่วยลดเวลาที่ใช้คำนวนได้

ผลที่ได้คือ : ลดเวลาได้แต่ว่าทำงานไม่ถูกต้อง

```
static int checkCount = 0;
static int processedCounter = 0;
static int workingCounter = 0;
const int workingLimit = 10;
const int THREADINDEX_LIMIT = 10000000;
static int currentIndex = 0;

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
```

```
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
```

#### Mo's Runtime
- Summation result: 95658994
- Time used: 9504ms

### Version 3 (Right Answer, Less time)

update from version 1, use the local variables and add the static variable only for the last statement.

สมมติฐาน : การเปลี่ยนให้ algorithm ให้ thread แต่ละ thread บวกลบกับค่า local variable ไว้ก่อนเมื่อบวกลบครบทุก index ที่ได้รับมอบหมายแล้วนำค่าไปบวก static variable ทีเดียว จะสามารถลดเวลาในการทำงานได้

ผลลัพธ์ : ลดเวลาได้ และ ทำงานถูกต้อง

```
static void Sum(int startIndex, int stopIndex)
{
    int result = 0;

    while(startIndex < stopIndex)
    {
      if (Data_Global[startIndex] % 2 == 0)
      {
          result -= Data_Global[startIndex];

      }
      else if (Data_Global[startIndex] % 3 == 0)
      {
          result += (Data_Global[startIndex] * 2);

      }
      else if (Data_Global[startIndex] % 5 == 0)
      {
          result += (Data_Global[startIndex] / 2);

      }
      else if (Data_Global[startIndex] % 7 == 0)
      {
          result += (Data_Global[startIndex] / 3);
      }
      Data_Global[startIndex] = 0;
      startIndex++;


    }
    Sum_Global += result;
}
```

```
Thread th1 = new Thread(() => Sum(0, 250000000));
Thread th2 = new Thread(() => Sum(250000000, 500000000));
Thread th3 = new Thread(() => Sum(500000000, 750000000));
Thread th4 = new Thread(() => Sum(750000000, 1000000000));

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
```

#### Mo's Runtime

- Summation result: 888701676
- Time used: 5870ms

### Version 4

ลองใช้ lock statement มีผลให้เมื่อ thread นึงที่กำลังใช้งาน code ส่วนที่อยู่ใน lock thread อื่นๆนอกเหนือจากนั้นจะถูก block ต้องรอจนกว่า thread ที่ใช้งานอยู่ release lock หรือทำงานเสร็จนั้นเอง

โดย version นี้ใช้ G_index ในการทำซึ่งมีความเสี่ยงต่อการจะเกิด IndexOutOfRangeException

สมมติฐาน : การใช้ lock statement ช่วยให้ลดเวลาในการทำงานและทำให้ผลลัพธ์ถูกต้อง

ผลลัพธ์ : ผลลัพธ์ถูกต้อง ใช้เวลามากขึ้น

```
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

```

#### Mo's Runtime

- Summation result: 888701676
- Time used: 42736ms

### Version 5

แก้ไขของ version 4 โดยใช้วิธีจาก version 2 แยก index ให้แต่ละ thread ทำเลย


```
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
        // static int G_index = 0;

        public long SumGlobal // Encapsulation
        {
            get { return Sum_Global; }
            set { Sum_Global = value; }
        }

        public void Sum(int startIndex, int stopIndex, byte[] Data_Global)
        {
            while(startIndex < stopIndex)
            {
                lock(myLocker)
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

            Thread th1 = new Thread(() => sumc.Sum(0, 250000000,Data_Global));
            Thread th2 = new Thread(() => sumc.Sum(250000000, 500000000,Data_Global));
            Thread th3 = new Thread(() => sumc.Sum(500000000, 750000000,Data_Global));
            Thread th4 = new Thread(() => sumc.Sum(750000000, 1000000000,Data_Global));

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

```
#### Mo's Runtime
- Summation result: 888701676
- Time used: 41659ms

### Version 6 (Right Answer, Less time)
Dynamic Threading local variable

สมมติฐาน : ใช้สมมติฐานจาก version 2 และ 3 มารวมกัน เนื่องจาก version 2 ช่วยให้ทำงานเร็วขึ้นและ version 3 ทำให้ผลถูกต้อง ถ้าเอาสอง version นี้มารวมกันอาจจะทำงานได้เร็วขึ้น

ผลที่ได้คือ : ลดเวลาได้ และทำงานถูกต้อง

```
static int checkCount = 0;
static int processedCounter = 0;
static int workingCounter = 0;
const int workingLimit = 10;
const int THREADINDEX_LIMIT = 10000000;
static int currentIndex = 0;

static void Sum(int startIndex, int stopIndex)
{
    checkCount++;
    int result = 0;
    while(startIndex < stopIndex)
    {
      if (Data_Global[startIndex] % 2 == 0)
      {
          result -= Data_Global[startIndex];
      }
      else if (Data_Global[startIndex] % 3 == 0)
      {
          result += (Data_Global[startIndex] * 2);
      }
      else if (Data_Global[startIndex] % 5 == 0)
      {
          result += (Data_Global[startIndex] / 2);
      }
      else if (Data_Global[startIndex] % 7 == 0)
      {
          result += (Data_Global[startIndex] / 3);
      }
      Data_Global[startIndex] = 0;
      startIndex++;
    }
    processedCounter++;
    workingCounter--;
    Sum_Global += result;
}
```

```
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
```

#### Mo's Runtime
- Summation result: 888701676
- Time used: 4784ms
