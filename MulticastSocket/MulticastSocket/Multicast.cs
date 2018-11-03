using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MulticastSocket
{
    class Multicast
    {
        private Socket listenerSocket;
        private Socket senderSocket;
        private Thread listenerThread;
        private BackgroundWorker backgroundWorker;
        private int loopbackPort = 5000;
        private int senderPort = 5000;

        public static int SOCKET_TIMEOUT = 10000;
        public static int RX_BUFFER_LENGTH = 64;
        public bool received;

        public Multicast(bool backgroundWorkerOn, bool listenerThreadOn)
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
            Multicast multicast = (Multicast)value;

            multicast.OpenSocket(out multicast.listenerSocket, multicast.loopbackPort);

            while (true)
            {
                try
                {
                    if (multicast.listenerSocket.Poll(SOCKET_TIMEOUT, SelectMode.SelectRead))
                    {
                        byte[] buffer = new byte[RX_BUFFER_LENGTH];
                        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                        int bytesRead = multicast.listenerSocket.ReceiveFrom(buffer, ref endPoint);
                        if (bytesRead > 0)
                        {
                            multicast.received = true;
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
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                }
            }
        }

        #endregion ListenerThread
        
        private void OpenSocket(out Socket socket, int port)
        {
            socket = null;

            try
            {
                socket.ExclusiveAddressUse = false;
                IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);

                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                socket.ExclusiveAddressUse = false;

                socket.Bind(localEp);
                
                UdpClient mClient = new UdpClient(port);
                IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
                mClient.JoinMulticastGroup(multicastaddress);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
        }
    }
}
