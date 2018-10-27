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
            Loopback loopback = new Loopback(false);
            int delayInMilliseconds = 250;

            loopback.backgroundWorker.RunWorkerAsync();
            //Thread.Sleep(delayInMilliseconds);
            loopback.Sender();

            while (true)
            {
                Thread.Sleep(delayInMilliseconds);
                //loopback.Sender();
            }
        }
    }
}