# Answer && Time 🐱

## Version 0 (Base Code)

#### Best's runtime

- Summation result: 888701676
- Time used: 22952 ms

#### Mo's Runtime

- Summation result: 888701676
- Time used: 22236ms

## Version 1
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
- Summation result: 888701676
- Time used: 20887ms

## Version 2 (Wrong Answer, Less time)
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
