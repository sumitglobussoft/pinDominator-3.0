using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinDominator.Account
{
    public class PInterestAccountManager_Producer
    {
       
            public static Dictionary<string, string> FinalCrawledURLs { get; set; }
            public static Dictionary<string, string> FinalCrawledURLsWP { get; set; }
            public static Dictionary<string, string> FinalCrawledURLsB2E { get; set; }
            public static Dictionary<string, string> FinalCrawledURLsMT { get; set; }
            public static Queue<string> QuePostURLsWP = new Queue<string>();
            public static Queue<string> QuePostURLsB2E = new Queue<string>();
            public static Queue<string> QuePostURLsMT = new Queue<string>();

            public static bool UseProxy { get; set; }
            public static List<string> listWorkingProxiesYahoo { get; set; }
            public static List<string> listWorkingProxiesGoogle { get; set; }

            public static Queue<string[]> _SeedUrlQueue = new Queue<string[]>();

            //readonly object _locker = new object();
            public static readonly object _locker = new object();

            Thread[] _workers;

            public PInterestAccountManager_Producer(int workerCount)
            {

                _workers = new Thread[workerCount];

                // Create and start a separate thread for each worker
                for (int i = 0; i < workerCount; i++)
                {
                    PCQueue pcqObj = new PCQueue();
                    (_workers[i] = new Thread(pcqObj.Consume)).Start();
                }
            }

            public void EnqueueItem(string[] item)
            {
                if (_SeedUrlQueue.Count >= 100000)
                {
                    Thread.Sleep(900000);
                }

                lock (_locker)
                {
                    //Thread.Sleep(2000);

                    _SeedUrlQueue.Enqueue(item);           // We must pulse because we're
                    Monitor.Pulse(_locker);         // changing a blocking condition.

                }
            }

            public void StopConsumerThreads()
            {
                foreach (Thread worker in _workers)
                {
                    try
                    {
                        worker.Abort();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
    }
}
