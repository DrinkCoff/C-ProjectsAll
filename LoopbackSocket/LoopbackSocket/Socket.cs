using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LoopbackSocket
{
    public class Loopback
    {
        private Socket socket;
        private Thread listenerThread;
        public BackgroundWorker backgroundWorker;
        private static int port = 5000;

        public static int SOCKET_TIMEOUT = 10000;
        public static int RX_BUFFER_LENGTH = 64;

        public Loopback(bool thredOn)
        {
            this.socket = null;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorkerProgress);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerCompleted);

            if (thredOn)
            {
                try
                {
                    this.listenerThread = new Thread(SocketListner);
                    this.listenerThread.Start(this);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error Opening Thread " + e);
                }
            }
        }

        private void BackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Received!");
        }

        private void BackgroundWorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Reporting Progress!");
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            this.OpenSocket();

            byte[] buffer = new byte[RX_BUFFER_LENGTH];
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            int bytesRead = this.socket.ReceiveFrom(buffer, ref endPoint);
            if (bytesRead > 0)
            {
                if (((IPEndPoint)endPoint).Address.Equals(IPAddress.Parse("127.0.0.1")))
                {
                    Console.WriteLine("Message from local host.");
                }
                else
                {
                    Console.WriteLine("Message from remote host.");
                }
            }
        }

        private static void SocketListner(object value)
        {
            Loopback loopback = (Loopback)value;

            loopback.OpenSocket();

            while(true)
            {
                try
                {
                    if (loopback.socket.Poll(SOCKET_TIMEOUT, SelectMode.SelectRead))
                    {
                        byte[] buffer = new byte[RX_BUFFER_LENGTH];
                        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                        int bytesRead = loopback.socket.ReceiveFrom(buffer, ref endPoint);
                        if (bytesRead > 0)
                        {
                            if (((IPEndPoint)endPoint).Address.Equals(IPAddress.Parse("127.0.0.1")))
                            {
                                Console.WriteLine("Message from local host.");
                            }
                            else
                            {
                                Console.WriteLine("Message from remote host.");
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                }
            }
        }

        public void Sender()
        {
            try
            {
                this.OpenSocket();

                string message = "Test";
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                int length = buffer.Length;
                IPAddress destinationAddress = IPAddress.Parse("127.0.0.1");
                if(this.socket.SendTo(buffer, new IPEndPoint(destinationAddress.Address, port)) != 0)
                {
                    Console.WriteLine("Send Successful.");
                }
                else
                {
                    Console.WriteLine("Send Failed.");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Sender Error: " + e);
            }
        }

        private void OpenSocket()
        {
            try
            {
                if (this.socket == null || !this.socket.IsBound)
                {
                    this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    this.socket.Ttl = 4;
                    this.socket.ExclusiveAddressUse = false;

                    // bind to port
                    this.socket.Bind(new IPEndPoint(IPAddress.Any, port));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("OpenSocket() Error: " + e);
            }
        }
    }
}