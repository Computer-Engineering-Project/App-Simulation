using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.Module
{
    public class FixedMode
    {
        public const string FIXED = "1";
        public const string BROARDCAST = "0";
    }
    public class IOMode
    {
        public const string INPUT = "0";
        public const string OUTPUT = "1";
    }

    public class SpeedRate
    {
        public const string RATE_300 = "300";
        public const string RATE_1200 = "1200";
        public const string RATE_2400 = "2400";
        public const string RATE_4800 = "4800";
        public const string RATE_9600 = "9600";
        public const string RATE_19200 = "19200";
        public const string RATE_38400 = "38400";
        public const string RATE_57600 = "57600";
        public const string RATE_115200 = "115200";
    }

    public class WorTime
    {
        public const string TIME_250 = "250";
        public const string TIME_500 = "500";
        public const string TIME_750 = "750";
        public const string TIME_1000 = "1000";
        public const string TIME_1250 = "1250";
        public const string TIME_1500 = "1500";
        public const string TIME_1750 = "1750";
        public const string TIME_2000 = "2000";
    }

    public class LoraParameterObject
    {
        public string Id { get; set; }
        public string UartRate { get; set; }
        public string Parity { get; set; }
        public string AirRate { get; set; }
        public string Channel { get; set; }
        public string Address { get; set; }
        public string Power { get; set; }
        public string FEC { get; set; }
        public string FixedMode { get; set; }
        public string WORTime { get; set; }
        public string IOMode { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationChannel { get; set; }
    }
}
