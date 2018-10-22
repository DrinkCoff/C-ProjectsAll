using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkBroadcaster
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte> msg = AdsbHeartbeat.Create();

            int PORT = 4000;
            UdpClient udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
            
            //var from = new IPEndPoint(0, 0);
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        var recvBuffer = udpClient.Receive(ref from);
            //        Console.WriteLine(Encoding.UTF8.GetString(recvBuffer));
            //    }
            //});

            var data = Encoding.UTF8.GetBytes("VIDHI");
            udpClient.Send(data, data.Length, "255.255.255.255", PORT);
        }


    }
}
