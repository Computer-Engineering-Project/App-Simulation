using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    }
}
