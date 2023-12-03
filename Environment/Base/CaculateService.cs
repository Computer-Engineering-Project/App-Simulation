using Environment.Model.Module;
using Environment.Model.Packet;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Base
{
    public static class  CaculateService
    {
        public static double caculateDelayTime(string airRate, string data)
        {
            return Double.Parse(airRate) / data.Length;
        }
        public static double computeRange(string transmissionPower)
        {
            double max_sensitivity = -110.225; // by measuring device in reality
            // parameters taken from paper "Do LoRa Low-Power Wide-Area Networks Scale?"
            double d0 = 40;
            double PL_d0_db = 127.41;
            double gamma = 2.08;
            // Caculate distance
            double transmissionPower_dbm = Double.Parse(transmissionPower);
            double rhs = (transmissionPower_dbm - PL_d0_db - max_sensitivity) / (10 * gamma);
            double distance = d0 * Math.Pow(10, rhs);
            return distance;
        }
        public static double computeDistance2Device(ModuleObject sender, ModuleObject receiver)
        {
            double distance = Math.Sqrt(Math.Pow(receiver.x - sender.x, 2) + Math.Pow(receiver.y - sender.y, 2));
            return distance;
        }
        public static double computePathLoss(ModuleObject sender, ModuleObject receiver)
        {
            string json = JsonConvert.SerializeObject(sender.parameters);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var distance = Math.Sqrt(Math.Pow(receiver.x - sender.x, 2) + Math.Pow(receiver.y - sender.y, 2));
            // parameters taken from paper "Do LoRa Low-Power Wide-Area Networks Scale?"
            if(listParams!=null)
            {
                double constValue = 33.45;
                double frequency = 410 + Double.Parse(listParams["Channel"], NumberStyles.HexNumber);
                double pthLoss = constValue + 20 * Math.Log10(distance) + 20 * Math.Log10(frequency);
                return pthLoss;
            }
            return Double.NaN;
        }
        
        private static void HandleCollisionPacket(List<PacketTransmit> packets, PacketTransmit packet)
        {
            foreach (var p in packets)
            {
                if (p != packet)
                {
                    //p.Collision = true;
                }
            }
        }

        public static bool acceptPacket(double percentPacketLoss)
        {
            Random random = new Random();
            double randomValue = random.NextDouble();
            if (randomValue < percentPacketLoss)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
