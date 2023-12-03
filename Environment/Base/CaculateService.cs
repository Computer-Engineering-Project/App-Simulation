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
            return data.Length * 8 / (Double.Parse(airRate)*1000);
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
        public static double computePathLoss(double distance, double frequency, double gainTx, double gainRx)
        {
            double constValue = 33.45;
            double pthLoss = constValue + 20 * Math.Log10(frequency) + 20 * Math.Log10(distance) - gainTx - gainRx;
            return pthLoss;
        }
        public static double computeRSSI(ModuleObject sender, ModuleObject receiver)
        {
            double rssi = 0;
            if(sender.type == ModuleObjectType.LORA && receiver.type == ModuleObjectType.LORA)
            {
                LoraParameterObject senderParameter = (LoraParameterObject)sender.parameters;
                LoraParameterObject receiverParameter = (LoraParameterObject)receiver.parameters;
                double distance = computeDistance2Device(sender, receiver);
                double frequency = 410 + Double.Parse(senderParameter.Channel, NumberStyles.HexNumber);
                double gainTx = Double.Parse(senderParameter.AntennaGain);
                double gainRx = Double.Parse(receiverParameter.AntennaGain);
                double pathLoss = computePathLoss(distance, frequency, gainTx, gainRx);
                double noise = 0;

                rssi = Double.Parse(senderParameter.Power) - pathLoss - noise;
                return rssi;
            }
            else if (sender.type == ModuleObjectType.ZIGBEE && receiver.type == ModuleObjectType.ZIGBEE)
            {
/*                ZigbeeParameterObject senderParameter = (ZigbeeParameterObject)sender.parameters;
                ZigbeeParameterObject receiverParameter = (ZigbeeParameterObject)receiver.parameters;
                double distance = computeDistance2Device(sender, receiver);
                double frequency = Double.Parse(senderParameter.Frequency);
                double gainTx = Double.Parse(senderParameter.AntennaGain);
                double gainRx = Double.Parse(receiverParameter.AntennaGain);
                double pathLoss = computePathLoss(distance, frequency, gainTx, gainRx);
                double noise = 0;

                double rssi = Double.Parse(senderParameter.Power) - pathLoss - noise;
*/
                return rssi;
            }
            else
            {
                return rssi;
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
