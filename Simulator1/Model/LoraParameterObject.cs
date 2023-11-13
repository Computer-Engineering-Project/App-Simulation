using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Model
{
    public class LoraParameterObject
    {
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
    }
}
