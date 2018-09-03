using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UatUplinkHeaderParser
{
    class Program
    {
        static double multiplier = 360 / Math.Pow(2, 24);
        public static void Main()
        {
            string uatUplinkHeader = "3bba6982f6d4bc50";

            List<byte> uatUplinkHeaderBytes = HexStringToBytes(uatUplinkHeader);

            double latitude = ParseLatitude(uatUplinkHeaderBytes);
            double longitude = ParseLongitude(uatUplinkHeaderBytes);

            Console.WriteLine("Latitude:  " + latitude.ToString());
            Console.WriteLine("Longitude: " + longitude.ToString());
        }

        static private List<byte> HexStringToBytes(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                             .ToList();
        }

        static double ParseLatitude(List<byte> uatUplinkHeaderBytes)
        {
            bool isNegative = (uatUplinkHeaderBytes[0] & 0x80) > 0;
            long latitudeEquvilent = ( ((uatUplinkHeaderBytes[0] & 0x7F) << 16) | (uatUplinkHeaderBytes[1] << 8) | uatUplinkHeaderBytes[2] ) >> 1;
            
            return (latitudeEquvilent * multiplier);
        }

        static double ParseLongitude(List<byte> uatUplinkHeaderBytes)
        {
            bool isNegative = (uatUplinkHeaderBytes[2] & 0x01) > 0;
            long longitudeEquvilent = ( (uatUplinkHeaderBytes[3] << 16) | (uatUplinkHeaderBytes[4] << 8) | uatUplinkHeaderBytes[5] ) >> 1;

            return (longitudeEquvilent * multiplier * (isNegative ? -1 : 1));
        }
    }
}