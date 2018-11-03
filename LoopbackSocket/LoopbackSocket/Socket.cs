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
        private Socket listenerSocket;
        private Socket senderSocket;
        private Thread listenerThread;
        public BackgroundWorker backgroundWorker;
        private int loopbackPort = 5000;
        private int senderPort = 5000;

        public static int SOCKET_TIMEOUT = 10000;
        public static int RX_BUFFER_LENGTH = 64;
        public bool received;

        public Loopback(bool backgroundWorkerOn, bool listenerThreadOn)
        {
            this.listenerSocket = null;
            this.OpenSocket(out this.senderSocket, this.senderPort);

            if (backgroundWorkerOn)
            {
                try
                {
                    backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
                    backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorkerProgress);
                    backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerCompleted);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error Setting Up Background Worker " + e);
                }
            }

            if (listenerThreadOn)
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

        #region BackgroundWorker

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            byte[] buffer = new byte[RX_BUFFER_LENGTH];
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            int bytesRead = this.listenerSocket.ReceiveFrom(buffer, ref endPoint);
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

        private void BackgroundWorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Reporting Progress!");
        }

        private void BackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Received!");
        }

        #endregion BackgroundWorker


        #region ListenerThread

        private static void SocketListner(object value)
        {
            Loopback loopback = (Loopback)value;

            loopback.OpenSocket(out loopback.listenerSocket, loopback.loopbackPort);

            while(true)
            {
                try
                {
                    if (loopback.listenerSocket.Poll(SOCKET_TIMEOUT, SelectMode.SelectRead))
                    {
                        byte[] buffer = new byte[RX_BUFFER_LENGTH];
                        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                        int bytesRead = loopback.listenerSocket.ReceiveFrom(buffer, ref endPoint);
                        if (bytesRead > 0)
                        {
                            loopback.received = true;
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

        #endregion ListenerThread

        public void Sender(string message, int port)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                int length = buffer.Length;
                IPAddress destinationAddress = IPAddress.Parse("127.0.0.1");
                if(this.senderSocket.SendTo(buffer, new IPEndPoint(destinationAddress.Address, port)) != 0)
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

        private void OpenSocket(out Socket socket, int port)
        {
            socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Ttl = 4;
                socket.ExclusiveAddressUse = false;
                
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // bind to port
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
            }
            catch(Exception e)
            {
                Console.WriteLine("OpenSocket() Error: " + e);
            }
        }
    }
}