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
        public static double caculateDelayTime(string airRate, string data, string preamble, string FEC)
        {
            double BW = 0;
            double SF = 0;
            double CR = 1;
            double n_preamble = preamble.Length;
            double PL = Encoding.ASCII.GetBytes(data).Length;
            double CRC = FEC == "0" ? 0 : 1;
            double IH = 1;
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
            return Tpacket;
        }
        public static double computeRange(string antenaGain, string transmissionPower)
        {
            double maxTransmitPower = 20;
            double productAntenaGain = 5;
            double productMaxRange = 3000;
            var val1 = (Double.Parse(transmissionPower) + Double.Parse(antenaGain) - maxTransmitPower - productAntenaGain + 20 * Math.Log10(productMaxRange)) / 20;
            return Math.Pow(10, val1) / 10;
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
            if (listParams != null)
            {
                double constValue = 32.45;
                double frequency = 410 + Double.Parse(listParams["Channel"], NumberStyles.HexNumber);
                double pthLoss = constValue + 20 * Math.Log10(distance) + 20 * Math.Log10(frequency);
                return pthLoss;
            }
            return Double.NaN;
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
