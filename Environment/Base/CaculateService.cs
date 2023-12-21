using Environment.Model.Module;
using Environment.Model.Packet;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Base
{
    public static class CaculateService
    {
        public static double caculateDelayTime(string airRate, string data, string preamble, string FEC, ModuleObject module)
        {
            double BW = 0;
            double SF = 0;
            double CR = 1;
            double n_preamble = preamble.Length;
            double PL = Encoding.ASCII.GetBytes(data).Length;
            double CRC = FEC == "0" ? 0 : 1;
            double IH = 0;
            double DE = 0;

            var airRateNum = Double.Parse(airRate);
            switch (airRateNum)
            {
                case 0.3:
                    BW = 125;
                    SF = 12;
                    DE = 1;
                    break;
                case 1.2:
                    BW = 250;
                    SF = 11;
                    break;
                case 2.4:
                    BW = 500;
                    SF = 11;
                    break;
                case 4.8:
                    BW = 250;
                    SF = 8;
                    break;
                case 9.6:
                    BW = 500;
                    SF = 8;
                    break;
                case 19.2:
                    BW = 500;
                    SF = 7;
                    break;
                default:

                    break;
            }
            var Tsym = Math.Pow(2, SF) / BW;
            var Tpreamble = (n_preamble + 4.25) * Tsym;
            var Tpayload = Tsym * (8 + Math.Max(Math.Ceiling((8 * PL - 4 * SF + 28 + 16 * CRC - 20 * IH) / (4 * (SF - 2 * DE))) * (CR + 4), 0));
            var Tpacket = Tpayload + Tpreamble;
            if (module.type == ModuleObjectType.ZIGBEE)
                return 10;
            return Tpacket;
        }
        public static double computeSNR(string RSSI, double noise)
        {
            return Double.Parse(RSSI) - noise;
        }
        public static double computeRange(string antenaGain, string transmissionPower, double productMaxRange)
        {
            double maxTransmitPower = 20;
            double productAntenaGain = 5;
            var val1 = (Double.Parse(transmissionPower) + Double.Parse(antenaGain) - maxTransmitPower - productAntenaGain + 20 * Math.Log10(productMaxRange)) / 20;
            return Math.Pow(10, val1);
        }
        public static double computeMaxRange(string antenaGain, string transmissionPower)
        {
            double maxTransmitPower = 20;
            double productAntenaGain = 5;
            double productMaxRange = 3300;
            var val1 = (Double.Parse(transmissionPower) + Double.Parse(antenaGain) - maxTransmitPower - productAntenaGain + 20 * Math.Log10(productMaxRange)) / 20;
            return Math.Pow(10, val1) / 5;
        }
        public static double computeDistance2Device(ModuleObject sender, ModuleObject receiver)
        {
            double distance = Math.Sqrt(Math.Pow(receiver.x - sender.x, 2) + Math.Pow(receiver.y - sender.y, 2));
            return distance;
        }
        public static double computePathLoss(double distance, double frequency, double gainTx, double gainRx)
        {
            double constValue = 32.45;
            double pthLoss = constValue + 20 * Math.Log10(frequency) + 20 * Math.Log10(distance) - gainTx - gainRx;
            return pthLoss;
        }
        public static double computeRSSI(ModuleObject sender, ModuleObject receiver)
        {
            double rssi = 0;
            if (sender.type == ModuleObjectType.LORA && receiver.type == ModuleObjectType.LORA)
            {
                LoraParameterObject senderParameter = (LoraParameterObject)sender.parameters;
                LoraParameterObject receiverParameter = (LoraParameterObject)receiver.parameters;
                double distance = computeDistance2Device(sender, receiver);
                double frequency = 410 + int.Parse(senderParameter.Channel, NumberStyles.HexNumber);
                double gainTx = Double.Parse(senderParameter.AntennaGain);
                double gainRx = Double.Parse(receiverParameter.AntennaGain);
                double pathLoss = computePathLoss(distance, frequency, gainTx, gainRx);

                rssi = Double.Parse(senderParameter.Power) - pathLoss;
                return rssi;
            }
            else if (sender.type == ModuleObjectType.ZIGBEE && receiver.type == ModuleObjectType.ZIGBEE)
            {
                ZigbeeParameterObject senderParameter = (ZigbeeParameterObject)sender.parameters;
                ZigbeeParameterObject receiverParameter = (ZigbeeParameterObject)receiver.parameters;
                double distance = computeDistance2Device(sender, receiver);
                double frequency = 2400 + (int.Parse(senderParameter.Channel, NumberStyles.Integer) - 10) * 5;
                double gainTx = Double.Parse(senderParameter.AntennaGain);
                double gainRx = Double.Parse(receiverParameter.AntennaGain);
                double pathLoss = computePathLoss(distance, frequency, gainTx, gainRx);

                rssi = Double.Parse(senderParameter.Power) - pathLoss;
                return rssi;
            }
            else
            {
                return rssi;
            }
        }
        public static double caculatePacketLossProbality(string _distance, double coveringArea, double coveringLoss)
        {
            var distance = Double.Parse(_distance);
            if (distance <= coveringArea)
            {
                return 0;
            }
            if (distance > coveringLoss)
            {
                return 1;
            }
            var a = 100 / (coveringLoss - coveringArea);
            var b = 100 * coveringArea / (coveringArea - coveringLoss);
            return a * distance + b;
        }
        public static bool isPacketLoss(double lossProbality)
        {
            var random = new Random();
            if (lossProbality == 1) return true;
            if (lossProbality == 0) return false;
            if (lossProbality >= random.Next(1, 99)) return true;
            return false;
        }
    }
}
