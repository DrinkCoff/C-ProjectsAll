using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;


namespace LoopbackSocket
{
    public static class Experiemental
    { 
        //This is the console application entry point.
        public static void Main()
        {
            Loopback loopback = new Loopback(false, true);
            int delayInMilliseconds = 10000;
            string message = "Hello";
            int sendPort = 6000;

            loopback.received = true;
            //loopback.backgroundWorker.RunWorkerAsync();
            //Thread.Sleep(delayInMilliseconds);

            while (true)
            {
                if (loopback.received)
                {
                    loopback.Sender(message, sendPort);
                    loopback.received = false;
                }

                Thread.Sleep(delayInMilliseconds);
                //loopback.Sender();
            }
        }
    }
}