using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.Packet
{
    public class PacketTransmit
    {
        public string module;
        public string cmdWord;
        public string dataLength;
        public string data;
    }
    public class PacketTransferToView
    {
        public string portName { get; set; }
        public PacketTransmit packet { get; set; }
    }
    public class InternalPacket
    {
        public PacketTransmit packet { get; set;}
        public double DelayTime { get; set;}
        public string? PreambleCode { get; set;}
    }
}
