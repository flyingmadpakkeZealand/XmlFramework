using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SandboxSandbox
{
    class Program
    {
        static Mutex evenMutex = new Mutex();
        static Mutex oddMutex = new Mutex();

        static List<int> _ints = new List<int>();
        static Random _rand = new Random();
        static void Main(string[] args)
        {
            //Test t = new Test(10);
            ////t.Start();
            //List<Task> allTasks = new List<Task>();

            //for (int i = 0; i < 1000; i++)
            //{
            //    allTasks.Add(Task.Run(() => Thread.Sleep(10000)));
            //}

            //foreach (Task task in allTasks)
            //{
            //    Console.WriteLine(task.Status);
            //}

            //object lck = new object();
            //Parallel.For(0, 100, InterestingMutex);

            
            ManualResetEvent mre = new ManualResetEvent(true);
            
            Task t1 = Task.Run(() => PutInController(mre));
            Task t2 = Task.Run(() => TakeOutController(mre));

            Task.WaitAll(t1, t2);
        }

        private static bool syncBool = false;
        static void TakeOutController(ManualResetEvent mre)
        {
            while (true)
            {
                TakeOutOfList();

                lock (_ints)
                {
                    if (_ints.Count < 10 && syncBool)
                    {
                        mre.Set();
                        Console.WriteLine("Opening inputs...\n");
                        syncBool = false;
                    }
                }
            }
        }

        static void PutInController(ManualResetEvent mre)
        {
            while (true)
            {
                PutIntoList();
                lock (_ints)
                {
                    if (_ints.Count > 20)
                    {
                        mre.Reset();
                        Console.WriteLine("Blocking inputs...\n");
                        syncBool = true;
                    }
                }

                mre.WaitOne();
            }
        }

        static void PutIntoList()
        {
            int toAdd = _rand.Next(1001);
            lock (_ints)
            {
                _ints.Add(toAdd);
            }

            Console.WriteLine($"Added: {toAdd}.\n");
            Thread.Sleep(500);
        }

        static void TakeOutOfList()
        {
            int taken = -1;
            lock (_ints)
            {
                if (_ints.Count != 0)
                {
                    taken = _ints[0];
                    _ints.RemoveAt(0);
                }
            }

            Console.WriteLine($"Number taken: {taken}.\n");
            Thread.Sleep(700);
        }

        static void InterestingLock(int count)
        {
            lock (count % 2 == 0 ? "Even" : "Odd")
            {
                Console.WriteLine($"{count} entered");
                Thread.Sleep(1000);
            }
        }

        static void InterestingMutex(int count)
        {
            if (count % 2 == 0)
            {
                evenMutex.WaitOne();
                PrintNumber();
                evenMutex.ReleaseMutex();
            }
            else
            {
                oddMutex.WaitOne();
                PrintNumber();
                oddMutex.ReleaseMutex();
            }

            void PrintNumber()
            {
                Console.WriteLine($"{count} entered");
                Thread.Sleep(1000);
            }
        }
    }
}
