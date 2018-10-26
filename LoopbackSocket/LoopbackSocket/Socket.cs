using System;
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
        private static int port = 5000;

        public static int SOCKET_TIMEOUT = 10000;
        public static int RX_BUFFER_LENGTH = 64;

        public Loopback()
        {
            this.socket = null;

            try
            {
                this.listenerThread = new Thread(SocketListner);
                this.listenerThread.Start(this);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error Opening Thread " + e);
            }
        }

        private static void SocketListner(object value)
        {
            Loopback loopback = (Loopback)value;

            loopback.OpenSocket();

            while(true)
            {
                if(loopback.socket.Poll(SOCKET_TIMEOUT, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[RX_BUFFER_LENGTH];
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    int bytesRead = loopback.socket.ReceiveFrom(buffer, ref endPoint);
                    if(bytesRead > 0)
                    {
                        if(((IPEndPoint)endPoint).Address.Equals(IPAddress.Parse("127.0.0.1")))
                        {
                            Console.WriteLine("Messag from local host.");
                        }
                        else
                        {
                            Console.WriteLine("Messag from remote host.");
                        }
                    }
                }
            }
        }

        public void Sender()
        {
            try
            {
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