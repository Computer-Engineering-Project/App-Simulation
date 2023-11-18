using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
namespace Environment.Base
{
    public static class Helper
    {
        public static byte[] createStringActiveHardware()
        {
            return new byte[0];
        }
        public static string getConfigFromHardware(SerialPort serialPort)
        {
            return "";
        }
        public static string decodeMessage(byte[] input)
        {
            return "";
        }
        public static string generatePreamble(int length)
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=<>? ";
            Random random = new Random();

            char[] preambleArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                preambleArray[i] = characters[random.Next(characters.Length)];
            }

            return new string(preambleArray);
        }
        public static double caculateDelayTime(string airRate, string dataLength)
        {
            return Double.Parse(airRate) / Double.Parse(dataLength);
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
        public static double computePathLoss(double src_x, double src_y, double des_x, double des_y)
        {
            var distance = Math.Sqrt(Math.Pow(des_x - src_x, 2) + Math.Pow(des_y - src_y, 2));
            // parameters taken from paper "Do LoRa Low-Power Wide-Area Networks Scale?"
            double d0 = 40;
            double PL_d0_db = 127.41;
            double gamma = 2.08;
            double sigma = 3.57;
            double PL_db = PL_d0_db + 10 * gamma * Math.Log10(distance / d0) + Normal(0.0, sigma);
            return Math.Pow(10, -PL_db / 10);
        }
        private static double Normal(double mean, double stdDev)
        {
            // Create a normal distribution with mean and standard deviation
            Normal normalDistribution = new Normal(mean, stdDev);

            // Generate a random sample from the normal distribution
            return normalDistribution.Sample();
        }
    }
}
