using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
namespace Swoopie
{
    static class Core
    {
        #region Declatarion Stuff
        public static WebClient client = null;
        private static int threadCount = 0;
        private static Dictionary<double, Thread> threadPool = new Dictionary<double, Thread>();
        private static ManualResetEvent syncEvent = new ManualResetEvent(false);
        #endregion

        public static Thread RunThread(Action methodName)
        {
            ManualResetEvent syncEvent = new ManualResetEvent(false);
            double unixMilli = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            Thread newThread = new Thread(
        () =>
        {
            syncEvent.Set();
            RefreshThreadLabel();
            methodName();
            syncEvent.WaitOne();
            threadPool.Remove(unixMilli);
            RefreshThreadLabel();
        }

    );
            while (threadPool.ContainsKey(unixMilli))
            {
                Thread.Sleep(5);
                unixMilli = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }


            threadPool.Add(unixMilli, newThread);
            newThread.Start();
            return newThread;
        }
        public static void CloseThreads()
        {
            foreach (double key in threadPool.Keys)
            {
                threadPool[key].Abort();
            }
        }
        private static void RefreshThreadLabel()
        {
            //Invoker.SetLabelText(main.ThreadLabel, "Threads running: " + threadPool.Count.ToString());
        }
        public static int ThreadCount()
        {
            return threadCount;
        }
        private static void InitClient()
        {
            if (client == null)
            {
                client = new WebClient()
                {
                    Proxy = null
                };
            }
        }
        public static string GetGenericType<T>(Dictionary<int, T> list)
        {
            Type type = list.GetType().GetProperty("Item").PropertyType;
            string typeName = type.Name.ToLower();
            return typeName;
        }

        public static void Init()
        {
            InitClient();
        }
        public static void MainInit()
        {

        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
        private static IntPtr GetWindowByTitle(string title)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle == title)
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd;
        }
        public static Rectangle GetGameRect()
        {
            Rectangle rect = new Rectangle();
            GetWindowRect(GetWindowByTitle("EscapeFromTarkov"), out rect);
            return rect;
        }
    }
}
