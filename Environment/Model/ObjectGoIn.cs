using Environment.Model.Packet;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model
{
    public class ObjectGoIn
    {
        public SerialPort serialport { get; set; }
        public object lockObject { get; set; }
        public Thread ListenHardwareThread { get; set; }
        public Queue<PacketTransmit> packetQueue = new Queue<PacketTransmit>(); 
    }
}
