// This program uses code hyperlinks available as part of the HyperAddin Visual Studio plug-in.
// It is available from http://www.codeplex.com/hyperAddin 
using System;

class Program
{
    public const int objectArrSize = 10000;
    public static int objectArrIndex = 0;
    public static object[] objectArr = new object[objectArrSize];
    static int Main(string[] args)
    {
        int numSec = 1;
        if (args.Length == 1)
            numSec = int.Parse(args[0]);

        RecSpin(numSec);
        return 0;
    }

    // Spin for 'timeSec' seconds.   We do only 1 second in this
    // method, doing the rest in the helper.   
    static void RecSpin(int timeSec)
    {
        if (timeSec <= 0)
            return;
        --timeSec;
        SpinForASecond();
        RecSpinHelper(timeSec);
    }

    // RecSpinHelper is a clone of RecSpin.   It is repeated 
    // to simulate mutual recursion (more interesting example)
    static void RecSpinHelper(int timeSec)
    {
        if (timeSec <= 0)
            return;
        --timeSec;
        SpinForASecond();
        RecSpin(timeSec);
    }

    // SpingForASecond repeatedly calls DateTime.Now until for
    // 1 second.  It also does some work of its own.
    static void SpinForASecond()
    {
        DateTime start = DateTime.Now;
        for (; ; )
        {
            if ((DateTime.Now - start).TotalSeconds > 1)
                break;

            // Do some work in this routine as well.   
            for (int i = 0; i < 10; i++)
            {
                if(objectArrIndex < objectArrSize)
                {
                    objectArr[objectArrIndex++] = new object();
                }
                else
                {
                    objectArr = new object[objectArrSize];
                }
            }
        }
    }
}

