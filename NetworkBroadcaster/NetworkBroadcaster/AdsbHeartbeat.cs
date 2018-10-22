using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkBroadcaster
{
    public static class AdsbHeartbeat
    {
        public static List<byte> Create()             
         {
            byte[] msg = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // See p.10.
            msg[0] = 0x00; // Message type "Heartbeat".
            msg[1] = 0x91; // "UAT Initialized".

            System.DateTime nowUTC= DateTime.UtcNow;
            // Seconds since 0000Z.
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(nowUTC.Year, nowUTC.Month, nowUTC.Day));
            
            msg[2] = (byte)(((Int32)timeSpan.TotalSeconds >> 16) << 7);
            msg[2] |= 0x01;
            msg[3] = (byte)(((Int32)timeSpan.TotalSeconds & 0xFF));
            msg[4] = (byte)(((Int32)timeSpan.TotalSeconds & 0xFFFF) >> 8);

            // TODO. Number of uplink messages. See p.12.

            msg[5] = 0x20;
            msg[6] = 0x04;

            List<byte> completeHeartbeatMessage = new List<byte>();

            completeHeartbeatMessage.Add(0x7E);

            completeHeartbeatMessage.AddRange(MessageWithChecksum(msg));

            completeHeartbeatMessage.Add(0x7E);


            return completeHeartbeatMessage;
        }

        public static byte[] MessageWithChecksum(byte[] message)
        {
            byte[] fullMessage = new byte[message.Length + 2];

            return fullMessage;
        }
    }
}
