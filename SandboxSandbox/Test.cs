using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SandboxSandbox
{
    public class Test
    {
        private readonly Random _rand = new Random();
        private readonly int[] _checks;
        private readonly Mutex[] _mutexes;
        private readonly Mutex publicMutex = new Mutex();

        public Test(int indexes)
        {
            _checks = new int[indexes];
            _mutexes = new Mutex[indexes];
            for (int i = 0; i < indexes; i++)
            {
                _checks[i] = 0;
                _mutexes[i] = new Mutex();
            }
        }

        public void Start()
        {
            Parallel.For(0, _checks.Length, OccupyChecks);
        }

        private void OccupyChecks(int index)
        {
            while (true)
            {
                Thread.Sleep(_rand.Next(100, 2001));
                TakeControl(index);
                Console.WriteLine(index + " has taken control.");

                if (_checks[index] != 0)
                {
                    throw new Exception();
                }

                Thread.Sleep(_rand.Next(100, 2001));

                if (_checks[index] != 0)
                {
                    throw new Exception();
                }

                ReleaseControl(index);
                Console.WriteLine(index + " has released control.");
            }
        }

        private void TakeControl(int index)
        {
            int before = index == 0 ? _checks.Length-1 : index - 1;
            int after = (index + 1) % _checks.Length;
            while (true)
            {
                publicMutex.WaitOne();
                if (_checks[index] != 0)
                {
                    publicMutex.ReleaseMutex();
                    _mutexes[before].WaitOne();
                    _mutexes[before].ReleaseMutex();

                    _mutexes[after].WaitOne();
                    _mutexes[after].ReleaseMutex();
                }
                else
                {
                    _checks[before]++;
                    _checks[after]++;
                    _mutexes[index].WaitOne();
                    publicMutex.ReleaseMutex();
                    break;
                }
            }
        }

        private void ReleaseControl(int index)
        {
            int before = index == 0 ? _checks.Length - 1 : index - 1;
            int after = (index + 1) % _checks.Length;
            _checks[before]--;
            _checks[after]--;
            _mutexes[index].ReleaseMutex();
        }
    }
}
