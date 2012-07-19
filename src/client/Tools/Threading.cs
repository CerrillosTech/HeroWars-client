using System.Collections.Generic;
using System.Threading;

namespace HeroWars.Tools
{
    public static class Threading
    {
        private static readonly List<ManualResetEvent> ThreadEvents = new List<ManualResetEvent>();

        public delegate void ThreadFunction(int threadID, object parameter);
        public delegate void ThreadEvent();

        public static int NumThreads
        {
            get
            {
                lock (ThreadEvents)
                {
                    return ThreadEvents.Count;
                }
            }
        }


        /// <summary>
        /// Führt eine Funktion in einem eigenen Thread aus und übergibt einen eigenen, optionalen Parameter
        /// </summary>
        /// <param name="function">Die Funktion die im eigenen Thread laufen soll</param>
        /// <param name="args">(Optional) Der zu übergebene Parameter.</param>
        public static void RunAsThread(ThreadFunction function, object args = null)
        {
            ThreadEvents.Add(new ManualResetEvent(false));

            int id = ThreadEvents.Count - 1;

            object[] pArgs = new object[3];
            pArgs[0] = function;
            pArgs[1] = args;
            pArgs[2] = id;

            ThreadPool.QueueUserWorkItem(RunThreadFunc, pArgs);
        }

        private static void RunThreadFunc(object o)
        {
            object[] args = (object[])o;
            lock (ThreadEvents)
                ThreadEvents[(int)args[2]].Reset();

            ((ThreadFunction)args[0])((int)args[2], args[1]);

            lock (ThreadEvents)
                ThreadEvents[(int)args[2]].Set();
        }


        /// <summary>
        /// Wartet asyncron auf alle noch laufenden Threads.
        /// 
        /// </summary>
        public static void WaitForThreadsAsnc()
        {
            ThreadPool.QueueUserWorkItem(WaitHelper);
        }

        /// <summary>
        /// Wartet asyncron auf alle noch laufenden Threads und führt <see cref="beforeWaitingHandler"/> vor dem warten
        /// und <see cref="threadsFinishedHandler"/> nach dem Warteprozess aus
        /// </summary>
        /// <param name="beforeWaitingHandler">Funktion die vor dem Warten ausgeführt wird (z.B. um die Oberfläche zu sperren)</param>
        /// <param name="threadsFinishedHandler">Funktion die nach dem Warten ausgeführt wird (z.B. um die Oberfläche wieder freizugeben)</param>
        public static void WaitForThreadsAsnc(ThreadEvent beforeWaitingHandler, ThreadEvent threadsFinishedHandler)
        {
            beforeWaitingHandler.Invoke();
            ThreadPool.QueueUserWorkItem(WaitHelper, threadsFinishedHandler);
        }

        /// <summary>
        /// Wartet syncron auf alle noch laufenden Threads
        /// 
        /// Nicht innerhalb eines UI-Threads aufrufen, da sonst die Oberfläche einfrieren könnte!
        /// </summary>
        public static void WaitForThreads()
        {
            WaitHandle.WaitAll(ThreadEvents.ToArray());
            ThreadEvents.Clear();
        }

        private static void WaitHelper(object obj)
        {
            WaitHandle.WaitAll(ThreadEvents.ToArray());
            ThreadEvents.Clear();
            if (obj != null)
                ((ThreadEvent)obj)();
        }
    }
}

