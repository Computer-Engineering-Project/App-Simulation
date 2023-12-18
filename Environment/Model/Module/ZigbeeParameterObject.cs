using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.Module
{
    public class TransmitMode
    {
        public const string POINT_TO_POINT = "0";
        public const string BROADCAST = "1";
    }
    public class ZigbeeParameterObject
    {
        public readonly string ReceivingSensitivity = "-97";
        public string Id { get; set; }
        public string UartRate { get; set; }
        public string Parity { get; set; }
        public string AirRate { get; set; }
        public string Channel { get; set; }
        public string Address { get; set; }
        public string Power { get; set; }
        public string TransmitMode { get; set; }
        public string AntennaGain { get; set; }
    }

}
